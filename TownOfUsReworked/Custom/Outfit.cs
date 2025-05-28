namespace TownOfUsReworked.Custom;

public sealed class CustomOutfit
{
    public int ColorId { get; set; } = -1;
    public string HatId { get; set; } = "hat_NoHat";
    public string PetId { get; set; } = "pet_EmptyPet";
    public string SkinId { get; set; } = "skin_None";
    public string VisorId { get; set; } = "visor_EmptyVisor";
    public string NamePlateId { get; set; } = "nameplate_NoPlate";
    public string PlayerName { get; set; } = " ";
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
    }

    public CustomOutfit(PlayerControl player, bool useCurrent = false) : this(useCurrent ? player.CurrentOutfit : player.Data.DefaultOutfit)
    {
        var lobby = IsLobby();
        Size = lobby ? 1f : player.GetSize();
        Speed = lobby ? 1f : player.GetSpeed();
        Alpha = lobby ? 1f : player.GetAlpha();
        Color = ColorId is -1 or -2 ? player.MyRend().material.GetColor(PlayerMaterial.BodyColor) : ColorId.GetColor(false);
    }

    public override string ToString() => $"name={PlayerName},colorId={ColorId},hat={HatId},pet={PetId},skin={SkinId},visor={VisorId},nameplate={NamePlateId},size={Size},speed={Speed},alpha={Alpha},color=({Color.r},{Color.g},{Color.b},{Color.a})";

    public ColorPair GetPair() => ColorId is -1 or -2 ? new(Color, Color.Shadow()) : new(ColorId.GetColor(false), ColorId.GetColor(true));

    public static implicit operator PlayerOutfit(CustomOutfit outfit) => new()
    {
        ColorId = outfit.ColorId,
        HatId = outfit.HatId,
        PetId = outfit.PetId,
        SkinId = outfit.SkinId,
        VisorId = outfit.VisorId,
        NamePlateId = outfit.NamePlateId,
        PlayerName = outfit.PlayerName
    };

    public static implicit operator CustomOutfit(PlayerOutfit outfit) => new(outfit);
}