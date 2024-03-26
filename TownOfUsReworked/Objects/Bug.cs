namespace TownOfUsReworked.Objects;

public class Bug(PlayerControl owner) : Range(owner, CustomColorManager.Operative, CustomGameOptions.BugRange, "Bug")
{
    private Dictionary<byte, float> Players { get; } = [];
    private Dictionary<byte, LayerEnum> Results { get; } = [];

    public override void Update()
    {
        base.Update();

        if (!Transform)
            return;

        var closest = GetClosestPlayers(Transform.position, Size);
        var remove = new List<byte>();

        foreach (var player in Players.Keys)
        {
            if (!closest.Contains(PlayerById(player)))
                remove.Add(player);
        }

        remove.ForEach(x => Players.Remove(x));

        foreach (var player in closest)
        {
            if (!Players.ContainsKey(player.PlayerId))
                Players.Add(player.PlayerId, 0f);

            Players[player.PlayerId] += Time.deltaTime;

            if (Players[player.PlayerId] >= CustomGameOptions.MinAmountOfTimeInBug && player != Owner && !Results.ContainsKey(player.PlayerId))
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

    public string GetResults()
    {
        var result = "";
        var results = Results.Values.ToList();
        results.Shuffle();
        results.ForEach(role => result += $"{PlayerLayer.GetLayers(role)[0]}, ");
        result = result.Remove(result.Length - 2);

        if (CustomGameOptions.PreciseOperativeInfo)
            result = $"Bug {Number}: {result}";

        Results.Clear();
        Players.Clear();
        return result;
    }
}