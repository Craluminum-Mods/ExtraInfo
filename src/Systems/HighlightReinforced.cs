using System;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace ExtraInfo;

public class HighlightReinforced : ModSystemHighlight
{
    public override string ThreadName => "ExtraInfo:Reinforcements";

    public override string Name => Lang.Get("extrainfo:HighlightReinforcedBlocks");
    public override string HotkeyCode => "extrainfo:highlightreinforced";

    public int Radius => 10;

    public int HighlightColor => Constants.ColorsRGBA.Cyan;

    public ModSystemBlockReinforcement ModSysBlockReinforcement { get; protected set; }

    public override void StartClientSide(ICoreClientAPI api)
    {
        base.StartClientSide(api);
        api.Input.RegisterHotKey(HotkeyCode, Constants.ToggleName(Name), GlKeys.T, HotkeyType.HelpAndOverlays, ctrlPressed: true);
        api.Input.SetHotKeyHandler(HotkeyCode, _ => ToggleRun(api));

        ModSysBlockReinforcement = api.ModLoader.GetModSystem<ModSystemBlockReinforcement>();
    }

    public override void OnRunning(ICoreClientAPI capi)
    {
        List<BlockPos> positions = new();
        List<int> colors = new();
        var playerPos = capi.World.Player.Entity.Pos.AsBlockPos;

        capi.World.BlockAccessor.WalkBlocks(playerPos.AddCopy(-Radius, -Radius, -Radius), playerPos.AddCopy(Radius, Radius, Radius), (_, x, y, z) =>
        {
            var bPos = new BlockPos(x, y, z);
            if (IsReinforced(bPos))
            {
                positions.Add(bPos);
                colors.Add(HighlightColor);
            }
        });

        capi.Event.EnqueueMainThreadTask(new Action(() => capi.World.HighlightBlocks(capi.World.Player, 5229, positions, colors)), ThreadName);
    }

    private bool IsReinforced(BlockPos pos) => ModSysBlockReinforcement.IsReinforced(pos);
}
