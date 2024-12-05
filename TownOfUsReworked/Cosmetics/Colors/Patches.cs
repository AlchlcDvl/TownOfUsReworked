// using static TownOfUsReworked.Cosmetics.CustomColors.CustomColorManager;

namespace TownOfUsReworked.Cosmetics.CustomColors;

[HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.OnEnable))]
public static class PlayerTabOnEnablePatch
{
    // private static TMP_Text Template;

    // private static void CreateColorPackage(List<CustomColor> colors, string packageName, ref float offset, PlayerTab __instance)
    // {
    //     if (Template)
    //     {
    //         var title = UObject.Instantiate(Template, __instance.ColorTabArea);
    //         title.transform.localPosition = new(2.5f, offset, -1f);
    //         title.transform.localScale = Vector3.one * 1.5f;
    //         title.fontSize *= 0.5f;
    //         title.enableAutoSizing = false;
    //         Coroutines.Start(PerformTimedAction(0.1f, _ => title.SetText(packageName)));
    //         title.transform.SetParent(__instance.scroller.Inner, true);
    //         offset -= __instance.YOffset;
    //     }

    //     for (var i = 0; i < colors.Count; i++)
    //     {
    //         var color = colors[i];
    //         var xpos = __instance.XRange.min + (i % 5);
    //         var ypos = offset - (i / 5 * __instance.YOffset);
    //         var colorChip = UObject.Instantiate(__instance.ColorTabPrefab, __instance.ColorTabArea);

    //         if (ActiveInputManager.currentControlType == ActiveInputManager.InputType.Keyboard)
    //         {
    //             colorChip.Button.OverrideOnMouseOverListeners(() => __instance.SelectColor(color.ColorID));
    //             colorChip.Button.OverrideOnMouseOutListeners(() => __instance.SelectColor(DataManager.Player.Customization.Color));
    //             colorChip.Button.OverrideOnClickListeners(__instance.ClickEquip);
    //         }
    //         else
    //             colorChip.Button.OverrideOnClickListeners(() => __instance.SelectColor(color.ColorID));

    //         colorChip.Inner.SpriteColor = color.GetColor();
    //         colorChip.transform.localPosition = new(xpos, ypos, 2f);
    //         colorChip.Tag = color.ColorID;
    //         colorChip.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
    //         colorChip.transform.SetParent(__instance.scroller.Inner);
    //         __instance.ColorChips.Add(colorChip);

    //         if (i == colors.Count - 1)
    //             offset = ypos;
    //     }

    //     offset -= 1f;
    // }

    // public static bool Prefix(PlayerTab __instance)
    // {
    //     __instance.PlayerPreview.gameObject.SetActive(true);

    //     if (__instance.HasLocalPlayer())
    //         __instance.PlayerPreview.UpdateFromLocalPlayer(PlayerMaterial.MaskType.None);
    //     else
    //         __instance.PlayerPreview.UpdateFromDataManager(PlayerMaterial.MaskType.None);

    //     var tab = PlayerCustomizationMenu.Instance.Tabs[1].Tab;

    //     if (!__instance.scroller)
    //     {
    //         __instance.scroller = UObject.Instantiate(tab.scroller, __instance.transform, true);
    //         __instance.scroller.Inner.DestroyChildren();
    //         __instance.scroller.name = "Scroller";
    //         var track = UObject.Instantiate(tab.transform.Find("UI_ScrollbarTrack"), __instance.transform, true);
    //         track.name = "UI_ScrollbarTrack";
    //         var bar = UObject.Instantiate(tab.transform.Find("UI_Scrollbar").GetComponent<Scrollbar>(), __instance.transform, true);
    //         bar.parent = __instance.scroller;
    //         bar.trackGraphic = track.GetComponent<SpriteRenderer>();
    //         bar.name = "UI_Scrollbar";
    //         __instance.scroller.ScrollbarY = bar;
    //     }

    //     __instance.ColorChips = new();

    //     foreach (var (i, color) in AllColors)
    //     {
    //         var num2 = __instance.XRange.min + (i % 5);
    //         var num3 = __instance.YStart - (i / 5 * __instance.YOffset);
    //         var colorChip = UObject.Instantiate(__instance.ColorTabPrefab, __instance.ColorTabArea);

