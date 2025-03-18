namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Glitch)]
public sealed class Glitch : NKilling, IBlocker
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

    private CustomButton HackButton { get; set; }
    private CustomButton MimicButton { get; set; }
    private CustomButton NeutraliseButton { get; set; }
    public PlayerControl HackTarget { get; private set; }
    private PlayerControl MimicTarget { get; set; }
    private CustomPlayerMenu MimicMenu { get; set; }
    public PlayerControl BlockTarget => HackTarget;

    public override UColor MainColor => CustomColorManager.Glitch;
    public override LayerEnum Type => LayerEnum.Glitch;
    public override Func<string> StartText => () => "foreach var PlayerControl Glitch.MurderPlayer";
    public override Func<string> Description => () => "- You can mimic players' appearances whenever you want to\n- Hacking blocks your target from being able to use their abilities for a " +
        "short while\n- You are immune to blocks\n- If you hack a <#336EFFFF>Serial Killer</color> they will be forced to kill you";
    public override AttackEnum AttackVal => AttackEnum.Basic;
    public override DefenseEnum DefenseVal => HackButton.EffectActive ? DefenseEnum.Powerful : DefenseEnum.None;
    public override bool RoleBlockImmune => true;
    public override bool CanVent => base.CanVent && GlitchVent;

    protected override void Init()
    {
        base.Init();
        Objectives = () => "- Neutralise anyone who can oppose you";
        MimicMenu = new(Player, Click, Exception3);
        NeutraliseButton ??= new(this, new SpriteName("Neutralise"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Neutralise, (PlayerBodyExclusion)Exception1, "NEUTRALISE",
            new Cooldown(NeutraliseCd));
        HackButton ??= new(this, new SpriteName("Hack"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)HitHack, new Cooldown(HackCd), (EndFunc)EndHack, new Duration(HackDur),
            (EffectEndVoid)UnHack, (PlayerBodyExclusion)Exception2, "HACK");
        MimicButton ??= new(this, new SpriteName("Mimic"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)HitMimic, new Cooldown(MimicCd), "MIMIC", (EffectEndVoid)UnMimic,
            new Duration(MimicDur), (EffectVoid)Mimic, (EndFunc)EndMimic);
    }

    public override void Reset(bool meeting, bool start) => MimicTarget = HackTarget = null;

    private void UnHack()
    {
        if (HackTarget.AmOwner)
            BlockExposed = false;

        HackTarget = null;

        if (Local)
            Play("UnHack");
    }

    private void Mimic() => Morph(Player, MimicTarget);

    private void UnMimic()
    {
        MimicTarget = null;
        DefaultOutfit(Player);
    }

    private void Click(PlayerControl player) => MimicTarget = player;

    private void HitHack(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            HackTarget = target;
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, HackButton, GlitchActionsRPC.Hack, HackTarget);
            HackButton.Begin();
            Play("Hack");
        }
        else
            HackButton.StartCooldown(cooldown);
    }

    private void Neutralise(PlayerControl target) => NeutraliseButton.StartCooldown(Interact(Player, target, true));

    private void HitMimic()
    {
        if (MimicTarget)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, MimicButton, GlitchActionsRPC.Mimic, MimicTarget);
            MimicButton.Begin();
        }
        else
            MimicMenu.Open();
    }

    private bool Exception1(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        Player.IsLinkedTo(player);

    private bool Exception2(PlayerControl player) => player == HackTarget || Exception1(player);

    private bool Exception3(PlayerControl player) => player == Player;

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (!KeyboardJoystick.player.GetButtonDown("Delete"))
            return;

        if (MimicTarget && !MimicButton.EffectActive)
            MimicTarget = null;

        Message("Removed a target");
    }

    private bool EndHack() => (HackTarget && HackTarget.HasDied()) || Dead;

    private bool EndMimic() => Dead;

    public override void ReadRPC(MessageReader reader)
    {
        var glitchAction = reader.ReadEnum<GlitchActionsRPC>();

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