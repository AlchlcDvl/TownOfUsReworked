namespace TownOfUsReworked.PlayerLayers.Roles;

public class Necromancer : Neutral
{
    public DeadBody ResurrectingBody { get; set; }
    public bool Success { get; set; }
    public CustomButton ResurrectButton { get; set; }
    public CustomButton KillButton { get; set; }
    public List<byte> Resurrected { get; set; }
    public int ResurrectUsesLeft { get; set; }
    public bool ResurrectButtonUsable => ResurrectUsesLeft > 0;
    public int KillUsesLeft { get; set; }
    public bool KillButtonUsable => KillUsesLeft > 0;
    public DateTime LastKilled { get; set; }
    public DateTime LastResurrected { get; set; }
    public int ResurrectedCount { get; set; }
    public int KillCount { get; set; }
    public bool Resurrecting { get; set; }
    public float TimeRemaining { get; set; }
    public bool IsResurrecting => TimeRemaining > 0f;

    public override Color32 Color => ClientGameOptions.CustomNeutColors ? Colors.Necromancer : Colors.Neutral;
    public override string Name => "Necromancer";
    public override LayerEnum Type => LayerEnum.Necromancer;
    public override Func<string> StartText => () => "Resurrect The Dead Into Doing Your Bidding";
    public override Func<string> Description => () => "- You can resurrect a dead body and bring them into the <color=#E6108AFF>Reanimated</color>\n- You can kill players to speed " +
        "up the process";
    public override InspectorResults InspectorResults => InspectorResults.PreservesLife;
    public float ResurrectTimer => ButtonUtils.Timer(Player, LastResurrected, CustomGameOptions.ResurrectCooldown, ResurrectedCount * CustomGameOptions.ResurrectCooldownIncrease);
    public float KillTimer => ButtonUtils.Timer(Player, LastKilled, CustomGameOptions.NecroKillCooldown, KillCount * CustomGameOptions.NecroKillCooldownIncrease);

    public Necromancer(PlayerControl player) : base(player)
    {
        Objectives = () => "- Resurrect or kill anyone who can oppose the <color=#E6108AFF>Reanimated</color>";
        RoleAlignment = RoleAlignment.NeutralNeo;
        Objectives = () => "- Resurrect the dead into helping you gain control of the crew";
        SubFaction = SubFaction.Reanimated;
        SubFactionColor = Colors.Reanimated;
        ResurrectUsesLeft = CustomGameOptions.ResurrectCount;
        KillUsesLeft = CustomGameOptions.NecroKillCount;
        ResurrectedCount = 0;
        KillCount = 0;
        Resurrected = new() { Player.PlayerId };
        ResurrectButton = new(this, "Ressurect", AbilityTypes.Dead, "ActionSecondary", HitResurrect, Exception2, true);
        KillButton = new(this, "NecroKill", AbilityTypes.Direct, "Secondary", Kill, Exception1, true);
        SubFactionSymbol = "Σ";
    }

    public void Resurrect()
    {
        if (!Resurrecting && CustomPlayer.Local.PlayerId == ResurrectButton.TargetBody.ParentId)
        {
            Flash(Colors.Reanimated, CustomGameOptions.NecroResurrectDuration);

            if (CustomGameOptions.NecromancerTargetBody)
                ResurrectButton.TargetBody?.gameObject.Destroy();
        }

        Resurrecting = true;
        TimeRemaining -= Time.deltaTime;

        if (Meeting || IsDead)
        {
            Success = false;
            TimeRemaining = 0f;
        }
    }

    public void UnResurrect()
    {
        Resurrecting = false;
        LastResurrected = DateTime.UtcNow;

        if (Success)
            FinishResurrect();
    }

    private void FinishResurrect()
    {
        var player = PlayerByBody(ResurrectingBody);

        if (!player.Data.IsDead)
            return;

        var targetRole = GetRole(player);
        targetRole.DeathReason = DeathReasonEnum.Revived;
        targetRole.KilledBy = " By " + PlayerName;
        RoleGen.Convert(player.PlayerId, Player.PlayerId, SubFaction.Reanimated, false);
        ResurrectedCount++;
        ResurrectUsesLeft--;
        player.Revive();

        if (player.Is(LayerEnum.Lovers) && CustomGameOptions.BothLoversDie)
        {
            var lover = player.GetOtherLover();
            var loverRole = GetRole(lover);
            loverRole.DeathReason = DeathReasonEnum.Revived;
            loverRole.KilledBy = " By " + PlayerName;
            RoleGen.Convert(lover.PlayerId, Player.PlayerId, SubFaction.Reanimated, false);
            lover.Revive();
        }
    }

    public bool Exception1(PlayerControl player) => Resurrected.Contains(player.PlayerId) || Player.IsLinkedTo(player);

    public bool Exception2(PlayerControl player) => Resurrected.Contains(player.PlayerId);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        KillButton.Update("KILL", KillTimer, CustomGameOptions.NecroKillCooldown, KillUsesLeft, CustomGameOptions.NecroKillCooldownIncreases ? KillCount *
            CustomGameOptions.NecroKillCooldownIncrease : 0, true, KillButtonUsable);
        ResurrectButton.Update("RESURRECT", ResurrectTimer, CustomGameOptions.ResurrectCooldown, ResurrectUsesLeft, IsResurrecting, TimeRemaining,
            CustomGameOptions.NecroResurrectDuration, CustomGameOptions.ResurrectCooldownIncreases ? ResurrectedCount * CustomGameOptions.ResurrectCooldownIncrease : 0, true,
            ResurrectButtonUsable);
    }

    public void HitResurrect()
    {
        if (IsTooFar(Player, ResurrectButton.TargetBody) || ResurrectTimer != 0f || !ResurrectButtonUsable)
            return;

        if (RoleGen.Convertible <= 0 || !PlayerByBody(ResurrectButton.TargetBody).Is(SubFaction.None))
        {
            Flash(new(255, 0, 0, 255));
            LastResurrected = DateTime.UtcNow;
        }
        else
        {
            Spread(Player, PlayerByBody(ResurrectingBody));
            CallRpc(CustomRPC.Action, ActionsRPC.NecromancerResurrect, this, ResurrectingBody);
            TimeRemaining = CustomGameOptions.NecroResurrectDuration;
            Success = true;
            Resurrect();

            if (CustomGameOptions.KillResurrectCooldownsLinked)
                LastKilled = DateTime.UtcNow;
        }
    }

    public void Kill()
    {
        if (KillTimer != 0f || IsTooFar(Player, KillButton.TargetPlayer) || !KillButtonUsable)
            return;

        var interact = Interact(Player, KillButton.TargetPlayer, true);

        if (interact[3])
        {
            KillCount++;
            KillUsesLeft--;
        }

        if (interact[0])
        {
            LastKilled = DateTime.UtcNow;

            if (CustomGameOptions.KillResurrectCooldownsLinked)
                LastResurrected = DateTime.UtcNow;
        }
        else if (interact[1])
        {
            LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);

            if (CustomGameOptions.KillResurrectCooldownsLinked)
                LastResurrected.AddSeconds(CustomGameOptions.ProtectKCReset);
        }
        else if (interact[2])
        {
            LastKilled.AddSeconds(CustomGameOptions.VestKCReset);

            if (CustomGameOptions.KillResurrectCooldownsLinked)
                LastResurrected.AddSeconds(CustomGameOptions.VestKCReset);
        }
    }
}