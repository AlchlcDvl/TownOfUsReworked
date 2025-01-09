using PowerTools;
using static TownOfUsReworked.Managers.CustomHatManager;

namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleAnimation))]
public static class PlayerPhysicsHandleAnimationPatch
{
    public static void Postfix(PlayerPhysics __instance)
    {
        if (!__instance.myPlayer)
            return;

        var currentAnimation = __instance.Animations.Animator.GetCurrentAnimation();

        if (currentAnimation.IsAny(__instance.Animations.group.ClimbUpAnim, __instance.Animations.group.ClimbDownAnim))
            return;

        var hp = __instance.myPlayer.cosmetics.hat;

        if (!hp || !hp.Hat || !CustomHatRegistry.TryGetValue(hp.Hat.ProductId, out var ch))
            return;

        var viewData = ch.ViewData;
        hp.FrontLayer.sprite = __instance.FlipX && viewData.LeftMainImage ? viewData.LeftMainImage : viewData.MainImage;
        hp.BackLayer.sprite = __instance.FlipX && viewData.LeftBackImage ? viewData.LeftMainImage : viewData.BackImage;
    }
}

[HarmonyPatch(typeof(HatParent))]
public static class HatPatches
{
    [HarmonyPatch(nameof(HatParent.UpdateMaterial)), HarmonyPrefix]
    public static bool UpdateMaterialPrefix(HatParent __instance)
    {
        try
        {
            __instance.viewAsset.GetAsset();
            return true;
        } catch {}

        if (!__instance.Hat || !CustomHatRegistry.TryGetValue(__instance.Hat.ProductId, out var ch))
            return true;

        if (!ch.ViewData)
            return false;

        var maskType = __instance.matProperties.MaskType;

        if (__instance.IsLoaded && ch.ViewData.MatchPlayerColor)
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

        if (__instance.IsLoaded && ch.ViewData.MatchPlayerColor)
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

    [HarmonyPatch(nameof(HatParent.IsLoaded), MethodType.Getter)]
    public static bool Prefix(HatParent __instance, ref bool __result)
    {
        if (!__instance.Hat && !CustomHatRegistry.ContainsKey(__instance.Hat.ProductId))
            return true;

        return !(__result = true);
    }

    [HarmonyPatch(nameof(HatParent.PopulateFromViewData)), HarmonyPrefix]
    public static bool PopulateFromViewDataPrefix(HatParent __instance)
    {
        try
        {
            __instance.viewAsset.GetAsset();
            return true;
        } catch {}

        if (!__instance.Hat || !CustomHatRegistry.TryGetValue(__instance.Hat.ProductId, out var ch) || !ch.ViewData)
            return true;

        var asset = ch.ViewData;
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
        if (!__instance.Hat)
            return false;

        try
        {
            __instance.viewAsset.GetAsset();
            return true;
        } catch {}

        if (!CustomHatRegistry.TryGetValue(__instance.Hat.ProductId, out var ch) || !ch.ViewData)
            return false;

        if (__instance.FrontLayer.sprite != ch.ViewData.ClimbImage && __instance.FrontLayer.sprite != ch.ViewData.FloorImage)
        {
            if ((__instance.Hat.InFront || ch.ViewData.BackImage) && ch.ViewData.LeftMainImage)
                __instance.FrontLayer.sprite = __instance.Parent.flipX ? ch.ViewData.LeftMainImage : ch.ViewData.MainImage;

            if (ch.ViewData.BackImage && ch.ViewData.LeftBackImage)
            {
                __instance.BackLayer.sprite = __instance.Parent.flipX ? ch.ViewData.LeftBackImage : ch.ViewData.BackImage;
                return false;
            }

            if (!ch.ViewData.BackImage && !__instance.Hat.InFront && ch.ViewData.LeftMainImage)
            {
                __instance.BackLayer.sprite = __instance.Parent.flipX ? ch.ViewData.LeftMainImage : ch.ViewData.MainImage;
                return false;
            }
        }
        else if (__instance.FrontLayer.sprite == ch.ViewData.ClimbImage || __instance.FrontLayer.sprite == ch.ViewData.LeftClimbImage)
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
        if (!__instance.Hat || !CustomHatRegistry.TryGetValue(__instance.Hat.ProductId, out var ch))
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
        if (!__instance.Hat)
            return true;

        try
        {
            __instance.viewAsset.GetAsset();
            return true;
        } catch {}

        if (!CustomHatRegistry.TryGetValue(__instance.Hat.ProductId, out var ch))
            return true;

        __instance.BackLayer.enabled = false;
        __instance.FrontLayer.enabled = true;
        __instance.FrontLayer.flipX = false;
        __instance.FrontLayer.sprite = ch.ViewData.FloorImage;
        return false;
    }

    [HarmonyPatch(nameof(HatParent.SetClimbAnim)), HarmonyPrefix]
    public static bool SetClimbAnimPrefix(HatParent __instance)
    {
        if (!__instance.Hat)
            return true;

        try
        {
            __instance.viewAsset.GetAsset();
            return true;
        } catch {}

        if (!CustomHatRegistry.TryGetValue(__instance.Hat.ProductId, out var ch))
            return true;

        if (!__instance.options.ShowForClimb)
            return false;

        __instance.BackLayer.enabled = false;
        __instance.FrontLayer.enabled = true;
        __instance.FrontLayer.sprite = ch.ViewData.ClimbImage;
        return false;
    }
}

[HarmonyPatch(typeof(HatsTab), nameof(HatsTab.OnEnable))]
public static class HatsTabOnEnablePatch
{
    private static TMP_Text Template;

    private static void CreateHatPackage(List<HatData> hats, string packageName, ref float yStart, HatsTab __instance)
    {
        var isDefaultPackage = "Innersloth" == packageName;

        if (!isDefaultPackage)
            hats = [ .. hats.OrderBy(x => x.name) ];

        var offset = yStart;

        if (Template)
        {
            var title = UObject.Instantiate(Template, __instance.scroller.Inner);
            var material = title.GetComponent<MeshRenderer>().material;
            material.SetFloat("_StencilComp", 4f);
            material.SetFloat("_Stencil", 1f);
            title.transform.localPosition = new(2.25f, yStart, -1f);
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

            if (CustomHatRegistry.ContainsKey(hat.ProductId))
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
            yStart = ypos;
        }

        yStart -= 1.75f;
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
            var package = "Innersloth";

            if (CustomHatRegistry.TryGetValue(data.ProductId, out var ch))
                package = ch.StreamOnly ? "Stream" : ch.Artist;

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
        keys.ForEach(key => CreateHatPackage(packages[key], key, ref yOffset, __instance));
        __instance.currentHatIsEquipped = true;
        __instance.scroller.ContentYBounds.max = -(yOffset + 4.1f);
        __instance.scroller.UpdateScrollBars();

        if (array.Length != 0)
            __instance.GetDefaultSelectable().PlayerEquippedForeground.SetActive(true);

        return false;
    }
}