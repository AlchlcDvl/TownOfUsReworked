namespace TownOfUsReworked.PlayerLayers.Roles;

public sealed class Retributionist : CSupport
{
    public static bool Exists;

    protected override UColor MainColor => RevivedRole?.Color ?? CustomColorManager.Retributionist;
    public override Layer Type => Layer.Retributionist;
    public override string StartText => "Mimic the Dead";
    public override string Description => "- You can mimic the abilities of dead <#8CFFFFFF>Crew</color>" + (RevivedRole ? $"\n{RevivedRole.Description}" : "");
    public override Attack Attack => RevivedRole switch
    {
        Bastion or Veteran { AlertButton.EffectActive: true } => Attack.Powerful,
        Vigilante => Attack.Basic,
        _ => Attack.None
    };
    public override Defense Defense => RevivedRole is Veteran { AlertButton.EffectActive: true } ? Defense.Basic : Defense.None;
    public override bool RoleBlockImmune => RevivedRole?.RoleBlockImmune ?? false;
    public override bool Local => Player?.AmOwner ?? false;
    public override bool Dead => Data?.IsDead ?? true;
    public override LayerHandler Handler { get; set; }

    private PlayerVoteArea Selected;
    public Crew RevivedRole;
    public CustomMeeting RetMenu;

    public override void Init() => RetMenu = new(Player, "RetActive", "RetDisabled", SetActive, IsExempt, new(-0.4f, 0.03f, -1.3f));

    private bool IsExempt(PlayerVoteArea voteArea)
    {
        var player = PlayerByVoteArea(voteArea);
        return Dead || !voteArea.AmDead || !player.HasDied() || player.GetRole() is Sovereign or Retributionist or not Crew || (player.Is<Revealer>(out var rev) && rev.FormerRole is Sovereign or
            Retributionist);
    }

    private void SetActive(PlayerVoteArea voteArea, MeetingHud __instance)
    {
        if (__instance.state == MeetingHud.VoteStates.Discussion)
            return;

        if (Selected)
            RetMenu.Actives[Selected.TargetPlayerId] = false;

        Selected = voteArea;
        RetMenu.Actives[voteArea.TargetPlayerId] = true;
    }

    public override void ReadRPC(RpcReader reader)
    {
        var role = reader.ReadLayer() as Crew;
        RevivedRole = role is Revealer rev ? rev.FormerRole : role;
        role.MimickedBy = this;
        Handler.ResetButtons();
    }

    public override void UpdateMeeting(MeetingHud __instance) => RetMenu.Update();

    public override void VoteComplete(MeetingHud __instance)
    {
        RetMenu.HideButtons();

        if (!Selected || Dead)
            return;

        var role = PlayerByVoteArea(Selected)?.GetRole() as Crew;
        RevivedRole = role is Revealer rev ? rev.FormerRole : role;
        role.EnteringLayer();
        role.MimickedBy = this;
        Handler.ResetButtons();
        PerformRpcAction(role);
        Selected = null;
    }

    public override void LocalOnMeetingStart(MeetingHud __instance)
    {
        RetMenu.GenButtons(__instance);

        if (RevivedRole)
            RevivedRole.LocalOnMeetingStart(__instance);
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        if (RevivedRole)
        {
            RevivedRole.OnMeetingStart(__instance);
            RevivedRole.ExitingLayer();
            RevivedRole.MimickedBy = null;
        }

        RevivedRole = null;
        Handler.ResetButtons();
    }

    public override void BeforeMeeting()
    {
        if (RevivedRole)
            RevivedRole.BeforeMeeting();
    }

    public override void ClearArrows()
    {
        if (RevivedRole)
            RevivedRole.ClearArrows();
    }

    public override void OnBodyReport(NetworkedPlayerInfo info)
    {
        if (RevivedRole)
            RevivedRole.OnBodyReport(info);
    }

    public override void OnDeath(DeathReasonEnum reason, PlayerControl killer)
    {
        if (RevivedRole)
            RevivedRole.OnDeath(reason, killer);
    }

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (RevivedRole)
            RevivedRole.UpdatePlayerName(handler, player, meeting, ref name, ref color, ref revealed, ref removeFromConsig);
    }
}