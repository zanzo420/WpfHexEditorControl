//////////////////////////////////////////////
// Fork 2017 : Derek Tremblay (derektremblay666@gmail.com) 
// Reference : https://www.codeproject.com/Tips/431000/Caret-for-WPF-User-Controls
// Reference license : The Code Project Open License (CPOL) 1.02
//////////////////////////////////////////////

using System;
using System.Windows;
using System.Windows.Media;
using System.Threading;

namespace WpfHexaEditor.Core
{
    public class Caret : FrameworkElement
    {
        #region Global class variables

        private Timer _timer;
        private Point _location;
        private int _blinkPeriod = 500;
        private readonly Pen _pen = new Pen(Brushes.Black, 1);
        
        #endregion

        #region Constructor

        public Caret()
        {
            _pen.Freeze();
            _timer = new Timer(BlinkCaret, null, 0, _blinkPeriod);
            Hide();
        }

        #endregion
        
        #region Properties
        private static readonly DependencyProperty VisibleProperty =
            DependencyProperty.Register(nameof(Visible), typeof(bool),
                typeof(Caret), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        private bool Visible
        {
            get => (bool)GetValue(VisibleProperty);
            set => SetValue(VisibleProperty, value);
        }

        public double CaretHeight { get; set; } = 18;

        public double Left
        {
            get => _location.X;
            set
            {
                if (_location.X == value) return;

                _location.X = Math.Floor(value) + .5; //to avoid WPF antialiasing
                if (Visible) Visible = false;
                
            }
        }

        public double Top
        {
            get => _location.Y;
            set
            {
                if (_location.Y == value) return;

                _location.Y = Math.Floor(value) + .5; //to avoid WPF antialiasing
                if (Visible) Visible = false;
            }
        }
        
        public bool IsVisibleCaret => Left >= CaretHeight && Top > CaretHeight;
        #endregion
        
        #region Methods

        public void Hide() => Top = Left = -1;

        private void BlinkCaret(Object state) => Dispatcher?.Invoke(() =>
        {
            Visible = !Visible;
        });

        protected override void OnRender(DrawingContext dc)
        {
            if (Visible)
                dc.DrawLine(_pen, _location, new Point(Left, _location.Y + CaretHeight));
        }
        #endregion
    }
}
