using Innersloth.Assets;
using static TownOfUsReworked.Managers.CustomVisorManager;

namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(VisorsTab), nameof(VisorsTab.OnEnable))]
public static class VisorsTabOnEnablePatch
{
    private static TMP_Text Template;

    private static void CreateVisorPackage(List<VisorData> visors, string packageName, ref float yStart, VisorsTab __instance)
    {
        var isDefaultPackage = "Innersloth" == packageName;

        if (!isDefaultPackage)
            visors = [ .. visors.OrderBy(x => x.name) ];

        var offset = yStart;

        if (Template)
        {
            var title = UObject.Instantiate(Template, __instance.scroller.Inner);
            var material = title.GetComponent<MeshRenderer>().material;
            material.SetFloat("_StencilComp", 4f);
            material.SetFloat("_Stencil", 1f);
            title.transform.localPosition = new(2.25f, offset, -1f);
            title.transform.localScale = Vector3.one * 1.5f;
            title.fontSize *= 0.5f;
            title.enableAutoSizing = false;
            title.GetComponent<TextTranslatorTMP>().Destroy();
            title.SetText(packageName);
            offset -= 0.8f * __instance.YOffset;
        }

        for (var i = 0; i < visors.Count; i++)
        {
            var visor = visors[i];
            var xpos = __instance.XRange.Lerp(i % __instance.NumPerRow / (__instance.NumPerRow - 1f));
            var ypos = offset - (i / __instance.NumPerRow * (isDefaultPackage ? 1f : 1.5f) * __instance.YOffset);
            var colorChip = UObject.Instantiate(__instance.ColorTabPrefab, __instance.scroller.Inner);

            if (ActiveInputManager.currentControlType == ActiveInputManager.InputType.Keyboard)
            {
                colorChip.Button.OverrideOnMouseOverListeners(() => __instance.SelectVisor(visor));
                colorChip.Button.OverrideOnMouseOutListeners(() => __instance.SelectVisor(HatManager.Instance.GetVisorById(DataManager.Player.Customization.Visor)));
                colorChip.Button.OverrideOnClickListeners(__instance.ClickEquip);
            }
            else
                colorChip.Button.OverrideOnClickListeners(() => __instance.SelectVisor(visor));

            if (visor.GetExtention() != null)
            {
                var background = colorChip.transform.FindChild("Background");
                var foreground = colorChip.transform.FindChild("ForeGround");

                if (background)
                {
                    background.localPosition = Vector3.down * 0.243f;
                    background.localScale = new(background.localScale.x, 0.8f, background.localScale.y);
                }

                if (foreground)
                    foreground.localPosition = Vector3.down * 0.243f;
            }

            colorChip.Button.ClickMask = __instance.scroller.Hitbox;
            colorChip.transform.localPosition = new(xpos, ypos, -1f);
            colorChip.Inner.SetMaskType(PlayerMaterial.MaskType.SimpleUI);
            colorChip.Inner.transform.localPosition = visor.ChipOffset;
            colorChip.ProductId = visor.ProductId;
            colorChip.Tag = visor;
            __instance.UpdateMaterials(colorChip.Inner.FrontLayer, visor);
            var colorId = __instance.HasLocalPlayer() ? CustomPlayer.LocalCustom.DefaultOutfit.ColorId : DataManager.Player.Customization.Color;

            if (CustomVisorViewDatas.TryGetValue(visor.ProductId, out var data) && data)
                ColorChipFix(colorChip, data.IdleFrame, colorId);
            else
                visor.SetPreview(colorChip.Inner.FrontLayer, colorId);

            colorChip.SelectionHighlight.gameObject.SetActive(false);
            __instance.ColorChips.Add(colorChip);
            yStart = ypos;
        }

        yStart -= 1.5f;
    }

    private static void ColorChipFix(ColorChip chip, Sprite sprite, int colorId)
    {
        chip.Inner.FrontLayer.sprite = sprite;
        AddressableAssetHandler.AddToGameObject(chip.Inner.FrontLayer.gameObject);

        if (Application.isPlaying)
            PlayerMaterial.SetColors(colorId, chip.Inner.FrontLayer);
    }

    public static bool Prefix(VisorsTab __instance)
    {
        __instance.PlayerPreview.gameObject.SetActive(true);

        if (__instance.HasLocalPlayer())
            __instance.PlayerPreview.UpdateFromLocalPlayer(PlayerMaterial.MaskType.None);
        else
            __instance.PlayerPreview.UpdateFromDataManager(PlayerMaterial.MaskType.None);

        __instance.scroller.Inner.DestroyChildren();
        __instance.ColorChips = new();
        var array = HatManager.Instance.GetUnlockedVisors();
        var packages = new Dictionary<string, List<VisorData>>();

        foreach (var data in array)
        {
            var ext = data.GetExtention();
            var package = "Innersloth";

            if (ext != null)
                package = ext.StreamOnly ? "Stream" : ext.Artist;

            if (IsNullEmptyOrWhiteSpace(package))
                package = "Misc";

            if (!packages.TryGetValue(package, out var value))
                packages[package] = value = [];

            value.Add(data);
        }

        Template = __instance.transform.FindChild("Text").GetComponent<TMP_Text>();
        var keys = packages.Keys.OrderBy(x => x switch
        {
            "Innersloth" => 4,
            "Stream" => 1,
            "Misc" => 3,
            _ => 2
        });
        var yOffset = __instance.YStart;
        keys.ForEach(key => CreateVisorPackage(packages[key], key, ref yOffset, __instance));
        __instance.visorId = DataManager.Player.Customization.Visor;
        __instance.currentVisorIsEquipped = true;
        __instance.scroller.ContentYBounds.max = -(yOffset + 4.1f);
        __instance.scroller.UpdateScrollBars();

        if (array.Length != 0)
            __instance.GetDefaultSelectable().PlayerEquippedForeground.SetActive(true);

        return false;
    }
}

