namespace TownOfUsReworked.PlayerLayers.Abilities;

public abstract class Ability : PlayerLayer
{
    public static readonly List<Ability> AllAbilities = new();
    public static Ability LocalAbility => CustomPlayer.Local.GetAbility();

    public override UColor Color => CustomColorManager.Ability;
    public override PlayerLayerEnum LayerType => PlayerLayerEnum.Ability;
    public override LayerEnum Type => LayerEnum.NoneAbility;

    protected Ability() : base() => AllAbilities.Add(this);

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);
        GetAssassins().ForEach(x => x.AssassinMenu.HideButtons());

        foreach (var swapper in GetLayers<Swapper>())
        {
            swapper.SwapMenu.HideButtons();
            swapper.Swap1 = null;
            swapper.Swap2 = null;
        }

        foreach (var pol in GetLayers<Politician>())
        {
            pol.DestroyAbstain();

            if (pol.VoteBank < 0)
                pol.VoteBank = 0;

            pol.VotedOnce = false;

            if (!pol.CanKill)
                pol.VoteBank++;
        }
    }

    public static List<Assassin> GetAssassins() => AllAbilities.Where(x => x.Type is LayerEnum.CrewAssassin or LayerEnum.NeutralAssassin or LayerEnum.IntruderAssassin or
        LayerEnum.SyndicateAssassin).Cast<Assassin>().ToList();
}