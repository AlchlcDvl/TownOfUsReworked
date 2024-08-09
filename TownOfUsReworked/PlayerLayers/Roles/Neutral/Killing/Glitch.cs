namespace TownOfUsReworked.PlayerLayers.Roles;

public class Glitch : Neutral
{
    public CustomButton HackButton { get; set; }
    public CustomButton MimicButton { get; set; }
    public CustomButton NeutraliseButton { get; set; }
    public PlayerControl HackTarget { get; set; }
    public PlayerControl MimicTarget { get; set; }
    public CustomMenu MimicMenu { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Glitch : CustomColorManager.Neutral;
    public override string Name => "Glitch";
    public override LayerEnum Type => LayerEnum.Glitch;
    public override Func<string> StartText => () => "foreach var PlayerControl Glitch.MurderPlayer";
    public override Func<string> Description => () => "- You can mimic players' appearances whenever you want to\n- Hacking blocks your target from being able to use their abilities for a " +
        "short while\n- You are immune to blocks\n- If you hack a <color=#336EFFFF>Serial Killer</color> they will be forced to kill you";
    public override AttackEnum AttackVal => AttackEnum.Basic;
    public override DefenseEnum DefenseVal => HackButton.EffectActive ? DefenseEnum.Powerful : DefenseEnum.None;

    public override void Init()
    {
        BaseStart();
        Objectives = () => "- Neutralise anyone who can oppose you";
        Alignment = Alignment.NeutralKill;
        MimicMenu = new(Player, Click, Exception3);
        RoleBlockImmune = true;
        NeutraliseButton = CreateButton(this, new SpriteName("Neutralise"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Neutralise, (PlayerBodyExclusion)Exception1,
            new Cooldown(CustomGameOptions.NeutraliseCd), "NEUTRALISE");
        HackButton = CreateButton(this, new SpriteName("Hack"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)HitHack, new Cooldown(CustomGameOptions.HackCd), (EffectVoid)Hack,
            new Duration(CustomGameOptions.HackDur), (EffectEndVoid)UnHack, (PlayerBodyExclusion)Exception2, "HACK", (EndFunc)EndHack);
        MimicButton = CreateButton(this, new SpriteName("Mimic"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)HitMimic, new Cooldown(CustomGameOptions.MimicCd), "MIMIC",
            new Duration(CustomGameOptions.MimicDur), (EffectVoid)Mimic, (EffectEndVoid)UnMimic, (EndFunc)EndMimic);
        Data.Role.IntroSound = GetAudio("GlitchIntro");
    }

    public void UnHack()
    {
        HackTarget.GetLayers().ForEach(x => x.IsBlocked = false);
        HackTarget = null;
    }

    public void Hack() => HackTarget.GetLayers().ForEach(x => x.IsBlocked = !HackTarget.GetRole().RoleBlockImmune);

    public void Mimic() => Morph(Player, MimicTarget);

    public void UnMimic()
    {
        MimicTarget = null;
        DefaultOutfit(Player);
    }

    public void Click(PlayerControl player) => MimicTarget = player;

    public void HitHack()
    {
        var cooldown = Interact(Player, HackButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
        {
            HackTarget = HackButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, HackButton, GlitchActionsRPC.Hack, HackTarget);
            HackButton.Begin();
        }
        else
            HackButton.StartCooldown(cooldown);
    }

    public void Neutralise() => NeutraliseButton.StartCooldown(Interact(Player, NeutraliseButton.TargetPlayer, true));

    public void HitMimic()
    {
        if (!MimicTarget)
            MimicMenu.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, MimicButton, GlitchActionsRPC.Mimic, MimicTarget);
            MimicButton.Begin();
        }
    }

    public bool Exception1(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        Player.IsLinkedTo(player);

    public bool Exception2(PlayerControl player) => player == HackTarget || Exception1(player);

    public bool Exception3(PlayerControl player) => player == Player;

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (MimicTarget && !MimicButton.EffectActive)
                MimicTarget = null;

            LogMessage("Removed a target");
        }
    }

    public bool EndHack() => (HackTarget && HackTarget.HasDied()) || Dead;

    public bool EndMimic() => Dead;

    public override void ReadRPC(MessageReader reader)
    {
        var glitchAction = (GlitchActionsRPC)reader.ReadByte();

        switch (glitchAction)
        {
            case GlitchActionsRPC.Mimic:
                MimicTarget = reader.ReadPlayer();
                break;

            case GlitchActionsRPC.Hack:
                HackTarget = reader.ReadPlayer();
                break;

            default:
                LogError($"Received unknown RPC - {(int)glitchAction}");
                break;
        }
    }
}