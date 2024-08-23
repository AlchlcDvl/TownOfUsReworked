namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Detective : Crew
{
    [NumberOption(MultiMenu2.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static float ExamineCd { get; set; } = 25f;

    [NumberOption(MultiMenu2.LayerSubOptions, 5f, 60f, 2.5f, Format.Time)]
    public static float RecentKill { get; set; } = 10f;

    [NumberOption(MultiMenu2.LayerSubOptions, 0.05f, 2f, 0.05f, Format.Time)]
    public static float FootprintInterval { get; set; } = 0.15f;

    [NumberOption(MultiMenu2.LayerSubOptions, 1f, 10f, 0.5f, Format.Time)]
    public static float FootprintDur { get; set; } = 10f;

    [StringOption(MultiMenu2.LayerSubOptions)]
    public static FootprintVisibility AnonymousFootPrint { get; set; } = FootprintVisibility.OnlyWhenCamouflaged;

    public CustomButton ExamineButton { get; set; }
    private static float _time;
    public List<Footprint> AllPrints { get; set; }
    public List<byte> Investigated { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Detective : CustomColorManager.Crew;
    public override string Name => "Detective";
    public override LayerEnum Type => LayerEnum.Detective;
    public override Func<string> StartText => () => "Examine Players For <color=#AA0000FF>Blood</color>";
    public override Func<string> Description => () => "- You can examine players to see if they have killed recently\n- Your screen will flash red if your target has killed in the last " +
        $"{RecentKill}s\n- You can view everyone's footprints to see where they go or where they came from";

    public override void Init()
    {
        BaseStart();
        AllPrints = [];
        Investigated = [];
        Alignment = Alignment.CrewInvest;
        ExamineButton = CreateButton(this, "EXAMINE", new SpriteName("Examine"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Examine, new Cooldown(ExamineCd));
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
                RecentKill) ? UColor.red : UColor.green);
            Investigated.Add(ExamineButton.TargetPlayer.PlayerId);
        }

        ExamineButton.StartCooldown(cooldown);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (!Dead)
        {
            _time += Time.deltaTime;

            if (_time >= FootprintInterval)
            {
                _time -= FootprintInterval;

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
        else if (AllPrints.Count > 0)
            OnLobby();
    }
}