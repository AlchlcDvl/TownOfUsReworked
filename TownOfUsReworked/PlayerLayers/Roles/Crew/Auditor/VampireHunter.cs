namespace TownOfUsReworked.PlayerLayers.Roles;

public class VampireHunter : Crew
{
    public static bool VampsDead => !CustomPlayer.AllPlayers.Any(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(SubFaction.Undead));
    public CustomButton StakeButton { get; set; }

    public override Color Color => ClientGameOptions.CustomCrewColors ? Colors.VampireHunter : Colors.Crew;
    public override string Name => "Vampire Hunter";
    public override LayerEnum Type => LayerEnum.VampireHunter;
    public override Func<string> StartText => () => "Stake The <color=#7B8968FF>Undead</color>";
    public override Func<string> Description => () => "- You can stake players to see if they have been turned\n- When you stake a turned person, or an <color=#7B8968FF>Undead</color> " +
        "tries to interact with you, you will kill them\n- When all <color=#7B8968FF>Undead</color> players die, you will become a <color=#FFFF00FF>Vigilante</color>";

    public VampireHunter(PlayerControl player) : base(player)
    {
        Alignment = Alignment.CrewAudit;
        StakeButton = new(this, "Stake", AbilityTypes.Target, "ActionSecondary", Stake, CustomGameOptions.StakeCd);
    }

    public void TurnVigilante() => new Vigilante(Player).RoleUpdate(this);

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

    public void Stake()
    {
        var interact = Interact(Player, StakeButton.TargetPlayer, StakeButton.TargetPlayer.Is(SubFaction.Undead) || StakeButton.TargetPlayer.IsFramed());
        var cooldown = CooldownType.Reset;

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;
        else if (interact.Vested)
            cooldown = CooldownType.Survivor;

        StakeButton.StartCooldown(cooldown);
    }
}