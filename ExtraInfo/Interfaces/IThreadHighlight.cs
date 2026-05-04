using System.Threading;

namespace ExtraInfo;

public interface IThreadHighlight
{
    bool Enabled { get; }
    Thread OpThread { get; }
    string ThreadName { get; }
}