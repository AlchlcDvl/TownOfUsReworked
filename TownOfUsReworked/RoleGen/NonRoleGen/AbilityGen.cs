using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public class AbilityGen : BaseGen
{
    public static readonly LayerEnum[] CrewAb = [ LayerEnum.Bullseye, LayerEnum.Swapper ];
    public static readonly LayerEnum[] Tasked = [ LayerEnum.Insider, LayerEnum.Multitasker ];
    public static readonly LayerEnum[] GlobalAb = [ LayerEnum.Radar, LayerEnum.Tiebreaker ];

    public override void Clear() => AllAbilities.Clear();

    public override void InitList()
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

        if (TownOfUsReworked.MCIActive && AllAbilities.Any())
        {
            var ids = "";

            foreach (var spawn in AllAbilities)
                ids += $" {spawn.ID}";

            Message("Abilities in the game: " + ids.Trim());
        }

        while (playerList.Any() && AllAbilities.Any())
        {
            var id = AllAbilities.TakeFirst().ID;
            PlayerControl assigned = null;

            if (id == LayerEnum.Snitch)
                assigned = playerList.FirstOrDefault(x => x.Is(Faction.Crew) && !x.Is<Traitor>() && !x.Is<Fanatic>());
            else if (id == LayerEnum.Sniper)
                assigned = playerList.FirstOrDefault(x => x.Is(Faction.Syndicate));
            else if (CrewAb.Contains(id))
                assigned = playerList.FirstOrDefault(x => x.Is(Faction.Crew));
            else if (id == LayerEnum.Slayer)
                assigned = playerList.FirstOrDefault(x => x.Is(Faction.Neutral) && (x.Is(Alignment.Neophyte) || x.Is(Alignment.Killing) || x.Is(Alignment.Harbinger)));
            else if (id == LayerEnum.Hitman)
                assigned = playerList.FirstOrDefault(x => x.Is(Faction.Intruder) && !(x.Is<Consigliere>() && Consigliere.ConsigInfo == ConsigInfo.Role));
            else if (id == LayerEnum.Ninja)
                assigned = playerList.FirstOrDefault(x => x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) || x.Is(Alignment.Neophyte) || x.Is(Alignment.Killing) || x.Is<Corrupted>());
            else if (id == LayerEnum.Torch)
            {
                assigned = playerList.FirstOrDefault(x => !(x.GetFaction() is Faction.Syndicate or Faction.Intruder || (x.Is(Faction.Neutral) && (!NeutralSettings.LightsAffectNeutrals ||
                    (x.Is(Alignment.Killing) && !NeutralKillingSettings.NKHasImpVision) || (x.Is(Alignment.Neophyte) && !NeutralNeophyteSettings.NNHasImpVision) || (x.Is(Alignment.Evil) &&
                    !NeutralEvilSettings.NEHasImpVision) || (x.Is(Alignment.Harbinger) && !NeutralHarbingerSettings.NHHasImpVision)))));
            }
            else if (id == LayerEnum.Underdog)
                assigned = playerList.FirstOrDefault(x => x.Is(Faction.Intruder) || x.Is(Faction.Syndicate));
            else if (Tasked.Contains(id))
                assigned = playerList.FirstOrDefault(x => x.CanDoTasks());
            else if (id == LayerEnum.Tunneler)
                assigned = playerList.FirstOrDefault(x => x.Is(Faction.Crew) && !x.Is<Engineer>());
            else if (id == LayerEnum.ButtonBarry)
            {
                assigned = playerList.FirstOrDefault(x => !((x.Is<Mayor>() && !Mayor.MayorButton) || (x.Is<Jester>() && !Jester.JesterButton) || (x.Is<Actor>() && !Actor.ActorButton) ||
                    (x.Is<Guesser>() && !Guesser.GuesserButton) || (x.Is<Executioner>() && !Executioner.ExecutionerButton) || (!Monarch.MonarchButton && x.Is<Monarch>()) ||
                    (!Dictator.DictatorButton && x.Is<Dictator>())));
            }
            else if (id == LayerEnum.Politician)
                assigned = playerList.FirstOrDefault(x => !(x.Is(Alignment.Evil) || x.Is(Alignment.Benign) || x.Is(Alignment.Neophyte)));
            else if (GlobalAb.Contains(id))
                assigned = playerList.FirstOrDefault();
            else if (id == LayerEnum.Ruthless)
            {
                assigned = playerList.FirstOrDefault(x => x.GetFaction() is Faction.Intruder or Faction.Syndicate || x.Is(Alignment.Neophyte) || x.Is<Corrupted>() ||
                    (x.Is(Faction.Neutral, Alignment.Killing) && !x.Is<Juggernaut>()) || x.Is(Faction.Crew, Alignment.Killing));
            }

            if (assigned)
            {
                playerList.Remove(assigned);
                playerList.Shuffle();
                AllAbilities.Shuffle();

                if (!assigned.GetAbility())
                    Gen(assigned, id, PlayerLayerEnum.Ability);
                else
                    invalid.Add(id);
            }
        }

        if (TownOfUsReworked.MCIActive && invalid.Any())
        {
            var ids = "";

            foreach (var spawn in invalid)
                ids += $" {spawn}";

            Message("Invalid Abilities in the game: " + ids.Trim());
        }

        AllAbilities.Clear();
    }
}