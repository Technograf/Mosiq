using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;
using Microsoft.QueryStringDotNET;
using Windows.Storage;
using System.Diagnostics;

namespace ToastTask
{
    public sealed class BGTask : IBackgroundTask
    {
        BackgroundTaskDeferral _deferral;
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();

            Debug.WriteLine("A ToastTask FOI INICIADA");

            var details = taskInstance.TriggerDetails as ToastNotificationActionTriggerDetail;

            await RunTask(details.Argument);

            _deferral.Complete();
        }

        private async Task RunTask(string argument)
        {
            Debug.WriteLine("ARGUMENTS: " + argument);

            if (argument == "turnOffTapToResumeToast")
            {
                ApplicationData.Current.LocalSettings.Values["DisplayTapToResumeToast"] = false;
            }
        }

        private string GetNavigationParameter(string args, string attribute)
        {
            if (string.IsNullOrWhiteSpace(args) == false)
            {
                QueryString arguments = QueryString.Parse(args);

                // See what action is being requested 
                if (arguments.Contains(attribute))
                {
                    return arguments[attribute];
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }

    }
}
