//////////////////////////////////////////////
// MIT License  - 2016-2017
// Author : Derek Tremblay (derektremblay666@gmail.com)
//////////////////////////////////////////////

using System;
using System.Windows;
using System.Windows.Controls;
using WPFHexaEditor.Core.CharacterTable;

namespace WPFHexaEditor.Control
{
    /// <summary>
    /// Logique d'interaction pour TBLEditor.xaml
    /// </summary>
    public partial class TBLEditor : UserControl
    {
        public TBLEditor()
        {
            InitializeComponent();
        }

        #region Properties
        public TBLStream TBL
        {
            get { return (TBLStream)GetValue(TBLProperty); }
            set { SetValue(TBLProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TBL.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TBLProperty =
            DependencyProperty.Register("TBL", typeof(TBLStream), typeof(TBLEditor),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(TBL_Changed)));

        private static void TBL_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        #endregion Properties

        #region methods
        public void Load()
        {
            if (TBL != null)
            {
                //Load section and content
                //foreach (DTE dte in TBL)
                //{

                //}
            }
        }
        #endregion methods
    }
}