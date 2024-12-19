namespace TownOfUsReworked.PlayerLayers.Abilities;

public abstract class Ability : PlayerLayer
{
    public override UColor Color => CustomColorManager.Ability;
    public override PlayerLayerEnum LayerType => PlayerLayerEnum.Ability;
    public override LayerEnum Type => LayerEnum.NoneAbility;

    public override void OnMeetingStart(MeetingHud __instance)
    {
        foreach (var swapper in GetLayers<Swapper>())
        {
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

    public static IEnumerable<Assassin> GetAssassins() => GetLayers<Assassin>().Where(x => x is Bullseye or Slayer or Hitman or Sniper);
}