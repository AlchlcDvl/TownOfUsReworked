namespace TownOfUsReworked.Patches.Cosmetics;

[HarmonyPatch(typeof(HatManager), nameof(HatManager.Initialize))]
public static class InjectCustomCosmeticsPatch
{
    public static void Postfix(HatManager __instance)
    {
        var allPlates = __instance.allNamePlates.ToList();
        allPlates.InsertRange(1, NameplateLoader.CustomCosmeticRegistry.Values.Select(x => x.CosmeticData));
        allPlates.ForEach((i, x) => x.displayOrder = i);
        __instance.allNamePlates = allPlates.ToArray();

        var allHats = __instance.allHats.ToList();
        allHats.InsertRange(1, HatLoader.CustomCosmeticRegistry.Values.Select(x => x.CosmeticData));
        allHats.ForEach((i, x) => x.displayOrder = i);
        __instance.allHats = allHats.ToArray();

        var allVisors = __instance.allVisors.ToList();
        allVisors.InsertRange(1, VisorLoader.CustomCosmeticRegistry.Values.Select(x => x.CosmeticData));
        allVisors.ForEach((i, x) => x.displayOrder = i);
        __instance.allVisors = allVisors.ToArray();

        // var allSkins = __instance.allSkins.ToList();
        // allSkins.InsertRange(1, SkinLoader.CustomCosmeticRegistry.Values.Select(x => x.CosmeticData));
        // allSkins.ForEach((i, x) => x.displayOrder = i);
        // __instance.allSkins = allSkins.ToArray();

        // foreach (var skin in SkinLoader.CustomCosmeticRegistry.Values)
        // {
        //     if (skin.AnimsInitialised)
        //         continue;

        //     var refAsset = allSkins.Find(x => x.ProductId == skin.BaseSkin).CreateAddressableAsset();
        //     refAsset.LoadAsync((Action)CloneProps);

        //     void CloneProps()
        //     {
        //         var asset = refAsset.GetAsset();
        //         skin.ViewData.IdleAnim = asset.IdleAnim;
        //         skin.ViewData.IdleLeftAnim = asset.IdleLeftAnim;
        //         skin.ViewData.RunAnim = asset.RunAnim;
        //         skin.ViewData.RunLeftAnim = asset.RunLeftAnim;
        //         skin.ViewData.EnterVentAnim = asset.EnterVentAnim;
        //         skin.ViewData.EnterLeftVentAnim = asset.EnterLeftVentAnim;
        //         skin.ViewData.ExitVentAnim = asset.ExitVentAnim;
        //         skin.ViewData.ExitLeftVentAnim = asset.ExitLeftVentAnim;
        //         skin.ViewData.ClimbAnim = asset.ClimbAnim;
        //         skin.ViewData.ClimbDownAnim = asset.ClimbDownAnim;
        //         skin.ViewData.SpawnAnim = asset.SpawnAnim;
        //         skin.ViewData.SpawnLeftAnim = asset.SpawnLeftAnim;
        //         skin.ViewData.KillTongueImpostor = asset.KillTongueImpostor;
        //         skin.ViewData.KillTongueVictim = asset.KillTongueVictim;
        //         skin.ViewData.KillShootImpostor = asset.KillShootImpostor;
        //         skin.ViewData.KillShootVictim = asset.KillShootVictim;
        //         skin.ViewData.KillNeckImpostor = asset.KillNeckImpostor;
        //         skin.ViewData.KillNeckVictim = asset.KillNeckVictim;
        //         skin.ViewData.KillStabImpostor = asset.KillStabImpostor;
        //         skin.ViewData.KillStabVictim = asset.KillStabVictim;
        //         skin.ViewData.KillRHMVictim = asset.KillRHMVictim;
        //         skin.AnimsInitialised = true;
        //     }
        // }

        foreach (var hat in HatLoader.CustomCosmeticRegistry.Values)
        {
            if (hat.NoLongMode)
                __instance.longModeBlackList.Add(hat.CosmeticData);
        }

        foreach (var visor in VisorLoader.CustomCosmeticRegistry.Values)
        {
            if (visor.NoLongMode)
                __instance.longModeBlackList.Add(visor.CosmeticData);
        }

        // foreach (var skin in SkinLoader.CustomCosmeticRegistry.Values)
        // {
        //     if (skin.NoLongMode)
        //         __instance.longModeBlackList.Add(skin.CosmeticData);
        // }
    }
}

