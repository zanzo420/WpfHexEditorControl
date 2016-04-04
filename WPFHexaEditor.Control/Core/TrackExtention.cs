using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WPFHexaEditor.Control.Core
{
    public static class TrackExtention
    {
        /// <summary>
        /// Get actual top position of track
        /// </summary>
        public static double Top(this Track s)
        {
            Grid parent = s.Parent as Grid;
            RepeatButton TopRepeatButton = (RepeatButton)parent.Children[1];

            return TopRepeatButton.ActualHeight + parent.Margin.Top + 1;
        }

        /// <summary>
        /// Get actual bottom position of track
        /// </summary>
        public static double Bottom(this Track s)
        {            
            Grid parent = s.Parent as Grid;

            Track TrackControl = (Track)parent.Children[2];

            return TrackControl.Top() + 
                TrackControl.ActualHeight + 
                parent.Margin.Top + 1;
        }

        /// <summary>
        /// Get actual bottom position of track
        /// </summary>
        public static double ButtonHeight(this Track s)
        {
            Grid parent = s.Parent as Grid;

            Track TrackControl = (Track)parent.Children[2];
            
            return TrackControl.Top() - 1;
        }

        /// <summary>
        /// Get actual Tick Height
        /// </summary>
        public static double TickHeight(this Track s)
        {
            Grid parent = s.Parent as Grid;

            RepeatButton TopRepeatButton = (RepeatButton)parent.Children[1];
            Track TrackControl = (Track)parent.Children[2];
            RepeatButton BottomRepeatButton = (RepeatButton)parent.Children[3];

            return TrackControl.ActualHeight / TrackControl.Maximum;
        }




        ///// <summary>
        ///// Get actual bottom position of track
        ///// </summary>
        //public static double ValuePosition(this Track s)
        //{
        //    Grid parent = s.Parent as Grid;

        //    RepeatButton TopRepeatButton = (RepeatButton)parent.Children[1];
        //    Track TrackControl = (Track)parent.Children[2];
        //    RepeatButton BottomRepeatButton = (RepeatButton)parent.Children[3];

        //    return TrackControl.Top() +
        //        TrackControl.ActualHeight +
        //        parent.Margin.Top + 1;
        //}
    }
}
