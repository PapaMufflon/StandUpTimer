using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using Concordion.Integration;
using StandUpTimer.Specs.Properties;
using TestStack.White;
using TestStack.White.Factory;
using TestStack.White.UIItems;
using TestStack.White.WindowsAPI;
using Image = TestStack.White.UIItems.Image;

namespace StandUpTimer.Specs
{
    [ConcordionTest]
    public class Index
    {
        public string ItBeginsWithTheSittingPhase(string locale)
        {
            Resources.Culture = new CultureInfo(locale);

            using (var application = Application.Launch("StandUpTimer.exe"))
            {
                var window = application.GetWindow("Stand-Up Timer", InitializeOption.NoCache);

                var currentImage = window.Get<Image>("CurrentImage");

                if (currentImage == null)
                    return "No image loaded.";

                var currentImageFileName = window.Get<Label>("CurrentImageFileName");

                if (currentImageFileName == null)
                    return "Cannot determine current image.";

                return currentImageFileName.Text.Equals("..\\Images\\sitting.png")
                           ? Resources.ItBeginsWithTheSittingPhase
                           : "It does not begin with the sitting phase.";
            }
        }

        public string YouCanSeeTheCloseButton(string locale)
        {
            Resources.Culture = new CultureInfo(locale);

            using (var application = Application.Launch("StandUpTimer.exe"))
            {
                var window = application.GetWindow("Stand-Up Timer", InitializeOption.NoCache);

                var closeButton = window.Get<Button>("CloseButton");

                if (closeButton == null)
                    return "There is no close button.";

                return closeButton.Visible
                           ? Resources.TheCloseButton
                           : "The close button is not visible.";
            }
        }

        public string YouCanSeeTheSkipButton(string locale)
        {
            Resources.Culture = new CultureInfo(locale);

            using (var application = Application.Launch("StandUpTimer.exe"))
            {
                var window = application.GetWindow("Stand-Up Timer", InitializeOption.NoCache);

                var skipButton = window.Get<Button>("SkipButton");

                if (skipButton == null)
                    return "There is no skip button.";

                return skipButton.Visible
                           ? Resources.TheSkipButton
                           : "The skip button is not visible.";
            }
        }

        public string YouCanSeeTheAttributionButton(string locale)
        {
            Resources.Culture = new CultureInfo(locale);

            using (var application = Application.Launch("StandUpTimer.exe"))
            {
                var window = application.GetWindow("Stand-Up Timer", InitializeOption.NoCache);

                var attributionButton = window.Get<Button>("AttributionButton");

                if (attributionButton == null)
                    return "There is no attribution button.";

                return attributionButton.Visible
                           ? Resources.TheAttributionButton
                           : "The attribution button is not visible.";
            }
        }

        public string YouCanSeeTheRemainingTime(string locale)
        {
            Resources.Culture = new CultureInfo(locale);

            using (var application = Application.Launch("StandUpTimer.exe"))
            {
                var window = application.GetWindow("Stand-Up Timer", InitializeOption.NoCache);

                var progressBar = window.Get<ProgressBar>("ProgressBar");

                if (progressBar == null)
                    return "There is no progress bar.";

                if (!progressBar.Visible)
                    return "The progress bar is not visible.";

                var progressText = window.Get<Label>("ProgressText");

                return string.IsNullOrEmpty(progressText.Text)
                           ? "No progress information available"
                           : Resources.YouCanSeeTheRemainingTime;
            }
        }

        public void TakeStartScreenshot()
        {
            using (var application = Application.Launch("StandUpTimer.exe"))
            {
                var window = application.GetWindow("Stand-Up Timer", InitializeOption.NoCache);

                window.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.PRINTSCREEN);

                Directory.CreateDirectory(@"results\StandUpTimer\Specs");
                File.Move("screenshot.png", @"results\StandUpTimer\Specs\start.png");
            }
        }

        public string WaitForTheTimeToElapseToGoToTheNextPosition(string locale)
        {
            Resources.Culture = new CultureInfo(locale);

            const int sittingWaitTime = 1000;
            var processStartInfo = new ProcessStartInfo("StandUpTimer.exe", string.Format("--sit {0} --stand 3600000", sittingWaitTime));

            using (var application = Application.Launch(processStartInfo))
            {
                var window = application.GetWindow("Stand-Up Timer", InitializeOption.NoCache);

                Thread.Sleep(sittingWaitTime);

                var currentImageFileName = window.Get<Label>("CurrentImageFileName");

                if (currentImageFileName == null)
                    return "Cannot determine current image.";

                return currentImageFileName.Text.Equals("..\\Images\\standing.png")
                           ? Resources.WaitForTheTimeToElapse
                           : "There was a wrong image after waiting the sitting time.";
            }
        }

        public string UseTheSkipButtonToGoToTheNextPosition(string locale)
        {
            Resources.Culture = new CultureInfo(locale);

            using (var application = Application.Launch("StandUpTimer.exe"))
            {
                var window = application.GetWindow("Stand-Up Timer", InitializeOption.NoCache);

                var skipButton = window.Get<Button>("SkipButton");
                skipButton.Click();

                var currentImageFileName = window.Get<Label>("CurrentImageFileName");

                if (currentImageFileName == null)
                    return "Cannot determine current image.";

                return currentImageFileName.Text.Equals("..\\Images\\standing.png")
                           ? Resources.UseTheSkipButton
                           : "There was a wrong image after clicking the skip button.";
            }
        }

