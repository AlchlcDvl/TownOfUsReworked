using static TownOfUsReworked.RoleGen2.RoleGenManager;

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

        RoleGenManager.CrewAuditorRoles.Clear();
        RoleGenManager.CrewInvestigativeRoles.Clear();
        RoleGenManager.CrewKillingRoles.Clear();
        RoleGenManager.CrewProtectiveRoles.Clear();
        RoleGenManager.CrewSovereignRoles.Clear();
        RoleGenManager.CrewSupportRoles.Clear();
        CrewRoles.Clear();

        RoleGenManager.NeutralEvilRoles.Clear();
        RoleGenManager.NeutralBenignRoles.Clear();
        RoleGenManager.NeutralKillingRoles.Clear();
        RoleGenManager.NeutralNeophyteRoles.Clear();
        RoleGenManager.NeutralHarbingerRoles.Clear();
        NeutralRoles.Clear();

        RoleGenManager.IntruderHeadRoles.Clear();
        RoleGenManager.IntruderKillingRoles.Clear();
        RoleGenManager.IntruderSupportRoles.Clear();
        RoleGenManager.IntruderDeceptionRoles.Clear();
        RoleGenManager.IntruderConcealingRoles.Clear();
        IntruderRoles.Clear();

        RoleGenManager.SyndicateDisruptionRoles.Clear();
        RoleGenManager.SyndicateKillingRoles.Clear();
        RoleGenManager.SyndicateSupportRoles.Clear();
        RoleGenManager.SyndicatePowerRoles.Clear();
        SyndicateRoles.Clear();

        AllRoles.Clear();
        GetAdjustedFactions();
    }

    public override void EndFiltering()
    {
        var players = AllPlayers();

        if (!AllRoles.Any(x => x.ID == LayerEnum.Dracula))
        {
            var count = AllRoles.RemoveAll(x => x.ID == LayerEnum.VampireHunter);

            for (var i = 0; i < count; i++)
                AllRoles.Add(GetSpawnItem(LayerEnum.Vigilante));
        }

        if (!AllRoles.Any(x => x.ID is LayerEnum.Dracula or LayerEnum.Jackal or LayerEnum.Necromancer or LayerEnum.Whisperer))
        {
            var count = AllRoles.RemoveAll(x => x.ID == LayerEnum.Mystic);

            for (var i = 0; i < count; i++)
                AllRoles.Add(GetSpawnItem(LayerEnum.Seer));
        }

        if (!AllRoles.Any(x => x.ID is LayerEnum.VampireHunter or LayerEnum.Amnesiac or LayerEnum.Thief or LayerEnum.Godfather or LayerEnum.Shifter or LayerEnum.Guesser or LayerEnum.Rebel
            or LayerEnum.Executioner or LayerEnum.GuardianAngel or LayerEnum.BountyHunter or LayerEnum.Mystic or LayerEnum.Actor))
        {
            var count = AllRoles.RemoveAll(x => x.ID == LayerEnum.Seer);

            for (var i = 0; i < count; i++)
                AllRoles.Add(GetSpawnItem(LayerEnum.Sheriff));
        }

        if (AllRoles.Any(x => x.ID == LayerEnum.Cannibal) && AllRoles.Any(x => x.ID == LayerEnum.Janitor) && GameModifiers.JaniCanMutuallyExclusive)
        {
            var chance = URandom.RandomRangeInt(0, 2);
            var count = AllRoles.RemoveAll(x => x.ID == (chance == 0 ? LayerEnum.Cannibal : LayerEnum.Janitor));

            for (var i = 0; i < count; i++)
            {
                if (chance == 0)
                    AllRoles.Add(NeutralRoles.Random(x => x.ID != LayerEnum.Cannibal, GetSpawnItem(LayerEnum.Amnesiac)));
                else
                    AllRoles.Add(IntruderRoles.Random(x => x.ID != LayerEnum.Janitor, GetSpawnItem(LayerEnum.Impostor)));
            }
        }

        if (players.Count <= 4 && AllRoles.Any(x => x.ID == LayerEnum.Amnesiac))
        {
            var count = AllRoles.RemoveAll(x => x.ID == LayerEnum.Amnesiac);

            for (var i = 0; i < count; i++)
                AllRoles.Add(GetSpawnItem(LayerEnum.Thief));
        }
    }

    public override void Assign()
    {
        var players = AllPlayers();
        AllRoles.Shuffle();
        players.Shuffle();
        Message("Layers Sorted");

        if (TownOfUsReworked.IsTest)
        {
            var ids = "";

            foreach (var spawn in AllRoles)
                ids += $" {spawn.ID}";

            Message("Roles in the game: " + ids.Trim());
        }

        while (players.Any() && AllRoles.Any())
            Gen(players.TakeFirst(), AllRoles.TakeFirst().ID, PlayerLayerEnum.Role);

        Message("Role Spawn Done");
    }

    public virtual void InitCrewList() {}

    public virtual void InitNeutList() {}

    public virtual void InitIntList() {}

    public virtual void InitSynList() {}

    public void GetAdjustedFactions()
    {
        var players = GameData.Instance.PlayerCount;
        Intruders = IntruderSettings.IntruderCount;
        Syndicate = SyndicateSettings.SyndicateCount;
        Neutrals = IsKilling() ? GameModeSettings.NeutralsCount : URandom.RandomRangeInt(NeutralSettings.NeutralMin, NeutralSettings.NeutralMax + 1);

        if (Intruders == 0 && Syndicate == 0 && Neutrals == 0)
        {
            _ = URandom.RandomRangeInt(0, 3) switch
            {
                0 => Intruders++,
                1 => Syndicate++,
                _ => Neutrals++,
            };
        }

        while (Neutrals >= players - Intruders - Syndicate)
            Neutrals--;

        Crew = players - Intruders - Syndicate - Neutrals;

        while (Crew == 0 && (Intruders > 0 || Syndicate > 0 || Neutrals > 0) && players > 1)
        {
            var random2 = URandom.RandomRangeInt(0, 3);

            if (random2 == 0 && Intruders > 0)
            {
                Intruders--;
                Crew++;
            }
            else if (random2 == 1 && Syndicate > 0)
            {
                Syndicate--;
                Crew++;
            }
            else if (random2 == 2 && Neutrals > 0)
            {
                Neutrals--;
                Crew++;
            }
        }

        if (this is not AllAnyGen)
        {
            if (TownOfUsReworked.IsTest)
                Info($"Crew = {Crew}, Int = {Intruders}, Syn = {Syndicate}, Neut = {Neutrals}");

            return;
        }

        Intruders = GetRandomCount();
        Neutrals = GetRandomCount();
        Syndicate = GetRandomCount();

        if (Intruders == 0 && Syndicate == 0 && Neutrals == 0)
        {
            var random = URandom.RandomRangeInt(0, 3);

            if (random == 0)
                Intruders++;
            else if (random == 1)
                Syndicate++;
            else if (random == 2)
                Neutrals++;
        }

        Crew = players - Intruders - Syndicate - Neutrals;

        while (Crew == 0 && (Intruders > 0 || Syndicate > 0 || Neutrals > 0) && players > 1)
        {
            var random2 = URandom.RandomRangeInt(0, 3);

            if (random2 == 0 && Intruders > 0)
            {
                Intruders--;
                Crew++;
            }
            else if (random2 == 1 && Syndicate > 0)
            {
                Syndicate--;
                Crew++;
            }
            else if (random2 == 2 && Neutrals > 0)
            {
                Neutrals--;
                Crew++;
            }
        }

        if (TownOfUsReworked.IsTest)
            Info($"Crew = {Crew}, Int = {Intruders}, Syn = {Syndicate}, Neut = {Neutrals}");
    }
}