using Microsoft.Win32;

namespace ClaudeAudioCue;

/// <summary>
/// Manages the Windows startup registry entry for the application.
/// Uses HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run (no admin required).
/// </summary>
public static class StartupManager
{
    private const string RegistryKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
    private const string AppName = "ClaudeAudioCue";

    /// <summary>
    /// Returns true if the app is currently registered to start with Windows.
    /// </summary>
    public static bool IsEnabled()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, false);
            return key?.GetValue(AppName) != null;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Enables or disables the startup entry.
    /// When enabled, uses the current executable path.
    /// Passes --minimized so the app starts in the system tray.
    /// </summary>
    public static void SetEnabled(bool enable)
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath, true);
            if (key == null) return;

            if (enable)
            {
                string exePath = Environment.ProcessPath ?? Application.ExecutablePath;
                key.SetValue(AppName, $"\"{exePath}\" --minimized");
            }
            else
            {
                key.DeleteValue(AppName, throwOnMissingValue: false);
            }
        }
        catch
        {
            // Silently fail — not critical
        }
    }
}
