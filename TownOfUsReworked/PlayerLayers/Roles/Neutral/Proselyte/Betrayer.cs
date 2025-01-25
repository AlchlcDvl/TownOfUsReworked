namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Betrayer : Neutral
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number BetrayCd = 25;

    [ToggleOption]
    public static bool BetrayerVent = true;

    public CustomButton KillButton { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Betrayer : FactionColor;
    public override LayerEnum Type => LayerEnum.Betrayer;
    public override Func<string> StartText => () => "Those Backs Are Ripe For Some Stabbing";
    public override Func<string> Description => () => "- You can kill";
    public override AttackEnum AttackVal => AttackEnum.Basic;

    public override void Init()
    {
        base.Init();
        Objectives = () => $"- Kill anyone who opposes the {FactionName}";
        Alignment = Alignment.Proselyte;
        KillButton ??= new(this, new SpriteName("BetrayerKill"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Kill, new Cooldown(BetrayCd), "BACKSTAB",
            (PlayerBodyExclusion)Exception);
    }

    public void Kill(PlayerControl target) => KillButton.StartCooldown(Interact(Player, target, true));

    public bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        Player.IsLinkedTo(player);
}