namespace TownOfUsReworked.PlayerLayers.Roles;

public class VampireHunter : Crew
{
    private static bool VampsDead => !CustomPlayer.AllPlayers.Any(x => !x.HasDied() && x.Is(SubFaction.Undead));
    private CustomButton StakeButton { get; set; }

    public override UColor Color => ClientGameOptions.CustomCrewColors ? CustomColorManager.VampireHunter : CustomColorManager.Crew;
    public override string Name => "Vampire Hunter";
    public override LayerEnum Type => LayerEnum.VampireHunter;
    public override Func<string> StartText => () => "Stake The <color=#7B8968FF>Undead</color>";
    public override Func<string> Description => () => "- You can stake players to see if they have been turned\n- When you stake a turned person, or an <color=#7B8968FF>Undead</color> " +
        "tries to interact with you, you will kill them\n- When all <color=#7B8968FF>Undead</color> players die, you will become a <color=#FFFF00FF>Vigilante</color>";

    public VampireHunter() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        BaseStart();
        Alignment = Alignment.CrewAudit;
        StakeButton = new(this, "Stake", AbilityTypes.Alive, "ActionSecondary", Stake, CustomGameOptions.StakeCd);
        return this;
    }

    public void TurnVigilante() => new Vigilante().Start<Role>(Player).RoleUpdate(this);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        StakeButton.Update2("STAKE");

        if (VampsDead && !IsDead)
        {
            CallRpc(CustomRPC.Change, TurnRPC.TurnVigilante, this);
            TurnVigilante();
        }
    }

    private void Stake() => StakeButton.StartCooldown(Interact(Player, StakeButton.TargetPlayer, ShouldKill(StakeButton.TargetPlayer)));

    private static bool ShouldKill(PlayerControl player) => player != null && (player.Is(SubFaction.Undead) || player.IsFramed());
}