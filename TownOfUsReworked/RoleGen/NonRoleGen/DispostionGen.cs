using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public class DispositionGen : BaseGen
{
    public static readonly LayerEnum[] LoverRival = [ LayerEnum.Lovers, LayerEnum.Rivals ];
    public static readonly LayerEnum[] CrewDisp = [ LayerEnum.Corrupted, LayerEnum.Fanatic, LayerEnum.Traitor ];
    public static readonly LayerEnum[] NeutralDisp = [ LayerEnum.Taskmaster, LayerEnum.Overlord, LayerEnum.Linked ];

    public override void Clear() => AllDispositions.Clear();

    public override void InitList()
    {
        foreach (var spawn in GetValuesFromToAndMorph(LayerEnum.Allied, LayerEnum.Traitor, GetSpawnItem))
        {
            if (spawn.IsActive())
                AllDispositions.AddMany(spawn.Clone, spawn.Count);
        }

        int maxDisp = DispositionsSettings.MaxDispositions;
        int minDisp = DispositionsSettings.MinDispositions;
        var playerCount = AllPlayers().Count(x => x != Pure);

        while (maxDisp > playerCount)
            maxDisp--;

        while (minDisp > playerCount)
            minDisp--;

        ModeFilters[GameModeSettings.GameMode].Filter(AllDispositions, GameModeSettings.IgnoreLayerCaps ? playerCount : URandom.RandomRangeInt(minDisp, maxDisp + 1), true);
    }

    public override void Assign()
    {
        var playerList = AllPlayers().ToList();
        playerList.Shuffle();
        AllDispositions.Shuffle();
        var invalid = new List<LayerEnum>();

        if (TownOfUsReworked.MCIActive && AllDispositions.Any())
        {
            var ids = "";

            foreach (var spawn in AllDispositions)
                ids += $" {spawn.ID}";

            Message("Dispositions in the game: " + ids.Trim());
        }

        while (playerList.Any() && AllDispositions.Any())
        {
            var id = AllDispositions.TakeFirst().ID;
            PlayerControl assigned = null;

            if (LoverRival.Contains(id) && playerList.Count > 1)
                assigned = playerList.FirstOrDefault(x => x.GetRole() is not (Altruist or Troll or Actor or Jester or Shifter));
            else if (CrewDisp.Contains(id))
                assigned = playerList.FirstOrDefault(x => x.Is(Faction.Crew));
            else if (NeutralDisp.Contains(id))
                assigned = playerList.FirstOrDefault(x => x.Is(Faction.Neutral));
            else if (id == LayerEnum.Allied)
                assigned = playerList.FirstOrDefault(x => x.Is(Alignment.Killing) && x.Is(Faction.Neutral));
            else if (id == LayerEnum.Mafia && playerList.Count > 1)
                assigned = playerList.FirstOrDefault();
            else if (id == LayerEnum.Defector && playerList.Count > 1)
                assigned = playerList.FirstOrDefault(x => x.Is(Faction.Intruder) || x.Is(Faction.Syndicate));

            if (assigned)
            {
                playerList.Remove(assigned);
                playerList.Shuffle();
                AllDispositions.Shuffle();

                if (!assigned.GetDisposition())
                    Gen(assigned, id, PlayerLayerEnum.Disposition);
                else
                    invalid.Add(id);
            }
        }

        if (TownOfUsReworked.MCIActive && invalid.Any())
        {
            var ids = "";

            foreach (var spawn in invalid)
                ids += $" {spawn}";

            Message("Invalid Dispositions in the game: " + ids.Trim());
        }

        AllDispositions.Clear();
    }
}