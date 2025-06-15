namespace TownOfUsReworked.PlayerLayers.Dispositions;

[LayerHeaderOption(Layer.Defector)]
public sealed class Defector : Disposition
{
    [ToggleOption]
    private static bool DefectorKnows = true;

    [StringOption<DefectorFaction>]
    private static DefectorFaction DefectorFaction = DefectorFaction.Random;

    private bool Turned;
    private Faction Side
    {
        get;
        set => Handler.CurrentFaction = field = value;
    }

    protected override UColor MainColor => CustomColorManager.Defector;
    public override string Symbol => "ε";
    public override Layer Type => Layer.Defector;
    public override string Description => "- Be the last one of your faction to switch sides";
    public override bool Hidden => !DefectorKnows && !Turned;

    public override void Init() => Side = Handler.CurrentFaction;

    public override void UpdateHud(HudManager __instance)
    {
        if (Dead || Turned || !Last(Side))
            return;

        var faction = GetFactionChoice();
        CallRpc(ReworkedRpc.Misc, MiscRpc.ChangeRoles, this, faction);
        TurnSides(faction);
    }

    protected override void CheckWin(HashSet<byte> winnerIds)
    {
        if (Side != Faction.Outcast || AllPlayers().Any(x => !x.HasDied() && !x.Is<Defector>() && !x.Is(Faction.Outcast)))
            return;

        WinState = WinLose.DefectorWins;
        winnerIds.Add(PlayerId);
    }

    private Faction GetFactionChoice()
    {
        var factions = new List<Faction>();

        switch (DefectorFaction)
        {
            case DefectorFaction.Outcast:
            {
                factions.Add(Faction.Outcast);
                break;
            }
            case DefectorFaction.OpposingEvil:
            {
                factions.Add(Faction.Intruder, Faction.Syndicate, Faction.Apocalypse);
                break;
            }
            case DefectorFaction.Random:
            {
                factions.Add(Faction.Crew, Faction.Outcast, Faction.Intruder, Faction.Syndicate, Faction.Apocalypse);
                break;
            }
            case DefectorFaction.NonCrew:
            {
                factions.Add(Faction.Outcast, Faction.Intruder, Faction.Syndicate, Faction.Apocalypse);
                break;
            }
            case DefectorFaction.NonOutcast:
            {
                factions.Add(Faction.Crew, Faction.Intruder, Faction.Syndicate, Faction.Apocalypse);
                break;
            }
            case DefectorFaction.NonFaction:
            {
                factions.Add(Faction.Crew, Faction.Outcast);
                break;
            }
            default:
            {
                factions.Add(Faction.Crew);
                break;
            }
        }

        if (BadGuysSettings.IlluminatiUnleashed)
        {
            factions.Add(Faction.Illuminati);

            if (BadGuysSettings.IlluminatiMembers == IlluminatiType.Killers || BadGuysSettings.IlluminatiMembers == IlluminatiType.Neophytes)
                factions.Remove(Faction.Outcast);

            if (BadGuysSettings.IlluminatiMembers == IlluminatiType.Intruders)
                factions.Remove(Faction.Intruder);

            if (BadGuysSettings.IlluminatiMembers == IlluminatiType.Syndicate)
                factions.Remove(Faction.Syndicate);

            if (BadGuysSettings.IlluminatiMembers == IlluminatiType.Apocalypse)
                factions.Remove(Faction.Apocalypse);
        }
        else
        {
            if (BadGuysSettings.OrderOfCompliance)
            {
                factions.Add(Faction.Compliance);

                if (BadGuysSettings.ComplianceMembers == ComplianceType.Killers)
                    factions.Remove(Faction.Outcast);
            }

            if (BadGuysSettings.PandoricaOpens)
            {
                factions.Add(Faction.Pandorica);

                if (BadGuysSettings.PandoricaMembers == PandoricaType.Intruders)
                    factions.Remove(Faction.Intruder);

                if (BadGuysSettings.PandoricaMembers == PandoricaType.Syndicate)
                    factions.Remove(Faction.Syndicate);

                if (BadGuysSettings.PandoricaMembers == PandoricaType.Apocalypse)
                    factions.Remove(Faction.Apocalypse);
            }
        }

        var players = AllPlayers();
        var aliveCounts = factions.ToDictionary(x => x, x => players.Count(y => y.Is(x) && !y.HasDied()));
        var validFactions = aliveCounts.Where(kvp => kvp.Value > 0).ToList();

        if (!validFactions.Any())
            return Side;

        if (validFactions.Count == 1)
            return validFactions[0].Key;

        var maxCount = validFactions.Max(kvp => kvp.Value);
        var weightedFactions = validFactions.Select(kvp => (kvp.Key, maxCount - kvp.Value + 1));
        var totalWeight = weightedFactions.Sum(x => x.Item2);
        var random = URandom.RandomRangeInt(0, totalWeight + 1);

        foreach (var (faction, weight) in weightedFactions)
        {
            if (random < weight)
                return faction;

            random -= weight;
        }

        return Side;
    }

    public void TurnSides(Faction faction)
    {
        if (Side == faction)
            return;

        Turned = true;
        Side = faction;

        if (faction == Faction.Outcast)
            Handler.CurrentRole.Objectives = () => "- Be the last one standing";

        if (Local)
            Flash(Color);

        if (LocalPlayer.Is<Mystic>())
            Flash(CustomColorManager.Mystic);
    }
}