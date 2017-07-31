# WPF Hexadecimal Editor UserControl
A fully customisable WPF user control for editing file or stream as hexadecimal. (SUPPORT THINGY CHARACTER TABLE (TBL))

Sample with standard ASCII character table
![example](WPFHexEditorControlSample7-NOTBL.png?raw=true)

Sample with Thingy character table on SNES Final Fantasy II US
![example](WPFHexEditorControlSample7-TBL.png?raw=true)


## NUGET  Last version : 2017-02-26
https://www.nuget.org/packages/WPFHexaEditor/

## Features
- Modify bytes
- Delete bytes
- Save changes
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
- ...

## Release Notes (version of NUGET package)
New in version 0.9.0.1
- Implementation of TBL file format. (CHARACTER TABLE FOR ROM REVERSE ENGINEERING)
- Added key fonction like ESC to unselect/unlight
- Code optimization and many bug fixed

New in version 0.7.0
- Add FindSelection
- HighLight for Find
- Somes bugs fix and minor add...


## How to use
Add a reference to `WPFHexaEditor.Control.dll` from your project, then add the following namespace to your XAML:

```xaml
xmlns:Control="clr-namespace:WPFHexaEditor.Control;assembly=WPFHexaEditor.Control"
```

Insert the control like this:

```xaml
<Control:HexaEditor/>
<Control:HexaEditor Width="NaN" Height="NaN"/>
<Control:HexaEditor Width="Auto" Height="Auto"/>
<Control:HexaEditor FileName={Binding FileNamePath} Width="Auto" Height="Auto"/>
```
