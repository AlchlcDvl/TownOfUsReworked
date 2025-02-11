using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public class ModifierGen : BaseGen
{
    private static readonly LayerEnum[] GlobalMod = [ LayerEnum.Dwarf, LayerEnum.Vip, LayerEnum.Giant, LayerEnum.Drunk, LayerEnum.Coward, LayerEnum.Volatile, LayerEnum.Astral,
        LayerEnum.Indomitable, LayerEnum.Yeller, LayerEnum.Colorblind ];

    public override void Clear() => AllModifiers.Clear();

    public override void InitList()
    {
        foreach (var spawn in GetValuesFromToAndMorph(LayerEnum.Astral, LayerEnum.Yeller, GetSpawnItem))
        {
            if (spawn.IsActive())
                AllModifiers.AddMany(spawn.Clone, spawn.Count);
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

        if (TownOfUsReworked.MciActive && AllModifiers.Any())
            Message("Modifiers in the game: " + string.Join(" ", AllModifiers.Select(ab => ab.ID)));

        while (playerList.Any() && AllModifiers.Any())
        {
            var id = AllModifiers.TakeFirst().ID;
            PlayerControl assigned = null;

            if (id == LayerEnum.Bait)
                assigned = playerList.FirstOrDefault(x => !(x.Is<Vigilante>() || x.Is<Shifter>() || x.Is<Thief>() || x.Is<Altruist>() || x.Is<Troll>()));
            else if (id == LayerEnum.Diseased)
                assigned = playerList.FirstOrDefault(x => !(x.Is<Altruist>() || x.Is<Troll>()));
            else if (id == LayerEnum.Professional)
                assigned = playerList.FirstOrDefault(x => x.Is<Assassin>());
            else if (GlobalMod.Contains(id))
                assigned = playerList.FirstOrDefault();
            else if (id == LayerEnum.Shy)
            {
                assigned = playerList.FirstOrDefault(x => !((x.Is<Mayor>() && !Mayor.MayorButton) || (x.Is<Jester>() && !Jester.JesterButton) || (x.Is<Swapper>() && !Swapper.SwapperButton) ||
                    (x.Is<Actor>() && !Actor.ActorButton) || (x.Is<Guesser>() && !Guesser.GuesserButton) || (x.Is<Executioner>() && !Executioner.ExecutionerButton) || (x.Is<Politician>() &&
                    !Politician.PoliticianButton) ||  x.Is<ButtonBarry>() || (!Dictator.DictatorButton && x.Is<Dictator>()) || (!Monarch.MonarchButton && x.Is<Monarch>())));
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

        if (TownOfUsReworked.MciActive && invalid.Any())
            Message("Invalid Modifiers in the game: " + string.Join(" ", invalid));

        AllModifiers.Clear();
    }
}