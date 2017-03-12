using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
                return this._dteType;
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
