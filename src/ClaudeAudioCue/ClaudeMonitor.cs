namespace ClaudeAudioCue;

public enum MonitorStatus
{
    Idle,
    Searching,
    Monitoring,
    StreamingDetected,
    StreamingEnded,
    Error
}

public class ClaudeMonitor : IDisposable
{
    private Thread? _thread;
    private volatile bool _running;
    private readonly IProgress<MonitorStatus> _progress;
    private readonly AudioPlayer _audioPlayer;
    private readonly int _pollIntervalMs;
    private readonly int _volumePercent;
    private readonly int _cooldownSeconds;
    private DateTime _lastAudioTime = DateTime.MinValue;
    private DateTime _streamingStartTime = DateTime.MinValue;
    private TimeSpan _lastResponseDuration = TimeSpan.Zero;

    public bool IsRunning => _running;
    public TimeSpan LastResponseDuration => _lastResponseDuration;

    public ClaudeMonitor(IProgress<MonitorStatus> progress, AudioPlayer audioPlayer, int pollIntervalMs = 500, int volumePercent = 100, int cooldownSeconds = 3)
    {
        _progress = progress;
        _audioPlayer = audioPlayer;
        _pollIntervalMs = pollIntervalMs;
        _volumePercent = volumePercent;
        _cooldownSeconds = cooldownSeconds;
    }

    public void Start()
    {
        if (_running)
            return;

        _running = true;
        _thread = new Thread(MonitorLoop)
        {
            IsBackground = true,
            Name = "ClaudeMonitorThread"
        };
        _thread.SetApartmentState(ApartmentState.STA); // Required for UIA3 COM
        _thread.Start();
    }

    public void Stop()
    {
        _running = false;
        _thread?.Join(timeout: TimeSpan.FromSeconds(3));
        _thread = null;
        _progress.Report(MonitorStatus.Idle);
    }

    private void MonitorLoop()
    {
        using var detector = new StreamingDetector();
        bool wasStreaming = false;

        _progress.Report(MonitorStatus.Searching);

        while (_running)
        {
            try
            {
                // Find Claude window if not already found
                if (!detector.IsWindowValid())
                {
                    wasStreaming = false;
                    _progress.Report(MonitorStatus.Searching);

                    if (!detector.FindClaudeWindow())
                    {
                        Thread.Sleep(1000); // Slower polling when searching
                        continue;
                    }

                    _progress.Report(MonitorStatus.Monitoring);
                }

                // Check streaming state
                bool isStreaming = detector.IsStreaming();

                if (isStreaming && !wasStreaming)
                {
                    // Streaming just started
                    _streamingStartTime = DateTime.UtcNow;
                    _progress.Report(MonitorStatus.StreamingDetected);
                }
                else if (!isStreaming && wasStreaming)
                {
                    // Streaming just ended — calculate response duration
                    _lastResponseDuration = DateTime.UtcNow.Subtract(_streamingStartTime);
                    _progress.Report(MonitorStatus.StreamingEnded);

                    // Only play audio if cooldown period has elapsed
                    if (DateTime.UtcNow.Subtract(_lastAudioTime).TotalSeconds >= _cooldownSeconds)
                    {
                        _audioPlayer.Play(_volumePercent);
                        _lastAudioTime = DateTime.UtcNow;
                    }

                    // Brief delay, then report back to monitoring
                    Thread.Sleep(500);
                    _progress.Report(MonitorStatus.Monitoring);
                }

                wasStreaming = isStreaming;
                Thread.Sleep(_pollIntervalMs);
            }
            catch (Exception)
            {
                _progress.Report(MonitorStatus.Error);
                Thread.Sleep(2000); // Back off on errors
            }
        }

        _progress.Report(MonitorStatus.Idle);
    }

    public void Dispose()
    {
        Stop();
        GC.SuppressFinalize(this);
    }
}
