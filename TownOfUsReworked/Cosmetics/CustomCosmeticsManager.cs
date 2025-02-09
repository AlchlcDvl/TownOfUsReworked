namespace TownOfUsReworked.Cosmetics;

public static class CustomCosmeticsManager
{
    public static Sprite CreateCosmeticSprite(string path, CosmeticTypeEnum cosmetic)
    {
        var texture = LoadDiskTexture(path);
        return CreateSprite(texture, path.SanitisePath(), cosmetic switch
        {
            CosmeticTypeEnum.Hat or CosmeticTypeEnum.Visor => 100f,
            _ => texture.width * 0.375f
        });
    }
}