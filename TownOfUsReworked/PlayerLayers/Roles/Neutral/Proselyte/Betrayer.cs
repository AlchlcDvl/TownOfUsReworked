namespace TownOfUsReworked.PlayerLayers.Roles;

public class Betrayer : Neutral
{
    public CustomButton KillButton { get; set; }

    public override UColor Color => ClientGameOptions.CustomNeutColors ? CustomColorManager.Betrayer : CustomColorManager.Neutral;
    public override string Name => "Betrayer";
    public override LayerEnum Type => LayerEnum.Betrayer;
    public override Func<string> StartText => () => "Those Backs Are Ripe For Some Stabbing";
    public override Func<string> Description => () => "- You can kill";

    public Betrayer() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        BaseStart();
        Objectives = () => $"- Kill anyone who opposes the {FactionName}";
        Alignment = Alignment.NeutralPros;
        KillButton = new(this, "BetKill", AbilityTypes.Alive, "ActionSecondary", Kill, CustomGameOptions.BetrayCd, Exception);
        return this;
    }

    public void Kill() => KillButton.StartCooldown(Interact(Player, KillButton.TargetPlayer, true));

    public bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        Player.IsLinkedTo(player);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        KillButton.Update2("BETRAY");
    }
}