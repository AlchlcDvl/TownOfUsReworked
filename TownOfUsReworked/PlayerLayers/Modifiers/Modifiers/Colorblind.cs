namespace TownOfUsReworked.PlayerLayers.Modifiers;

public class Colorblind : Modifier
{
    public override UColor Color => ClientOptions.CustomModColors ? CustomColorManager.Colorblind : CustomColorManager.Modifier;
    public override string Name => "Colorblind";
    public override LayerEnum Type => LayerEnum.Colorblind;
    public override Func<string> Description => () => "- You can't tell the difference between players";

    public override void Init()
    {
        if (Local)
            ColorAll();
    }

    public override void Deinit() => AllToNormal();

    public override void ExitingLayer()
    {
        AllToNormal();
        CameraEffect.Instance.Materials.Clear();
    }

    public override void EnteringLayer()
    {
        ColorAll();
        CameraEffect.Initialize();
        CameraEffect.Instance.Materials.Clear();
        CameraEffect.Instance.Materials.Add(UnityGet<Material>("SoundV"));
    }

    public override void UpdateHud(HudManager __instance)
    {
        if (!Dead)
            ColorAll();
    }

    public override void OnMeetingEnd(MeetingHud __instance)
    {
        if (Dead)
            AllToNormal();
    }

    private static void ColorChar(PlayerControl player)
    {
        var fit = player.GetCustomOutfitType();

        if (fit is not (CustomPlayerOutfitType.Colorblind or CustomPlayerOutfitType.Invis or CustomPlayerOutfitType.PlayerNameOnly))
            player.SetOutfit(CustomPlayerOutfitType.Colorblind, ColorblindOutfit());
    }

    private static void AllToNormal()
    {
        foreach (var p in AllPlayers())
        {
            DefaultOutfit(p);
            p.MyRend().color = UColor.white;
        }
    }

    private void ColorAll()
    {
        foreach (var player in AllPlayers())
        {
            if (player == Player)
                continue;

            ColorChar(player);
        }
    }
}