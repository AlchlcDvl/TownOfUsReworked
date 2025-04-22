namespace TownOfUsReworked.PlayerLayers.Dispositions;

[LayerHeaderOption(LayerEnum.Defector)]
public sealed class Defector : Disposition
{
    [ToggleOption]
    private static bool DefectorKnows = true;

    [StringOption<DefectorFaction>]
    private static DefectorFaction DefectorFaction = DefectorFaction.Random;

    public bool Turned { get; private set; }
    public Faction Side { get; private set; }
    private Role PlayerRole { get; set; }

    protected override UColor MainColor => Turned ? PlayerRole.FactionColor : CustomColorManager.Defector;
    public override string Symbol => "ε";
    public override LayerEnum Type => LayerEnum.Defector;
    public override Func<string> Description => () => "- Be the last one of your faction to switch sides";
    public override bool Hidden => !DefectorKnows && !Turned;

    protected override void Init()
    {
        base.Init();
        PlayerRole = Player.GetRole();
        Side = PlayerRole.Faction;
    }

    public override void UpdateHud(HudManager __instance)
    {
        if (Dead || Turned || !Last(Side))
            return;

        var faction = GetFactionChoice();
        CallRpc(CustomRPC.Misc, MiscRPC.ChangeRoles, this, faction);
        TurnSides(faction);
    }

    protected override void CheckWin(List<byte> winnerIds)
    {
        if (Side != Faction.Neutral || AllPlayers().Any(x => !x.HasDied() && !x.Is<Defector>() && !x.Is(Faction.Neutral)))
            return;

        WinState = WinLose.DefectorWins;
        winnerIds.Add(PlayerId);
    }

    private Faction GetFactionChoice()
    {
        if (GameModifiers.IlluminatiUnleashed)
        {
            return DefectorFaction switch
            {
                DefectorFaction.Neutral or DefectorFaction.NonCrew => Faction.Neutral,
                DefectorFaction.Crew or DefectorFaction.NonNeutral => Faction.Crew,
                _ => Side
            };
        }

        var factions = new List<Faction>();

        switch (DefectorFaction)
        {
            case DefectorFaction.Neutral:
            {
                factions.Add(Faction.Neutral);
                break;
            }
            case DefectorFaction.OpposingEvil:
            {
                factions.Add(Faction.Intruder, Faction.Syndicate, Faction.Apocalypse);
                break;
            }
            case DefectorFaction.Random:
            {
                factions.Add(Faction.Crew, Faction.Neutral, Faction.Intruder, Faction.Syndicate, Faction.Apocalypse);
                break;
            }
            case DefectorFaction.NonCrew:
            {
                factions.Add(Faction.Neutral, Faction.Intruder, Faction.Syndicate, Faction.Apocalypse);
                break;
            }
            case DefectorFaction.NonNeutral:
            {
                factions.Add(Faction.Crew, Faction.Intruder, Faction.Syndicate, Faction.Apocalypse);
                break;
            }
            case DefectorFaction.NonFaction:
            {
                factions.Add(Faction.Crew, Faction.Neutral);
                break;
            }
            default:
            {
                factions.Add(Faction.Crew);
                break;
            }
        }

        if (GameModifiers.OrderOfCompliance)
        {
            factions.Add(Faction.Compliance);

            if (GameModifiers.ComplianceType == ComplianceType.Killers)
                factions.RemoveAll(Faction.Neutral);
        }

        if (GameModifiers.PandoricaOpens)
        {
            factions.RemoveAll(Faction.Intruder, Faction.Syndicate, Faction.Apocalypse);
            factions.Add(Faction.Pandorica);
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
        PlayerRole.Faction = Side = faction;

        if (faction == Faction.Neutral)
            PlayerRole.Objectives = () => "- Be the last one standing";

        if (Local)
            Flash(Color);

        if (CustomPlayer.Local.Is<Mystic>())
            Flash(CustomColorManager.Mystic);
    }
}