namespace ClaudeAudioCue;

public partial class MainForm : Form
{
    private readonly AppSettings _settings;
    private readonly AudioPlayer _audioPlayer;
    private ClaudeMonitor? _monitor;
    private bool _isExiting;

    // Color scheme for light/dark themes
    private static class Colors
    {
        public static class Light
        {
            public static Color Background = Color.FromArgb(245, 245, 245);
            public static Color Text = Color.FromArgb(51, 51, 51);
            public static Color TextSecondary = Color.FromArgb(102, 102, 102);
            public static Color Accent = Color.FromArgb(0, 120, 212);
            public static Color AccentHover = Color.FromArgb(0, 100, 180);
            public static Color ComboBackground = Color.White;
        }

        public static class Dark
        {
            public static Color Background = Color.FromArgb(32, 32, 32);
            public static Color Text = Color.FromArgb(240, 240, 240);
            public static Color TextSecondary = Color.FromArgb(180, 180, 180);
            public static Color Accent = Color.FromArgb(90, 180, 255);
            public static Color AccentHover = Color.FromArgb(60, 150, 230);
            public static Color ComboBackground = Color.FromArgb(45, 45, 45);
        }

        // Status colors (muted palette)
        public static Color StatusIdle = Color.FromArgb(153, 153, 153);
        public static Color StatusSearching = Color.FromArgb(212, 176, 64);
        public static Color StatusMonitoring = Color.FromArgb(16, 124, 16);
        public static Color StatusStreaming = Color.FromArgb(16, 110, 190);
        public static Color StatusError = Color.FromArgb(209, 52, 56);
    }

    public MainForm()
    {
        InitializeComponent();

        _settings = AppSettings.Load();
        _audioPlayer = new AudioPlayer();

        // Apply theme before loading data
        ApplyTheme();

        SetupSoundList();
        SetupEventHandlers();

        // Auto-start monitoring
        StartMonitoring();
    }

    private void SetupSoundList()
    {
        var sounds = AudioPlayer.GetAvailableSounds();
        cboSound.Items.AddRange(sounds);

        // Select saved sound or default
        int idx = Array.IndexOf(sounds, _settings.SelectedSound);
        if (idx >= 0)
            cboSound.SelectedIndex = idx;
        else if (sounds.Length > 0)
            cboSound.SelectedIndex = 0;

        UpdateAudioPlayerPath();
    }

    private void SetupEventHandlers()
    {
        btnToggle.Click += BtnToggle_Click;
        btnTest.Click += BtnTest_Click;
        btnSettings.Click += BtnSettings_Click;
        cboSound.SelectedIndexChanged += CboSound_SelectedIndexChanged;

        trayMenuShow.Click += (_, _) => ShowFromTray();
        trayMenuToggle.Click += (_, _) => BtnToggle_Click(this, EventArgs.Empty);
        trayMenuExit.Click += (_, _) => ExitApplication();

        trayIcon.DoubleClick += (_, _) => ShowFromTray();

        this.FormClosing += MainForm_FormClosing;
        this.Resize += MainForm_Resize;
    }

    private void UpdateAudioPlayerPath()
    {
        if (cboSound.SelectedItem is string filename)
        {
            _audioPlayer.SoundFilePath = AudioPlayer.GetFullSoundPath(filename);
        }
    }

    private void StartMonitoring()
    {
        if (_monitor != null)
            return;

        var progress = new Progress<MonitorStatus>(OnStatusChanged);
        _monitor = new ClaudeMonitor(progress, _audioPlayer, _settings.PollIntervalMs, _settings.VolumePercent);
        _monitor.Start();

        btnToggle.Text = "Stop Monitoring";
        trayMenuToggle.Text = "Stop Monitoring";
    }

    private void StopMonitoring()
    {
        _monitor?.Dispose();
        _monitor = null;

        btnToggle.Text = "Start Monitoring";
        trayMenuToggle.Text = "Start Monitoring";
        UpdateStatus("Idle", Colors.StatusIdle);
    }

    private void OnStatusChanged(MonitorStatus status)
    {
        switch (status)
        {
            case MonitorStatus.Idle:
                UpdateStatus("Idle", Colors.StatusIdle);
                break;
            case MonitorStatus.Searching:
                UpdateStatus("Searching for Claude...", Colors.StatusSearching);
                break;
            case MonitorStatus.Monitoring:
                UpdateStatus("Monitoring", Colors.StatusMonitoring);
                break;
            case MonitorStatus.StreamingDetected:
                UpdateStatus("Claude is responding...", Colors.StatusStreaming);
                break;
            case MonitorStatus.StreamingEnded:
                UpdateStatus("Response complete!", Colors.StatusMonitoring);
                break;
            case MonitorStatus.Error:
                UpdateStatus("Error — retrying...", Colors.StatusError);
                break;
        }
    }

