namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.OnEnable))]
public static class PlayerTabPatches
{
    [HarmonyPatch(nameof(PlayerTab.OnEnable)), HarmonyPostfix]
    public static void OnEnablePostfix(PlayerTab __instance)
    {
        if (!PlayerCustomizationMenu.Instance)
            return;

        var tab = PlayerCustomizationMenu.Instance.Tabs[1].Tab;

        if (!__instance.scroller && tab)
        {
            __instance.scroller = UObject.Instantiate(tab.scroller, __instance.transform, true);
            __instance.scroller.Inner.DestroyChildren();
            __instance.scroller.name = "Scroller";
        }

        for (var i = 0; i < __instance.ColorChips.Count; i++)
        {
            var colorChip = __instance.ColorChips[i];
            var xpos = __instance.XRange.min + (i % 5);
            var ypos = __instance.YStart - (i / 5 * __instance.YOffset);
            colorChip.transform.localPosition = new(xpos, ypos, -1f);
            colorChip.transform.SetParent(__instance.scroller.Inner);
            colorChip.Button.ClickMask = __instance.scroller.Hitbox;
            colorChip.transform.FindChild("ForeGround").GetComponent<SpriteMask>().Destroy();
            colorChip.transform.GetAllComponents<SpriteRenderer>().ForEach(x => x.maskInteraction = SpriteMaskInteraction.VisibleInsideMask);
        }

        __instance.SetScrollerBounds();
    }

    private static float TimePassed;
    private static bool Shadow;

    [HarmonyPatch(nameof(PlayerTab.Update)), HarmonyPostfix]
    public static void UpdatePostfix(PlayerTab __instance)
    {
        TimePassed += Time.deltaTime;

        if (TimePassed > 2f)
        {
            TimePassed = 0f;
            Shadow = !Shadow;
        }

        for (var i = 0; i < __instance.ColorChips.Count; i++)
            __instance.ColorChips[i].Inner.SpriteColor = i.GetColor(Shadow);
    }

    [HarmonyPatch(nameof(PlayerTab.UpdateAvailableColors))]
    public static bool Prefix(PlayerTab __instance)
    {
        __instance.AvailableColors.Clear();

        for (var i = 0; i < Palette.PlayerColors.Count; i++)
        {
            if (DataManager.Player.Customization.Color != i)
                __instance.AvailableColors.Add(i);
        }

        return false;
    }
}

[HarmonyPatch(typeof(PlayerMaterial))]
public static class SetPlayerMaterialPatch1
{
    [HarmonyPatch(nameof(PlayerMaterial.SetColors), typeof(int), typeof(Renderer))]
    public static void Prefix(int colorId, Renderer rend) => ColorHandler.Instance.SetRend(rend, colorId);

    [HarmonyPatch(nameof(PlayerMaterial.SetColors), typeof(UColor), typeof(Renderer))]
    public static void Prefix(Renderer rend, UColor color) => ColorHandler.Instance.SetRend(rend, color);
}

[HarmonyPatch(typeof(RoleEffectAnimation), nameof(RoleEffectAnimation.SetMaterialColor))]
public static class SetPlayerMaterialPatch2
{
    public static void Prefix(RoleEffectAnimation __instance, int colorId) => ColorHandler.Instance.SetRend(__instance.Renderer, colorId);
}