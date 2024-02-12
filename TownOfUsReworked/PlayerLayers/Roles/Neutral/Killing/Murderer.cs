namespace TownOfUsReworked.PlayerLayers.Roles;

public class Murderer : Neutral
{
    public CustomButton MurderButton { get; set; }

    public override UColor Color => ClientGameOptions.CustomNeutColors ? CustomColorManager.Murderer : CustomColorManager.Neutral;
    public override string Name => "Murderer";
    public override LayerEnum Type => LayerEnum.Murderer;
    public override Func<string> StartText => () => "I Got Murder On My Mind";
    public override Func<string> Description => () => "- You can kill";
    public override AttackEnum AttackVal => AttackEnum.Basic;
    public override DefenseEnum DefenseVal => DefenseEnum.Basic;

    public Murderer() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        BaseStart();
        Objectives = () => "- Murder anyone who can oppose you";
        Alignment = Alignment.NeutralKill;
        MurderButton = new(this, "Murder", AbilityTypes.Alive, "ActionSecondary", Murder, CustomGameOptions.MurderCd, Exception);
        return this;
    }

    public void Murder() => MurderButton.StartCooldown(Interact(Player, MurderButton.TargetPlayer, true));

    public bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        Player.IsLinkedTo(player);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        MurderButton.Update2("MURDER");
    }
}