namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Betrayer : Neutral
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static float BetrayCd { get; set; } = 25f;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool BetrayerVent { get; set; } = true;

    public CustomButton KillButton { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Betrayer : CustomColorManager.Neutral;
    public override string Name => "Betrayer";
    public override LayerEnum Type => LayerEnum.Betrayer;
    public override Func<string> StartText => () => "Those Backs Are Ripe For Some Stabbing";
    public override Func<string> Description => () => "- You can kill";

    public override void Init()
    {
        BaseStart();
        Objectives = () => $"- Kill anyone who opposes the {FactionName}";
        Alignment = Alignment.NeutralPros;
        KillButton = CreateButton(this, new SpriteName("BetrayerKill"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Kill, new Cooldown(BetrayCd), "BETRAY",
            (PlayerBodyExclusion)Exception);
    }

    public void Kill() => KillButton.StartCooldown(Interact(Player, KillButton.TargetPlayer, true));

    public bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        Player.IsLinkedTo(player);
}