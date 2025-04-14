using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public sealed class AbilityGen : BaseGen
{
    private static readonly LayerEnum[] CrewAb = [ LayerEnum.Bullseye, LayerEnum.Swapper ];
    private static readonly LayerEnum[] Tasked = [ LayerEnum.Insider, LayerEnum.Multitasker ];
    private static readonly LayerEnum[] GlobalAb = [ LayerEnum.Radar, LayerEnum.Tiebreaker ];

    public override void Clear() => AllAbilities.Clear();

    public override void InitList()
    {
        if (IsList())
            InitRlList();
        else
            InitRegList();
    }

    private static void InitRlList()
    {
        var abilities = GetValuesFromTo(LayerEnum.Bullseye, LayerEnum.Underdog);

        foreach (var entry in Option.GetOptions<ListEntryOption>().Where(x => !x.IsBan && x.EntryType == PlayerLayerEnum.Ability))
        {
            foreach (var id in entry.Value)
            {
                if (id == ListSlot.None)
                    break;

                var rateLimit = 0;
                var cachedCount = AllAbilities.Count;

                while (rateLimit < 10000 && AllAbilities.Count == cachedCount)
                {
                    if (!id.TryCastToLayer(out var layer))
                        layer = abilities.Random();

                    if (ListGen.CannotAdd(layer, AllAbilities))
                        rateLimit++;
                    else
                        AllAbilities.Add(GetSpawnItem(layer));
                }
            }
        }
    }

    private static void InitRegList()
    {
        foreach (var spawn in GetValuesFromToAndMorph(LayerEnum.Bullseye, LayerEnum.Underdog, GetSpawnItem))
        {
            if (spawn.IsActive())
                AllAbilities.AddMany(spawn.Clone, spawn.Count);
        }

        int maxAb = AbilitiesSettings.MaxAbilities;
        int minAb = AbilitiesSettings.MinAbilities;
        var players = GameData.Instance.PlayerCount;

        while (maxAb > players)
            maxAb--;

        while (minAb > players)
            minAb--;

        ModeFilters[GameModeSettings.GameMode].Filter(AllAbilities, GameModeSettings.IgnoreLayerCaps ? players : URandom.RandomRangeInt(minAb, maxAb + 1), true);
    }

    public override void Assign()
    {
        var playerList = AllPlayers().ToList();
        playerList.Shuffle();
        AllAbilities.Shuffle();
        var invalid = new List<LayerEnum>();

        if (TownOfUsReworked.MciActive && AllAbilities.Any())
            Message("Abilities in the game: " + Join(" ", AllAbilities.Select(ab => ab.ID)));

        while (playerList.Any() && AllAbilities.Any())
        {
            var id = AllAbilities.TakeFirst().ID;
            var assigned = id switch
            {
                LayerEnum.Snitch => playerList.FirstOrDefault(x => x.Is(Faction.Crew) && !x.Is<Traitor>() && !x.Is<Fanatic>()),
                LayerEnum.Sniper => playerList.FirstOrDefault(x => x.Is(Faction.Syndicate)),
                LayerEnum.Ritualist => playerList.FirstOrDefault(x => x.Is(Faction.Apocalypse)),
                LayerEnum.Slayer => playerList.FirstOrDefault(x => x.Is(Faction.Neutral) && (x.Is(Alignment.Neophyte) || x.Is(Alignment.Killing))),
                LayerEnum.Hitman => playerList.FirstOrDefault(x => x.Is(Faction.Intruder) && (!x.Is<Consigliere>() || Consigliere.ConsigInfo != ConsigInfo.Role)),
                LayerEnum.Ninja => playerList.FirstOrDefault(x => x.GetFaction() is Faction.Intruder or Faction.Syndicate or Faction.Apocalypse || x.Is(Alignment.Neophyte) ||
                    x.Is(Alignment.Killing) || x.Is<Corrupted>()),
                LayerEnum.Torch => playerList.FirstOrDefault(x => x.GetRole().AffectedByLights),
                LayerEnum.Underdog => playerList.FirstOrDefault(x => x.GetFaction() is Faction.Intruder or Faction.Syndicate or Faction.Illuminati or Faction.Pandorica or Faction.Compliance or
                    Faction.Apocalypse),
                LayerEnum.Tunneler => playerList.FirstOrDefault(x => x.Is(Faction.Crew)),
                LayerEnum.ButtonBarry => playerList.FirstOrDefault(x => !((x.Is<Democrat>() && (!Mayor.MayorButton || !Democrat.DemocratButton)) || (x.Is<Jester>() && !Jester.JesterButton) ||
                    (x.Is<Actor>() && !Actor.ActorButton) || (x.Is<Guesser>() && !Guesser.GuesserButton) || (x.Is<Executioner>() && !Executioner.ExecutionerButton) || (!Monarch.MonarchButton &&
                    x.Is<Monarch>()) || (x.Is<Dictator>() && !Dictator.DictatorButton) || (x.Is<Mayor>() && !Mayor.MayorButton))),
                LayerEnum.Politician => playerList.FirstOrDefault(x => !(x.Is(Alignment.Evil) || x.Is(Alignment.Benign) || x.Is(Alignment.Neophyte))),
                LayerEnum.Ruthless => playerList.FirstOrDefault(x => x.GetFaction() is Faction.Intruder or Faction.Syndicate or Faction.Apocalypse || x.Is(Alignment.Neophyte) ||
                    x.Is<Corrupted>() || (x.Is(Faction.Neutral, Alignment.Killing) && !x.Is<Juggernaut>()) || x.Is(Faction.Crew, Alignment.Killing)),
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