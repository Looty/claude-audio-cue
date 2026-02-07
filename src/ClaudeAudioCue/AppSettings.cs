using System.Text.Json;

namespace ClaudeAudioCue;

public enum ThemeMode
{
    Light,
    Dark
}

public class WindowPosition
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}

public class AppSettings
{
    private const string SettingsFileName = "settings.json";
    private const string AppFolderName = "ClaudeAudioCue";

    public string SelectedSound { get; set; } = "notify.wav";
    public int PollIntervalMs { get; set; } = 500;
    public int VolumePercent { get; set; } = 70; // 30% reduction from default
    public ThemeMode ThemeMode { get; set; } = ThemeMode.Dark;
    public int CooldownSeconds { get; set; } = 3; // Minimum seconds between audio cues
    public bool StartWithWindows { get; set; } = false;
    public WindowPosition? MainWindowPosition { get; set; }

    private static string GetSettingsPath()
    {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(appData, AppFolderName, SettingsFileName);
    }

    public void Save()
    {
        try
        {
            string path = GetSettingsPath();
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }
        catch
        {
            // Silently fail — settings are not critical
        }
    }

    public static AppSettings Load()
    {
        try
        {
            string path = GetSettingsPath();
            if (!File.Exists(path))
                return new AppSettings();

            string json = File.ReadAllText(path);
            var settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();

            // Validate volume percent (0-200%)
            settings.VolumePercent = Math.Clamp(settings.VolumePercent, 0, 200);

            return settings;
        }
        catch
        {
            return new AppSettings();
        }
    }
}
