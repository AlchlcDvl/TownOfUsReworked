namespace TownOfUsReworked.Extensions;

public static class PlayerControlExtensions
{
    public static bool HasDied(this PlayerControl player) => !player || !player.Data || player.Data.IsDead || player.Data.Disconnected;

    public static void RawSetHat(this PlayerControl player, string hatId, UColor color)
    {
        player.cosmetics.SetHat(hatId, color);
        player.cosmetics.SetNamePosition(new(0f, IsNullEmptyOrWhiteSpace(hatId) ? 0.8f : 1f, -0.5f));
    }

    public static void SetHat(this CosmeticsLayer layer, string hatId, UColor color)
    {
        if (layer.hat)
            layer.hat.SetHat(hatId, color);

        layer.OnCosmeticSet?.Invoke(hatId, -2, CosmeticsLayer.CosmeticKind.HAT);
    }

    public static void SetHat(this HatParent parent, string hatId, UColor color)
    {
        if (HatManager.InstanceExists)
            parent.SetHat(HatManager.Instance.GetHatById(hatId), color);
    }

    public static void SetHat(this HatParent parent, HatData hat, UColor color)
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
        var viewData = (HatViewData)null;

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

        if (!loaded)
            return;

        if (colorVal is int colorId and not (-2 or -1))
        {
            PlayerMaterial.SetColors(colorId, parent.BackLayer);
            PlayerMaterial.SetColors(colorId, parent.FrontLayer);
        }
        else if (colorVal is UColor color)
        {
            PlayerMaterial.SetColors(color, parent.BackLayer);
            PlayerMaterial.SetColors(color, parent.FrontLayer);
        }
    }
}