[HarmonyPatch(typeof(VisorLayer))]
public static class VisorPatches
{
    [HarmonyPatch(nameof(VisorLayer.UpdateMaterial)), HarmonyPrefix]
    public static bool UpdateMaterialPrefix(VisorLayer __instance)
    {
        if (!__instance.visorData || !CustomVisorViewDatas.TryGetValue(__instance.visorData.ProductId, out var asset) || !asset)
            return true;

        var maskType = __instance.matProperties.MaskType;
        var masked = __instance.visorData && __instance.IsLoaded && __instance.viewAsset.GetAsset().MatchPlayerColor;
        __instance.Image.sharedMaterial = maskType is PlayerMaterial.MaskType.ComplexUI or PlayerMaterial.MaskType.ScrollingUI
            ? (masked ? HatManager.Instance.MaskedPlayerMaterial : HatManager.Instance.MaskedMaterial)
            : (masked ? HatManager.Instance.PlayerMaterial : HatManager.Instance.DefaultShader);
        __instance.Image.maskInteraction = maskType switch
        {
            PlayerMaterial.MaskType.SimpleUI => SpriteMaskInteraction.VisibleInsideMask,
            PlayerMaterial.MaskType.Exile => SpriteMaskInteraction.VisibleOutsideMask,
            _ => SpriteMaskInteraction.None,
        };
        __instance.Image.material.SetInt(PlayerMaterial.MaskLayer, __instance.matProperties.MaskLayer);

        if (__instance.visorData && __instance.IsLoaded && __instance.viewAsset.GetAsset().MatchPlayerColor)
            PlayerMaterial.SetColors(__instance.matProperties.ColorId, __instance.Image);

        if (__instance.matProperties.MaskLayer <= 0)
            PlayerMaterial.SetMaskLayerBasedOnLocalPlayer(__instance.Image, __instance.matProperties.IsLocalPlayer);

        return false;
    }

    [HarmonyPatch(nameof(VisorLayer.SetFlipX)), HarmonyPrefix]
    public static bool SetFlipXPrefix(VisorLayer __instance, bool flipX)
    {
        if (!__instance.visorData || !CustomVisorViewDatas.TryGetValue(__instance.visorData.ProductId, out var asset) || !asset)
            return true;

        __instance.Image.flipX = flipX;

        if (!__instance.IsLoaded)
            return false;

        __instance.Image.sprite = flipX && asset.LeftIdleFrame ? asset.LeftIdleFrame : asset.IdleFrame;
        return false;
    }

    [HarmonyPatch(nameof(VisorLayer.SetVisor), typeof(VisorData), typeof(int)), HarmonyPrefix]
    public static bool SetVisorPrefix(VisorLayer __instance, VisorData data, int color)
    {
        if (!CustomVisorViewDatas.TryGetValue(data.ProductId, out var asset) || !asset)
            return true;

        __instance.visorData = data;
        __instance.SetMaterialColor(color);
        __instance.PopulateFromViewData();
        return false;
    }

    [HarmonyPatch(nameof(VisorLayer.PopulateFromViewData)), HarmonyPrefix]
    public static bool PopulateFromViewDataPrefix(VisorLayer __instance)
    {
        VisorViewData asset = null;

        try
        {
            asset = __instance.viewAsset.GetAsset();
            return true;
        } catch {}

        if (!__instance.visorData || !CustomVisorViewDatas.TryGetValue(__instance.visorData.ProductId, out asset) || !asset)
            return true;

        __instance.UpdateMaterial();
        __instance.transform.SetLocalZ(__instance.DesiredLocalZPosition);
        __instance.SetFlipX(__instance.Image.flipX);
        return false;
    }

    [HarmonyPatch(nameof(VisorLayer.SetFloorAnim)), HarmonyPrefix]
    public static bool SetFloorAnimPrefix(VisorLayer __instance)
    {
        var result = CustomVisorRegistry.TryGetValue(__instance.visorData.name, out var visor);

        if (result)
            __instance.Image.sprite = visor.FloorImage;

        return !result;
    }

    [HarmonyPatch(nameof(VisorLayer.SetClimbAnim)), HarmonyPrefix]
    public static bool SetClimbAnimPrefix(VisorLayer __instance)
    {
        if (!CustomVisorRegistry.TryGetValue(__instance.visorData.name, out var visor))
            return true;

        if (!__instance.options.HideDuringClimb)
            __instance.Image.sprite = visor.ClimbImage;

        return false;
    }
}