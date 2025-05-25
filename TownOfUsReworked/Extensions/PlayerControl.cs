using Innersloth.Assets;

namespace TownOfUsReworked.Extensions;

public static class PlayerControlExtensions
{
    public static bool HasDied(this PlayerControl player) => !player || !player.Data || player.Data.IsDead || player.Data.Disconnected;

    public static void RawSetHat(this PlayerControl player, string hatId, UColor color) => player.cosmetics.SetHat(hatId, color);

    private static void SetHat(this CosmeticsLayer layer, string hatId, UColor color)
    {
        if (layer.hat)
            layer.hat.SetHat(hatId, color);

        layer.OnCosmeticSet?.Invoke(hatId, -2, CosmeticsLayer.CosmeticKind.HAT);
    }

    private static void SetHat(this HatParent parent, string hatId, UColor color)
    {
        if (HatManager.InstanceExists)
            parent.SetHat(HatManager.Instance.GetHatById(hatId), color);
    }

    private static void SetHat(this HatParent parent, HatData hat, UColor color)
    {
        if (!hat || hat != parent.Hat)
        {
            parent.BackLayer.sprite = null;
            parent.FrontLayer.sprite = null;
        }

        parent.Hat = hat;
        parent.SetHat(color);
    }

    private static void SetHat(this HatParent parent, UColor color)
    {
        if (!parent.Hat)
            return;

        var props = parent.matProperties;
        props.ColorId = -2;
        parent.matProperties = props;
        parent.UpdateMaterial(color);
        parent.UnloadAsset();
        parent.viewAsset = parent.Hat.CreateAddressableAsset();
        parent.viewAsset.LoadAsync((Action)parent.PopulateFromViewData);
    }

    public static void UpdateMaterial(this HatParent parent, object colorVal)
    {
        HatViewData viewData = null;

        try
        {
            viewData = parent.viewAsset.GetAsset();
        }
        catch
        {
            if (!parent.Hat)
                return;

            if (HatLoader.CustomCosmeticRegistry.TryGetValue(parent.Hat.ProductId, out var ch))
                viewData = ch.ViewData;
        }

        if (!viewData)
            return;

        var maskType = parent.matProperties.MaskType;
        var loaded = parent.IsLoaded && viewData.MatchPlayerColor;

        parent.BackLayer.sharedMaterial = parent.FrontLayer.sharedMaterial = maskType switch
        {
            PlayerMaterial.MaskType.ComplexUI or PlayerMaterial.MaskType.ScrollingUI => loaded ? HatManager.Instance.MaskedPlayerMaterial : HatManager.Instance.MaskedMaterial,
            _ => loaded ? HatManager.Instance.PlayerMaterial : HatManager.Instance.DefaultShader
        };

        parent.BackLayer.maskInteraction = parent.FrontLayer.maskInteraction = maskType switch
        {
            PlayerMaterial.MaskType.SimpleUI => SpriteMaskInteraction.VisibleInsideMask,
            PlayerMaterial.MaskType.Exile => SpriteMaskInteraction.VisibleOutsideMask,
            _ => SpriteMaskInteraction.None
        };

        parent.BackLayer.material.SetInt(PlayerMaterial.MaskLayer, parent.matProperties.MaskLayer);
        parent.FrontLayer.material.SetInt(PlayerMaterial.MaskLayer, parent.matProperties.MaskLayer);

        if (parent.matProperties.MaskLayer <= 0)
        {
            PlayerMaterial.SetMaskLayerBasedOnLocalPlayer(parent.BackLayer, parent.matProperties.IsLocalPlayer);
            PlayerMaterial.SetMaskLayerBasedOnLocalPlayer(parent.FrontLayer, parent.matProperties.IsLocalPlayer);
        }

        if (loaded)
        {
            SetRendererColor(colorVal, parent.BackLayer);
            SetRendererColor(colorVal, parent.FrontLayer);
        }
    }

    public static void RawSetVisor(this PlayerControl player, string visorId, UColor color) => player.cosmetics.SetVisor(visorId, color);

    public static void SetVisor(this CosmeticsLayer layer, string visorId, UColor color)
    {
        var visorLayer = layer.visor;

        if (visorLayer)
            visorLayer.SetVisor(visorId, color);

        layer.OnCosmeticSet?.Invoke(visorId, -2, CosmeticsLayer.CosmeticKind.VISOR);
    }

    public static void SetVisor(this VisorLayer visor, string visorId, UColor color)
    {
        if (HatManager.InstanceExists)
            visor.SetVisor(HatManager.Instance.GetVisorById(visorId), color);
    }

    public static void SetVisor(this VisorLayer visor, VisorData data, UColor color)
    {
        if (!data || data != visor.visorData)
            visor.Image.sprite = null;

        var props = visor.matProperties;
        props.ColorId = -2;
        visor.matProperties = props;
        visor.UpdateMaterial(color);
        visor.UnloadAsset();
        visor.viewAsset = visor.visorData.CreateAddressableAsset();
        visor.viewAsset.LoadAsync((Action)visor.PopulateFromViewData);
    }

