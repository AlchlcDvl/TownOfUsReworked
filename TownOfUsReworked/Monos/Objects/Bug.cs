namespace TownOfUsReworked.Monos;

public class Bug : Range
{
    [HideFromIl2Cpp]
    private Dictionary<byte, float> Players { get; } = [];

    [HideFromIl2Cpp]
    private Dictionary<byte, LayerEnum> Results { get; } = [];

    public override void Update()
    {
        base.Update();
        var closest = GetClosestPlayers(transform.position, Size);
        var remove = new List<byte>();

        foreach (var player in Players.Keys)
        {
            if (!closest.Contains(PlayerById(player)))
                remove.Add(player);
        }

        remove.ForEach(x => Players.Remove(x));

        foreach (var player in closest)
        {
            if (!Players.TryGetValue(player.PlayerId, out var time))
                Players.Add(player.PlayerId, 0f);

            Players[player.PlayerId] = time + Time.deltaTime;

            if (Players[player.PlayerId] >= Operative.MinAmountOfTimeInBug && player != Owner && !Results.ContainsKey(player.PlayerId))
            {
                var role = player.GetRole();
                var type = role.Type;
                Results[player.PlayerId] = type;

                if (role is Operative op)
                    op.BuggedPlayers.Add(type);
                else if (role is Retributionist ret)
                    ret.BuggedPlayers.Add(type);
            }
        }
    }

    [HideFromIl2Cpp]
    public string GetResults()
    {
        var result = "";
        var results = Results.Values.ToList();
        results.Shuffle();
        results.ForEach(role => result += $"{LayerDictionary[role].Name}, ");
        result = result.Remove(result.Length - 2);

        if (Operative.PreciseOperativeInfo)
            result = $"Bug {Number}: {result}";

        Results.Clear();
        Players.Clear();
        return result;
    }

    public static Bug CreateBug(PlayerControl owner)
    {
        var gameObject = CreateRange(CustomColorManager.Operative, Operative.BugRange, "Bug");
        var bug = gameObject.AddComponent<Bug>();
        bug.Owner = owner;
        bug.Size = Operative.BugRange;
        var position = owner.GetTruePosition();
        gameObject.transform.position = new(position.x, position.y, (position.y / 1000f) + 0.001f);
        return bug;
    }
}