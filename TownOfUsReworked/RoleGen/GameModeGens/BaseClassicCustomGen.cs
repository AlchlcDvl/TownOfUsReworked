using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen2;

public abstract class BaseClassicCustomAllAnyGen : BaseRoleGen
{
    public override void InitList() => GetAdjustedFactions();

    public override void InitCrewList()
    {
        if (Crew == 0)
            return;

        foreach (var layer in GetValuesFromTo(LayerEnum.Altruist, LayerEnum.Vigilante))
        {
            var spawn = GetSpawnItem(layer);

            if (layer == LayerEnum.Revealer)
                SetPostmortals.RevealerOn = Check(spawn);
            else if (spawn.IsActive())
            {
                for (var j = 0; j < spawn.Count; j++)
                {
                    if (layer is LayerEnum.Mayor or LayerEnum.Monarch or LayerEnum.Dictator)
                        RoleGenManager.CrewSovereignRoles.Add(spawn);
                    else if (layer is LayerEnum.Mystic or LayerEnum.VampireHunter)
                        RoleGenManager.CrewAuditorRoles.Add(spawn);
                    else if (layer is LayerEnum.Coroner or LayerEnum.Detective or LayerEnum.Medium or LayerEnum.Operative or LayerEnum.Seer or LayerEnum.Sheriff or LayerEnum.Tracker)
                        RoleGenManager.CrewInvestigativeRoles.Add(spawn);
                    else if (layer is LayerEnum.Bastion or LayerEnum.Vigilante or LayerEnum.Veteran)
                        RoleGenManager.CrewKillingRoles.Add(spawn);
                    else if (layer is LayerEnum.Altruist or LayerEnum.Medic or LayerEnum.Trapper)
                        RoleGenManager.CrewProtectiveRoles.Add(spawn);
                    else if (layer is LayerEnum.Chameleon or LayerEnum.Engineer or LayerEnum.Escort or LayerEnum.Retributionist or LayerEnum.Shifter or LayerEnum.Transporter)
                        RoleGenManager.CrewSupportRoles.Add(spawn);
                    else if (layer == LayerEnum.Crewmate)
                        CrewRoles.Add(spawn);
                }
            }
        }
    }

    public override void InitIntList()
    {
        if (Intruders == 0)
            return;

        foreach (var layer in GetValuesFromTo(LayerEnum.Ambusher, LayerEnum.Wraith, x => x is not (LayerEnum.PromotedGodfather or LayerEnum.Mafioso)))
        {
            var spawn = GetSpawnItem(layer);

            if (layer == LayerEnum.Ghoul)
                SetPostmortals.GhoulOn = Check(spawn);
            else if (spawn.IsActive(Intruders))
            {
                for (var j = 0; j < spawn.Count; j++)
                {
                    if (layer == LayerEnum.Godfather)
                        RoleGenManager.IntruderHeadRoles.Add(spawn);
                    else if (layer is LayerEnum.Blackmailer or LayerEnum.Camouflager or LayerEnum.Grenadier or LayerEnum.Janitor)
                        RoleGenManager.IntruderConcealingRoles.Add(spawn);
                    else if (layer is LayerEnum.Disguiser or LayerEnum.Morphling or LayerEnum.Wraith)
                        RoleGenManager.IntruderDeceptionRoles.Add(spawn);
                    else if (layer is LayerEnum.Ambusher or LayerEnum.Enforcer)
                        RoleGenManager.IntruderKillingRoles.Add(spawn);
                    else if (layer is LayerEnum.Consigliere or LayerEnum.Consort or LayerEnum.Miner or LayerEnum.Teleporter)
                        RoleGenManager.IntruderSupportRoles.Add(spawn);
                    else if (layer == LayerEnum.Impostor)
                        IntruderRoles.Add(spawn);
                }
            }
        }
    }

    public override void InitNeutList()
    {
        if (Neutrals == 0)
            return;

        foreach (var layer in GetValuesFromTo(LayerEnum.Actor, LayerEnum.Whisperer, x => x != LayerEnum.Betrayer && !(x == LayerEnum.Pestilence && !NeutralApocalypseSettings.DirectSpawn) &&
            !(x == LayerEnum.Plaguebearer && NeutralApocalypseSettings.DirectSpawn)))
        {
            var spawn = GetSpawnItem(layer);

            if (layer == LayerEnum.Phantom)
                SetPostmortals.PhantomOn = Check(spawn);
            else if (spawn.IsActive())
            {
                for (var j = 0; j < spawn.Count; j++)
                {
                    if (layer is LayerEnum.Plaguebearer or LayerEnum.Pestilence)
                        RoleGenManager.NeutralHarbingerRoles.Add(spawn);
                    else if (layer is LayerEnum.Amnesiac or LayerEnum.GuardianAngel or LayerEnum.Survivor or LayerEnum.Thief)
                        RoleGenManager.NeutralBenignRoles.Add(spawn);
                    else if (layer is LayerEnum.Actor or LayerEnum.BountyHunter or LayerEnum.Cannibal or LayerEnum.Executioner or LayerEnum.Guesser or LayerEnum.Jester or LayerEnum.Troll)
                        RoleGenManager.NeutralEvilRoles.Add(spawn);
                    else if (layer is LayerEnum.Arsonist or LayerEnum.Cryomaniac or LayerEnum.Glitch or LayerEnum.Juggernaut or LayerEnum.Murderer or LayerEnum.SerialKiller or
                        LayerEnum.Werewolf)
                    {
                        RoleGenManager.NeutralKillingRoles.Add(spawn);
                    }
                    else if (layer is LayerEnum.Dracula or LayerEnum.Jackal or LayerEnum.Necromancer or LayerEnum.Whisperer)
                        RoleGenManager.NeutralNeophyteRoles.Add(spawn);
                }
            }
        }
    }

