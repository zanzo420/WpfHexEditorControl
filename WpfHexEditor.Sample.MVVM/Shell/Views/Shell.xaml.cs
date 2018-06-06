using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Data;
using WpfHexaEditor;
using WpfHexEditor.Sample.MVVM.Contracts.Shell;

namespace WpfHexEditor.Sample.MVVM.Views {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Export(typeof(IShell))]
    public partial class Shell : Window,IShell {
        public Shell() {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) {
            dock.Children.Clear();
            for (int i = 0; i < 2; i++) {
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
            }
            
        }
    }
}
