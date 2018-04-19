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
            
            //Cuz xaml designer doesn't support generic type binding;We have to set the some bindings(valuetuple) in codebehind :(;
            HexEdit.SetBinding(DrawedHexEditor.CustomBackgroundBlocksProperty, new Binding(nameof(HexEdit.CustomBackgroundBlocks)));
            //CustomBackgroundBlocks = "{Binding CustomBackgroundBlocks}"
            //(this.DataContext as ShellViewModel).FileEditor = HexEdit;
        }

       

      
    }
}
