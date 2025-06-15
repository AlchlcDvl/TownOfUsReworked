using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public sealed class TargetGen : BaseGen
{
    public override void Assign()
    {
        if (GetSpawnItem(Layer.Allied).IsActive())
        {
            foreach (var ally in PlayerLayer.GetLayers<Allied>())
            {
                var factions = new List<Faction>() { Faction.Crew, Faction.Intruder, Faction.Syndicate, Faction.Apocalypse };

                if (BadGuysSettings.PandoricaOpens)
                {
                    factions.RemoveAll(Faction.Intruder, Faction.Syndicate, Faction.Apocalypse);
                    factions.Add(Faction.Pandorica);
                }

                if (BadGuysSettings.OrderOfCompliance && BadGuysSettings.ComplianceMembers != ComplianceType.Killers)
                    factions.Add(Faction.Compliance);

                var faction = Allied.AlliedFaction == AlliedFaction.Random ? factions.Random() : factions.Find(x => x.ToString() == Allied.AlliedFaction.ToString());
                ally.Side = faction;
                CallRpc(MiscRpc.SetTarget, ally, faction);
            }

            Message("Allied Factions Set");
        }

        if (GetSpawnItem(Layer.Lovers).IsActive())
        {
            var lovers = PlayerLayer.GetLayers<Lovers>().ToList();
            lovers.Shuffle();

            for (var i = 0; i < lovers.Count - 1; i += 2)
            {
                var lover = lovers[i];

                if (lover.Other)
                    continue;

                var other = lovers[i + 1];

                if (!other || other.Other)
                    continue;

                lover.Other = other.Player;
                other.Other = lover.Player;
                CallRpc(MiscRpc.SetTarget, lover, other);

                if (TownOfUsReworked.MciActive)
                    Message($"Lovers = {lover.PlayerName} & {other.PlayerName}");
            }

            lovers.Where(lover => !lover.Other).Do(x => NullLayer(x.Player, PlayerLayerEnum.Disposition));
            Success("Lovers Set");
        }

        if (GetSpawnItem(Layer.Rivals).IsActive())
        {
            var rivals = PlayerLayer.GetLayers<Rivals>().ToList();
            rivals.Shuffle();

            for (var i = 0; i < rivals.Count - 1; i += 2)
            {
                var rival = rivals[i];

                if (rival.Other)
                    continue;

                var other = rivals[i + 1];

                if (!other || other.Other)
                    continue;

                rival.Other = other.Player;
                other.Other = rival.Player;
                CallRpc(MiscRpc.SetTarget, rival, other);

                if (TownOfUsReworked.MciActive)
                    Message($"Rivals = {rival.PlayerName} & {other.PlayerName}");
            }

            rivals.Where(rival => !rival.Other).Do(x => NullLayer(x.Player, PlayerLayerEnum.Disposition));
            Success("Rivals Set");
        }

        if (GetSpawnItem(Layer.Linked).IsActive())
        {
            var linked = PlayerLayer.GetLayers<Linked>().ToList();
            linked.Shuffle();

            for (var i = 0; i < linked.Count - 1; i += 2)
            {
                var link = linked[i];

                if (link.Other)
                    continue;

                var other = linked[i + 1];

                if (!other || other.Other)
                    continue;

                link.Other = other.Player;
                other.Other = link.Player;
                CallRpc(MiscRpc.SetTarget, link, other);

                if (TownOfUsReworked.MciActive)
                    Message($"Linked = {link.PlayerName} & {other.PlayerName}");
            }

            linked.Where(link => !link.Other).Do(x => NullLayer(x.Player, PlayerLayerEnum.Disposition));
            Success("Linked Set");
        }

        if (GetSpawnItem(Layer.Mafia).IsActive())
        {
            var mafia = PlayerLayer.GetLayers<Mafia>();

            if (mafia.Count() == 1)
                NullLayer(mafia.First().Player, PlayerLayerEnum.Disposition);

            Success("Mafia Set");
        }

        if (!Executioner.ExecutionerCanPickTargets && GetSpawnItem(Layer.Executioner).IsActive())
        {
            foreach (var exe in PlayerLayer.GetLayers<Executioner>())
            {
                exe.TargetPlayer = AllPlayers().Random(x => x != exe.Player && !x.IsLinkedTo(exe.Player) && !x.Is(Alignment.Sovereign));

                if (!exe.TargetPlayer)
                    continue;

                CallRpc(MiscRpc.SetTarget, exe, exe.TargetPlayer);

                if (TownOfUsReworked.MciActive)
                    Message($"Exe Target = {exe.TargetPlayer.name}");
            }

            Success("Exe Targets Set");
        }

        if (!Guesser.GuesserCanPickTargets && GetSpawnItem(Layer.Guesser).IsActive())
        {
            foreach (var guess in PlayerLayer.GetLayers<Guesser>())
            {
                guess.TargetPlayer = AllPlayers().Random(x => x != guess.Player && !x.IsLinkedTo(guess.Player) && !x.Is(Alignment.Evil) && !x.Is(Alignment.Investigative) &&
                    !x.Is<Indomitable>());

                if (!guess.TargetPlayer)
                    continue;

                CallRpc(MiscRpc.SetTarget, guess, guess.TargetPlayer);

                if (TownOfUsReworked.MciActive)
                    Message($"Guess Target = {guess.TargetPlayer.name}");
            }

            Success("Guess Targets Set");
        }

        if (!GuardianAngel.GuardianAngelCanPickTargets && GetSpawnItem(Layer.GuardianAngel).IsActive())
        {
            foreach (var ga in PlayerLayer.GetLayers<GuardianAngel>())
            {
                ga.TargetPlayer = AllPlayers().Random(x => x != ga.Player && !x.IsLinkedTo(ga.Player) && !x.Is(Alignment.Evil));

                if (!ga.TargetPlayer)
                    continue;

                CallRpc(MiscRpc.SetTarget, ga, ga.TargetPlayer);

                if (TownOfUsReworked.MciActive)
                    Message($"GA Target = {ga.TargetPlayer.name}");
            }

            Success("GA Target Set");
        }

        if (!BountyHunter.BountyHunterCanPickTargets && GetSpawnItem(Layer.BountyHunter).IsActive())
        {
            foreach (var bh in PlayerLayer.GetLayers<BountyHunter>())
            {
                bh.TargetPlayer = AllPlayers().Random(x => x != bh.Player && !bh.Player.IsLinkedTo(x));

                if (!bh.TargetPlayer)
                    continue;

                CallRpc(MiscRpc.SetTarget, bh, bh.TargetPlayer);

                if (TownOfUsReworked.MciActive)
                    Message($"BH Target = {bh.TargetPlayer.name}");
            }

            Success("BH Targets Set");
        }

        if (!Actor.ActorCanPickRole && GetSpawnItem(Layer.Actor).IsActive())
        {
            foreach (var act in PlayerLayer.GetLayers<Actor>())
            {
                act.FillRoles(AllPlayers().Random(x => x != act.Player));
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
                jackal.Recruit1 = AllPlayers().Random(x => x.GetAlignment() is not (Alignment.Neophyte or Alignment.Evil or Alignment.Benign) && x.GetFaction().IsConvertible());

                if (jackal.Recruit1)
                {
                    jackal.Recruit2 = AllPlayers().Random(x => x.GetAlignment() is not (Alignment.Neophyte or Alignment.Evil or Alignment.Benign) && x.GetFaction().IsConvertible() &&
                        (jackal.Recruit1.GetFaction() != x.GetFaction() || (jackal.Recruit1.GetFaction().IsOk() == x.GetFaction().IsOk() && x.GetRole().Type != jackal.Recruit1.GetRole().Type)));
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