    public override void InitSynList()
    {
        if (Syndicate == 0)
            return;

        foreach (var layer in GetValuesFromTo(LayerEnum.Anarchist, LayerEnum.Warper, x => x is not (LayerEnum.PromotedRebel or LayerEnum.Sidekick)))
        {
            var spawn = GetSpawnItem(layer);

            if (layer == LayerEnum.Banshee)
                SetPostmortals.BansheeOn = Check(spawn);
            else if (spawn.IsActive(Syndicate))
            {
                for (var j = 0; j < spawn.Count; j++)
                {
                    if (layer is LayerEnum.Rebel or LayerEnum.Spellslinger)
                        RoleGenManager.SyndicatePowerRoles.Add(spawn);
                    else if (layer is LayerEnum.Concealer or LayerEnum.Drunkard or LayerEnum.Framer or LayerEnum.Shapeshifter or LayerEnum.Silencer or LayerEnum.Timekeeper)
                        RoleGenManager.SyndicateDisruptionRoles.Add(spawn);
                    else if (layer is LayerEnum.Bomber or LayerEnum.Collider or LayerEnum.Crusader or LayerEnum.Poisoner)
                        RoleGenManager.SyndicateKillingRoles.Add(spawn);
                    else if (layer is LayerEnum.Stalker or LayerEnum.Warper)
                        RoleGenManager.SyndicateSupportRoles.Add(spawn);
                    else if (layer == LayerEnum.Anarchist)
                        SyndicateRoles.Add(spawn);
                }
            }
        }
    }

    public override void Filter()
    {
        AllRoles.AddRanges(NeutralRoles, CrewRoles, SyndicateRoles);

        if (!SyndicateSettings.AltImps)
            AllRoles.AddRange(IntruderRoles);

        NeutralRoles.Clear();
        CrewRoles.Clear();
        SyndicateRoles.Clear();
        IntruderRoles.Clear();
    }

    public override void PostAssignment()
    {
        var allPlayers = AllPlayers();

        if (!allPlayers.Any(x => x.Is(LayerEnum.Dracula)))
        {
            foreach (var player in allPlayers.Where(x => x.Is(LayerEnum.VampireHunter)))
            {
                var role = player.GetRole();
                role.End();
                Gen(player, LayerEnum.Vigilante, PlayerLayerEnum.Role);
            }
        }

        if (!allPlayers.Any(x => x.GetRole() is Dracula or Jackal or Necromancer or Whisperer))
        {
            foreach (var player in allPlayers.Where(x => x.Is(LayerEnum.Mystic)))
            {
                var role = player.GetRole();
                role.End();
                new Seer().Start(player);
            }
        }

        if (!allPlayers.Any(x => x.GetRole() is VampireHunter or Amnesiac or Thief or Godfather or Shifter or Guesser or Rebel or Executioner or GuardianAngel or BountyHunter or Mystic or
            Actor || x.GetDisposition() is Traitor or Fanatic))
        {
            foreach (var player in allPlayers.Where(x => x.Is(LayerEnum.Seer)))
            {
                var role = player.GetRole();
                role.End();
                new Sheriff().Start(player);
            }
        }

        if (allPlayers.Any(x => x.Is(LayerEnum.Cannibal)) && allPlayers.Any(x => x.Is(LayerEnum.Janitor)) && GameModifiers.JaniCanMutuallyExclusive)
        {
            var chance = URandom.RandomRangeInt(0, 2);
            var count = AllRoles.RemoveAll(x => x.ID == (chance == 0 ? LayerEnum.Cannibal : LayerEnum.Janitor));
            var value = chance == 0 ? NeutralRoles.Random(x => x.ID != LayerEnum.Cannibal, GetSpawnItem(LayerEnum.Amnesiac)) : IntruderRoles.Random(x => x.ID != LayerEnum.Janitor,
                GetSpawnItem(LayerEnum.Impostor));

            for (var i = 0; i < count; i++)
                AllRoles.Add(value);
        }

        if (GameData.Instance.PlayerCount <= 4)
        {
            foreach (var player in allPlayers.Where(x => x.Is(LayerEnum.Amnesiac)))
            {
                var role = player.GetRole();
                role.End();
                new Thief().Start(player);
            }
        }
    }
}