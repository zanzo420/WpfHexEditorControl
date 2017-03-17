//////////////////////////////////////////////
// MIT License  - 2016-2017
// Author : Derek Tremblay (derektremblay666@gmail.com)
//////////////////////////////////////////////

using System.Windows.Controls;
using WPFHexaEditor.Core.CharacterTable;

namespace WPFHexaEditor.Control
{
    /// <summary>
    /// Logique d'interaction pour TBLEntryControl.xaml
    /// </summary>
    internal partial class TBLEntryControl : UserControl
    {
        private DTEType _dteType;

        public TBLEntryControl()
        {
            InitializeComponent();
        }

        private DTEType Type
        {
            get
            {
                return _dteType;
            }
            set
            {
                //switch (value)
                //{
                //    case DTEType.DualTitleEncoding:
                //        picType.Image = ListImageTBL.Images[0];
                //        this._dteType = DTEType.DualTitleEncoding;
                //        break;
                //    case DTEType.ASCII:
                //        picType.Image = ListImageTBL.Images[2];
                //        this._dteType = DTEType.ASCII;
                //        break;
                //    case DTEType.MultipleTitleEncoding:
                //        picType.Image = ListImageTBL.Images[1];
                //        this._dteType = DTEType.MultipleTitleEncoding;
                //        break;
                //    case DTEType.EndBlock:
                //        picType.Image = ListImageTBL.Images[4];
                //        this._dteType = DTEType.EndBlock;
                //        break;
                //    case DTEType.EndLine:
                //        picType.Image = ListImageTBL.Images[4];
                //        this._dteType = DTEType.EndLine;
                //        break;
                //    case DTEType.Japonais:
                //        picType.Image = ListImageTBL.Images[3];
                //        this._dteType = DTEType.Japonais;
                //        break;
                //}
            }
        }
    }
}