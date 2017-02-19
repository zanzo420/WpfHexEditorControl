# WPF Hexadecimal Editor control (Beta)
A WPF Hexadecimal editor for view/modify file.

![example](WPFHexEditorControlSample5.png?raw=true)

##NUGET
https://www.nuget.org/packages/WPFHexaEditor/

## Features
- Use TBL character table file insted of default ASCII.
- Modify bytes
- Delete bytes
- Add bytes (soon) 
- Save changes (work fine with file large file but when bytes as deleted of less than 500mg (for now)...)
- Selection with mouse/keyboard or property
- Most of property is Dependency Property. You can use binding :)
- Choose the number of byte per line to show 
- Set position in code
- Unlimited Undo (no redo for now)
- Move in file with mouse wheel / or keyboard
- Modify mode (hexa / char)
- Finds methods (FindFirst, FindNext, FindAll, FindLast, FindSelection) and overlord for (string, byte[])
- Highlight byte with somes find methods
- Scollbar marker for selection start and byte finded (soon bookmark will be added)
- ...

##Release Notes (version of NUGET package)
New in version 0.8.0
- Implementation of TBL file format. (CHARACTER TABLE FOR ROM REVERSE ENGINEERING)

New in version 0.7.0
- Add FindSelection
- HighLight for Find
- Somes bugs fix and minor add...

New in version 0.6.1 
- Add functionnality - FindFirst, FindNext, FindLast
- Bug Fix - When refresh control at last line
- Bug Fix - refresh file after saving

New in version 0.1.0 
- Add functionnality - BytePerLine 
- Add functionality - Set position
- Add edition mode (in developpement)

## How to use
Add a reference to `WPFHexaEditor.Control.dll` from your project, then add the following namespace to your XAML:

```xaml
xmlns:Control="clr-namespace:WPFHexaEditor.Control;assembly=WPFHexaEditor.Control"
```

Insert the control like this:

```xaml
<Control:HexaEditor FileName={Binding FileNamePath} Width="Auto" Height="Auto"/>
```
