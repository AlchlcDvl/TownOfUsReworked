using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public abstract class BaseRoleGen : BaseGen
{
    protected int Intruders { get; set; }
    protected int Crew { get; set; }
    protected int Syndicate { get; set; }
    protected int Neutrals { get; set; }

    public virtual bool AllowNonRoles => true;
    public virtual bool HasTargets => true;

    public override void Clear()
    {
        Intruders = 0;
        Crew = 0;
        Syndicate = 0;
        Neutrals = 0;

        AllRoles.Clear();
    }

    public override void Assign()
    {
        var players = AllPlayers().ToList();
        AllRoles.Shuffle();
        players.Shuffle();

        if (TownOfUsReworked.MciActive && AllRoles.Any())
            Message("Roles in the game: " + string.Join(" ", AllRoles.Select(ab => ab.ID)));

        while (players.Any() && AllRoles.Any())
            Gen(players.TakeFirst(), AllRoles.TakeFirst().ID, PlayerLayerEnum.Role);

        AllRoles.Clear();
    }

    public virtual void InitCrewList() {}

    public virtual void InitNeutList() {}

    public virtual void InitIntList() {}

    public virtual void InitSynList() {}

    protected virtual void GetAdjustedFactions()
    {
        var players = GameData.Instance.PlayerCount;
        Intruders = IntruderSettings.IntruderCount;
        Syndicate = SyndicateSettings.SyndicateCount;
        Neutrals = URandom.RandomRangeInt(NeutralSettings.NeutralMin, NeutralSettings.NeutralMax + 1);

        if (Intruders == 0 && Syndicate == 0 && Neutrals == 0)
        {
            _ = URandom.RandomRangeInt(0, 3) switch
            {
                0 => Intruders++,
                1 => Syndicate++,
                _ => Neutrals++,
            };
        }

        while (Intruders + Syndicate >= players)
        {
            if (Intruders > Syndicate)
                Intruders--;
            else
                Syndicate--;
        }

        while (Neutrals >= players - Intruders - Syndicate)
            Neutrals--;

        Crew = players - Intruders - Syndicate - Neutrals;
    }
}