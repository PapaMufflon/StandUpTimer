using Concordion.Integration;
using TestStack.White;
using TestStack.White.Factory;
using TestStack.White.UIItems;

namespace StandUpTimer.Specs
{
    [ConcordionTest]
    public class Index
    {
        public string ItBeginsWithTheSittingPhase()
        {
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
                           ? "It begins with the sitting phase."
                           : "It does not begin with the sitting phase.";
            }
        }

        public string YouCanSeeTheCloseButton()
        {
            using (var application = Application.Launch("StandUpTimer.exe"))
            {
                var window = application.GetWindow("Stand-Up Timer", InitializeOption.NoCache);

                var closeButton = window.Get<Button>("CloseButton");

                if (closeButton == null)
                    return "There is no close button loaded.";

                return closeButton.Visible
                           ? "The close button"
                           : "The close button is not visible.";
            }
        }

        public string YouCanSeeTheSkipButton()
        {
            using (var application = Application.Launch("StandUpTimer.exe"))
            {
                var window = application.GetWindow("Stand-Up Timer", InitializeOption.NoCache);

                var closeButton = window.Get<Button>("SkipButton");

                if (closeButton == null)
                    return "There is no skip button loaded.";

                return closeButton.Visible
                           ? "The skip button"
                           : "The skip button is not visible.";
            }
        }

        public string YouCanSeeTheAttributionButton()
        {
            using (var application = Application.Launch("StandUpTimer.exe"))
            {
                var window = application.GetWindow("Stand-Up Timer", InitializeOption.NoCache);

                var closeButton = window.Get<Button>("AttributionButton");

                if (closeButton == null)
                    return "There is no attribution button loaded.";

                return closeButton.Visible
                           ? "The attribution button"
                           : "The attribution button is not visible.";
            }
        }
    }
}