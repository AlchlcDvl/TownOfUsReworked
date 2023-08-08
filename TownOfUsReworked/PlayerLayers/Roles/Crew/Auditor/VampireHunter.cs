namespace TownOfUsReworked.PlayerLayers.Roles;

public class VampireHunter : Crew
{
    public DateTime LastStaked { get; set; }
    public static bool VampsDead => !CustomPlayer.AllPlayers.Any(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(SubFaction.Undead));
    public CustomButton StakeButton { get; set; }
    public float Timer => ButtonUtils.Timer(Player, LastStaked, CustomGameOptions.StakeCooldown);

    public override Color32 Color => ClientGameOptions.CustomCrewColors ? Colors.VampireHunter : Colors.Crew;
    public override string Name => "Vampire Hunter";
    public override LayerEnum Type => LayerEnum.VampireHunter;
    public override Func<string> StartText => () => "Stake The <color=#7B8968FF>Undead</color>";
    public override Func<string> Description => () => "- You can stake players to see if they have been turned\n- When you stake a turned person, or an <color=#7B8968FF>Undead" +
        "</color> tries to interact with you, you will kill them\n- When all <color=#7B8968FF>Undead</color> players die, you will become a <color=#FFFF00FF>Vigilante</color>";
    public override InspectorResults InspectorResults => InspectorResults.TracksOthers;

    public VampireHunter(PlayerControl player) : base(player)
    {
        RoleAlignment = RoleAlignment.CrewAudit;
        StakeButton = new(this, "Stake", AbilityTypes.Direct, "ActionSecondary", Stake);
    }

    public void TurnVigilante()
    {
        var newRole = new Vigilante(Player);
        newRole.RoleUpdate(this);

        if (Local)
            Flash(Colors.Vigilante);

        if (CustomPlayer.Local.Is(LayerEnum.Seer))
            Flash(Colors.Seer);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        StakeButton.Update("STAKE", Timer, CustomGameOptions.StakeCooldown);

        if (VampsDead && !IsDead)
        {
            CallRpc(CustomRPC.Change, TurnRPC.TurnVigilante, this);
            TurnVigilante();
        }
    }

    public void Stake()
    {
        if (IsTooFar(Player, StakeButton.TargetPlayer) || Timer != 0f)
            return;

        var interact = Interact(Player, StakeButton.TargetPlayer, StakeButton.TargetPlayer.Is(SubFaction.Undead) || StakeButton.TargetPlayer.IsFramed());

        if (interact[3] || interact[0])
            LastStaked = DateTime.UtcNow;
        else if (interact[1])
            LastStaked.AddSeconds(CustomGameOptions.ProtectKCReset);
        else if (interact[2])
            LastStaked.AddSeconds(CustomGameOptions.VestKCReset);
    }
}