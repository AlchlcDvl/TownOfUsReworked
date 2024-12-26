namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Necromancer : Neophyte
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number ResurrectCd { get; set; } = new(25);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ResurrectCdIncreases { get; set; } = true;

    [NumberOption(MultiMenu.LayerSubOptions, 2.5f, 30f, 2.5f, Format.Time)]
    public static Number ResurrectCdIncrease { get; set; } = new(5);

    [NumberOption(MultiMenu.LayerSubOptions, 0, 15, 1, ZeroIsInfinity = true)]
    public static Number MaxResurrections { get; set; } = new(5);

    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number SacrificeCd { get; set; } = new(25);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool SacrificeCdIncreases { get; set; } = true;

    [NumberOption(MultiMenu.LayerSubOptions, 2.5f, 30f, 2.5f, Format.Time)]
    public static Number SacrificeCdIncrease { get; set; } = new(5);

    [NumberOption(MultiMenu.LayerSubOptions, 0, 15, 1, ZeroIsInfinity = true)]
    public static Number MaxSacrifices { get; set; } = new(5);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool NecroCooldownsLinked { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool NecromancerTargetBody { get; set; } = false;

    [NumberOption(MultiMenu.LayerSubOptions, 1f, 15f, 1f, Format.Time)]
    public static Number ResurrectDur { get; set; } = new(10);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool NecroVent { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ResurrectVent { get; set; } = false;

    public byte ParentId { get; set; }
    public CustomButton ResurrectButton { get; set; }
    public CustomButton SacrificeButton { get; set; }
    public int ResurrectedCount { get; set; }
    public int KillCount { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Necromancer : FactionColor;
    public override string Name => "Necromancer";
    public override LayerEnum Type => LayerEnum.Necromancer;
    public override Func<string> StartText => () => "Resurrect The Dead Into Doing Your Bidding";
    public override Func<string> Description => () => "- You can resurrect a dead body and bring them into the <#E6108AFF>Reanimated</color>\n- You can kill players to speed " +
        "up the process";
    public override AttackEnum AttackVal => AttackEnum.Basic;

    public override void Init()
    {
        base.Init();
        Objectives = () => "- Resurrect or kill anyone who can oppose the <#E6108AFF>Reanimated</color>";
        SubFaction = SubFaction.Reanimated;
        ResurrectedCount = 0;
        KillCount = 0;
        ResurrectButton ??= new(this, new SpriteName("Revive"), AbilityTypes.Body, KeybindType.ActionSecondary, (OnClickBody)Resurrect, new Cooldown(ResurrectCd), MaxResurrections,
            new Duration(ResurrectDur), (EffectEndVoid)UponEnd, (PlayerBodyExclusion)Exception, "RESURRECT", (DifferenceFunc)Difference1, (EndFunc)EndEffect, new CanClickAgain(false));
        SacrificeButton ??= new(this, new SpriteName("NecroKill"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Kill, new Cooldown(SacrificeCd), "SACRIFICE",
            (PlayerBodyExclusion)Exception, (DifferenceFunc)Difference2);
    }

    public void UponEnd()
    {
        if (!(Meeting() || Dead))
            FinishResurrect();
    }

    private void FinishResurrect()
    {
        var player = PlayerById(ParentId);

        if (!player.Data.IsDead)
            return;

        var targetRole = player.GetRole();
        targetRole.DeathReason = DeathReasonEnum.Revived;
        targetRole.KilledBy = " By " + PlayerName;
        Convert(player.PlayerId, Player.PlayerId, SubFaction.Reanimated, false);
        ResurrectedCount++;
        player.Revive();

        if (Lovers.BothLoversDie && player.TryGetLayer<Lovers>(out var lovers))
        {
            var lover = lovers.OtherLover;
            var loverRole = lover.GetRole();
            loverRole.DeathReason = DeathReasonEnum.Revived;
            loverRole.KilledBy = " By " + PlayerName;
            Convert(lover.PlayerId, Player.PlayerId, SubFaction.Reanimated, false);
            lover.Revive();
        }
    }

    public void Resurrect(DeadBody target)
    {
        var player = PlayerByBody(target);

        if (RoleGenManager.Convertible <= 0 || !player.Is(SubFaction.None))
        {
            Flash(new(255, 0, 0, 255));
            ResurrectButton.StartCooldown();
        }
        else
        {
            ParentId = target.ParentId;
            Spread(Player, player);
            CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, ResurrectButton, ParentId);
            ResurrectButton.Begin();
            Flash(Color, ResurrectDur);

            if (NecromancerTargetBody)
                target?.gameObject.Destroy();

            if (NecroCooldownsLinked)
                SacrificeButton.StartCooldown();
        }
    }

    public bool Exception(PlayerControl player) => Members.Contains(player.PlayerId) || Player.IsLinkedTo(player);

    public void Kill(PlayerControl target)
    {
        var cooldown = Interact(Player, target, true);

        if (cooldown != CooldownType.Fail)
            KillCount++;

        SacrificeButton.StartCooldown(cooldown);

        if (NecroCooldownsLinked)
            ResurrectButton.StartCooldown(cooldown);
    }

    public float Difference1() => ResurrectCdIncreases ? (ResurrectedCount * ResurrectCdIncrease) : 0;

    public float Difference2() => SacrificeCdIncreases ? (KillCount * SacrificeCdIncrease) : 0;

    public bool EndEffect() => Dead;

    public override void ReadRPC(MessageReader reader)
    {
        ParentId = reader.ReadByte();

        if (CustomPlayer.Local.PlayerId == ParentId)
            Flash(CustomColorManager.Necromancer, ResurrectDur);

        if (NecromancerTargetBody)
            BodyById(ParentId).gameObject.Destroy();
    }
}