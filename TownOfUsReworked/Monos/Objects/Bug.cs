namespace TownOfUsReworked.Monos;

public sealed class Bug : Range
{
    private Operative? OwnerBugger;
    private int BugNumber;
    private readonly Dictionary<byte, float> Players = [];
    private readonly Dictionary<byte, Layer> Results = [];
    private readonly List<byte> StalePlayers = [];

    public override void Update()
    {
        base.Update();
        var closest = GetClosestPlayers(transform.position, Size);

        StalePlayers.AddRange(Players.Keys);

        foreach (var player in closest)
        {
            if (player == Owner)
                continue;

            var id = player.PlayerId;

            StalePlayers.Remove(id);

            if (Results.ContainsKey(id))
                continue;

            Players.TryGetValue(id, out var time);
            var newTime = time + Time.deltaTime;
            Players[id] = newTime;

            if (newTime < Operative.MinAmountOfTimeInBug)
                continue;

            var type = player.GetRole().Type;
            Results[id] = type;
            OwnerBugger!.BuggedPlayers.Add(type);
        }

        foreach (var staleId in StalePlayers)
            Players.Remove(staleId);

        StalePlayers.Clear();
    }

    [HideFromIl2Cpp]
    public string GetResults()
    {
        var results = Results.Values.ToList();
        results.Shuffle();
        var result = Join(", ", results.Select(x => LayerDictionary[x].Name));

        if (Operative.PreciseOperativeInfo)
            result = $"Bug {BugNumber}: {result}";

        Results.Clear();
        Players.Clear();
        return result;
    }

    public static Bug CreateBug(PlayerControl owner)
    {
        var gameObject = CreateRange(CustomColorManager.Operative, Operative.BugRange, "Bug", owner.GetTruePosition());
        var bug = gameObject.AddComponent<Bug>();
        bug.Owner = owner;
        bug.OwnerBugger = owner.GetLayer<Operative>();
        bug.Size = Operative.BugRange;
        bug.BugNumber = Number;
        return bug;
    }
}