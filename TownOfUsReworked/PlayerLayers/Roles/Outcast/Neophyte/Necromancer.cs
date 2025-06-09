namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Necromancer)]
public sealed class Necromancer : Neophyte, IReviver
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number NecroManaCd = 25;

    [NumberOption(0, 15, 1)]
    private static Number MaxNecroMana = 5;

    [NumberOption(0, 15, 1)]
    private static Number NecroManaGainedPerBody = 1;

    [NumberOption(0, 15, 1)]
    private static Number PassiveNecroManaGain = 0;

    [NumberOption(0, 15, 1)]
    private static Number NecroManaCost = 2;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number ResurrectCd = 25;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number SacrificeCd = 25;

    [ToggleOption]
    private static bool SacrificeCdIncreases = true;

    [NumberOption(2.5f, 30f, 2.5f, Format.Time)]
    private static Number SacrificeCdIncrease = 5;

    [NumberOption(0, 15, 1, zeroIsInf: true)]
    private static Number MaxSacrifices = 5;

    [ToggleOption]
    private static bool NecroCooldownsLinked = false;

    [ToggleOption]
    private static bool NecromancerTargetBody = false;

    [NumberOption(1f, 15f, 1f, Format.Time)]
    private static Number ResurrectDur = 10;

    [ToggleOption]
    private static bool NecroVent = false;

    [ToggleOption]
    public static bool ResurrectVent = false;

    private byte ParentId { get; set; }
    private CustomButton ResurrectButton { get; set; }
    private CustomButton SacrificeButton { get; set; }
    private CustomButton ManaButton { get; set; }

    protected override UColor MainColor => CustomColorManager.Necromancer;
    public override LayerEnum Type => LayerEnum.Necromancer;
    public override Func<string> StartText { get; } = () => "Resurrect The Dead Into Doing Your Bidding";
    public override Func<string> Description => () => "- You can resurrect a dead body and bring them into the <#E6108AFF>Reanimated</color>\n- You can kill players to speed " +
        "up the process";
    public override AttackEnum AttackVal => AttackEnum.Basic;
    public override bool CanVent => base.CanVent && NecroVent;
    protected override Faction ActualFaction => Faction.Reanimated;

    protected override void Init()
    {
        base.Init();
        Objectives = () => "- Resurrect or kill anyone who can oppose the <#E6108AFF>Reanimated</color>";
        ManaButton ??= new(this, "GAIN MANA", new SpriteName("NecroManaGain"), AbilityTypes.Body, KeybindType.Tertiary, (OnClickBody)GainMana, new Cooldown(NecroManaCd), (UsableFunc)Usable);
        ResurrectButton ??= new(this, new SpriteName("Revive"), AbilityTypes.Body, KeybindType.ActionSecondary, (OnClickBody)Resurrect, new Cooldown(ResurrectCd), MaxNecroMana, "RESURRECT",
            new Duration(ResurrectDur), (EffectEndVoid)UponEnd, (PlayerBodyExclusion)Exception, (EndFunc)EndEffect, new CanClickAgain(false), new UsesDecrement(NecroManaCost));
        SacrificeButton ??= new(this, new SpriteName("NecroKill"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Kill, new Cooldown(SacrificeCd), "SACRIFICE", MaxSacrifices,
            (PlayerBodyExclusion)Exception, (DifferenceFunc)Difference2);
    }

    private void UponEnd()
    {
        if (!(Meeting() || Dead))
            FinishResurrect();
    }

    private void FinishResurrect()
    {
        var player = PlayerById(ParentId);

        if (!player.Data.IsDead || !LayerHandler.Handlers.TryGetValue(player.PlayerId, out var targetHandler))
            return;

        targetHandler.DeathReason = DeathReasonEnum.Revived;
        targetHandler.KilledBy = " By " + PlayerName;
        ConvertPlayer(player.PlayerId, Player.PlayerId, false);
        player.Revive();

        if (Lovers.BothLoversDie && player.Is<Lovers>(out var lovers) && LayerHandler.Handlers.TryGetValue(lovers.Other.PlayerId, out var loverHandler))
        {
            loverHandler.DeathReason = DeathReasonEnum.Revived;
            loverHandler.KilledBy = " By " + PlayerName;
            lovers.Other.Revive();

            if (Local && lovers.Other.Is<Sovereign>(out var loverSov) && !loverSov.Revealed)
                CustomAchievementManager.UnlockAchievement("RekindledPower");
        }

        if (Local && player.Is<Sovereign>(out var sov) && !sov.Revealed)
            CustomAchievementManager.UnlockAchievement("RekindledPower");
    }

    private void Resurrect(DeadBody target)
    {
        var player = PlayerByBody(target);

        if (RoleGenManager.Convertible <= 0 || !player.GetFaction().IsConvertible())
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
                target?.gameObject?.Destroy();

            if (NecroCooldownsLinked)
                SacrificeButton.StartCooldown();
        }

        if (Local && player.Is<Sovereign>(out var sov) && !sov.Revealed)
            CustomAchievementManager.UnlockAchievement("RekindledPower");
    }

    private bool Exception(PlayerControl player) => Members.Contains(player.PlayerId) || Player.IsLinkedTo(player);

    private void Kill(PlayerControl target)
    {
        var cooldown = Interact(Player, target, true);

        SacrificeButton.StartCooldown(cooldown);

        if (NecroCooldownsLinked)
            ResurrectButton.StartCooldown(cooldown);
    }

    private void GainMana(DeadBody target)
    {
        ResurrectButton.Uses += NecroManaGainedPerBody;
        Spread(Player, PlayerByBody(target));
        CallRpc(CustomRPC.Action, ActionsRPC.FadeBody, target);
        FadeBody(target);
        ManaButton.StartCooldown();
    }

    private bool Usable() => ResurrectButton.UsesCount != ResurrectButton.Max;

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);
        ResurrectButton.Uses += PassiveNecroManaGain;
    }

    private float Difference2() => SacrificeCdIncreases ? (KillCounts.GetValueOrDefault(PlayerId, 0) * SacrificeCdIncrease) : 0;

    private bool EndEffect() => Dead;

    public override void ReadRPC(RpcReader reader)
    {
        ParentId = reader.ReadByte();

        if (LocalPlayer.PlayerId == ParentId)
            Flash(CustomColorManager.Necromancer, ResurrectDur);

        if (NecromancerTargetBody)
            BodyById(ParentId)?.gameObject?.Destroy();
    }
}