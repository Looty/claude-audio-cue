#nullable enable

namespace ClaudeAudioCue;

partial class MainForm
{
    private System.ComponentModel.IContainer? components;

    // These fields are initialized in InitializeComponent(), called from the constructor.
    private Label lblTitle = null!;
    private Label lblStatus = null!;
    private Label lblStatusValue = null!;
    private Label lblSound = null!;
    private ComboBox cboSound = null!;
    private Button btnToggle = null!;
    private Button btnTest = null!;
    private Button btnSettings = null!;
    private NotifyIcon trayIcon = null!;
    private ContextMenuStrip trayMenu = null!;
    private ToolStripMenuItem trayMenuShow = null!;
    private ToolStripMenuItem trayMenuToggle = null!;
    private ToolStripSeparator trayMenuSep = null!;
    private ToolStripMenuItem trayMenuExit = null!;

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

        // Form settings - modern styling (colors applied by ApplyTheme)
        this.Text = "Claude Audio Cue";
        this.Size = new Size(420, 310);
        this.MinimumSize = new Size(420, 310);
        this.MaximizeBox = false;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.ShowInTaskbar = true;

        // Title
        lblTitle = new Label
        {
            Text = "Claude Audio Cue",
            Font = new Font("Segoe UI", 13F, FontStyle.Bold),
            Location = new Point(20, 16),
            AutoSize = true
        };

        // Status label
        lblStatus = new Label
        {
            Text = "Status:",
            Font = new Font("Segoe UI", 9F),
            Location = new Point(20, 55),
            AutoSize = true
        };

        // Status value
        lblStatusValue = new Label
        {
            Text = "Idle",
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Location = new Point(75, 55),
            AutoSize = true
        };

        // Sound label
        lblSound = new Label
        {
            Text = "Notification Sound:",
            Font = new Font("Segoe UI", 9F),
            Location = new Point(20, 95),
            AutoSize = true
        };

        // Sound combo box - modern styling
        cboSound = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(20, 118),
            Size = new Size(280, 24),
            Font = new Font("Segoe UI", 9F)
        };

        // Test button - flat modern style (below combo box)
        btnTest = new Button
        {
            Text = "Preview Sound",
            Location = new Point(20, 155),
            Size = new Size(280, 32),
            Font = new Font("Segoe UI", 9F),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnTest.FlatAppearance.BorderSize = 0;
        btnTest.FlatAppearance.MouseOverBackColor = Color.Empty;

        // Settings button
        btnSettings = new Button
        {
            Text = "⚙ Settings",
            Location = new Point(20, 210),
            Size = new Size(110, 36),
            Font = new Font("Segoe UI", 9F),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnSettings.FlatAppearance.BorderSize = 0;
        btnSettings.FlatAppearance.MouseOverBackColor = Color.Empty;

        // Toggle button - flat modern style
        btnToggle = new Button
        {
            Text = "Stop Monitoring",
            Location = new Point(140, 210),
            Size = new Size(250, 36),
            Font = new Font("Segoe UI", 9F),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnToggle.FlatAppearance.BorderSize = 0;
        btnToggle.FlatAppearance.MouseOverBackColor = Color.Empty;

        // Tray context menu
        trayMenuShow = new ToolStripMenuItem("Show");
        trayMenuToggle = new ToolStripMenuItem("Stop Monitoring");
        trayMenuSep = new ToolStripSeparator();
        trayMenuExit = new ToolStripMenuItem("Exit");

        trayMenu = new ContextMenuStrip(components);
        trayMenu.Items.AddRange(new ToolStripItem[] {
            trayMenuShow,
            trayMenuToggle,
            trayMenuSep,
            trayMenuExit
        });

        // Tray icon
        trayIcon = new NotifyIcon(components)
        {
            Text = "Claude Audio Cue",
            ContextMenuStrip = trayMenu,
            Visible = false
        };

        // Set a default icon (use system app icon)
        try
        {
            string? iconPath = Path.Combine(AppContext.BaseDirectory, "Resources", "app-icon.ico");
            if (File.Exists(iconPath))
            {
                trayIcon.Icon = new Icon(iconPath);
                this.Icon = new Icon(iconPath);
            }
            else
            {
                // Use default application icon
                trayIcon.Icon = SystemIcons.Application;
                this.Icon = SystemIcons.Application;
            }
        }
        catch
        {
            trayIcon.Icon = SystemIcons.Application;
            this.Icon = SystemIcons.Application;
        }

        // Add controls to form
        this.Controls.Add(lblTitle);
        this.Controls.Add(lblStatus);
        this.Controls.Add(lblStatusValue);
        this.Controls.Add(lblSound);
        this.Controls.Add(cboSound);
        this.Controls.Add(btnTest);
        this.Controls.Add(btnSettings);
        this.Controls.Add(btnToggle);
    }
}