[HarmonyPatch(typeof(CosmeticsCache))]
public static class CosmeticsCacheGetCosmeticsPatches
{
    [HarmonyPatch(nameof(CosmeticsCache.GetNameplate))]
    public static bool Prefix(CosmeticsCache __instance, string id, ref NamePlateViewData __result)
    {
        if (!NameplateLoader.CustomCosmeticRegistry.TryGetValue(id, out var cn))
            return true;

        return !(__result = cn.ViewData ?? __instance.nameplates["nameplate_NoPlate"].GetAsset());
    }

    [HarmonyPatch(nameof(CosmeticsCache.GetHat))]
    public static bool Prefix(CosmeticsCache __instance, string id, ref HatViewData __result)
    {
        if (!HatLoader.CustomCosmeticRegistry.TryGetValue(id, out var ch))
            return true;

        return !(__result = ch.ViewData ?? __instance.hats["hat_NoHat"].GetAsset());
    }

    [HarmonyPatch(nameof(CosmeticsCache.GetVisor))]
    public static bool Prefix(CosmeticsCache __instance, string id, ref VisorViewData __result)
    {
        if (!VisorLoader.CustomCosmeticRegistry.TryGetValue(id, out var cv))
            return true;

        return !(__result = cv.ViewData ?? __instance.visors["visor_EmptyVisor"].GetAsset());
    }

    // [HarmonyPatch(nameof(CosmeticsCache.GetSkin))]
    // public static bool Prefix(CosmeticsCache __instance, string id, ref SkinViewData __result)
    // {
    //     if (!SkinLoader.CustomCosmeticRegistry.TryGetValue(id, out var cv))
    //         return true;

    //     return !(__result = cv.ViewData ?? __instance.skins["skin_None"].GetAsset());
    // }
}

[HarmonyPatch(typeof(PlayerCustomizationMenu), nameof(PlayerCustomizationMenu.Start))]
public static class CosmeticTabPatches
{
    private static TMP_Text Template;

    public static void Postfix(PlayerCustomizationMenu __instance)
    {
        if (!Template)
            Template = __instance.Tabs[0].Tab.transform.FindChild("Text").GetComponent<TMP_Text>();
    }

    public static void BaseOnEnable(this InventoryTab __instance)
    {
        __instance.PlayerPreview.gameObject.SetActive(true);

        if (__instance.HasLocalPlayer())
            __instance.PlayerPreview.UpdateFromLocalPlayer(PlayerMaterial.MaskType.None);
        else
            __instance.PlayerPreview.UpdateFromDataManager(PlayerMaterial.MaskType.None);

        __instance.scroller.Inner.DestroyChildren();
        __instance.ColorChips = new();
    }

    public static Dictionary<string, List<TData>> GeneratePackages<TData, TCustomData, TView>(Il2CppReferenceArray<TData> array, Dictionary<string, TCustomData> registry)
        where TData : CosmeticData
        where TCustomData : CustomCosmetic<TView, TData>
        where TView : ScriptableObject
    {
        var packages = new Dictionary<string, List<TData>>();

        foreach (var data in array)
        {
            var package = "Innersloth";

            if (registry.TryGetValue(data.ProductId, out var cn))
                package = cn.StreamOnly ? "Stream" : cn.Artist;

            if (IsNullEmptyOrWhiteSpace(package))
                package = "Misc";

            if (!packages.TryGetValue(package, out var value))
                packages[package] = value = [];

            value.Add(data);
        }

        return packages;
    }

