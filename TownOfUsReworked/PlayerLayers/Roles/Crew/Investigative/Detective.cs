namespace TownOfUsReworked.PlayerLayers.Roles;

public class Detective : Crew
{
    public CustomButton ExamineButton { get; set; }
    private static float _time;

    public override Color Color => ClientGameOptions.CustomCrewColors ? Colors.Detective : Colors.Crew;
    public override string Name => "Detective";
    public override LayerEnum Type => LayerEnum.Detective;
    public override Func<string> StartText => () => "Examine Players For <color=#AA0000FF>Blood</color>";
    public override Func<string> Description => () => "- You can examine players to see if they have killed recently\n- Your screen will flash red if your target has killed in the last " +
        $"{CustomGameOptions.RecentKill}s\n- You can view everyone's footprints to see where they go or where they came from";
    public override InspectorResults InspectorResults => InspectorResults.GainsInfo;

    public Detective(PlayerControl player) : base(player)
    {
        Alignment = Alignment.CrewInvest;
        ExamineButton = new(this, "Examine", AbilityTypes.Target, "ActionSecondary", Examine, CustomGameOptions.ExamineCd);
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
        var interact = Interact(Player, ExamineButton.TargetPlayer);
        var cooldown = CooldownType.Reset;

        if (interact.AbilityUsed)
        {
            Flash(ExamineButton.TargetPlayer.IsFramed() || KilledPlayers.Any(x => x.KillerId == ExamineButton.TargetPlayer.PlayerId && (DateTime.UtcNow - x.KillTime).TotalSeconds <=
                CustomGameOptions.RecentKill) ? UColor.red : UColor.green);
        }

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;

        ExamineButton.StartCooldown(cooldown);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        ExamineButton.Update2("EXAMINE");

        if (!IsDead)
        {
            _time += Time.deltaTime;

            if (_time >= CustomGameOptions.FootprintInterval)
            {
                _time -= CustomGameOptions.FootprintInterval;

                foreach (var player in CustomPlayer.AllPlayers)
                {
                    if (player.HasDied() || player == CustomPlayer.Local)
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