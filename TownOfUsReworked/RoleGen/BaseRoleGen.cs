using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public abstract class BaseRoleGen : BaseGen
{
    protected int Intruders { get; set; }
    protected int Crew { get; set; }
    protected int Syndicate { get; set; }
    protected int Apocalypse { get; set; }
    protected int Outcasts { get; set; }

    public virtual bool AllowNonRoles => true;
    public virtual bool HasTargets => true;

    public override void Clear()
    {
        Intruders = 0;
        Crew = 0;
        Syndicate = 0;
        Apocalypse = 0;
        Outcasts = 0;
    }

    public override void Assign()
    {
        var players = AllPlayers().ToList();
        AllRoles.Shuffle();
        players.Shuffle();

        if (TownOfUsReworked.MciActive && AllRoles.Any())
            Message("Roles in the game: " + Join(" ", AllRoles.Select(ab => ab.ID)));

        while (players.Any() && AllRoles.Any())
            Gen(players.TakeFirst(), AllRoles.TakeFirst().ID, PlayerLayerEnum.Role);

        AllRoles.Clear();
    }

    public virtual void InitCrewList() {}

    public virtual void InitNeutList() {}

    public virtual void InitIntList() {}

    public virtual void InitSynList() {}

    public virtual void InitApocList() {}

    public virtual void PreFilter() {}

    protected virtual void GetAdjustedFactions()
    {
        var players = GameData.Instance.PlayerCount;
        Intruders = IntruderSettings.IntruderCount;
        Syndicate = SyndicateSettings.SyndicateCount;
        Apocalypse = ApocalypseSettings.ApocalypseCount;
        Outcasts = URandom.RandomRangeInt(OutcastSettings.OutcastMin, OutcastSettings.OutcastMax + 1);

        if (Intruders == 0 && Syndicate == 0 && Outcasts == 0 && Apocalypse == 0)
        {
            _ = URandom.RandomRangeInt(0, 4) switch
            {
                0 => Intruders++,
                1 => Syndicate++,
                2 => Apocalypse++,
                _ => Outcasts++,
            };
        }

        while (Intruders + Syndicate + Apocalypse > players)
        {
            var max = Mathf.Max(Intruders, Syndicate, Apocalypse);

            if (Intruders == max)
                Intruders--;
            else if (Syndicate == max)
                Syndicate--;
            else
                Apocalypse--;
        }

        while (Outcasts > players - Intruders - Syndicate - Apocalypse)
            Outcasts--;

        Crew = players - Intruders - Syndicate - Outcasts - Apocalypse;

        if (TownOfUsReworked.MciActive)
            Info($"Crew = {Crew}, Int = {Intruders}, Syn = {Syndicate}, Neut = {Outcasts}, Apoc = {Apocalypse}");
    }

    protected static Layer GetRandomBaseEvil(Faction faction)
    {
        if (faction is < Faction.Outcast and > Faction.Crew)
        {
            return faction switch
            {
                Faction.Intruder => Layer.Impostor,
                Faction.Syndicate => Layer.Anarchist,
                _ => Layer.Cultist,
            };
        }

        var choices = new List<Layer>();
        string values = faction switch
        {
            Faction.Illuminati => BadGuysSettings.IlluminatiMembers,
            Faction.Compliance => BadGuysSettings.ComplianceMembers,
            Faction.Pandorica => BadGuysSettings.PandoricaMembers,
            _ => string.Empty,
        };

        if (IsNullEmptyOrWhiteSpace(values))
            return Layer.Crewmate;

        if (values.Contains("Intruders"))
            choices.Add(Layer.Impostor);

        if (values.Contains("Apocalypse"))
            choices.Add(Layer.Cultist);

        if (values.Contains("Syndicate"))
            choices.Add(Layer.Anarchist);

        if (values.Contains("Killers"))
            choices.Add(Layer.Murderer);

        if (values.Contains("Neophytes"))
            choices.Add(Layer.Zealot);

        return choices.Random();
    }
}