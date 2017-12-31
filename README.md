![example](Logo/Logo.png?raw=true)
  
[![NuGet](https://img.shields.io/badge/Nuget-v1.3.6.2-green.svg)](https://www.nuget.org/packages/WPFHexaEditor/)
[![NetFramework](https://img.shields.io/badge/.Net%20Framework-4.7-green.svg)](https://www.microsoft.com/net/download/windows)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://github.com/abbaye/WpfHexEditorControl/blob/master/LICENSE)

A fast, fully customisable Wpf user control for editing file or stream as hexadecimal. 

Can be used in WPF or WinForm application.

Localized in English, French, Russian and Chinese

### You want to say thank or just like project  ?

Hexeditor control is totaly free and can be used in all project you want like open source and commercial applications. I make it in my free time and a few colaborators help me when they can... Please hit the ⭐️ button or fork and I will be very happy ;) I accept help contribution...

## Screenshots

Sample with standard ASCII character table
![example](Sample11-NOTBL.png?raw=true)

Sample with custom thingy character table (TBL) on SNES Final Fantasy II US
![example](Sample9-TBL.png?raw=true)

Sample use ByteShiftLeft and BytePerLine properties with custom TBL for edit fixed lenght table...
![example](Sample12-FIXEDTBL-BYTESHIFT.png?raw=true)

Sample use of find dialog... (Replace dialog are under construction)
![example](Sample13-FindDialog.png?raw=true)

## Somes features
- Shift the first visible byte in the view to the left for adjust view in fixed TBL... 
- Append byte at end of file
- Include HexBox, an Hexadecimal TextBox with spinner
- Modify and Delete bytes
- Save changes
- Fill selection (or another array) with byte.
- Replace byte by another one.
- Support Brush for customize background, selection, highlight and more 
- Support of common key in window like CTRL+C, CTRL+V, CTRL+Z, CTRL+A, ESC...
- Copy to clipboard as code like C#, VB.Net, C, Java, F# ... 
- Support TBL character table file insted of default ASCII.
- Choose color set of TBL string via dependency property
- Selection with mouse/keyboard or property
- Choose the number of byte per line to show 
- Unlimited Undo (no redo for now)
- Move in file with mouse wheel / or keyboard
- Finds methods (FindFirst, FindNext, FindAll, FindLast, FindSelection) and overload for (string, byte[])
- Highlight byte with somes find methods
- Scollbar marker for selection start and byte finded
- Set Bookmark
- Group byte in block 
- Show data as hexadecimal or decimal
- ...

## How to use
Add a reference to `WPFHexaEditor.dll` from your project, then add the following namespace to your XAML:

```xaml
xmlns:control="clr-namespace:WpfHexaEditor;assembly=WPFHexaEditor"
```

Insert the control like this:

```xaml
<control:HexEditor/>
<control:HexEditor Width="NaN" Height="NaN"/>
<control:HexEditor Width="Auto" Height="Auto"/>
<control:HexEditor FileName={Binding FileNamePath} Width="Auto" Height="Auto"/>
```