    public static void UpdateMaterial(this VisorLayer __instance, object colorVal)
    {
        VisorViewData viewData = null;

        try
        {
            viewData = __instance.viewAsset.GetAsset();
        }
        catch
        {
            if (!__instance.visorData)
                return;

            if (VisorLoader.CustomCosmeticRegistry.TryGetValue(__instance.visorData.ProductId, out var cv))
                viewData = cv.ViewData;
        }

        if (!viewData)
            return;

        var maskType = __instance.matProperties.MaskType;
        var loaded = __instance.IsLoaded && viewData.MatchPlayerColor;
        __instance.Image.sharedMaterial = maskType is PlayerMaterial.MaskType.ComplexUI or PlayerMaterial.MaskType.ScrollingUI
            ? (loaded ? HatManager.Instance.MaskedPlayerMaterial : HatManager.Instance.MaskedMaterial)
            : (loaded ? HatManager.Instance.PlayerMaterial : HatManager.Instance.DefaultShader);
        __instance.Image.maskInteraction = maskType switch
        {
            PlayerMaterial.MaskType.SimpleUI => SpriteMaskInteraction.VisibleInsideMask,
            PlayerMaterial.MaskType.Exile => SpriteMaskInteraction.VisibleOutsideMask,
            _ => SpriteMaskInteraction.None
        };
        __instance.Image.material.SetInt(PlayerMaterial.MaskLayer, __instance.matProperties.MaskLayer);

        if (__instance.matProperties.MaskLayer <= 0)
            PlayerMaterial.SetMaskLayerBasedOnLocalPlayer(__instance.Image, __instance.matProperties.IsLocalPlayer);

        if (loaded)
            SetRendererColor(colorVal, __instance.Image);
    }

    public static void RawSetSkin(this PlayerControl player, string skinId, UColor color) => player.MyPhysics.SetSkin(skinId, color);

    public static void SetSkin(this PlayerPhysics physics, string skinId, UColor color)
    {
        physics.myPlayer.cosmetics.SetSkin(skinId, color, () =>
        {
            if (physics.Animations.IsPlayingSpawnAnimation())
                physics.myPlayer.cosmetics.AnimateSkinSpawn(physics.Animations.Time);

            if (Ship()?.Type != ShipStatus.MapType.Fungle)
                return;

            if (physics.myPlayer.inMovingPlat)
                physics.myPlayer.cosmetics.AnimateSkinJump();

            if (physics.Animations.IsPlayingClimbAnimation())
            {
                var flag = physics.Velocity.y <= 0f;
                physics.Animations.PlayClimbAnimation(flag);
                physics.myPlayer.cosmetics.AnimateClimb(flag);
            }
        });
    }

    public static void SetSkin(this CosmeticsLayer layer, string skinId, UColor color, Action onLoaded)
    {
        if (HatManager.InstanceExists)
            layer.SetSkin(HatManager.Instance.GetSkinById(skinId), color, onLoaded);
    }

    public static void SetSkin(this CosmeticsLayer layer, SkinData skin, UColor color, Action onLoaded)
    {
        if (!layer.skin)
            return;

        if (AprilFoolsMode.ShouldLongAround() && HatManager.Instance.CheckLongModeValidCosmetic(skin.ProductId, false))
            skin = HatManager.Instance.GetSkinById("skin_None");

        layer.skin.SetSkin(skin, color, layer.currentBodySprite.BodySprite.flipX, layer, onLoaded);
        layer.skin.Flipped = layer.currentBodySprite.BodySprite.flipX;
    }

    public static void SetSkin(this SkinLayer layer, SkinData skinData, UColor color, bool isLeft, CosmeticsLayer cosmeticsLayer, Action onLoaded )
    {
        layer.LoadAssetAsync(skinData.Cast<IAddressableAssetProvider<SkinViewData>>(), (Action<SkinViewData>)(skinView =>
        {
            if (layer.IsDestroyedOrNull() || layer.gameObject.IsDestroyedOrNull())
                return;

            layer.data = skinData;
            layer.SetSkin(skinView, color, isLeft);
            cosmeticsLayer.OnCosmeticSet?.Invoke(skinData.ProdId, -2, CosmeticsLayer.CosmeticKind.SKIN);
            onLoaded?.Invoke();
        }));
    }

    public static void SetSkin(this SkinLayer layer, SkinViewData skin, UColor color, bool isLeft)
    {
        layer.skin = skin;
        var props = layer.matProperties;
        props.ColorId = -2;
        layer.matProperties = props;
        layer.UpdateMaterial(color);
        layer.SetIdle(isLeft);
    }

