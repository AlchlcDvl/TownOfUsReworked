using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

public class TargetGen : BaseGen
{
    public override void Assign()
    {
        if (GetSpawnItem(LayerEnum.Allied).IsActive())
        {
            foreach (var ally in PlayerLayer.GetLayers<Allied>())
            {
                var alliedRole = ally.Player.GetRole();
                var factions = new List<Faction>() { Faction.Crew, Faction.Intruder, Faction.Syndicate };
                var faction = Allied.AlliedFaction == AlliedFaction.Random ? factions.Random() : factions[(int)Allied.AlliedFaction - 1];
                ally.Side = alliedRole.Faction = faction;
                CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, ally, faction);
            }

            Message("Allied Factions Set");
        }

        if (GetSpawnItem(LayerEnum.Lovers).IsActive())
        {
            var lovers = PlayerLayer.GetLayers<Lovers>().ToList();
            lovers.Shuffle();

            for (var i = 0; i < lovers.Count - 1; i += 2)
            {
                var lover = lovers[i];

                if (lover.OtherLover)
                    continue;

                var other = lovers[i + 1];

                if (!other || other.OtherLover)
                    continue;

                lover.OtherLover = other.Player;
                other.OtherLover = lover.Player;
                CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, lover, other);

                if (TownOfUsReworked.MCIActive)
                    Message($"Lovers = {lover.PlayerName} & {other.PlayerName}");
            }

            foreach (var lover in lovers)
            {
                if (!lover.OtherLover)
                    NullLayer(lover.Player, PlayerLayerEnum.Disposition);
            }

