namespace TownOfUsReworked.PlayerLayers.Roles;

public class Poisoner : Syndicate
{
    public CustomButton PoisonButton { get; set; }
    public CustomButton GlobalPoisonButton { get; set; }
    public PlayerControl PoisonedPlayer { get; set; }
    public CustomMenu PoisonMenu { get; set; }

    public override Color Color => ClientGameOptions.CustomSynColors ? Colors.Poisoner : Colors.Syndicate;
    public override string Name => "Poisoner";
    public override LayerEnum Type => LayerEnum.Poisoner;
    public override Func<string> StartText => () => "Delay A Kill To Decieve The <color=#8CFFFFFF>Crew</color>";
    public override Func<string> Description => () => $"- You can poison players{(HoldsDrive ? " from afar" : "")}\n- Poisoned players will die after {CustomGameOptions.PoisonDur}s\n" +
        CommonAbilities;

    public Poisoner(PlayerControl player) : base(player)
    {
        PoisonedPlayer = null;
        Alignment = Alignment.SyndicateKill;
        PoisonMenu = new(Player, Click, Exception1);
        PoisonButton = new(this, "Poison", AbilityTypes.Target, "ActionSecondary", HitPoison, CustomGameOptions.PoisonCd, CustomGameOptions.PoisonDur, UnPoison, Exception1);
        GlobalPoisonButton = new(this, "GlobalPoison", AbilityTypes.Targetless, "ActionSecondary", HitGlobalPoison, CustomGameOptions.PoisonCd, CustomGameOptions.PoisonDur, UnPoison);
    }

    public bool End() => PoisonedPlayer.HasDied() || IsDead;

    public void UnPoison()
    {
        if (!(PoisonedPlayer.HasDied() || PoisonedPlayer.Is(LayerEnum.Pestilence)))
            RpcMurderPlayer(Player, PoisonedPlayer, DeathReasonEnum.Poisoned, false);

        PoisonedPlayer = null;
    }

    public void Click(PlayerControl player)
    {
        var interact = Interact(Player, player, poisoning: true);

        if (interact.AbilityUsed)
            PoisonedPlayer = player;
        else if (interact.Reset)
            GlobalPoisonButton.StartCooldown();
        else if (interact.Protected)
            GlobalPoisonButton.StartCooldown(CooldownType.GuardianAngel);
        else if (interact.Vested)
            GlobalPoisonButton.StartCooldown(CooldownType.Survivor);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        GlobalPoisonButton.Update2(PoisonedPlayer == null && HoldsDrive ? "SET TARGET" : "POISON", HoldsDrive);
        PoisonButton.Update2("POISON", !HoldsDrive);

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (PoisonedPlayer != null && HoldsDrive && !(PoisonButton.EffectActive || GlobalPoisonButton.EffectActive))
                PoisonedPlayer = null;

            LogInfo("Removed a target");
        }
    }

    public bool Exception1(PlayerControl player) => player == PoisonedPlayer || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) &&
        SubFaction != SubFaction.None);

    public void HitPoison()
    {
        var interact = Interact(Player, PoisonButton.TargetPlayer, poisoning: true);

        if (interact.AbilityUsed)
        {
            PoisonedPlayer = PoisonButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, PoisonButton, PoisonedPlayer);
            PoisonButton.Begin();
        }
        else if (interact.Reset)
            PoisonButton.StartCooldown();
        else if (interact.Protected)
            PoisonButton.StartCooldown(CooldownType.GuardianAngel);
        else if (interact.Vested)
            PoisonButton.StartCooldown(CooldownType.Survivor);
    }

    public void HitGlobalPoison()
    {
        if (PoisonedPlayer == null)
            PoisonMenu.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, PoisonButton, PoisonedPlayer);
            GlobalPoisonButton.Begin();
        }
    }

    public override void ReadRPC(MessageReader reader) => PoisonedPlayer = reader.ReadPlayer();
}