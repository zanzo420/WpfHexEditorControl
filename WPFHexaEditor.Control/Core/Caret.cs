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
        private readonly Pen _pen = new Pen(Brushes.Black, 1);
        private int _blinkPeriod = 500;

        #endregion

        #region Constructor
        public Caret()
        {
            _pen.Freeze();
            InitializeTimer();
            Hide();
        }
        #endregion
        
        #region Properties
        private static readonly DependencyProperty VisibleProperty =
            DependencyProperty.Register(nameof(Visible), typeof(bool),
                typeof(Caret), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Propertie used when caret is blinking
        /// </summary>
        private bool Visible
        {
            get => (bool)GetValue(VisibleProperty);
            set => SetValue(VisibleProperty, value);
        }

        /// <summary>
        /// Height of the caret
        /// </summary>
        public double CaretHeight { get; set; } = 18;

        /// <summary>
        /// Left position of the caret
        /// </summary>
        public double Left
        {
            get => _location.X;
            internal set
            {
                if (_location.X == value) return;

                _location.X = Math.Floor(value) + .5; //to avoid WPF antialiasing
                if (Visible) Visible = false;
            }
        }

        /// <summary>
        /// Top position of the caret
        /// </summary>
        public double Top
        {
            get => _location.Y;
            internal set
            {
                if (_location.Y == value) return;

                _location.Y = Math.Floor(value) + .5; //to avoid WPF antialiasing
                if (Visible) Visible = false;
            }
        }
        
        /// <summary>
        /// Properties return true if caret is visible
        /// </summary>
        public bool IsVisibleCaret => Left >= 0 && Top > CaretHeight;

        /// <summary>
        /// Blick period in millisecond
        /// </summary>
        public int BlinkPeriod
        {
            get => _blinkPeriod;
            set
            {
                _blinkPeriod = value;
                InitializeTimer();
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Hide the caret
        /// </summary>
        public void Hide() => Top = Left = -1;

        /// <summary>
        /// Methode delegate for blink the caret
        /// </summary>
        private void BlinkCaret(Object state) => Dispatcher?.Invoke(() =>
        {
            Visible = !Visible;
        });

        /// <summary>
        /// Initialise the timer
        /// </summary>
        private void InitializeTimer() => _timer = new Timer(BlinkCaret, null, 0, BlinkPeriod);
        
        /// <summary>
        /// Move the caret over the position defined by point parameter
        /// </summary>
        public void MoveCaret(Point point)
        {
            Left = point.X;
            Top = point.Y;
        }

        /// <summary>
        /// Render the caret
        /// </summary>
        protected override void OnRender(DrawingContext dc)
        {
            if (Visible)
                dc.DrawLine(_pen, _location, new Point(Left, _location.Y + CaretHeight));
        }
        #endregion
    }
}
