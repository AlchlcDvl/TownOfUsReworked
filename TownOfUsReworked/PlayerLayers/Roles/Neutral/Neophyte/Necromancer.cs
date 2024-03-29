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

    public override void Init()
    {
        BaseStart();
        Objectives = () => "- Resurrect or kill anyone who can oppose the <color=#E6108AFF>Reanimated</color>";
        Alignment = Alignment.NeutralNeo;
        SubFaction = SubFaction.Reanimated;
        SubFactionColor = CustomColorManager.Reanimated;
        ResurrectedCount = 0;
        KillCount = 0;
        Resurrected = [Player.PlayerId];
        ResurrectButton = CreateButton(this, new SpriteName("Revive"), AbilityTypes.Dead, KeybindType.ActionSecondary, (OnClick)Resurrect, new Cooldown(CustomGameOptions.ResurrectCd),
            new Duration(CustomGameOptions.ResurrectDur), (EffectEndVoid)UponEnd, CustomGameOptions.MaxResurrections, (PlayerBodyExclusion)Exception, "RESURRECT",
            (DifferenceFunc)Difference1, (EndFunc)EndEffect);
        SacrificeButton = CreateButton(this, new SpriteName("NecroKill"), AbilityTypes.Alive, KeybindType.Secondary, (OnClick)Kill, CustomGameOptions.NecroKillCd, "SACRIFICE",
            (PlayerBodyExclusion)Exception, (DifferenceFunc)Difference2);
    }

    public void UponEnd()
    {
        if (!(Meeting || Dead))
            FinishResurrect();
    }

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
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, ResurrectButton, ResurrectingBody);
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

    public float Difference1() => CustomGameOptions.ResurrectCdIncreases ? (ResurrectedCount * CustomGameOptions.ResurrectCdIncrease) : 0;

    public float Difference2() => CustomGameOptions.NecroKillCdIncreases ? (KillCount * CustomGameOptions.NecroKillCdIncrease) : 0;

    public bool EndEffect() => Dead;

    public override void ReadRPC(MessageReader reader)
    {
        ResurrectingBody = reader.ReadBody();

        if (CustomPlayer.Local.PlayerId == ResurrectingBody.ParentId)
            Flash(CustomColorManager.Necromancer, CustomGameOptions.ResurrectDur);

        if (CustomGameOptions.NecromancerTargetBody)
            ResurrectingBody.gameObject.Destroy();
    }
}