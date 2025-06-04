namespace TownOfUsReworked.Custom;

public sealed class CustomOutfit
{
    public string HatId { get; init; } = "hat_NoHat";
    public string PetId { get; init; } = "pet_EmptyPet";
    public string SkinId { get; init; } = "skin_None";
    public string VisorId { get; init; } = "visor_EmptyVisor";
    public string NamePlateId { get; init; } = "nameplate_NoPlate";
    public string PlayerName { get; init; } = " ";
    public string ColorName { get; init; } = "???";
    public float Size { get; init; } = 1f;
    public float Speed { get; init; } = 1f;
    public float Alpha { get; init; } = 1f;
    public Color32 Color { get; init; } = UColor.white;

    public int ColorId
    {
        get;
        init
        {
            field = value;

            if (!CustomColorManager.AllColors.TryGetValue(value, out var color))
                return;

            var translation = Palette.GetColorName(value);
            ColorName = color.Default ? (translation[0] + translation[1..].ToLower()) : translation;
        }
    } = -1;

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

    public CustomOutfit(PlayerControl player, bool useCurrent = false) : this(useCurrent ? player.CurrentOutfit : player.Data.Outfits[PlayerOutfitType.Default])
    {
        var lobby = IsLobby();
        Size = lobby ? 1f : player.GetSize();
        Speed = lobby ? 1f : player.GetSpeed();
        Alpha = lobby ? 1f : player.GetAlpha();
        Color = ColorId is -1 or -2 ? player.MyRend().material.GetColor(PlayerMaterial.BodyColor) : ColorId.GetColor(false);
    }

    public override string ToString() => $"name={PlayerName},colorId={ColorId},colorName={ColorName},hat={HatId},pet={PetId},skin={SkinId},visor={VisorId},nameplate={NamePlateId},size={Size},speed={Speed},alpha={Alpha},color=({Color.r},{Color.g},{Color.b},{Color.a})";

    public ColorPair GetPair() => ColorId is -1 or -2 ? new(Color, Color.Shadow()) : new(ColorId.GetColor(false), ColorId.GetColor(true));

    public string GetLightOrDark() => (ColorId is -1 or -2 ? Color.IsDark() : !ColorId.IsLighter()) ? "D" : "L";

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