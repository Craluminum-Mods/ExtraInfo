namespace ExtraInfo;

[HarmonyPatchCategory("FixVTMLInBlockTooltip")]
[HarmonyPatch(typeof(HudElementBlockAndEntityInfo), "ComposeBlockInfoHud")]
public static class ComposeBlockInfoHudPatch
{
    [HarmonyPrefix]
    public static bool Prefix(HudElementBlockAndEntityInfo __instance, ref Block ___currentBlock, ref int ___currentSelectionIndex, ref Entity ___currentEntity, ref BlockPos ___currentPos, ref string ___title, ref string ___detail, ref GuiComposer ___composer, ref ICoreClientAPI ___capi)
    {
        if (Core.Config == null || !Core.Config.FixVTMLInBlockTooltip)
        {
            return true;
        }

        string newTitle = "";
        string newDetail = "";
        if (___currentBlock != null)
        {
            if (___currentBlock.Code == null)
            {
                newTitle = "Unknown block ID " + ___capi.World.BlockAccessor.GetBlockId(___currentPos);
                newDetail = "";
            }
            else
            {
                newTitle = ___currentBlock.GetPlacedBlockName(___capi.World, ___currentPos);
                newDetail = ___currentBlock.GetPlacedBlockInfo(___capi.World, ___currentPos, ___capi.World.Player);
                if (newDetail == null)
                {
                    newDetail = "";
                }
                if (newTitle == null)
                {
                    newTitle = Lang.Get("Unknown");
                }
            }
        }
        if (___currentEntity != null)
        {
            newTitle = ___currentEntity.GetName();
            newDetail = ___currentEntity.GetInfoText();
            if (newDetail == null)
            {
                newDetail = "";
            }
            if (newTitle == null)
            {
                newTitle = "Unknown Entity code " + ___currentEntity.Code;
            }
        }
        if (!(___title == newTitle) || !(___detail == newDetail))
        {
            ___title = newTitle;
            ___detail = newDetail;
            ElementBounds textBounds = ElementBounds.Fixed(EnumDialogArea.CenterFixed, 0, 0, 500, 24);
            ElementBounds detailTextBounds = textBounds.BelowCopy(0, 10);
            detailTextBounds.Alignment = EnumDialogArea.None;
            ElementBounds overlayBounds = new ElementBounds();
            overlayBounds.BothSizing = ElementSizing.FitToChildren;
            overlayBounds.WithFixedPadding(5.0, 5.0);
            ElementBounds dialogBounds = ElementStdBounds.AutosizedMainDialog.WithAlignment(EnumDialogArea.CenterTop).WithFixedAlignmentOffset(0.0, GuiStyle.DialogToScreenPadding);
            LoadedTexture reuseRichTextTexture = null;
            GuiElementRichtext rtElem;
            if (___composer == null)
            {
                ___composer = ___capi.Gui.CreateCompo("blockinfohud", dialogBounds);
            }
            else
            {
                rtElem = ___composer.GetRichtext("rt");
                reuseRichTextTexture = rtElem.richtTextTexture;
                rtElem.richtTextTexture = null;
                ___composer.Clear(dialogBounds);
            }
            __instance.Composers["blockinfohud"] = ___composer;
            ___composer.AddGameOverlay(overlayBounds).BeginChildElements(overlayBounds);


            GuiElementRichtext element = new(___composer.Api, VtmlUtil.Richtextify(___composer.Api, ___title, CairoFont.WhiteSmallishText()), textBounds);
            ___composer.AddInteractiveElement(element);
            element.CalcHeightAndPositions();
            ___composer.AddRichtext(___title, CairoFont.WhiteSmallishText(), textBounds);


            ___composer.AddRichtext(___detail, CairoFont.WhiteDetailText(), detailTextBounds, "rt");
                ___composer.EndChildElements();
            rtElem = ___composer.GetRichtext("rt");
            if (___detail.Length == 0)
            {
                detailTextBounds.fixedY = 0.0;
                detailTextBounds.fixedHeight = 0.0;
            }
            if (reuseRichTextTexture != null)
            {
                rtElem.richtTextTexture = reuseRichTextTexture;
            }
            rtElem.BeforeCalcBounds();
            detailTextBounds.fixedWidth = Math.Min(500.0, rtElem.MaxLineWidth / (double)RuntimeEnv.GUIScale + 1.0);


            textBounds.fixedWidth = Math.Min(500.0, element.MaxLineWidth / (double)RuntimeEnv.GUIScale + 1.0);


            ___composer.Compose();
        }

        return false;
    }
}