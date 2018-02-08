using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
using WpfHexEditor.Sample.MVVM.ViewModels;

namespace WpfHexEditor.Sample.MVVM.Views {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Shell : Window {
        public Shell() {
            this.WindowState = WindowState.Maximized;
            InitializeComponent();

            //(this.DataContext as ShellViewModel).FileEditor = HexEdit;
        }
    }
}
