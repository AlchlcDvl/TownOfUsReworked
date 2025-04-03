namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.GuardianAngel)]
public sealed class GuardianAngel : Neutral
{
    [ToggleOption]
    public static bool GuardianAngelCanPickTargets = false;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number ProtectCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    private static Number ProtectDur = 10;

    [NumberOption(0, 15, 1, zeroIsInf: true)]
    private static Number MaxProtects = 5;

    [MultiSelectOption<ProtectOptions>(NoneValue = ProtectOptions.Nobody, AllValue = ProtectOptions.Everyone )]
    public static MultiSelectValue<ProtectOptions> ShowProtect = ProtectOptions.Protected;

    [ToggleOption]
    public static bool GaTargetKnows = false;

    [ToggleOption]
    public static bool ProtectBeyondTheGrave = false;

    [ToggleOption]
    private static bool GaKnowsTargetRole = false;

    [ToggleOption]
    private static bool GaVent = false;

    [ToggleOption]
    private static bool GaSwitchVent = false;

    [ToggleOption]
    private static bool GaToSurv = true;

    public PlayerControl TargetPlayer { get; set; }
    public bool TargetAlive => !Disconnected && !TargetPlayer.HasDied();
    private CustomButton ProtectButton { get; set; }
    public CustomButton GraveProtectButton { get; private set; }
    private int Rounds { get; set; }
    private CustomButton TargetButton { get; set; }
    public bool Failed => TargetPlayer ? !TargetAlive : Rounds > 2;
    public bool Protecting { get; private set; }

    protected override UColor MainColor => CustomColorManager.GuardianAngel;
    public override LayerEnum Type => LayerEnum.GuardianAngel;
    public override Func<string> StartText => () => "Find Someone To Protect";
    public override Func<string> Description => () => !TargetPlayer ? "- You can select a player to be your target" : ($"- You can protect {TargetPlayer?.name} from death for a short while" +
        $"\n- If {TargetPlayer?.name} dies, you will become a <#DDDD00FF>Survivor</color>");
    public override bool CanVent => base.CanVent && GaVent;
    public override bool CanSwitchVents => GaSwitchVent;

    protected override void Init()
    {
        base.Init();
        Objectives = () => !TargetPlayer ? "- Find a target to protect" : $"- Have {TargetPlayer?.name} live to the end of the game";
        Alignment = Alignment.Benign;
        TargetPlayer = null;
        ProtectButton ??= new(this, new SpriteName("Protect"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)HitProtect, new Cooldown(ProtectCd), "PROTECT",
            new Duration(ProtectDur), MaxProtects, (UsableFunc)Usable1, (EndFunc)EndEffect, (EffectStartVoid)ProtectStart, (EffectEndVoid)ProtectEnd);

        if (ProtectBeyondTheGrave)
        {
            GraveProtectButton ??= new(this, new SpriteName("GraveProtect"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)HitGraveProtect, new PostDeath(true),
                "PROTECT", new Cooldown(ProtectCd), new Duration(ProtectDur), MaxProtects, (UsableFunc)Usable1, (EndFunc)EndEffect, (EffectStartVoid)ProtectStart, (EffectEndVoid)ProtectEnd);
        }

        Rounds = 0;
    }

    public override void Reset(bool meeting, bool start)
    {
        if (meeting && !TargetPlayer)
            Rounds++;
    }

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (player != TargetPlayer)
            return;

        name += " <#FFFFFFFF>★</color>";

        if (player.IsProtected() && ShowProtect == ProtectOptions.Ga)
            name += " <#FFFFFFFF>η</color>";

        if (!GaKnowsTargetRole || revealed)
            return;

        var role = handler.CustomRole;
        color = role.Color;
        name += $"\n{role}";
        revealed = true;
    }

    public override void PostAssignment()
    {
        if (GuardianAngelCanPickTargets || !TargetPlayer)
            TargetButton ??= new(this, new SpriteName("GATarget"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)SelectTarget, "WATCH", (UsableFunc)Usable2);
    }

    public override List<PlayerControl> Team()
    {
        var team = base.Team();

        if (TargetPlayer)
            team.Add(TargetPlayer);

        return team;
    }

    private void SelectTarget(PlayerControl target)
    {
        TargetPlayer = target;
        CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, this, TargetPlayer);
    }

    private void TurnSurv() => new Survivor().RoleUpdate(this);

    private void HitProtect()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, ProtectButton);
        ProtectButton.Begin();
        GraveProtectButton.Uses--;
        TrulyDead = GraveProtectButton.Uses <= 0 && Dead;
    }

    private void HitGraveProtect()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, GraveProtectButton);
        GraveProtectButton.Begin();
        ProtectButton.Uses--;
        TrulyDead = ProtectButton.Uses <= 0 && Dead;
    }

    private void ProtectStart() => Protecting = true;

    private void ProtectEnd() => Protecting = false;

    private bool Usable1() => !Failed && TargetPlayer && TargetAlive;

    private bool Usable2() => !TargetPlayer;

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (!Failed || Dead || GaToSurv)
            return;

        if (GuardianAngelCanPickTargets)
        {
            TargetPlayer = null;
            Rounds = 0;
            CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, this, 255);
        }
        else
            Player.RpcSuicide();
    }

    public override void UpdatePlayer()
    {
        base.UpdatePlayer();

        if (Failed && !Dead && GaToSurv)
            TurnSurv();
    }

    private bool EndEffect() => Dead || !TargetAlive;
}