namespace TownOfUsReworked.PlayerLayers.Roles;

public class Detective : Crew
{
    public DateTime LastExamined { get; set; }
    public CustomButton ExamineButton { get; set; }
    public float Timer => ButtonUtils.Timer(Player, LastExamined, CustomGameOptions.ExamineCd);
    private static float _time;

    public override Color32 Color => ClientGameOptions.CustomCrewColors ? Colors.Detective : Colors.Crew;
    public override string Name => "Detective";
    public override LayerEnum Type => LayerEnum.Detective;
    public override Func<string> StartText => () => "Examine Players For <color=#AA0000FF>Blood</color>";
    public override Func<string> Description => () => "- You can examine players to see if they have killed recently\n- Your screen will flash red if your target has killed in the " +
        $"last {CustomGameOptions.RecentKill}s\n- You can view everyone's footprints to see where they go or where they came from";
    public override InspectorResults InspectorResults => InspectorResults.GainsInfo;

    public Detective(PlayerControl player) : base(player)
    {
        RoleAlignment = RoleAlignment.CrewInvest;
        ExamineButton = new(this, "Examine", AbilityTypes.Direct, "ActionSecondary", Examine);
    }

    public override void OnLobby()
    {
        base.OnLobby();
        AllPrints.ForEach(x => x.Destroy());
        AllPrints.Clear();
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);
        AllPrints.ForEach(x => x.Destroy());
        AllPrints.Clear();
    }

    private static Vector2 Position(PlayerControl player) => player.GetTruePosition() + new Vector2(0, 0.366667f);

    public void Examine()
    {
        if (Timer != 0f || IsTooFar(Player, ExamineButton.TargetPlayer))
            return;

        var interact = Interact(Player, ExamineButton.TargetPlayer);

        if (interact[3])
        {
            Flash(ExamineButton.TargetPlayer.IsFramed() || KilledPlayers.Any(x => x.KillerId == ExamineButton.TargetPlayer.PlayerId && (DateTime.UtcNow - x.KillTime).TotalSeconds <=
                CustomGameOptions.RecentKill) ? UColor.red : UColor.green);
        }

        if (interact[0])
            LastExamined = DateTime.UtcNow;
        else if (interact[1])
            LastExamined.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        ExamineButton.Update("EXAMINE", Timer, CustomGameOptions.ExamineCd);

        if (!IsDead)
        {
            _time += Time.deltaTime;

            if (_time >= CustomGameOptions.FootprintInterval)
            {
                _time -= CustomGameOptions.FootprintInterval;

                foreach (var player in CustomPlayer.AllPlayers)
                {
                    if (player.Data.IsDead || player.Data.Disconnected || player == CustomPlayer.Local)
                        continue;

                    if (!AllPrints.Any(print => Vector3.Distance(print.Position, Position(player)) < 0.5f && print.Color.a > 0.5 && print.PlayerId == player.PlayerId))
                        AllPrints.Add(new(player));
                }

                for (var i = 0; i < AllPrints.Count; i++)
                {
                    try
                    {
                        var footprint = AllPrints[i];

                        if (footprint.Update())
                            i--;
                    } catch { /*Assume footprint value is null and allow the loop to continue*/ }
                }
            }
        }
        else
            OnLobby();
    }
}