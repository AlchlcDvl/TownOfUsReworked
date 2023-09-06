namespace TownOfUsReworked.Objects;

public class Bug : Range
{
    private readonly Dictionary<byte, float> Players = new();
    private List<PlayerControl> Closest { get; set; }
    private readonly Dictionary<PlayerControl, LayerEnum> Results = new();

    public Bug(PlayerControl owner) : base(owner, Colors.Operative, CustomGameOptions.BugRange, "Bug")
    {
        Results = new();
        Closest = new();
        Players = new();
        Coroutines.Start(Timer());
    }

    public override IEnumerator Timer()
    {
        while (Transform)
        {
            yield return 0;
            Update();
        }
    }

    public override void Update()
    {
        base.Update();
        Closest = GetClosestPlayers(Transform.position, Size);
        var remove = new List<byte>();

        foreach (var player in Players.Keys)
        {
            if (!Closest.Contains(PlayerById(player)))
                remove.Add(player);
        }

        remove.ForEach(x => Players.Remove(x));

        foreach (var player in Closest)
        {
            if (!Players.ContainsKey(player.PlayerId))
                Players.Add(player.PlayerId, 0f);

            Players[player.PlayerId] += Time.deltaTime;

            if (Players[player.PlayerId] >= CustomGameOptions.MinAmountOfTimeInBug && player != Owner && !Results.ContainsKey(player))
            {
                var type = Role.GetRole(player).Type;
                Results.Add(player, type);

                if (Owner.Is(LayerEnum.Operative))
                    Role.GetRole<Operative>(Owner).BuggedPlayers.Add(type);
                else if (Owner.Is(LayerEnum.Retributionist))
                    Role.GetRole<Retributionist>(Owner).BuggedPlayers.Add(type);
            }
        }
    }

    public string GetResults()
    {
        var result = "";
        var results = Results.Values.ToList();
        results.Shuffle();
        results.ForEach(role => result += $"{Role.GetRoles(role)[0]}, ");
        result = result.Remove(result.Length - 2);

        if (CustomGameOptions.PreciseOperativeInfo)
            result = $"Bug {Number}: {result}";

        Results.Clear();
        Players.Clear();
        return result;
    }

    public static void Clear(List<Bug> obj)
    {
        obj.ForEach(b => b.Destroy());
        obj.Clear();
    }
}