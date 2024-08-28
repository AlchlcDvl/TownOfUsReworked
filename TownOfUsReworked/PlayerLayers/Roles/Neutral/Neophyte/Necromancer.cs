namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Necromancer : Neutral
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static float ResurrectCd { get; set; } = 25f;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ResurrectCdIncreases { get; set; } = true;

    [NumberOption(MultiMenu.LayerSubOptions, 2.5f, 30f, 2.5f, Format.Time)]
    public static float ResurrectCdIncrease { get; set; } = 5f;

    [NumberOption(MultiMenu.LayerSubOptions, 0, 15, 1, ZeroIsInfinity = true)]
    public static int MaxResurrections { get; set; } = 5;

    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static float NecroKillCd { get; set; } = 25f;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool NecroKillCdIncreases { get; set; } = true;

    [NumberOption(MultiMenu.LayerSubOptions, 2.5f, 30f, 2.5f, Format.Time)]
    public static float NecroKillCdIncrease { get; set; } = 5f;

    [NumberOption(MultiMenu.LayerSubOptions, 0, 15, 1, ZeroIsInfinity = true)]
    public static int MaxNecroKills { get; set; } = 5;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool NecroCooldownsLinked { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool NecromancerTargetBody { get; set; } = false;

    [NumberOption(MultiMenu.LayerSubOptions, 1f, 15f, 1f, Format.Time)]
    public static float ResurrectDur { get; set; } = 10f;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool NecroVent { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ResurrectVent { get; set; } = false;

    public DeadBody ResurrectingBody { get; set; }
    public CustomButton ResurrectButton { get; set; }
    public CustomButton SacrificeButton { get; set; }
    public List<byte> Resurrected { get; set; }
    public int ResurrectedCount { get; set; }
    public int KillCount { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Necromancer : CustomColorManager.Neutral;
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
        Resurrected = [ Player.PlayerId ];
        ResurrectButton = CreateButton(this, new SpriteName("Revive"), AbilityTypes.Dead, KeybindType.ActionSecondary, (OnClick)Resurrect, new Cooldown(ResurrectCd), MaxResurrections,
            new Duration(ResurrectDur), (EffectEndVoid)UponEnd, (PlayerBodyExclusion)Exception, "RESURRECT", (DifferenceFunc)Difference1, (EndFunc)EndEffect, new CanClickAgain(false));
        SacrificeButton = CreateButton(this, new SpriteName("NecroKill"), AbilityTypes.Alive, KeybindType.Secondary, (OnClick)Kill, new Cooldown(NecroKillCd), "SACRIFICE",
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

        if (player.Is(LayerEnum.Lovers) && Lovers.BothLoversDie)
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
            Flash(Color, ResurrectDur);

            if (NecromancerTargetBody)
                ResurrectButton.TargetBody?.gameObject.Destroy();

            if (NecroCooldownsLinked)
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

        if (NecroCooldownsLinked)
            ResurrectButton.StartCooldown(cooldown);
    }

    public float Difference1() => ResurrectCdIncreases ? (ResurrectedCount * ResurrectCdIncrease) : 0;

    public float Difference2() => NecroKillCdIncreases ? (KillCount * NecroKillCdIncrease) : 0;

    public bool EndEffect() => Dead;

    public override void ReadRPC(MessageReader reader)
    {
        ResurrectingBody = reader.ReadBody();

        if (CustomPlayer.Local.PlayerId == ResurrectingBody.ParentId)
            Flash(CustomColorManager.Necromancer, ResurrectDur);

        if (NecromancerTargetBody)
            ResurrectingBody.gameObject.Destroy();
    }
}