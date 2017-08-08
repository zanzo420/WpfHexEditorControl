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
using WPFHexaEditor.Core;
using WPFHexaEditor.Core.Bytes;

namespace WPFHexaEditor.Control
{
    /// <summary>
    /// Logique d'interaction pour HexBox.xaml
    /// </summary>
    public partial class HexBox : UserControl
    {
        public HexBox()
        {
            InitializeComponent();
        }



        public long MaximumValue
        {
            get { return (long)GetValue(MaximumValueProperty); }
            set { SetValue(MaximumValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaximumValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumValueProperty =
            DependencyProperty.Register("MaximumValue", typeof(long), typeof(HexBox), 
                new FrameworkPropertyMetadata(long.MaxValue, new PropertyChangedCallback(MaximumValue_Changed)));

        private static void MaximumValue_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HexBox ctrl = d as HexBox;

            if (e.NewValue != e.OldValue)            
                if (ctrl.LongValue > (long)e.NewValue)
                    ctrl.UpdateValueFrom((long)e.NewValue);            
        }

        /// <summary>
        /// Get or set the hex value show in control
        /// </summary>
        public long LongValue
        {
            get { return (long)GetValue(LongValueProperty); }
            set { SetValue(LongValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LongValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LongValueProperty =
            DependencyProperty.Register("LongValue", typeof(long), typeof(HexBox), 
                new FrameworkPropertyMetadata(0L, 
                    new PropertyChangedCallback(LongValue_Changed), 
                    new CoerceValueCallback(LongValue_CoerceValue)));

        private static object LongValue_CoerceValue(DependencyObject d, object baseValue)
        {
            //long coerceVal = (long)baseValue;

            //if ((long)baseValue > )

            return (long)baseValue;
        }

        private static void LongValue_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HexBox ctrl = d as HexBox;

            if (e.NewValue != e.OldValue)
            {
                ctrl.HexTextBox.Text = ByteConverters.LongToHex((long)e.NewValue).TrimStart('0').ToUpper();
                ctrl.ToolTip = e.NewValue;
            }
        }

        private void HexTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (KeyValidator.IsHexKey(e.Key) ||
                KeyValidator.IsBackspaceKey(e.Key) ||
                KeyValidator.IsDeleteKey(e.Key) ||
                KeyValidator.IsArrowKey(e.Key) ||
                KeyValidator.IsEnterKey(e.Key))
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void HexTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up) AddOne();
            else if (e.Key == Key.Down) SubstractOne();
        }

        /// <summary>
        /// Substract one to the LongValue
        /// </summary>
        private void SubstractOne()
        {
            if (LongValue > 0)
                LongValue--;
        }

        /// <summary>
        /// Add one to the LongValue
        /// </summary>
        private void AddOne()
        {
            LongValue++;
        }

        /// <summary>
        /// Update value from decimal long
        /// </summary>
        /// <param name="value"></param>
        private void UpdateValueFrom(long value)
        {
            LongValue = value;
        }

        /// <summary>
        /// Update value from hex string
        /// </summary>
        /// <param name="value"></param>
        private void UpdateValueFrom(string value)
        {
            try
            {
                LongValue = ByteConverters.HexLiteralToLong(value);
            }
            catch
            {
                LongValue = 0;
            }
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            AddOne();
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            SubstractOne();
        }

        private void HexTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateValueFrom(HexTextBox.Text);
        }
        
        private void CopyHexaMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText($"0x{HexTextBox.Text}");
        }

        private void CopyLongMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(LongValue.ToString());
        }
    }
}
