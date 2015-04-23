using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using TestStack.White;
using TestStack.White.Factory;
using TestStack.White.UIItems;
using TestStack.White.UIItems.WindowItems;
using TestStack.White.WindowsAPI;

namespace StandUpTimer.Specs
{
    internal class StandUpTimer : IDisposable
    {
        private readonly Application application;
        private readonly Window window;

        private StandUpTimer(Application application)
        {
            this.application = application;

            window = application.GetWindow("Stand-Up Timer", InitializeOption.NoCache);
        }

        public static StandUpTimer Launch(int sittingWaitTime = 1200000)
        {
            var processStartInfo = new ProcessStartInfo("StandUpTimer.exe", string.Format("--sit {0} --stand 3600000 --noUpdate", sittingWaitTime));

            return new StandUpTimer(Application.Launch(processStartInfo));
        }

        public void Dispose()
        {
            application.Dispose();
        }

        public Image CurrentImage
        {
            get { return window.Get<Image>("CurrentImage"); }
        }

        public string CurrentImageFileName
        {
            get
            {
                var currentImageFileNameLabel = window.Get<Label>("CurrentImageFileName");

                return currentImageFileNameLabel == null ? null : currentImageFileNameLabel.Text;
            }
        }

        public Button CloseButton
        {
            get { return window.Get<Button>("CloseButton"); }
        }
        
        public Button SkipButton
        {
            get { return window.Get<Button>("SkipButton"); }
        }

        public Button AttributionButton
        {
            get { return window.Get<Button>("AttributionButton"); }
        }

        public Button LoginButton
        {
            get { return window.Get<Button>("LoginButton"); }
        }

        public Button OkButton
        {
            get { return window.Get<Button>("OkButton"); }
        }

        public ProgressBar ProgressBar
        {
            get { return window.Get<ProgressBar>("ProgressBar"); }
        }

        public string ProgressBarText
        {
            get
            {
                var progressBarTextLabel = window.Get<Label>("ProgressText");

                return progressBarTextLabel == null ? null : progressBarTextLabel.Text;
            }
        }

        public bool IsFocussed
        {
            get { return window.IsFocussed; }
        }

        public Point Location { get { return window.Location; } }

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
            window.Mouse.Location = AttributionButton.ClickablePoint;

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

            var loginWindow = application.GetWindow("Login", InitializeOption.NoCache);

            errorMessage = string.Empty;
            return new LoginDialog(loginWindow);
        }

        public void TakeScreenshot(string fileName)
        {
            window.TakeScreenshot(fileName);
        }
    }
}