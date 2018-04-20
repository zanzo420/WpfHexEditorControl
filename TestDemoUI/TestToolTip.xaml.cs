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

namespace TestDemoUI {
    /// <summary>
    /// Interaction logic for TestToolTip.xaml
    /// </summary>
    public partial class TestToolTip : UserControl {
        public TestToolTip() {
            InitializeComponent();
        }

        private void add_Click(object sender, RoutedEventArgs e) {
            for (int i = 0; i < 1000; i++) {
                var elem = new ToolTipItem();
                stack.Children.Add(elem);
                elems.Add(elem);
            }
            
        }
        private List<UIElement> elems = new List<UIElement>();
        private void remove_Click(object sender, RoutedEventArgs e) {
            GC.Collect();
            for (int i = 0; i < 1000; i++) {
                
                if (elems.Count == 0) {
                    return;
                }

                var lastElem = elems[elems.Count - 1];
                elems.Remove(lastElem);
                stack.Children.Remove(lastElem);
                
            }
            GC.Collect();
        }
    }
}
