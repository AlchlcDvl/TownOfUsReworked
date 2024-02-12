namespace TownOfUsReworked.PlayerLayers.Roles;

public class Poisoner : Syndicate
{
    public CustomButton PoisonButton { get; set; }
    public CustomButton GlobalPoisonButton { get; set; }
    public PlayerControl PoisonedPlayer { get; set; }
    public CustomMenu PoisonMenu { get; set; }

    public override UColor Color => ClientGameOptions.CustomSynColors ? CustomColorManager.Poisoner : CustomColorManager.Syndicate;
    public override string Name => "Poisoner";
    public override LayerEnum Type => LayerEnum.Poisoner;
    public override Func<string> StartText => () => "Delay A Kill To Decieve The <color=#8CFFFFFF>Crew</color>";
    public override Func<string> Description => () => $"- You can poison players{(HoldsDrive ? " from afar" : "")}\n- Poisoned players will die after {CustomGameOptions.PoisonDur}s\n" +
        CommonAbilities;

    public Poisoner() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        BaseStart();
        PoisonedPlayer = null;
        Alignment = Alignment.SyndicateKill;
        PoisonMenu = new(Player, Click, Exception1);
        PoisonButton = new(this, "Poison", AbilityTypes.Alive, "ActionSecondary", HitPoison, CustomGameOptions.PoisonCd, CustomGameOptions.PoisonDur, UnPoison, Exception1);
        GlobalPoisonButton = new(this, "GlobalPoison", AbilityTypes.Targetless, "ActionSecondary", HitGlobalPoison, CustomGameOptions.PoisonCd, CustomGameOptions.PoisonDur, UnPoison);
        return this;
    }

    public bool End() => PoisonedPlayer.HasDied() || IsDead;

    public void UnPoison()
    {
        if (CanAttack(AttackEnum.Basic, PoisonedPlayer.GetDefenseValue(Player)))
            RpcMurderPlayer(Player, PoisonedPlayer, DeathReasonEnum.Poisoned, false);

        PoisonedPlayer = null;
    }

    public void Click(PlayerControl player)
    {
        var cooldown = Interact(Player, player, astral: true, delayed: true);

        if (cooldown != CooldownType.Fail)
            PoisonedPlayer = player;
        else
            GlobalPoisonButton.StartCooldown(cooldown);
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

            LogMessage("Removed a target");
        }
    }

    public bool Exception1(PlayerControl player) => player == PoisonedPlayer || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) &&
        SubFaction != SubFaction.None);

    public void HitPoison()
    {
        var cooldown = Interact(Player, PoisonButton.TargetPlayer, delayed: true);

        if (cooldown != CooldownType.Fail)
        {
            PoisonedPlayer = PoisonButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, PoisonButton, PoisonedPlayer);
            PoisonButton.Begin();
        }
        else
            PoisonButton.StartCooldown(cooldown);
    }

    public void HitGlobalPoison()
    {
        if (PoisonedPlayer == null)
            PoisonMenu.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, GlobalPoisonButton, PoisonedPlayer);
            GlobalPoisonButton.Begin();
        }
    }

    public override void ReadRPC(MessageReader reader) => PoisonedPlayer = reader.ReadPlayer();
}