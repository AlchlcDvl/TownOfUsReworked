namespace TownOfUsReworked.PlayerLayers.Roles;

public class Altruist : Crew
{
    public CustomButton ReviveButton { get; set; }
    public DeadBody RevivingBody { get; set; }

    public override UColor Color => ClientGameOptions.CustomCrewColors ? CustomColorManager.Altruist : CustomColorManager.Crew;
    public override string Name => "Altruist";
    public override LayerEnum Type => LayerEnum.Altruist;
    public override Func<string> StartText => () => "Sacrifice Yourself To Save Another";
    public override Func<string> Description => () => $"- You can revive a dead body\n- Reviving a body takes {CustomGameOptions.ReviveDur}s\n- If a meeting is called or you are killed " +
        "during your revive, the revive fails";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.CrewProt;
        ReviveButton = CreateButton(this, "REVIVE", new SpriteName("Revive"), AbilityTypes.Dead, KeybindType.ActionSecondary, (OnClick)Revive, new Cooldown(CustomGameOptions.ReviveCd),
            new Duration(CustomGameOptions.ReviveDur), (EffectEndVoid)UponEnd, CustomGameOptions.MaxRevives, (EndFunc)EndEffect, new CanClickAgain(false));
    }

    public bool EndEffect() => Dead;

    public void UponEnd()
    {
        if (!(Meeting || Dead))
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

        if (player.Is(LayerEnum.Lovers) && CustomGameOptions.BothLoversDie)
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
        RevivingBody = ReviveButton.TargetBody;
        Spread(Player, PlayerByBody(RevivingBody));
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, ReviveButton, RevivingBody);
        ReviveButton.Begin();
        Flash(Color, CustomGameOptions.ReviveDur);

        if (CustomGameOptions.AltruistTargetBody)
            ReviveButton.TargetBody?.gameObject.Destroy();
    }

    public override void ReadRPC(MessageReader reader)
    {
        RevivingBody = reader.ReadBody();

        if (CustomPlayer.Local.PlayerId == RevivingBody.ParentId)
            Flash(CustomColorManager.Altruist, CustomGameOptions.ReviveDur);

        if (CustomGameOptions.AltruistTargetBody)
            RevivingBody.gameObject.Destroy();
    }
}