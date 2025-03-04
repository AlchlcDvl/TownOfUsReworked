using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public sealed class ModifierGen : BaseGen
{
    private static readonly LayerEnum[] GlobalMod = [ LayerEnum.Dwarf, LayerEnum.Vip, LayerEnum.Giant, LayerEnum.Drunk, LayerEnum.Coward, LayerEnum.Volatile, LayerEnum.Astral,
        LayerEnum.Indomitable, LayerEnum.Yeller, LayerEnum.Colorblind ];

    public override void Clear() => AllModifiers.Clear();

    public override void InitList()
    {
        if (IsRoleList())
            InitRlList();
        else
            InitRegList();
    }

    private static void InitRlList()
    {
        var abilities = GetValuesFromTo(LayerEnum.Astral, LayerEnum.Yeller);

        foreach (var entry in OptionAttribute.GetOptions<ListEntryAttribute>().Where(x => !x.IsBan && x.EntryType == PlayerLayerEnum.Modifier))
        {
            foreach (var id in entry.Get())
            {
                if (id == RoleListSlot.None)
                    break;

                var rateLimit = 0;
                var cachedCount = AllModifiers.Count;

                while (rateLimit < 10000 && AllModifiers.Count == cachedCount)
                {
                    if (!id.TryCastToLayer(out var layer))
                        layer = abilities.Random();

                    if (RoleListGen.CannotAdd(layer, AllModifiers))
                        rateLimit++;
                    else
                        AllModifiers.Add(GetSpawnItem(layer));
                }
            }
        }
    }

    private static void InitRegList()
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
            Message("Modifiers in the game: " + Join(" ", AllModifiers.Select(ab => ab.ID)));

        while (playerList.Any() && AllModifiers.Any())
        {
            var id = AllModifiers.TakeFirst().ID;
            var assigned = id switch
            {
                LayerEnum.Bait => playerList.FirstOrDefault(x => !(x.Is<Vigilante>() || x.Is<Shifter>() || x.Is<Thief>() || x.Is<Altruist>() || x.Is<Troll>())),
                LayerEnum.Diseased => playerList.FirstOrDefault(x => !(x.Is<Altruist>() || x.Is<Troll>())),
                LayerEnum.Professional => playerList.FirstOrDefault(x => x.Is<Assassin>()),
                LayerEnum.Shy => playerList.FirstOrDefault(x => !((x.Is<Democrat>() && (!Mayor.MayorButton || !Democrat.DemocratButton)) || (x.Is<Jester>() && !Jester.JesterButton) ||
                    (x.Is<Swapper>() && !Swapper.SwapperButton) || (x.Is<Actor>() && !Actor.ActorButton) || (x.Is<Guesser>() && !Guesser.GuesserButton) || (x.Is<Executioner>() &&
                    !Executioner.ExecutionerButton) || (x.Is<Politician>() && !Politician.PoliticianButton) ||  x.Is<ButtonBarry>() || (!Dictator.DictatorButton && x.Is<Dictator>()) ||
                    (!Monarch.MonarchButton && x.Is<Monarch>()) || (x.Is<Mayor>() && !Mayor.MayorButton))),
                _ => GlobalMod.Contains(id) ? playerList.FirstOrDefault() : null
            };

            if (!assigned)
                continue;

            playerList.Remove(assigned);
            playerList.Shuffle();
            AllModifiers.Shuffle();

            if (!assigned.GetModifier())
                Gen(assigned, id, PlayerLayerEnum.Modifier);
            else
                invalid.Add(id);
        }

        if (TownOfUsReworked.MciActive && invalid.Any())
            Message("Invalid Modifiers in the game: " + Join(" ", invalid));

        AllModifiers.Clear();
    }
}