using PowerTools;
using static TownOfUsReworked.Cosmetics.CustomHats.CustomHatManager;

namespace TownOfUsReworked.Cosmetics.CustomHats;

[HarmonyPatch(typeof(HatManager), nameof(HatManager.GetHatById))]
public static class HatManagerPatch
{
    private static bool IsLoaded;

    public static void Prefix(HatManager __instance)
    {
        if (IsLoaded)
            return;

        var allHats = __instance.allHats.ToList();
        allHats.AddRange(RegisteredHats);
        __instance.allHats = allHats.ToArray();
        IsLoaded = true;
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleAnimation))]
public static class PlayerPhysicsHandleAnimationPatch
{
    public static void Postfix(PlayerPhysics __instance)
    {
        try
        {
            if (!__instance.myPlayer || !CustomHatViewDatas.TryGetValue(__instance.myPlayer.cosmetics.hat.Hat.ProductId, out var viewData))
                return;

            var currentAnimation = __instance.Animations.Animator.GetCurrentAnimation();

            if (currentAnimation == __instance.Animations.group.ClimbUpAnim || currentAnimation == __instance.Animations.group.ClimbDownAnim)
                return;

            var hp = __instance.myPlayer.cosmetics.hat;

            if (!hp || !hp.Hat)
                return;

            var extend = hp.Hat.GetExtention();

            if (extend.FlipImage != null)
                hp.FrontLayer.sprite = __instance.FlipX ? extend.FlipImage : viewData.MainImage;

            if (extend.BackFlipImage != null)
                hp.BackLayer.sprite = __instance.FlipX ? extend.BackFlipImage : viewData.BackImage;
        } catch {}
    }
}

[HarmonyPatch(typeof(HatParent))]
public static class HatParentPatches
{
    private static bool SetCustomHat(HatParent hatParent)
    {
        var dirPath = Path.Combine(TownOfUsReworked.Hats, "Test");

        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);

        var filePath = Path.Combine(TownOfUsReworked.Hats, "Test", "Hats.json");

        if (!File.Exists(filePath) || !TutorialManager.InstanceExists)
            return true;

        var json = File.ReadAllText(filePath);
        var data = JsonSerializer.Deserialize<HatsJSON>(json);
        data.Hats.ForEach(x => x.TestOnly = true);

        if (data.Hats.Count <= 0)
            return false;