        public string AfterTheTimeElapsedTheAppGetsIntoView(string locale)
        {
            Resources.Culture = new CultureInfo(locale);

            const int sittingWaitTime = 1000;
            var processStartInfo = new ProcessStartInfo("StandUpTimer.exe", string.Format("--sit {0} --stand 3600000", sittingWaitTime));

            using (var application = Application.Launch(processStartInfo))
            {
                var window = application.GetWindow("Stand-Up Timer", InitializeOption.NoCache);

                var psi = new ProcessStartInfo("notepad.exe") { WindowStyle = ProcessWindowStyle.Maximized };
                var process = Process.Start(psi);

                Thread.Sleep(sittingWaitTime);

                var result = window.IsFocussed
                                 ? Resources.TheAppWillGetIntoView
                                 : "the app didn't get into view";

                process.Kill();

                return result;
            }
        }

        public string AfterTheTimeElapsedTheOkButtonIsVisible(string locale)
        {
            Resources.Culture = new CultureInfo(locale);

            const int sittingWaitTime = 1000;
            var processStartInfo = new ProcessStartInfo("StandUpTimer.exe", string.Format("--sit {0} --stand 3600000", sittingWaitTime));

            using (var application = Application.Launch(processStartInfo))
            {
                var window = application.GetWindow("Stand-Up Timer", InitializeOption.NoCache);

                Thread.Sleep(sittingWaitTime);

                var okButton = window.Get<Button>("OkButton");

                if (okButton == null)
                    return "Cannot find the OK button.";

                return okButton.Visible
                           ? Resources.TheOkButtonIsVisible
                           : "the OK button is not visible";
            }
        }

        public string AfterClickingOkTheTimeTicksAgain(string locale)
        {
            Resources.Culture = new CultureInfo(locale);

            const int sittingWaitTime = 1000;
            var processStartInfo = new ProcessStartInfo("StandUpTimer.exe", string.Format("--sit {0} --stand 3600000", sittingWaitTime));

            using (var application = Application.Launch(processStartInfo))
            {
                var window = application.GetWindow("Stand-Up Timer", InitializeOption.NoCache);

                Thread.Sleep(sittingWaitTime);

                var okButton = window.Get<Button>("OkButton");

                if (okButton == null)
                    return "Cannot find the OK button.";

                okButton.Click();

                var progressText = window.Get<Label>("ProgressText");

                WaitFor(() => progressText.Text.Equals("60\nmin"));

                return progressText.Text.Equals("60\nmin")
                           ? Resources.TheTimeIsTickingAgain
                           : "the time is not ticking correctly, " + progressText.Text + " was shown.";
            }
        }

        public void TakeNextPhaseScreenshot()
        {
            using (var application = Application.Launch("StandUpTimer.exe"))
            {
                var window = application.GetWindow("Stand-Up Timer", InitializeOption.NoCache);

                var skipButton = window.Get<Button>("SkipButton");
                skipButton.Click();

                var progressText = window.Get<Label>("ProgressText");

                WaitFor(() => progressText.Text.Equals("20\nmin"));

                window.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.PRINTSCREEN);

                Directory.CreateDirectory(@"results\StandUpTimer\Specs");
                File.Move("screenshot.png", @"results\StandUpTimer\Specs\nextPhase.png");
            }
        }

        public string TheAppStartsOnTheSamePosition(string locale)
        {
            Resources.Culture = new CultureInfo(locale);

            Point savedLocation;

            using (var application = Application.Launch("StandUpTimer.exe"))
            {
                var window = application.GetWindow("Stand-Up Timer", InitializeOption.NoCache);

                savedLocation = window.Location;
            }

            using (var application = Application.Launch("StandUpTimer.exe"))
            {
                var window = application.GetWindow("Stand-Up Timer", InitializeOption.NoCache);

                return savedLocation == window.Location
                           ? Resources.OnTheNextStartupTheAppStartOnThatPositionAgain
                           : "the app is not on the previous position.";
            }
        }

        public void TakeAttributionScreenshot()
        {
            using (var application = Application.Launch("StandUpTimer.exe"))
            {
                var window = application.GetWindow("Stand-Up Timer", InitializeOption.NoCache);

                var attributionButton = window.Get<Button>("AttributionButton");
                window.Mouse.Location = attributionButton.ClickablePoint;

                Thread.Sleep(200);

                window.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.PRINTSCREEN);

                Directory.CreateDirectory(@"results\StandUpTimer\Specs");
                File.Move("screenshot.png", @"results\StandUpTimer\Specs\attribution.png");
            }
        }

        private void WaitFor(Func<bool> func)
        {
            var stopwatch = Stopwatch.StartNew();

            while (stopwatch.Elapsed < TimeSpan.FromSeconds(5))
            {
                if (func())
                    return;
            }
        }
    }
}