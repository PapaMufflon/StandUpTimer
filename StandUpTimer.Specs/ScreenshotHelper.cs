using System.IO;
using TestStack.White.UIItems.WindowItems;
using TestStack.White.WindowsAPI;

namespace StandUpTimer.Specs
{
    internal static class ScreenshotHelper
    {
        public static void TakeScreenshot(this Window window, string fileName)
        {
            window.Keyboard.PressSpecialKey(KeyboardInput.SpecialKeys.PRINTSCREEN);

            Directory.CreateDirectory(@"results\StandUpTimer\Specs");
            File.Copy("screenshot.png", string.Format(@"results\StandUpTimer\Specs\{0}", fileName), true);
        }
    }
}