    public static void UpdateMaterial(this SkinLayer __instance, object colorVal)
    {
        var maskType = __instance.matProperties.MaskType;
        var loaded = __instance.skin && __instance.IsLoaded && __instance.skin.MatchPlayerColor;
        __instance.layer.sharedMaterial = maskType is PlayerMaterial.MaskType.ComplexUI or PlayerMaterial.MaskType.ScrollingUI
            ? (loaded ? HatManager.Instance.MaskedPlayerMaterial : HatManager.Instance.MaskedMaterial)
            : (loaded ? HatManager.Instance.PlayerMaterial : HatManager.Instance.DefaultShader);
        __instance.layer.maskInteraction = maskType switch
        {
            PlayerMaterial.MaskType.SimpleUI => SpriteMaskInteraction.VisibleInsideMask,
            PlayerMaterial.MaskType.Exile => SpriteMaskInteraction.VisibleOutsideMask,
            _ => SpriteMaskInteraction.None
        };
        __instance.layer.material.SetInt(PlayerMaterial.MaskLayer, __instance.matProperties.MaskLayer);

        if (__instance.matProperties.MaskLayer <= 0)
            PlayerMaterial.SetMaskLayerBasedOnLocalPlayer(__instance.layer, __instance.matProperties.IsLocalPlayer);

        if (loaded)
            SetRendererColor(colorVal, __instance.layer);
    }

    public static void RawSetPet(this PlayerControl player, string petId, UColor color)
    {
        player.cosmetics.SetPetIdle(petId, color, () =>
        {
            player.cosmetics.SetPetSource(player);

            if (player.inMovingPlat)
                player.cosmetics.SetPetVisible(false);
        });
    }

    public static void SetPetIdle(this CosmeticsLayer layer, string petId, UColor color, Action onComplete)
    {
        if (HatManager.InstanceExists)
            layer.SetPetIdle(HatManager.Instance.GetPetById(petId), color, onComplete);
    }

    public static void SetPetIdle(this CosmeticsLayer layer, PetData petData, UColor color, Action onComplete)
    {
        layer.StopAllCoroutines();

        if (layer.currentPet && layer.currentPet.Data.ProdId == petData.ProdId)
        {
            layer.currentPet.SetIdle();
            onComplete?.Invoke();
        }
        else
            layer.StartCoroutine(layer.CoLoadAndSetPetIdle(petData, color, onComplete));
    }

    private static IEnumerator CoLoadAndSetPetIdle(this CosmeticsLayer layer, PetData petData, UColor color, Action onComplete)
    {
        layer.UnloadAddressableAsset(layer.petAsset);
        yield return layer.CoLoadAssetAsync(petData.Cast<IAddressableAssetProvider<PetBehaviour>>(), (Action<PetBehaviour>)(pet => layer.SetPetIdle(pet, color, onComplete)));
    }

    public static void SetPetIdle(this CosmeticsLayer layer, PetBehaviour petBehaviour, UColor color, Action onComplete)
    {
        layer.StopAllCoroutines();
        layer.InstantiatePetCopy(petBehaviour, color);
        layer.currentPet.SetIdle();
        layer.currentPet.Visible = layer.visible;
        onComplete?.Invoke();
        layer.OnCosmeticSet?.Invoke(petBehaviour.Data.ProdId, -2, CosmeticsLayer.CosmeticKind.PET);
    }

    private static void InstantiatePetCopy(this CosmeticsLayer layer, PetBehaviour petBehaviour, UColor color)
    {
        if (layer.currentPet)
        {
            UObject.Destroy(layer.currentPet.gameObject);
            layer.currentPet = null;
        }

        layer.currentPet = UObject.Instantiate(petBehaviour, layer.petParent);

        if (layer.uiPet)
            layer.currentPet.PrepareForUI();

        layer.currentPet.SetCrewmateColor(color);
        layer.currentPet.transform.localPosition = Vector3.zero;
        layer.currentPet.SetDefaultMaterial();
        layer.SetPetFlipX(layer.FlipX);

        if (layer.localPlayer)
            layer.currentPet.SetAsLocalPlayer();
    }

    public static void SetCrewmateColor(this PetBehaviour pet, object colorVal)
    {
        if (colorVal is int colorId and not (-2 or -1))
            pet.ForEachRenderer(false, (Action<SpriteRenderer>)(x => PlayerMaterial.SetColors(colorId, x)));
        else if (colorVal is UColor color)
            pet.ForEachRenderer(false, (Action<SpriteRenderer>)(x => PlayerMaterial.SetColors(color, x)));
    }

    private static void SetRendererColor(object colorVal, SpriteRenderer rend)
    {
        if (colorVal is int colorId and not (-2 or -1))
            PlayerMaterial.SetColors(colorId, rend);
        else if (colorVal is UColor color)
            PlayerMaterial.SetColors(color, rend);
    }
}