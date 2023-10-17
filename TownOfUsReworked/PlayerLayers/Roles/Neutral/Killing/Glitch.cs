﻿namespace TownOfUsReworked.PlayerLayers.Roles;

public class Glitch : Neutral
{
    public CustomButton HackButton { get; set; }
    public CustomButton MimicButton { get; set; }
    public CustomButton NeutraliseButton { get; set; }
    public PlayerControl HackTarget { get; set; }
    public PlayerControl MimicTarget { get; set; }
    public CustomMenu MimicMenu { get; set; }

    public override Color Color => ClientGameOptions.CustomNeutColors ? Colors.Glitch : Colors.Neutral;
    public override string Name => "Glitch";
    public override LayerEnum Type => LayerEnum.Glitch;
    public override Func<string> StartText => () => "foreach var PlayerControl Glitch.MurderPlayer";
    public override Func<string> Description => () => "- You can mimic players' appearances whenever you want to\n- Hacking blocks your target from being able to use their abilities for a " +
        "short while\n- You are immune to blocks\n- If you hack a <color=#336EFFFF>Serial Killer</color> they will be forced to kill you";

    public Glitch(PlayerControl player) : base(player)
    {
        Objectives = () => "- Neutralise anyone who can oppose you";
        Alignment = Alignment.NeutralKill;
        MimicMenu = new(Player, Click, Exception3);
        RoleBlockImmune = true;
        NeutraliseButton = new(this, "Neutralise", AbilityTypes.Target, "ActionSecondary", Neutralise, CustomGameOptions.NeutraliseCd, Exception1);
        HackButton = new(this, "Hack", AbilityTypes.Target, "ActionSecondary", HitHack, CustomGameOptions.HackCd, CustomGameOptions.HackDur, Hack, UnHack, Exception2);
        MimicButton = new(this, "Mimic", AbilityTypes.Targetless, "Secondary", HitMimic, CustomGameOptions.MimicCd, CustomGameOptions.MimicDur, (CustomButton.EffectVoid)Mimic, UnMimic);
        player.Data.Role.IntroSound = GetAudio("GlitchIntro");
    }

    public void UnHack()
    {
        GetLayers(HackTarget).ForEach(x => x.IsBlocked = false);
        HackTarget = null;
    }

    public void Hack() => GetLayers(HackTarget).ForEach(x => x.IsBlocked = !GetRole(HackTarget).RoleBlockImmune);

    public void Mimic() => Morph(Player, MimicTarget);

    public void UnMimic()
    {
        MimicTarget = null;
        DefaultOutfit(Player);
    }

    public void Click(PlayerControl player) => MimicTarget = player;

    public void HitHack()
    {
        var interact = Interact(Player, HackButton.TargetPlayer);

        if (interact.AbilityUsed)
        {
            HackTarget = HackButton.TargetPlayer;
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, GlitchActionsRPC.Hack, HackTarget);
            HackButton.Begin();
        }
        else if (interact.Reset)
            HackButton.StartCooldown(CooldownType.Reset);
        else if (interact.Protected)
            HackButton.StartCooldown(CooldownType.GuardianAngel);
    }

    public void Neutralise()
    {
        var interact = Interact(Player, NeutraliseButton.TargetPlayer, true);
        var cooldown = CooldownType.Reset;

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;
        else if (interact.Vested)
            cooldown = CooldownType.Survivor;

        NeutraliseButton.StartCooldown(cooldown);
    }

    public void HitMimic()
    {
        if (MimicTarget == null)
            MimicMenu.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, GlitchActionsRPC.Mimic, MimicTarget);
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
        NeutraliseButton.Update2("NEUTRALISE");
        HackButton.Update2("HACK");
        MimicButton.Update2("MIMIC");

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (MimicTarget != null && !MimicButton.EffectActive)
                MimicTarget = null;

            LogInfo("Removed a target");
        }
    }

    public override void TryEndEffect()
    {
        HackButton.Update3((HackTarget != null && HackTarget.HasDied()) || IsDead);
        MimicButton.Update3(IsDead);
    }

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
                LogError($"Received unknown RPC - {glitchAction}");
                break;
        }
    }
}