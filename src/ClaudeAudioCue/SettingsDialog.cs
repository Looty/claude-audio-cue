namespace ClaudeAudioCue;

public partial class SettingsDialog : Form
{
    private readonly AppSettings _settings;
    private readonly AudioPlayer _audioPlayer;

    // Shared color scheme matching MainForm
    private static class Colors
    {
        public static class Light
        {
            public static Color Background = Color.FromArgb(245, 245, 245);
            public static Color Text = Color.FromArgb(51, 51, 51);
            public static Color TextSecondary = Color.FromArgb(102, 102, 102);
            public static Color Accent = Color.FromArgb(0, 120, 212);
            public static Color AccentHover = Color.FromArgb(0, 100, 180);
            public static Color ButtonAlt = Color.FromArgb(224, 224, 224);
        }

        public static class Dark
        {
            public static Color Background = Color.FromArgb(32, 32, 32);
            public static Color Text = Color.FromArgb(240, 240, 240);
            public static Color TextSecondary = Color.FromArgb(180, 180, 180);
            public static Color Accent = Color.FromArgb(90, 180, 255);
            public static Color AccentHover = Color.FromArgb(60, 150, 230);
            public static Color ButtonAlt = Color.FromArgb(50, 50, 50);
        }
    }

    public SettingsDialog(AppSettings settings, AudioPlayer audioPlayer)
    {
        _settings = settings;
        _audioPlayer = audioPlayer;

        InitializeComponent();
        ApplyTheme();
        LoadSettings();
        SetupEventHandlers();
    }

    private void ApplyTheme()
    {
        Color bgColor = _settings.ThemeMode == ThemeMode.Dark ? Colors.Dark.Background : Colors.Light.Background;
        Color textColor = _settings.ThemeMode == ThemeMode.Dark ? Colors.Dark.Text : Colors.Light.Text;
        Color secColor = _settings.ThemeMode == ThemeMode.Dark ? Colors.Dark.TextSecondary : Colors.Light.TextSecondary;
        Color accentColor = _settings.ThemeMode == ThemeMode.Dark ? Colors.Dark.Accent : Colors.Light.Accent;
        Color accentHoverColor = _settings.ThemeMode == ThemeMode.Dark ? Colors.Dark.AccentHover : Colors.Light.AccentHover;
        Color buttonAltColor = _settings.ThemeMode == ThemeMode.Dark ? Colors.Dark.ButtonAlt : Colors.Light.ButtonAlt;

        // Apply to form and base controls
        this.BackColor = bgColor;
        this.ForeColor = textColor;

        // Labels
        lblTitle.ForeColor = textColor;
        lblVolume.ForeColor = secColor;
        lblVolumeValue.ForeColor = secColor;
        lblPollInterval.ForeColor = secColor;
        lblPollIntervalValue.ForeColor = secColor;
        lblTheme.ForeColor = secColor;

        // Track bars
        trkVolume.BackColor = bgColor;
        trkPollInterval.BackColor = bgColor;

        // Radio buttons
        radThemeLight.ForeColor = textColor;
        radThemeDark.ForeColor = textColor;

        // Startup section
        lblStartup.ForeColor = secColor;
        chkStartWithWindows.ForeColor = textColor;

        // Primary button (OK - Accent)
        btnOK.BackColor = accentColor;
        btnOK.ForeColor = Color.White;
        btnOK.FlatAppearance.MouseOverBackColor = accentHoverColor;

        // Secondary buttons (Test & Cancel - Alt color)
        btnTestSound.BackColor = buttonAltColor;
        btnTestSound.ForeColor = textColor;
        btnTestSound.FlatAppearance.MouseOverBackColor = ControlPaint.Light(buttonAltColor);

        btnCancel.BackColor = buttonAltColor;
        btnCancel.ForeColor = textColor;
        btnCancel.FlatAppearance.MouseOverBackColor = ControlPaint.Light(buttonAltColor);
    }

    private void LoadSettings()
    {
        // Load volume
        trkVolume.Value = _settings.VolumePercent;
        lblVolumeValue.Text = $"{_settings.VolumePercent}%";

        // Load poll interval
        trkPollInterval.Value = _settings.PollIntervalMs;
        lblPollIntervalValue.Text = $"{_settings.PollIntervalMs} ms";

        // Load cooldown
        trkCooldown.Value = _settings.CooldownSeconds;
        lblCooldownValue.Text = $"{_settings.CooldownSeconds} sec";

        // Load theme
        if (_settings.ThemeMode == ThemeMode.Dark)
            radThemeDark.Checked = true;
        else
            radThemeLight.Checked = true;

        // Load startup — read actual registry state as source of truth
        chkStartWithWindows.Checked = StartupManager.IsEnabled();
    }

    private void SetupEventHandlers()
    {
        trkVolume.Scroll += (_, _) =>
        {
            lblVolumeValue.Text = $"{trkVolume.Value}%";
        };

        trkPollInterval.Scroll += (_, _) =>
        {
            lblPollIntervalValue.Text = $"{trkPollInterval.Value} ms";
        };

        trkCooldown.Scroll += (_, _) =>
        {
            lblCooldownValue.Text = $"{trkCooldown.Value} sec";
        };

        btnTestSound.Click += (_, _) =>
        {
            // Debug: Check the file path and file existence
            string? filePath = _audioPlayer.SoundFilePath;

            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("No sound selected. Please select a sound in the main window first.",
                    "No Sound Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!File.Exists(filePath))
            {
                MessageBox.Show($"Audio file not found:\n{filePath}\n\nMake sure the file exists in C:\\Windows\\Media",
                    "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Play preview with current volume setting on background thread
            Task.Run(() => _audioPlayer.PlaySync(trkVolume.Value));
        };

        btnOK.Click += OkButton_Click;
    }

    private void OkButton_Click(object? sender, EventArgs e)
    {
        // Save settings
        _settings.VolumePercent = trkVolume.Value;
        _settings.PollIntervalMs = trkPollInterval.Value;
        _settings.CooldownSeconds = trkCooldown.Value;
        _settings.ThemeMode = radThemeDark.Checked ? ThemeMode.Dark : ThemeMode.Light;
        _settings.StartWithWindows = chkStartWithWindows.Checked;
        _settings.Save();

        // Update the Windows startup registry entry
        StartupManager.SetEnabled(chkStartWithWindows.Checked);

        this.DialogResult = DialogResult.OK;
        this.Close();
    }
}
