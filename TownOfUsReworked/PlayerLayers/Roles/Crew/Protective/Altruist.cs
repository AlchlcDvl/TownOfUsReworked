namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Altruist)]
public sealed class Altruist : Protective, IReviver
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number AltManaCd = 25;

    [NumberOption(0, 15, 1)]
    public static Number MaxAltMana = 5;

    [NumberOption(0, 15, 1)]
    public static Number AltManaGainedPerBody = 1;

    [NumberOption(0, 15, 1)]
    public static Number PassiveAltManaGain = 0;

    [NumberOption(0, 15, 1)]
    private static Number AltManaCost = 2;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number ReviveCd = 25;

    [NumberOption(1f, 15f, 1f, Format.Time)]
    public static Number ReviveDur = 10;

    [ToggleOption]
    public static bool AltruistTargetBody = false;

    private CustomButton ReviveButton;
    private CustomButton ManaButton;
    private byte ParentId;

    protected override UColor MainColor => CustomColorManager.Altruist;
    public override Layer Type => Layer.Altruist;
    public override string StartText => "Sacrifice Yourself To Save Another";
    public override string Description => $"- You can revive a dead body\n- Reviving a body takes {ReviveDur}s\n- If a meeting is called or you are killed during your revive, " +
        "the revive fails";

    public override void Init()
    {
        ManaButton ??= new(this, "GAIN MANA", new SpriteName("AltManaGain"), AbilityTypes.Body, KeybindType.Tertiary, (OnClickBody)GainMana, new Cooldown(AltManaCd), (UsableFunc)Usable);
        ReviveButton ??= new(this, "REVIVE", new SpriteName("Revive"), AbilityTypes.Body, KeybindType.ActionSecondary, (OnClickBody)Revive, new Cooldown(ReviveCd), (EffectEndVoid)UponEnd,
            MaxAltMana, new Duration(ReviveDur), (EndFunc)EndEffect, new CanClickAgain(false), new UsesDecrement(AltManaCost));
        ReviveButton.UsesCount = 0;
    }

    private bool EndEffect() => Dead;

    private void UponEnd()
    {
        if (!(Meeting() || Dead))
            FinishRevive();
    }

    private void FinishRevive()
    {
        var player = PlayerById(ParentId);

        if (!player.Data.IsDead || !LayerHandler.Handlers.TryGetValue(player.PlayerId, out var targetHandler))
            return;

        targetHandler.DeathReason = DeathReasonEnum.Revived;
        targetHandler.KilledBy = " By " + PlayerName;
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

    private void Revive(DeadBody target)
    {
        ParentId = target.ParentId;
        Spread(Player, PlayerByBody(target));
        ReviveButton.TriggerRpcAndBegin(ParentId);
        Flash(Color, ReviveDur);

        if (AltruistTargetBody)
            target.gameObject.Destroy();
    }

    private void GainMana(DeadBody target)
    {
        ReviveButton.Uses += AltManaGainedPerBody;
        Spread(Player, PlayerByBody(target));
        CallRpc(ActionsRpc.FadeBody, target);
        FadeBody(target);
        ManaButton.StartCooldown();
    }

    private bool Usable() => ReviveButton.UsesCount != ReviveButton.Max;

    public override void LocalOnMeetingStart(MeetingHud __instance) => ReviveButton.Uses += PassiveAltManaGain;

    public override void ReadRPC(RpcReader reader)
    {
        ParentId = reader.ReadByte();

        if (LocalPlayer.PlayerId == ParentId)
            Flash(CustomColorManager.Altruist, ReviveDur);

        if (AltruistTargetBody)
            BodyById(ParentId).gameObject.Destroy();
    }
}