    //         if (ActiveInputManager.currentControlType == ActiveInputManager.InputType.Keyboard)
    //         {
    //             colorChip.Button.OverrideOnMouseOverListeners(() => __instance.SelectColor(color.ColorID));
    //             colorChip.Button.OverrideOnMouseOutListeners(() => __instance.SelectColor(DataManager.Player.Customization.Color));
    //             colorChip.Button.OverrideOnClickListeners(__instance.ClickEquip);
    //         }
    //         else
    //             colorChip.Button.OverrideOnClickListeners(() => __instance.SelectColor(color.ColorID));

    //         colorChip.Tag = color.ColorID;
    //         colorChip.transform.localPosition = new(num2, num3, 2f);
    //         colorChip.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
    //         colorChip.transform.SetParent(__instance.scroller.Inner);
    //         colorChip.name = color.Name;
    //         __instance.ColorChips.Add(colorChip);
    //     }

    //     var packages = new Dictionary<string, List<CustomColor>>();

    //     foreach (var data in AllColors.Values)
    //     {
    //         var package = data.Title;

    //         if (IsNullEmptyOrWhiteSpace(package))
    //             package = "Misc";

    //         if (!packages.TryGetValue(package, out var value))
    //             packages[package] = value = [];

    //         value.Add(data);
    //     }

    //     var offset = __instance.YStart - 0.3f;
    //     Template = __instance.transform.FindChild("Text").GetComponent<TMP_Text>();
    //     var keys = packages.Keys.OrderBy(x => x switch
    //     {
    //         "Stream" => 1,
    //         "Custom" => 2,
    //         "Contrasting" => 3,
    //         "Changing" => 4,
    //         _ => 5
    //     });
    //     keys.ForEach(key => CreateColorPackage(packages[key], key, ref offset, __instance));
    //     __instance.currentColorIsEquipped = true;
    //     __instance.currentColor = DataManager.Player.Customization.Color;
    //     __instance.SetScrollerBounds();
    //     __instance.scroller.ContentYBounds.max = -(offset + 2f);

    //     if (AllColors.Count != 0)
    //         __instance.GetDefaultSelectable().PlayerEquippedForeground.SetActive(true);

    //     return false;
    // }

    public static void Postfix(PlayerTab __instance)
    {
        if (!PlayerCustomizationMenu.Instance)
            return;

        var tab = PlayerCustomizationMenu.Instance.Tabs[1].Tab;

        if (!__instance.scroller)
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
            colorChip.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }

        __instance.SetScrollerBounds();
    }
}

[HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.Update))]
public static class PlayerTabUpdatePatch
{
    private static float TimePassed;
    private static bool Shadow;

    public static void Postfix(PlayerTab __instance)
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
}

[HarmonyPatch(typeof(PlayerMaterial), nameof(PlayerMaterial.SetColors), typeof(int), typeof(Renderer))]
public static class SetPlayerMaterialPatch1
{
    public static void Prefix(int colorId, Renderer rend) => ColorHandler.Instance.SetRend(rend, colorId);
}

[HarmonyPatch(typeof(PlayerMaterial), nameof(PlayerMaterial.SetColors), typeof(UColor), typeof(Renderer))]
public static class SetPlayerMaterialPatch2
{
    public static void Prefix(Renderer rend, UColor color) => ColorHandler.Instance.SetRend(rend, color);
}

[HarmonyPatch(typeof(RoleEffectAnimation), nameof(RoleEffectAnimation.SetMaterialColor))]
public static class SetPlayerMaterialPatch3
{
    public static void Prefix(RoleEffectAnimation __instance, int colorId) => ColorHandler.Instance.SetRend(__instance.Renderer, colorId);
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdCheckColor))]
public static class CmdCheckColorPatch
{
    public static bool Prefix(PlayerControl __instance, byte bodyColor)
    {
        CallRpc(CustomRPC.Vanilla, VanillaRPC.SetColor, __instance, bodyColor);
        __instance.SetColor(bodyColor);
        return false;
    }
}

[HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.UpdateAvailableColors))]
public static class UpdateAvailableColorsPatch
{
    public static bool Prefix(PlayerTab __instance)
    {
        __instance.AvailableColors.Clear();

        for (var i = 0; i < Palette.PlayerColors.Count; i++)
        {
            if (!CustomPlayer.Local || CustomPlayer.Local.CurrentOutfit.ColorId != i)
                __instance.AvailableColors.Add(i);
        }

        return false;
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetPlayerMaterialColors))]
public static class FixPlayerMaterials
{
    public static bool Prefix(PlayerControl __instance, Renderer rend)
    {
        PlayerMaterial.SetColors(__instance.Data.DefaultOutfit.ColorId, rend);
        return false;
    }
}