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

    public override void Init()
    {
        BaseStart();
        PoisonedPlayer = null;
        Alignment = Alignment.SyndicateKill;
        PoisonMenu = new(Player, Click, Exception1);
        PoisonButton = CreateButton(this, new SpriteName("Poison"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)HitPoison, new Cooldown(CustomGameOptions.PoisonCd), "POISON",
            new Duration(CustomGameOptions.PoisonDur), (EffectEndVoid)UnPoison, (PlayerBodyExclusion)Exception1, (UsableFunc)Usable1, (EndFunc)EndEffect);
        GlobalPoisonButton = CreateButton(this, new SpriteName("GlobalPoison"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClick)HitGlobalPoison, (LabelFunc)Label,
            new Cooldown(CustomGameOptions.PoisonCd), CustomGameOptions.PoisonDur, (EffectEndVoid)UnPoison, (UsableFunc)Usable2, (EndFunc)EndEffect);
    }

    public bool EndEffect() => PoisonedPlayer.HasDied() || Dead;

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

    public string Label() => PoisonedPlayer ? "POISON" : "SET TARGET";

    public bool Usable1() => !HoldsDrive;

    public bool Usable2() => HoldsDrive;

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (PoisonedPlayer && HoldsDrive && !(PoisonButton.EffectActive || GlobalPoisonButton.EffectActive))
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
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, PoisonButton, PoisonedPlayer);
            PoisonButton.Begin();
        }
        else
            PoisonButton.StartCooldown(cooldown);
    }

    public void HitGlobalPoison()
    {
        if (!PoisonedPlayer)
            PoisonMenu.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, GlobalPoisonButton, PoisonedPlayer);
            GlobalPoisonButton.Begin();
        }
    }

    public override void ReadRPC(MessageReader reader) => PoisonedPlayer = reader.ReadPlayer();
}