namespace TownOfUsReworked.Custom;

public sealed class CustomOutfit : PlayerOutfit
{
    public float Size { get; set; } = 1f;
    public float Speed { get; set; } = 1f;
    public float Alpha { get; set; } = 1f;
    public Color32 Color { get; set; } = UColor.white;

    public CustomOutfit() : base(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<CustomOutfit>.NativeClassPtr)) {}

    public CustomOutfit(nint ptr) : base(ptr) {}

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
        var lobby = IsLobby();
        Size = lobby ? 1f : player.GetSize();
        Speed = lobby ? 1f : player.GetSpeed();
        Alpha = lobby ? 1f : player.GetAlpha();
        Color = ColorId is -1 or -2 ? player.MyRend().material.GetColor(PlayerMaterial.BodyColor) : ColorId.GetColor(false);
    }

    public override string ToString() => $"name={PlayerName},colorId={ColorId},hat={HatId},pet={PetId},skin={SkinId},visor={VisorId},nameplate={NamePlateId},size={Size},speed={Speed},alpha={Alpha},color=({Color.r},{Color.g},{Color.b},{Color.a})";
}