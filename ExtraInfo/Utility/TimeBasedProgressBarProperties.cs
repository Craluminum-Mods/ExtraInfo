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
}