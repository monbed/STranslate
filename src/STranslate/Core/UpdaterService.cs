using Microsoft.Extensions.Logging;
using STranslate.Plugin;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text.Json;
using System.Windows;
using Velopack;
using Velopack.Sources;
using MessageBox = iNKORE.UI.WPF.Modern.Controls.MessageBox;

namespace STranslate.Core;

public class UpdaterService(
    ILogger<UpdaterService> logger,
    Internationalization i18n,
    INotification notification
    )
{
    private SemaphoreSlim UpdateLock { get; } = new SemaphoreSlim(1);

    public async Task UpdateAppAsync(bool silentUpdate = true)
    {
        await UpdateLock.WaitAsync();
        try
        {
            if (!silentUpdate)
                notification.Show("Update Check", "Checking for updates...");

            var updateManager = new UpdateManager(new GithubSource(Constant.GitHub, accessToken: default, prerelease: false));

            var newUpdateInfo = await updateManager.CheckForUpdatesAsync();

            if (newUpdateInfo == null)
            {
                if (!silentUpdate)
                    MessageBox.Show("No update info found.");
                logger.LogInformation("No update info found.");
                return;
            }

            var newReleaseVersion = SemanticVersioning.Version.Parse(newUpdateInfo.TargetFullRelease.Version.ToString());
            var currentVersion = SemanticVersioning.Version.Parse(Constant.Version);

            logger.LogInformation($"Future Release <{JsonSerializer.Serialize(newUpdateInfo.TargetFullRelease)}>");

            if (newReleaseVersion <= currentVersion)
            {
                if (!silentUpdate)
                    MessageBox.Show("You are already on the latest version.");
                return;
            }

            if (!silentUpdate)
                notification.Show("Update Available", $"New version {newReleaseVersion} found. Updating...");

            await updateManager.DownloadUpdatesAsync(newUpdateInfo);

            if (DataLocation.PortableDataLocationInUse())
            {
                var targetDestination = Path.Combine(Path.GetTempPath(), Constant.TmpConfigFolderName);
                FilesFolders.CopyAll(DataLocation.PortableDataPath, targetDestination, MessageBox.Show);

                if (!FilesFolders.VerifyBothFolderFilesEqual(DataLocation.PortableDataPath, targetDestination, MessageBox.Show))
                    MessageBox.Show(string.Format(i18n.GetTranslation("update_flowlauncher_fail_moving_portable_user_profile_data"),
                        DataLocation.PortableDataPath,
                        targetDestination));
            }

            var newVersionTips = NewVersionTips(newReleaseVersion.ToString());

            if (!silentUpdate)
                notification.Show("Update Ready", newVersionTips);
            logger.LogInformation($"Update success:{newVersionTips}");

            if (MessageBox.Show(newVersionTips, "STranslate", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                updateManager.WaitExitThenApplyUpdates(newUpdateInfo);
                Application.Current.Shutdown();
            }
        }
        catch (Exception e)
        {
            if (e is HttpRequestException or WebException or SocketException ||
                e.InnerException is TimeoutException)
            {
                logger.LogError(e, $"Check your connection and proxy settings to github-cloud.s3.amazonaws.com.");
            }
            else
            {
                logger.LogError(e, $"Error Occurred");
            }

            if (!silentUpdate)
                notification.Show("Update Failed", "Failed to update the application. Please check your connection.");
        }
        finally
        {
            UpdateLock.Release();
        }
    }

    private string NewVersionTips(string version)
    {
        var tips = string.Format("New version {0} is available, would you like to restart STranslate to use the update?", version);

        return tips;
    }
}