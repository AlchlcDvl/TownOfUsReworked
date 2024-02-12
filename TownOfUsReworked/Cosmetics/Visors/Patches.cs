using static TownOfUsReworked.Cosmetics.CustomVisors.CustomVisorManager;

namespace TownOfUsReworked.Cosmetics.CustomVisors;

[HarmonyPatch(typeof(HatManager), nameof(HatManager.GetVisorById))]
public static class HatManagerPatch
{
    private static bool IsLoaded;

    public static void Prefix(HatManager __instance)
    {
        if (IsLoaded)
            return;

        var allVisors = __instance.allVisors.ToList();
        allVisors.AddRange(RegisteredVisors);
        __instance.allVisors = allVisors.ToArray();
        IsLoaded = true;
    }
}

[HarmonyPatch(typeof(VisorsTab), nameof(VisorsTab.OnEnable))]
public static class VisorsTabOnEnablePatch
{
    private static TMP_Text Template;

    private static float CreateVisorPackage(List<VisorData> visors, string packageName, float YStart, VisorsTab __instance)
    {
        var isDefaultPackage = "Innersloth" == packageName;

        if (!isDefaultPackage)
            visors = visors.OrderBy(x => x.name).ToList();

        var offset = YStart;

        if (Template != null)
        {
            var title = UObject.Instantiate(Template, __instance.scroller.Inner);
            var material = title.GetComponent<MeshRenderer>().material;
            material.SetFloat("_StencilComp", 4f);
            material.SetFloat("_Stencil", 1f);
            title.transform.localPosition = new(2.25f, YStart, -1f);
            title.transform.localScale = Vector3.one * 1.5f;
            title.fontSize *= 0.5f;
            title.enableAutoSizing = false;
            __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>(_ => title.SetText(packageName, true))));
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
                colorChip.Button.OnMouseOver.AddListener((Action)(() => __instance.SelectVisor(visor)));
                colorChip.Button.OnMouseOut.AddListener((Action)(() => __instance.SelectVisor(HatManager.Instance.GetVisorById(DataManager.Player.Customization.Visor))));
                colorChip.Button.OnClick.AddListener((Action)(() => __instance.ClickEquip()));
            }
            else
                colorChip.Button.OnClick.AddListener((Action)(() => __instance.SelectVisor(visor)));

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
            colorChip.Inner.SetMaskType(PlayerMaterial.MaskType.ScrollingUI);
            colorChip.Inner.transform.localPosition = visor.ChipOffset;
            colorChip.ProductId = visor.ProductId;
            colorChip.Tag = visor;
            __instance.UpdateMaterials(colorChip.Inner.FrontLayer, visor);
            visor.SetPreview(colorChip.Inner.FrontLayer, __instance.HasLocalPlayer() ? CustomPlayer.LocalCustom.DefaultOutfit.ColorId : DataManager.Player.Customization.Color);
            colorChip.SelectionHighlight.gameObject.SetActive(false);
            __instance.ColorChips.Add(colorChip);
        }

        return offset - ((visors.Count - 1) / __instance.NumPerRow * (isDefaultPackage ? 1f : 1.5f) * __instance.YOffset) - 1.5f;
    }

    public static bool Prefix(VisorsTab __instance)
    {
        for (var i = 0; i < __instance.scroller.Inner.childCount; i++)
            __instance.scroller.Inner.GetChild(i).gameObject.Destroy();

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

            if (!packages.ContainsKey(package))
                packages[package] = new();

            packages[package].Add(data);
        }

        var yOffset = __instance.YStart;
        Template = __instance.transform.FindChild("Text").gameObject.GetComponent<TMP_Text>();
        var keys = packages.Keys.OrderBy(x => x switch
        {
            "Innersloth" => 4,
            "Stream" => 1,
            "Misc" => 3,
            _ => 2
        });
        keys.ForEach(key => yOffset = CreateVisorPackage(packages[key], key, yOffset, __instance));

        if (array.Length != 0)
            __instance.GetDefaultSelectable().PlayerEquippedForeground.SetActive(true);

        __instance.visorId = DataManager.Player.Customization.Visor;
        __instance.currentVisorIsEquipped = true;
        __instance.SetScrollerBounds();
        __instance.scroller.ContentYBounds.max = -(yOffset + 4.1f);
        return false;
    }
}

