//////////////////////////////////////////////
// Fork : Derek Tremblay (derektremblay666@gmail.com) 
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
        private Timer timer;
        private Point _location;
        private int _blinkPeriod = 500;
        private readonly Pen _pen = new Pen(Brushes.Black, 1);

 
        public Caret()
        {
            _pen.Freeze();
            CaretHeight = 18;
            Visible = true;
            timer = new Timer(BlinkCaret, null, 0, _blinkPeriod);
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (Visible)
                dc.DrawLine(_pen, _location, new Point(Left, _location.Y + CaretHeight));
        }

        public static readonly DependencyProperty VisibleProperty =
            DependencyProperty.Register("Visible", typeof(bool),
                typeof(Caret), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
        
        public bool Visible
        {
            get => (bool)GetValue(VisibleProperty);
            set => SetValue(VisibleProperty, value);
        }

        private void BlinkCaret(Object state) => Dispatcher.Invoke(delegate { Visible = !Visible; });

        private double CaretHeight { get; }

        public double Left
        {
            get => _location.X;
            set
            {
                if (_location.X == value) return;

                _location.X = Math.Floor(value) + .5; //to avoid WPF antialiasing
                if (Visible)
                {
                    Visible = false;
                }
            }
        }

        public double Top
        {
            get => _location.Y;
            set
            {
                if (_location.Y != value)
                {
                    _location.Y = Math.Floor(value) + .5; //to avoid WPF antialiasing
                    if (Visible)
                    {
                        Visible = false;
                    }
                }
            }
        }
    }
}
