using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public sealed class AbilityGen : BaseGen
{
    private static readonly Layer[] CrewAb = [ Layer.Bullseye, Layer.Swapper ];
    private static readonly Layer[] Tasked = [ Layer.Insider, Layer.Multitasker ];
    private static readonly Layer[] GlobalAb = [ Layer.Radar, Layer.Tiebreaker ];

    public override void InitList()
    {
        if (IsList())
            InitRlList();
        else
            InitRegList();
    }

    private static void InitRlList()
    {
        var abilities = GetValuesFromTo(Layer.Bullseye, Layer.Underdog);

        foreach (var entry in Option.GetOptions<ListEntryOption>().Where(x => !x.IsBan && x.EntryType == PlayerLayerEnum.Ability && x.Num <= GameData.Instance.PlayerCount))
        {
            var rateLimit = 0;
            var cachedCount = AllAbilities.Count;
            var bucket = entry.Value.Select(x => x.TryCastToLayer(out var layer) ? layer : abilities.Random());

            while (rateLimit < 10000 && AllAbilities.Count == cachedCount)
            {
                var layer2 = bucket.Random();

                if (ListGen.CannotAdd(layer2, AllAbilities))
                    rateLimit++;
                else
                    AllAbilities.Add(GetSpawnItem(layer2));
            }
        }
    }

    private static void InitRegList()
    {
        foreach (var spawn in GetValuesFromToAndMorph(Layer.Bullseye, Layer.Underdog, GetSpawnItem))
        {
            if (spawn.IsActive())
                AllAbilities.AddMany(spawn.Clone, spawn.Count);
        }

        var players = GameData.Instance.PlayerCount;
        var maxAb = Mathf.Clamp(AbilitiesSettings.MaxAbilities.Value, 0, players);
        var minAb = Mathf.Clamp(AbilitiesSettings.MinAbilities.Value, 0, players);
        ModeFilters[GameModeSettings.GameMode].Filter(AllAbilities, GameModeSettings.IgnoreLayerCaps ? players : URandom.RandomRangeInt(minAb, maxAb + 1), true);
    }

    public override void Assign()
    {
        var playerList = AllPlayers().ToList();
        playerList.Shuffle();
        AllAbilities.Shuffle();
        var invalid = new List<Layer>();

        if (TownOfUsReworked.MciActive && AllAbilities.Any())
            Message("Abilities in the game: " + Join(" ", AllAbilities.Select(ab => ab.ID)));

        while (playerList.Any() && AllAbilities.Any())
        {
            var id = AllAbilities.TakeFirst().ID;
            var assigned = id switch
            {
                Layer.Snitch => playerList.FirstOrDefault(x => x.Is(Faction.Crew) && !x.Is<Traitor>() && !x.Is<Fanatic>()),
                Layer.Sniper => playerList.FirstOrDefault(x => x.Is(Faction.Syndicate)),
                Layer.Ritualist => playerList.FirstOrDefault(x => x.Is(Faction.Apocalypse)),
                Layer.Slayer => playerList.FirstOrDefault(x => x.GetFaction().IsFactionedEvil()),
                Layer.Hitman => playerList.FirstOrDefault(x => x.Is(Faction.Intruder) && (!x.Is<Consigliere>() || Consigliere.ConsigInfo != ConsigInfo.Role)),
                Layer.Ninja => playerList.FirstOrDefault(x => x.GetFaction().IsFactionedEvil() || x.Is<Corrupted>()),
                Layer.Torch => playerList.FirstOrDefault(x => x.GetRole().AffectedByLights),
                Layer.Underdog => playerList.FirstOrDefault(x => x.GetFaction().IsFactionedEvil(true)),
                Layer.Tunneler => playerList.FirstOrDefault(x => x.Is(Faction.Crew)),
                Layer.ButtonBarry => playerList.FirstOrDefault(x => !((x.Is<Democrat>() && (!Mayor.MayorButton || !Democrat.DemocratButton)) || (x.Is<Jester>() && !Jester.JesterButton) ||
                    (x.Is<Actor>() && !Actor.ActorButton) || (x.Is<Guesser>() && !Guesser.GuesserButton) || (x.Is<Executioner>() && !Executioner.ExecutionerButton) || (!Monarch.MonarchButton &&
                    x.Is<Monarch>()) || (x.Is<Dictator>() && !Dictator.DictatorButton) || (x.Is<Mayor>() && !Mayor.MayorButton))),
                Layer.Politician => playerList.FirstOrDefault(x => !(x.Is(Alignment.Evil) || x.Is(Alignment.Benign) || x.Is(Alignment.Neophyte))),
                Layer.Ruthless => playerList.FirstOrDefault(x => (x.GetFaction().IsFactionedEvil() && !x.Is<Juggernaut>()) || x.Is<Corrupted>() || x.Is<CKilling>()),
                _ when GlobalAb.Contains(id) => playerList.FirstOrDefault(),
                _ when Tasked.Contains(id) => playerList.FirstOrDefault(x => x.CanDoTasks()),
                _ when CrewAb.Contains(id) => playerList.FirstOrDefault(x => x.Is(Faction.Crew)),
                _ => null
            };

            if (!assigned)
                continue;

            playerList.Remove(assigned);
            playerList.Shuffle();
            AllAbilities.Shuffle();

            if (!assigned.GetAbility())
                Gen(assigned, id, PlayerLayerEnum.Ability);
            else
                invalid.Add(id);
        }

        if (TownOfUsReworked.MciActive && invalid.Any())
            Message("Invalid Abilities in the game: " + Join(" ", invalid));

        AllAbilities.Clear();
    }
}