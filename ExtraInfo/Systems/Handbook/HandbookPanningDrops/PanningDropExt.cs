using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;

namespace ExtraInfo.Systems.Handbook.HandbookPanningDrops;

public class PanningDropExt : JsonItemStack
{
    public NatFloat Chance;
    public string DropModbyStat;
    public bool ManMade;

    public PanningDropExt() { }

    public static PanningDropExt FromOriginal(PanningDrop original)
    {
        PanningDropExt ext = new PanningDropExt();
        
        ext.Type = original.Type;
        ext.Code = original.Code?.Clone();
        ext.StackSize = original.StackSize;
        ext.Attributes = original.Attributes?.Clone();
        ext.ResolvedItemStack = original.ResolvedItemStack?.Clone();

        ext.Chance = original.Chance?.Clone();
        ext.DropModbyStat = original.DropModbyStat;
        ext.ManMade = original.ManMade;

        return ext;
    }

    public override PanningDropExt Clone()
    {
        PanningDropExt cloned = new PanningDropExt();
        this.CloneTo(cloned);
        return cloned;
    }

    protected override void CloneTo(object stack)
    {
        base.CloneTo(stack); 
        if (stack is PanningDropExt pDropExt)
        {
            pDropExt.Chance = this.Chance?.Clone();
            pDropExt.DropModbyStat = this.DropModbyStat;
            pDropExt.ManMade = this.ManMade;
        }
    }
}