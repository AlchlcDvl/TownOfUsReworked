using Innersloth.Assets;

namespace TownOfUsReworked.Extensions;

public static class PlayerControlExtensions
{
    public static void OverrideOutfit(this PlayerControl player, CustomOutfit outfit, PlayerOutfitType type, float duration = -1, Func<bool> func = null, Action concurrent = null) =>
        player.GetComponent<AppearanceHandler>().OverrideOutfit(outfit, type, duration, func, concurrent);

    public static void QueueOutfit(this PlayerControl player, CustomOutfit outfit, PlayerOutfitType type, float duration = -1, Func<bool> func = null, Action concurrent = null) =>
        player.GetComponent<AppearanceHandler>().QueueOutfit(outfit, type, duration, func, concurrent);

    public static void SetMimicked(this PlayerControl player, PlayerControl mimicked, float duration, Func<bool> func) => player.GetComponent<AppearanceHandler>().SetMimicked(mimicked, duration,
        func);

    public static void UpdateColor(this PlayerControl player, SpriteRenderer rend) => player.GetComponent<AppearanceHandler>().UpdateColor(rend);

    public static bool HasDied(this PlayerControl player) => !player || !player.Data || player.Data.IsDead || player.Data.Disconnected;

    public static void RawSetHat(this PlayerControl player, string hatId, ColorPair color) => player.cosmetics.SetHat(hatId, color);

    private static void SetHat(this CosmeticsLayer layer, string hatId, ColorPair color)
    {
        if (layer.hat)
            layer.hat.SetHat(hatId, color);

        layer.OnCosmeticSet?.Invoke(hatId, -2, CosmeticsLayer.CosmeticKind.HAT);
    }

    private static void SetHat(this HatParent parent, string hatId, ColorPair color)
    {
        if (HatManager.InstanceExists)
            parent.SetHat(HatManager.Instance.GetHatById(hatId), color);
    }

    private static void SetHat(this HatParent parent, HatData hat, ColorPair color)
    {
        if (!hat || hat != parent.Hat)
        {
            parent.BackLayer.sprite = null;
            parent.FrontLayer.sprite = null;
        }

        parent.Hat = hat;
        parent.SetHat(color);
    }

    private static void SetHat(this HatParent parent, ColorPair color)
    {
        if (!parent.Hat)
            return;

        parent.UnloadAsset();
        var props = parent.matProperties;
        props.ColorId = -2;
        parent.matProperties = props;
        parent.viewAsset = HatLoader.CustomCosmeticRegistry.ContainsKey(parent.Hat.ProductId) ? null : parent.Hat.CreateAddressableAsset();

        if (parent.viewAsset != null)
        {
            parent.viewAsset.LoadAsync((Action)(() =>
            {
                parent.PopulateFromViewData();
                parent.UpdateMaterial(color);
            }));
        }
        else
        {
            parent.PopulateFromViewData();
            parent.UpdateMaterial(color);
        }
    }

    public static void UpdateMaterial(this HatParent __instance, object colorVal)
    {
        HatViewData viewData = null;

        try
        {
            viewData = __instance.viewAsset.GetAsset();
        }
        catch
        {
            if (__instance.Hat && HatLoader.CustomCosmeticRegistry.TryGetValue(__instance.Hat.ProductId, out var ch))
                viewData = ch.ViewData;
        }

        UpdateMaterial(viewData && __instance.IsLoaded && viewData.MatchPlayerColor, __instance.matProperties, __instance.FrontLayer, colorVal);
        UpdateMaterial(viewData && __instance.IsLoaded && viewData.MatchPlayerColor, __instance.matProperties, __instance.BackLayer, colorVal);
    }

    public static void RawSetVisor(this PlayerControl player, string visorId, ColorPair color) => player.cosmetics.SetVisor(visorId, color);

