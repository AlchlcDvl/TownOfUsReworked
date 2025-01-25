namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Altruist : Crew
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
    public static Number AltManaCost = 2;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number ReviveCd = 25;

    [NumberOption(1f, 15f, 1f, Format.Time)]
    public static Number ReviveDur = 10;

    [ToggleOption]
    public static bool AltruistTargetBody = false;

    public CustomButton ReviveButton { get; set; }
    public CustomButton ManaButton { get; set; }
    public byte ParentId { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Altruist : FactionColor;
    public override LayerEnum Type => LayerEnum.Altruist;
    public override Func<string> StartText => () => "Sacrifice Yourself To Save Another";
    public override Func<string> Description => () => $"- You can revive a dead body\n- Reviving a body takes {ReviveDur}s\n- If a meeting is called or you are killed during your revive, " +
        "the revive fails";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Protective;
        ManaButton ??= new(this, "GAIN MANA", new SpriteName("AltManaGain"), AbilityTypes.Body, KeybindType.Tertiary, (OnClickBody)GainMana, new Cooldown(AltManaCd), (UsableFunc)Usable);
        ReviveButton ??= new(this, "REVIVE", new SpriteName("Revive"), AbilityTypes.Body, KeybindType.ActionSecondary, (OnClickBody)Revive, new Cooldown(ReviveCd), (EffectEndVoid)UponEnd,
            MaxAltMana, new Duration(ReviveDur), (EndFunc)EndEffect, new CanClickAgain(false), new UsesDecrement(AltManaCost));
        ReviveButton.uses = 0;
    }

    public bool EndEffect() => Dead;

    public void UponEnd()
    {
        if (!(Meeting() || Dead))
            FinishRevive();
    }

    private void FinishRevive()
    {
        var player = PlayerById(ParentId);

        if (!player.Data.IsDead)
            return;

        var targetRole = player.GetRole();
        var formerKiller = targetRole.KilledBy;
        targetRole.DeathReason = DeathReasonEnum.Revived;
        targetRole.KilledBy = " By " + PlayerName;
        player.Revive();

        if (Lovers.BothLoversDie && player.TryGetLayer<Lovers>(out var lovers))
        {
            var lover = lovers.OtherLover;
            var loverRole = lover.GetRole();
            loverRole.DeathReason = DeathReasonEnum.Revived;
            loverRole.KilledBy = " By " + PlayerName;
            lover.Revive();
        }

        if (Local && player.IIs<ISovereign>())
            CustomAchievementManager.UnlockAchievement("RekindledPower");
    }

    public void Revive(DeadBody target)
    {
        ParentId = target.ParentId;
        Spread(Player, PlayerByBody(target));
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, ReviveButton, ParentId);
        ReviveButton.Begin();
        Flash(Color, ReviveDur);

        if (AltruistTargetBody)
            target.gameObject.Destroy();
    }

    public void GainMana(DeadBody target)
    {
        ReviveButton.Uses += AltManaGainedPerBody;
        Spread(Player, PlayerByBody(target));
        CallRpc(CustomRPC.Action, ActionsRPC.FadeBody, target);
        FadeBody(target);
        ManaButton.StartCooldown();
    }

    public bool Usable() => ReviveButton.uses != ReviveButton.maxUses;

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);
        ReviveButton.Uses += PassiveAltManaGain;
    }

    public override void ReadRPC(MessageReader reader)
    {
        ParentId = reader.ReadByte();

        if (CustomPlayer.Local.PlayerId == ParentId)
            Flash(CustomColorManager.Altruist, ReviveDur);

        if (AltruistTargetBody)
            BodyById(ParentId).gameObject.Destroy();
    }
}