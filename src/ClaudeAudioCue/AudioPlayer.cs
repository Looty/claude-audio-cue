using NAudio.Wave;

namespace ClaudeAudioCue;

public class AudioPlayer
{
    private const string WindowsMediaPath = @"C:\Windows\Media";
    private static readonly string UserSoundsPath = 
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
            "ClaudeAudioCue", "Sounds");

    public string? SoundFilePath { get; set; }

    /// <summary>
    /// Get all .wav filenames from C:\Windows\Media and user sounds folder, sorted alphabetically.
    /// </summary>
    public static string[] GetAvailableSounds()
    {
        var sounds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Get sounds from Windows\Media
        try
        {
            if (Directory.Exists(WindowsMediaPath))
            {
                var files = Directory.GetFiles(WindowsMediaPath, "*.wav")
                    .Select(Path.GetFileName)
                    .Where(f => f != null)
                    .Cast<string>();
                foreach (var f in files)
                    sounds.Add(f);
            }
        }
        catch { }

        // Get sounds from user folder
        try
        {
            if (Directory.Exists(UserSoundsPath))
            {
                var files = Directory.GetFiles(UserSoundsPath, "*.wav")
                    .Select(Path.GetFileName)
                    .Where(f => f != null)
                    .Cast<string>();
                foreach (var f in files)
                    sounds.Add(f);
            }
        }
        catch { }

        return sounds.OrderBy(f => f, StringComparer.OrdinalIgnoreCase).ToArray();
    }

    /// <summary>
    /// Get the full path for a sound filename (checks both Windows\Media and user folder).
    /// </summary>
    public static string GetFullSoundPath(string filename)
    {
        // Check user folder first
        string userPath = Path.Combine(UserSoundsPath, filename);
        if (File.Exists(userPath))
            return userPath;

        // Fall back to Windows\Media
        return Path.Combine(WindowsMediaPath, filename);
    }

    /// <summary>
    /// Play the selected sound asynchronously (non-blocking) with volume control.
    /// </summary>
    public void Play(int volumePercent = 100)
    {
        if (string.IsNullOrEmpty(SoundFilePath) || !File.Exists(SoundFilePath))
        {
            LogError($"Cannot play: {SoundFilePath ?? "(null)"}");
            return;
        }

        try
        {
            // Use NAudio for better format support and volume control
            var audioFileReader = new AudioFileReader(SoundFilePath);
            
            // Set volume (0.0 to 1.0 scale)
            float volumeLevel = Math.Clamp(volumePercent / 100f, 0f, 1f);
            audioFileReader.Volume = volumeLevel;

            var wavePlayer = new WaveOutEvent();
            wavePlayer.Init(audioFileReader);
            wavePlayer.Play();

            // Fire and forget - NAudio handles cleanup
        }
        catch (Exception ex)
        {
            LogError($"Play() failed: {ex.GetType().Name} - {ex.Message}");
        }
    }

    /// <summary>
    /// Play the selected sound synchronously (blocks until done) with volume control.
    /// Useful for preview.
    /// </summary>
    public void PlaySync(int volumePercent = 100)
    {
        if (string.IsNullOrEmpty(SoundFilePath))
        {
            LogError("SoundFilePath is null or empty");
            return;
        }

        if (!File.Exists(SoundFilePath))
        {
            LogError($"File not found: {SoundFilePath}");
            return;
        }

        try
        {
            using var audioFileReader = new AudioFileReader(SoundFilePath);
            
            // Set volume (0.0 to 1.0 scale)
            float volumeLevel = Math.Clamp(volumePercent / 100f, 0f, 1f);
            audioFileReader.Volume = volumeLevel;

            using var wavePlayer = new WaveOutEvent();
            wavePlayer.Init(audioFileReader);
            wavePlayer.Play();

            // Block until playback completes
            while (wavePlayer.PlaybackState == PlaybackState.Playing)
            {
                System.Threading.Thread.Sleep(100);
            }
        }
        catch (Exception ex)
        {
            LogError($"PlaySync failed: {ex.GetType().Name} - {ex.Message}");
        }
    }

    private static void LogError(string message)
    {
        try
        {
            string logPath = Path.Combine(Path.GetTempPath(), "AudioCueError.log");
            File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}\n");
        }
        catch { }
    }
}
