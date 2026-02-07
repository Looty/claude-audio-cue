#nullable enable

namespace ClaudeAudioCue;

partial class SettingsDialog
{
    private System.ComponentModel.IContainer? components;

    private Label lblTitle = null!;
    private Label lblVolume = null!;
    private TrackBar trkVolume = null!;
    private Label lblVolumeValue = null!;
    private Button btnTestSound = null!;

    private Label lblPollInterval = null!;
    private TrackBar trkPollInterval = null!;
    private Label lblPollIntervalValue = null!;

    private Label lblCooldown = null!;
    private TrackBar trkCooldown = null!;
    private Label lblCooldownValue = null!;

    private Label lblTheme = null!;
    private RadioButton radThemeLight = null!;
    private RadioButton radThemeDark = null!;

    private Label lblStartup = null!;
    private CheckBox chkStartWithWindows = null!;

    private Button btnOK = null!;
    private Button btnCancel = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();

        // Form settings (colors will be applied by ApplyTheme)
        this.Text = "Settings";
        this.Size = new Size(500, 540);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.ShowInTaskbar = false;

        // Title
        lblTitle = new Label
        {
            Text = "Audio Cue Settings",
            Font = new Font("Segoe UI", 13F, FontStyle.Bold),
            Location = new Point(16, 12),
            AutoSize = true
        };

        // ===== VOLUME SECTION =====
        lblVolume = new Label
        {
            Text = "Notification Volume",
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Location = new Point(16, 50),
            AutoSize = true
        };

        trkVolume = new TrackBar
        {
            Location = new Point(16, 75),
            Size = new Size(220, 45),
            Minimum = 0,
            Maximum = 200,
            Value = 100,
            TickFrequency = 20
        };

        lblVolumeValue = new Label
        {
            Text = "100%",
            Font = new Font("Segoe UI", 9F),
            Location = new Point(245, 80),
            Size = new Size(45, 20),
            TextAlign = ContentAlignment.MiddleRight
        };

        btnTestSound = new Button
        {
            Text = "Test",
            Location = new Point(300, 75),
            Size = new Size(70, 28),
            Font = new Font("Segoe UI", 9F),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnTestSound.FlatAppearance.BorderSize = 0;
        btnTestSound.FlatAppearance.MouseOverBackColor = Color.Empty;

        // ===== POLL INTERVAL SECTION =====
        lblPollInterval = new Label
        {
            Text = "Monitor Poll Interval",
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Location = new Point(16, 135),
            AutoSize = true
        };

        trkPollInterval = new TrackBar
        {
            Location = new Point(16, 160),
            Size = new Size(250, 45),
            Minimum = 100,
            Maximum = 2000,
            Value = 500,
            TickFrequency = 100
        };

        lblPollIntervalValue = new Label
        {
            Text = "500 ms",
            Font = new Font("Segoe UI", 9F),
            Location = new Point(280, 165),
            AutoSize = true
        };

        // ===== COOLDOWN SECTION =====
        lblCooldown = new Label
        {
            Text = "Minimum Cooldown Between Cues",
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Location = new Point(16, 220),
            AutoSize = true
        };

        trkCooldown = new TrackBar
        {
            Location = new Point(16, 245),
            Size = new Size(250, 45),
            Minimum = 1,
            Maximum = 30,
            Value = 3,
            TickFrequency = 1
        };

        lblCooldownValue = new Label
        {
            Text = "3 sec",
            Font = new Font("Segoe UI", 9F),
            Location = new Point(280, 250),
            AutoSize = true
        };

        // ===== THEME SECTION =====
        lblTheme = new Label
        {
            Text = "Theme",
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Location = new Point(16, 310),
            AutoSize = true
        };

        radThemeLight = new RadioButton
        {
            Text = "Light",
            Location = new Point(16, 335),
            AutoSize = true,
            Font = new Font("Segoe UI", 9F),
            Checked = true
        };

        radThemeDark = new RadioButton
        {
            Text = "Dark",
            Location = new Point(100, 335),
            AutoSize = true,
            Font = new Font("Segoe UI", 9F)
        };

        // ===== STARTUP SECTION =====
        lblStartup = new Label
        {
            Text = "Startup",
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Location = new Point(16, 375),
            AutoSize = true
        };

        chkStartWithWindows = new CheckBox
        {
            Text = "Start with Windows (minimized to tray)",
            Font = new Font("Segoe UI", 9F),
            Location = new Point(16, 400),
            AutoSize = true
        };

        // ===== BUTTONS =====
        btnOK = new Button
        {
            Text = "OK",
            Location = new Point(250, 450),
            Size = new Size(80, 32),
            Font = new Font("Segoe UI", 9F),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            DialogResult = DialogResult.OK
        };
        btnOK.FlatAppearance.BorderSize = 0;
        btnOK.FlatAppearance.MouseOverBackColor = Color.Empty;

        btnCancel = new Button
        {
            Text = "Cancel",
            Location = new Point(340, 450),
            Size = new Size(80, 32),
            Font = new Font("Segoe UI", 9F),
            FlatStyle = FlatStyle.Flat,
            DialogResult = DialogResult.Cancel,
            Cursor = Cursors.Hand
        };
        btnCancel.FlatAppearance.BorderSize = 0;
        btnCancel.FlatAppearance.MouseOverBackColor = Color.Empty;

        // Add controls to form
        this.Controls.AddRange(new Control[] {
            lblTitle,
            lblVolume,
            trkVolume,
            lblVolumeValue,
            btnTestSound,
            lblPollInterval,
            trkPollInterval,
            lblPollIntervalValue,
            lblCooldown,
            trkCooldown,
            lblCooldownValue,
            lblTheme,
            radThemeLight,
            radThemeDark,
            lblStartup,
            chkStartWithWindows,
            btnOK,
            btnCancel
        });

        this.AcceptButton = btnOK;
        this.CancelButton = btnCancel;
    }
}
