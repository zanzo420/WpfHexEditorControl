using System;
using System.Collections.Generic;
using System.IO;
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

namespace TestDemoUI {
    /// <summary>
    /// Interaction logic for TestGoToOffset.xaml
    /// </summary>
    public partial class TestGoToOffset : UserControl {
        public TestGoToOffset() {
            InitializeComponent();
            editor.Stream = File.OpenRead("E://HBMS400M.img");
            
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            editor.Position = int.Parse(txbNum.Text);
            editor.FocusPosition = int.Parse(txbNum.Text);
        }
    }
}
