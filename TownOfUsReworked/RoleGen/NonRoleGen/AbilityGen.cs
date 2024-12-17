using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen2;

public class AbilityGen : BaseGen
{
    public override void Clear() => AllAbilities.Clear();

    public override void InitList()
    {
        foreach (var spawn in GetValuesFromToAndMorph(LayerEnum.Bullseye, LayerEnum.Underdog, GetSpawnItem))
        {
            if (spawn.IsActive())
            {
                for (var j = 0; j < spawn.Count; j++)
                    AllAbilities.Add(spawn);
            }
        }

        int maxAb = AbilitiesSettings.MaxAbilities;
        int minAb = AbilitiesSettings.MinAbilities;
        var players = GameData.Instance.PlayerCount;

        while (maxAb > players)
            maxAb--;

        while (minAb > players)
            minAb--;

        ModeFilters[GameModeSettings.GameMode].Filter(AllAbilities, GameModeSettings.IgnoreLayerCaps ? players : URandom.RandomRangeInt(minAb, maxAb + 1));
    }

    public override void Assign()
    {
        var playerList = AllPlayers().ToList();
        playerList.Shuffle();
        AllAbilities.Shuffle();
        var invalid = new List<LayerEnum>();

        if (TownOfUsReworked.IsTest)
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
                assigned = playerList.FirstOrDefault(x => x.Is(Faction.Crew) && !x.Is(LayerEnum.Traitor) && !x.Is(LayerEnum.Fanatic));
            else if (id == LayerEnum.Sniper)
                assigned = playerList.FirstOrDefault(x => x.Is(Faction.Syndicate));
            else if (CrewAb.Contains(id))
                assigned = playerList.FirstOrDefault(x => x.Is(Faction.Crew));
            else if (id == LayerEnum.Slayer)
                assigned = playerList.FirstOrDefault(x => x.Is(Alignment.NeutralNeo) || x.Is(Alignment.NeutralKill) || x.Is(Alignment.NeutralHarb));
            else if (id == LayerEnum.Hitman)
                assigned = playerList.FirstOrDefault(x => x.Is(Faction.Intruder) && !(x.Is(LayerEnum.Consigliere) && Consigliere.ConsigInfo == ConsigInfo.Role));
            else if (id == LayerEnum.Ninja)
            {
                assigned = playerList.FirstOrDefault(x => x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) || x.Is(Alignment.NeutralNeo) || x.Is(Alignment.NeutralKill) ||
                    x.Is(Alignment.CrewKill) || x.Is(LayerEnum.Corrupted));
            }
            else if (id == LayerEnum.Torch)
            {
                assigned = playerList.FirstOrDefault(x => !((x.Is(Alignment.NeutralKill) && !NeutralKillingSettings.NKHasImpVision) || x.Is(Faction.Syndicate) || x.Is(Faction.Intruder) ||
                    (x.Is(Faction.Neutral) && !NeutralSettings.LightsAffectNeutrals) || (x.Is(Alignment.NeutralNeo) && !NeutralNeophyteSettings.NNHasImpVision) ||
                    (x.Is(Alignment.NeutralEvil) && !NeutralEvilSettings.NEHasImpVision) || (x.Is(Alignment.NeutralHarb) && !NeutralHarbingerSettings.NHHasImpVision)));
            }
            else if (id == LayerEnum.Underdog)
                assigned = playerList.FirstOrDefault(x => x.Is(Faction.Intruder) || x.Is(Faction.Syndicate));
            else if (Tasked.Contains(id))
                assigned = playerList.FirstOrDefault(x => x.CanDoTasks());
            else if (id == LayerEnum.Tunneler)
                assigned = playerList.FirstOrDefault(x => x.Is(Faction.Crew) && !x.Is(LayerEnum.Engineer));
            else if (id == LayerEnum.ButtonBarry)
            {
                assigned = playerList.FirstOrDefault(x => !((x.Is(LayerEnum.Mayor) && !Mayor.MayorButton) || (x.Is(LayerEnum.Jester) && !Jester.JesterButton) || (x.Is(LayerEnum.Actor) &&
                    !Actor.ActorButton) || (x.Is(LayerEnum.Guesser) && !Guesser.GuesserButton) || (x.Is(LayerEnum.Executioner) && !Executioner.ExecutionerButton) || (!Monarch.MonarchButton &&
                    x.Is(LayerEnum.Monarch)) || (!Dictator.DictatorButton && x.Is(LayerEnum.Dictator))));
            }
            else if (id == LayerEnum.Politician)
                assigned = playerList.FirstOrDefault(x => !(x.Is(Alignment.NeutralEvil) || x.Is(Alignment.NeutralBen) || x.Is(Alignment.NeutralNeo)));
            else if (GlobalAb.Contains(id))
                assigned = playerList.FirstOrDefault();
            else if (id == LayerEnum.Ruthless)
            {
                assigned = playerList.FirstOrDefault(x => x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) || x.Is(Alignment.NeutralNeo) || (x.Is(Alignment.NeutralKill) &&
                    !x.Is(LayerEnum.Juggernaut)) || x.Is(Alignment.CrewKill) || x.Is(LayerEnum.Corrupted));
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

        if (TownOfUsReworked.IsTest)
        {
            var ids = "";

            foreach (var spawn in invalid)
                ids += $" {spawn}";

            Message("Invalid Abilities in the game: " + ids.Trim());
        }

        AllAbilities.Clear();
    }
}