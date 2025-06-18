namespace TownOfUsReworked.Patches.Cosmetics;

[HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.OnEnable))]
public static class PlayerTabPatches
{
    // private static bool VisorsActive;

    // private static PassiveButton SwapButton;
    // private static TextMeshPro ButtonText;
    // private static TextMeshPro Title;

    // private static void SwitchSelector(PlayerTab __instance)
    // {
    //     VisorsActive = !VisorsActive;
    //     __instance.currentColor = VisorsActive ? ReworkedDataManager.VisorColorId : DataManager.Player.Customization.Color;
    //     Title.text = (VisorsActive ? "Visor" : "Player") + " Color";
    //     ButtonText.text = (VisorsActive ? "Player" : "Visor") + " Color";
    // }

    [HarmonyPatch(nameof(PlayerTab.OnEnable)), HarmonyPostfix]
    public static void OnEnablePostfix(PlayerTab __instance)
    {
        // if (!SwapButton)
        // {
        //     Title = __instance.transform.FindChild("Text").GetComponent<TextMeshPro>();
        //     Title.GetComponent<TextTranslatorTMP>().Destroy();
        //     Title.text = (VisorsActive ? "Visor" : "Player") + " Color";

        //     SwapButton = UObject.Instantiate(PlayerCustomizationMenu.Instance.transform.FindRecursive("Equip").GetComponent<PassiveButton>(), __instance.ColorTabArea);
        //     SwapButton.transform.localScale = Vector3.one * 0.5f;
        //     SwapButton.transform.localPosition = new(4f, 1.5f, -2);
        //     SwapButton.OverrideOnClickListeners(() => SwitchSelector(__instance));
        //     SwapButton.gameObject.SetActive(true);

        //     ButtonText = SwapButton.GetComponentInChildren<TextMeshPro>();
        //     ButtonText.GetComponent<TextTranslatorTMP>().Destroy();
        //     ButtonText.text = (VisorsActive ? "Player" : "Visor") + " Color";
        // }

        var tab = PlayerCustomizationMenu.Instance.Tabs[1].Tab;

        if (!__instance.scroller && tab)
        {
            __instance.scroller = UObject.Instantiate(tab.scroller, __instance.transform, true);
            __instance.scroller.Inner.DestroyChildren();
            __instance.scroller.name = "Scroller";
            var track = UObject.Instantiate(tab.transform.Find("UI_ScrollbarTrack"), __instance.transform, true);
            track.name = "UI_ScrollbarTrack";
            var bar = UObject.Instantiate(tab.transform.Find("UI_Scrollbar").GetComponent<Scrollbar>(), __instance.transform, true);
            bar.parent = __instance.scroller;
            bar.trackGraphic = track.GetComponent<SpriteRenderer>();
            bar.name = "UI_Scrollbar";
            __instance.scroller.ScrollbarY = bar;
            __instance.scroller.ScrollbarYBounds = new(-1.45f, 1.28f);
        }

        var offset = __instance.YStart;

        for (var i = 0; i < __instance.ColorChips.Count; i++)
        {
            var colorChip = __instance.ColorChips._items[i];
            var xpos = __instance.XRange.min + (i % 5);
            var ypos = __instance.YStart - (i  / 5* __instance.YOffset);
            colorChip.transform.localPosition = new(xpos, ypos, -1f);
            colorChip.transform.SetParent(__instance.scroller.Inner);
            colorChip.Button.ClickMask = __instance.scroller.Hitbox;
            colorChip.transform.FindChild("ForeGround").GetComponent<SpriteMask>().enabled = false;
            colorChip.transform.GetAllComponents<SpriteRenderer>().Do(x => x.maskInteraction = SpriteMaskInteraction.VisibleInsideMask);
            offset = ypos;
        }

        __instance.scroller.ContentYBounds.max = -(offset + 1.25f);
        __instance.scroller.UpdateScrollBars();
    }

    [HarmonyPatch(nameof(PlayerTab.Update)), HarmonyPostfix]
    public static void UpdatePostfix(PlayerTab __instance)
    {
        for (var i = 0; i < __instance.ColorChips.Count; i++)
            __instance.ColorChips._items[i].Inner.SpriteColor = i.GetColor(false);

        // if (VisorsActive)
        //     __instance.currentColorIsEquipped = __instance.currentColor == ReworkedDataManager.VisorColorId;
    }

    [HarmonyPatch(nameof(PlayerTab.UpdateAvailableColors)), HarmonyPrefix]
    public static bool UpdateAvailableColorsPrefix(PlayerTab __instance)
    {
        __instance.AvailableColors.Clear();
        __instance.AvailableColors.AddRange(CustomColorManager.AllColors.Keys.Where(i => i != DataManager.Player.Customization.Color));
        return false;
    }

    // [HarmonyPatch(nameof(PlayerTab.GetCurrentColorId)), HarmonyPrefix]
    // public static bool GetCurrentColorPrefix(ref int __result)
    // {
    //     if (!VisorsActive)
    //         return true;

    //     __result = ReworkedDataManager.VisorColorId;
    //     return false;
    // }

    // [HarmonyPatch(nameof(PlayerTab.ClickEquip)), HarmonyPrefix]
    // public static bool ClickPrefix(PlayerTab __instance)
    // {
    //     if (VisorsActive && __instance.AvailableColors.Remove(__instance.currentColor))
    //     {
    //         ReworkedDataManager.VisorColorId = __instance.currentColor;
    //         __instance.PlayerPreview.UpdateFromDataManager(MaskType.None);
    //         return false;
    //     }

    //     return true;
    // }

    // [HarmonyPatch(nameof(PlayerTab.SelectColor)), HarmonyPrefix]
    // public static bool SelectPrefix(PlayerTab __instance, int colorId)
    // {
    //     if (!VisorsActive)
    //         return true;

    //     __instance.UpdateAvailableColors();
    //     __instance.currentColor = colorId;
    //     PlayerCustomizationMenu.Instance.SetItemName(Palette.GetColorName(colorId));
    //     __instance.PlayerPreview.UpdateFromDataManager(MaskType.None);
    //     return false;
    // }
}

[HarmonyPatch(typeof(PlayerMaterial))]
public static class SetPlayerMaterialPatch1
{
    [HarmonyPatch(nameof(PlayerMaterial.SetColors), typeof(int), typeof(Renderer))]
    public static void Prefix(int colorId, Renderer rend) => Colors.Instance.SetRend(rend, colorId);

    [HarmonyPatch(nameof(PlayerMaterial.SetColors), typeof(UColor), typeof(Renderer))]
    public static void Prefix(Renderer rend, UColor color) => Colors.Instance.SetRend(rend, color);
}

[HarmonyPatch(typeof(RoleEffectAnimation), nameof(RoleEffectAnimation.SetMaterialColor))]
public static class SetPlayerMaterialPatch2
{
    public static void Prefix(RoleEffectAnimation __instance, int colorId) => Colors.Instance.SetRend(__instance.Renderer, colorId);
}

// [HarmonyPatch(typeof(Material), nameof(Material.SetColor), typeof(int), typeof(UColor))]
// public static class VisorColorPatches
// {
//     public static void Prefix(int nameID, ref UColor value)
//     {
//         if (nameID == PlayerMaterial.VisorColor && value == Palette.VisorColor)
//             value = ReworkedDataManager.VisorColorId.GetColor(false);
//     }
// }