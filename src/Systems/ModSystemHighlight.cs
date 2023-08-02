using System;
using System.Collections.Generic;
using System.Threading;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace ExtraInfo;

public class ModSystemHighlight : ModSystem, IThreadHighlight
{
    public bool Enabled { get; set; }
    public Thread OpThread { get; set; }
    public virtual string ThreadName { get; }

    public virtual string Name { get; }
    public virtual string HotkeyCode { get; }

    public bool ToggleRun(ICoreClientAPI capi)
    {
        if (!Enabled)
        {
            Enabled = true;
            OpThread = new Thread(() => Run(capi))
            {
                IsBackground = true,
                Name = ThreadName
            };
            OpThread.Start();
        }
        else
        {
            Enabled = false;
        }

        capi.TriggerChatMessage(Constants.StringToggle(Enabled, Name));

        return true;
    }

    protected void Run(ICoreClientAPI capi)
    {
        while (Enabled)
        {
            Thread.Sleep(100);
            try
            {
                OnRunning(capi);
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
                break;
            }
            catch { }
        }
        ClearHighlights(capi);
    }

    public virtual void OnRunning(ICoreClientAPI capi)
    {
    }

    private void ClearHighlights(ICoreClientAPI capi)
    {
        capi.Event.EnqueueMainThreadTask(new Action(() => capi.World.HighlightBlocks(capi.World.Player, 5229, new List<BlockPos>())), ThreadName);
    }
}