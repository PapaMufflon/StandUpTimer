using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using StandUpTimer.Services;
using TestStack.White;
using TestStack.White.Factory;
using TestStack.White.UIItems;
using TestStack.White.UIItems.WindowItems;

namespace StandUpTimer.Specs.PageObjects
{
    internal class StandUpTimer : IDisposable
    {
        private readonly Application application;
        private Window window;

        private StandUpTimer(Application application)
        {
            this.application = application;
        }

        private Window Window
        {
            get
            {
                return window ?? (window = application.Find(s => s.Contains("Stand-Up Timer"), InitializeOption.NoCache));
            }
        }

        public static StandUpTimer Launch(string executable = "StandUpTimer.exe", int sittingWaitTime = 1200000, bool deleteCookies = true)
        {
            if (deleteCookies)
                CookieContainerPersistance.DeleteCookiesFromDisk();

            var processStartInfo = new ProcessStartInfo(executable,
                $"--sit {sittingWaitTime} --stand 3600000 --noUpdate --baseUrl http://localhost:12346/");

            return new StandUpTimer(Application.Launch(processStartInfo));
        }

        private T TryAction<T>(Func<T> action)
        {
            var tries = 0;
            AutomationException lastException = null;

            while (tries < 5)
            {
                try
                {
                    var result = action();
                    return result;
                }
                catch (AutomationException e)
                {
                    lastException = e;
                    window = null;
                    tries++;
                }
            }

            throw lastException;
        }

        public void Dispose()
        {
            application.Dispose();
        }

        public string Title
        {
            get { return TryAction(() => Window.Title); }
        }

        public Image CurrentImage
        {
            get { return TryAction(() => Window.Get<Image>("CurrentImage")); }
        }

        public string CurrentImageFileName
        {
            get
            {
                var currentImageFileNameLabel = TryAction(() => Window.Get<Label>("CurrentImageFileName"));

                return currentImageFileNameLabel?.Text;
            }
        }

        public Button CloseButton
        {
            get { return TryAction(() => Window.Get<Button>("CloseButton")); }
        }
        
        public Button SkipButton
        {
            get { return TryAction(() => Window.Get<Button>("SkipButton")); }
        }

        public Button AttributionButton
        {
            get { return TryAction(() => Window.Get<Button>("AttributionButton")); }
        }

        public Button LoginButton
        {
            get { return TryAction(() => Window.Get<Button>("ChangeAuthenticationStateButton")); }
        }

        public string CurrentAuthenticationStatusFileName
        {
            get
            {
                var currentAuthenticationStatusFileNameLabel = TryAction(() => Window.Get<Label>("CurrentAuthenticationStatusFileName"));

                return currentAuthenticationStatusFileNameLabel?.Text;
            }
        }

        public Button OkButton
        {
            get { return TryAction(() => Window.Get<Button>("OkButton")); }
        }

        public ProgressBar ProgressBar
        {
            get { return TryAction(() => Window.Get<ProgressBar>("ProgressBar")); }
        }

        public string ProgressBarText
        {
            get
            {
                var progressBarTextLabel = TryAction(() => Window.Get<Label>("ProgressText"));

                return progressBarTextLabel?.Text;
            }
        }

        public bool IsFocussed
        {
            get { return TryAction(() => Window.IsFocussed); }
        }

        public Point Location => Window.Location;

        public string VersionNumber
        {
            get
            {
                var versionNumberTextBox = TryAction(() => Window.Get<Label>("VersionNumber"));

                return versionNumberTextBox?.Text;
            }
        }

        public void WaitUntilProgressBarTextIs(string text)
        {
            WaitFor(() => ProgressBarText.Equals("60\nmin"));
        }

        private static void WaitFor(Func<bool> func)
        {
            var stopwatch = Stopwatch.StartNew();

            while (stopwatch.Elapsed < TimeSpan.FromSeconds(5))
            {
                if (func())
                    return;
            }
        }

        public bool TryGoToNextPosition(out string errorMessage)
        {
            var skipButton = SkipButton;

            if (skipButton == null)
            {
                errorMessage = "cannot find the skip button";
                return false;
            }

            skipButton.Click();

            errorMessage = string.Empty;
            return true;
        }

        public bool TryStopShaking(out string errorMessage)
        {
            var okButton = OkButton;

            if (okButton == null)
            {
                errorMessage = "Cannot find the OK button.";
                return false;
            }

            okButton.Click();

            errorMessage = string.Empty;
            return true;
        }

        public void OpenAttributionBox()
        {
            Window.Mouse.Location = AttributionButton.ClickablePoint;

            // wait to show
            Thread.Sleep(200);
        }

        public LoginDialog OpenLoginDialog(out string errorMessage)
        {
            var loginButton = LoginButton;

            if (loginButton == null)
            {
                errorMessage = "Cannot find the login button.";
                return null;
            }

            loginButton.Click();

            errorMessage = string.Empty;
            return FindLoginDialog();
        }

        public LoginDialog FindLoginDialog()
        {
            Window loginWindow = null;

            WaitFor(() => (loginWindow = application.GetWindow("Login", InitializeOption.NoCache)) != null);

            return new LoginDialog(loginWindow);
        }

        public void TakeScreenshot(string fileName)
        {
            Window.TakeScreenshot(fileName);
        }

        public void WaitUntilLoggedIn()
        {
            WaitFor(() => CurrentAuthenticationStatusFileName.Equals("..\\Images\\loggedInButton.png"));
        }
    }
}