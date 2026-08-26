# VaderConsulting.AdvancedTextEditor

C# .NET 3.5 WinForms UserControl: Word-like rich-text editor with ruler, formatting toolbars, find, RTF I/O, and print. Hosts sibling `VaderConsulting.ExtendedRichTextBox` and `TextRuler` controls plus a `dlgFind` dialog for match-case / whole-word search. Menus and buttons cover bold/italic/underline/strike, alignment, lists, colours, insert picture/date-time, undo/redo, and print preview. The project references `VaderConsulting.TextRuler` from a sibling Historical Dev folder; `FontComboBox` appears only as a commented designer stub.

**Source last updated:** 2020-05-06 · **Language:** C# · **Target:** .NET Framework 3.5 · **Output:** class library (WinForms `UserControl`)

## Solution structure

| Project | Language | Type | Purpose |
|---------|----------|------|---------|
| `VaderConsulting.AdvancedTextEditor` | C# | class library (`net35`, WinForms UserControl) | Rich-text editor surface (`AdvancedTextEditor`) with toolbars, menus, RTF I/O, print, and `dlgFind`. |

## How to open

Open `VaderConsulting.AdvancedTextEditor.csproj` in Visual Studio 2013 or later (ToolsVersion 12.0). A full build also needs the sibling projects `VaderConsulting.TextRuler` and `VaderConsulting.ExtendedRichTextBox` (referenced from the designer / project reference paths).

## Attribution and provenance

Working copy from Dave Robinson's OneDrive Historical Dev folder `VaderConsulting.AdvancedTextEditor`. Assembly copyright © 2015; namespace `VaderConsulting`.

## License

MIT © 2026 VaderConsulting. See `LICENSE`.
