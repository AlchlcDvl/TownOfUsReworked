namespace TownOfUsReworked.PlayerLayers.Roles;

public class Coroner : Crew
{
    public Dictionary<byte, CustomArrow> BodyArrows { get; set; }
    public List<byte> Reported { get; set; }
    public CustomButton CompareButton { get; set; }
    public List<DeadPlayer> ReferenceBodies { get; set; }
    public DateTime LastCompared { get; set; }
    public DateTime LastAutopsied { get; set; }
    public CustomButton AutopsyButton { get; set; }
    public float AutopsyTimer => ButtonUtils.Timer(Player, LastAutopsied, CustomGameOptions.AutopsyCd);
    public float CompareTimer => ButtonUtils.Timer(Player, LastCompared, CustomGameOptions.CompareCd);

    public override Color Color => ClientGameOptions.CustomCrewColors ? Colors.Coroner : Colors.Crew;
    public override string Name => "Coroner";
    public override LayerEnum Type => LayerEnum.Coroner;
    public override Func<string> StartText => () => "Examine The Dead For Information";
    public override Func<string> Description => () => "- You know when players die and will be notified to as to where their body is for a brief period of time\n- You will get a " +
        "report when you report a body\n- You can perform an autopsy on bodies, to get a reference\n- You can compare the autopsy reference with players to see if they killed the body "
        + "you examined";
    public override InspectorResults InspectorResults => InspectorResults.DealsWithDead;

    public Coroner(PlayerControl player) : base(player)
    {
        Alignment = Alignment.CrewInvest;
        BodyArrows = new();
        Reported = new();
        ReferenceBodies = new();
        AutopsyButton = new(this, "Autopsy", AbilityTypes.Dead, "ActionSecondary", Autopsy);
        CompareButton = new(this, "Compare", AbilityTypes.Direct, "Secondary", Compare);
    }

    public void DestroyArrow(byte targetPlayerId)
    {
        BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId).Value?.Destroy();
        BodyArrows.Remove(targetPlayerId);
    }

    public override void OnLobby()
    {
        base.OnLobby();
        BodyArrows.Values.ToList().DestroyAll();
        BodyArrows.Clear();
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        AutopsyButton.Update("AUTOPSY", AutopsyTimer, CustomGameOptions.AutopsyCd);
        CompareButton.Update("COMPARE", CompareTimer, CustomGameOptions.CompareCd, true, ReferenceBodies.Count > 0);

        if (!CustomPlayer.LocalCustom.IsDead)
        {
            var validBodies = AllBodies.Where(x => KilledPlayers.Any(y => y.PlayerId == x.ParentId && DateTime.UtcNow < y.KillTime.AddSeconds(CustomGameOptions.CoronerArrowDur)));

            foreach (var bodyArrow in BodyArrows.Keys)
            {
                if (!validBodies.Any(x => x.ParentId == bodyArrow))
                    DestroyArrow(bodyArrow);
            }

            foreach (var body in validBodies)
            {
                if (!BodyArrows.ContainsKey(body.ParentId))
                    BodyArrows.Add(body.ParentId, new(Player, Color));

                BodyArrows[body.ParentId]?.Update(body.TruePosition);
            }
        }
        else if (BodyArrows.Count != 0)
            OnLobby();
    }

    public void Autopsy()
    {
        if (IsTooFar(Player, AutopsyButton.TargetBody) || AutopsyTimer != 0f)
            return;

        var playerId = AutopsyButton.TargetBody.ParentId;
        Spread(Player, PlayerById(playerId));
        ReferenceBodies.AddRange(KilledPlayers.Where(x => x.PlayerId == playerId));
        LastAutopsied = DateTime.UtcNow;
    }

    public void Compare()
    {
        if (ReferenceBodies.Count == 0 || IsTooFar(Player, CompareButton.TargetPlayer) || CompareTimer != 0f)
            return;

        var interact = Interact(Player, CompareButton.TargetPlayer);

        if (interact.AbilityUsed)
            Flash(ReferenceBodies.Any(x => CompareButton.TargetPlayer.PlayerId == x.KillerId) ? UColor.red : UColor.green);

        if (interact.Reset)
            LastCompared = DateTime.UtcNow;
        else if (interact.Protected)
            LastCompared.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    public override void OnBodyReport(GameData.PlayerInfo info)
    {
        base.OnBodyReport(info);

        if (info == null || !Local)
            return;

        var body = KilledPlayers.Find(x => x.PlayerId == info.PlayerId);

        if (body == null)
            return;

        Reported.Add(info.PlayerId);
        body.Reporter = Player;
        body.KillAge = (float)(DateTime.UtcNow - body.KillTime).TotalMilliseconds;
        var reportMsg = body.ParseBodyReport();

        if (string.IsNullOrWhiteSpace(reportMsg))
            return;

        //Only Coroner can see this
        if (HUD)
            Run(HUD.Chat, "<color=#4D99E6FF>〖 Autopsy Results 〗</color>", reportMsg);
    }
}