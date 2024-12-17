using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen2;

public class DispositionGen : BaseGen
{
    public override void Clear() => AllDispositions.Clear();

    public override void InitList()
    {
        foreach (var spawn in GetValuesFromToAndMorph(LayerEnum.Allied, LayerEnum.Traitor, GetSpawnItem))
        {
            if (spawn.IsActive())
            {
                for (var j = 0; j < spawn.Count; j++)
                    AllDispositions.Add(spawn);
            }
        }

        int maxDisp = DispositionsSettings.MaxDispositions;
        int minDisp = DispositionsSettings.MinDispositions;
        var playerCount = AllPlayers().Count(x => x != Pure);

        while (maxDisp > playerCount)
            maxDisp--;

        while (minDisp > playerCount)
            minDisp--;

        var linked = AllDispositions.Count(x => x.ID == LayerEnum.Linked);

        for (var i = 0; i < linked; i++)
            AllDispositions.Add(GetSpawnItem(LayerEnum.Linked));

        var lovers = AllDispositions.Count(x => x.ID == LayerEnum.Lovers);

        for (var i = 0; i < lovers; i++)
            AllDispositions.Add(GetSpawnItem(LayerEnum.Lovers));

        var rivals = AllDispositions.Count(x => x.ID == LayerEnum.Rivals);

        for (var i = 0; i < rivals; i++)
            AllDispositions.Add(GetSpawnItem(LayerEnum.Rivals));

        ModeFilters[GameModeSettings.GameMode].Filter(AllDispositions, GameModeSettings.IgnoreLayerCaps ? playerCount : URandom.RandomRangeInt(minDisp, maxDisp + 1), true);
    }

    public override void Assign()
    {
        var playerList = AllPlayers().ToList();
        playerList.Shuffle();
        AllDispositions.Shuffle();
        var invalid = new List<LayerEnum>();

        if (TownOfUsReworked.IsTest)
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
                assigned = playerList.FirstOrDefault(x => x.GetRole().Type is not (LayerEnum.Altruist or LayerEnum.Troll or LayerEnum.Actor or LayerEnum.Jester or LayerEnum.Shifter));
            else if (CrewDisp.Contains(id))
                assigned = playerList.FirstOrDefault(x => x.Is(Faction.Crew));
            else if (NeutralDisp.Contains(id))
                assigned = playerList.FirstOrDefault(x => x.Is(Faction.Neutral));
            else if (id == LayerEnum.Allied)
                assigned = playerList.FirstOrDefault(x => x.Is(Alignment.NeutralKill));
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

        if (TownOfUsReworked.IsTest)
        {
            var ids = "";

            foreach (var spawn in invalid)
                ids += $" {spawn}";

            Message("Invalid Dispositions in the game: " + ids.Trim());
        }

        AllDispositions.Clear();
    }
}