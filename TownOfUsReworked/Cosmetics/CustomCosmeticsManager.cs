namespace TownOfUsReworked.Cosmetics;

public static class CustomCosmeticsManager
{
    public static Sprite CreateCosmeticSprite(string path, CosmeticTypeEnum cosmetic)
    {
        var texture = LoadDiskTexture(path);
        var size = cosmetic switch
        {
            CosmeticTypeEnum.Hat or CosmeticTypeEnum.Visor => 100f,
            CosmeticTypeEnum.Nameplate or _ => texture.width * 0.375f
        };
        var sprite = Sprite.Create(texture, new(0f, 0f, texture.width, texture.height), new(0.5f, 0.5f), size);
        sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
        return sprite;
    }
}