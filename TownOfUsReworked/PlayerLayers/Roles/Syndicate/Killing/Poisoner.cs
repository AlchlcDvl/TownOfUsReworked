namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Poisoner)]
public sealed class Poisoner : Syndicate
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number PoisonCd = 25;

    [NumberOption(1f, 15f, 1f, Format.Time)]
    public static Number PoisonDur = 5;

    private CustomButton PoisonButton { get; set; }
    private CustomButton GlobalPoisonButton { get; set; }
    private PlayerControl PoisonedPlayer { get; set; }
    private CustomPlayerMenu PoisonMenu { get; set; }

    protected override UColor MainColor => CustomColorManager.Poisoner;
    public override LayerEnum Type => LayerEnum.Poisoner;
    public override Func<string> StartText { get; } = () => "Delay A Kill To Deceive The <#8CFFFFFF>Crew</color>";
    public override Func<string> Description => () => $"- You can poison players{(HoldsDrive ? " from afar" : "")}\n- Poisoned players will die after {PoisonDur}s\n" +
        CommonAbilities;

    public override void Init()
    {
        base.Init();
        PoisonedPlayer = null;
        Alignment = Alignment.Killing;
        PoisonMenu = new(Player, Click, Color, Exception1);
        PoisonButton ??= new(this, new SpriteName("Poison"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)HitPoison, new Cooldown(PoisonCd), "POISON", (UsableFunc)Usable1,
            new Duration(PoisonDur), (EffectEndVoid)UnPoison, (PlayerBodyExclusion)Exception1, (EndFunc)EndEffect);
        GlobalPoisonButton ??= new(this, new SpriteName("GlobalPoison"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)HitGlobalPoison, (LabelFunc)Label,
            new Cooldown(PoisonCd), new Duration(PoisonDur), (EffectEndVoid)UnPoison, (UsableFunc)Usable2, (EndFunc)EndEffect);
    }

    public override void Reset(bool meeting, bool start) => PoisonedPlayer = null;

    private bool EndEffect() => PoisonedPlayer.HasDied() || Dead;

    private void UnPoison()
    {
        if (!PoisonedPlayer.HasDied() && CanAttack(AttackEnum.Basic, PoisonedPlayer.GetDefenseValue(Player)))
            Player.MurderPlayer(PoisonedPlayer, DeathReasonEnum.Poisoned, false);

        PoisonedPlayer = null;
    }

    private void Click(PlayerControl player)
    {
        var cooldown = Interact(Player, player, astral: true, delayed: true);

        if (cooldown != CooldownType.Fail)
            PoisonedPlayer = player;
        else
            GlobalPoisonButton.StartCooldown(cooldown);
    }

    private string Label() => PoisonedPlayer ? "POISON" : "SET TARGET";

    private bool Usable1() => !HoldsDrive;

    private bool Usable2() => HoldsDrive;

    public override void UpdateHud(HudManager __instance)
    {
        if (!HoldsDrive || !KeyboardJoystick.player.GetButtonDown("Delete"))
            return;

        if (PoisonedPlayer && !(PoisonButton.EffectActive || GlobalPoisonButton.EffectActive))
            PoisonedPlayer = null;

        Message("Removed a target");
    }

    private bool Exception1(PlayerControl player) => player == PoisonedPlayer || Player.IsBuddyWith(player, Faction);

    private void HitPoison(PlayerControl target)
    {
        var cooldown = Interact(Player, target, true, delayed: true);

        if (cooldown != CooldownType.Fail)
        {
            PoisonedPlayer = target;
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, PoisonButton, PoisonedPlayer);
            PoisonButton.Begin();
        }
        else
            PoisonButton.StartCooldown(cooldown);
    }

    private void HitGlobalPoison()
    {
        if (PoisonedPlayer)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, GlobalPoisonButton, PoisonedPlayer);
            GlobalPoisonButton.Begin();
        }
        else
            PoisonMenu.Open();
    }

    public override void ReadRPC(RpcReader reader) => PoisonedPlayer = reader.ReadPlayer();
}