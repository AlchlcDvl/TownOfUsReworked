namespace TownOfUsReworked.PlayerLayers.Dispositions;

[LayerHeaderOption(LayerEnum.Traitor)]
public sealed class Traitor : FactionChanger
{
    [ToggleOption]
    private static bool TraitorKnows = true;

    [ToggleOption]
    private static bool TraitorColourSwap = false;

    protected override UColor MainColor => CustomColorManager.Traitor;
    public override string Symbol => "♣";
    public override LayerEnum Type => LayerEnum.Traitor;
    public override string Description => !Turned ? "- Finish your tasks to join either the <#FF1919FF>Intruders</color> or the <#008000FF>Syndicate</color>" : "";
    public override bool Hidden => !TraitorKnows && !Turned && !Dead;
    public override bool SnitchReveals => Snitch.SnitchSeesTraitor;
    public override bool RevealerReveals => Revealer.RevealerRevealsTraitor;
    public override bool SheriffSwap => TraitorColourSwap;

    public override void UponTaskComplete(uint taskId)
    {
        if (!TasksDone || !Local)
            return;

        var faction = GetFactionChoice();
        CallRpc(CustomRPC.Misc, MiscRPC.ChangeRoles, this, false, faction);
        TurnFaction(faction);
    }

    private static Faction GetFactionChoice()
    {
        var factions = new List<Faction>() { Faction.Intruder, Faction.Syndicate, Faction.Apocalypse };

        if (BadGuysSettings.IlluminatiUnleashed)
        {
            factions.Add(Faction.Illuminati);

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
                factions.Add(Faction.Compliance);

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
            return Faction.Crew;

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

        return Faction.Crew;
    }
}