    private void UpdateStatus(string text, Color color)
    {
        if (InvokeRequired)
        {
            Invoke(() => UpdateStatus(text, color));
            return;
        }

        lblStatusValue.Text = text;
        lblStatusValue.ForeColor = color;
        trayIcon.Text = $"Claude Audio Cue — {text}";
    }

    // -- Event handlers --

    private void BtnSettings_Click(object? sender, EventArgs e)
    {
        using (var dialog = new SettingsDialog(_settings, _audioPlayer))
        {
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                // Theme changed
                ApplyTheme();

                // Poll interval changed — restart monitoring with new interval
                if (_monitor?.IsRunning == true)
                {
                    StopMonitoring();
                    StartMonitoring();
                }

                // Audio player path still valid; volume passed directly to Play()
            }
        }
    }

    private void ApplyTheme()
    {
        Color bgColor = _settings.ThemeMode == ThemeMode.Dark ? Colors.Dark.Background : Colors.Light.Background;
        Color textColor = _settings.ThemeMode == ThemeMode.Dark ? Colors.Dark.Text : Colors.Light.Text;
        Color secColor = _settings.ThemeMode == ThemeMode.Dark ? Colors.Dark.TextSecondary : Colors.Light.TextSecondary;
        Color accentColor = _settings.ThemeMode == ThemeMode.Dark ? Colors.Dark.Accent : Colors.Light.Accent;
        Color accentHoverColor = _settings.ThemeMode == ThemeMode.Dark ? Colors.Dark.AccentHover : Colors.Light.AccentHover;
        Color comboColor = _settings.ThemeMode == ThemeMode.Dark ? Colors.Dark.ComboBackground : Colors.Light.ComboBackground;

        this.BackColor = bgColor;
        lblTitle.ForeColor = textColor;
        lblStatus.ForeColor = secColor;
        lblSound.ForeColor = secColor;

        cboSound.BackColor = comboColor;
        cboSound.ForeColor = textColor;

        // Apply to buttons with modern styling
        var primaryButtons = new[] { btnToggle, btnSettings };
        foreach (var btn in primaryButtons)
        {
            btn.BackColor = accentColor;
            btn.ForeColor = Color.White;
            btn.FlatAppearance.MouseOverBackColor = accentHoverColor;
        }

        btnTest.BackColor = accentColor;
        btnTest.ForeColor = Color.White;
        btnTest.FlatAppearance.MouseOverBackColor = accentHoverColor;
    }

    private void BtnToggle_Click(object? sender, EventArgs e)
    {
        if (_monitor?.IsRunning == true)
            StopMonitoring();
        else
            StartMonitoring();
    }

    private void BtnTest_Click(object? sender, EventArgs e)
    {
        // Play preview on a background thread to avoid blocking UI
        Task.Run(() => _audioPlayer.PlaySync(_settings.VolumePercent));
    }

    private void CboSound_SelectedIndexChanged(object? sender, EventArgs e)
    {
        UpdateAudioPlayerPath();

        if (cboSound.SelectedItem is string filename)
        {
            _settings.SelectedSound = filename;
            _settings.Save();
        }
    }

    private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (_isExiting)
        {
            // Actually exit
            _monitor?.Dispose();
            trayIcon.Visible = false;
            return;
        }

        if (e.CloseReason == CloseReason.UserClosing)
        {
            // Minimize to tray instead of closing
            e.Cancel = true;
            this.Hide();
            trayIcon.Visible = true;
            trayIcon.ShowBalloonTip(
                2000,
                "Claude Audio Cue",
                "Still monitoring in the background. Right-click the tray icon to exit.",
                ToolTipIcon.Info);
        }
    }

    private void MainForm_Resize(object? sender, EventArgs e)
    {
        if (this.WindowState == FormWindowState.Minimized)
        {
            this.Hide();
            trayIcon.Visible = true;
            trayIcon.ShowBalloonTip(
                2000,
                "Claude Audio Cue",
                "Minimized to tray. Double-click the tray icon to restore.",
                ToolTipIcon.Info);
        }
    }

    private void ShowFromTray()
    {
        this.Show();
        this.WindowState = FormWindowState.Normal;
        this.BringToFront();
        this.Activate();
        trayIcon.Visible = false;
    }

    private void ExitApplication()
    {
        _isExiting = true;
        this.Close();
    }
}
