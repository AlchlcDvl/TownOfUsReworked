namespace TownOfUsReworked.Custom;

public sealed class CustomOutfit : PlayerOutfit
{
    public float Size { get; set; } = 1f;
    public float Speed { get; set; } = 1f;
    public float Alpha { get; set; } = 1f;
    public Color32 Color { get; set; } = UColor.white;

    public CustomOutfit() {}

    public CustomOutfit(PlayerOutfit source)
    {
        ColorId = source.ColorId;
        HatId = source.HatId;
        SkinId = source.SkinId;
        VisorId = source.VisorId;
        NamePlateId = source.NamePlateId;
        PlayerName = source.PlayerName;
        PetId = source.PetId;

        if (!source.TryCast<CustomOutfit>(out var custom))
            return;

        Size = custom.Size;
        Speed = custom.Speed;
        Alpha = custom.Alpha;
        Color = custom.Color;
    }

    public CustomOutfit(PlayerControl player) : this(player.CurrentOutfit)
    {
        Size = player.GetSize();
        Speed = player.GetSpeed();
        Alpha = player.GetAlpha();
        Color = ColorId == -1 ? player.MyRend().material.GetColor(PlayerMaterial.BodyColor) : ColorId.GetColor(false);
    }

    public override string ToString() => $"{base.ToString()},size={Size},speed={Speed},alpha={Alpha}";
}