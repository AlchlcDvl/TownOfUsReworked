using static TownOfUsReworked.Cosmetics.CustomColors.CustomColorManager;

namespace TownOfUsReworked.Cosmetics.CustomColors;

[HarmonyPatch(typeof(PlayerTab), nameof(PlayerTab.OnEnable))]
public static class PlayerTabOnEnablePatch
{
    /*private static TMP_Text Template;

    private static float CreateColorPackage(List<CustomColor> colors, string packageName, float YStart, PlayerTab __instance)
    {
        var offset = YStart;

        if (Template != null)
        {
            var title = UObject.Instantiate(Template, __instance.ColorTabArea);
            var material = title.GetComponent<MeshRenderer>().material;
            material.SetFloat("_StencilComp", 4f);
            material.SetFloat("_Stencil", 1f);
            title.transform.localPosition = new(Template.transform.localPosition.x, YStart, -1f);
            title.transform.localScale = Vector3.one * 1.5f;
            title.fontSize *= 0.5f;
            title.enableAutoSizing = false;
            title.gameObject.SetActive(true);
            __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>(_ => title.SetText(packageName, true))));
            offset -= 0.8f * __instance.YOffset;
        }

        for (var i = 0; i < colors.Count; i++)
        {
            var color = colors[i];
            var xpos = __instance.XRange.min + (i % 15 * 0.35f);
            var ypos = offset - (i / 15 * 0.35f);
            var colorChip = UObject.Instantiate(__instance.ColorTabPrefab, __instance.ColorTabArea);

            if (ActiveInputManager.currentControlType == ActiveInputManager.InputType.Keyboard)
            {
                colorChip.Button.OnMouseOver.AddListener((Action)(() => __instance.SelectColor(color.ColorID)));
                colorChip.Button.OnMouseOut.AddListener((Action)(() => __instance.SelectColor(DataManager.Player.Customization.Color)));
                colorChip.Button.OnClick.AddListener((Action)(() => __instance.ClickEquip()));
            }
            else
                colorChip.Button.OnClick.AddListener((Action)(() => __instance.SelectColor(color.ColorID)));

            colorChip.transform.localScale *= 0.6f;
            colorChip.Inner.SpriteColor = GetColor(i, false);
            colorChip.transform.localPosition = new(xpos, ypos, 2f);
            colorChip.SelectionHighlight.gameObject.SetActive(false);
            colorChip.Tag = color.ColorID;
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

        __instance.ColorChips = new();
        var packages = new Dictionary<string, List<CustomColor>>();

        foreach (var data in AllColors)
        {
            var package = data.Title;

            if (IsNullEmptyOrWhiteSpace(package))
                package = "Misc";

            if (!packages.ContainsKey(package))
                packages[package] = new();

            packages[package].Add(data);
        }

        var yOffset = __instance.YStart;
        Template = __instance.transform.FindChild("Text").gameObject.GetComponent<TMP_Text>();
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
    public static void Postfix(PlayerTab __instance)
    {
        for (var i = 0; i < __instance.ColorChips.Count; i++)
            __instance.ColorChips[i].Inner.SpriteColor = GetColor(i, false);
    }
}

[HarmonyPatch(typeof(PlayerMaterial), nameof(PlayerMaterial.SetColors), typeof(int), typeof(Renderer))]
public static class SetPlayerMaterialPatch1
{
    public static bool Prefix(ref int colorId, ref Renderer rend)
    {
        rend.gameObject.EnsureComponent<ColorBehaviour>().AddRend(rend, colorId);
        return !colorId.IsChanging();
    }
}

[HarmonyPatch(typeof(PlayerMaterial), nameof(PlayerMaterial.SetColors), typeof(UColor), typeof(Renderer))]
public static class SetPlayerMaterialPatch2
{
    public static bool Prefix(ref Renderer rend)
    {
        rend.gameObject.EnsureComponent<ColorBehaviour>().AddRend(rend, -1);
        return true;
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdCheckColor))]
public static class CmdCheckColorPatch
{
    public static bool Prefix(PlayerControl __instance, ref byte bodyColor)
    {
        CallRpc(CustomRPC.Misc, MiscRPC.SetColor, __instance, bodyColor);
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