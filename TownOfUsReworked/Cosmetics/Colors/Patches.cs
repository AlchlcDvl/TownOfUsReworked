// using static TownOfUsReworked.Cosmetics.CustomColors.CustomColorManager;

namespace TownOfUsReworked.Cosmetics.CustomColors;

[HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.OnEnable))]
public static class PlayerTabOnEnablePatch
{
    /*private static TMP_Text Template;
    private static BoxCollider2D Collider;

    private static float CreateColorPackage(List<CustomColor> colors, string packageName, float YStart, PlayerTab __instance)
    {
        var offset = YStart;

        if (Template)
        {
            var title = UObject.Instantiate(Template, __instance.scroller.Inner);
            var material = title.GetComponent<MeshRenderer>().material;
            material.SetFloat("_StencilComp", 4f);
            material.SetFloat("_Stencil", 1f);
            title.transform.localPosition = new(Template.transform.localPosition.x, YStart, -1f);
            title.transform.localScale = Vector3.one * 1.5f;
            title.fontSize *= 0.5f;
            title.enableAutoSizing = false;
            title.gameObject.SetActive(true);
            Coroutines.Start(PerformTimedAction(0.1f, _ => title.SetText(packageName, true)));
            offset -= 0.8f * __instance.YOffset;
        }

        for (var i = 0; i < colors.Count; i++)
        {
            var color = colors[i];
            var xpos = __instance.XRange.min + (i % 15 * 0.35f);
            var ypos = offset - (i / 15 * 0.35f);
            var colorChip = UObject.Instantiate(__instance.ColorTabPrefab, __instance.scroller.Inner);

            if (ActiveInputManager.currentControlType == ActiveInputManager.InputType.Keyboard)
            {
                colorChip.Button.OverrideOnMouseOverListeners(() => __instance.SelectColor(color.ColorID));
                colorChip.Button.OverrideOnMouseOutListeners(() => __instance.SelectColor(DataManager.Player.Customization.Color));
                colorChip.Button.OverrideOnClickListeners(__instance.ClickEquip);
            }
            else
                colorChip.Button.OverrideOnClickListeners(() => __instance.SelectColor(color.ColorID));

            colorChip.transform.localScale *= 0.6f;
            colorChip.Inner.SpriteColor = i.GetColor(false);
            colorChip.transform.localPosition = new(xpos, ypos, 2f);
            colorChip.SelectionHighlight.gameObject.SetActive(false);
            colorChip.Tag = color.ColorID;
            colorChip.Button.ClickMask = Collider;
            colorChip.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            __instance.ColorChips.Add(colorChip);
        }

        return offset - (colors.Count / 15 * 0.35f);
    }

    public static bool Prefix(PlayerTab __instance)
    {
        try
        {
            for (var i = 0; i < __instance.scroller.Inner.childCount; i++)
                __instance.scroller.Inner.GetChild(i).gameObject.Destroy();
        } catch {}

        var tab = PlayerCustomizationMenu.Instance.Tabs[1].Tab;

        if (!__instance.scroller)
        {
            __instance.scroller = UObject.Instantiate(tab.scroller, __instance.transform, true);
            __instance.scroller.Inner.transform.DestroyChildren();
            var gameObject = new GameObject("SpriteMask") { layer = 5 };
            gameObject.transform.SetParent(__instance.transform);
            gameObject.transform.localPosition = new(0f, 0f, 0f);
            gameObject.transform.localScale = new(500f, 4.76f, 0f);
            gameObject.AddComponent<SpriteMask>().sprite = GetSprite("Blank");
            Collider = gameObject.AddComponent<BoxCollider2D>();
            Collider.size = new(1f, 0.75f);
            Collider.enabled = true;
        }

        __instance.ColorChips = new();
        var packages = new Dictionary<string, List<CustomColor>>();

        foreach (var data in AllColors)
        {
            var package = data.Title;

            if (IsNullEmptyOrWhiteSpace(package))
                package = "Misc";

            if (!packages.ContainsKey(package))
                packages[package] = [];

            packages[package].Add(data);
        }

        var yOffset = __instance.YStart;
        Template = __instance.transform.FindChild("Text").GetComponent<TMP_Text>();
        var keys = packages.Keys.OrderBy(x => x switch
        {
            "Innersloth" => 7,
            "Stream" => 1,
            "Everchanging" => 2,
            "Constrast" => 3,
            "Solid" => 4,
            "Fusion" => 5,
            _ => 6
        });
        keys.ForEach(key => yOffset = CreateColorPackage(packages[key], key, yOffset, __instance));
        __instance.currentColor = DataManager.Player.Customization.Color;
        __instance.SetScrollerBounds();
        return false;
    }*/

    public static void Postfix(PlayerTab __instance)
    {
        for (var i = 0; i < __instance.ColorChips.Count; i++)
        {
            var colorChip = __instance.ColorChips[i];
            colorChip.transform.localScale *= 0.6f;
            var x = __instance.XRange.min + (i % 10 * 0.35f);
            var y = __instance.YStart - (i / 10 * 0.35f);
            colorChip.transform.localPosition = new(x, y, 2f);
        }
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