## Folder Structure

Below is the proposed layout of the database root directory. Each table gets its own subfolder,
containing the binary data, indexing/metadata, and schema definition:

```text
database_root/
├── PageView/
│   ├── data.bin
│   ├── metadata.index
│   └── schema.ven
└── Visitor/
    ├── data.bin
    ├── metadata.index
    └── schema.ven
```

### File Descriptions

* **`data.bin`**: Binary file storing the raw record/column data.
* **`metadata.index`**: Indexing information, record offsets, and metadata for fast lookup.
* **`schema.ven`**: Schema definition file detailing the fields, types, and constraints of the table.

## Metadata.index

```text
[ FILE: metadata.index ]
╔══════════════════════════════════════════════════════════╗
║ GLOBAL HEADER (16 Bytes)                                 ║
╟──────────────────────────────────────────────────────────╢
║ 0x00 │ MagicBytes    : "METADATA" (8 Bytes)              ║ -> just nice to have
║ 0x08 │ Version       : 1 (2 Bytes, ushort)               ║ -> check if version matches
║ 0x0A │ RowGroupCount : M (4 Bytes, int)                  ║ -> how many row groups exist
║ 0x0E │ ColumnCount   : N (2 Bytes, ushort)               ║ -> how many columns does the table have
╠══════════════════════════════════════════════════════════╣
╠══════════════════════════════════════════════════════════╣ --> start of the row groups
║ ROW GROUP 0                                              ║
╟──────────────────────────────────────────────────────────╢
║ Var  │ RowGroupId    : (8 Bytes, long)                   ║ -> usually just incrementing identifier
║ Var  │ RowCount      : (4 Bytes, int)                    ║
╟──────────────────────────────────────────────────────────╢
║ PHYSICAL EXTENTS (Where does this live in .bin?)         ║
╟──────────────────────────────────────────────────────────╢
║ Var  │ ExtentCount   : E (2 Bytes, ushort)               ║ -> How many 4096 byte blocks
║ Var  │ [ Offset (8 Bytes) | BytesUsed (4 Bytes) ] x E    ║
╟──────────────────────────────────────────────────────────╢
║ COLUMN STATS (Min/Max Zone Maps for Data Skipping)       ║
╟──────────────────────────────────────────────────────────╢
║ Var  │ COLUMN 0: [ ID | Type | Nulls | Min | Max ]       ║
║ Var  │ COLUMN 1: [ ID | Type | Nulls | Min | Max ]       ║
║ Var  │ ... (Repeated N times for all columns)            ║
╠══════════════════════════════════════════════════════════╣
║ ROW GROUP 1                                              ║
╟──────────────────────────────────────────────────────────╢
║ ... (Same structure as above)                            ║
╚══════════════════════════════════════════════════════════╝
```
