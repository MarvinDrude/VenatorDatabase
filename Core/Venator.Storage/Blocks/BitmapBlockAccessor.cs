
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Venator.Storage.Contracts.Blocks;
using Venator.Storage.Contracts.Ids;

namespace Venator.Storage.Blocks;

public readonly ref struct BitmapBlockAccessor
{
   private readonly Span<byte> _blockSpan;
   private readonly ref BitmapBlockHeader _header;
   private readonly Span<ulong> _words;

   public ref readonly BitmapBlockHeader Header => ref _header;
   public bool IsFull => _header.FreeBlockCount == 0;

   public BitmapBlockAccessor(Span<byte> blockSpan)
   {
      _blockSpan = blockSpan;
      _header = ref Unsafe.As<byte, BitmapBlockHeader>(ref MemoryMarshal.GetReference(blockSpan));

      var bitmapBytes = blockSpan[Unsafe.SizeOf<BitmapBlockHeader>()..];
      _words = MemoryMarshal.Cast<byte, ulong>(bitmapBytes);
   }

   public bool TryAllocateSingle(out BlockId allocatedBlockId)
   {
      if (_header.FreeBlockCount == 0)
      {
         allocatedBlockId = BlockId.Invalid;
         return false;
      }

      var startWord = (int)_header.SearchHintWordIndex;
      var wordCount = _words.Length;

      for (var i = startWord; i < wordCount; i++)
      {
         var word = _words[i];
         if (word == ulong.MaxValue)
            continue;

         var bitIndex = BitOperations.TrailingZeroCount(~word);
         _words[i] = word | (1UL << bitIndex);

         UpdateHeaderAfterAlloc(hintWord: (uint)i, allocatedCount: 1);
         var globalBlockId = _header.FirstTrackedBlockId + (uint)(i * 64 + bitIndex);
         allocatedBlockId = new BlockId(globalBlockId);

         return true;
      }

      allocatedBlockId = BlockId.Invalid;
      return false;
   }

   public bool TryAllocateExtent(uint blockCount, out BlockId startBlockId)
   {
      if (blockCount == 1)
         return TryAllocateSingle(out startBlockId);

      if (_header.FreeBlockCount < blockCount)
      {
         startBlockId = BlockId.Invalid;
         return false;
      }

      var wordCount = _words.Length;
      var contiguousFound = 0;
      var bestStartBit = -1;

      for (var w = (int)_header.SearchHintWordIndex; w < wordCount; w++)
      {
         var word = _words[w];
         if (word == ulong.MaxValue)
         {
            contiguousFound = 0;
            continue;
         }

         for (var b = 0; b < 64; b++)
         {
            if ((word & (1UL << b)) == 0)
            {
               if (contiguousFound == 0)
                  bestStartBit = w * 64 + b;

               contiguousFound++;

               if (contiguousFound == blockCount)
               {
                  SetBitRange(bestStartBit, (int)blockCount, value: true);
                  UpdateHeaderAfterAlloc(hintWord: (uint)(bestStartBit / 64), allocatedCount: blockCount);

                  startBlockId = new BlockId(_header.FirstTrackedBlockId + (uint)bestStartBit);
                  return true;
               }
            }
            else
            {
               contiguousFound = 0;
            }
         }

      }

      startBlockId = BlockId.Invalid;
      return false;
   }

   public void Free(BlockId blockId)
   {
      var relativeId = (uint)blockId.Value - _header.FirstTrackedBlockId;
      var wordIndex = (int)(relativeId / 64);
      var bitIndex = (int)(relativeId % 64);

      _words[wordIndex] &= ~(1UL << bitIndex);

      var hint = Math.Min(_header.SearchHintWordIndex, (uint)wordIndex);
      UpdateHeaderAfterFree(hintWord: hint, freedCount: 1);
   }

   public void FreeExtent(BlockId startBlockId, uint count)
   {
      var relativeId = (uint)startBlockId.Value - _header.FirstTrackedBlockId;
      SetBitRange((int)relativeId, (int)count, value: false);

      var hint = Math.Min(_header.SearchHintWordIndex, relativeId / 64);
      UpdateHeaderAfterFree(hintWord: hint, freedCount: count);
   }

   private void SetBitRange(int startBit, int count, bool value)
   {
      for (var i = 0; i < count; i++)
      {
         var bit = startBit + i;
         var w = bit / 64;
         var b = bit % 64;

         if (value)
            _words[w] |= (1UL << b);
         else
            _words[w] &= ~(1UL << b);
      }
   }

   private void UpdateHeaderAfterAlloc(uint hintWord, uint allocatedCount)
   {
      _header = new BitmapBlockHeader(
         magicNumber: _header.MagicNumber,
         version: _header.Version,
         nextBitmapBlockId: _header.NextBitmapBlockId,
         firstTrackedBlockId: _header.FirstTrackedBlockId,
         totalTrackedBlocks: _header.TotalTrackedBlocks,
         freeBlockCount: _header.FreeBlockCount - allocatedCount,
         searchHintWordIndex: hintWord);
   }

   private void UpdateHeaderAfterFree(uint hintWord, uint freedCount)
   {
      _header = new BitmapBlockHeader(
         magicNumber: _header.MagicNumber,
         version: _header.Version,
         nextBitmapBlockId: _header.NextBitmapBlockId,
         firstTrackedBlockId: _header.FirstTrackedBlockId,
         totalTrackedBlocks: _header.TotalTrackedBlocks,
         freeBlockCount: _header.FreeBlockCount + freedCount,
         searchHintWordIndex: hintWord);
   }
}
