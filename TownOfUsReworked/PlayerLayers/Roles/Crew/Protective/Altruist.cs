namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Altruist : Crew
{
    [NumberOption(MultiMenu.LayerSubOptions, 0, 15, 1, ZeroIsInfinity = true)]
    public static Number MaxRevives { get; set; } = new(5);

    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number ReviveCd { get; set; } = new(25);

    [NumberOption(MultiMenu.LayerSubOptions, 1f, 15f, 1f, Format.Time)]
    public static Number ReviveDur { get; set; } = new(10);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool AltruistTargetBody { get; set; } = false;

    public CustomButton ReviveButton { get; set; }
    public DeadBody RevivingBody { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Altruist : CustomColorManager.Crew;
    public override string Name => "Altruist";
    public override LayerEnum Type => LayerEnum.Altruist;
    public override Func<string> StartText => () => "Sacrifice Yourself To Save Another";
    public override Func<string> Description => () => $"- You can revive a dead body\n- Reviving a body takes {ReviveDur}s\n- If a meeting is called or you are killed during your revive, " +
        "the revive fails";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.CrewProt;
        ReviveButton ??= CreateButton(this, "REVIVE", new SpriteName("Revive"), AbilityTypes.Dead, KeybindType.ActionSecondary, (OnClick)Revive, new Cooldown(ReviveCd), (EffectEndVoid)UponEnd,
            MaxRevives, new Duration(ReviveDur), (EndFunc)EndEffect, new CanClickAgain(false));
    }

    public bool EndEffect() => Dead;

    public void UponEnd()
    {
        if (!(Meeting() || Dead))
            FinishRevive();
    }

    private void FinishRevive()
    {
        var player = PlayerByBody(RevivingBody);

        if (!player.Data.IsDead)
            return;

        var targetRole = player.GetRole();
        var formerKiller = targetRole.KilledBy;
        targetRole.DeathReason = DeathReasonEnum.Revived;
        targetRole.KilledBy = " By " + PlayerName;
        player.Revive();

        if (player.Is(LayerEnum.Lovers) && Lovers.BothLoversDie)
        {
            var lover = player.GetOtherLover();
            var loverRole = lover.GetRole();
            loverRole.DeathReason = DeathReasonEnum.Revived;
            loverRole.KilledBy = " By " + PlayerName;
            lover.Revive();
        }

        if (ReviveButton.Uses == 0 && Local)
            RpcMurderPlayer(Player);

        if (formerKiller.Contains(CustomPlayer.LocalCustom.PlayerName))
        {
            LocalRole.AllArrows.Add(player.PlayerId, new(CustomPlayer.Local, Color));
            Flash(Color);
        }
    }

    public void Revive()
    {
        RevivingBody = ReviveButton.GetTarget<DeadBody>();
        Spread(Player, PlayerByBody(RevivingBody));
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, ReviveButton, RevivingBody);
        ReviveButton.Begin();
        Flash(Color, ReviveDur);

        if (AltruistTargetBody)
            ReviveButton.GetTarget<DeadBody>()?.gameObject.Destroy();
    }

    public override void ReadRPC(MessageReader reader)
    {
        RevivingBody = reader.ReadBody();

        if (CustomPlayer.Local.PlayerId == RevivingBody.ParentId)
            Flash(CustomColorManager.Altruist, ReviveDur);

        if (AltruistTargetBody)
            RevivingBody.gameObject.Destroy();
    }
}