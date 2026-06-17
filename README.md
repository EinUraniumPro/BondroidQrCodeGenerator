# Bondroid QR-Code Generator

A cross-platform .NET CLI tool that reads a Bondroid-exported CSV file and generates a ready-to-print PDF containing a grid of QR codes - one per table, each with a label showing the table name.

## Features

- Reads table data from a Bondroid-exported CSV file
- Generates high-resolution QR codes (ECC Level H) for each table
- Renders a bold text label ("Tisch Nr.: \<name\>") below each QR code
- Combines all QR codes into a multipage A4 PDF grid (3 columns by default)
- Parallel QR code generation for fast processing
- Runs on Windows, macOS, and Linux

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later

## Getting Started

### Clone the repository

```bash
git clone https://github.com/EinUraniumPro/bondroid-qr-generator.git
cd bondroid-qr-generator
```

### Build

```bash
dotnet build
```

### Run

```bash
dotnet run --project BondroidQrGenerator -- --input <path-to-csv> [--output <path-to-pdf>]
```

#### Options

| Option      | Alias | Required | Description                                                                                                      |
|-------------|-------|----------|------------------------------------------------------------------------------------------------------------------|
| `--input`   | `-i`  | Yes      | Path to the Bondroid-exported CSV file                                                                           |
| `--output`  | `-o`  | No       | Path for the output PDF. Defaults to `bondroid_qr_codes_<timestamp>.pdf` in the same directory as the input file |
| `--help`    | `-h`  | No       | Shows the Help-Page                                                                                              |

#### Examples

```bash
# Minimal — output PDF is placed next to the CSV
BondroidQrGenerator.exe --input ./tables.csv

# With explicit output path
BondroidQrGenerator.exe --input ./tables.csv --output ./qr_codes.pdf

# Show the help
BondroidQrGenerator.exe --help
```

## CSV Format

The tool expects a semicolon-delimited (`;`) CSV file with a header row, as exported by the Bondroid system. The following columns are required:

| Column header | Description                                                    |
|---------------|----------------------------------------------------------------|
| `ID`          | Numeric Bondroid table ID                                      |
| `Bereich`     | Area/zone where the table is located                           |
| `Tisch`       | Display name of the table (printed as label below the QR code) |
| `QR Text`     | The raw text to be encoded into the QR code                    |

> **Note:** Rows where `Tisch` or `QR Text` are empty or whitespace-only are automatically skipped.  
> Additional columns (e.g. Zebra QR) are ignored by the tool.

### Example CSV

```csv
ID;Bereich;Tisch;QR Text;Zebra QR
120;Zelt;11;id:120;^XA^FO0,14^A0N,128,64^FB456,1,0,C,0^FDZelt / 11\&^FS^FO120,160^BQN,2,10,Q,7^FDQA,id:120^FS^XZ
121;Zelt;12;id:121;^XA^FO0,14^A0N,128,64^FB456,1,0,C,0^FDZelt / 12\&^FS^FO120,160^BQN,2,10,Q,7^FDQA,id:121^FS^XZ
122;Zelt;13;id:122;^XA^FO0,14^A0N,128,64^FB456,1,0,C,0^FDZelt / 13\&^FS^FO120,160^BQN,2,10,Q,7^FDQA,id:122^FS^XZ
123;Zelt;14;id:123;^XA^FO0,14^A0N,128,64^FB456,1,0,C,0^FDZelt / 14\&^FS^FO120,160^BQN,2,10,Q,7^FDQA,id:123^FS^XZ
```

## Output

The generated PDF is formatted for **A4 paper** (595 × 842 pt) and contains:
- A **3-column grid** of QR codes per page (configurable per code)
- Each cell contains a QR code with the table name label rendered below it
- Pages are added automatically as needed

## Project Structure

```
BondroidQrGenerator/
├── Interfaces/
│   ├── ICsvUtil.cs        # Contract for CSV parsing
│   ├── IQrCodeUtil.cs     # Contract for QR code generation & combining
│   ├── ITextUtil.cs       # Contract for label rendering
│   └── IPdfUtil.cs        # Contract for PDF creation
├── Model/
│   └── Table.cs           # Data model mapped from CSV columns
├── Utils/
│   ├── CsvUtil.cs         # Reads and streams Table records from CSV
│   ├── QrCodeUtil.cs      # Renders QR bitmaps and combines them with labels
│   ├── TextUtil.cs        # Renders bold text label bitmaps via SkiaSharp
│   └── PdfUtil.cs         # Lays out bitmaps in a grid and writes the PDF
└── Program.cs             # CLI entry point (System.CommandLine)
```

## Dependencies

| Package                                                                                                                   | Purpose                                    |
|---------------------------------------------------------------------------------------------------------------------------|--------------------------------------------|
| [QRCoder](https://github.com/codebude/QRCoder)                                                                            | QR code data generation                    |
| [SkiaSharp](https://github.com/mono/SkiaSharp)                                                                            | Cross-platform 2D graphics & PDF rendering |
| [CsvHelper](https://joshclose.github.io/CsvHelper/)                                                                       | CSV parsing                                |
| [System.CommandLine](https://github.com/dotnet/command-line-api)                                                          | CLI argument parsing                       |
| [Microsoft.Extensions.DependencyInjection](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) | Dependency injection                       |

## License

This project is licensed under the [MIT License](LICENSE).