    private static void SetVisor(this CosmeticsLayer layer, string visorId, ColorPair color)
    {
        if (layer.visor)
            layer.visor.SetVisor(visorId, color);

        layer.OnCosmeticSet?.Invoke(visorId, -2, CosmeticsLayer.CosmeticKind.VISOR);
    }

    private static void SetVisor(this VisorLayer visor, string visorId, ColorPair color)
    {
        if (HatManager.InstanceExists)
            visor.SetVisor(HatManager.Instance.GetVisorById(visorId), color);
    }

    private static void SetVisor(this VisorLayer visor, VisorData data, ColorPair color)
    {
        if (!data || data != visor.visorData)
            visor.Image.sprite = null;

        visor.UnloadAsset();
        var props = visor.matProperties;
        props.ColorId = -2;
        visor.matProperties = props;
        visor.visorData = data;
        visor.viewAsset = VisorLoader.CustomCosmeticRegistry.ContainsKey(data.ProductId) ? null : visor.visorData.CreateAddressableAsset();

        if (visor.viewAsset != null)
        {
            visor.viewAsset.LoadAsync((Action)(() =>
            {
                visor.PopulateFromViewData();
                visor.UpdateMaterial(color);
            }));
        }
        else
        {
            visor.PopulateFromViewData();
            visor.UpdateMaterial(color);
        }
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
            if (__instance.visorData && VisorLoader.CustomCosmeticRegistry.TryGetValue(__instance.visorData.ProductId, out var cv))
                viewData = cv.ViewData;
        }

