namespace TownOfUsReworked.PlayerLayers.Roles;

public class Poisoner : Syndicate
{
    public CustomButton PoisonButton { get; set; }
    public CustomButton GlobalPoisonButton { get; set; }
    public DateTime LastPoisoned { get; set; }
    public PlayerControl PoisonedPlayer { get; set; }
    public float TimeRemaining { get; set; }
    public bool Enabled { get; set; }
    public bool Poisoned => TimeRemaining > 0f;
    public CustomMenu PoisonMenu { get; set; }

    public override Color32 Color => ClientGameOptions.CustomSynColors ? Colors.Poisoner : Colors.Syndicate;
    public override string Name => "Poisoner";
    public override LayerEnum Type => LayerEnum.Poisoner;
    public override Func<string> StartText => () => "Delay A Kill To Decieve The <color=#8CFFFFFF>Crew</color>";
    public override Func<string> Description => () => $"- You can poison players{(HoldsDrive ? " from afar" : "")}\n- Poisoned players will die after {CustomGameOptions.PoisonDur}"
        + $"s\n{CommonAbilities}";
    public override InspectorResults InspectorResults => InspectorResults.Unseen;
    public float Timer => ButtonUtils.Timer(Player, LastPoisoned, CustomGameOptions.PoisonCd);

    public Poisoner(PlayerControl player) : base(player)
    {
        PoisonedPlayer = null;
        RoleAlignment = RoleAlignment.SyndicateKill;
        PoisonMenu = new(Player, Click, Exception1);
        PoisonButton = new(this, "Poison", AbilityTypes.Direct, "ActionSecondary", HitPoison, Exception1);
        GlobalPoisonButton = new(this, "GlobalPoison", AbilityTypes.Effect, "ActionSecondary", HitGlobalPoison);
    }

    public void Poison()
    {
        Enabled = true;
        TimeRemaining -= Time.deltaTime;

        if (Meeting || PoisonedPlayer.Data.IsDead || PoisonedPlayer.Data.Disconnected || IsDead)
            TimeRemaining = 0f;
    }

    public void PoisonKill()
    {
        if (!(PoisonedPlayer.Data.IsDead || PoisonedPlayer.Data.Disconnected || PoisonedPlayer.Is(LayerEnum.Pestilence)))
            RpcMurderPlayer(Player, PoisonedPlayer, DeathReasonEnum.Poisoned, false);

        PoisonedPlayer = null;
        Enabled = false;
        LastPoisoned = DateTime.UtcNow;
    }

    public void Click(PlayerControl player)
    {
        var interact = Interact(Player, player);

        if (interact[3] && !player.IsProtected() && !player.IsVesting() && !player.IsProtectedMonarch())
            PoisonedPlayer = player;
        else if (interact[0])
            LastPoisoned = DateTime.UtcNow;
        else if (interact[1] || player.IsProtected())
            LastPoisoned.AddSeconds(CustomGameOptions.ProtectKCReset);
        else if (player.IsVesting())
            LastPoisoned.AddSeconds(CustomGameOptions.VestKCReset);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        var flag = PoisonedPlayer == null && HoldsDrive;
        GlobalPoisonButton.Update(flag ? "SET TARGET" : "POISON", Timer, CustomGameOptions.PoisonCd, Poisoned, TimeRemaining, CustomGameOptions.PoisonDur, true,
            HoldsDrive);
        PoisonButton.Update("POISON", Timer, CustomGameOptions.PoisonCd, Poisoned, TimeRemaining, CustomGameOptions.PoisonDur, true, !HoldsDrive);

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (PoisonedPlayer != null && HoldsDrive && !Poisoned)
                PoisonedPlayer = null;

            LogInfo("Removed a target");
        }
    }

    public bool Exception1(PlayerControl player) => player == PoisonedPlayer || player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None);

    public void HitPoison()
    {
        if (Timer != 0f || Poisoned || HoldsDrive || IsTooFar(Player, PoisonButton.TargetPlayer))
            return;

        var interact = Interact(Player, PoisonButton.TargetPlayer);

        if (interact[3] && !PoisonButton.TargetPlayer.IsProtected() && !PoisonButton.TargetPlayer.IsVesting() && !PoisonButton.TargetPlayer.IsProtectedMonarch())
        {
            PoisonedPlayer = PoisonButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.Poison, this, PoisonedPlayer);
            TimeRemaining = CustomGameOptions.PoisonDur;
            Poison();
        }
        else if (interact[1] || PoisonButton.TargetPlayer.IsProtected())
            LastPoisoned.AddSeconds(CustomGameOptions.ProtectKCReset);
        else if (interact[0])
            LastPoisoned = DateTime.UtcNow;
        else if (interact[2])
            LastPoisoned.AddSeconds(CustomGameOptions.VestKCReset);
    }

    public void HitGlobalPoison()
    {
        if (Timer != 0f || Poisoned || !HoldsDrive)
            return;

        if (PoisonedPlayer == null)
            PoisonMenu.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.Poison, this, PoisonedPlayer);
            TimeRemaining = CustomGameOptions.PoisonDur;
            Poison();
        }
    }
}