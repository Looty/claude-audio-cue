using System.Text;
using System.Text.RegularExpressions;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;

namespace ClaudeAudioCue;

public class StreamingDetector : IDisposable
{
    private readonly UIA3Automation _automation;
    private AutomationElement? _claudeWindow;

    // Window title patterns to search for
    private static readonly string[] WindowTitles = ["Claude", "Anthropic Claude", "Claude Desktop"];

    // Button name patterns that indicate streaming is active.
    // These buttons only appear while Claude is generating a response.
    // Covers both regular chat (Stop/Pause) and Claude Code (Interrupt/Cancel).
    private static readonly Regex StreamingButtonPattern = new(
        @"stop|pause|cancel\s*generat|stop\s*generat|abort|interrupt",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public StreamingDetector()
    {
        _automation = new UIA3Automation();
    }

    /// <summary>
    /// Find the Claude Desktop window. Returns true if found.
    /// </summary>
    public bool FindClaudeWindow()
    {
        try
        {
            var desktop = _automation.GetDesktop();
            var cf = _automation.ConditionFactory;

            // Try exact title matches first
            foreach (var title in WindowTitles)
            {
                var window = desktop.FindFirstChild(cf.ByName(title));
                if (window != null)
                {
                    _claudeWindow = window;
                    return true;
                }
            }

            // Fallback: any top-level window containing "Claude" in the name
            var allWindows = desktop.FindAllChildren();
            foreach (var window in allWindows)
            {
                try
                {
                    string name = window.Name ?? "";
                    if (name.Contains("Claude", StringComparison.OrdinalIgnoreCase))
                    {
                        _claudeWindow = window;
                        return true;
                    }
                }
                catch
                {
                    // Some windows may not be accessible
                }
            }

            _claudeWindow = null;
            return false;
        }
        catch
        {
            _claudeWindow = null;
            return false;
        }
    }

    /// <summary>
    /// Check if the cached window handle is still valid.
    /// </summary>
    public bool IsWindowValid()
    {
        if (_claudeWindow == null)
            return false;

        try
        {
            // Accessing Name will throw if the window is gone
            _ = _claudeWindow.Name;
            return true;
        }
        catch
        {
            _claudeWindow = null;
            return false;
        }
    }

    /// <summary>
    /// Returns true if Claude is currently streaming (generating a response).
    /// Checks for stop/pause/interrupt buttons that only appear during streaming.
    /// </summary>
    public bool IsStreaming()
    {
        if (_claudeWindow == null)
            return false;

        try
        {
            var cf = _automation.ConditionFactory;

            // Strategy 1: Check all Button elements for streaming-related names
            var buttons = _claudeWindow.FindAllDescendants(cf.ByControlType(ControlType.Button));
            foreach (var button in buttons)
            {
                try
                {
                    string name = button.Name ?? "";
                    if (!string.IsNullOrEmpty(name) && StreamingButtonPattern.IsMatch(name))
                        return true;
                }
                catch
                {
                    // Skip inaccessible elements
                }
            }

            // Strategy 2: Check Custom controls (Electron/React may render non-standard controls)
            var customs = _claudeWindow.FindAllDescendants(cf.ByControlType(ControlType.Custom));
            foreach (var custom in customs)
            {
                try
                {
                    string name = custom.Name ?? "";
                    if (!string.IsNullOrEmpty(name) && StreamingButtonPattern.IsMatch(name))
                        return true;
                }
                catch
                {
                    // Skip inaccessible elements
                }
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Get the name of the currently cached Claude window.
    /// </summary>
    public string? GetWindowName()
    {
        try
        {
            return _claudeWindow?.Name;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Dump all UI elements from the Claude window for debugging.
    /// Returns a formatted string of the element tree.
    /// </summary>
    public string DumpElements(int maxDepth = 6)
    {
        if (_claudeWindow == null)
            return "No Claude window found.";

        var sb = new StringBuilder();
        sb.AppendLine($"=== UI Element Dump: '{_claudeWindow.Name}' ===");
        sb.AppendLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine();

        DumpElementRecursive(sb, _claudeWindow, 0, maxDepth);

        sb.AppendLine("=== End Dump ===");
        return sb.ToString();
    }

    private static void DumpElementRecursive(StringBuilder sb, AutomationElement element, int depth, int maxDepth)
    {
        if (depth > maxDepth)
            return;

        try
        {
            string indent = new(' ', depth * 2);
            string name = element.Name ?? "";
            string autoId = element.AutomationId ?? "";
            string className = element.ClassName ?? "";
            string controlType = element.ControlType.ToString();

            if (!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(autoId) || !string.IsNullOrEmpty(className))
            {
                sb.AppendLine($"{indent}[{controlType}] Name='{name}' AutomationId='{autoId}' Class='{className}'");
            }

            var children = element.FindAllChildren();
            foreach (var child in children)
            {
                DumpElementRecursive(sb, child, depth + 1, maxDepth);
            }
        }
        catch
        {
            // Skip inaccessible elements
        }
    }

    public void Dispose()
    {
        _automation.Dispose();
        GC.SuppressFinalize(this);
    }
}
