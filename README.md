# ReoGrid

![NuGet](https://img.shields.io/nuget/v/unvell.ReoGrid.DLL.svg)

Fast and powerful open-source .NET spreadsheet component for building Excel-like experiences in WPF and WinForms applications.

https://reogrid.net

## Features

- Excel (XLSX) import and export via OpenXML
- Rich cell formatting (font, size, color, borders, alignment, wrapping, rotation)
- Formulas and functions (SUM, COUNT, IF, VLOOKUP, and more)
- Charts, images, and drawing objects
- Merge cells, freeze panes, and split view
- Sorting and AutoFilter (column filters)
- Cell types and controls (checkbox, dropdown, hyperlink, button, etc.)
- Grouping and outline for rows/columns
- Printing and page setup
- High performance with large worksheets
- Extensible rendering and event model

## Supported frameworks

- .NET 8 (Windows): `net8.0-windows7.0`
- .NET Framework: `net48`

## Installation

- .NET CLI

  ```bash
  dotnet add package unvell.ReoGrid.DLL
  ```

- Package Manager Console (Visual Studio)

  ```powershell
  Install-Package unvell.ReoGrid.DLL
  ```

## Quick start

### WinForms

```csharp
using unvell.ReoGrid;

var grid = new ReoGridControl { Dock = DockStyle.Fill };
this.Controls.Add(grid);

var sheet = grid.CurrentWorksheet;
sheet["A1"] = "Hello ReoGrid";
sheet.Cells["B1"].Data = DateTime.Now;

// Load/Save XLSX
// sheet.Load("input.xlsx");
// sheet.Save("output.xlsx", FileFormat.Excel2007);
```

### WPF

- XAML

  ```xml
  <Window
      x:Class="MyApp.MainWindow"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:rg="clr-namespace:unvell.ReoGrid;assembly=unvell.ReoGrid"
      Title="ReoGrid WPF" Height="450" Width="800">
    <Grid>
      <rg:ReoGridControl x:Name="grid"/>
    </Grid>
  </Window>
  ```

- Code-behind

  ```csharp
  using unvell.ReoGrid;

  // In Window Loaded
  var sheet = grid.CurrentWorksheet;
  sheet["A1"] = "Hello ReoGrid";
  sheet.Cells["B1"].Data = 123.45;

  // Load/Save XLSX
  // sheet.Load("input.xlsx");
  // sheet.Save("output.xlsx", FileFormat.Excel2007);
  ```

- Or create the control in code

  ```csharp
  var grid = new unvell.ReoGrid.ReoGridControl();
  Content = grid;
  var sheet = grid.CurrentWorksheet;
  sheet["A1"] = "Hello from code";
  ```


## Documentation

- Getting started, API reference, and advanced topics:
  https://reogrid.net/document

## Snapshots

Read from Excel<br />
![Snapshot - Read from Excel](https://raw.githubusercontent.com/unvell/ReoGrid/master/Snapshots/02.png)

Print Settings<br />
![Snapshot - Print setting](https://raw.githubusercontent.com/unvell/ReoGrid/master/Snapshots/01_2.png)

Charts<br />
![Snapshot - Charts](https://raw.githubusercontent.com/unvell/ReoGrid/master/Snapshots/276.png)

Cells Freeze<br />
![Snapshot - Freeze](https://raw.githubusercontent.com/unvell/ReoGrid/master/Snapshots/08.png)

Cell Types and Controls<br />
![Snapshot - Cell Types and Controls](https://raw.githubusercontent.com/unvell/ReoGrid/master/Snapshots/62.png)

Group and Outline<br />
![Snapshot - Group and Outline](https://raw.githubusercontent.com/unvell/ReoGrid/master/Snapshots/61.png)

Custom Control Appearance<br />
![Snapshot - Custom Control Appearance](https://raw.githubusercontent.com/unvell/ReoGrid/master/Snapshots/21.png)

Script and Macro Execution<br />
![Snapshot - Script and Macro](https://raw.githubusercontent.com/unvell/ReoGrid/master/Snapshots/27.png)

## Samples

- A runnable WPF sample app is available under the `DemoWPF` project.

## License

MIT License

Copyright (c) UNVELL Inc. 2012-2026, All rights reserved.

