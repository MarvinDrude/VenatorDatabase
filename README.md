# todo

Very early passion project. Nothing to see here yet.
A columnar analytical one file db inspired by ClickHouse but for simple medium-sized projects all local.

Some rules:
- No AI output (Solely for research and maybe some tests/benchmarks)
- No big external libraries - C# first
- Understand everything

Planning (roughly):

End of 2026:
- Solid core of Storage Engine
- File Engine
- Schema & Catalog

2027
- Solid Query Planer & Executor
- Merge & Compact Engine
- C# first-querying & writing
- Bulk Data
- Actually usable

2028
- Networking Layer (with https://github.com/MarvinDrude/Beskar.Networking)
- Protocol Engine for remote access
- SQL Language Parsing (minimal)

2029
- Auth & Security layer
- Encryption

2030
- Overall improvements and readying Beta

## Short Working Desc
A simple one-file analytical columnar database with all the topics that interest me a lot:
- Memory & CPU Management
- Text & Parsing
- Networking
- Caching
- Files
- Databases
- Analytics

## Future Feature board

- Storage Engine
- Schema & Catalog
- Merge / Compact Engine
- File Engine
- Query Executor
- Query Planer
- Networking Layer
- Protocol Engine
- Auth & Security
- Bulk Data

## File Setup

- Dual Wayfinders (vs )
  - Wayfinder A
  - Wayfinder B

---

- Block 1 - Catalog Root
  - Catalog Header
  - Table Header
    - Block Ids to segments of row groups
    - ...
  - Column Header
    - ...

---

- Free-Block Bitmap
  - Flat array of 64 bit ulongs
    - Bit 0 -> Wayfinder allocated
    - Bit 1 -> Root Catalog allocated
    - Bit 2 -> Bitmap allocated
    - Bit 3, 4, n -> Row groups

---

- Block N - RowGroup #N (1 Column)
   - ColumnChunkHeader
   - Payload
- Block N + 1 - RowGroup #N + 1 (1 Column Next)
  - ColumnChunkHeader
  - Payload
