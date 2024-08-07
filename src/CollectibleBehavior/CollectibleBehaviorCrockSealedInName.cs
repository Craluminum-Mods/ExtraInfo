namespace ExtraInfo;

public class CollectibleBehaviorCrockSealedInName : CollectibleBehavior
{
    public CollectibleBehaviorCrockSealedInName(CollectibleObject collObj) : base(collObj) { }

    public override void GetHeldItemName(StringBuilder sb, ItemStack itemStack)
    {
        StringBuilder dsc = new();

        if (itemStack.Attributes.GetBool(Constants.Text.SealedAttr))
        {
            dsc.Append(Constants.Text.SealedText);
            dsc.Append(' ');
        }

        sb.Insert(0, dsc);
    }
}