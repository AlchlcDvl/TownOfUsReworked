namespace TownOfUsReworked.PlayerLayers.Modifiers;

public sealed class Colorblind : Modifier
{
    protected override UColor MainColor => CustomColorManager.Colorblind;
    public override LayerEnum Type => LayerEnum.Colorblind;
    public override Func<string> Description => () => "- You can't tell the difference between players";

    private bool AllToNormal { get; set; }

    public override void Init()
    {
        if (Local)
            ColorAll();
    }

    public override void ExitingLayer()
    {
        AllToNormal = true;
        CameraEffectHandler.RemoveEffect("SoundV");
    }

    public override void EnteringLayer()
    {
        ColorAll();
        CameraEffectHandler.AddEffect("SoundV");
    }

    public override void OnMeetingEnd(MeetingHud __instance)
    {
        if (Dead)
            AllToNormal = true;
    }

    private bool Normalise() => AllToNormal;

    private void ColorAll()
    {
        foreach (var player in AllPlayers())
        {
            if (player != Player)
                player.OverrideOutfit(ColorblindOutfit(), CustomPlayerOutfitType.Colorblind, -1, Normalise);
        }
    }
}