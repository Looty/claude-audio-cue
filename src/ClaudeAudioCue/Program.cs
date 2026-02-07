namespace ClaudeAudioCue;

static class Program
{
    private static Mutex? _mutex;

    /// <summary>
    /// When true, the app starts minimized to the system tray (used for auto-start with Windows).
    /// </summary>
    public static bool StartMinimized { get; private set; }

    [STAThread]
    static void Main(string[] args)
    {
        _mutex = new Mutex(true, "ClaudeAudioCue_SingleInstance", out bool createdNew);
        if (!createdNew)
        {
            MessageBox.Show(
                "Claude Audio Cue is already running.\nCheck the system tray.",
                "Claude Audio Cue",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        StartMinimized = args.Contains("--minimized", StringComparer.OrdinalIgnoreCase);

        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());

        GC.KeepAlive(_mutex);
    }
}
