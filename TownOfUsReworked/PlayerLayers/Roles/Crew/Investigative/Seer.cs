namespace TownOfUsReworked.PlayerLayers.Roles;

public class Seer : Crew
{
    public DateTime LastSeered { get; set; }
    public bool ChangedDead => !AllRoles.Any(x => x.Player != null && !x.IsDead && !x.Disconnected && (x.RoleHistory.Count > 0 || x.Is(LayerEnum.Amnesiac) || x.Is(LayerEnum.Thief) ||
        x.Player.Is(LayerEnum.Traitor) || x.Is(LayerEnum.VampireHunter) || x.Is(LayerEnum.Godfather) || x.Is(LayerEnum.Mafioso) || x.Is(LayerEnum.Shifter) || x.Is(LayerEnum.Guesser) ||
        x.Is(LayerEnum.Rebel) || x.Is(LayerEnum.Mystic) || (x.Is(LayerEnum.Seer) && x != this) || x.Is(LayerEnum.Sidekick) || x.Is(LayerEnum.GuardianAngel) || x.Is(LayerEnum.Executioner) ||
        x.Is(LayerEnum.BountyHunter) || x.Player.Is(LayerEnum.Fanatic)));
    public CustomButton SeerButton { get; set; }
    public float Timer => ButtonUtils.Timer(Player, LastSeered, CustomGameOptions.SeerCooldown);

    public override Color32 Color => ClientGameOptions.CustomCrewColors ? Colors.Seer : Colors.Crew;
    public override string Name => "Seer";
    public override LayerEnum Type => LayerEnum.Seer;
    public override Func<string> StartText => () => "You Can See People's Histories";
    public override Func<string> Description => () => "- You can investigate players to see if their roles have changed\n- If all players whose roles changed have died, you will " +
        "become a <color=#FFCC80FF>Sheriff</color>";
    public override InspectorResults InspectorResults => InspectorResults.GainsInfo;

    public Seer(PlayerControl player) : base(player)
    {
        RoleAlignment = RoleAlignment.CrewInvest;
        SeerButton = new(this, "Seer", AbilityTypes.Direct, "ActionSecondary", See);
    }

    public void TurnSheriff()
    {
        var role = new Sheriff(Player);
        role.RoleUpdate(this);

        if (Local)
            Flash(Colors.Sheriff);
    }

    public void See()
    {
        if (Timer != 0f || IsTooFar(Player, SeerButton.TargetPlayer))
            return;

        var interact = Interact(Player, SeerButton.TargetPlayer);

        if (interact[3])
            Flash(GetRole(SeerButton.TargetPlayer).RoleHistory.Count > 0 || SeerButton.TargetPlayer.IsFramed() ? UColor.red : UColor.green);

        if (interact[0])
            LastSeered = DateTime.UtcNow;
        else if (interact[1])
            LastSeered.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        SeerButton.Update("SEE", Timer, CustomGameOptions.SeerCooldown);

        if (ChangedDead && !IsDead)
        {
            CallRpc(CustomRPC.Change, TurnRPC.TurnSheriff, this);
            TurnSheriff();
        }
    }
}