using PowerTools;
using static TownOfUsReworked.Managers.CustomHatManager;

namespace TownOfUsReworked.Patches;

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

            if (extend.FlipImage)
                hp.FrontLayer.sprite = __instance.FlipX ? extend.FlipImage : viewData.MainImage;

            if (extend.BackFlipImage)
                hp.BackLayer.sprite = __instance.FlipX ? extend.BackFlipImage : viewData.BackImage;
        } catch {}
    }
}

[HarmonyPatch(typeof(HatParent))]
public static class HatPatches
{
    [HarmonyPatch(nameof(HatParent.UpdateMaterial)), HarmonyPrefix]
    public static bool UpdateMaterialPrefix(HatParent __instance)
    {
        HatViewData asset;

        try
        {
            asset = __instance.viewAsset.GetAsset();
            return true;
        }
        catch
        {
            try
            {
                if (!CustomHatViewDatas.TryGetValue(__instance.Hat.ProductId, out asset))
                    return true;
            }
            catch
            {
                return false;
            }
        }

        if (!asset)
            return false;

        var maskType = __instance.matProperties.MaskType;

        if (__instance.IsLoaded && asset.MatchPlayerColor)
        {
            if (maskType is PlayerMaterial.MaskType.ComplexUI or PlayerMaterial.MaskType.ScrollingUI)
            {
                __instance.BackLayer.sharedMaterial = HatManager.Instance.MaskedPlayerMaterial;
                __instance.FrontLayer.sharedMaterial = HatManager.Instance.MaskedPlayerMaterial;
            }
            else
            {
                __instance.BackLayer.sharedMaterial = HatManager.Instance.PlayerMaterial;
                __instance.FrontLayer.sharedMaterial = HatManager.Instance.PlayerMaterial;
            }
        }
        else if (maskType is PlayerMaterial.MaskType.ComplexUI or PlayerMaterial.MaskType.ScrollingUI)
        {
            __instance.BackLayer.sharedMaterial = HatManager.Instance.MaskedMaterial;
            __instance.FrontLayer.sharedMaterial = HatManager.Instance.MaskedMaterial;
        }
        else
        {
            __instance.BackLayer.sharedMaterial = HatManager.Instance.DefaultShader;
            __instance.FrontLayer.sharedMaterial = HatManager.Instance.DefaultShader;
        }

        __instance.BackLayer.maskInteraction = __instance.FrontLayer.maskInteraction = maskType switch
        {
            PlayerMaterial.MaskType.SimpleUI => SpriteMaskInteraction.VisibleInsideMask,
            PlayerMaterial.MaskType.Exile => SpriteMaskInteraction.VisibleOutsideMask,
            _ => SpriteMaskInteraction.None
        };

        __instance.BackLayer.material.SetInt(PlayerMaterial.MaskLayer, __instance.matProperties.MaskLayer);
        __instance.FrontLayer.material.SetInt(PlayerMaterial.MaskLayer, __instance.matProperties.MaskLayer);

        if (__instance.IsLoaded && __instance.viewAsset.GetAsset().MatchPlayerColor)
        {
            PlayerMaterial.SetColors(__instance.matProperties.ColorId, __instance.BackLayer);
            PlayerMaterial.SetColors(__instance.matProperties.ColorId, __instance.FrontLayer);
        }

        if (__instance.matProperties.MaskLayer <= 0)
        {
            PlayerMaterial.SetMaskLayerBasedOnLocalPlayer(__instance.BackLayer, __instance.matProperties.IsLocalPlayer);
            PlayerMaterial.SetMaskLayerBasedOnLocalPlayer(__instance.FrontLayer, __instance.matProperties.IsLocalPlayer);
        }

        return false;
    }

    [HarmonyPatch(nameof(HatParent.PopulateFromViewData)), HarmonyPrefix]
    public static bool PopulateFromViewDataPrefix(HatParent __instance)
    {
        HatViewData asset = null;

        try
        {
            asset = __instance.viewAsset.GetAsset();
            return true;
        } catch {}

        if (!__instance.Hat || !CustomHatViewDatas.TryGetValue(__instance.Hat?.ProductId, out asset) || !asset)
            return true;

        __instance.UpdateMaterial();
        __instance.SpriteSyncNode ??= __instance.GetComponent<SpriteAnimNodeSync>();

        if (__instance.SpriteSyncNode)
            __instance.SpriteSyncNode.NodeId = __instance.Hat.NoBounce ? 1 : 0;

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

        if (__instance.HideHat())
        {
            __instance.FrontLayer.enabled = false;
            __instance.BackLayer.enabled = false;
        }

        return false;
    }

