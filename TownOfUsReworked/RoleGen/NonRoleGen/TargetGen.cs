using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public sealed class TargetGen : BaseGen
{
    private static readonly List<(Layer, Func<PlayerControl, bool>, Func<bool>)> Targeting =
    [
        (Layer.Executioner, x => x.Is(Alignment.Sovereign), () => Executioner.ExecutionerCanPickTargets),
        (Layer.Guesser, x => x.Is<Evil>() || x.Is(Alignment.Investigative) || x.Is<Indomitable>(), () => Guesser.GuesserCanPickTargets),
        (Layer.GuardianAngel, x => x.Is<Evil>(), () => GuardianAngel.GuardianAngelCanPickTargets),
        (Layer.BountyHunter, _ => false, () => BountyHunter.BountyHunterCanPickTargets),
    ];

    public override void Assign()
    {
        if (GetSpawnItem(Layer.Allied).IsActive())
        {
            foreach (var ally in PlayerLayer.GetLayers<Allied>())
            {
                var factions = new List<Faction>() { Faction.Crew, Faction.Intruder, Faction.Syndicate, Faction.Apocalypse };

                if (BadGuysSettings.PandoricaOpens)
                {
                    if (BadGuysSettings.PandoricaMembers == PandoricaType.Apocalypse)
                        factions.Remove(Faction.Apocalypse);

                    if (BadGuysSettings.PandoricaMembers == PandoricaType.Syndicate)
                        factions.Remove(Faction.Syndicate);

                    if (BadGuysSettings.PandoricaMembers == PandoricaType.Intruders)
                        factions.Remove(Faction.Intruder);

                    factions.Add(Faction.Pandorica);
                }

                if (BadGuysSettings.OrderOfCompliance && BadGuysSettings.ComplianceMembers != ComplianceType.Killers)
                    factions.Add(Faction.Compliance);

                var faction = Allied.AlliedFaction == AlliedFaction.Random ? factions.Random() : factions.Find(x => x.ToString() == Allied.AlliedFaction.ToString());
                ally.Side = faction;
                CallRpc(MiscRpc.SetTarget, ally, faction);

                if (TownOfUsReworked.MciActive)
                    Message($"Ally = {ally.PlayerName} & {faction}");
            }

            Message("Allied Factions Set");
        }

        if (GetSpawnItem(Layer.Lovers).IsActive() || GetSpawnItem(Layer.Linked).IsActive() || GetSpawnItem(Layer.Rivals).IsActive())
        {
            var allPaired = PlayerLayer.GetLayers<Paired>().SplitBy(x => x switch
            {
                Lovers => 0,
                Linked => 1,
                _ => 3
            });
            allPaired.Values.Do(x => x.Shuffle());

            foreach (var list in allPaired.Values)
            {
                for (var i = 0; i < list.Count - 1; i++)
                {
                    var paired = list[i];

                    if (paired.Other)
                        continue;

                    var other = list[i + 1];

                    if (!other || other.Other)
                        continue;

                    paired.Other = other.Player;
                    other.Other = paired.Player;
                    CallRpc(MiscRpc.SetTarget, paired, other);

                    if (TownOfUsReworked.MciActive)
                        Message($"{list[0].Type} = {paired.PlayerName} & {other.PlayerName}");
                }

                list.Where(lover => !lover.Other).Do(x => NullLayer(x.Player, PlayerLayerEnum.Disposition));
                Success($"{list[0].Type} Set");
            }
        }

        if (GetSpawnItem(Layer.Mafia).IsActive())
        {
            var mafia = PlayerLayer.GetLayers<Mafia>();

            if (mafia.Count() == 1)
                NullLayer(mafia.First().Player, PlayerLayerEnum.Disposition);

            Success("Mafia Set");
        }

        var targeters = PlayerLayer.GetLayers<ITargeter>();
        var allPlayers = AllPlayers();

        foreach (var (type, playerCheck, settingCheck) in Targeting)
        {
            if (!settingCheck() || !GetSpawnItem(type).IsActive())
                continue;

            foreach (var targeter in targeters.Where(x => x.Type == type))
            {
                targeter.TargetPlayer = allPlayers.Random(x => x != targeter.Player && !x!.IsLinkedTo(targeter.Player) && !playerCheck(x!))!;

                if (!targeter.TargetPlayer)
                    continue;

                CallRpc(MiscRpc.SetTarget, targeter, targeter.TargetPlayer);

                if (TownOfUsReworked.MciActive)
                    Message($"{type} Target = {targeter.TargetPlayer.name}");
            }

            Success($"{type} Targets Set");
        }

        if (!Actor.ActorCanPickRole && GetSpawnItem(Layer.Actor).IsActive())
        {
            foreach (var act in PlayerLayer.GetLayers<Actor>())
            {
                act.FillRoles(allPlayers.Random(x => x != act.Player)!);
                CallRpc(MiscRpc.SetTarget, act, act.PretendRoles);

                if (TownOfUsReworked.MciActive && act.PretendRoles.Count > 0)
                    Message($"Act Targets = {act.PretendListString()}");
            }

            Success("Act Variables Set");
        }

        if (GetSpawnItem(Layer.Jackal).IsActive())
        {
            foreach (var jackal in PlayerLayer.GetLayers<Jackal>())
            {
                jackal.Recruit1 = allPlayers.Random(x => x!.GetAlignment() is not (Alignment.Neophyte or Alignment.Evil or Alignment.Benign) && x!.GetFaction().IsConvertible())!;

                if (jackal.Recruit1)
                {
                    jackal.Recruit2 = allPlayers.Random(x => x!.GetAlignment() is not (Alignment.Neophyte or Alignment.Evil or Alignment.Benign) && x!.GetFaction().IsConvertible() &&
                        (jackal.Recruit1.GetFaction() != x!.GetFaction() || (jackal.Recruit1.GetFaction().IsOk() == x!.GetFaction().IsOk() && x!.GetRole().Type != jackal.Recruit1.GetRole().Type)))!;
                }

                if (jackal.Recruit1)
                    RpcConvert(jackal.Recruit1.PlayerId, jackal.PlayerId);

                if (jackal.Recruit2)
                    RpcConvert(jackal.Recruit2.PlayerId, jackal.PlayerId);

                if (TownOfUsReworked.MciActive && jackal.Recruit1 && jackal.Recruit2)
                    Message($"Recruits = {jackal.Recruit1.name} & {jackal.Recruit2.name}");
            }

            Success("Jackal Recruits Set");
        }

        Info("Targets set");
    }
}