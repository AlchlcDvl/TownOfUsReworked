namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Murderer : Neutral
{
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
        MurderButton = CreateButton(this, new SpriteName("Murder"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Murder, new Cooldown(CustomGameOptions.MurderCd), "MURDER",
            (PlayerBodyExclusion)Exception);
    }

    public void Murder() => MurderButton.StartCooldown(Interact(Player, MurderButton.TargetPlayer, true));

    public bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        Player.IsLinkedTo(player);
}