    [HarmonyPatch(nameof(HatParent.LateUpdate)), HarmonyPrefix]
    public static bool LateUpdatePrefix(HatParent __instance)
    {
        if (!__instance.Parent || !__instance.Hat)
            return false;

        HatViewData hatViewData;

        try
        {
            hatViewData = __instance.viewAsset.GetAsset();
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

        if (!hatViewData)
            return false;

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
            __instance.SpriteSyncNode ??= __instance.GetComponent<SpriteAnimNodeSync>();

            if (__instance.SpriteSyncNode)
                __instance.SpriteSyncNode.NodeId = 0;
        }

        return false;
    }

    [HarmonyPatch(nameof(HatParent.SetHat), typeof(int)), HarmonyPrefix]
    public static bool SetHatPrefix(HatParent __instance, int color)
    {
        if (!CustomHatViewDatas.ContainsKey(__instance.Hat.ProductId))
            return true;

        __instance.UnloadAsset();
        __instance.viewAsset = null;
        __instance.SetMaterialColor(color);
        __instance.PopulateFromViewData();
        return false;
    }

    [HarmonyPatch(nameof(HatParent.SetFloorAnim))]
    public static bool SetFloorAnimPrefix(HatParent __instance)
    {
        HatViewData hatViewData;

        try
        {
            hatViewData = __instance.viewAsset.GetAsset();
            return true;
        } catch {}

        if (!CustomHatViewDatas.TryGetValue(__instance.Hat.ProductId, out hatViewData))
            return true;

        __instance.BackLayer.enabled = false;
        __instance.FrontLayer.enabled = true;
        __instance.FrontLayer.sprite = hatViewData.FloorImage;
        return false;
    }

    [HarmonyPatch(nameof(HatParent.SetIdleAnim)), HarmonyPrefix]
    public static bool SetIdleAnimPrefix(HatParent __instance, int colorId)
    {
        if (!__instance.Hat)
            return false;

        if (!CustomHatViewDatas.TryGetValue(__instance.Hat.ProductId, out var hatViewData))
            return true;

        __instance.viewAsset = null;
        __instance.PopulateFromViewData();
        __instance.SetMaterialColor(colorId);
        return false;
    }

    [HarmonyPatch(nameof(HatParent.SetClimbAnim)), HarmonyPrefix]
    public static bool SetClimbAnimPrefix(HatParent __instance)
    {
        HatViewData hatViewData;

        try
        {
            hatViewData = __instance.viewAsset.GetAsset();
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
}

[HarmonyPatch(typeof(HatsTab), nameof(HatsTab.OnEnable))]
public static class HatsTabOnEnablePatch
{
    private static TMP_Text Template;

    private static float CreateHatPackage(List<HatData> hats, string packageName, float YStart, HatsTab __instance)
    {
        var isDefaultPackage = "Innersloth" == packageName;

        if (!isDefaultPackage)
            hats = [ .. hats.OrderBy(x => x.name) ];

        var offset = YStart;

        if (Template)
        {
            var title = UObject.Instantiate(Template, __instance.scroller.Inner);
            var material = title.GetComponent<MeshRenderer>().material;
            material.SetFloat("_StencilComp", 4f);
            material.SetFloat("_Stencil", 1f);
            title.transform.localPosition = new(2.25f, YStart, -1f);
            title.transform.localScale = Vector3.one * 1.5f;
            title.fontSize *= 0.5f;
            title.enableAutoSizing = false;
            title.GetComponent<TextTranslatorTMP>().Destroy();
            title.SetText(packageName);
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
                colorChip.Button.OverrideOnMouseOverListeners(() => __instance.SelectHat(hat));
                colorChip.Button.OverrideOnMouseOutListeners(() => __instance.SelectHat(HatManager.Instance.GetHatById(DataManager.Player.Customization.Hat)));
                colorChip.Button.OverrideOnClickListeners(__instance.ClickEquip);
            }
            else
                colorChip.Button.OverrideOnClickListeners(() => __instance.SelectHat(hat));

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
            colorChip.Inner.SetMaskType(PlayerMaterial.MaskType.SimpleUI);
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
        __instance.PlayerPreview.gameObject.SetActive(true);

        if (__instance.HasLocalPlayer())
            __instance.PlayerPreview.UpdateFromLocalPlayer(PlayerMaterial.MaskType.None);
        else
            __instance.PlayerPreview.UpdateFromDataManager(PlayerMaterial.MaskType.None);

        __instance.scroller.Inner.DestroyChildren();
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

            if (!packages.TryGetValue(package, out var value))
                packages[package] = value = [];

            value.Add(data);
        }

        var YOffset = __instance.YStart;
        Template = __instance.transform.FindChild("Text").GetComponent<TMP_Text>();
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

        if (array.Length != 0)
            __instance.GetDefaultSelectable().PlayerEquippedForeground.SetActive(true);

        return false;
    }
}