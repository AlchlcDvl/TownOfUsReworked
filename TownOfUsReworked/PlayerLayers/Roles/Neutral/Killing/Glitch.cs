namespace TownOfUsReworked.PlayerLayers.Roles;

public class Glitch : Neutral
{
    public DateTime LastMimic { get; set; }
    public DateTime LastHacked { get; set; }
    public DateTime LastKilled { get; set; }
    public CustomButton HackButton { get; set; }
    public CustomButton MimicButton { get; set; }
    public CustomButton NeutraliseButton { get; set; }
    public PlayerControl HackTarget { get; set; }
    public bool IsUsingMimic => TimeRemaining2 > 0f;
    public float TimeRemaining { get; set; }
    public float TimeRemaining2 { get; set; }
    public bool IsUsingHack => TimeRemaining > 0f;
    public bool MimicEnabled { get; set; }
    public bool HackEnabled { get; set; }
    public PlayerControl MimicTarget { get; set; }
    public CustomMenu MimicMenu { get; set; }

    public override Color Color => ClientGameOptions.CustomNeutColors ? Colors.Glitch : Colors.Neutral;
    public override string Name => "Glitch";
    public override LayerEnum Type => LayerEnum.Glitch;
    public override Func<string> StartText => () => "foreach var PlayerControl Glitch.MurderPlayer";
    public override Func<string> Description => () => "- You can mimic players' appearances whenever you want to\n- Hacking blocks your target from being able to use their abilities " +
        "for a short while\n- You are immune to blocks\n- If you hack a <color=#336EFFFF>Serial Killer</color> they will be forced to kill you";
    public override InspectorResults InspectorResults => InspectorResults.HindersOthers;
    public float HackTimer => ButtonUtils.Timer(Player, LastHacked, CustomGameOptions.HackCd);
    public float MimicTimer => ButtonUtils.Timer(Player, LastMimic, CustomGameOptions.MimicCd);
    public float NeutraliseTimer => ButtonUtils.Timer(Player, LastKilled, CustomGameOptions.NeutraliseCd);

    public Glitch(PlayerControl owner) : base(owner)
    {
        Objectives = () => "- Neutralise anyone who can oppose you";
        Alignment = Alignment.NeutralKill;
        MimicMenu = new(Player, Click, Exception3);
        RoleBlockImmune = true;
        NeutraliseButton = new(this, "Neutralise", AbilityTypes.Direct, "ActionSecondary", Neutralise, Exception1);
        HackButton = new(this, "Hack", AbilityTypes.Direct, "Tertiary", HitHack, Exception2);
        MimicButton = new(this, "Mimic", AbilityTypes.Effect, "Secondary", HitMimic);
    }

    public void UnHack()
    {
        HackEnabled = false;
        GetLayers(HackTarget).ForEach(x => x.IsBlocked = false);
        HackTarget = null;
        LastHacked = DateTime.UtcNow;
    }

    public void Hack()
    {
        HackEnabled = true;
        TimeRemaining -= Time.deltaTime;
        GetLayers(HackTarget).ForEach(x => x.IsBlocked = !GetRole(HackTarget).RoleBlockImmune);

        if (Meeting || IsDead || HackTarget.HasDied())
            TimeRemaining = 0f;
    }

    public void Mimic()
    {
        TimeRemaining2 -= Time.deltaTime;
        Morph(Player, MimicTarget);
        MimicEnabled = true;

        if (IsDead || Meeting)
            TimeRemaining2 = 0f;
    }

    public void UnMimic()
    {
        MimicTarget = null;
        MimicEnabled = false;
        DefaultOutfit(Player);
        LastMimic = DateTime.UtcNow;
    }

    public void Click(PlayerControl player) => MimicTarget = player;

    public void HitHack()
    {
        if (HackTimer != 0f || IsTooFar(Player, HackButton.TargetPlayer) || IsUsingHack)
            return;

        var interact = Interact(Player, HackButton.TargetPlayer);

        if (interact.AbilityUsed)
        {
            HackTarget = HackButton.TargetPlayer;
            TimeRemaining = CustomGameOptions.HackDur;
            CallRpc(CustomRPC.Action, ActionsRPC.GlitchRoleblock, this, HackTarget);
        }
        else if (interact.Reset)
            LastHacked = DateTime.UtcNow;
        else if (interact.Protected)
            LastHacked.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    public void Neutralise()
    {
        if (IsTooFar(Player, NeutraliseButton.TargetPlayer) || NeutraliseTimer != 0f)
            return;

        var interact = Interact(Player, NeutraliseButton.TargetPlayer, true);

        if (interact.AbilityUsed || interact.Reset)
            LastKilled = DateTime.UtcNow;
        else if (interact.Protected)
            LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
        else if (interact.Vested)
            LastKilled.AddSeconds(CustomGameOptions.VestKCReset);
    }

    public void HitMimic()
    {
        if (MimicTimer != 0f || IsUsingMimic)
            return;

        if (MimicTarget == null)
            MimicMenu.Open();
        else
        {
            CallRpc(CustomRPC.Action, ActionsRPC.Mimic, this, MimicTarget);
            TimeRemaining2 = CustomGameOptions.MimicDur;
        }
    }

    public bool Exception1(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate)
        || Player.IsLinkedTo(player);

    public bool Exception2(PlayerControl player) => player == HackTarget;

    public bool Exception3(PlayerControl player) => player == Player;

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        NeutraliseButton.Update("NEUTRALISE", NeutraliseTimer, CustomGameOptions.NeutraliseCd);
        HackButton.Update("HACK", HackTimer, CustomGameOptions.HackCd, IsUsingHack, TimeRemaining, CustomGameOptions.HackDur);
        MimicButton.Update("MIMIC", MimicTimer, CustomGameOptions.MimicCd, IsUsingMimic, TimeRemaining2, CustomGameOptions.MimicDur);

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (MimicTarget != null && !IsUsingMimic)
                MimicTarget = null;

            LogInfo("Removed a target");
        }
    }
}