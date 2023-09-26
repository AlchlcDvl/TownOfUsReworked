namespace TownOfUsReworked.PlayerLayers.Roles;

public class Altruist : Crew
{
    public CustomButton ReviveButton { get; set; }
    public DeadBody RevivingBody { get; set; }

    public override Color Color => ClientGameOptions.CustomCrewColors ? Colors.Altruist : Colors.Crew;
    public override string Name => "Altruist";
    public override LayerEnum Type => LayerEnum.Altruist;
    public override Func<string> StartText => () => "Sacrifice Yourself To Save Another";
    public override Func<string> Description => () => $"- You can revive a dead body\n- Reviving a body takes {CustomGameOptions.ReviveDur}s\n- If a meeting is called or you are killed " +
        "during your revive, the revive fails";
    public override InspectorResults InspectorResults => InspectorResults.PreservesLife;

    public Altruist(PlayerControl player) : base(player)
    {
        Alignment = Alignment.CrewProt;
        ReviveButton = new(this, "Revive", AbilityTypes.Dead, "ActionSecondary", Revive, CustomGameOptions.ReviveCd, CustomGameOptions.ReviveDur, UponEnd, CustomGameOptions.MaxRevives);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        ReviveButton.Update2("REVIVE");
    }

    public override void TryEndEffect() => ReviveButton.Update3(IsDead);

    public void UponEnd()
    {
        if (!(Meeting || IsDead))
            FinishRevive();
    }

    private void FinishRevive()
    {
        var player = PlayerByBody(RevivingBody);

        if (!player.Data.IsDead)
            return;

        var targetRole = GetRole(player);
        var formerKiller = targetRole.KilledBy;
        targetRole.DeathReason = DeathReasonEnum.Revived;
        targetRole.KilledBy = " By " + PlayerName;
        player.Revive();

        if (player.Is(LayerEnum.Lovers) && CustomGameOptions.BothLoversDie)
        {
            var lover = player.GetOtherLover();
            var loverRole = GetRole(lover);
            loverRole.DeathReason = DeathReasonEnum.Revived;
            loverRole.KilledBy = " By " + PlayerName;
            lover.Revive();
        }

        if (ReviveButton.Uses == 0 && Local)
            RpcMurderPlayer(Player, Player);

        if (formerKiller.Contains(CustomPlayer.LocalCustom.Data.PlayerName))
        {
            LocalRole.AllArrows.Add(player.PlayerId, new(CustomPlayer.Local, Color));
            Flash(Color);
        }
    }

    public void Revive()
    {
        RevivingBody = ReviveButton.TargetBody;
        Spread(Player, PlayerByBody(RevivingBody));
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction1, ReviveButton, RevivingBody);
        ReviveButton.Begin();
        Flash(Color, CustomGameOptions.ReviveDur);

        if (CustomGameOptions.AltruistTargetBody)
            ReviveButton.TargetBody?.gameObject.Destroy();
    }

    public override void ReadRPC(MessageReader reader)
    {
        RevivingBody = reader.ReadBody();

        if (CustomPlayer.Local.PlayerId == RevivingBody.ParentId)
            Flash(Colors.Altruist, CustomGameOptions.ReviveDur);

        if (CustomGameOptions.AltruistTargetBody)
            RevivingBody.gameObject.Destroy();
    }
}