    private static void CreatePackage<T>(List<T> cosmetics, string packageName, ref float yStart, InventoryTab __instance, bool isNameplate, Action<T> onOver, Action onOut, Action<ColorChip, T,
        InventoryTab> setPreview)
        where T : CosmeticData
    {
        var offset = yStart;

        if (Template)
        {
            var title = UObject.Instantiate(Template, __instance.scroller.Inner);
            title.fontMaterial.SetFloat(StencilComp, 4f);
            title.fontMaterial.SetFloat(Stencil, 1f);
            title.transform.localPosition = new(2.25f, offset, -1f);
            title.transform.localScale = Vector3.one * 1.5f;
            title.fontSize *= 0.5f;
            title.enableAutoSizing = false;
            title.GetComponent<TextTranslatorTMP>().Destroy();
            title.text = packageName;
            offset -= 0.8f * __instance.YOffset;
        }

        for (var i = 0; i < cosmetics.Count; i++)
        {
            var cosmetic = cosmetics[i];
            var xpos = __instance.XRange.Lerp(i % __instance.NumPerRow / (__instance.NumPerRow - 1f));
            var ypos = offset - (i / __instance.NumPerRow * __instance.YOffset);
            var colorChip = UObject.Instantiate(__instance.ColorTabPrefab, __instance.scroller.Inner);

            if (ActiveInputManager.currentControlType == ActiveInputManager.InputType.Keyboard)
            {
                colorChip.Button.OverrideOnMouseOverListeners(() => onOver(cosmetic));
                colorChip.Button.OverrideOnMouseOutListeners(onOut);
                colorChip.Button.OverrideOnClickListeners(__instance.ClickEquip);
            }
            else
                colorChip.Button.OverrideOnClickListeners(() => onOver(cosmetic));

            colorChip.Button.ClickMask = __instance.scroller.Hitbox;
            colorChip.transform.localPosition = new(xpos, ypos, -1f);
            colorChip.ProductId = colorChip.name = cosmetic.ProductId;
            colorChip.Tag = cosmetic;
            colorChip.SelectionHighlight.gameObject.SetActive(false);

            if (!isNameplate)
            {
                colorChip.Inner.SetMaskType(PlayerMaterial.MaskType.SimpleUI);
                colorChip.Inner.transform.localPosition = cosmetic.ChipOffset;
            }

            if (setPreview != null)
                setPreview(colorChip, cosmetic, __instance);
            else
                cosmetic.SetPreview(colorChip.Inner.FrontLayer, __instance.HasLocalPlayer() ? LocalPlayer.Data.DefaultOutfit.ColorId : DataManager.Player.Customization.Color);

            __instance.ColorChips.Add(colorChip);

            if (!HatManager.Instance.CheckLongModeValidCosmetic(cosmetic.ProductId, __instance.PlayerPreview.GetIgnoreLongMode()))
                colorChip.SetUnavailable();

            yStart = ypos;
        }

        yStart -= 1.5f;
    }

    public static void CreatePackages<T>(this InventoryTab __instance, Dictionary<string, List<T>> packages, bool isNameplate, Action<T> onOver, Action onOut, out float yOffset, Action<ColorChip,
        T, InventoryTab> setPreview = null)
        where T : CosmeticData
    {
        var keys = packages.Keys.OrderBy(x => x switch
        {
            "Innersloth" => 4,
            "Stream" => 1,
            "Misc" => 3,
            _ => 2
        });
        var yOffsetInner = __instance.YStart;
        keys.Do(key => CreatePackage(packages[key], key, ref yOffsetInner, __instance, isNameplate, onOver, onOut, setPreview));
        yOffset = yOffsetInner;
    }

    public static void EndOnEnable<T>(this InventoryTab __instance, float yOffset, Il2CppReferenceArray<T> array)
        where T : CosmeticData
    {
        __instance.scroller.ContentYBounds.max = -(yOffset + 4.1f);
        __instance.scroller.UpdateScrollBars();

        if (array.Length != 0)
            __instance.GetDefaultSelectable().PlayerEquippedForeground.SetActive(true);
    }
}

[HarmonyPatch(typeof(CosmeticData), nameof(CosmeticData.SetPreview))]
public static class ProperlySetPreviews
{
    public static readonly Dictionary<string, Sprite> Previews = [];

    public static bool Prefix(CosmeticData __instance, SpriteRenderer renderer, int color)
    {
        if (!Previews.TryGetValue(__instance.ProductId, out var sprite))
            return true;

        renderer.sprite = sprite;

        if (Application.isPlaying)
            PlayerMaterial.SetColors(color, renderer);

        return false;
    }
}