        UpdateMaterial(viewData && __instance.IsLoaded && viewData.MatchPlayerColor, __instance.matProperties, __instance.Image, colorVal);
    }

    public static void RawSetSkin(this PlayerControl player, string skinId, ColorPair color) => player.MyPhysics.SetSkin(skinId, color);

    private static void SetSkin(this PlayerPhysics physics, string skinId, ColorPair color) => physics.myPlayer.cosmetics.SetSkin(skinId, color, () =>
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

    private static void SetSkin(this CosmeticsLayer layer, string skinId, ColorPair color, Action onLoaded)
    {
        if (HatManager.InstanceExists)
            layer.SetSkin(HatManager.Instance.GetSkinById(skinId), color, onLoaded);
    }

    private static void SetSkin(this CosmeticsLayer layer, SkinData skin, ColorPair color, Action onLoaded)
    {
        if (!layer.skin)
            return;

        if (AprilFoolsMode.ShouldLongAround() && HatManager.Instance.CheckLongModeValidCosmetic(skin.ProductId))
            skin = HatManager.Instance.GetSkinById("skin_None");

        layer.skin.SetSkin(skin, color, layer.currentBodySprite.BodySprite.flipX, layer, onLoaded);
        layer.skin.Flipped = layer.currentBodySprite.BodySprite.flipX;
    }

    private static void SetSkin(this SkinLayer layer, SkinData skinData, ColorPair color, bool isLeft, CosmeticsLayer cosmeticsLayer, Action onLoaded) =>
        layer.LoadAssetAsync(skinData.Cast<IAddressableAssetProvider<SkinViewData>>(), (Action<SkinViewData>)(skinView =>
        {
            if (layer.IsDestroyedOrNull() || layer.gameObject.IsDestroyedOrNull())
                return;

            layer.data = skinData;
            layer.SetSkin(skinView, color, isLeft);
            cosmeticsLayer.OnCosmeticSet?.Invoke(skinData.ProdId, -2, CosmeticsLayer.CosmeticKind.SKIN);
            onLoaded?.Invoke();
        }));

    private static void SetSkin(this SkinLayer layer, SkinViewData skin, ColorPair color, bool isLeft)
    {
        layer.skin = skin;
        var props = layer.matProperties;
        props.ColorId = -2;
        layer.matProperties = props;
        layer.UpdateMaterial(color);
        layer.SetIdle(isLeft);
    }

    public static void UpdateMaterial(this SkinLayer __instance, object colorVal) => UpdateMaterial(__instance.skin && __instance.IsLoaded && __instance.skin.MatchPlayerColor,
        __instance.matProperties, __instance.layer, colorVal);

    private static void UpdateMaterial(bool loaded, PlayerMaterial.Properties matProperties, SpriteRenderer rend, object colorVal)
    {
        rend.sharedMaterial = matProperties.MaskType is PlayerMaterial.MaskType.ComplexUI or PlayerMaterial.MaskType.ScrollingUI
            ? (loaded ? HatManager.Instance.MaskedPlayerMaterial : HatManager.Instance.MaskedMaterial)
            : (loaded ? HatManager.Instance.PlayerMaterial : HatManager.Instance.DefaultShader);
        rend.maskInteraction = matProperties.MaskType switch
        {
            PlayerMaterial.MaskType.SimpleUI => SpriteMaskInteraction.VisibleInsideMask,
            PlayerMaterial.MaskType.Exile => SpriteMaskInteraction.VisibleOutsideMask,
            _ => SpriteMaskInteraction.None
        };
        rend.material.SetInt(PlayerMaterial.MaskLayer, matProperties.MaskLayer);

        if (matProperties.MaskLayer <= 0)
            PlayerMaterial.SetMaskLayerBasedOnLocalPlayer(rend, matProperties.IsLocalPlayer);

        if (loaded)
            SetRendererColor(colorVal, rend);
    }

    public static void RawSetPet(this PlayerControl player, string petId, ColorPair color) => player.cosmetics.SetPetIdle(petId, color, () =>
    {
        player.cosmetics.SetPetSource(player);

        if (player.inMovingPlat)
            player.cosmetics.SetPetVisible(false);
    });

    private static void SetPetIdle(this CosmeticsLayer layer, string petId, ColorPair color, Action onComplete)
    {
        if (HatManager.InstanceExists)
            layer.SetPetIdle(HatManager.Instance.GetPetById(petId), color, onComplete);
    }

    private static void SetPetIdle(this CosmeticsLayer layer, PetData petData, ColorPair color, Action onComplete)
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

    private static IEnumerator CoLoadAndSetPetIdle(this CosmeticsLayer layer, PetData petData, ColorPair color, Action onComplete)
    {
        layer.UnloadAddressableAsset(layer.petAsset);
        yield return layer.CoLoadAssetAsync(petData.Cast<IAddressableAssetProvider<PetBehaviour>>(), (Action<PetBehaviour>)(pet => layer.SetPetIdle(pet, color, onComplete)));
    }

    private static void SetPetIdle(this CosmeticsLayer layer, PetBehaviour petBehaviour, ColorPair color, Action onComplete)
    {
        layer.StopAllCoroutines();
        layer.InstantiatePetCopy(petBehaviour, color);
        layer.currentPet.SetIdle();
        layer.currentPet.Visible = layer.visible;
        onComplete?.Invoke();
        layer.OnCosmeticSet?.Invoke(petBehaviour.Data.ProdId, -2, CosmeticsLayer.CosmeticKind.PET);
    }

    private static void InstantiatePetCopy(this CosmeticsLayer layer, PetBehaviour petBehaviour, ColorPair color)
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
            pet.ForEachRenderer(true, (Action<SpriteRenderer>)(x => PlayerMaterial.SetColors(colorId, x)));
        else if (colorVal is ColorPair pair)
            pet.ForEachRenderer(true, (Action<SpriteRenderer>)(x => Colors.Instance.SetRend(pair, x)));
    }

    private static void SetRendererColor(object colorVal, SpriteRenderer rend)
    {
        if (colorVal is int colorId and not (-2 or -1))
            PlayerMaterial.SetColors(colorId, rend);
        else if (colorVal is ColorPair pair)
            Colors.Instance.SetRend(pair, rend);
    }
}