[HarmonyPatch(typeof(CosmeticsCache), nameof(CosmeticsCache.GetVisor))]
public static class CosmeticsCacheGetVisorPatch
{
    public static bool Prefix(CosmeticsCache __instance, ref string id, ref VisorViewData __result)
    {
        var cache = __result;

        if (!CustomVisorViewDatas.TryGetValue(id, out __result))
        {
            __result = cache;
            return true;
        }

        if (__result == null)
            __result = __instance.visors["visor_EmptyVisor"].GetAsset();

        return false;
    }
}

[HarmonyPatch(typeof(VisorLayer))]
public static class VisorlayerPatches
{
    [HarmonyPatch(nameof(VisorLayer.UpdateMaterial))]
    [HarmonyPrefix]
    public static bool UpdateMaterialPrefix(VisorLayer __instance)
    {
        if (__instance.currentVisor == null || !CustomVisorViewDatas.TryGetValue(__instance.currentVisor.ProductId, out var asset) || !asset)
            return true;

        __instance.Image.sharedMaterial = asset.AltShader ?? HatManager.Instance.DefaultShader;
        PlayerMaterial.SetColors(__instance.matProperties.ColorId, __instance.Image);
        __instance.Image.maskInteraction = __instance.matProperties.MaskType switch
        {
            PlayerMaterial.MaskType.SimpleUI or PlayerMaterial.MaskType.ScrollingUI => SpriteMaskInteraction.VisibleInsideMask,
            PlayerMaterial.MaskType.Exile => SpriteMaskInteraction.VisibleOutsideMask,
            _ => SpriteMaskInteraction.None,
        };
        return false;
    }

    [HarmonyPatch(nameof(VisorLayer.SetFlipX))]
    [HarmonyPrefix]
    public static bool SetFlipXPrefix(VisorLayer __instance, ref bool flipX)
    {
        if (__instance.currentVisor == null || !CustomVisorViewDatas.TryGetValue(__instance.currentVisor.ProductId, out var asset) || !asset)
            return true;

        __instance.Image.flipX = flipX;
        __instance.Image.sprite = flipX && asset.LeftIdleFrame ? asset.LeftIdleFrame : asset.IdleFrame;
        return false;
    }

    [HarmonyPatch(nameof(VisorLayer.SetVisor), typeof(VisorData), typeof(VisorViewData), typeof(int))]
    [HarmonyPrefix]
    public static bool SetVisorPrefix1(VisorLayer __instance, ref VisorData data, ref VisorViewData visorView, ref int colorId)
    {
        var cache = visorView;

        if (!CustomVisorViewDatas.TryGetValue(__instance.currentVisor.ProductId, out visorView))
        {
            visorView = cache;
            return true;
        }

        __instance.currentVisor = data;
        __instance.transform.SetLocalZ(__instance.ZIndexSpacing * (data.BehindHats ? -1.5f : -3f));
        __instance.SetFlipX(__instance.Image.flipX);
        __instance.SetMaterialColor(colorId);
        return false;
    }

    [HarmonyPatch(nameof(VisorLayer.SetVisor), typeof(VisorData), typeof(int))]
    [HarmonyPrefix]
    public static bool SetVisorPrefix2(VisorLayer __instance, ref VisorData data, ref int colorId)
    {
        if (!CustomVisorViewDatas.TryGetValue(data.ProductId, out var asset))
            return true;

        __instance.currentVisor = data;
        __instance.SetVisor(__instance.currentVisor, asset, colorId);
        return false;
    }
}