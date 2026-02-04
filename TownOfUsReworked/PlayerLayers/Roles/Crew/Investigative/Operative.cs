namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Operative)]
public sealed class Operative : Investigative
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

    public readonly List<Layer> BuggedPlayers = [];

    private CustomButton BugButton;
    private readonly List<Bug> Bugs = [];

    protected override UColor MainColor => CustomColorManager.Operative;
    public override Layer Type => Layer.Operative;
    public override string StartText => "Detect Which Roles Are Here";
    public override string Description => "- You can place bugs around the map\n- Upon triggering the bugs, the player's role will be included in a list to be shown in the next" +
        " meeting\n- You can see which colors are where on the admin table\n- On Vitals, the time of death for each player will be shown";

    public override void Init()
    {
        BuggedPlayers.Clear();
        Bugs.Clear();
        BugButton ??= new(this, "BUG", new SpriteName("Bug"), ReworkedAbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)PlaceBug, new Cooldown(BugCd), MaxBugs,
            (ConditionFunc)Condition);
    }

    public override void Reset(bool meeting, bool start)
    {
        BuggedPlayers.Clear();

        if (!BugsRemoveOnNewRound || !meeting)
            return;

        Bugs.ForEach(x => x.gameObject.Destroy());
        Bugs.Clear();
    }

    protected override void Deinit()
    {
        Bugs.ForEach(x => x?.gameObject?.Destroy());
        Bugs.Clear();
    }

    public override void LocalOnMeetingStart(MeetingHud __instance)
    {
        if (Dead)
            return;

        string message;

        if (BuggedPlayers.Count == 0)
            message = "No one triggered your bugs.";
        else if (BuggedPlayers.Count < MinAmountOfPlayersInBug)
            message = "Not enough players triggered your bugs.";
        else if (BuggedPlayers.Count == 1)
        {
            var result = BuggedPlayers[0];
            var aAn = result is Layer.Altruist or Layer.Engineer or Layer.Escort or Layer.Operative or Layer.Amnesiac or Layer.Actor or Layer.Arsonist or
                Layer.Executioner or Layer.Ambusher or Layer.Enforcer or Layer.Impostor or Layer.Anarchist ? "n" : string.Empty;
            message = $"A{aAn} {result} triggered your bug.";
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
            message += Join(", ", BuggedPlayers.Select(x => LayerDictionary[x].Name));
        }

        if (!IsNullEmptyOrWhiteSpace(message))
            Run("<#A7D1B3FF>〖 Bug Results 〗</color>", message);
    }

    private bool Condition() => !Bugs.Any(x => Vector2.Distance(Player.transform.position, x.transform.position) < x.Size * 2);

    private void PlaceBug()
    {
        Bugs.Add(Bug.CreateBug(Player));
        BugButton.StartCooldown();
    }
}