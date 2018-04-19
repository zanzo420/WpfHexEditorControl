using System.Windows.Controls;

namespace TestDemoUI {
    /// <summary>
    /// Interaction logic for TestOffsetsInfoLayer.xaml
    /// </summary>
    public partial class TestOffsetsInfoLayer : UserControl {
        public TestOffsetsInfoLayer() {
            InitializeComponent();
            infoHoriLayer.StartStepIndex = 0;
            infoHoriLayer.StepsCount = 16;
            infoVerLayer.StartStepIndex = 44546419846419;
            infoVerLayer.StepsCount = 22;
        }
    }
}
