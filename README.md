# todo


## File Setup

- Dual Wayfinders
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
