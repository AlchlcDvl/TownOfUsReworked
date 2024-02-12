namespace TownOfUsReworked.PlayerLayers.Roles;

public class Detective : Crew
{
    public CustomButton ExamineButton { get; set; }
    private static float _time;
    public List<Footprint> AllPrints { get; set; }
    public List<byte> Investigated { get; set; }

    public override UColor Color => ClientGameOptions.CustomCrewColors ? CustomColorManager.Detective : CustomColorManager.Crew;
    public override string Name => "Detective";
    public override LayerEnum Type => LayerEnum.Detective;
    public override Func<string> StartText => () => "Examine Players For <color=#AA0000FF>Blood</color>";
    public override Func<string> Description => () => "- You can examine players to see if they have killed recently\n- Your screen will flash red if your target has killed in the last " +
        $"{CustomGameOptions.RecentKill}s\n- You can view everyone's footprints to see where they go or where they came from";

    public Detective() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        BaseStart();
        AllPrints = new();
        Investigated = new();
        Alignment = Alignment.CrewInvest;
        ExamineButton = new(this, "Examine", AbilityTypes.Alive, "ActionSecondary", Examine, CustomGameOptions.ExamineCd);
        return this;
    }

    public override void OnLobby()
    {
        base.OnLobby();
        Clear();
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);
        Clear();
    }

    public void Clear()
    {
        AllPrints.ForEach(x => x.Destroy());
        AllPrints.Clear();
    }

    private static Vector2 Position(PlayerControl player) => player.GetTruePosition() + new Vector2(0, 0.366667f);

    public void Examine()
    {
        var cooldown = Interact(Player, ExamineButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
        {
            Flash(ExamineButton.TargetPlayer.IsFramed() || KilledPlayers.Any(x => x.KillerId == ExamineButton.TargetPlayer.PlayerId && (DateTime.UtcNow - x.KillTime).TotalSeconds <=
                CustomGameOptions.RecentKill) ? UColor.red : UColor.green);
            Investigated.Add(ExamineButton.TargetPlayer.PlayerId);
        }

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

                foreach (var id in Investigated)
                {
                    var player = PlayerById(id);

                    if (player.HasDied() || player == CustomPlayer.Local)
                        continue;

                    if (!AllPrints.Any(print => Vector2.Distance(print.Position, Position(player)) < 0.5f && print.Color.a > 0.5 && print.PlayerId == player.PlayerId))
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