namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Glitch)]
public sealed class Glitch : OKilling, IBlocker
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number MimicCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    private static Number MimicDur = 10;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number HackCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    private static Number HackDur = 10;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number NeutraliseCd = 25;

    [ToggleOption]
    private static bool GlitchVent = false;

    private CustomButton HackButton;
    private CustomButton MimicButton;
    private CustomButton NeutraliseButton;
    public PlayerControl HackTarget;
    private PlayerControl MimicTarget;
    private CustomPlayerMenu MimicMenu;
    private bool ClickedAgain;
    public PlayerControl BlockTarget => HackTarget;

    protected override UColor MainColor => CustomColorManager.Glitch;
    public override Layer Type => Layer.Glitch;
    public override string StartText => "foreach var PlayerControl Glitch.MurderPlayer";
    public override string Description => "- You can mimic players' appearances whenever you want to\n- Hacking blocks your target from being able to use their abilities for a " +
        "short while\n- You are immune to blocks\n- If you hack a <#336EFFFF>Serial Killer</color> they will be forced to kill you";
    public override Attack Attack => Attack.Basic;
    public override Defense Defense => HackButton.EffectActive ? Defense.Powerful : Defense.None;
    public override bool RoleBlockImmune => true;
    public override bool CanVent => base.CanVent && GlitchVent;
    protected override Faction ActualFaction => Faction.Glitch;

    public override void Init()
    {
        Objectives = () => "- Neutralise anyone who can oppose you";
        MimicMenu = new(Player, Click, Color, Exception3);
        NeutraliseButton ??= new(this, new SpriteName("Neutralise"), ReworkedAbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Neutralise, (PlayerBodyExclusion)Exception1, "NEUTRALISE",
            new Cooldown(NeutraliseCd));
        HackButton ??= new(this, new SpriteName("Hack"), ReworkedAbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)HitHack, new Cooldown(HackCd), (EndFunc)EndHack, new Duration(HackDur),
            (EffectEndVoid)UnHack, (PlayerBodyExclusion)Exception2, "HACK");
        MimicButton ??= new(this, new SpriteName("Mimic"), ReworkedAbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)HitMimic, new Cooldown(MimicCd), "MIMIC", (EffectEndVoid)UnMimic,
            new Duration(MimicDur), (EffectVoid)Mimic, (EndFunc)EndMimic, (ClickedAgainVoid)OnClickedAgain);
    }

    public override void Reset(bool meeting, bool start) => MimicTarget = HackTarget = null;

    private void OnClickedAgain() => ClickedAgain = true;

    private void UnHack()
    {
        if (HackTarget.AmOwner)
            BlockExposed = false;

        HackTarget = null;

        if (Local)
            Play("UnHack");
    }

    private void Mimic() => Player.SetMimicked(MimicTarget, MimicDur, EndHack);

    private void UnMimic()
    {
        ClickedAgain = false;
        MimicTarget = null;
    }

    private void Click(PlayerControl player) => MimicTarget = player;

    private void HitHack(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            HackTarget = target;
            HackButton.TriggerRpcAndBegin(GlitchActionsRPC.Hack, HackTarget);
            Play("Hack");
        }
        else
            HackButton.StartCooldown(cooldown);
    }

    private void Neutralise(PlayerControl target) => NeutraliseButton.StartCooldown(Interact(Player, target, true));

    private void HitMimic()
    {
        if (MimicTarget)
            MimicButton.TriggerRpcAndBegin(GlitchActionsRPC.Mimic, MimicTarget);
        else
            MimicMenu.Open();
    }

    private bool Exception1(PlayerControl player) => Player.IsBuddyWith(player, Handler.CurrentFaction);

    private bool Exception2(PlayerControl player) => player == HackTarget || Exception1(player);

    private bool Exception3(PlayerControl player) => player == Player;

    public override void UpdateHud(HudManager __instance)
    {
        if (!KeyboardJoystick.player.GetButtonDown("Delete"))
            return;

        if (MimicTarget && !MimicButton.EffectActive)
            MimicTarget = null;

        Message("Removed a target");
    }

    private bool EndHack() => (HackTarget && HackTarget.HasDied()) || Dead || ClickedAgain;

    private bool EndMimic() => Dead;

    public override void ReadRPC(RpcReader reader)
    {
        var glitchAction = reader.Read<GlitchActionsRPC>();

        switch (glitchAction)
        {
            case GlitchActionsRPC.Mimic:
            {
                MimicTarget = reader.ReadPlayer();
                break;
            }
            case GlitchActionsRPC.Hack:
            {
                HackTarget = reader.ReadPlayer();

                if (HackTarget.AmOwner)
                    CustomStatsManager.IncrementStat(CustomStatsManager.StatsRoleblocked);

                break;
            }
            default:
            {
                Failure($"Received unknown RPC - {glitchAction}");
                break;
            }
        }
    }
}