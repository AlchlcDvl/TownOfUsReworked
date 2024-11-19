namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Detective : Crew
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number ExamineCd { get; set; } = new(25);

    [NumberOption(MultiMenu.LayerSubOptions, 5f, 60f, 2.5f, Format.Time)]
    public static Number RecentKill { get; set; } = new(10);

    [NumberOption(MultiMenu.LayerSubOptions, 0.05f, 2f, 0.05f, Format.Time)]
    public static Number FootprintInterval { get; set; } = new(0.15f);

    [NumberOption(MultiMenu.LayerSubOptions, 1f, 10f, 0.5f, Format.Time)]
    public static Number FootprintDur { get; set; } = new(10);

    [StringOption(MultiMenu.LayerSubOptions)]
    public static FootprintVisibility AnonymousFootPrint { get; set; } = FootprintVisibility.OnlyWhenCamouflaged;

    public CustomButton ExamineButton { get; set; }
    public List<Footprint> AllPrints { get; set; }
    public List<byte> Investigated { get; set; }
    private Dictionary<byte, float> PlayerTimes { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Detective : CustomColorManager.Crew;
    public override string Name => "Detective";
    public override LayerEnum Type => LayerEnum.Detective;
    public override Func<string> StartText => () => "Examine Players For <color=#AA0000FF>Blood</color>";
    public override Func<string> Description => () => "- You can examine players to see if they have killed recently\n- Your screen will flash red if your target has killed in the last " +
        $"{RecentKill}s\n- You can view everyone's footprints to see where they go or where they came from";

    public override void Init()
    {
        base.Init();
        AllPrints = [];
        Investigated = [];
        PlayerTimes = [];
        Alignment = Alignment.CrewInvest;
        ExamineButton ??= new(this, "EXAMINE", new SpriteName("Examine"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Examine, new Cooldown(ExamineCd));
    }

    public override void Deinit()
    {
        base.Deinit();
        Clear();
    }

    public override void BeforeMeeting() => Clear();

    public void Clear()
    {
        AllPrints.ForEach(x => x.Destroy());
        AllPrints.Clear();
    }

    private static Vector2 Position(PlayerControl player) => player.GetTruePosition() + new Vector2(0, 0.366667f);

    public void Examine()
    {
        var target = ExamineButton.GetTarget<PlayerControl>();
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            Flash(target.IsFramed() || KilledPlayers.Any(x => x.KillerId == target.PlayerId && (DateTime.UtcNow - x.KillTime).TotalSeconds <= RecentKill) ? UColor.red : UColor.green);
            Investigated.Add(target.PlayerId);
        }

        ExamineButton.StartCooldown(cooldown);
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (!Dead)
        {
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
        else if (AllPrints.Count > 0)
            Deinit();
    }

    public override void UpdatePlayer(PlayerControl __instance)
    {
        if (Dead || !Investigated.Contains(__instance.PlayerId) || __instance.AmOwner || __instance.HasDied())
            return;

        if (!PlayerTimes.ContainsKey(__instance.PlayerId))
            PlayerTimes.Add(__instance.PlayerId, 0f);

        PlayerTimes[__instance.PlayerId] += Time.deltaTime;

        if (PlayerTimes[__instance.PlayerId] >= FootprintInterval)
        {
            PlayerTimes[__instance.PlayerId] -= FootprintInterval;

            if (!AllPrints.Any(print => Vector2.Distance(print.Position, Position(__instance)) < 0.5f && print.Color.a > 0.5 && print.PlayerId == __instance.PlayerId))
                AllPrints.Add(new(__instance));
        }
    }
}