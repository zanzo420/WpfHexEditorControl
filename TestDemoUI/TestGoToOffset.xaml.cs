using System.IO;
using System.Windows;
using System.Windows.Controls;

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