        try
        {
            hatParent.Hat = CreateHatBehaviour(data.Hats[0]);
            return false;
        }
        catch (Exception err)
        {
            LogWarning($"Unable to create test hat\n{err}");
            return true;
        }
    }

    [HarmonyPatch(nameof(HatParent.SetHat), typeof(int))]
    [HarmonyPriority(Priority.High)]
    [HarmonyPrefix]
    public static void SetHatPrefix(HatParent __instance) => SetCustomHat(__instance);

    [HarmonyPatch(nameof(HatParent.SetHat), typeof(HatData), typeof(int))]
    [HarmonyPrefix]
    public static bool SetHatPrefix1(HatParent __instance, ref int color)
    {
        if (SetCustomHat(__instance))
            return true;

        __instance.PopulateFromHatViewData();
        __instance.SetMaterialColor(color);
        return false;
    }

    [HarmonyPatch(nameof(HatParent.SetHat), typeof(int))]
    [HarmonyPrefix]
    public static bool SetHatPrefix2(HatParent __instance, ref int color)
    {
        if (!CustomHatViewDatas.ContainsKey(__instance.Hat.ProductId))
            return true;

        __instance.hatDataAsset = null;
        __instance.PopulateFromHatViewData();
        __instance.SetMaterialColor(color);
        return false;
    }

    [HarmonyPatch(nameof(HatParent.UpdateMaterial))]
    [HarmonyPrefix]
    public static bool UpdateMaterialPrefix(HatParent __instance)
    {
        HatViewData asset;

        try
        {
            asset = __instance.hatDataAsset.GetAsset();
            return true;
        }
        catch
        {
            try
            {
                CustomHatViewDatas.TryGetValue(__instance.Hat.ProductId, out asset);
            }
            catch
            {
                return false;
            }
        }

        if (asset == null || asset.AltShader == null)
            return true;

        __instance.FrontLayer.sharedMaterial = asset?.AltShader ?? HatManager.Instance.DefaultShader;

        if (__instance.BackLayer)
            __instance.BackLayer.sharedMaterial = asset?.AltShader ?? HatManager.Instance.DefaultShader;

        var colorId = __instance.matProperties.ColorId;
        PlayerMaterial.SetColors(colorId, __instance.FrontLayer);

        if (__instance.BackLayer)
            PlayerMaterial.SetColors(colorId, __instance.BackLayer);

        __instance.FrontLayer.material.SetInt(PlayerMaterial.MaskLayer, __instance.matProperties.MaskLayer);

        if (__instance.BackLayer)
            __instance.BackLayer.material.SetInt(PlayerMaterial.MaskLayer, __instance.matProperties.MaskLayer);

        var maskInteraction = __instance.matProperties.MaskType switch
        {
            PlayerMaterial.MaskType.ScrollingUI => SpriteMaskInteraction.VisibleInsideMask,
            PlayerMaterial.MaskType.Exile => SpriteMaskInteraction.VisibleOutsideMask,
            _ => SpriteMaskInteraction.None
        };

        __instance.FrontLayer.maskInteraction = maskInteraction;

        if (__instance.BackLayer)
            __instance.BackLayer.maskInteraction = maskInteraction;

        if (__instance.matProperties.MaskLayer <= 0)
        {
            PlayerMaterial.SetMaskLayerBasedOnLocalPlayer(__instance.FrontLayer, __instance.matProperties.IsLocalPlayer);

            if (__instance.BackLayer)
                PlayerMaterial.SetMaskLayerBasedOnLocalPlayer(__instance.BackLayer, __instance.matProperties.IsLocalPlayer);
        }

        return false;
    }

    [HarmonyPatch(nameof(HatParent.LateUpdate))]
    [HarmonyPrefix]
    public static bool LateUpdatePrefix(HatParent __instance)
    {
        if (!__instance.Parent || !__instance.Hat)
            return false;

        HatViewData hatViewData;

        try
        {
            hatViewData = __instance.hatDataAsset.GetAsset();
            return true;
        }
        catch
        {
            try
            {
                CustomHatViewDatas.TryGetValue(__instance.Hat.ProductId, out hatViewData);
            }
            catch
            {
                return false;
            }
        }

        if (__instance.FrontLayer.sprite != hatViewData.ClimbImage && __instance.FrontLayer.sprite != hatViewData.FloorImage)
        {
            if ((__instance.Hat.InFront || hatViewData.BackImage) && hatViewData.LeftMainImage)
                __instance.FrontLayer.sprite = __instance.Parent.flipX ? hatViewData.LeftMainImage : hatViewData.MainImage;

            if (hatViewData.BackImage && hatViewData.LeftBackImage)
            {
                __instance.BackLayer.sprite = __instance.Parent.flipX ? hatViewData.LeftBackImage : hatViewData.BackImage;
                return false;
            }

            if (!hatViewData.BackImage && !__instance.Hat.InFront && hatViewData.LeftMainImage)
            {
                __instance.BackLayer.sprite = __instance.Parent.flipX ? hatViewData.LeftMainImage : hatViewData.MainImage;
                return false;
            }
        }
        else if (__instance.FrontLayer.sprite == hatViewData.ClimbImage || __instance.FrontLayer.sprite == hatViewData.LeftClimbImage)
        {
            var spriteAnimNodeSync = __instance.SpriteSyncNode ??= __instance.GetComponent<SpriteAnimNodeSync>();

            if (spriteAnimNodeSync)
                spriteAnimNodeSync.NodeId = 0;
        }

        return false;
    }

    [HarmonyPatch(nameof(HatParent.SetFloorAnim))]
    [HarmonyPrefix]
    public static bool SetFloorAnimPrefix(HatParent __instance)
    {
        HatViewData hatViewData;

        try
        {
            hatViewData = __instance.hatDataAsset.GetAsset();
            return true;
        } catch {}

        if (!CustomHatViewDatas.TryGetValue(__instance.Hat.ProductId, out hatViewData))
            return true;

        __instance.BackLayer.enabled = false;
        __instance.FrontLayer.enabled = true;
        __instance.FrontLayer.sprite = hatViewData.FloorImage;
        return false;
    }

    [HarmonyPatch(nameof(HatParent.SetIdleAnim))]
    [HarmonyPrefix]
    public static bool SetIdleAnimPrefix(HatParent __instance, ref int colorId)
    {
        if (!__instance.Hat)
            return false;

        if (!CustomHatViewDatas.TryGetValue(__instance.Hat.ProductId, out var hatViewData))
            return true;

        __instance.hatDataAsset = null;
        __instance.PopulateFromHatViewData();
        __instance.SetMaterialColor(colorId);
        return false;
    }

    [HarmonyPatch(nameof(HatParent.SetClimbAnim))]
    [HarmonyPrefix]
    public static bool SetClimbAnimPrefix(HatParent __instance)
    {
        HatViewData hatViewData;

        try
        {
            hatViewData = __instance.hatDataAsset.GetAsset();
            return true;
        } catch {}

        if (!CustomHatViewDatas.TryGetValue(__instance.Hat.ProductId, out hatViewData))
            return true;

        if (!__instance.options.ShowForClimb)
            return false;

        __instance.BackLayer.enabled = false;
        __instance.FrontLayer.enabled = true;
        __instance.FrontLayer.sprite = hatViewData.ClimbImage;
        return false;
    }

    [HarmonyPatch(nameof(HatParent.PopulateFromHatViewData))]
    [HarmonyPrefix]
    public static bool PopulateFromHatViewDataPrefix(HatParent __instance)
    {
        HatViewData asset = null;

        try
        {
            asset = __instance.hatDataAsset.GetAsset();
            return true;
        } catch {}

        if (!__instance.Hat || !CustomHatViewDatas.TryGetValue(__instance.Hat?.ProductId, out asset) || !asset)
            return true;

        __instance.UpdateMaterial();
        var spriteAnimNodeSync = __instance.SpriteSyncNode ??= __instance.GetComponent<SpriteAnimNodeSync>();

        if (spriteAnimNodeSync)
            spriteAnimNodeSync.NodeId = __instance.Hat.NoBounce ? 1 : 0;

        if (__instance.Hat.InFront)
        {
            __instance.BackLayer.enabled = false;
            __instance.FrontLayer.enabled = true;
            __instance.FrontLayer.sprite = asset.MainImage;
        }
        else if (asset.BackImage)
        {
            __instance.BackLayer.enabled = true;
            __instance.FrontLayer.enabled = true;
            __instance.BackLayer.sprite = asset.BackImage;
            __instance.FrontLayer.sprite = asset.MainImage;
        }
        else
        {
            __instance.BackLayer.enabled = true;
            __instance.FrontLayer.enabled = false;
            __instance.FrontLayer.sprite = null;
            __instance.BackLayer.sprite = asset.MainImage;
        }

        if (__instance.options.Initialized && __instance.HideHat())
        {
            __instance.FrontLayer.enabled = false;
            __instance.BackLayer.enabled = false;
        }

        return false;
    }
}

