using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFHexaEditor.Control.Core;

namespace WPFHexaEditor.Control
{
    /// <summary>
    /// Interaction logic for StringByteControl.xaml
    /// </summary>
    public partial class StringByteControl : UserControl
    {
        private byte? _byte = null;

        public event EventHandler StringByteModified;

        public StringByteControl()
        {
            InitializeComponent();
        }

        public byte? Byte
        {
            get
            {
                return this._byte;
            }
            set
            {
                bool modified = false;

                if (_byte.HasValue)
                    if (value != _byte)
                        modified = true;

                this._byte = value;

                UpdateLabelFromByte();

                if (modified)
                    if (StringByteModified != null)
                        StringByteModified(this, new EventArgs());
            }
        }

        /// <summary>
        /// Update control label from byte property
        /// </summary>
        private void UpdateLabelFromByte()
        {
            if (_byte != null)
            {                
                StringByteLabel.Content = Converters.ByteToChar(_byte.Value);
            }
            else
            {
                StringByteLabel.Content = "";                
            }
        }
    }
}
