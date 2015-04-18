using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Squirrel;

namespace StandUpTimer.Services
{
    internal class Updater
    {
        private readonly Action close;
        private UpdateManager updateManager;
        private UpdateInfo updateInfo;

        public Updater(Action close)
        {
            Contract.Requires(close != null);

            this.close = close;

            updateManager = new UpdateManager(@"http://mufflonosoft.blob.core.windows.net/standuptimer", "StandUpTimer", FrameworkVersion.Net45);

            SquirrelAwareApp.HandleEvents(
                onInitialInstall: v => updateManager.CreateShortcutForThisExe(),
                onAppUpdate: v =>
                    {
                        updateManager.CreateShortcutForThisExe();
                        Close();
                    },
                onAppUninstall: v =>
                    {
                        updateManager.RemoveShortcutForThisExe();
                        Close();
                    });

            Task.Factory.StartNew(TryFetchUpdates);
        }

        private async void TryFetchUpdates()
        {
            updateInfo = await updateManager.CheckForUpdate();

            if (updateInfo.ReleasesToApply.Count > 0)
            {
                await updateManager.DownloadReleases(updateInfo.ReleasesToApply);
                await updateManager.ApplyReleases(updateInfo);
            }

            updateManager.Dispose();
            updateManager = null;
        }

        private void Close()
        {
            if (updateManager != null)
                updateManager.Dispose();

            close();
        }
    }
}