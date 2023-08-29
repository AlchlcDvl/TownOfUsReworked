namespace TownOfUsReworked.PlayerLayers.Roles;

public class GuardianAngel : Neutral
{
    public bool Enabled { get; set; }
    public DateTime LastProtected { get; set; }
    public float TimeRemaining { get; set; }
    public int UsesLeft { get; set; }
    public bool ButtonUsable => UsesLeft > 0;
    public PlayerControl TargetPlayer { get; set; }
    public bool TargetAlive => !Disconnected && !TargetPlayer.Data.IsDead && !TargetPlayer.Data.Disconnected;
    public bool Protecting => TimeRemaining > 0f;
    public CustomButton ProtectButton { get; set; }
    public CustomButton GraveProtectButton { get; set; }
    public int Rounds { get; set; }
    public CustomButton TargetButton { get; set; }
    public bool Failed => TargetPlayer == null && Rounds > 2;

    public override Color32 Color => ClientGameOptions.CustomNeutColors ? Colors.GuardianAngel : Colors.Neutral;
    public override string Name => "Guardian Angel";
    public override LayerEnum Type => LayerEnum.GuardianAngel;
    public override Func<string> StartText => () => "Find Someone To Protect";
    public override Func<string> Description => () => TargetPlayer == null ? "- You can select a player to be your target" : ($"- You can protect {TargetPlayer?.name} from death for "
        + $"a short while\n- If {TargetPlayer?.name} dies, you will become a <color=#DDDD00FF>Survivor</color>");
    public override InspectorResults InspectorResults => InspectorResults.PreservesLife;
    public float Timer => ButtonUtils.Timer(Player, LastProtected, CustomGameOptions.ProtectCd);

    public GuardianAngel(PlayerControl player) : base(player)
    {
        Objectives = () => TargetPlayer == null ? "- Find a target to protect" : $"- Have {TargetPlayer?.name} live to the end of the game";
        UsesLeft = CustomGameOptions.MaxProtects;
        RoleAlignment = RoleAlignment.NeutralBen;
        TargetPlayer = null;
        ProtectButton = new(this, "Protect", AbilityTypes.Effect, "ActionSecondary", HitProtect, true);
        TargetButton = new(this, "GATarget", AbilityTypes.Direct, "ActionSecondary", SelectTarget);
        Rounds = 0;

        if (CustomGameOptions.ProtectBeyondTheGrave)
            GraveProtectButton = new(this, "Protect", AbilityTypes.Effect, "ActionSecondary", HitProtect, true, true);
    }

    public void SelectTarget()
    {
        if (TargetPlayer != null)
            return;

        TargetPlayer = TargetButton.TargetPlayer;
        CallRpc(CustomRPC.Target, TargetRPC.SetGATarget, this, TargetPlayer);
    }

    public void Protect()
    {
        Enabled = true;
        TimeRemaining -= Time.deltaTime;

        if (Meeting)
            TimeRemaining = 0f;
    }

    public void TurnSurv()
    {
        var newRole = new Survivor(Player) { UsesLeft = UsesLeft };
        newRole.RoleUpdate(this);

        if (Local)
            Flash(Colors.Survivor);

        if (CustomPlayer.Local.Is(LayerEnum.Seer))
            Flash(Colors.Seer);
    }

    public void UnProtect()
    {
        Enabled = false;
        LastProtected = DateTime.UtcNow;
    }

    public void HitProtect()
    {
        if (!ButtonUsable || Timer != 0f || !TargetAlive || Protecting)
            return;

        TimeRemaining = CustomGameOptions.ProtectDur;
        UsesLeft--;
        Protect();
        CallRpc(CustomRPC.Action, ActionsRPC.GAProtect, this);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        ProtectButton.Update("PROTECT", Timer, CustomGameOptions.ProtectCd, UsesLeft, Protecting, TimeRemaining, CustomGameOptions.ProtectDur, true, !Failed &&
            TargetPlayer != null && TargetAlive);
        TargetButton.Update("WATCH", true, TargetPlayer == null);

        if (CustomGameOptions.ProtectBeyondTheGrave)
        {
            GraveProtectButton.Update("PROTECT", Timer, CustomGameOptions.ProtectCd, UsesLeft, Protecting, TimeRemaining, CustomGameOptions.ProtectDur, true, IsDead &&
                !Failed && TargetPlayer != null && TargetAlive);
        }

        if ((Failed || (TargetPlayer != null && !TargetAlive)) && !IsDead)
        {
            CallRpc(CustomRPC.Change, TurnRPC.TurnSurv, this);
            TurnSurv();
        }
    }
}