# WPF Hexadecimal Editor control (Beta)
A WPF Hexadecimal editor for view/modify file.

![example](WPFHexEditorControlSample4.png?raw=true)

##NUGET
https://www.nuget.org/packages/WPFHexaEditor/

## Features
- Modify bytes
- Delete bytes
- Add bytes (soon) 
- Save changes (work fine with file large file but when bytes as deleted of less than 500mg (for now)...)
- Selection with mouse/keyboard or property
- Most of property is Dependency Property. You can use binding :)
- Choose the number of byte per line to show 
- Set position in code
- Move in file with mouse wheel / or keyboard
- ...

##Release Notes
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
<Control:HexaEditor FileName={Binding FileNamePath} />
```
