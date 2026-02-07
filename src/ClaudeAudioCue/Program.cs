namespace ClaudeAudioCue;

static class Program
{
    private static Mutex? _mutex;

    [STAThread]
    static void Main()
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

        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());

        GC.KeepAlive(_mutex);
    }
}
