//////////////////////////////////////////////
// MIT License  - 2016-2017
// Author : Derek Tremblay (derektremblay666@gmail.com)
//////////////////////////////////////////////

using System;
using System.Windows.Threading;

namespace WPFHexaEditor.Core.Helper
{
    /// <summary>
    /// WPF UI Dispatcher
    /// </summary>
    public class DispatcherHelper
    {
        private static DispatcherOperationCallback exitFrameCallback = new DispatcherOperationCallback(ExitFrame);
        
        /// <summary>
        /// Execute all message in message Queud
        /// </summary>
        public static void DoEvents(DispatcherPriority priority = DispatcherPriority.Background)
        {            

            DispatcherFrame nestedFrame = new DispatcherFrame();
            
            DispatcherOperation exitOperation = Dispatcher.CurrentDispatcher.BeginInvoke(priority, exitFrameCallback, nestedFrame);

            try
            {
                //execute all next message
                Dispatcher.PushFrame(nestedFrame);


                //If not completed, will stop it
                if (exitOperation.Status != DispatcherOperationStatus.Completed)
                    exitOperation.Abort();
            }
            catch
            {
                exitOperation.Abort();
            }
            
        }

        private static Object ExitFrame(Object state)
        {
            DispatcherFrame frame = state as DispatcherFrame;

            // exit the message loop
            frame.Continue = false;
            return null;
        }
    }
}
