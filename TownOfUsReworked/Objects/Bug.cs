namespace TownOfUsReworked.Objects;

public class Bug : Range
{
    private Dictionary<byte, float> Players { get; }
    private List<PlayerControl> Closest { get; set; }
    private Dictionary<byte, LayerEnum> Results { get; }

    public Bug(PlayerControl owner) : base(owner, CustomColorManager.Operative, CustomGameOptions.BugRange, "Bug")
    {
        Results = new();
        Closest = new();
        Players = new();
    }

    public override void Update()
    {
        base.Update();

        if (!Transform)
            return;

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

            if (Players[player.PlayerId] >= CustomGameOptions.MinAmountOfTimeInBug && player != Owner && !Results.ContainsKey(player.PlayerId))
            {
                var type = player.GetRole().Type;
                Results[player.PlayerId] = type;

                if (Owner.Is(LayerEnum.Operative))
                    Owner.GetRole<Operative>().BuggedPlayers.Add(type);
                else if (Owner.Is(LayerEnum.Retributionist))
                    Owner.GetRole<Retributionist>().BuggedPlayers.Add(type);
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