            Message("Lovers Set");
        }

        if (GetSpawnItem(LayerEnum.Rivals).IsActive())
        {
            var rivals = PlayerLayer.GetLayers<Rivals>().ToList();
            rivals.Shuffle();

            for (var i = 0; i < rivals.Count - 1; i += 2)
            {
                var rival = rivals[i];

                if (rival.OtherRival)
                    continue;

                var other = rivals[i + 1];

                if (!other || other.OtherRival)
                    continue;

                rival.OtherRival = other.Player;
                other.OtherRival = rival.Player;
                CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, rival, other);

                if (TownOfUsReworked.MCIActive)
                    Message($"Rivals = {rival.PlayerName} & {other.PlayerName}");
            }

            foreach (var rival in rivals)
            {
                if (!rival.OtherRival)
                    NullLayer(rival.Player, PlayerLayerEnum.Disposition);
            }

            Message("Rivals Set");
        }

        if (GetSpawnItem(LayerEnum.Linked).IsActive())
        {
            var linked = PlayerLayer.GetLayers<Linked>().ToList();
            linked.Shuffle();

            for (var i = 0; i < linked.Count - 1; i += 2)
            {
                var link = linked[i];

                if (link.OtherLink)
                    continue;

                var other = linked[i + 1];

                if (!other || other.OtherLink)
                    continue;

                link.OtherLink = other.Player;
                other.OtherLink = link.Player;
                CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, link, other);

                if (TownOfUsReworked.MCIActive)
                    Message($"Linked = {link.PlayerName} & {other.PlayerName}");
            }

            foreach (var link in linked)
            {
                if (!link.OtherLink)
                    NullLayer(link.Player, PlayerLayerEnum.Disposition);
            }

            Message("Linked Set");
        }

        if (GetSpawnItem(LayerEnum.Mafia).IsActive())
        {
            var mafia = PlayerLayer.GetLayers<Mafia>();

            if (mafia.Count() == 1)
                NullLayer(mafia.First().Player, PlayerLayerEnum.Disposition);

            Message("Mafia Set");
        }

        if (!Executioner.ExecutionerCanPickTargets && GetSpawnItem(LayerEnum.Executioner).IsActive())
        {
            foreach (var exe in PlayerLayer.GetLayers<Executioner>())
            {
                exe.TargetPlayer = AllPlayers().Random(x => x != exe.Player && !x.IsLinkedTo(exe.Player) && !x.Is(Alignment.CrewSov));

                if (exe.TargetPlayer)
                {
                    CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, exe, exe.TargetPlayer);

                    if (TownOfUsReworked.MCIActive)
                        Message($"Exe Target = {exe.TargetPlayer.name}");
                }
            }

            Message("Exe Targets Set");
        }

        if (!Guesser.GuesserCanPickTargets && GetSpawnItem(LayerEnum.Guesser).IsActive())
        {
            foreach (var guess in PlayerLayer.GetLayers<Guesser>())
            {
                guess.TargetPlayer = AllPlayers().Random(x => x != guess.Player && !x.IsLinkedTo(guess.Player) && !x.Is(Alignment.NeutralEvil) && !x.Is(Alignment.CrewInvest) &&
                    !x.Is<Indomitable>());

                if (guess.TargetPlayer)
                {
                    CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, guess, guess.TargetPlayer);

                    if (TownOfUsReworked.MCIActive)
                        Message($"Guess Target = {guess.TargetPlayer.name}");
                }
            }

            Message("Guess Targets Set");
        }

        if (!GuardianAngel.GuardianAngelCanPickTargets && GetSpawnItem(LayerEnum.GuardianAngel).IsActive())
        {
            foreach (var ga in PlayerLayer.GetLayers<GuardianAngel>())
            {
                ga.TargetPlayer = AllPlayers().Random(x => x != ga.Player && !x.IsLinkedTo(ga.Player) && !x.Is(Alignment.NeutralEvil));

                if (ga.TargetPlayer)
                {
                    CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, ga, ga.TargetPlayer);

                    if (TownOfUsReworked.MCIActive)
                        Message($"GA Target = {ga.TargetPlayer.name}");
                }
            }

            Message("GA Target Set");
        }

        if (!BountyHunter.BountyHunterCanPickTargets && GetSpawnItem(LayerEnum.BountyHunter).IsActive())
        {
            foreach (var bh in PlayerLayer.GetLayers<BountyHunter>())
            {
                bh.TargetPlayer = AllPlayers().Random(x => x != bh.Player && !bh.Player.IsLinkedTo(x));

                if (bh.TargetPlayer)
                {
                    CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, bh, bh.TargetPlayer);

                    if (TownOfUsReworked.MCIActive)
                        Message($"BH Target = {bh.TargetPlayer.name}");
                }
            }

            Message("BH Targets Set");
        }

        if (!Actor.ActorCanPickRole && GetSpawnItem(LayerEnum.Actor).IsActive())
        {
            foreach (var act in PlayerLayer.GetLayers<Actor>())
            {
                act.FillRoles(AllPlayers().Random(x => x != act.Player));
                CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, act, act.PretendRoles);

                if (TownOfUsReworked.MCIActive && act.PretendRoles.Count > 0)
                    Message($"Act Targets = {act.PretendListString()}");
            }

            Message("Act Variables Set");
        }

        if (GetSpawnItem(LayerEnum.Jackal).IsActive())
        {
            foreach (var jackal in PlayerLayer.GetLayers<Jackal>())
            {
                jackal.Recruit1 = AllPlayers().Random(x => !x.Is(Alignment.NeutralNeo) && x.Is(SubFaction.None) && !x.Is(Alignment.NeutralEvil) && !x.Is(Alignment.NeutralBen));

                if (jackal.Recruit1)
                {
                    jackal.Recruit2 = AllPlayers().Random(x => x.GetAlignment() is not (Alignment.NeutralNeo or Alignment.NeutralEvil or Alignment.NeutralBen) && x.Is(SubFaction.None) &&
                        (jackal.Recruit1.GetFaction() != x.GetFaction() || (jackal.Recruit1.GetFaction() == Faction.Neutral && x.GetFaction() == Faction.Neutral && x.GetRole().Type !=
                        jackal.Recruit1.GetRole().Type)));
                }

                if (jackal.Recruit1)
                    RpcConvert(jackal.Recruit1.PlayerId, jackal.PlayerId, SubFaction.Cabal);

                if (jackal.Recruit2)
                    RpcConvert(jackal.Recruit2.PlayerId, jackal.PlayerId, SubFaction.Cabal);

                if (TownOfUsReworked.MCIActive && jackal.Recruit1 && jackal.Recruit2)
                    Message($"Recruits = {jackal.Recruit1.name} & {jackal.Recruit2.name}");
            }

            Message("Jackal Recruits Set");
        }
    }
}