namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Necromancer : Neophyte
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number NecroManaCd = 25;

    [NumberOption(0, 15, 1)]
    public static Number MaxNecroMana = 5;

    [NumberOption(0, 15, 1)]
    public static Number NecroManaGainedPerBody = 1;

    [NumberOption(0, 15, 1)]
    public static Number PassiveNecroManaGain = 0;

    [NumberOption(0, 15, 1)]
    public static Number NecroManaCost = 2;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number ResurrectCd = 25;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number SacrificeCd = 25;

    [ToggleOption]
    public static bool SacrificeCdIncreases = true;

    [NumberOption(2.5f, 30f, 2.5f, Format.Time)]
    public static Number SacrificeCdIncrease = 5;

    [NumberOption(0, 15, 1, zeroIsInf: true)]
    public static Number MaxSacrifices = 5;

    [ToggleOption]
    public static bool NecroCooldownsLinked = false;

    [ToggleOption]
    public static bool NecromancerTargetBody = false;

    [NumberOption(1f, 15f, 1f, Format.Time)]
    public static Number ResurrectDur = 10;

    [ToggleOption]
    public static bool NecroVent = false;

    [ToggleOption]
    public static bool ResurrectVent = false;

    public byte ParentId { get; set; }
    public CustomButton ResurrectButton { get; set; }
    public CustomButton SacrificeButton { get; set; }
    public CustomButton ManaButton { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Necromancer : FactionColor;
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
        ManaButton ??= new(this, "GAIN MANA", new SpriteName("NecroManaGain"), AbilityTypes.Body, KeybindType.Tertiary, (OnClickBody)GainMana, new Cooldown(NecroManaCd), (UsableFunc)Usable);
        ResurrectButton ??= new(this, new SpriteName("Revive"), AbilityTypes.Body, KeybindType.ActionSecondary, (OnClickBody)Resurrect, new Cooldown(ResurrectCd), MaxNecroMana, "RESURRECT",
            new Duration(ResurrectDur), (EffectEndVoid)UponEnd, (PlayerBodyExclusion)Exception, (EndFunc)EndEffect, new CanClickAgain(false));
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
        ConvertPlayer(player.PlayerId, Player.PlayerId, SubFaction.Reanimated, false);
        player.Revive();

        if (Lovers.BothLoversDie && player.TryGetLayer<Lovers>(out var lovers))
        {
            var lover = lovers.OtherLover;
            var loverRole = lover.GetRole();
            loverRole.DeathReason = DeathReasonEnum.Revived;
            loverRole.KilledBy = " By " + PlayerName;
            ConvertPlayer(lover.PlayerId, Player.PlayerId, SubFaction.Reanimated, false);
            lover.Revive();
        }
    }

    public void Resurrect(DeadBody target)
    {
        var player = PlayerByBody(target);

        if (RoleGenManager.Convertible <= 0 || !player.Is(SubFaction.None))
        {
            Flash(UColor.red);
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

        if (Local && player.IIs<ISovereign>())
            CustomAchievementManager.UnlockAchievement("RekindledPower");
    }

    public bool Exception(PlayerControl player) => Members.Contains(player.PlayerId) || Player.IsLinkedTo(player);

    public void Kill(PlayerControl target)
    {
        var cooldown = Interact(Player, target, true);

        SacrificeButton.StartCooldown(cooldown);

        if (NecroCooldownsLinked)
            ResurrectButton.StartCooldown(cooldown);
    }

    public void GainMana(DeadBody target)
    {
        ResurrectButton.Uses += NecroManaGainedPerBody;
        Spread(Player, PlayerByBody(target));
        CallRpc(CustomRPC.Action, ActionsRPC.FadeBody, target);
        FadeBody(target);
        ManaButton.StartCooldown();
    }

    public bool Usable() => ResurrectButton.uses != ResurrectButton.maxUses;

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);
        ResurrectButton.Uses += PassiveNecroManaGain;
    }

    public float Difference2() => SacrificeCdIncreases ? ((KillCounts.TryGetValue(PlayerId, out var count) ? count : 0) * SacrificeCdIncrease) : 0;

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