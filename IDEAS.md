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
