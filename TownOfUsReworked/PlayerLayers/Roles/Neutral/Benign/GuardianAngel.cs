namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class GuardianAngel : Neutral
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool GuardianAngelCanPickTargets { get; set; } = false;

    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number ProtectCd { get; set; } = new(25);

    [NumberOption(MultiMenu.LayerSubOptions, 5f, 30f, 1f, Format.Time)]
    public static Number ProtectDur { get; set; } = new(10);

    [NumberOption(MultiMenu.LayerSubOptions, 0, 15, 1, zeroIsInf: true)]
    public static Number MaxProtects { get; set; } = new(5);

    [StringOption(MultiMenu.LayerSubOptions)]
    public static ProtectOptions ShowProtect { get; set; } = ProtectOptions.Protected;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool GATargetKnows { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ProtectBeyondTheGrave { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool GAKnowsTargetRole { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool GAVent { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool GASwitchVent { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool GAToSurv { get; set; } = true;

    public PlayerControl TargetPlayer { get; set; }
    public bool TargetAlive => !Disconnected && !TargetPlayer.HasDied();
    public CustomButton ProtectButton { get; set; }
    public CustomButton GraveProtectButton { get; set; }
    public int Rounds { get; set; }
    public CustomButton TargetButton { get; set; }
    public bool Failed => TargetPlayer ? !TargetAlive : Rounds > 2;
    public bool Protecting { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.GuardianAngel : FactionColor;
    public override LayerEnum Type => LayerEnum.GuardianAngel;
    public override Func<string> StartText => () => "Find Someone To Protect";
    public override Func<string> Description => () => !TargetPlayer ? "- You can select a player to be your target" : ($"- You can protect {TargetPlayer?.name} from death for a short while" +
        $"\n- If {TargetPlayer?.name} dies, you will become a <#DDDD00FF>Survivor</color>");

    public override void Init()
    {
        base.Init();
        Objectives = () => !TargetPlayer ? "- Find a target to protect" : $"- Have {TargetPlayer?.name} live to the end of the game";
        Alignment = Alignment.NeutralBen;
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

    public override void PostAssignment()
    {
        if (GuardianAngelCanPickTargets || !TargetPlayer)
            TargetButton ??= new(this, new SpriteName("GATarget"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)SelectTarget, "WATCH", (UsableFunc)Usable2);
    }

    public override List<PlayerControl> Team()
    {
        var team = base.Team();
        team.Add(TargetPlayer);
        return team;
    }

    public void SelectTarget(PlayerControl target)
    {
        TargetPlayer = target;
        CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, this, TargetPlayer);
    }

    public void TurnSurv() => new Survivor().RoleUpdate(this);

    public void HitProtect()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, ProtectButton);
        ProtectButton.Begin();
        GraveProtectButton.Uses--;
        TrulyDead = GraveProtectButton.Uses <= 0 && Dead;
    }

    public void HitGraveProtect()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, GraveProtectButton);
        GraveProtectButton.Begin();
        ProtectButton.Uses--;
        TrulyDead = ProtectButton.Uses <= 0 && Dead;
    }

    public void ProtectStart() => Protecting = true;

    public void ProtectEnd() => Protecting = false;

    public bool Usable1() => !Failed && TargetPlayer && TargetAlive;

    public bool Usable2() => !TargetPlayer;

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (Failed && !Dead && !GAToSurv)
        {
            if (GuardianAngelCanPickTargets)
            {
                TargetPlayer = null;
                Rounds = 0;
                CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, this, 255);
            }
            else
                RpcMurderPlayer(Player);
        }
    }

    public override void UpdatePlayer()
    {
        if (Failed && !Dead && GAToSurv)
            TurnSurv();
    }

    public bool EndEffect() => Dead || !TargetAlive;
}