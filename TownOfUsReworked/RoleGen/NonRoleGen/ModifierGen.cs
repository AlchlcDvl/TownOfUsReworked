using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen2;

public class ModifierGen : BaseGen
{
    public override void Clear() => AllModifiers.Clear();

    public override void InitList()
    {
        foreach (var spawn in GetValuesFromToAndMorph(LayerEnum.Astral, LayerEnum.Yeller, GetSpawnItem))
        {
            if (spawn.IsActive())
            {
                for (var j = 0; j < spawn.Count; j++)
                    AllModifiers.Add(spawn);
            }
        }

        int maxMod = ModifiersSettings.MaxModifiers;
        int minMod = ModifiersSettings.MinModifiers;
        var players = GameData.Instance.PlayerCount;

        while (maxMod > players)
            maxMod--;

        while (minMod > players)
            minMod--;

        ModeFilters[GameModeSettings.GameMode].Filter(AllModifiers, GameModeSettings.IgnoreLayerCaps ? players : URandom.RandomRangeInt(minMod, maxMod + 1), true);
    }

    public override void Assign()
    {
        var playerList = AllPlayers().ToList();
        playerList.Shuffle();
        AllModifiers.Shuffle();
        var invalid = new List<LayerEnum>();

        if (TownOfUsReworked.IsTest)
        {
            var ids = "";

            foreach (var spawn in AllModifiers)
                ids += $" {spawn.ID}";

            Message("Modifiers in the game: " + ids.Trim());
        }

        while (playerList.Any() && AllModifiers.Any())
        {
            var id = AllModifiers.TakeFirst().ID;
            PlayerControl assigned = null;

            if (id == LayerEnum.Bait)
                assigned = playerList.FirstOrDefault(x => !(x.Is(LayerEnum.Vigilante) || x.Is(LayerEnum.Shifter) || x.Is(LayerEnum.Thief) || x.Is(LayerEnum.Altruist) || x.Is(LayerEnum.Troll)));
            else if (id == LayerEnum.Diseased)
                assigned = playerList.FirstOrDefault(x => !(x.Is(LayerEnum.Altruist) || x.Is(LayerEnum.Troll)));
            else if (id == LayerEnum.Professional)
                assigned = playerList.FirstOrDefault(x => x.Is(LayerEnum.Bullseye) || x.Is(LayerEnum.Slayer) || x.Is(LayerEnum.Hitman) || x.Is(LayerEnum.Sniper));
            else if (GlobalMod.Contains(id))
                assigned = playerList.FirstOrDefault();
            else if (id == LayerEnum.Shy)
            {
                assigned = playerList.FirstOrDefault(x => !((x.Is(LayerEnum.Mayor) && !Mayor.MayorButton) || (x.Is(LayerEnum.Jester) && !Jester.JesterButton) || (x.Is(LayerEnum.Swapper) &&
                    !Swapper.SwapperButton) || (x.Is(LayerEnum.Actor) && !Actor.ActorButton) || (x.Is(LayerEnum.Guesser) && !Guesser.GuesserButton) || (x.Is(LayerEnum.Executioner) &&
                    !Executioner.ExecutionerButton) || (x.Is(LayerEnum.Politician) && !Politician.PoliticianButton) ||  x.Is(LayerEnum.ButtonBarry) || (!Dictator.DictatorButton &&
                    x.Is(LayerEnum.Dictator)) || (!Monarch.MonarchButton && x.Is(LayerEnum.Monarch))));
            }

            if (assigned)
            {
                playerList.Remove(assigned);
                playerList.Shuffle();
                AllModifiers.Shuffle();

                if (!assigned.GetModifier())
                    Gen(assigned, id, PlayerLayerEnum.Modifier);
                else
                    invalid.Add(id);
            }
        }

        if (TownOfUsReworked.IsTest)
        {
            var ids = "";

            foreach (var spawn in invalid)
                ids += $" {spawn}";

            Message("Invalid Modifiers in the game: " + ids.Trim());
        }

        AllModifiers.Clear();
    }
}