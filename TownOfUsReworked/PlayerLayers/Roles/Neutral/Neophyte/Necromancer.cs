namespace TownOfUsReworked.PlayerLayers.Roles;

public class Necromancer : Neutral
{
    public DeadBody ResurrectingBody { get; set; }
    public CustomButton ResurrectButton { get; set; }
    public CustomButton SacrificeButton { get; set; }
    public List<byte> Resurrected { get; set; }
    public int ResurrectedCount { get; set; }
    public int KillCount { get; set; }

    public override UColor Color => ClientGameOptions.CustomNeutColors ? CustomColorManager.Necromancer : CustomColorManager.Neutral;
    public override string Name => "Necromancer";
    public override LayerEnum Type => LayerEnum.Necromancer;
    public override Func<string> StartText => () => "Resurrect The Dead Into Doing Your Bidding";
    public override Func<string> Description => () => "- You can resurrect a dead body and bring them into the <color=#E6108AFF>Reanimated</color>\n- You can kill players to speed " +
        "up the process";
    public override AttackEnum AttackVal => AttackEnum.Basic;

    public Necromancer() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        BaseStart();
        Objectives = () => "- Resurrect or kill anyone who can oppose the <color=#E6108AFF>Reanimated</color>";
        Alignment = Alignment.NeutralNeo;
        SubFaction = SubFaction.Reanimated;
        SubFactionColor = CustomColorManager.Reanimated;
        ResurrectedCount = 0;
        KillCount = 0;
        Resurrected = new() { Player.PlayerId };
        ResurrectButton = new(this, "Revive", AbilityTypes.Dead, "ActionSecondary", Resurrect, CustomGameOptions.ResurrectCd, CustomGameOptions.ResurrectDur, UponEnd,
            CustomGameOptions.MaxResurrections, Exception);
        SacrificeButton = new(this, "NecroKill", AbilityTypes.Alive, "Secondary", Kill, CustomGameOptions.NecroKillCd, Exception);
        return this;
    }

    public void UponEnd()
    {
        if (!(Meeting || IsDead))
            FinishResurrect();
    }

    public bool End() => IsDead;

    private void FinishResurrect()
    {
        var player = PlayerByBody(ResurrectingBody);

        if (!player.Data.IsDead)
            return;

        var targetRole = player.GetRole();
        targetRole.DeathReason = DeathReasonEnum.Revived;
        targetRole.KilledBy = " By " + PlayerName;
        RoleGen.Convert(player.PlayerId, Player.PlayerId, SubFaction.Reanimated, false);
        ResurrectedCount++;
        player.Revive();

        if (player.Is(LayerEnum.Lovers) && CustomGameOptions.BothLoversDie)
        {
            var lover = player.GetOtherLover();
            var loverRole = lover.GetRole();
            loverRole.DeathReason = DeathReasonEnum.Revived;
            loverRole.KilledBy = " By " + PlayerName;
            RoleGen.Convert(lover.PlayerId, Player.PlayerId, SubFaction.Reanimated, false);
            lover.Revive();
        }
    }

    public void Resurrect()
    {
        if (RoleGen.Convertible <= 0 || !PlayerByBody(ResurrectButton.TargetBody).Is(SubFaction.None))
        {
            Flash(new(255, 0, 0, 255));
            ResurrectButton.StartCooldown();
        }
        else
        {
            ResurrectingBody = ResurrectButton.TargetBody;
            Spread(Player, PlayerByBody(ResurrectingBody));
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, ResurrectButton, ResurrectingBody);
            ResurrectButton.Begin();
            Flash(Color, CustomGameOptions.ResurrectDur);

            if (CustomGameOptions.NecromancerTargetBody)
                ResurrectButton.TargetBody?.gameObject.Destroy();

            if (CustomGameOptions.NecroCooldownsLinked)
                SacrificeButton.StartCooldown();
        }
    }

    public bool Exception(PlayerControl player) => Resurrected.Contains(player.PlayerId) || Player.IsLinkedTo(player);

    public void Kill()
    {
        var cooldown = Interact(Player, SacrificeButton.TargetPlayer, true);

        if (cooldown != CooldownType.Fail)
            KillCount++;

        SacrificeButton.StartCooldown(cooldown);

        if (CustomGameOptions.NecroCooldownsLinked)
            ResurrectButton.StartCooldown(cooldown);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        ResurrectButton.Update2("RESURRECT", difference: CustomGameOptions.ResurrectCdIncreases ? (ResurrectedCount * CustomGameOptions.ResurrectCdIncrease) : 0);
        SacrificeButton.Update2("SACRIFICE", difference: CustomGameOptions.NecroKillCdIncreases ? (KillCount * CustomGameOptions.NecroKillCdIncrease) : 0);
    }

    public override void TryEndEffect() => ResurrectButton.Update3(IsDead);

    public override void ReadRPC(MessageReader reader)
    {
        ResurrectingBody = reader.ReadBody();

        if (CustomPlayer.Local.PlayerId == ResurrectingBody.ParentId)
            Flash(CustomColorManager.Necromancer, CustomGameOptions.ResurrectDur);

        if (CustomGameOptions.AltruistTargetBody)
            ResurrectingBody.gameObject.Destroy();
    }
}