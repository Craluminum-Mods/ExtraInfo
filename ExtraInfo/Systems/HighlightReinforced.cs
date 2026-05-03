namespace ExtraInfo;

public class HighlightReinforced : ModSystemHighlight
{
    public override string ThreadName => "ExtraInfo:Reinforcements";

    public override string Name => Lang.Get("extrainfo:HighlightReinforcedBlocks");
    public override string HotkeyCode => "extrainfo:highlightreinforced";

    public static int Radius => 10;

    public static int HighlightColor => ColorsRGBA.Cyan;

    public ModSystemBlockReinforcement ModSysBlockReinforcement { get; protected set; }

    public override void StartClientSide(ICoreClientAPI api)
    {
        if (Core.Config == null || !Core.Config.HighlightReinforcedBlocks)
        {
            return;
        }

        api.Input.RegisterHotKey(HotkeyCode, ToggleName(Name), GlKeys.T, HotkeyType.HelpAndOverlays, ctrlPressed: true);
        api.Input.SetHotKeyHandler(HotkeyCode, _ => ToggleRun(api));

        ModSysBlockReinforcement = api.ModLoader.GetModSystem<ModSystemBlockReinforcement>();
    }

    public override void OnRunning(ICoreClientAPI capi)
    {
        List<BlockPos> positions = new();
        List<int> colors = new();
        BlockPos playerPos = capi.World.Player.Entity.Pos.AsBlockPos;

        capi.World.BlockAccessor.WalkBlocks(playerPos.AddCopy(-Radius, -Radius, -Radius), playerPos.AddCopy(Radius, Radius, Radius), (_, x, y, z) =>
        {
            BlockPos bPos = new(x, y, z, playerPos.dimension);
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
