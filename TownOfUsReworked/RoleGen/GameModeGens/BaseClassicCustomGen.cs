using static TownOfUsReworked.Managers.RoleGenManager;

namespace TownOfUsReworked.RoleGen;

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
            {
                for (var j = 0; j < spawn.Count; j++)
                {
                    if (Check(spawn))
                        SetPostmortals.Revealers++;
                }
            }
            else if (spawn.IsActive())
            {
                if (layer is LayerEnum.Mayor or LayerEnum.Monarch or LayerEnum.Dictator)
                    RoleGenManager.CrewSovereignRoles.AddMany(spawn.Clone, spawn.Count);
                else if (layer is LayerEnum.Mystic or LayerEnum.VampireHunter)
                    RoleGenManager.CrewAuditorRoles.AddMany(spawn.Clone, spawn.Count);
                else if (layer is LayerEnum.Coroner or LayerEnum.Detective or LayerEnum.Medium or LayerEnum.Operative or LayerEnum.Seer or LayerEnum.Sheriff or LayerEnum.Tracker)
                    RoleGenManager.CrewInvestigativeRoles.AddMany(spawn.Clone, spawn.Count);
                else if (layer is LayerEnum.Bastion or LayerEnum.Vigilante or LayerEnum.Veteran)
                    RoleGenManager.CrewKillingRoles.AddMany(spawn.Clone, spawn.Count);
                else if (layer is LayerEnum.Altruist or LayerEnum.Medic or LayerEnum.Trapper)
                    RoleGenManager.CrewProtectiveRoles.AddMany(spawn.Clone, spawn.Count);
                else if (layer is LayerEnum.Chameleon or LayerEnum.Engineer or LayerEnum.Escort or LayerEnum.Retributionist or LayerEnum.Shifter or LayerEnum.Transporter)
                    RoleGenManager.CrewSupportRoles.AddMany(spawn.Clone, spawn.Count);
                else if (layer == LayerEnum.Crewmate)
                    CrewRoles.AddMany(spawn.Clone, spawn.Count);
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
            {
                for (var j = 0; j < spawn.Count; j++)
                {
                    if (Check(spawn))
                        SetPostmortals.Ghouls++;
                }
            }
            else if (spawn.IsActive(Intruders))
            {
                if (layer == LayerEnum.Godfather)
                    RoleGenManager.IntruderHeadRoles.AddMany(spawn.Clone, spawn.Count);
                else if (layer is LayerEnum.Blackmailer or LayerEnum.Camouflager or LayerEnum.Grenadier or LayerEnum.Janitor)
                    RoleGenManager.IntruderConcealingRoles.AddMany(spawn.Clone, spawn.Count);
                else if (layer is LayerEnum.Disguiser or LayerEnum.Morphling or LayerEnum.Wraith)
                    RoleGenManager.IntruderDeceptionRoles.AddMany(spawn.Clone, spawn.Count);
                else if (layer is LayerEnum.Ambusher or LayerEnum.Enforcer)
                    RoleGenManager.IntruderKillingRoles.AddMany(spawn.Clone, spawn.Count);
                else if (layer is LayerEnum.Consigliere or LayerEnum.Consort or LayerEnum.Miner or LayerEnum.Teleporter)
                    RoleGenManager.IntruderSupportRoles.AddMany(spawn.Clone, spawn.Count);
                else if (layer == LayerEnum.Impostor)
                    IntruderRoles.AddMany(spawn.Clone, spawn.Count);
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
            {
                for (var j = 0; j < spawn.Count; j++)
                {
                    if (Check(spawn))
                        SetPostmortals.Phantoms++;
                }
            }
            else if (spawn.IsActive())
            {
                if (layer is LayerEnum.Plaguebearer or LayerEnum.Pestilence)
                    RoleGenManager.NeutralHarbingerRoles.AddMany(spawn.Clone, spawn.Count);
                else if (layer is LayerEnum.Amnesiac or LayerEnum.GuardianAngel or LayerEnum.Survivor or LayerEnum.Thief)
                    RoleGenManager.NeutralBenignRoles.AddMany(spawn.Clone, spawn.Count);
                else if (layer is LayerEnum.Actor or LayerEnum.BountyHunter or LayerEnum.Cannibal or LayerEnum.Executioner or LayerEnum.Guesser or LayerEnum.Jester or LayerEnum.Troll)
                    RoleGenManager.NeutralEvilRoles.AddMany(spawn.Clone, spawn.Count);
                else if (layer is LayerEnum.Arsonist or LayerEnum.Cryomaniac or LayerEnum.Glitch or LayerEnum.Juggernaut or LayerEnum.Murderer or LayerEnum.SerialKiller or LayerEnum.Werewolf)
                    RoleGenManager.NeutralKillingRoles.AddMany(spawn.Clone, spawn.Count);
                else if (layer is LayerEnum.Dracula or LayerEnum.Jackal or LayerEnum.Necromancer or LayerEnum.Whisperer)
                    RoleGenManager.NeutralNeophyteRoles.AddMany(spawn.Clone, spawn.Count);
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
            {
                for (var j = 0; j < spawn.Count; j++)
                {
                    if (Check(spawn))
                        SetPostmortals.Banshees++;
                }
            }
            else if (spawn.IsActive(Syndicate))
            {
                if (layer is LayerEnum.Rebel or LayerEnum.Spellslinger)
                    RoleGenManager.SyndicatePowerRoles.AddMany(spawn.Clone, spawn.Count);
                else if (layer is LayerEnum.Concealer or LayerEnum.Drunkard or LayerEnum.Framer or LayerEnum.Shapeshifter or LayerEnum.Silencer or LayerEnum.Timekeeper)
                    RoleGenManager.SyndicateDisruptionRoles.AddMany(spawn.Clone, spawn.Count);
                else if (layer is LayerEnum.Bomber or LayerEnum.Collider or LayerEnum.Crusader or LayerEnum.Poisoner)
                    RoleGenManager.SyndicateKillingRoles.AddMany(spawn.Clone, spawn.Count);
                else if (layer is LayerEnum.Stalker or LayerEnum.Warper)
                    RoleGenManager.SyndicateSupportRoles.AddMany(spawn.Clone, spawn.Count);
                else if (layer == LayerEnum.Anarchist)
                    SyndicateRoles.AddMany(spawn.Clone, spawn.Count);
            }
        }
    }

    public override void Filter()
    {
        AllRoles.AddRanges(NeutralRoles, CrewRoles, SyndicateRoles);

        if (!SyndicateSettings.AltImps)
            AllRoles.AddRange(IntruderRoles);

        if (!AllRoles.Any(x => x.ID == LayerEnum.Dracula))
            AllRoles.AddMany(GetSpawnItem(LayerEnum.Vigilante).Clone, AllRoles.RemoveAll(x => x.ID == LayerEnum.VampireHunter));

        if (!AllRoles.Any(x => x.ID is LayerEnum.Dracula or LayerEnum.Jackal or LayerEnum.Necromancer or LayerEnum.Whisperer))
            AllRoles.AddMany(GetSpawnItem(LayerEnum.Seer).Clone, AllRoles.RemoveAll(x => x.ID == LayerEnum.Mystic));

        if (AllRoles.Any(x => x.ID == LayerEnum.Cannibal) && AllRoles.Any(x => x.ID == LayerEnum.Janitor) && GameModifiers.JaniCanMutuallyExclusive)
        {
            var chance = URandom.RandomRangeInt(0, 2);
            var value = chance == 0 ? NeutralRoles.Random(x => x.ID != LayerEnum.Cannibal, GetSpawnItem(LayerEnum.Amnesiac)) : IntruderRoles.Random(x => x.ID != LayerEnum.Janitor,
                GetSpawnItem(LayerEnum.Impostor));
            AllRoles.AddMany(value.Clone, AllRoles.RemoveAll(x => x.ID == (chance == 0 ? LayerEnum.Cannibal : LayerEnum.Janitor)));
        }

        if (GameData.Instance.PlayerCount <= 4 && AllRoles.Any(x => x.ID == LayerEnum.Amnesiac))
            AllRoles.AddMany(GetSpawnItem(LayerEnum.Thief).Clone, AllRoles.RemoveAll(x => x.ID == LayerEnum.Amnesiac));

        NeutralRoles.Clear();
        CrewRoles.Clear();
        SyndicateRoles.Clear();
        IntruderRoles.Clear();
    }

    public override void PostAssignment()
    {
        var allPlayers = AllPlayers();

        if (!allPlayers.Any(x => x.GetRole() is VampireHunter or Amnesiac or Thief or Godfather or Shifter or Guesser or Rebel or Executioner or GuardianAngel or BountyHunter or Mystic or Actor
            || x.GetDisposition() is Traitor or Fanatic))
        {
            allPlayers.Where(x => x.Is<Seer>()).ForEach(x => Gen(x, LayerEnum.Sheriff, PlayerLayerEnum.Role));
        }
    }
}