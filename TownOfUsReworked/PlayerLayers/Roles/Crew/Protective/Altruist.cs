namespace TownOfUsReworked.PlayerLayers.Roles;

public class Altruist : Crew
{
    public CustomButton ReviveButton { get; set; }
    public int UsesLeft { get; set; }
    public bool ButtonUsable => UsesLeft > 0;
    public bool Reviving { get; set; }
    public float TimeRemaining { get; set; }
    public bool IsReviving => TimeRemaining > 0f;
    public DeadBody RevivingBody { get; set; }
    public bool Success { get; set; }
    public DateTime LastRevived { get; set; }
    public float Timer => ButtonUtils.Timer(Player, LastRevived, CustomGameOptions.ReviveCooldown);

    public override Color32 Color => ClientGameOptions.CustomCrewColors ? Colors.Altruist : Colors.Crew;
    public override string Name => "Altruist";
    public override LayerEnum Type => LayerEnum.Altruist;
    public override Func<string> StartText => () => "Sacrifice Yourself To Save Another";
    public override Func<string> Description => () => $"- You can revive a dead body\n- Reviving someone takes {CustomGameOptions.AltReviveDuration}s\n- If a meeting is called during"
        + " your revive, the revive fails";
    public override InspectorResults InspectorResults => InspectorResults.PreservesLife;

    public Altruist(PlayerControl player) : base(player)
    {
        RoleAlignment = RoleAlignment.CrewProt;
        UsesLeft = CustomGameOptions.ReviveCount;
        ReviveButton = new(this, "Revive", AbilityTypes.Dead, "ActionSecondary", HitRevive, true);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        ReviveButton.Update("REVIVE", Timer, CustomGameOptions.ReviveCooldown, UsesLeft, IsReviving, TimeRemaining, CustomGameOptions.AltReviveDuration, true, ButtonUsable);
    }

    public void Revive()
    {
        if (!Reviving && CustomPlayer.Local.PlayerId == ReviveButton.TargetBody.ParentId)
        {
            Flash(Color, CustomGameOptions.AltReviveDuration);

            if (CustomGameOptions.AltruistTargetBody)
                ReviveButton.TargetBody?.gameObject.Destroy();
        }

        Reviving = true;
        TimeRemaining -= Time.deltaTime;

        if (Meeting || IsDead)
        {
            Success = false;
            TimeRemaining = 0f;
        }
    }

    public void UnRevive()
    {
        Reviving = false;
        LastRevived = DateTime.UtcNow;

        if (Success)
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
        UsesLeft--;

        if (player.Is(LayerEnum.Lovers) && CustomGameOptions.BothLoversDie)
        {
            var lover = player.GetOtherLover();
            var loverRole = GetRole(lover);
            loverRole.DeathReason = DeathReasonEnum.Revived;
            loverRole.KilledBy = " By " + PlayerName;
            lover.Revive();
        }

        if (UsesLeft == 0)
            RpcMurderPlayer(Player, Player);

        if (formerKiller.Contains(CustomPlayer.LocalCustom.Data.PlayerName))
        {
            LocalRole.AllArrows.Add(player.PlayerId, new(CustomPlayer.Local, Color));
            Flash(Color);
        }
    }

    public void HitRevive()
    {
        if (IsTooFar(Player, ReviveButton.TargetBody) || Timer != 0f || !ButtonUsable)
            return;

        RevivingBody = ReviveButton.TargetBody;
        Spread(Player, PlayerByBody(RevivingBody));
        CallRpc(CustomRPC.Action, ActionsRPC.AltruistRevive, this, RevivingBody);
        TimeRemaining = CustomGameOptions.AltReviveDuration;
        Success = true;
        Revive();
    }
}