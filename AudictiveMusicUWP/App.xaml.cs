using AudictiveMusicUWP.Gui.Pages;
using ClassLibrary.Db;
using ClassLibrary.Helpers;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.UI;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace AudictiveMusicUWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private LicenseInformation licenseInformation;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            licenseInformation = CurrentApp.LicenseInformation;
            this.Resuming += App_Resuming;
            this.Suspending += OnSuspending;
            //this.UnhandledException += App_UnhandledException;
        }

        //private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        //{
        //    e.Handled = true;
        //}

        private void App_Resuming(object sender, object e)
        {
            ApplicationSettings.AppState = BackgroundAudioShared.AppState.Active;

            //MessageService.SendMessageToBackground(new AppStateMessage(BackgroundAudioShared.AppState.Active));
            //ApplicationData.Current.LocalSettings.Values["AppState"] = BackgroundAudioShared.AppState.Active.ToString();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        async protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            BuildInfo.RetrieveApiInfo();


#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                //TODO: Load state from previously suspended application
            }

            string parameter = e.Arguments;

            OnLaunchedOrActivated(parameter);
            //if (e.PrelaunchActivated == false)
            //{

                

                // Ensure the current window is active
            //}
        }

        private async void OnLaunchedOrActivated(string arguments)
        {
            Frame rootFrame = await SetMainFrame();

            ApplicationSettings.AppState = BackgroundAudioShared.AppState.Active;
            //MessageService.SendMessageToBackground(new AppStateMessage(BackgroundAudioShared.AppState.Active));
            //ApplicationData.Current.LocalSettings.Values["AppState"] = BackgroundAudioShared.AppState.Active.ToString();

            

            if (rootFrame.Content == null || string.IsNullOrEmpty(arguments) == false)
            {
                bool passedWelcomeScreen = false;

                DB.InitializeDatabase();

                passedWelcomeScreen = ApplicationData.Current.LocalSettings.Values.ContainsKey("AudictiveMusic10RTM");


                if (!passedWelcomeScreen)
                {
                    rootFrame.Navigate(typeof(SetupWizard), arguments);

                    if (ApplicationInfo.Current.IsMobile == false)
                    {
                        if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
                        {
                            ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = Colors.White;
                            ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Colors.Transparent;
                            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
                        }
                    }
                    else
                    {
                        await StatusBar.GetForCurrentView().HideAsync();
                    }
                }
                //else if (needsToRecheckCollection)
                //{
                //    rootFrame.Navigate(typeof(PreparingCollection), arguments);
                //}
                else
                {
                    rootFrame.Navigate(typeof(MainPage), arguments);
                }
            }

            Window.Current.Activate();

            ApplicationData.Current.LocalSettings.Values["AppVersion"] = ApplicationInfo.Current.AppVersion;
        }

        private async Task<Frame> SetMainFrame()
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (JumpList.IsSupported())
            {
                var jumpList = await JumpList.LoadCurrentAsync();

                jumpList.SystemGroupKind = JumpListSystemGroupKind.None;

                var taskItem1 = CreateJumpListItem("action=playEverything",
                    ApplicationInfo.Current.Resources.GetString("PlayEverything"),
                    new Uri("ms-appx:///Assets/resumePlayback-icon.png", UriKind.Absolute));

                //var taskItem2 = CreateJumpListItem("action=navigate&amp;target=settings&amp;path=appInfo",
                //    "Informações do aplicativo",
                //    null);

                //var taskItem3 = CreateJumpListItem("action=navigate&amp;target=settings&amp;path=playback",
                //    "Configurações de notificação",
                //    null);

                //var taskItem4 = CreateJumpListItem("action=navigate&amp;target=settings&amp;path=feedback",
                //    "Enviar comentários",
                //    null);

                var taskItem5 = CreateJumpListItem("action=resumePlayback",
                    ApplicationInfo.Current.Resources.GetString("ResumePlayback"),
                    new Uri("ms-appx:///Assets/resumePlayback-icon.png", UriKind.Absolute));

                jumpList.Items.Clear();

                jumpList.Items.Add(taskItem1);
                //jumpList.Items.Add(taskItem2);
                //jumpList.Items.Add(taskItem3);
                //jumpList.Items.Add(taskItem4);
                jumpList.Items.Add(taskItem5);

                await jumpList.SaveAsync();

            }

            //BackgroundExecutionManager.RemoveAccess();

            //var taskRegistered = false;
            //var taskName = "ToastBGTask";

            //foreach (var task in BackgroundTaskRegistration.AllTasks)
            //{
            //    if (task.Value.Name == taskName)
            //    {
            //        taskRegistered = true;
            //        break;
            //    }
            //}

            //if (taskRegistered == false)
            //{
            //    BackgroundAccessStatus status = await BackgroundExecutionManager.RequestAccessAsync();

            //    try
            //    {
            //        // Construct the background task
            //        BackgroundTaskBuilder builder = new BackgroundTaskBuilder()
            //        {
            //            Name = taskName,
            //            TaskEntryPoint = "ToastTask.BGTask",
            //        };

            //        builder.SetTrigger(new ToastNotificationActionTrigger());
            //        BackgroundTaskRegistration registration = builder.Register();
            //    }
            //    catch
            //    {

            //    }
            //}

            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(320, 400));

            return rootFrame;
        }

        private JumpListItem CreateJumpListItem(string arguments, string displayName, Uri imageUri)
        {
            var taskItem = JumpListItem.CreateWithArguments(
                            arguments, displayName);

            taskItem.Logo = imageUri;

            return taskItem;
        }

        async protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);

            string parameter = string.Empty;

            if (args.Kind == ActivationKind.ToastNotification)
            {
                ToastNotificationActivatedEventArgs t = args as ToastNotificationActivatedEventArgs;
                // Parse the query string
                parameter = t.Argument;
            }

            OnLaunchedOrActivated(parameter);
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            ApplicationSettings.AppState = BackgroundAudioShared.AppState.Suspended;

            //ApplicationData.Current.LocalSettings.Values["AppState"] = BackgroundAudioShared.AppState.Suspended.ToString();
            //MessageService.SendMessageToBackground(new AppStateMessage(BackgroundAudioShared.AppState.Suspended));
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
