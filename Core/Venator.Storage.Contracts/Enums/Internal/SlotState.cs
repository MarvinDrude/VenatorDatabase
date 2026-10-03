namespace Venator.Storage.Contracts.Enums.Internal;

public enum SlotState : byte
{
   Empty = 0,
   Loading = 1,
   Loaded = 2,
   Evicting = 3
}
