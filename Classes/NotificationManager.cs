/*
 * General class to define generic notification functions
 */
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskCurator.Classes
{
    public static partial class NotificationManager
    {
        static partial void DoSendNotification(string title, string message, DateTime scheduledTime);
        
        //Function for toast pop-up notifications
        public static async void ShowToast(string message)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            ToastDuration duration = ToastDuration.Short;
            double fontSize = 14;
            var toast = Toast.Make(message, duration, fontSize);
            await toast.Show(cancellationTokenSource.Token);
        }

        public static void SendNotification(string title, string message, DateTime scheduledTime)
        {
            DoSendNotification(title, message, scheduledTime);
        }

    }
}
