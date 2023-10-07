namespace TownOfUsReworked.PlayerLayers.Abilities;

public abstract class Ability : PlayerLayer
{
    public static readonly List<Ability> AllAbilities = new();
    public static Ability LocalAbility => GetAbility(CustomPlayer.Local);

    public override Color Color => Colors.Ability;
    public override PlayerLayerEnum LayerType => PlayerLayerEnum.Ability;

    public virtual bool Hidden => false;

    protected Ability(PlayerControl player) : base(player)
    {
        if (GetAbility(player))
            GetAbility(player).Player = null;

        AllAbilities.Add(this);
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);
        GetAssassins().ForEach(x => x.AssassinMenu.HideButtons());

        foreach (var swapper in GetAbilities<Swapper>(LayerEnum.Swapper))
        {
            swapper.SwapMenu.HideButtons();
            swapper.Swap1 = null;
            swapper.Swap2 = null;
        }

        foreach (var pol in GetAbilities<Politician>(LayerEnum.Politician))
        {
            pol.DestroyAbstain();
            pol.ExtraVotes.Clear();

            if (pol.VoteBank < 0)
                pol.VoteBank = 0;

            pol.VotedOnce = false;

            if (!pol.CanKill)
                pol.VoteBank++;
        }
    }

    public static Ability GetAbility(PlayerControl player) => AllAbilities.Find(x => x.Player == player);

    public static T GetAbility<T>(PlayerControl player) where T : Ability => GetAbility(player) as T;

    public static Ability GetAbility(PlayerVoteArea area) => GetAbility(PlayerByVoteArea(area));

    public static List<Ability> GetAbilities(LayerEnum abilitytype) => AllAbilities.Where(x => x.Type == abilitytype).ToList();

    public static List<T> GetAbilities<T>(LayerEnum abilitytype) where T : Ability => GetAbilities(abilitytype).Cast<T>().ToList();

    public static List<Assassin> GetAssassins() => AllAbilities.Where(x => x.Type is LayerEnum.CrewAssassin or LayerEnum.NeutralAssassin or LayerEnum.IntruderAssassin or
        LayerEnum.SyndicateAssassin).Cast<Assassin>().ToList();
}