[HarmonyPatch(typeof(CosmeticsCache), nameof(CosmeticsCache.GetHat))]
public static class CosmeticsCacheGetVisorPatch
{
    public static bool Prefix(CosmeticsCache __instance, ref string id, ref HatViewData __result)
    {
        var cache = __result;

        if (!CustomHatViewDatas.TryGetValue(id, out __result))
        {
            __result = cache;
            return true;
        }

        if (__result == null)
            __result = __instance.hats["hat_NoHat"].GetAsset();

        return false;
    }
}

[HarmonyPatch(typeof(HatsTab), nameof(HatsTab.OnEnable))]
public static class HatsTabOnEnablePatch
{
    private static TMP_Text Template;

    private static float CreateHatPackage(List<HatData> hats, string packageName, float YStart, HatsTab __instance)
    {
        var isDefaultPackage = "Innersloth" == packageName;

        if (!isDefaultPackage)
            hats = hats.OrderBy(x => x.name).ToList();

        var offset = YStart;

        if (Template)
        {
            var title = UObject.Instantiate(Template, __instance.scroller.Inner);
            title.transform.localPosition = new(2.25f, YStart, -1f);
            title.transform.localScale = Vector3.one * 1.5f;
            title.fontSize *= 0.5f;
            title.enableAutoSizing = false;
            __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>(_ => title.SetText(packageName))));
            offset -= 0.8f * __instance.YOffset;
        }

        for (var i = 0; i < hats.Count; i++)
        {
            var hat = hats[i];
            var xpos = __instance.XRange.Lerp(i % __instance.NumPerRow / (__instance.NumPerRow - 1f));
            var ypos = offset - (i / __instance.NumPerRow * (isDefaultPackage ? 1f : 1.5f) * __instance.YOffset);
            var colorChip = UObject.Instantiate(__instance.ColorTabPrefab, __instance.scroller.Inner);

            if (ActiveInputManager.currentControlType == ActiveInputManager.InputType.Keyboard)
            {
                colorChip.Button.OnMouseOver.AddListener((Action)(() => __instance.SelectHat(hat)));
                colorChip.Button.OnMouseOut.AddListener((Action)(() => __instance.SelectHat(HatManager.Instance.GetHatById(DataManager.Player.Customization.Hat))));
                colorChip.Button.OnClick.AddListener((Action)(() => __instance.ClickEquip()));
            }
            else
                colorChip.Button.OnClick.AddListener((Action)(() => __instance.SelectHat(hat)));

            if (hat.GetExtention() != null)
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

            colorChip.transform.localPosition = new(xpos, ypos, -1f);
            colorChip.Button.ClickMask = __instance.scroller.Hitbox;
            colorChip.Inner.SetMaskType(PlayerMaterial.MaskType.ScrollingUI);
            __instance.UpdateMaterials(colorChip.Inner.FrontLayer, hat);
            colorChip.Inner.SetHat(hat, __instance.HasLocalPlayer() ? CustomPlayer.LocalCustom.DefaultOutfit.ColorId : DataManager.Player.Customization.Color);
            colorChip.Inner.transform.localPosition = hat.ChipOffset;
            colorChip.Tag = hat;
            colorChip.SelectionHighlight.gameObject.SetActive(false);
            __instance.ColorChips.Add(colorChip);
        }

        return offset - ((hats.Count - 1) / __instance.NumPerRow * (isDefaultPackage ? 1f : 1.5f) * __instance.YOffset) - 1.75f;
    }

    public static bool Prefix(HatsTab __instance)
    {
        for (var i = 0; i < __instance.scroller.Inner.childCount; i++)
            __instance.scroller.Inner.GetChild(i).gameObject.Destroy();

        __instance.ColorChips = new();
        var array = HatManager.Instance.GetUnlockedHats();
        var packages = new Dictionary<string, List<HatData>>();

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

        var YOffset = __instance.YStart;
        Template = GameObject.Find("HatsGroup").transform.FindChild("Text").GetComponent<TMP_Text>();
        var keys = packages.Keys.OrderBy(x => x switch
        {
            "Innersloth" => 4,
            "Stream" => 1,
            "Misc" => 3,
            _ => 2
        });
        keys.ForEach(key => YOffset = CreateHatPackage(packages[key], key, YOffset, __instance));
        __instance.currentHatIsEquipped = true;
        __instance.SetScrollerBounds();
        __instance.scroller.ContentYBounds.max = -(YOffset + 4.1f);
        return false;
    }
}