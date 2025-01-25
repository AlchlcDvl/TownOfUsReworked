namespace TownOfUsReworked.PlayerLayers.Modifiers;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Bait : Modifier
{
    [ToggleOption]
    public static bool BaitKnows = true;

    [NumberOption(0f, 15f, 0.5f, Format.Time)]
    public static Number BaitMinDelay = 0;

    [NumberOption(0f, 15f, 0.5f, Format.Time)]
    public static Number BaitMaxDelay = 1;

    public override UColor Color => ClientOptions.CustomModColors ? CustomColorManager.Bait : CustomColorManager.Modifier;
    public override LayerEnum Type => LayerEnum.Bait;
    public override Func<string> Description => () => "- Killing you causes the killer to report your body, albeit with a slight delay";
    public override bool Hidden => !BaitKnows && !Dead;

    public override void OnDeath(DeathReason reason, DeathReasonEnum reason2, PlayerControl killer)
    {
        if (killer != Player)
            BaitReport(killer, Player);
    }
}