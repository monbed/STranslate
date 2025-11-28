using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using STranslate.Core;
using STranslate.Plugin;
using System.Diagnostics;
using System.IO;

namespace STranslate.ViewModels.Pages;

public partial class AboutViewModel(Settings settings, DataProvider dataProvider) : ObservableObject
{
    public Settings Settings { get; } = settings;
    public DataProvider DataProvider { get; } = dataProvider;
    [ObservableProperty] public partial string AppVersion { get; set; } = VersionInfo.GetVersion();

    [RelayCommand]
    private void LocateUserData()
    {
        var settingsFolderPath = Path.Combine(DataLocation.SettingsDirectory);
        var parentFolderPath = Path.GetDirectoryName(settingsFolderPath);
        if (Directory.Exists(parentFolderPath))
        {
            Process.Start("explorer.exe", parentFolderPath);
        }
    }

    [RelayCommand]
    private void LocateLog()
    {
        var logFolderPath = Path.Combine(Constant.LogDirectory);
        if (Directory.Exists(logFolderPath))
        {
            Process.Start("explorer.exe", logFolderPath);
        }
    }

    [RelayCommand]
    private void LocateSettings()
    {
        var settingsFolderPath = Path.Combine(DataLocation.SettingsDirectory);
        if (Directory.Exists(settingsFolderPath))
        {
            Process.Start("explorer.exe", settingsFolderPath);
        }
    }

    [RelayCommand]
    private void Backup()
    {
        var saveFileDialog = new SaveFileDialog
        {
            Title = "Select Backup File",
            Filter = "zip(*.zip)|*.zip",
            FileName = $"stranslate_backup_{DateTime.Now:yyyyMMddHHmmss}"
        };

        if (saveFileDialog.ShowDialog() != true)
            return;

        var filePath = saveFileDialog.FileName;
        string[] args = [
            "backup",
            "-m", "backup",
            "-a", filePath,
            "-f", DataLocation.PluginsDirectory,
            "-f", DataLocation.SettingsDirectory,
            "-d", "3",
            "-l", DataLocation.AppExePath,
            "-c", DataLocation.InfoFilePath,
            "-w", $"备份配置成功 [{filePath}]"
            ];
        Utilities.ExecuteProgram(DataLocation.HostExePath, args);
        App.Current.Shutdown();
    }

    [RelayCommand]
    private void Restore()
    {
        var openFileDialog = new OpenFileDialog
        {
            Title = "Select Restore File",
            Filter = "zip(*.zip)|*.zip"
        };
        if (openFileDialog.ShowDialog() != true)
            return;
        var filePath = openFileDialog.FileName;
        string[] args = [
            "backup", "-m",
            "restore", "-a",
            filePath, "-s", Constant.Plugins,
            "-t", DataLocation.PluginsDirectory,
            "-s", Constant.Settings,
            "-t", DataLocation.SettingsDirectory,
            "-d", "3",
            "-l", DataLocation.AppExePath,
            "-c", DataLocation.InfoFilePath,
            "-w", $"恢复配置成功 [{filePath}]"
        ];
        Utilities.ExecuteProgram(DataLocation.HostExePath, args);
        App.Current.Shutdown();
    }
}
