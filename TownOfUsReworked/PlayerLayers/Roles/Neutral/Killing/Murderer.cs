namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Murderer : Neutral
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number MurderCd { get; set; } = new(25);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool MurdVent { get; set; } = false;

    public CustomButton MurderButton { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Murderer : CustomColorManager.Neutral;
    public override string Name => "Murderer";
    public override LayerEnum Type => LayerEnum.Murderer;
    public override Func<string> StartText => () => "I Got Murder On My Mind";
    public override Func<string> Description => () => "- You can kill";
    public override AttackEnum AttackVal => AttackEnum.Basic;
    public override DefenseEnum DefenseVal => DefenseEnum.Basic;

    public override void Init()
    {
        BaseStart();
        Objectives = () => "- Murder anyone who can oppose you";
        Alignment = Alignment.NeutralKill;
        MurderButton = CreateButton(this, new SpriteName("Murder"), AbilityType.Alive, KeybindType.ActionSecondary, (OnClick)Murder, new Cooldown(MurderCd), "MURDER",
            (PlayerBodyExclusion)Exception);
    }

    public void Murder() => MurderButton.StartCooldown(Interact(Player, MurderButton.GetTarget<PlayerControl>(), true));

    public bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        Player.IsLinkedTo(player);
}