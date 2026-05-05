namespace ExtraInfo;

public class TimeBasedProgressBarProperties
{
    /// <summary>
    /// Words above the progress bar, for example:  <br/>
    /// [HeaderKey]  <br/>
    /// [ProgressBar]
    /// </summary>
    public string HeaderKey = "";

    /// <summary>
    /// Used as 100% percent
    /// </summary>
    public double HoursTotal;

    /// <summary>
    /// Used together with <see cref="HoursTotal"/> to show progress percent
    /// </summary>
    public double HoursLeft;

    /// <summary>
    /// If true, skips speed and calendar multipliers and treats values as 1:1 seconds.
    /// </summary>
    public bool IsRawSeconds = false;

    /// <summary>
    /// If false, skips progress bar
    /// </summary>
    public bool ShowProgressBar = true;

    /// <summary>
    /// If false, skips in-game time
    /// </summary>
    public bool ShowInGameTime = true;

    /// <summary>
    /// If false, skips real time
    /// </summary>
    public bool ShowRealTime = true;
}