using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen2;

public abstract class BaseRoleGen : BaseGen
{
    public int Intruders { get; set; }
    public int Crew { get; set; }
    public int Syndicate { get; set; }
    public int Neutrals { get; set; }

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

        if (TownOfUsReworked.IsTest)
        {
            var ids = "";

            foreach (var spawn in AllRoles)
                ids += $" {spawn.ID}";

            Message("Roles in the game: " + ids.Trim());
        }

        while (players.Any() && AllRoles.Any())
            Gen(players.TakeFirst(), AllRoles.TakeFirst().ID, PlayerLayerEnum.Role);

        AllRoles.Clear();
    }

    public virtual void InitCrewList() {}

    public virtual void InitNeutList() {}

    public virtual void InitIntList() {}

    public virtual void InitSynList() {}

    public virtual void GetAdjustedFactions() {}
}