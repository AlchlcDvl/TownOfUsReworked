namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Operative : Crew
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number BugCd = 25;

    [NumberOption(0f, 15f, 0.5f, Format.Time)]
    public static Number MinAmountOfTimeInBug = 0;

    [ToggleOption]
    public static bool BugsRemoveOnNewRound = true;

    [NumberOption(0, 15, 1, zeroIsInf: true)]
    public static Number MaxBugs = 5;

    [NumberOption(0.5f, 5f, 0.25f, Format.Distance)]
    public static Number BugRange = 1.5f;

    [NumberOption(1, 10, 1)]
    public static Number MinAmountOfPlayersInBug = 1;

    [StringOption<AdminDeadPlayers>]
    public static AdminDeadPlayers WhoSeesDead = AdminDeadPlayers.Nobody;

    [ToggleOption]
    public static bool PreciseOperativeInfo = false;

    public List<Bug> Bugs { get; } = [];
    public List<LayerEnum> BuggedPlayers { get; } = [];
    public CustomButton BugButton { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Operative : FactionColor;
    public override LayerEnum Type => LayerEnum.Operative;
    public override Func<string> StartText => () => "Detect Which Roles Are Here";
    public override Func<string> Description => () => "- You can place bugs around the map\n- Upon triggering the bugs, the player's role will be included in a list to be shown in the next" +
        " meeting\n- You can see which colors are where on the admin table\n- On Vitals, the time of death for each player will be shown";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Investigative;
        BuggedPlayers.Clear();
        Bugs.Clear();
        BugButton ??= new(this, "BUG", new SpriteName("Bug"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)PlaceBug, new Cooldown(BugCd), MaxBugs,
            (ConditionFunc)Condition);
    }

    public override void Deinit()
    {
        base.Deinit();
        Bugs.ForEach(x => x?.gameObject?.Destroy());
        Bugs.Clear();
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);

        if (Dead)
            return;

        var message = "";

        if (BuggedPlayers.Count == 0)
            message = "No one triggered your bugs.";
        else if (BuggedPlayers.Count < MinAmountOfPlayersInBug)
            message = "Not enough players triggered your bugs.";
        else if (BuggedPlayers.Count == 1)
        {
            var result = BuggedPlayers[0];
            var a_an = result is LayerEnum.Altruist or LayerEnum.Engineer or LayerEnum.Escort or LayerEnum.Operative or LayerEnum.Amnesiac or LayerEnum.Actor or LayerEnum.Arsonist or
                LayerEnum.Executioner or LayerEnum.Ambusher or LayerEnum.Enforcer or LayerEnum.Impostor or LayerEnum.Anarchist ? "n" : "";
            message = $"A{a_an} {result} triggered your bug.";
        }
        else if (PreciseOperativeInfo)
        {
            message = "Your bugs returned the following results:";
            Bugs.ForEach(bug => message += $"\n{bug.GetResults()}");
        }
        else
        {
            message = "The following roles triggered your bugs: ";
            BuggedPlayers.Shuffle();
            BuggedPlayers.ForEach(role => message += $"{LayerDictionary[role].Name}, ");
            message = message[..^2];
        }

        if (!IsNullEmptyOrWhiteSpace(message))
            Run("<#A7D1B3FF>〖 Bug Results 〗</color>", message);
    }

    public bool Condition() => !Bugs.Any(x => Vector2.Distance(Player.transform.position, x.transform.position) < x.Size * 2);

    public void PlaceBug()
    {
        Bugs.Add(Bug.CreateBug(Player));
        BugButton.StartCooldown();
    }
}