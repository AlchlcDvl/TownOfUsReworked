using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Hazel;
using Reactor.Utilities;
using TownOfUsReworked.PlayerLayers.Roles;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Modifiers;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Objectifiers.AlliedMod;
using TownOfUsReworked.PlayerLayers.Abilities.AssassinMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TimeLordMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.SwapperMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RevealerMod;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GuesserMod;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.PhantomMod;
using UnityEngine;
using TownOfUsReworked.BetterMaps.Airship;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.ConsigliereMod;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GodfatherMod;
using TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.RebelMod;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.TimeMasterMod;
using TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.DrunkardMod;
using TownOfUsReworked.PlayerLayers.Objectifiers.TraitorMod;
using Reactor.Networking.Extensions;
using AmongUs.GameOptions;
using TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.BansheeMod;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GhoulMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod;
using Coroutine = TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.JanitorMod.Coroutine;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using Eat = TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.CannibalMod.Coroutine;
using Revive = TownOfUsReworked.PlayerLayers.Roles.CrewRoles.AltruistMod.Coroutine;
using Resurrect = TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.NecromancerMod.Coroutine;
using RetRevive = TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod.Coroutine;
using PerformRemember = TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.AmnesiacMod.PerformRemember;
using RetStopKill = TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod.StopKill;
using MedStopKill = TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod.StopKill;
using PerformSteal = TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.ThiefMod.PerformSteal;
using PerformDeclare = TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GodfatherMod.PerformAbility;
using Coroutine2 = TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GodfatherMod.Coroutine;
using PerformSidekick = TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.RebelMod.PerformAbility;
using PerformShift = TownOfUsReworked.PlayerLayers.Roles.CrewRoles.ShifterMod.PerformShift;
using PerformConvert = TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.DraculaMod.PerformConvert;
using Mine = TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.MinerMod.PerformMine;
using Recruit = TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.JackalMod.PerformRecruit;

namespace TownOfUsReworked.Patches
{
    public static class RPCHandling
    {
        private static readonly List<(Type, int, int, bool)> CrewAuditorRoles = new();
        private static readonly List<(Type, int, int, bool)> CrewInvestigativeRoles = new();
        private static readonly List<(Type, int, int, bool)> CrewKillingRoles = new();
        private static readonly List<(Type, int, int, bool)> CrewProtectiveRoles = new();
        private static readonly List<(Type, int, int, bool)> CrewSovereignRoles = new();
        private static readonly List<(Type, int, int, bool)> CrewSupportRoles = new();
        private static readonly List<(Type, int, int, bool)> CrewRoles = new();

        private static readonly List<(Type, int, int, bool)> NeutralEvilRoles = new();
        private static readonly List<(Type, int, int, bool)> NeutralBenignRoles = new();
        private static readonly List<(Type, int, int, bool)> NeutralKillingRoles = new();
        private static readonly List<(Type, int, int, bool)> NeutralNeophyteRoles = new();
        private static readonly List<(Type, int, int, bool)> NeutralRoles = new();

        private static readonly List<(Type, int, int, bool)> IntruderDeceptionRoles = new();
        private static readonly List<(Type, int, int, bool)> IntruderConcealingRoles = new();
        private static readonly List<(Type, int, int, bool)> IntruderKillingRoles = new();
        private static readonly List<(Type, int, int, bool)> IntruderSupportRoles = new();
        private static readonly List<(Type, int, int, bool)> IntruderRoles = new();

        private static readonly List<(Type, int, int, bool)> SyndicateDisruptionRoles = new();
        private static readonly List<(Type, int, int, bool)> SyndicateKillingRoles = new();
        private static readonly List<(Type, int, int, bool)> SyndicatePowerRoles = new();
        private static readonly List<(Type, int, int, bool)> SyndicateSupportRoles = new();
        private static readonly List<(Type, int, int, bool)> SyndicateRoles = new();

        private static readonly List<(Type, int, int, bool)> AllModifiers = new();
        private static readonly List<(Type, int, int, bool)> AllAbilities = new();
        private static readonly List<(Type, int, int, bool)> AllObjectifiers = new();

        private static bool PhantomOn;
        private static bool RevealerOn;
        private static bool BansheeOn;
        private static bool GhoulOn;

        private static void Sort(List<(Type, int, int, bool)> items, int max, int min)
        {
            if (items.Count == 0)
                return;

            items.Shuffle();

            if (items.Count < max)
                max = items.Count;

            if (min > max)
                (max, min) = (min, max);

            if (items.Count < max)
                max = items.Count;

            if (min > items.Count)
                min = items.Count;

            if (min < 0)
                min = 0;

            var amount = Random.RandomRangeInt(min, max + 1);
            var tempList = new List<(Type, int, int, bool)>();

            foreach (var item in items)
            {
                var (_, chance, _, _) = (item.Item1, item.Item2, item.Item3, item.Item4);

                if (chance == 100)
                {
                    tempList.Add(item);
                }
                else
                {
                    var random = Random.RandomRangeInt(0, 100);

                    if (random < chance)
                        tempList.Add(item);
                }

                if (tempList.Count >= amount)
                    break;
            }

            tempList.Shuffle();
        }

        private static List<(Type, int, int, bool)> AASort(List<(Type, int, int, bool)> items, int amount)
        {
            var newList = new List<(Type, int, int, bool)>();

            while (newList.Count < amount && items.Count > 0)
            {
                items.Shuffle();
                newList.Add(items[0]);

                if (items[0].Item4 && CustomGameOptions.EnableUniques)
                    items.Remove(items[0]);

                newList.Shuffle();
            }

            return newList;
        }

        private static void GenVanilla(List<GameData.PlayerInfo> infected)
        {
            Utils.LogSomething("Role Gen Start");
            var impostors = Utils.GetImpostors(infected);
            var crewmates = Utils.GetCrewmates(impostors);

            #pragma warning disable
            var spawnList1 = new List<(Type, int, int, bool)>();
            var spawnList2 = new List<(Type, int, int, bool)>();
            #pragma warning restore

            CrewRoles.Clear();
            IntruderRoles.Clear();

            while (CrewRoles.Count < crewmates.Count)
                CrewRoles.Add((typeof(Crewmate), 100, 20, false));

            while (IntruderRoles.Count < impostors.Count)
            {
                if (CustomGameOptions.AltImps)
                    IntruderRoles.Add((typeof(Anarchist), 100, 57, false));
                else
                    IntruderRoles.Add((typeof(Impostor), 100, 52, false));
            }

            spawnList1 = CrewRoles;
            spawnList2 = IntruderRoles;

            Utils.LogSomething("Layers Sorted");

            while (impostors.Count > 0 && spawnList2.Count > 0)
            {
                var (type, _, id, _) = spawnList2.TakeFirst();
                Role.GenRole<Role>(type, impostors.TakeFirst(), id);
            }

            while (crewmates.Count > 0 && spawnList1.Count > 0)
            {
                var (type, _, id, _) = spawnList1.TakeFirst();
                Role.GenRole<Role>(type, crewmates.TakeFirst(), id);
            }

            Utils.LogSomething("Role Spawn Done");
        }

        private static void GenKilling(List<GameData.PlayerInfo> infected)
        {
            Utils.LogSomething("Role Gen Start");
            var impostors = Utils.GetImpostors(infected);
            var crewmates = Utils.GetCrewmates(impostors);

            #pragma warning disable
            var spawnList1 = new List<(Type, int, int, bool)>();
            var spawnList2 = new List<(Type, int, int, bool)>();
            #pragma warning restore

            CrewRoles.Clear();
            IntruderRoles.Clear();
            SyndicateRoles.Clear();
            NeutralKillingRoles.Clear();

            Utils.LogSomething("Lists Cleared - Killing Only");

            if (!CustomGameOptions.AltImps)
            {
                IntruderRoles.Clear();
                IntruderRoles.Add((typeof(Undertaker), CustomGameOptions.UndertakerOn, 41, CustomGameOptions.UniqueUndertaker));
                IntruderRoles.Add((typeof(Morphling), CustomGameOptions.MorphlingOn, 42, CustomGameOptions.UniqueMorphling));
                IntruderRoles.Add((typeof(Blackmailer), CustomGameOptions.BlackmailerOn, 43, CustomGameOptions.UniqueBlackmailer));
                IntruderRoles.Add((typeof(Miner), CustomGameOptions.MinerOn, 44, CustomGameOptions.UniqueMiner));
                IntruderRoles.Add((typeof(Wraith), CustomGameOptions.WraithOn, 46, CustomGameOptions.UniqueWraith));
                IntruderRoles.Add((typeof(Grenadier), CustomGameOptions.GrenadierOn, 50, CustomGameOptions.UniqueGrenadier));
                IntruderRoles.Add((typeof(Poisoner), CustomGameOptions.PoisonerOn, 51, CustomGameOptions.UniquePoisoner));
                IntruderRoles.Add((typeof(TimeMaster), CustomGameOptions.TimeMasterOn, 55, CustomGameOptions.UniqueTimeMaster));
                IntruderRoles.Add((typeof(Disguiser), CustomGameOptions.DisguiserOn, 54, CustomGameOptions.UniqueDisguiser));
                IntruderRoles.Add((typeof(Consigliere), CustomGameOptions.ConsigliereOn, 53, CustomGameOptions.UniqueConsigliere));
                IntruderRoles.Add((typeof(Consort), CustomGameOptions.ConsortOn, 47, CustomGameOptions.UniqueConsort));
                IntruderRoles.Add((typeof(Janitor), CustomGameOptions.JanitorOn, 48, CustomGameOptions.UniqueJanitor));
                IntruderRoles.Add((typeof(Camouflager), CustomGameOptions.CamouflagerOn, 49, CustomGameOptions.UniqueCamouflager));
                IntruderRoles.Add((typeof(Teleporter), CustomGameOptions.TeleporterOn, 45, CustomGameOptions.UniqueTeleporter));
                IntruderRoles.Add((typeof(Ambusher), CustomGameOptions.AmbusherOn, 75, CustomGameOptions.UniqueAmbusher));
                IntruderRoles.Add((typeof(Impostor), 5, 52, false));

                if (CustomGameOptions.IntruderCount >= 3)
                    IntruderRoles.Add((typeof(Godfather), CustomGameOptions.GodfatherOn, 56, CustomGameOptions.UniqueGodfather));
            }

            SyndicateRoles.Add((typeof(Anarchist), 5, 57, false));
            SyndicateRoles.Add((typeof(Concealer), CustomGameOptions.ConcealerOn, 62, CustomGameOptions.UniqueConcealer));
            SyndicateRoles.Add((typeof(Shapeshifter), CustomGameOptions.ShapeshifterOn, 58, CustomGameOptions.UniqueShapeshifter));
            SyndicateRoles.Add((typeof(Warper), CustomGameOptions.WarperOn, 63, CustomGameOptions.UniqueWarper));
            SyndicateRoles.Add((typeof(Gorgon), CustomGameOptions.GorgonOn, 59, CustomGameOptions.UniqueGorgon));
            SyndicateRoles.Add((typeof(Bomber), CustomGameOptions.BomberOn, 64, CustomGameOptions.UniqueBomber));
            SyndicateRoles.Add((typeof(Framer), CustomGameOptions.FramerOn, 60, CustomGameOptions.UniqueFramer));
            SyndicateRoles.Add((typeof(Crusader), CustomGameOptions.CrusaderOn, 76, CustomGameOptions.UniqueCrusader));

            if (CustomGameOptions.SyndicateCount >= 3)
                SyndicateRoles.Add((typeof(Rebel), CustomGameOptions.RebelOn, 61, CustomGameOptions.UniqueRebel));

            NeutralKillingRoles.Add((typeof(Glitch), CustomGameOptions.GlitchOn, 27, CustomGameOptions.UniqueGlitch));
            NeutralKillingRoles.Add((typeof(Werewolf), CustomGameOptions.WerewolfOn, 30, CustomGameOptions.UniqueWerewolf));
            NeutralKillingRoles.Add((typeof(SerialKiller), CustomGameOptions.SerialKillerOn, 35, CustomGameOptions.UniqueSerialKiller));
            NeutralKillingRoles.Add((typeof(Juggernaut), CustomGameOptions.JuggernautOn, 36, CustomGameOptions.UniqueJuggernaut));
            NeutralKillingRoles.Add((typeof(Murderer), CustomGameOptions.MurdererOn, 28, CustomGameOptions.UniqueMurderer));
            NeutralKillingRoles.Add((typeof(Thief), CustomGameOptions.ThiefOn, 38, CustomGameOptions.UniqueThief));

            if (CustomGameOptions.AddArsonist)
                NeutralKillingRoles.Add((typeof(Arsonist), CustomGameOptions.ArsonistOn, 31, CustomGameOptions.UniqueArsonist));

            if (CustomGameOptions.AddCryomaniac)
                NeutralKillingRoles.Add((typeof(Cryomaniac), CustomGameOptions.CryomaniacOn, 29, CustomGameOptions.UniqueCryomaniac));

            if (CustomGameOptions.AddPlaguebearer)
            {
                if (CustomGameOptions.PestSpawn)
                    NeutralKillingRoles.Add((typeof(Pestilence), CustomGameOptions.PlaguebearerOn, 33, CustomGameOptions.UniquePlaguebearer));
                else
                    NeutralKillingRoles.Add((typeof(Plaguebearer), CustomGameOptions.PlaguebearerOn, 34, CustomGameOptions.UniquePlaguebearer));
            }

            NeutralKillingRoles.Shuffle();

            int vigis = (crewmates.Count - NeutralKillingRoles.Count - (CustomGameOptions.AltImps ? 0 : CustomGameOptions.SyndicateCount)) / 2;
            int vets = (crewmates.Count - NeutralKillingRoles.Count - (CustomGameOptions.AltImps ? 0 : CustomGameOptions.SyndicateCount)) / 2;

            while (vigis > 0 || vets > 0)
            {
                if (vigis > 0)
                {
                    CrewRoles.Add((typeof(Vigilante), 100, 3, false));
                    vigis--;
                }

                if (vets > 0)
                {
                    CrewRoles.Add((typeof(Veteran), 100, 11, false));
                    vets--;
                }
            }

            Utils.LogSomething("Lists Set - Killing Only");

            var nonIntruderRoles = new List<(Type, int, int, bool)>();

            nonIntruderRoles.AddRange(CrewRoles);
            nonIntruderRoles.AddRange(NeutralKillingRoles);

            if (!CustomGameOptions.AltImps)
            {
                nonIntruderRoles.AddRange(SyndicateRoles);
            }
            else
            {
                IntruderRoles.Clear();
                IntruderRoles.AddRange(SyndicateRoles);
            }

            Utils.LogSomething("Ranges Set");

            //Hoping it doesn't come to this
            while (nonIntruderRoles.Count < crewmates.Count)
                nonIntruderRoles.Add((typeof(Crewmate), 100, 20, false));

            while (IntruderRoles.Count < impostors.Count)
            {
                if (CustomGameOptions.AltImps)
                    IntruderRoles.Add((typeof(Anarchist), 100, 57, false));
                else
                    IntruderRoles.Add((typeof(Impostor), 100, 52, false));
            }

            Utils.LogSomething("Default Roles Entered");

            Sort(nonIntruderRoles, crewmates.Count, crewmates.Count);
            Sort(IntruderRoles, impostors.Count, impostors.Count);

            Utils.LogSomething("Killing Role List Sorted");

            spawnList1 = nonIntruderRoles;
            spawnList2 = IntruderRoles;

            Utils.LogSomething("Layers Sorted");

            while (impostors.Count > 0 && spawnList2.Count > 0)
            {
                var (type, _, id, _) = spawnList2.TakeFirst();
                Role.GenRole<Role>(type, impostors.TakeFirst(), id);
            }

            while (crewmates.Count > 0 && spawnList1.Count > 0)
            {
                var (type, _, id, _) = spawnList1.TakeFirst();
                Role.GenRole<Role>(type, crewmates.TakeFirst(), id);
            }

            Utils.LogSomething("Role Spawn Done");
        }

        private static void GenClassicCustomAA(List<GameData.PlayerInfo> infected)
        {
            Utils.LogSomething("Role Gen Start");
            var impostors = Utils.GetImpostors(infected);
            var crewmates = Utils.GetCrewmates(impostors);
            var allCount = crewmates.Count + impostors.Count;

            #pragma warning disable
            var spawnList1 = new List<(Type, int, int, bool)>();
            var spawnList2 = new List<(Type, int, int, bool)>();
            #pragma warning restore

            crewmates.Shuffle();
            impostors.Shuffle();

            CrewAuditorRoles.Clear();
            CrewInvestigativeRoles.Clear();
            CrewKillingRoles.Clear();
            CrewProtectiveRoles.Clear();
            CrewSovereignRoles.Clear();
            CrewSupportRoles.Clear();
            CrewRoles.Clear();

            NeutralEvilRoles.Clear();
            NeutralBenignRoles.Clear();
            NeutralKillingRoles.Clear();
            NeutralNeophyteRoles.Clear();
            NeutralRoles.Clear();

            IntruderDeceptionRoles.Clear();
            IntruderConcealingRoles.Clear();
            IntruderKillingRoles.Clear();
            IntruderSupportRoles.Clear();
            IntruderRoles.Clear();

            SyndicateDisruptionRoles.Clear();
            SyndicateKillingRoles.Clear();
            SyndicateSupportRoles.Clear();
            SyndicatePowerRoles.Clear();
            SyndicateRoles.Clear();

            PhantomOn = Utils.Check(CustomGameOptions.PhantomOn);
            RevealerOn = Utils.Check(CustomGameOptions.RevealerOn);
            BansheeOn = Utils.Check(CustomGameOptions.BansheeOn);
            GhoulOn = Utils.Check(CustomGameOptions.GhoulOn);

            var num = 0;

            if (CustomGameOptions.MayorOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.MayorCount : 1;

                while (num > 0)
                {
                    CrewSovereignRoles.Add((typeof(Mayor), CustomGameOptions.MayorOn, 0, CustomGameOptions.UniqueMayor));
                    num--;
                }

                Utils.LogSomething("Mayor Done");
            }

            if (CustomGameOptions.SheriffOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.SheriffCount : 1;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add((typeof(Sheriff), CustomGameOptions.SheriffOn, 1, CustomGameOptions.UniqueSheriff));
                    num--;
                }

                Utils.LogSomething("Sheriff Done");
            }

            if (CustomGameOptions.InspectorOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.InspectorCount : 1;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add((typeof(Inspector), CustomGameOptions.InspectorOn, 2, CustomGameOptions.UniqueInspector));
                    num--;
                }

                Utils.LogSomething("Inspector Done");
            }

            if (CustomGameOptions.VigilanteOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.VigilanteCount : 1;

                while (num > 0)
                {
                    CrewKillingRoles.Add((typeof(Vigilante), CustomGameOptions.VigilanteOn, 3, CustomGameOptions.UniqueVigilante));
                    num--;
                }

                Utils.LogSomething("Vigilante Done");
            }

            if (CustomGameOptions.EngineerOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.EngineerCount : 1;

                while (num > 0)
                {
                    CrewSupportRoles.Add((typeof(Engineer), CustomGameOptions.EngineerOn, 4, CustomGameOptions.UniqueEngineer));
                    num--;
                }

                Utils.LogSomething("Engineer Done");
            }

            if (CustomGameOptions.SwapperOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.SwapperCount : 1;

                while (num > 0)
                {
                    CrewSovereignRoles.Add((typeof(Swapper), CustomGameOptions.SwapperOn, 5, CustomGameOptions.UniqueSwapper));
                    num--;
                }

                Utils.LogSomething("Swapper Done");
            }

            if (CustomGameOptions.TimeLordOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.TimeLordCount : 1;

                while (num > 0)
                {
                    CrewProtectiveRoles.Add((typeof(TimeLord), CustomGameOptions.TimeLordOn, 7, CustomGameOptions.UniqueTimeLord));
                    num--;
                }

                Utils.LogSomething("Time Lord Done");
            }

            if (CustomGameOptions.MedicOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.MedicCount : 1;

                while (num > 0)
                {
                    CrewProtectiveRoles.Add((typeof(Medic), CustomGameOptions.MedicOn, 8, CustomGameOptions.UniqueMedic));
                    num--;
                }

                Utils.LogSomething("Medic Done");
            }

            if (CustomGameOptions.AgentOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.AgentCount : 1;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add((typeof(Agent), CustomGameOptions.AgentOn, 9, CustomGameOptions.UniqueAgent));
                    num--;
                }

                Utils.LogSomething("Agent Done");
            }

            if (CustomGameOptions.AltruistOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.AltruistCount : 1;

                while (num > 0)
                {
                    CrewProtectiveRoles.Add((typeof(Altruist), CustomGameOptions.AltruistOn, 10, CustomGameOptions.UniqueAltruist));
                    num--;
                }

                Utils.LogSomething("Altruist Done");
            }

            if (CustomGameOptions.VeteranOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.VeteranCount : 1;

                while (num > 0)
                {
                    CrewKillingRoles.Add((typeof(Veteran), CustomGameOptions.VeteranOn, 11, CustomGameOptions.UniqueVeteran));
                    num--;
                }

                Utils.LogSomething("Veteran Done");
            }

            if (CustomGameOptions.TrackerOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.TrackerCount : 1;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add((typeof(Tracker), CustomGameOptions.TrackerOn, 12, CustomGameOptions.UniqueTracker));
                    num--;
                }

                Utils.LogSomething("Tracker Done");
            }

            if (CustomGameOptions.TransporterOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.TransporterCount : 1;

                while (num > 0)
                {
                    CrewSupportRoles.Add((typeof(Transporter), CustomGameOptions.TransporterOn, 13, CustomGameOptions.UniqueTransporter));
                    num--;
                }

                Utils.LogSomething("Transporter Done");
            }

            if (CustomGameOptions.MediumOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.MediumCount : 1;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add((typeof(Medium), CustomGameOptions.MediumOn, 14, CustomGameOptions.UniqueMedium));
                    num--;
                }

                Utils.LogSomething("Medium Done");
            }

            if (CustomGameOptions.CoronerOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.CoronerCount : 1;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add((typeof(Coroner), CustomGameOptions.CoronerOn, 15, CustomGameOptions.UniqueCoroner));
                    num--;
                }

                Utils.LogSomething("Coroner Done");
            }

            if (CustomGameOptions.OperativeOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.OperativeCount : 1;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add((typeof(Operative), CustomGameOptions.OperativeOn, 16, CustomGameOptions.UniqueOperative));
                    num--;
                }

                Utils.LogSomething("Operative Done");
            }

            if (CustomGameOptions.DetectiveOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.DetectiveCount : 1;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add((typeof(Detective), CustomGameOptions.DetectiveOn, 17, CustomGameOptions.UniqueDetective));
                    num--;
                }

                Utils.LogSomething("Detective Done");
            }

            if (CustomGameOptions.EscortOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.EscortCount : 1;

                while (num > 0)
                {
                    CrewSupportRoles.Add((typeof(Escort), CustomGameOptions.EscortOn, 18, CustomGameOptions.UniqueEscort));
                    num--;
                }

                Utils.LogSomething("Escort Done");
            }

            if (CustomGameOptions.ShifterOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.ShifterCount : 1;

                while (num > 0)
                {
                    CrewSupportRoles.Add((typeof(Shifter), CustomGameOptions.ShifterOn, 19, CustomGameOptions.UniqueShifter));
                    num--;
                }

                Utils.LogSomething("Shifter Done");
            }

            if (CustomGameOptions.ChameleonOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.ChameleonCount : 1;

                while (num > 0)
                {
                    CrewSupportRoles.Add((typeof(Chameleon), CustomGameOptions.ChameleonOn, 65, CustomGameOptions.UniqueChameleon));
                    num--;
                }

                Utils.LogSomething("Chameleon Done");
            }

            if (CustomGameOptions.RetributionistOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.RetributionistCount : 1;

                while (num > 0)
                {
                    CrewSupportRoles.Add((typeof(Retributionist), CustomGameOptions.RetributionistOn, 68, CustomGameOptions.UniqueRetributionist));
                    num--;
                }

                Utils.LogSomething("Chameleon Done");
            }

            if (CustomGameOptions.CrewmateOn > 0 && GameStates.IsCustom)
            {
                num = CustomGameOptions.CrewCount;

                while (num > 0)
                {
                    CrewRoles.Add((typeof(Crewmate), CustomGameOptions.CrewmateOn, 20, false));
                    num--;
                }

                Utils.LogSomething("Crewmate Done");
            }

            if (CustomGameOptions.VampireHunterOn > 0 && CustomGameOptions.DraculaOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.VampireHunterCount : 1;

                while (num > 0)
                {
                    CrewAuditorRoles.Add((typeof(VampireHunter), CustomGameOptions.VampireHunterOn, 21, CustomGameOptions.UniqueVampireHunter));
                    num--;
                }

                Utils.LogSomething("Vampire Hunter Done");
            }

            if (CustomGameOptions.MysticOn > 0 && (CustomGameOptions.DraculaOn > 0 || CustomGameOptions.NecromancerOn > 0 || CustomGameOptions.WhispererOn > 0 ||
                CustomGameOptions.JackalOn > 0))
            {
                num = GameStates.IsCustom ? CustomGameOptions.MysticCount : 1;

                while (num > 0)
                {
                    CrewAuditorRoles.Add((typeof(Mystic), CustomGameOptions.MysticOn, 71, CustomGameOptions.UniqueMystic));
                    num--;
                }

                Utils.LogSomething("Mystic Done");
            }

            if (CustomGameOptions.SeerOn > 0 && ((CustomGameOptions.VampireHunterOn > 0 && CustomGameOptions.DraculaOn > 0) || CustomGameOptions.BountyHunterOn > 0 ||
                CustomGameOptions.GodfatherOn > 0 || CustomGameOptions.RebelOn > 0 || CustomGameOptions.PlaguebearerOn > 0 || CustomGameOptions.MysticOn > 0 ||
                CustomGameOptions.TraitorOn > 0 || CustomGameOptions.AmnesiacOn > 0 || CustomGameOptions.ThiefOn > 0 || CustomGameOptions.ExecutionerOn > 0 ||
                CustomGameOptions.GuardianAngelOn > 0 || CustomGameOptions.GuesserOn > 0))
            {
                num = GameStates.IsCustom ? CustomGameOptions.SeerCount : 1;

                while (num > 0)
                {
                    CrewInvestigativeRoles.Add((typeof(Seer), CustomGameOptions.SeerOn, 72, CustomGameOptions.UniqueSeer));
                    num--;
                }

                Utils.LogSomething("Seer Done");
            }

            if (CustomGameOptions.JesterOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.JesterCount : 1;

                while (num > 0)
                {
                    NeutralEvilRoles.Add((typeof(Jester), CustomGameOptions.JesterOn, 22, CustomGameOptions.UniqueJester));
                    num--;
                }

                Utils.LogSomething("Jester Done");
            }

            if (CustomGameOptions.AmnesiacOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.AmnesiacCount : 1;

                while (num > 0)
                {
                    NeutralBenignRoles.Add((typeof(Amnesiac), CustomGameOptions.AmnesiacOn, 23, CustomGameOptions.UniqueAmnesiac));
                    num--;
                }

                Utils.LogSomething("Amnesiac Done");
            }

            if (CustomGameOptions.ExecutionerOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.ExecutionerCount : 1;

                while (num > 0)
                {
                    NeutralEvilRoles.Add((typeof(Executioner), CustomGameOptions.ExecutionerOn, 24, CustomGameOptions.UniqueExecutioner));
                    num--;
                }

                Utils.LogSomething("Executioner Done");
            }

            if (CustomGameOptions.SurvivorOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.SurvivorCount : 1;

                while (num > 0)
                {
                    NeutralBenignRoles.Add((typeof(Survivor), CustomGameOptions.SurvivorOn, 25, CustomGameOptions.UniqueSurvivor));
                    num--;
                }

                Utils.LogSomething("Survivor Done");
            }

            if (CustomGameOptions.GuardianAngelOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.GuardianAngelCount : 1;

                while (num > 0)
                {
                    NeutralBenignRoles.Add((typeof(GuardianAngel), CustomGameOptions.GuardianAngelOn, 26, CustomGameOptions.UniqueGuardianAngel));
                    num--;
                }

                Utils.LogSomething("Guardian Angel Done");
            }

            if (CustomGameOptions.GlitchOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.GlitchCount : 1;

                while (num > 0)
                {
                    NeutralKillingRoles.Add((typeof(Glitch), CustomGameOptions.GlitchOn, 27, CustomGameOptions.UniqueGlitch));
                    num--;
                }

                Utils.LogSomething("Glitch Done");
            }

            if (CustomGameOptions.MurdererOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.MurdCount : 1;

                while (num > 0)
                {
                    NeutralKillingRoles.Add((typeof(Murderer), CustomGameOptions.MurdererOn, 28, CustomGameOptions.UniqueMurderer));
                    num--;
                }

                Utils.LogSomething("Murderer Done");
            }

            if (CustomGameOptions.CryomaniacOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.CryomaniacCount : 1;

                while (num > 0)
                {
                    NeutralKillingRoles.Add((typeof(Cryomaniac), CustomGameOptions.CryomaniacOn, 29, CustomGameOptions.UniqueCryomaniac));
                    num--;
                }

                Utils.LogSomething("Cryomaniac Done");
            }

            if (CustomGameOptions.WerewolfOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.WerewolfCount : 1;

                while (num > 0)
                {
                    NeutralKillingRoles.Add((typeof(Werewolf), CustomGameOptions.WerewolfOn, 30, CustomGameOptions.UniqueWerewolf));
                    num--;
                }

                Utils.LogSomething("Werewolf Done");
            }

            if (CustomGameOptions.ArsonistOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.ArsonistCount : 1;

                while (num > 0)
                {
                    NeutralKillingRoles.Add((typeof(Arsonist), CustomGameOptions.ArsonistOn, 31, CustomGameOptions.UniqueArsonist));
                    num--;
                }

                Utils.LogSomething("Arsonist Done");
            }

            if (CustomGameOptions.JackalOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.JackalCount : 1;

                while (num > 0)
                {
                    NeutralNeophyteRoles.Add((typeof(Jackal), CustomGameOptions.JackalOn, 32, CustomGameOptions.UniqueJackal));
                    num--;
                }

                Utils.LogSomething("Jackal Done");
            }

            if (CustomGameOptions.NecromancerOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.NecromancerCount : 1;

                while (num > 0)
                {
                    NeutralNeophyteRoles.Add((typeof(Necromancer), CustomGameOptions.NecromancerOn, 73, CustomGameOptions.UniqueNecromancer));
                    num--;
                }

                Utils.LogSomething("Necromancer Done");
            }

            if (CustomGameOptions.PlaguebearerOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.PlaguebearerCount : 1;

                while (num > 0)
                {
                    if (CustomGameOptions.PestSpawn)
                        NeutralKillingRoles.Add((typeof(Pestilence), CustomGameOptions.PlaguebearerOn, 33, CustomGameOptions.UniquePlaguebearer));
                    else
                        NeutralKillingRoles.Add((typeof(Plaguebearer), CustomGameOptions.PlaguebearerOn, 34, CustomGameOptions.UniquePlaguebearer));

                    num--;
                }

                var PBorPest = CustomGameOptions.PestSpawn ? "Pestilence" : "Plaguebearer";

                Utils.LogSomething($"{PBorPest} Done");
            }

            if (CustomGameOptions.SerialKillerOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.SKCount : 1;

                while (num > 0)
                {
                    NeutralKillingRoles.Add((typeof(SerialKiller), CustomGameOptions.SerialKillerOn, 35, CustomGameOptions.UniqueSerialKiller));
                    num--;
                }

                Utils.LogSomething("Serial Killer Done");
            }

            if (CustomGameOptions.JuggernautOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.JuggernautCount : 1;

                while (num > 0)
                {
                    NeutralKillingRoles.Add((typeof(Juggernaut), CustomGameOptions.JuggernautOn, 36, CustomGameOptions.UniqueJuggernaut));
                    num--;
                }

                Utils.LogSomething("Juggeraut Done");
            }

            if (CustomGameOptions.CannibalOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.CannibalCount : 1;

                while (num > 0)
                {
                    NeutralEvilRoles.Add((typeof(Cannibal), CustomGameOptions.CannibalOn, 37, CustomGameOptions.UniqueCannibal));
                    num--;
                }

                Utils.LogSomething("Cannibal Done");
            }

            if (CustomGameOptions.GuesserOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.GuesserCount : 1;

                while (num > 0)
                {
                    NeutralEvilRoles.Add((typeof(Guesser), CustomGameOptions.GuesserOn, 66, CustomGameOptions.UniqueGuesser));
                    num--;
                }

                Utils.LogSomething("Guesser Done");
            }

            if (CustomGameOptions.ActorOn > 0 && (CustomGameOptions.CrewAssassinOn > 0 || CustomGameOptions.NeutralAssassinOn > 0 || CustomGameOptions.SyndicateAssassinOn > 0 ||
                CustomGameOptions.IntruderAssassinOn > 0))
            {
                num = GameStates.IsCustom ? CustomGameOptions.ActorCount : 1;

                while (num > 0)
                {
                    NeutralEvilRoles.Add((typeof(Actor), CustomGameOptions.ActorOn, 69, CustomGameOptions.UniqueActor));
                    num--;
                }

                Utils.LogSomething("Actor Done");
            }

            if (CustomGameOptions.ThiefOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.ThiefCount : 1;

                while (num > 0)
                {
                    NeutralBenignRoles.Add((typeof(Thief), CustomGameOptions.ThiefOn, 38, CustomGameOptions.UniqueThief));
                    num--;
                }

                Utils.LogSomething("Thief Done");
            }

            if (CustomGameOptions.DraculaOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.DraculaCount : 1;

                while (num > 0)
                {
                    NeutralNeophyteRoles.Add((typeof(Dracula), CustomGameOptions.DraculaOn, 39, CustomGameOptions.UniqueDracula));
                    num--;
                }

                Utils.LogSomething("Dracula Done");
            }

            if (CustomGameOptions.WhispererOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.WhispererCount : 1;

                while (num > 0)
                {
                    NeutralNeophyteRoles.Add((typeof(Whisperer), CustomGameOptions.WhispererOn, 67, CustomGameOptions.UniqueWhisperer));
                    num--;
                }

                Utils.LogSomething("Whisperer Done");
            }

            if (CustomGameOptions.TrollOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.TrollCount : 1;

                while (num > 0)
                {
                    NeutralEvilRoles.Add((typeof(Troll), CustomGameOptions.TrollOn, 40, CustomGameOptions.UniqueTroll));
                    num--;
                }

                Utils.LogSomething("Troll Done");
            }

            if (CustomGameOptions.BountyHunterOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.BHCount : 1;

                while (num > 0)
                {
                    NeutralEvilRoles.Add((typeof(BountyHunter), CustomGameOptions.BountyHunterOn, 70, CustomGameOptions.UniqueBountyHunter));
                    num--;
                }

                Utils.LogSomething("Bounty Hunter Done");
            }

            if (CustomGameOptions.UndertakerOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.UndertakerCount : 1;

                while (num > 0)
                {
                    IntruderConcealingRoles.Add((typeof(Undertaker), CustomGameOptions.UndertakerOn, 41, CustomGameOptions.UniqueUndertaker));
                    num--;
                }

                Utils.LogSomething("Undertaker Done");
            }

            if (CustomGameOptions.MorphlingOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.MorphlingCount : 1;

                while (num > 0)
                {
                    IntruderDeceptionRoles.Add((typeof(Morphling), CustomGameOptions.MorphlingOn, 42, CustomGameOptions.UniqueMorphling));
                    num--;
                }

                Utils.LogSomething("Morphling Done");
            }

            if (CustomGameOptions.BlackmailerOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.BlackmailerCount : 1;

                while (num > 0)
                {
                    IntruderConcealingRoles.Add((typeof(Blackmailer), CustomGameOptions.BlackmailerOn, 43, CustomGameOptions.UniqueBlackmailer));
                    num--;
                }

                Utils.LogSomething("Blackmailer Done");
            }

            if (CustomGameOptions.MinerOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.MinerCount : 1;

                while (num > 0)
                {
                    IntruderSupportRoles.Add((typeof(Miner), CustomGameOptions.MinerOn, 44, CustomGameOptions.UniqueMiner));
                    num--;
                }

                Utils.LogSomething("Miner Done");
            }

            if (CustomGameOptions.TeleporterOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.TeleporterCount : 1;

                while (num > 0)
                {
                    IntruderSupportRoles.Add((typeof(Teleporter), CustomGameOptions.TeleporterOn, 45, CustomGameOptions.UniqueTeleporter));
                    num--;
                }

                Utils.LogSomething("Teleporter Done");
            }

            if (CustomGameOptions.AmbusherOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.AmbusherCount : 1;

                while (num > 0)
                {
                    IntruderKillingRoles.Add((typeof(Ambusher), CustomGameOptions.AmbusherOn, 75, CustomGameOptions.UniqueAmbusher));
                    num--;
                }

                Utils.LogSomething("Ambusher Done");
            }

            if (CustomGameOptions.WraithOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.WraithCount : 1;

                while (num > 0)
                {
                    IntruderDeceptionRoles.Add((typeof(Wraith), CustomGameOptions.WraithOn, 46, CustomGameOptions.UniqueWraith));
                    num--;
                }

                Utils.LogSomething("Wraith Done");
            }

            if (CustomGameOptions.ConsortOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.ConsortCount : 1;

                while (num > 0)
                {
                    IntruderSupportRoles.Add((typeof(Consort), CustomGameOptions.ConsortOn, 47, CustomGameOptions.UniqueConsort));
                    num--;
                }

                Utils.LogSomething("Consort Done");
            }

            if (CustomGameOptions.JanitorOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.JanitorCount : 1;

                while (num > 0)
                {
                    IntruderConcealingRoles.Add((typeof(Janitor), CustomGameOptions.JanitorOn, 48, CustomGameOptions.UniqueJanitor));
                    num--;
                }

                Utils.LogSomething("Janitor Done");
            }

            if (CustomGameOptions.CamouflagerOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.CamouflagerCount : 1;

                while (num > 0)
                {
                    IntruderConcealingRoles.Add((typeof(Camouflager), CustomGameOptions.CamouflagerOn, 49, CustomGameOptions.UniqueCamouflager));
                    num--;
                }

                Utils.LogSomething("Camouflager Done");
            }

            if (CustomGameOptions.GrenadierOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.GrenadierCount : 1;

                while (num > 0)
                {
                    IntruderConcealingRoles.Add((typeof(Grenadier), CustomGameOptions.GrenadierOn, 50, CustomGameOptions.UniqueGrenadier));
                    num--;
                }

                Utils.LogSomething("Grenadier Done");
            }

            if (CustomGameOptions.ImpostorOn > 0 && GameStates.IsCustom)
            {
                num = CustomGameOptions.ImpCount;

                while (num > 0)
                {
                    IntruderRoles.Add((typeof(Impostor), CustomGameOptions.ImpostorOn, 52, false));
                    num--;
                }

                Utils.LogSomething("Impostor Done");
            }

            if (CustomGameOptions.ConsigliereOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.ConsigliereCount : 1;

                while (num > 0)
                {
                    IntruderSupportRoles.Add((typeof(Consigliere), CustomGameOptions.ConsigliereOn, 53, CustomGameOptions.UniqueConsigliere));
                    num--;
                }

                Utils.LogSomething("Consigliere Done");
            }

            if (CustomGameOptions.DisguiserOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.DisguiserCount : 1;

                while (num > 0)
                {
                    IntruderDeceptionRoles.Add((typeof(Disguiser), CustomGameOptions.DisguiserOn, 54, CustomGameOptions.UniqueDisguiser));
                    num--;
                }

                Utils.LogSomething("Disguiser Done");
            }

            if (CustomGameOptions.TimeMasterOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.TimeMasterCount : 1;

                while (num > 0)
                {
                    IntruderSupportRoles.Add((typeof(TimeMaster), CustomGameOptions.TimeMasterOn, 55, CustomGameOptions.UniqueTimeMaster));
                    num--;
                }

                Utils.LogSomething("Time Master Done");
            }

            if (CustomGameOptions.GodfatherOn > 0 && CustomGameOptions.IntruderCount >= 3)
            {
                num = GameStates.IsCustom ? CustomGameOptions.GodfatherCount : 1;

                while (num > 0)
                {
                    IntruderSupportRoles.Add((typeof(Godfather), CustomGameOptions.GodfatherOn, 56, CustomGameOptions.UniqueGodfather));
                    num--;
                }

                Utils.LogSomething("Godfather Done");
            }

            if (CustomGameOptions.AnarchistOn > 0 && GameStates.IsCustom)
            {
                num = CustomGameOptions.AnarchistCount;

                while (num > 0)
                {
                    SyndicateRoles.Add((typeof(Anarchist), CustomGameOptions.AnarchistOn, 57, false));
                    num--;
                }

                Utils.LogSomething("Anarchist Done");
            }

            if (CustomGameOptions.ShapeshifterOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.ShapeshifterCount : 1;

                while (num > 0)
                {
                    SyndicateDisruptionRoles.Add((typeof(Shapeshifter), CustomGameOptions.ShapeshifterOn, 58, CustomGameOptions.UniqueShapeshifter));
                    num--;
                }

                Utils.LogSomething("Shapeshifter Done");
            }

            if (CustomGameOptions.GorgonOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.GorgonCount : 1;

                while (num > 0)
                {
                    SyndicateKillingRoles.Add((typeof(Gorgon), CustomGameOptions.GorgonOn, 59, CustomGameOptions.UniqueGorgon));
                    num--;
                }

                Utils.LogSomething("Gorgon Done");
            }

            if (CustomGameOptions.FramerOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.FramerCount : 1;

                while (num > 0)
                {
                    SyndicateDisruptionRoles.Add((typeof(Framer), CustomGameOptions.FramerOn, 60, CustomGameOptions.UniqueFramer));
                    num--;
                }

                Utils.LogSomething("Framer Done");
            }

            if (CustomGameOptions.CrusaderOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.CrusaderCount : 1;

                while (num > 0)
                {
                    SyndicateKillingRoles.Add((typeof(Crusader), CustomGameOptions.CrusaderOn, 76, CustomGameOptions.UniqueCrusader));
                    num--;
                }

                Utils.LogSomething("Crusader Done");
            }

            if (CustomGameOptions.RebelOn > 0 && CustomGameOptions.SyndicateCount >= 3)
            {
                num = GameStates.IsCustom ? CustomGameOptions.RebelCount : 1;

                while (num > 0)
                {
                    SyndicateSupportRoles.Add((typeof(Rebel), CustomGameOptions.RebelOn, 61, CustomGameOptions.UniqueRebel));
                    num--;
                }

                Utils.LogSomething("Rebel Done");
            }

            if (CustomGameOptions.BeamerOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.BeamerCount : 1;

                while (num > 0)
                {
                    SyndicateSupportRoles.Add((typeof(Beamer), CustomGameOptions.BeamerOn, 74, CustomGameOptions.UniqueBeamer));
                    num--;
                }

                Utils.LogSomething("Beamer Done");
            }

            if (CustomGameOptions.PoisonerOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.PoisonerCount : 1;

                while (num > 0)
                {
                    SyndicateDisruptionRoles.Add((typeof(Poisoner), CustomGameOptions.PoisonerOn, 51, CustomGameOptions.UniquePoisoner));
                    num--;
                }

                Utils.LogSomething("Poisoner Done");
            }

            if (CustomGameOptions.ConcealerOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.ConcealerCount : 1;

                while (num > 0)
                {
                    SyndicateDisruptionRoles.Add((typeof(Concealer), CustomGameOptions.ConcealerOn, 62, CustomGameOptions.UniqueConcealer));
                    num--;
                }

                Utils.LogSomething("Concealer Done");
            }

            if (CustomGameOptions.WarperOn > 0 && GameOptionsManager.Instance.currentNormalGameOptions.MapId != 4 && GameOptionsManager.Instance.currentNormalGameOptions.MapId != 5)
            {
                num = GameStates.IsCustom ? CustomGameOptions.WarperCount : 1;

                while (num > 0)
                {
                    SyndicateSupportRoles.Add((typeof(Warper), CustomGameOptions.WarperOn, 63, CustomGameOptions.UniqueWarper));
                    num--;
                }

                Utils.LogSomething("Warper Done");
            }

            if (CustomGameOptions.BomberOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.BomberCount : 1;

                while (num > 0)
                {
                    SyndicateKillingRoles.Add((typeof(Bomber), CustomGameOptions.BomberOn, 64, CustomGameOptions.UniqueBomber));
                    num--;
                }

                Utils.LogSomething("Bomber Done");
            }

            if (CustomGameOptions.DrunkardOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.DrunkardCount : 1;

                while (num > 0)
                {
                    SyndicateDisruptionRoles.Add((typeof(Drunkard), CustomGameOptions.DrunkardOn, 6, CustomGameOptions.UniqueDrunkard));
                    num--;
                }

                Utils.LogSomething("Drunkard Done");
            }

            if (GameStates.IsClassic || GameStates.IsCustom)
            {
                if (!CustomGameOptions.AltImps)
                {
                    var minIC = CustomGameOptions.ICMin;
                    var maxIC = CustomGameOptions.ICMax;
                    var minID = CustomGameOptions.IDMin;
                    var maxID = CustomGameOptions.IDMax;
                    var minIK = CustomGameOptions.IKMin;
                    var maxIK = CustomGameOptions.IKMax;
                    var minIS = CustomGameOptions.ISMin;
                    var maxIS = CustomGameOptions.ISMax;
                    var minInt = CustomGameOptions.IntruderMin;
                    var maxInt = CustomGameOptions.IntruderMax;
                    var maxIntSum = maxIC + maxID + maxIK + maxIS;
                    var minIntSum = minIC + minID + minIK + minIS;

                    while (maxIntSum > maxInt)
                    {
                        switch (Random.RandomRangeInt(0, 4))
                        {
                            case 0:
                                if (maxIC > 0) maxIC--;
                                break;

                            case 1:
                                if (maxID > 0) maxID--;
                                break;

                            case 2:
                                if (maxIK > 0) maxIK--;
                                break;

                            case 3:
                                if (maxIS > 0) maxIS--;
                                break;
                        }

                        maxIntSum = maxIC + maxID + maxIK + maxIS;
                    }

                    while (minIntSum > minInt)
                    {
                        switch (Random.RandomRangeInt(0, 4))
                        {
                            case 0:
                                if (minIC > 0) minIC--;
                                break;

                            case 1:
                                if (minID > 0) minID--;
                                break;

                            case 2:
                                if (minIK > 0) minIK--;
                                break;

                            case 3:
                                if (minIS > 0) minIS--;
                                break;
                        }

                        minIntSum = minIC + minID + minIK + minIS;
                    }

                    Sort(IntruderConcealingRoles, maxIC, minIC);
                    Sort(IntruderDeceptionRoles, maxID, minID);
                    Sort(IntruderKillingRoles, maxIK, minIK);
                    Sort(IntruderSupportRoles, maxIS, minIS);

                    IntruderRoles.AddRange(IntruderConcealingRoles);
                    IntruderRoles.AddRange(IntruderDeceptionRoles);
                    IntruderRoles.AddRange(IntruderKillingRoles);
                    IntruderRoles.AddRange(IntruderSupportRoles);

                    while (maxInt > CustomGameOptions.IntruderCount)
                        maxInt--;

                    while (minInt > CustomGameOptions.IntruderCount)
                        minInt--;

                    Sort(IntruderRoles, maxInt, minInt);

                    while (IntruderRoles.Count < CustomGameOptions.IntruderCount)
                        IntruderRoles.Add((typeof(Impostor), 100, 52, false));

                    IntruderRoles.Shuffle();
                }

                if (CustomGameOptions.NeutralMax > 0)
                {
                    var minNE = CustomGameOptions.NEMin;
                    var maxNE = CustomGameOptions.NEMax;
                    var minNB = CustomGameOptions.NBMin;
                    var maxNB = CustomGameOptions.NBMax;
                    var minNK = CustomGameOptions.NKMin;
                    var maxNK = CustomGameOptions.NKMax;
                    var minNN = CustomGameOptions.NNMin;
                    var maxNN = CustomGameOptions.NNMax;
                    var minNeut = CustomGameOptions.NeutralMin;
                    var maxNeut = CustomGameOptions.NeutralMax;
                    var maxNeutSum = maxNE + maxNB + maxNK + maxNN;
                    var minNeutSum = minNE + minNB + minNK + minNN;

                    while (maxNeutSum > maxNeut)
                    {
                        switch (Random.RandomRangeInt(0, 4))
                        {
                            case 0:
                                if (maxNE > 0) maxNE--;
                                break;

                            case 1:
                                if (maxNB > 0) maxNB--;
                                break;

                            case 2:
                                if (maxNK > 0) maxNK--;
                                break;

                            case 3:
                                if (maxNN > 0) maxNN--;
                                break;
                        }

                        maxNeutSum = maxNE + maxNB + maxNK + maxNN;
                    }

                    while (minNeutSum > minNeut)
                    {
                        switch (Random.RandomRangeInt(0, 4))
                        {
                            case 0:
                                if (minNE > 0) minNE--;
                                break;

                            case 1:
                                if (minNB > 0) minNB--;
                                break;

                            case 2:
                                if (minNK > 0) minNK--;
                                break;

                            case 3:
                                if (minNN > 0) minNN--;
                                break;
                        }

                        minNeutSum = minNE + minNB + minNK + minNN;
                    }

                    Sort(NeutralBenignRoles, maxNB, minNB);
                    Sort(NeutralEvilRoles, maxNE, minNE);
                    Sort(NeutralKillingRoles, maxNK, minNK);
                    Sort(NeutralNeophyteRoles, maxNN, minNN);

                    NeutralRoles.AddRange(NeutralBenignRoles);
                    NeutralRoles.AddRange(NeutralEvilRoles);
                    NeutralRoles.AddRange(NeutralKillingRoles);
                    NeutralRoles.AddRange(NeutralNeophyteRoles);

                    while (maxNeut >= crewmates.Count)
                        maxNeut--;

                    while (minNeut >= crewmates.Count)
                        minNeut--;

                    Sort(NeutralRoles, maxNeut, minNeut);
                }

                if (CustomGameOptions.SyndicateCount > 0)
                {
                    var minSSu = CustomGameOptions.SSuMin;
                    var maxSSu = CustomGameOptions.SSuMax;
                    var minSD = CustomGameOptions.SDMin;
                    var maxSD = CustomGameOptions.SDMax;
                    var minSyK = CustomGameOptions.SyKMin;
                    var maxSyK = CustomGameOptions.SyKMax;
                    var minSP = CustomGameOptions.SPMin;
                    var maxSP = CustomGameOptions.SPMax;
                    var minSyn = CustomGameOptions.SyndicateMin;
                    var maxSyn = CustomGameOptions.SyndicateMax;
                    var maxSynSum = maxSSu + maxSD + maxSyK + maxSP;
                    var minSynSum = minSSu + minSD + minSyK + minSP;

                    while (maxSynSum > maxSyn)
                    {
                        switch (Random.RandomRangeInt(0, 4))
                        {
                            case 0:
                                if (maxSSu > 0) maxSSu--;
                                break;

                            case 1:
                                if (maxSD > 0) maxSD--;
                                break;

                            case 2:
                                if (maxSyK > 0) maxSyK--;
                                break;

                            case 3:
                                if (maxSP > 0) maxSP--;
                                break;
                        }

                        maxSynSum = maxSSu + maxSD + maxSyK + maxSP;
                    }

                    while (minSynSum > minSyn)
                    {
                        switch (Random.RandomRangeInt(0, 4))
                        {
                            case 0:
                                if (minSSu > 0) minSSu--;
                                break;

                            case 1:
                                if (minSD > 0) minSD--;
                                break;

                            case 2:
                                if (minSyK > 0) minSyK--;
                                break;

                            case 3:
                                if (minSP > 0) minSP--;
                                break;
                        }

                        minSynSum = minSSu + minSD + minSyK + minSP;
                    }

                    Sort(SyndicateSupportRoles, maxSSu, minSSu);
                    Sort(SyndicateDisruptionRoles, maxSD, minSD);
                    Sort(SyndicateKillingRoles, maxSyK, minSyK);
                    Sort(SyndicatePowerRoles, maxSP, minSP);

                    SyndicateRoles.AddRange(SyndicateSupportRoles);
                    SyndicateRoles.AddRange(SyndicateDisruptionRoles);
                    SyndicateRoles.AddRange(SyndicateKillingRoles);
                    SyndicateRoles.AddRange(SyndicatePowerRoles);

                    while (maxSyn > CustomGameOptions.SyndicateCount || maxSyn >= crewmates.Count - NeutralRoles.Count)
                        maxSyn--;

                    while (minSyn > CustomGameOptions.SyndicateCount || minSyn >= crewmates.Count - NeutralRoles.Count)
                        minSyn--;

                    Sort(SyndicateRoles, maxSyn, minSyn);

                    while (SyndicateRoles.Count < CustomGameOptions.SyndicateCount)
                        SyndicateRoles.Add((typeof(Anarchist), 100, 57, false));

                    SyndicateRoles.Shuffle();
                }

                if (CustomGameOptions.CrewMax > 0)
                {
                    var minCI = CustomGameOptions.CIMin;
                    var maxCI = CustomGameOptions.CIMax;
                    var minCS = CustomGameOptions.CSMin;
                    var maxCS = CustomGameOptions.CSMax;
                    var minCA = CustomGameOptions.CAMin;
                    var maxCA = CustomGameOptions.CAMax;
                    var minCK = CustomGameOptions.CKMin;
                    var maxCK = CustomGameOptions.CKMax;
                    var minCP = CustomGameOptions.CPMin;
                    var maxCP = CustomGameOptions.CPMax;
                    var minCSv = CustomGameOptions.CSvMin;
                    var maxCSv = CustomGameOptions.CSvMax;
                    var minCrew = CustomGameOptions.CrewMin;
                    var maxCrew = CustomGameOptions.CrewMax;
                    var maxCrewSum = maxCA + maxCI + maxCK + maxCP + maxCS + maxCSv;
                    var minCrewSum = minCA + minCI + minCK + minCP + minCS + minCSv;

                    while (maxCrewSum > maxCrew)
                    {
                        switch (Random.RandomRangeInt(0, 6))
                        {
                            case 0:
                                if (maxCA > 0) maxCA--;
                                break;

                            case 1:
                                if (maxCI > 0) maxCI--;
                                break;

                            case 2:
                                if (maxCK > 0) maxCK--;
                                break;

                            case 3:
                                if (maxCS > 0) maxCS--;
                                break;

                            case 4:
                                if (maxCSv > 0) maxCSv--;
                                break;

                            case 5:
                                if (maxCP > 0) maxCP--;
                                break;
                        }

                        maxCrewSum = maxCA + maxCI + maxCK + maxCP + maxCS + maxCSv;
                    }

                    while (minCrewSum > minCrew)
                    {
                        var random = Random.RandomRangeInt(0, 6);

                        switch (random)
                        {
                            case 0:
                                if (minCA > 0) minCA--;
                                break;

                            case 1:
                                if (minCI > 0) minCI--;
                                break;

                            case 2:
                                if (minCK > 0) minCK--;
                                break;

                            case 3:
                                if (minCP > 0) minCP--;
                                break;

                            case 4:
                                if (minCS > 0) minCS--;
                                break;

                            case 5:
                                if (minCSv > 0) minCSv--;
                                break;
                        }

                        minCrewSum = minCA + minCI + minCK + minCP + minCS + minCSv;
                    }

                    Sort(CrewAuditorRoles, maxCA, minCA);
                    Sort(CrewInvestigativeRoles, maxCI, minCI);
                    Sort(CrewKillingRoles, maxCK, minCK);
                    Sort(CrewProtectiveRoles, maxCP, minCP);
                    Sort(CrewSupportRoles, maxCS, minCS);
                    Sort(CrewSovereignRoles, maxCSv, minCSv);

                    CrewRoles.AddRange(CrewAuditorRoles);
                    CrewRoles.AddRange(CrewInvestigativeRoles);
                    CrewRoles.AddRange(CrewKillingRoles);
                    CrewRoles.AddRange(CrewSupportRoles);
                    CrewRoles.AddRange(CrewProtectiveRoles);
                    CrewRoles.AddRange(CrewSovereignRoles);

                    while (maxCrew > crewmates.Count - SyndicateRoles.Count - NeutralRoles.Count)
                        maxCrew--;

                    while (minCrew > crewmates.Count - SyndicateRoles.Count - NeutralRoles.Count)
                        minCrew--;

                    Sort(CrewRoles, maxCrew, minCrew);

                    while (CrewRoles.Count < crewmates.Count - CustomGameOptions.SyndicateCount - NeutralRoles.Count)
                        CrewRoles.Add((typeof(Crewmate), 100, 20, false));

                    CrewRoles.Shuffle();
                }

                Utils.LogSomething("Classic/Custom Sorting Done");
            }
            else if (GameStates.IsAA)
            {
                CrewRoles.AddRange(CrewAuditorRoles);
                CrewRoles.AddRange(CrewInvestigativeRoles);
                CrewRoles.AddRange(CrewKillingRoles);
                CrewRoles.AddRange(CrewSupportRoles);
                CrewRoles.AddRange(CrewProtectiveRoles);
                CrewRoles.AddRange(CrewSovereignRoles);

                SyndicateRoles.AddRange(SyndicateSupportRoles);
                SyndicateRoles.AddRange(SyndicateDisruptionRoles);
                SyndicateRoles.AddRange(SyndicateKillingRoles);
                SyndicateRoles.AddRange(SyndicatePowerRoles);

                NeutralRoles.AddRange(NeutralBenignRoles);
                NeutralRoles.AddRange(NeutralEvilRoles);
                NeutralRoles.AddRange(NeutralKillingRoles);
                NeutralRoles.AddRange(NeutralNeophyteRoles);

                if (!CustomGameOptions.AltImps)
                {
                    IntruderRoles.AddRange(IntruderConcealingRoles);
                    IntruderRoles.AddRange(IntruderDeceptionRoles);
                    IntruderRoles.AddRange(IntruderKillingRoles);
                    IntruderRoles.AddRange(IntruderSupportRoles);
                }
                else
                {
                    IntruderRoles.Clear();
                    IntruderRoles.AddRange(SyndicateRoles);
                }

                CrewRoles.Shuffle();
                SyndicateRoles.Shuffle();
                IntruderRoles.Shuffle();
                NeutralRoles.Shuffle();

                Utils.LogSomething("All Any Sorting Done");
            }

            var NonIntruderRoles = new List<(Type, int, int, bool)>();

            NonIntruderRoles.AddRange(CrewRoles);
            NonIntruderRoles.AddRange(NeutralRoles);

            if (!CustomGameOptions.AltImps)
            {
                NonIntruderRoles.AddRange(SyndicateRoles);
            }
            else
            {
                IntruderRoles.Clear();
                IntruderRoles.AddRange(SyndicateRoles);
            }

            spawnList1 = GameStates.IsAA ? AASort(NonIntruderRoles, crewmates.Count) : NonIntruderRoles;
            spawnList2 = GameStates.IsAA ? AASort(IntruderRoles, impostors.Count) : IntruderRoles;

            while (spawnList1.Count < crewmates.Count)
                spawnList1.Add((typeof(Crewmate), 100, 20, false));

            while (spawnList2.Count < impostors.Count)
            {
                if (CustomGameOptions.AltImps)
                    spawnList2.Add((typeof(Anarchist), 100, 57, false));
                else
                    spawnList2.Add((typeof(Impostor), 100, 52, false));
            }

            if (!spawnList1.Contains((typeof(Dracula), CustomGameOptions.DraculaOn, 39, CustomGameOptions.UniqueDracula)))
            {
                while (spawnList1.Contains((typeof(VampireHunter), CustomGameOptions.VampireHunterOn, 21, CustomGameOptions.UniqueVampireHunter)))
                {
                    spawnList1.Replace((typeof(VampireHunter), CustomGameOptions.VampireHunterOn, 21, CustomGameOptions.UniqueVampireHunter), (typeof(Vigilante),
                        CustomGameOptions.VampireHunterOn, 3, CustomGameOptions.UniqueVampireHunter));
                }

                spawnList1.Shuffle();
            }

            if (!(spawnList1.Contains((typeof(Dracula), CustomGameOptions.DraculaOn, 39, CustomGameOptions.UniqueDracula)) ||
                spawnList1.Contains((typeof(Jackal), CustomGameOptions.JackalOn, 32, CustomGameOptions.UniqueJackal)) ||
                spawnList1.Contains((typeof(Whisperer), CustomGameOptions.WhispererOn, 67, CustomGameOptions.UniqueWhisperer)) ||
                spawnList1.Contains((typeof(Necromancer), CustomGameOptions.NecromancerOn, 73, CustomGameOptions.UniqueNecromancer))))
            {
                while (spawnList1.Contains((typeof(Mystic), CustomGameOptions.MysticOn, 71, CustomGameOptions.UniqueMystic)))
                {
                    spawnList1.Replace((typeof(Mystic), CustomGameOptions.MysticOn, 71, CustomGameOptions.UniqueMystic), (typeof(Seer), CustomGameOptions.MysticOn, 72,
                        CustomGameOptions.UniqueMystic));
                }

                spawnList1.Shuffle();
            }

            if (!(spawnList1.Contains((typeof(VampireHunter), CustomGameOptions.VampireHunterOn, 21, CustomGameOptions.UniqueVampireHunter)) ||
                spawnList1.Contains((typeof(Rebel), CustomGameOptions.RebelOn, 61, CustomGameOptions.UniqueRebel)) ||
                spawnList1.Contains((typeof(Plaguebearer), CustomGameOptions.PlaguebearerOn, 33, CustomGameOptions.UniquePlaguebearer)) ||
                spawnList2.Contains((typeof(Rebel), CustomGameOptions.RebelOn, 61, CustomGameOptions.UniqueRebel)) ||
                spawnList1.Contains((typeof(Amnesiac), CustomGameOptions.AmnesiacOn, 23, CustomGameOptions.UniqueAmnesiac)) ||
                spawnList1.Contains((typeof(Thief), CustomGameOptions.ThiefOn, 38, CustomGameOptions.UniqueThief)) ||
                spawnList1.Contains((typeof(GuardianAngel), CustomGameOptions.GuardianAngelOn, 26, CustomGameOptions.UniqueGuardianAngel)) ||
                spawnList1.Contains((typeof(Guesser), CustomGameOptions.GuesserOn, 66, CustomGameOptions.UniqueGuesser)) ||
                spawnList1.Contains((typeof(BountyHunter), CustomGameOptions.BountyHunterOn, 70, CustomGameOptions.UniqueBountyHunter)) ||
                spawnList1.Contains((typeof(Executioner), CustomGameOptions.ExecutionerOn, 24, CustomGameOptions.UniqueExecutioner)) ||
                spawnList2.Contains((typeof(Godfather), CustomGameOptions.GodfatherOn, 56, CustomGameOptions.UniqueGodfather))))
            {
                while (spawnList1.Contains((typeof(Seer), CustomGameOptions.SeerOn, 72, CustomGameOptions.UniqueSeer)))
                {
                    spawnList1.Replace((typeof(Seer), CustomGameOptions.SeerOn, 72, CustomGameOptions.UniqueSeer), (typeof(Sheriff), CustomGameOptions.SeerOn, 1,
                        CustomGameOptions.UniqueSeer));
                }

                while (spawnList1.Contains((typeof(Seer), CustomGameOptions.MysticOn, 72, CustomGameOptions.UniqueMystic)))
                {
                    spawnList1.Replace((typeof(Seer), CustomGameOptions.MysticOn, 72, CustomGameOptions.UniqueMystic), (typeof(Sheriff), CustomGameOptions.MysticOn, 1,
                        CustomGameOptions.UniqueMystic));
                }

                spawnList1.Shuffle();
            }

            spawnList1.Shuffle();
            spawnList2.Shuffle();

            Utils.LogSomething("Layers Sorted");

            while (impostors.Count > 0 && spawnList2.Count > 0)
            {
                var (type, _, id, unique) = spawnList2.TakeFirst();
                Role.GenRole<Role>(type, impostors.TakeFirst(), id);
            }

            while (crewmates.Count > 0 && spawnList1.Count > 0)
            {
                var (type, _, id, unique) = spawnList1.TakeFirst();
                Role.GenRole<Role>(type, crewmates.TakeFirst(), id);
            }

            Utils.LogSomething("Role Spawn Done");

            var toChooseFromNeut = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Neutral) && !x.Is(ObjectifierEnum.Allied)).ToList();
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetPhantom, SendOption.Reliable);

            if (PhantomOn && toChooseFromNeut.Count != 0)
            {
                var rand = Random.RandomRangeInt(0, toChooseFromNeut.Count);
                var pc = toChooseFromNeut[rand];
                SetPhantom.WillBePhantom = pc;
                writer.Write(pc.PlayerId);
            }
            else
            {
                writer.Write(byte.MaxValue);
            }

            AmongUsClient.Instance.FinishRpcImmediately(writer);

            var toChooseFromSyn = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Syndicate)).ToList();
            var writer3 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetBanshee, SendOption.Reliable);

            if (BansheeOn && toChooseFromSyn.Count != 0)
            {
                var rand = Random.RandomRangeInt(0, toChooseFromSyn.Count);
                var pc = toChooseFromSyn[rand];
                SetBanshee.WillBeBanshee = pc;
                writer3.Write(pc.PlayerId);
            }
            else
            {
                writer3.Write(byte.MaxValue);
            }

            AmongUsClient.Instance.FinishRpcImmediately(writer3);

            var toChooseFromCrew = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crew) && !x.Is(ObjectifierEnum.Traitor) && !x.Is(ObjectifierEnum.Corrupted) &&
                !x.Is(ObjectifierEnum.Fanatic)).ToList();
            var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetRevealer, SendOption.Reliable);

            if (RevealerOn && toChooseFromCrew.Count != 0)
            {
                var rand = Random.RandomRangeInt(0, toChooseFromCrew.Count);
                var pc = toChooseFromCrew[rand];
                SetRevealer.WillBeRevealer = pc;
                writer2.Write(pc.PlayerId);
            }
            else
            {
                writer2.Write(byte.MaxValue);
            }

            AmongUsClient.Instance.FinishRpcImmediately(writer2);

            var toChooseFromInt = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Intruder)).ToList();
            var writer4 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetGhoul, SendOption.Reliable);

            if (GhoulOn && toChooseFromInt.Count != 0)
            {
                var rand = Random.RandomRangeInt(0, toChooseFromInt.Count);
                var pc = toChooseFromInt[rand];
                SetGhoul.WillBeGhoul = pc;
                writer4.Write(pc.PlayerId);
            }
            else
            {
                writer4.Write(byte.MaxValue);
            }

            AmongUsClient.Instance.FinishRpcImmediately(writer4);

            Utils.LogSomething("Role Gen End");
        }

        private static void GenAbilities()
        {
            AllAbilities.Clear();
            var num = 0;

            if (CustomGameOptions.CrewAssassinOn > 0)
            {
                num = CustomGameOptions.NumberOfCrewAssassins;

                while (num > 0)
                {
                    AllAbilities.Add((typeof(Assassin), CustomGameOptions.CrewAssassinOn, 0, CustomGameOptions.UniqueAssassin));
                    num--;
                }

                Utils.LogSomething("Crew Assassin Done");
            }

            if (CustomGameOptions.SyndicateAssassinOn > 0)
            {
                num = CustomGameOptions.NumberOfSyndicateAssassins;

                while (num > 0)
                {
                    AllAbilities.Add((typeof(Assassin), CustomGameOptions.SyndicateAssassinOn, 12, CustomGameOptions.UniqueAssassin));
                    num--;
                }

                Utils.LogSomething("Syndicate Assassin Done");
            }

            if (CustomGameOptions.IntruderAssassinOn > 0)
            {
                num = CustomGameOptions.NumberOfImpostorAssassins;

                while (num > 0)
                {
                    AllAbilities.Add((typeof(Assassin), CustomGameOptions.IntruderAssassinOn, 11, CustomGameOptions.UniqueAssassin));
                    num--;
                }

                Utils.LogSomething("Intruder Assassin Done");
            }

            if (CustomGameOptions.NeutralAssassinOn > 0)
            {
                num = CustomGameOptions.NumberOfNeutralAssassins;

                while (num > 0)
                {
                    AllAbilities.Add((typeof(Assassin), CustomGameOptions.NeutralAssassinOn, 14, CustomGameOptions.UniqueAssassin));
                    num--;
                }

                Utils.LogSomething("Neutral Assassin Done");
            }

            if (CustomGameOptions.RuthlessOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.RuthlessCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((typeof(Ruthless), CustomGameOptions.RuthlessOn, 10, CustomGameOptions.UniqueRuthless));
                    num--;
                }

                Utils.LogSomething("Ruthless Done");
            }

            if (CustomGameOptions.SnitchOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.SnitchCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((typeof(Snitch), CustomGameOptions.SnitchOn, 1, CustomGameOptions.UniqueSnitch));
                    num--;
                }

                Utils.LogSomething("Snitch Done");
            }

            if (CustomGameOptions.InsiderOn > 0 && CustomGameOptions.AnonymousVoting)
            {
                num = GameStates.IsCustom ? CustomGameOptions.InsiderCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((typeof(Insider), CustomGameOptions.InsiderOn, 2, CustomGameOptions.UniqueInsider));
                    num--;
                }

                Utils.LogSomething("Insider Done");
            }

            if (CustomGameOptions.MultitaskerOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.MultitaskerCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((typeof(Multitasker), CustomGameOptions.MultitaskerOn, 4, CustomGameOptions.UniqueMultitasker));
                    num--;
                }

                Utils.LogSomething("Multitasker Done");
            }

            if (CustomGameOptions.RadarOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.RadarCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((typeof(Radar), CustomGameOptions.RadarOn, 5, CustomGameOptions.UniqueRadar));
                    num--;
                }

                Utils.LogSomething("Radar Done");
            }

            if (CustomGameOptions.TiebreakerOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.TiebreakerCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((typeof(Tiebreaker), CustomGameOptions.TiebreakerOn, 6, CustomGameOptions.UniqueTiebreaker));
                    num--;
                }

                Utils.LogSomething("Tiebreaker Done");
            }

            if (CustomGameOptions.TorchOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.TorchCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((typeof(Torch), CustomGameOptions.TorchOn, 7, CustomGameOptions.UniqueTorch));
                    num--;
                }

                Utils.LogSomething("Torch Done");
            }

            if (CustomGameOptions.UnderdogOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.UnderdogCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((typeof(Underdog), CustomGameOptions.UnderdogOn, 8, CustomGameOptions.UniqueUnderdog));
                    num--;
                }

                Utils.LogSomething("Underdog Done");
            }

            if (CustomGameOptions.TunnelerOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.TunnelerCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((typeof(Tunneler), CustomGameOptions.TunnelerOn, 9, CustomGameOptions.UniqueTunneler));
                    num--;
                }

                Utils.LogSomething("Tunneler Done");
            }

            if (CustomGameOptions.NinjaOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.NinjaCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((typeof(Ninja), CustomGameOptions.NinjaOn, 14, CustomGameOptions.UniqueNinja));
                    num--;
                }

                Utils.LogSomething("Ninja Done");
            }

            if (CustomGameOptions.ButtonBarryOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.ButtonBarryCount : 1;

                while (num > 0)
                {
                    AllAbilities.Add((typeof(ButtonBarry), CustomGameOptions.ButtonBarryOn, 15, CustomGameOptions.UniqueButtonBarry));
                    num--;
                }

                Utils.LogSomething("Button Barry Done");
            }

            var allCount = PlayerControl.AllPlayerControls.Count;
            Sort(AllAbilities, CustomGameOptions.MaxAbilities, CustomGameOptions.MinAbilities);
            AllAbilities.Shuffle();

            var canHaveIntruderAbility = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveNeutralAbility = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveCrewAbility = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveSyndicateAbility = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveTunnelerAbility = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveSnitch = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveTaskedAbility = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveNonEvilAbility = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveEvilAbility = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveKillingAbility = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveAbility = PlayerControl.AllPlayerControls.ToArray().ToList();

            canHaveIntruderAbility.RemoveAll(player => !player.Is(Faction.Intruder) || (player.Is(RoleEnum.Consigliere) && CustomGameOptions.ConsigInfo == ConsigInfo.Role));
            canHaveIntruderAbility.Shuffle();

            canHaveNeutralAbility.RemoveAll(player => !(player.Is(RoleAlignment.NeutralNeo) || player.Is(RoleAlignment.NeutralKill) || player.Is(RoleAlignment.NeutralPros)));
            canHaveNeutralAbility.Shuffle();

            canHaveCrewAbility.RemoveAll(player => !player.Is(Faction.Crew));
            canHaveCrewAbility.Shuffle();

            canHaveSyndicateAbility.RemoveAll(player => !player.Is(Faction.Syndicate));
            canHaveSyndicateAbility.Shuffle();

            canHaveTunnelerAbility.RemoveAll(player => !player.Is(Faction.Crew) || player.Is(RoleEnum.Engineer));
            canHaveTunnelerAbility.Shuffle();

            canHaveSnitch.RemoveAll(player => !player.Is(Faction.Crew) || player.Is(ObjectifierEnum.Traitor));
            canHaveSnitch.Shuffle();

            canHaveTaskedAbility.RemoveAll(player => !player.CanDoTasks());
            canHaveTaskedAbility.Shuffle();

            canHaveNonEvilAbility.RemoveAll(player => !(player.Is(Faction.Crew) || player.Is(RoleAlignment.NeutralBen) || player.Is(RoleAlignment.NeutralEvil)));
            canHaveNonEvilAbility.Shuffle();

            canHaveEvilAbility.RemoveAll(player => !(player.Is(Faction.Intruder) || player.Is(Faction.Syndicate)));
            canHaveEvilAbility.Shuffle();

            canHaveKillingAbility.RemoveAll(player => !(player.Is(Faction.Intruder) || player.Is(Faction.Syndicate) || player.Is(RoleAlignment.NeutralNeo) ||
                player.Is(RoleAlignment.NeutralKill)));
            canHaveKillingAbility.Shuffle();

            var spawnList = GameStates.IsAA ? AASort(AllAbilities, allCount) : AllAbilities;
            spawnList.Shuffle();

            while (canHaveSnitch.Count > 0 || (CustomGameOptions.WhoCanVent == WhoCanVentOptions.Default && canHaveTunnelerAbility.Count > 0) || canHaveIntruderAbility.Count > 0 ||
                canHaveNeutralAbility.Count > 0 || canHaveCrewAbility.Count > 0 || canHaveSyndicateAbility.Count > 0 || canHaveAbility.Count > 0 || canHaveEvilAbility.Count > 0 ||
                canHaveTaskedAbility.Count > 0 || canHaveNonEvilAbility.Count > 0 || canHaveKillingAbility.Count > 0)
            {
                if (spawnList.Count == 0)
                    break;

                var (type, _, id, unique) = spawnList.TakeFirst();
                int[] Snitch = { 1 };
                int[] Syndicate = { 12 };
                int[] Crew = { 0 };
                int[] Neutral = { 13 };
                int[] Intruder = { 11 };
                int[] Killing = { 10, 14 };
                int[] NonEvil = { 7 };
                int[] Evil = { 8 };
                int[] Tasked = { 2, 4 };
                int[] Global = { 5, 6, 15 };
                int[] Tunneler = { 9 };

                if (canHaveSnitch.Count > 0 && Snitch.Contains(id))
                    Ability.GenAbility<Ability>(type, canHaveSnitch.TakeFirst(), id);
                else if (canHaveSyndicateAbility.Count > 0 && Syndicate.Contains(id))
                    Ability.GenAbility<Ability>(type, canHaveSyndicateAbility.TakeFirst(), id);
                else if (canHaveCrewAbility.Count > 0 && Crew.Contains(id))
                    Ability.GenAbility<Ability>(type, canHaveCrewAbility.TakeFirst(), id);
                else if (canHaveNeutralAbility.Count > 0 && Neutral.Contains(id))
                    Ability.GenAbility<Ability>(type, canHaveNeutralAbility.TakeFirst(), id);
                else if (canHaveIntruderAbility.Count > 0 && Intruder.Contains(id))
                    Ability.GenAbility<Ability>(type, canHaveIntruderAbility.TakeFirst(), id);
                else if (canHaveKillingAbility.Count > 0 && Killing.Contains(id))
                    Ability.GenAbility<Ability>(type, canHaveKillingAbility.TakeFirst(), id);
                else if (canHaveNonEvilAbility.Count > 0 && NonEvil.Contains(id))
                    Ability.GenAbility<Ability>(type, canHaveNonEvilAbility.TakeFirst(), id);
                else if (canHaveEvilAbility.Count > 0 && Evil.Contains(id))
                    Ability.GenAbility<Ability>(type, canHaveEvilAbility.TakeFirst(), id);
                else if (canHaveTaskedAbility.Count > 0 && Tasked.Contains(id))
                    Ability.GenAbility<Ability>(type, canHaveTaskedAbility.TakeFirst(), id);
                else if (canHaveAbility.Count > 0 && Global.Contains(id))
                    Ability.GenAbility<Ability>(type, canHaveAbility.TakeFirst(), id);
                else if (canHaveTunnelerAbility.Count > 0 && Tunneler.Contains(id) && CustomGameOptions.WhoCanVent == WhoCanVentOptions.Default)
                    Ability.GenAbility<Ability>(type, canHaveTunnelerAbility.TakeFirst(), id);
            }

            Utils.LogSomething("Abilities Done");
        }

        private static void GenObjectifiers()
        {
            AllObjectifiers.Clear();
            var num = 0;

            if (CustomGameOptions.LoversOn > 0 && PlayerControl.AllPlayerControls.Count > 4)
            {
                num = GameStates.IsCustom ? CustomGameOptions.LoversCount : 1;

                while (num > 0)
                {
                    AllObjectifiers.Add((typeof(Lovers), CustomGameOptions.LoversOn, 0, CustomGameOptions.UniqueLovers));
                    num--;
                }

                Utils.LogSomething("Lovers Done");
            }

            if (CustomGameOptions.RivalsOn > 0 && PlayerControl.AllPlayerControls.Count > 4)
            {
                num = GameStates.IsCustom ? CustomGameOptions.RivalsCount : 1;

                while (num > 0)
                {
                    AllObjectifiers.Add((typeof(Rivals), CustomGameOptions.RivalsOn, 1, CustomGameOptions.UniqueRivals));
                    num--;
                }

                Utils.LogSomething("Rivals Done");
            }

            if (CustomGameOptions.FanaticOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.FanaticCount : 1;

                while (num > 0)
                {
                    AllObjectifiers.Add((typeof(Fanatic), CustomGameOptions.FanaticOn, 2, CustomGameOptions.UniqueFanatic));
                    num--;
                }

                Utils.LogSomething("Fanatic Done");
            }

            if (CustomGameOptions.CorruptedOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.CorruptedCount : 1;

                while (num > 0)
                {
                    AllObjectifiers.Add((typeof(Corrupted), CustomGameOptions.CorruptedOn, 3, CustomGameOptions.UniqueCorrupted));
                    num--;
                }

                Utils.LogSomething("Corrupted Done");
            }

            if (CustomGameOptions.OverlordOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.OverlordCount : 1;

                while (num > 0)
                {
                    AllObjectifiers.Add((typeof(Overlord), CustomGameOptions.OverlordOn, 4, CustomGameOptions.UniqueOverlord));
                    num--;
                }

                Utils.LogSomething("Overlord Done");
            }

            if (CustomGameOptions.AlliedOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.AlliedCount : 1;

                while (num > 0)
                {
                    AllObjectifiers.Add((typeof(Allied), CustomGameOptions.AlliedOn, 5, CustomGameOptions.UniqueAllied));
                    num--;
                }

                Utils.LogSomething("Allied Done");
            }

            if (CustomGameOptions.TraitorOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.TraitorCount : 1;

                while (num > 0)
                {
                    AllObjectifiers.Add((typeof(Traitor), CustomGameOptions.TraitorOn, 6, CustomGameOptions.UniqueTraitor));
                    num--;
                }

                Utils.LogSomething("Traitor Done");
            }

            if (CustomGameOptions.TaskmasterOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.TaskmasterCount : 1;

                while (num > 0)
                {
                    AllObjectifiers.Add((typeof(Taskmaster), CustomGameOptions.TaskmasterOn, 7, CustomGameOptions.UniqueTaskmaster));
                    num--;
                }

                Utils.LogSomething("Taskmaster Done");
            }

            var allCount = PlayerControl.AllPlayerControls.Count;
            Sort(AllObjectifiers, CustomGameOptions.MaxObjectifiers, CustomGameOptions.MinObjectifiers);
            AllObjectifiers.Shuffle();

            var canHaveLoverorRival = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveNeutralObjectifier = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveCrewObjectifier = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveAllied = PlayerControl.AllPlayerControls.ToArray().ToList();

            canHaveLoverorRival.RemoveAll(player => player.Is(RoleEnum.Altruist) || player.Is(RoleEnum.Troll) || player.Is(RoleEnum.Actor) || player.Is(RoleEnum.Jester) ||
                player.Is(RoleEnum.Shifter));
            canHaveLoverorRival.Shuffle();

            canHaveNeutralObjectifier.RemoveAll(player => !player.Is(Faction.Neutral));
            canHaveNeutralObjectifier.Shuffle();

            canHaveCrewObjectifier.RemoveAll(player => !player.Is(Faction.Crew));
            canHaveCrewObjectifier.Shuffle();

            canHaveAllied.RemoveAll(player => !player.Is(RoleAlignment.NeutralKill));
            canHaveAllied.Shuffle();

            var spawnList = GameStates.IsAA ? AASort(AllObjectifiers, allCount) : AllObjectifiers;
            spawnList.Shuffle();

            while (canHaveNeutralObjectifier.Count > 0 || canHaveCrewObjectifier.Count > 0 || canHaveLoverorRival.Count > 4)
            {
                if (spawnList.Count == 0)
                    break;

                var (type, _, id, unique) = spawnList.TakeFirst();
                int[] LoverRival = { 0, 1 };
                int[] Crew = { 2, 3, 6 };
                int[] Neutral = { 4, 7 };
                int[] Allied = { 5 };

                if (LoverRival.Contains(id) && canHaveLoverorRival.Count > 4)
                {
                    if (id == 0)
                        Lovers.Gen(canHaveLoverorRival);
                    else if (id == 1)
                        Rivals.Gen(canHaveLoverorRival);
                }
                else if (Crew.Contains(id) && canHaveCrewObjectifier.Count > 0)
                {
                    Objectifier.GenObjectifier<Objectifier>(type, canHaveCrewObjectifier.TakeFirst(), id);
                }
                else if (Neutral.Contains(id) && canHaveNeutralObjectifier.Count > 0)
                {
                    Objectifier.GenObjectifier<Objectifier>(type, canHaveNeutralObjectifier.TakeFirst(), id);
                }
                else if (Allied.Contains(id) && canHaveAllied.Count > 0)
                {
                    Objectifier.GenObjectifier<Objectifier>(type, canHaveAllied.TakeFirst(), id);
                }
            }

            Utils.LogSomething("Objectifiers Done");
        }

        private static void GenModifiers()
        {
            AllModifiers.Clear();
            var num = 0;

            if (CustomGameOptions.DiseasedOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.DiseasedCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((typeof(Diseased), CustomGameOptions.DiseasedOn, 0, CustomGameOptions.UniqueDiseased));
                    num--;
                }

                Utils.LogSomething("Diseased Done");
            }

            if (CustomGameOptions.BaitOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.BaitCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((typeof(Bait), CustomGameOptions.BaitOn, 1, CustomGameOptions.UniqueBait));
                    num--;
                }

                Utils.LogSomething("Bait Done");
            }

            if (CustomGameOptions.DwarfOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.DwarfCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((typeof(Dwarf), CustomGameOptions.DwarfOn, 2, CustomGameOptions.UniqueDwarf));
                    num--;
                }

                Utils.LogSomething("Dwarf Done");
            }

            if (CustomGameOptions.VIPOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.VIPCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((typeof(VIP), CustomGameOptions.VIPOn, 3, CustomGameOptions.UniqueVIP));
                    num--;
                }

                Utils.LogSomething("VIP Done");
            }

            if (CustomGameOptions.ShyOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.ShyCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((typeof(Shy), CustomGameOptions.ShyOn, 4, CustomGameOptions.UniqueShy));
                    num--;
                }

                Utils.LogSomething("Shy Done");
            }

            if (CustomGameOptions.GiantOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.GiantCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((typeof(Giant), CustomGameOptions.GiantOn, 5, CustomGameOptions.UniqueGiant));
                    num--;
                }

                Utils.LogSomething("Giant Done");
            }

            if (CustomGameOptions.DrunkOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.DrunkCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((typeof(Drunk), CustomGameOptions.DrunkOn, 6, CustomGameOptions.UniqueDrunk));
                    num--;
                }

                Utils.LogSomething("Drunk Done");
            }

            if (CustomGameOptions.FlincherOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.FlincherCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((typeof(Flincher), CustomGameOptions.FlincherOn, 7, CustomGameOptions.UniqueFlincher));
                    num--;
                }

                Utils.LogSomething("Flincher Done");
            }

            if (CustomGameOptions.CowardOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.CowardCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((typeof(Coward), CustomGameOptions.CowardOn, 8, CustomGameOptions.UniqueCoward));
                    num--;
                }

                Utils.LogSomething("Coward Done");
            }

            if (CustomGameOptions.VolatileOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.VolatileCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((typeof(Volatile), CustomGameOptions.VolatileOn, 9, CustomGameOptions.UniqueVolatile));
                    num--;
                }

                Utils.LogSomething("Volatile Done");
            }

            if (CustomGameOptions.IndomitableOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.IndomitableCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((typeof(Indomitable), CustomGameOptions.IndomitableOn, 11, CustomGameOptions.UniqueIndomitable));
                    num--;
                }

                Utils.LogSomething("Indomitable Done");
            }

            if (CustomGameOptions.ProfessionalOn > 0)
            {
                num = GameStates.IsCustom ? CustomGameOptions.ProfessionalCount : 1;

                while (num > 0)
                {
                    AllModifiers.Add((typeof(Professional), CustomGameOptions.ProfessionalOn, 10, CustomGameOptions.UniqueProfessional));
                    num--;
                }

                Utils.LogSomething("Professional Done");
            }

            var allCount = PlayerControl.AllPlayerControls.Count;
            Sort(AllModifiers, CustomGameOptions.MaxModifiers, CustomGameOptions.MinModifiers);
            AllModifiers.Shuffle();

            var canHaveBait = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveDiseased = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveProfessional = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveModifier = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveShy = PlayerControl.AllPlayerControls.ToArray().ToList();

            canHaveBait.RemoveAll(player => player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.Shifter) || player.Is(RoleEnum.Altruist) || player.Is(RoleEnum.Troll));
            canHaveBait.Shuffle();

            canHaveDiseased.RemoveAll(player => player.Is(RoleEnum.Altruist) || player.Is(RoleEnum.Troll));
            canHaveDiseased.Shuffle();

            canHaveProfessional.RemoveAll(player => !player.Is(AbilityEnum.Assassin));
            canHaveProfessional.Shuffle();

            canHaveShy.RemoveAll(player => (player.Is(RoleEnum.Mayor) && !CustomGameOptions.MayorButton) || (player.Is(RoleEnum.Jester) && !CustomGameOptions.JesterButton) ||
                (player.Is(RoleEnum.Swapper) && !CustomGameOptions.SwapperButton) || (player.Is(RoleEnum.Actor) && !CustomGameOptions.ActorButton) || (player.Is(RoleEnum.Guesser) &&
                !CustomGameOptions.GuesserButton) || (player.Is(RoleEnum.Executioner) && !CustomGameOptions.ExecutionerButton));
            canHaveShy.Shuffle();

            var spawnList = GameStates.IsAA ? AASort(AllModifiers, allCount) : AllModifiers;
            spawnList.Shuffle();

            while (canHaveBait.Count > 0 || canHaveDiseased.Count > 0 || canHaveProfessional.Count > 0 || canHaveModifier.Count > 0)
            {
                if (spawnList.Count == 0)
                    break;

                var (type, _, id, unique) = spawnList.TakeFirst();
                int[] Bait = { 1 };
                int[] Diseased = { 0 };
                int[] Professional = { 10 };
                int[] Global = { 2, 3, 5, 6, 7, 8, 9, 11 };
                int[] Shy = { 4 };

                if (canHaveBait.Count > 0 && Bait.Contains(id))
                    Modifier.GenModifier<Modifier>(type, canHaveBait.TakeFirst(), id);
                else if (canHaveDiseased.Count > 0 && Diseased.Contains(id))
                    Modifier.GenModifier<Modifier>(type, canHaveDiseased.TakeFirst(), id);
                else if (canHaveProfessional.Count > 0 && Professional.Contains(id))
                    Modifier.GenModifier<Modifier>(type, canHaveProfessional.TakeFirst(), id);
                else if (canHaveModifier.Count > 0 && Global.Contains(id))
                    Modifier.GenModifier<Modifier>(type, canHaveModifier.TakeFirst(), id);
                else if (canHaveShy.Count > 0 && Shy.Contains(id))
                    Modifier.GenModifier<Modifier>(type, canHaveShy.TakeFirst(), id);
            }

            Utils.LogSomething("Modifiers Done");
        }

        private static void SetTargets()
        {
            if (CustomGameOptions.AlliedOn > 0)
            {
                foreach (Allied ally in Objectifier.GetObjectifiers(ObjectifierEnum.Allied).Cast<Allied>())
                {
                    var alliedRole = Role.GetRole(ally.Player);

                    if (CustomGameOptions.AlliedFaction == AlliedFaction.Intruder)
                    {
                        alliedRole.Faction = Faction.Intruder;
                        alliedRole.IsIntAlly = true;
                        alliedRole.FactionColor = Colors.Intruder;
                        ally.Color = Colors.Intruder;
                    }
                    else if (CustomGameOptions.AlliedFaction == AlliedFaction.Syndicate)
                    {
                        alliedRole.Faction = Faction.Syndicate;
                        alliedRole.IsSynAlly = true;
                        alliedRole.FactionColor = Colors.Syndicate;
                        ally.Color = Colors.Syndicate;
                    }
                    else if (CustomGameOptions.AlliedFaction == AlliedFaction.Crew)
                    {
                        alliedRole.Faction = Faction.Crew;
                        alliedRole.IsCrewAlly = true;
                        alliedRole.FactionColor = Colors.Crew;
                        ally.Color = Colors.Crew;
                    }
                    else if (CustomGameOptions.AlliedFaction == AlliedFaction.Random)
                    {
                        var random = Random.RandomRangeInt(0, 3);

                        if (random == 0)
                        {
                            alliedRole.Faction = Faction.Intruder;
                            alliedRole.IsIntAlly = true;
                            alliedRole.FactionColor = Colors.Intruder;
                            ally.Color = Colors.Intruder;
                        }
                        else if (random == 1)
                        {
                            alliedRole.Faction = Faction.Syndicate;
                            alliedRole.IsSynAlly = true;
                            alliedRole.FactionColor = Colors.Syndicate;
                            ally.Color = Colors.Syndicate;
                        }
                        else if (random == 2)
                        {
                            alliedRole.Faction = Faction.Crew;
                            alliedRole.IsCrewAlly = true;
                            alliedRole.FactionColor = Colors.Crew;
                            ally.Color = Colors.Crew;
                        }
                    }

                    ally.Side = alliedRole.Faction;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetAlliedFaction, SendOption.Reliable);
                    writer.Write(ally.Player.PlayerId);
                    writer.Write((byte)alliedRole.Faction);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                Utils.LogSomething("Allied Faction Set Done");
            }

            var pretendRoles = new List<InspectorResults>();
            var exeTargets = new List<PlayerControl>();
            var gaTargets = new List<PlayerControl>();
            var guessTargets = new List<PlayerControl>();
            var goodRecruits = new List<PlayerControl>();
            var evilRecruits = new List<PlayerControl>();
            var bhTargets = new List<PlayerControl>();

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(Faction.Crew))
                {
                    if (!player.Is(RoleEnum.Altruist))
                        gaTargets.Add(player);

                    if (Objectifier.GetObjectifier(player) != null)
                        goodRecruits.Add(player);

                    if (!player.Is(RoleAlignment.CrewSov) && !player.Is(ObjectifierEnum.Traitor))
                        exeTargets.Add(player);
                }
                else if (player.Is(Faction.Neutral))
                {
                    if (!player.Is(RoleEnum.Executioner) && !player.Is(RoleEnum.Troll) && !player.Is(RoleEnum.GuardianAngel) && !player.Is(RoleEnum.Jester))
                    {
                        gaTargets.Add(player);

                        if (player.Is(RoleAlignment.NeutralKill) && Objectifier.GetObjectifier(player) != null)
                            evilRecruits.Add(player);
                    }

                    if (CustomGameOptions.ExeCanHaveNeutralTargets)
                        exeTargets.Add(player);
                }
                else if (player.Is(Faction.Intruder) || player.Is(Faction.Syndicate))
                {
                    gaTargets.Add(player);

                    if (Objectifier.GetObjectifier(player) != null)
                        evilRecruits.Add(player);

                    if ((player.Is(Faction.Intruder) && CustomGameOptions.ExeCanHaveIntruderTargets) || (player.Is(Faction.Syndicate) && CustomGameOptions.ExeCanHaveSyndicateTargets))
                        exeTargets.Add(player);
                }

                if (!player.Is(RoleEnum.Actor))
                    pretendRoles.Add((InspectorResults)Role.GetRole(player)?.InspectorResults);

                guessTargets.Add(player);
                bhTargets.Add(player);
            }

            Utils.LogSomething("Targets Set");

            if (CustomGameOptions.ExecutionerOn > 0)
            {
                foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner).Cast<Executioner>())
                {
                    exe.TargetPlayer = null;

                    if (exeTargets.Count > 0)
                    {
                        while (exe.TargetPlayer == null || exe.TargetPlayer == exe.Player)
                        {
                            exeTargets.Shuffle();
                            var exeNum = Random.RandomRangeInt(0, exeTargets.Count);
                            exe.TargetPlayer = exeTargets[exeNum];
                        }

                        exeTargets.Remove(exe.TargetPlayer);

                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetTarget, SendOption.Reliable);
                        writer.Write(exe.Player.PlayerId);
                        writer.Write(exe.TargetPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        //Utils.LogSomething($"Exe Target = {exe.TargetPlayer.name}");
                    }
                }

                Utils.LogSomething("Exe Target Set");
            }

            if (CustomGameOptions.GuesserOn > 0)
            {
                foreach (Guesser guess in Role.GetRoles(RoleEnum.Guesser).Cast<Guesser>())
                {
                    guess.TargetPlayer = null;

                    if (guessTargets.Count > 0)
                    {
                        while (guess.TargetPlayer == null || guess.TargetPlayer == guess.Player || guess.TargetPlayer.Is(ModifierEnum.Indomitable))
                        {
                            guessTargets.Shuffle();
                            var guessNum = Random.RandomRangeInt(0, guessTargets.Count);
                            guess.TargetPlayer = guessTargets[guessNum];
                        }

                        guessTargets.Remove(guess.TargetPlayer);

                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetGuessTarget, SendOption.Reliable);
                        writer.Write(guess.Player.PlayerId);
                        writer.Write(guess.TargetPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        //Utils.LogSomething($"Exe Target = {exe.TargetPlayer.name}");
                    }
                }

                Utils.LogSomething("Guess Target Set");
            }

            if (CustomGameOptions.GuardianAngelOn > 0)
            {
                foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel).Cast<GuardianAngel>())
                {
                    ga.TargetPlayer = null;

                    if (gaTargets.Count > 0)
                    {
                        while (ga.TargetPlayer == null || ga.TargetPlayer == ga.Player)
                        {
                            gaTargets.Shuffle();
                            var gaNum = Random.RandomRangeInt(0, gaTargets.Count);
                            ga.TargetPlayer = gaTargets[gaNum];
                        }

                        gaTargets.Remove(ga.TargetPlayer);

                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetGATarget, SendOption.Reliable);
                        writer.Write(ga.Player.PlayerId);
                        writer.Write(ga.TargetPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        //Utils.LogSomething($"GA Target = {ga.TargetPlayer.name}");
                    }
                }

                Utils.LogSomething("GA Target Set");
            }

            if (CustomGameOptions.BountyHunterOn > 0)
            {
                foreach (BountyHunter bh in Role.GetRoles(RoleEnum.BountyHunter).Cast<BountyHunter>())
                {
                    bh.TargetPlayer = null;

                    if (bhTargets.Count > 0)
                    {
                        while (bh.TargetPlayer == null || bh.TargetPlayer == bh.Player)
                        {
                            bhTargets.Shuffle();
                            var bhNum = Random.RandomRangeInt(0, gaTargets.Count);
                            bh.TargetPlayer = bhTargets[bhNum];
                        }

                        bhTargets.Remove(bh.TargetPlayer);

                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetBHTarget, SendOption.Reliable);
                        writer.Write(bh.Player.PlayerId);
                        writer.Write(bh.TargetPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        //Utils.LogSomething($"BH Target = {ga.TargetPlayer.name}");
                    }
                }

                Utils.LogSomething("BH Target Set");
            }

            if (CustomGameOptions.ActorOn > 0)
            {
                foreach (Actor act in Role.GetRoles(RoleEnum.Actor).Cast<Actor>())
                {
                    act.PretendRoles = InspectorResults.None;

                    if (pretendRoles.Count > 0)
                    {
                        while (act.PretendRoles == InspectorResults.None)
                        {
                            pretendRoles.Shuffle();
                            var actNum = Random.RandomRangeInt(0, pretendRoles.Count);
                            act.PretendRoles = pretendRoles[actNum];
                        }

                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetActPretendList, SendOption.Reliable);
                        writer.Write(act.Player.PlayerId);
                        writer.Write((byte)act.PretendRoles);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                }

                Utils.LogSomething("Act Variables Set");
            }

            if (CustomGameOptions.JackalOn > 0)
            {
                foreach (Jackal jackal in Role.GetRoles(RoleEnum.Jackal).Cast<Jackal>())
                {
                    jackal.GoodRecruit = null;
                    jackal.EvilRecruit = null;
                    jackal.BackupRecruit = null;

                    if (goodRecruits.Count > 0)
                    {
                        while (jackal.GoodRecruit == null || jackal.GoodRecruit == jackal.Player)
                        {
                            goodRecruits.Shuffle();
                            var goodNum = Random.RandomRangeInt(0, goodRecruits.Count - 1);
                            jackal.GoodRecruit = goodRecruits[goodNum];
                        }

                        goodRecruits.Remove(jackal.GoodRecruit);
                        (Role.GetRole(jackal.GoodRecruit)).SubFaction = SubFaction.Cabal;
                        Role.GetRole(jackal.GoodRecruit).IsRecruit = true;
                        jackal.Recruited.Add(jackal.GoodRecruit.PlayerId);

                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetGoodRecruit, SendOption.Reliable);
                        writer.Write(jackal.Player.PlayerId);
                        writer.Write(jackal.GoodRecruit.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        //Utils.LogSomething($"Good Recruit = {jackal.GoodRecruit.name}");
                    }

                    if (evilRecruits.Count > 0)
                    {
                        while (jackal.EvilRecruit == null || jackal.EvilRecruit == jackal.Player)
                        {
                            evilRecruits.Shuffle();
                            var evilNum = Random.RandomRangeInt(0, evilRecruits.Count - 1);
                            jackal.EvilRecruit = evilRecruits[evilNum];
                        }

                        evilRecruits.Remove(jackal.EvilRecruit);
                        Role.GetRole(jackal.EvilRecruit).SubFaction = SubFaction.Cabal;
                        (Role.GetRole(jackal.EvilRecruit)).IsRecruit = true;
                        jackal.Recruited.Add(jackal.EvilRecruit.PlayerId);

                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetEvilRecruit, SendOption.Reliable);
                        writer.Write(jackal.Player.PlayerId);
                        writer.Write(jackal.EvilRecruit.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        //Utils.LogSomething($"Evil Recruit = {jackal.EvilRecruit.name}");
                    }
                }

                Utils.LogSomething("Jackal Recruits Set");
            }
        }

        private static void ResetEverything()
        {
            Role.NobodyWins = false;
            Objectifier.NobodyWins = false;

            Role.CrewWin = false;
            Role.SyndicateWin = false;
            Role.IntruderWin = false;
            Role.AllNeutralsWin = false;

            Role.UndeadWin = false;
            Role.CabalWin = false;
            Role.SectWin = false;
            Role.ReanimatedWin = false;
            Role.InfectorsWin = false;

            Role.NKWins = false;

            Role.GlitchWins = false;
            Role.WerewolfWins = false;
            Role.JuggernautWins = false;
            Role.ArsonistWins = false;
            Role.MurdererWins = false;
            Role.SerialKillerWins = false;

            Role.PhantomWins = false;

            Objectifier.LoveWins = false;
            Objectifier.RivalWins = false;
            Objectifier.TaskmasterWins = false;
            Objectifier.OverlordWins = false;
            Objectifier.CorruptedWins = false;

            Role.SyndicateHasChaosDrive = false;
            Role.ChaosDriveMeetingTimerCount = 0;

            MeetingPatches.MeetingCount = 0;

            RecordRewind.points.Clear();
            Murder.KilledPlayers.Clear();

            Role.Buttons.Clear();
            Role.SetColors();

            UpdateNames.PlayerNames.Clear();
            AssetManager.LoadAndReload();
            LayerInfo.LoadInfo();

            MiscPatches.ExileControllerPatch.lastExiled = null;
        }

        private static void BeginRoleGen(List<GameData.PlayerInfo> infected)
        {
            if (!GameStates.IsHnS)
            {
                if (GameStates.IsKilling)
                    GenKilling(infected.ToList());
                else if (GameStates.IsVanilla)
                    GenVanilla(infected.ToList());
                else
                    GenClassicCustomAA(infected.ToList());

                if (!GameStates.IsVanilla)
                {
                    if (CustomGameOptions.EnableObjectifiers)
                        GenObjectifiers();

                    if (CustomGameOptions.EnableAbilities)
                        GenAbilities();

                    if (CustomGameOptions.EnableModifiers)
                        GenModifiers();

                    SetTargets();
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
        public static class HandleRPC
        {
            public static void Postfix([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
            {
                byte readByte, readByte1;

                switch ((CustomRPC)callId)
                {
                    case CustomRPC.SetRole:
                        var player = Utils.PlayerById(reader.ReadByte());

                        switch (reader.ReadByte())
                        {
                            case 0:
                                _ = new Mayor(player);
                                break;
                            case 1:
                                _ = new Sheriff(player);
                                break;
                            case 2:
                                _ = new Inspector(player);
                                break;
                            case 3:
                                _ = new Vigilante(player);
                                break;
                            case 4:
                                _ = new Engineer(player);
                                break;
                            case 5:
                                _ = new Swapper(player);
                                break;
                            case 6:
                                _ = new Drunkard(player);
                                break;
                            case 7:
                                _ = new TimeLord(player);
                                break;
                            case 8:
                                _ = new Medic(player);
                                break;
                            case 9:
                                _ = new Agent(player);
                                break;
                            case 10:
                                _ = new Altruist(player);
                                break;
                            case 11:
                                _ = new Veteran(player);
                                break;
                            case 12:
                                _ = new Tracker(player);
                                break;
                            case 13:
                                _ = new Transporter(player);
                                break;
                            case 14:
                                _ = new Medium(player);
                                break;
                            case 15:
                                _ = new Coroner(player);
                                break;
                            case 16:
                                _ = new Operative(player);
                                break;
                            case 17:
                                _ = new Detective(player);
                                break;
                            case 18:
                                _ = new Escort(player);
                                break;
                            case 19:
                                _ = new Shifter(player);
                                break;
                            case 20:
                                _ = new Crewmate(player);
                                break;
                            case 21:
                                _ = new VampireHunter(player);
                                break;
                            case 22:
                                _ = new Jester(player);
                                break;
                            case 23:
                                _ = new Amnesiac(player);
                                break;
                            case 24:
                                _ = new Executioner(player);
                                break;
                            case 25:
                                _ = new Survivor(player);
                                break;
                            case 26:
                                _ = new GuardianAngel(player);
                                break;
                            case 27:
                                _ = new Glitch(player);
                                break;
                            case 28:
                                _ = new Murderer(player);
                                break;
                            case 29:
                                _ = new Cryomaniac(player);
                                break;
                            case 30:
                                _ = new Werewolf(player);
                                break;
                            case 31:
                                _ = new Arsonist(player);
                                break;
                            case 32:
                                _ = new Jackal(player);
                                break;
                            case 33:
                                _ = new Plaguebearer(player);
                                break;
                            case 34:
                                _ = new Pestilence(player);
                                break;
                            case 35:
                                _ = new SerialKiller(player);
                                break;
                            case 36:
                                _ = new Juggernaut(player);
                                break;
                            case 37:
                                _ = new Cannibal(player);
                                break;
                            case 38:
                                _ = new Thief(player);
                                break;
                            case 39:
                                _ = new Dracula(player);
                                break;
                            case 40:
                                _ = new Troll(player);
                                break;
                            case 41:
                                _ = new Undertaker(player);
                                break;
                            case 42:
                                _ = new Morphling(player);
                                break;
                            case 43:
                                _ = new Blackmailer(player);
                                break;
                            case 44:
                                _ = new Miner(player);
                                break;
                            case 45:
                                _ = new Teleporter(player);
                                break;
                            case 46:
                                _ = new Wraith(player);
                                break;
                            case 47:
                                _ = new Consort(player);
                                break;
                            case 48:
                                _ = new Janitor(player);
                                break;
                            case 49:
                                _ = new Camouflager(player);
                                break;
                            case 50:
                                _ = new Grenadier(player);
                                break;
                            case 51:
                                _ = new Poisoner(player);
                                break;
                            case 52:
                                _ = new Impostor(player);
                                break;
                            case 53:
                                _ = new Consigliere(player);
                                break;
                            case 54:
                                _ = new Disguiser(player);
                                break;
                            case 55:
                                _ = new TimeMaster(player);
                                break;
                            case 56:
                                _ = new Godfather(player);
                                break;
                            case 57:
                                _ = new Anarchist(player);
                                break;
                            case 58:
                                _ = new Shapeshifter(player);
                                break;
                            case 59:
                                _ = new Gorgon(player);
                                break;
                            case 60:
                                _ = new Framer(player);
                                break;
                            case 61:
                                _ = new Rebel(player);
                                break;
                            case 62:
                                _ = new Concealer(player);
                                break;
                            case 63:
                                _ = new Warper(player);
                                break;
                            case 64:
                                _ = new Bomber(player);
                                break;
                            case 65:
                                _ = new Chameleon(player);
                                break;
                            case 66:
                                _ = new Guesser(player);
                                break;
                            case 67:
                                _ = new Whisperer(player);
                                break;
                            case 68:
                                _ = new Retributionist(player);
                                break;
                            case 69:
                                _ = new Actor(player);
                                break;
                            case 70:
                                _ = new BountyHunter(player);
                                break;
                            case 71:
                                _ = new Mystic(player);
                                break;
                            case 72:
                                _ = new Seer(player);
                                break;
                            case 73:
                                _ = new Necromancer(player);
                                break;
                            case 74:
                                _ = new Beamer(player);
                                break;
                            case 75:
                                _ = new Ambusher(player);
                                break;
                            case 76:
                                _ = new Crusader(player);
                                break;
                        }

                        break;

                    case CustomRPC.SetModifier:
                        var player2 = Utils.PlayerById(reader.ReadByte());

                        switch (reader.ReadInt32())
                        {
                            case 0:
                                _ = new Diseased(player2);
                                break;
                            case 1:
                                _ = new Bait(player2);
                                break;
                            case 2:
                                _ = new Dwarf(player2);
                                break;
                            case 3:
                                _ = new VIP(player2);
                                break;
                            case 4:
                                _ = new Shy(player2);
                                break;
                            case 5:
                                _ = new Giant(player2);
                                break;
                            case 6:
                                _ = new Drunk(player2);
                                break;
                            case 7:
                                _ = new Flincher(player2);
                                break;
                            case 8:
                                _ = new Coward(player2);
                                break;
                            case 9:
                                _ = new Volatile(player2);
                                break;
                            case 10:
                                _ = new Professional(player2);
                                break;
                            case 11:
                                _ = new Indomitable(player2);
                                break;
                        }

                        break;

                    case CustomRPC.SetAbility:
                        var player3 = Utils.PlayerById(reader.ReadByte());

                        switch (reader.ReadInt32())
                        {
                            case 0:
                            case 11:
                            case 12:
                            case 13:
                                _ = new Assassin(player3);
                                break;
                            case 1:
                                _ = new Snitch(player3);
                                break;
                            case 2:
                                _ = new Insider(player3);
                                break;
                            case 4:
                                _ = new Multitasker(player3);
                                break;
                            case 5:
                                _ = new Radar(player3);
                                break;
                            case 6:
                                _ = new Tiebreaker(player3);
                                break;
                            case 7:
                                _ = new Torch(player3);
                                break;
                            case 8:
                                _ = new Underdog(player3);
                                break;
                            case 9:
                                _ = new Tunneler(player3);
                                break;
                            case 10:
                                _ = new Ruthless(player3);
                                break;
                            case 14:
                                _ = new Ninja(player3);
                                break;
                            case 15:
                                _ = new ButtonBarry(player3);
                                break;
                        }

                        break;

                    case CustomRPC.SetObjectifier:
                        var player4 = Utils.PlayerById(reader.ReadByte());

                        switch (reader.ReadInt32())
                        {
                            case 2:
                                _ = new Fanatic(player4);
                                break;
                            case 3:
                                _ = new Corrupted(player4);
                                break;
                            case 4:
                                _ = new Overlord(player4);
                                break;
                            case 5:
                                _ = new Allied(player4);
                                break;
                            case 6:
                                _ = new Traitor(player4);
                                break;
                            case 7:
                                _ = new Taskmaster(player4);
                                break;
                        }

                        break;

                    case CustomRPC.SetCouple:
                        var lover1 = Utils.PlayerById(reader.ReadByte());
                        var lover2 = Utils.PlayerById(reader.ReadByte());
                        var objectifierLover1 = new Lovers(lover1);
                        var objectifierLover2 = new Lovers(lover2);
                        objectifierLover1.OtherLover = lover2;
                        objectifierLover2.OtherLover = lover1;
                        break;

                    case CustomRPC.SetDuo:
                        var rival1 = Utils.PlayerById(reader.ReadByte());
                        var rival2 = Utils.PlayerById(reader.ReadByte());
                        var objectifierRival1 = new Rivals(rival1);
                        var objectifierRival2 = new Rivals(rival2);
                        objectifierRival1.OtherRival = rival2;
                        objectifierRival2.OtherRival = rival1;
                        break;

                    case CustomRPC.Change:
                        var id5 = reader.ReadByte();

                        switch ((TurnRPC)id5)
                        {
                            case TurnRPC.TurnTraitorBetrayer:
                                Objectifier.GetObjectifier<Traitor>(Utils.PlayerById(reader.ReadByte())).TurnBetrayer();
                                break;

                            case TurnRPC.TurnFanaticBetrayer:
                                Objectifier.GetObjectifier<Fanatic>(Utils.PlayerById(reader.ReadByte())).TurnBetrayer();
                                break;

                            case TurnRPC.TurnPestilence:
                                Role.GetRole<Plaguebearer>(Utils.PlayerById(reader.ReadByte())).TurnPestilence();
                                break;

                            case TurnRPC.TurnVigilante:
                                Role.GetRole<VampireHunter>(Utils.PlayerById(reader.ReadByte())).TurnVigilante();
                                break;

                            case TurnRPC.TurnTroll:
                                Role.GetRole<BountyHunter>(Utils.PlayerById(reader.ReadByte())).TurnTroll();
                                break;

                            case TurnRPC.TurnSurv:
                                Role.GetRole<GuardianAngel>(Utils.PlayerById(reader.ReadByte())).TurnSurv();
                                break;

                            case TurnRPC.TurnGodfather:
                                Role.GetRole<Mafioso>(Utils.PlayerById(reader.ReadByte())).TurnGodfather();
                                break;

                            case TurnRPC.TurnJest:
                                Role.GetRole<Executioner>(Utils.PlayerById(reader.ReadByte())).TurnJest();
                                break;

                            case TurnRPC.TurnRebel:
                                Role.GetRole<Sidekick>(Utils.PlayerById(reader.ReadByte())).TurnRebel();
                                break;

                            case TurnRPC.TurnSheriff:
                                Role.GetRole<Seer>(Utils.PlayerById(reader.ReadByte())).TurnSheriff();
                                break;

                            case TurnRPC.TurnAct:
                                Role.GetRole<Guesser>(Utils.PlayerById(reader.ReadByte())).TurnAct();
                                break;

                            case TurnRPC.TurnSeer:
                                Role.GetRole<Mystic>(Utils.PlayerById(reader.ReadByte())).TurnSeer();
                                break;

                            case TurnRPC.TurnTraitor:
                                SetTraitor.TurnTraitor(Utils.PlayerById(reader.ReadByte()));
                                break;

                            case TurnRPC.TurnFanatic:
                                var attacker = Utils.PlayerById(reader.ReadByte());
                                var fanatic = Utils.PlayerById(reader.ReadByte());
                                var attackerRole = Role.GetRole(attacker);
                                Fanatic.TurnFanatic(fanatic, attackerRole.Faction);
                                break;
                        }

                        break;

                    case CustomRPC.SetRevealer:
                        readByte = reader.ReadByte();
                        SetRevealer.WillBeRevealer = readByte == byte.MaxValue ? null : Utils.PlayerById(readByte);
                        break;

                    case CustomRPC.SetPhantom:
                        readByte = reader.ReadByte();
                        SetPhantom.WillBePhantom = readByte == byte.MaxValue ? null : Utils.PlayerById(readByte);
                        break;

                    case CustomRPC.SetBanshee:
                        readByte = reader.ReadByte();
                        SetBanshee.WillBeBanshee = readByte == byte.MaxValue ? null : Utils.PlayerById(readByte);
                        break;

                    case CustomRPC.SetGhoul:
                        readByte = reader.ReadByte();
                        SetGhoul.WillBeGhoul = readByte == byte.MaxValue ? null : Utils.PlayerById(readByte);
                        break;

                    case CustomRPC.RevealerDied:
                        var revealer = Utils.PlayerById(reader.ReadByte());
                        var former = Role.GetRole(revealer);
                        var revealerRole = new Revealer(revealer);
                        revealer.RegenTask();
                        revealerRole.FormerRole = former;
                        revealerRole.RoleUpdate(former);
                        revealer.gameObject.layer = LayerMask.NameToLayer("Players");
                        SetRevealer.RemoveTasks(revealer);
                            PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
                            PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();

                        PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();

                        break;

                    case CustomRPC.PhantomDied:
                        var phantom = Utils.PlayerById(reader.ReadByte());
                        var phantomFormer = Role.GetRole(phantom);
                        var phantomRole = new Phantom(phantom);
                        phantom.RegenTask();
                        phantomRole.RoleUpdate(phantomFormer);
                        phantom.gameObject.layer = LayerMask.NameToLayer("Players");
                        SetPhantom.RemoveTasks(phantom);
                        PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
                        break;

                    case CustomRPC.BansheeDied:
                        var banshee = Utils.PlayerById(reader.ReadByte());
                        var bansheeFormer = Role.GetRole(banshee);
                        var bansheeRole = new Banshee(banshee);
                        banshee.RegenTask();
                        bansheeRole.RoleUpdate(bansheeFormer);
                        banshee.gameObject.layer = LayerMask.NameToLayer("Players");
                        PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
                        break;

                    case CustomRPC.GhoulDied:
                        var ghoul = Utils.PlayerById(reader.ReadByte());
                        var ghoulFormer = Role.GetRole(ghoul);
                        var ghoulRole = new Ghoul(ghoul);
                        ghoul.RegenTask();
                        ghoulRole.RoleUpdate(ghoulFormer);
                        ghoul.gameObject.layer = LayerMask.NameToLayer("Players");
                        PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
                        break;

                    case CustomRPC.Whisper:
                        var whisperer = Utils.PlayerById(reader.ReadByte());
                        var whispered = Utils.PlayerById(reader.ReadByte());
                        var message = reader.ReadString();

                        if (whispered == PlayerControl.LocalPlayer)
                        {
                            HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"{whisperer.name} whispers to you:{message}");
                        }
                        else if ((PlayerControl.LocalPlayer.Is(RoleEnum.Blackmailer) && CustomGameOptions.WhispersNotPrivate) || (PlayerControl.LocalPlayer.Data.IsDead &&
                                                    CustomGameOptions.DeadSeeEverything))
                        {
                            HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"{whisperer.name} is whispering to {whispered.name}:{message}");
                        }
                        else if (CustomGameOptions.WhispersAnnouncement)
                        {
                            HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"{whisperer.name} is whispering to {whispered.name}.");
                        }

                        break;

                    case CustomRPC.Guess:
                        break;

                    case CustomRPC.CatchPhantom:
                        Role.GetRole<Phantom>(Utils.PlayerById(reader.ReadByte())).Caught = true;
                        break;

                    case CustomRPC.CatchBanshee:
                        Role.GetRole<Banshee>(Utils.PlayerById(reader.ReadByte())).Caught = true;
                        break;

                    case CustomRPC.CatchGhoul:
                        Role.GetRole<Ghoul>(Utils.PlayerById(reader.ReadByte())).Caught = true;
                        break;

                    case CustomRPC.Start:
                        ResetEverything();
                        break;

                    case CustomRPC.AttemptSound:
                        var medicId = reader.ReadByte();
                        readByte = reader.ReadByte();

                        if (Utils.PlayerById(medicId).Is(RoleEnum.Retributionist))
                            RetStopKill.BreakShield(medicId, readByte, CustomGameOptions.ShieldBreaks);
                        else if (Utils.PlayerById(medicId).Is(RoleEnum.Medium))
                            MedStopKill.BreakShield(medicId, readByte, CustomGameOptions.ShieldBreaks);

                        break;

                    case CustomRPC.SetTarget:
                        var exe = Utils.PlayerById(reader.ReadByte());
                        var exeTarget = Utils.PlayerById(reader.ReadByte());
                        var exeRole = Role.GetRole<Executioner>(exe);
                        exeRole.TargetPlayer = exeTarget;
                        break;

                    case CustomRPC.SetGuessTarget:
                        var guess = Utils.PlayerById(reader.ReadByte());
                        var guessTarget = Utils.PlayerById(reader.ReadByte());
                        var guessRole = Role.GetRole<Guesser>(guess);
                        guessRole.TargetPlayer = guessTarget;
                        break;

                    case CustomRPC.SetGATarget:
                        var ga = Utils.PlayerById(reader.ReadByte());
                        var gaTarget = Utils.PlayerById(reader.ReadByte());
                        var gaRole = Role.GetRole<GuardianAngel>(ga);
                        gaRole.TargetPlayer = gaTarget;
                        break;

                    case CustomRPC.SetBHTarget:
                        var bh = Utils.PlayerById(reader.ReadByte());
                        var bhTarget = Utils.PlayerById(reader.ReadByte());
                        var bhRole = Role.GetRole<BountyHunter>(bh);
                        bhRole.TargetPlayer = bhTarget;
                        break;

                    case CustomRPC.SetActPretendList:
                        var act = Utils.PlayerById(reader.ReadByte());
                        var targetRoles = reader.ReadByte();
                        var actRole = Role.GetRole<Actor>(act);
                        actRole.PretendRoles = (InspectorResults)targetRoles;
                        break;

                    case CustomRPC.SetGoodRecruit:
                        var jackal = Utils.PlayerById(reader.ReadByte());
                        var goodRecruit = Utils.PlayerById(reader.ReadByte());
                        var jackalRole = Role.GetRole<Jackal>(jackal);
                        jackalRole.GoodRecruit = goodRecruit;
                        jackalRole.Recruited.Add(goodRecruit.PlayerId);
                        Role.GetRole(goodRecruit).SubFaction = SubFaction.Cabal;
                        Role.GetRole(goodRecruit).IsRecruit = true;
                        break;

                    case CustomRPC.SetBackupRecruit:
                        var jackal3 = Utils.PlayerById(reader.ReadByte());
                        var backRecruit = Utils.PlayerById(reader.ReadByte());
                        var jackalRole3 = Role.GetRole<Jackal>(jackal3);
                        Recruit.Recruit(jackalRole3, backRecruit);
                        break;

                    case CustomRPC.SetEvilRecruit:
                        var jackal2 = Utils.PlayerById(reader.ReadByte());
                        var evilRecruit = Utils.PlayerById(reader.ReadByte());
                        var jackalRole2 = Role.GetRole<Jackal>(jackal2);
                        jackalRole2.EvilRecruit = evilRecruit;
                        jackalRole2.Recruited.Add(evilRecruit.PlayerId);
                        Role.GetRole(evilRecruit).SubFaction = SubFaction.Cabal;
                        Role.GetRole(evilRecruit).IsRecruit = true;
                        break;

                    case CustomRPC.SendChat:
                        string report = reader.ReadString();
                        HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, report);
                        break;

                    case CustomRPC.CatchRevealer:
                        Role.GetRole<Revealer>(Utils.PlayerById(reader.ReadByte())).Caught = true;
                        break;

                    case CustomRPC.AddMayorVoteBank:
                        Role.GetRole<Mayor>(Utils.PlayerById(reader.ReadByte())).VoteBank += reader.ReadInt32();
                        break;

                    case CustomRPC.RemoveAllBodies:
                        foreach (var body in Object.FindObjectsOfType<DeadBody>())
                            body.gameObject.Destroy();

                        break;

                    case CustomRPC.SetSpawnAirship:
                        SpawnInMinigamePatch.SpawnPoints = reader.ReadBytesAndSize().ToList();
                        break;

                    case CustomRPC.DoorSyncToilet:
                        int Id = reader.ReadInt32();
                        PlainDoor DoorToSync = Object.FindObjectsOfType<PlainDoor>().FirstOrDefault(door => door.Id == Id);
                        DoorToSync.SetDoorway(true);
                        break;

                    case CustomRPC.SyncPlateform:
                        bool isLeft = reader.ReadBoolean();
                        CallPlateform.SyncPlateform(isLeft);
                        break;

                    case CustomRPC.CheckMurder:
                        var murderKiller = Utils.PlayerById(reader.ReadByte());
                        var murderTarget = Utils.PlayerById(reader.ReadByte());
                        murderKiller.CheckMurder(murderTarget);
                        break;

                    case CustomRPC.FixAnimation:
                        var player5 = Utils.PlayerById(reader.ReadByte());
                        player5.MyPhysics.ResetMoveState();
                        player5.Collider.enabled = true;
                        player5.moveable = true;
                        player5.NetTransform.enabled = true;
                        break;

                    case CustomRPC.VersionHandshake:
                        var major = reader.ReadByte();
                        var minor = reader.ReadByte();
                        var patch = reader.ReadByte();
                        var timer = reader.ReadSingle();

                        if (!AmongUsClient.Instance.AmHost && timer >= 0f)
                            GameStartManagerPatch.timer = timer;

                        var versionOwnerId = reader.ReadPackedInt32();
                        var revision = byte.MaxValue;
                        Guid guid;

                        if (reader.Length - reader.Position >= 17)
                        {
                            // Enough bytes left to read
                            revision = reader.ReadByte();
                            // GUID
                            var gbytes = reader.ReadBytes(16);
                            guid = new Guid(gbytes);
                        }
                        else
                        {
                            guid = new Guid(new byte[16]);
                        }

                        Utils.VersionHandshake(major, minor, patch, revision == byte.MaxValue ? -1 : revision, guid, versionOwnerId);
                        break;

                    case CustomRPC.SetAlliedFaction:
                        var player6 = Utils.PlayerById(reader.ReadByte());
                        var alliedRole = Role.GetRole(player6);
                        var ally = Objectifier.GetObjectifier<Allied>(player6);
                        var faction = (Faction)reader.ReadByte();
                        alliedRole.Faction = faction;

                        if (faction == Faction.Crew)
                        {
                            alliedRole.FactionColor = Colors.Crew;
                            alliedRole.IsCrewAlly = true;
                            ally.Color = Colors.Crew;
                        }
                        else if (faction == Faction.Intruder)
                        {
                            alliedRole.FactionColor = Colors.Intruder;
                            alliedRole.IsIntAlly = true;
                            ally.Color = Colors.Intruder;
                        }
                        else if (faction == Faction.Syndicate)
                        {
                            alliedRole.FactionColor = Colors.Syndicate;
                            alliedRole.IsSynAlly = true;
                            ally.Color = Colors.Syndicate;
                        }

                        ally.Side = alliedRole.Faction;
                        break;

                    case CustomRPC.SubmergedFixOxygen:
                        SubmergedCompatibility.RepairOxygen();
                        break;

                    case CustomRPC.ChaosDrive:
                        Role.SyndicateHasChaosDrive = reader.ReadBoolean();
                        break;

                    case CustomRPC.SetPos:
                        var setplayer = Utils.PlayerById(reader.ReadByte());
                        setplayer.transform.position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), setplayer.transform.position.z);
                        break;

                    case CustomRPC.SyncCustomSettings:
                        CustomOptions.RPC.ReceiveRPC(reader);
                        break;

                    case CustomRPC.SetSettings:
                        readByte = reader.ReadByte();
                        GameOptionsManager.Instance.currentNormalGameOptions.MapId = readByte == byte.MaxValue ? (byte)0 : readByte;
                        GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Scientist, 0, 0);
                        GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Engineer, 0, 0);
                        GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.GuardianAngel, 0, 0);
                        GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Shapeshifter, 0, 0);
                        GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod = CustomGameOptions.CrewVision;
                        GameOptionsManager.Instance.currentNormalGameOptions.ImpostorLightMod = CustomGameOptions.IntruderVision;
                        GameOptionsManager.Instance.currentNormalGameOptions.AnonymousVotes = CustomGameOptions.AnonymousVoting;
                        GameOptionsManager.Instance.currentNormalGameOptions.VisualTasks = CustomGameOptions.VisualTasks;
                        GameOptionsManager.Instance.currentNormalGameOptions.PlayerSpeedMod = CustomGameOptions.PlayerSpeed;
                        GameOptionsManager.Instance.currentNormalGameOptions.NumImpostors = CustomGameOptions.IntruderCount;
                        GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks = CustomGameOptions.GhostTasksCountToWin;
                        GameOptionsManager.Instance.currentNormalGameOptions.TaskBarMode = (AmongUs.GameOptions.TaskBarMode)CustomGameOptions.TaskBarMode;
                        GameOptionsManager.Instance.currentNormalGameOptions.ConfirmImpostor = CustomGameOptions.ConfirmEjects;
                        GameOptionsManager.Instance.currentNormalGameOptions.VotingTime = CustomGameOptions.VotingTime;
                        GameOptionsManager.Instance.currentNormalGameOptions.DiscussionTime = CustomGameOptions.DiscussionTime;
                        GameOptionsManager.Instance.currentNormalGameOptions.KillDistance = CustomGameOptions.InteractionDistance;
                        GameOptionsManager.Instance.currentNormalGameOptions.EmergencyCooldown = CustomGameOptions.EmergencyButtonCooldown;
                        GameOptionsManager.Instance.currentNormalGameOptions.NumEmergencyMeetings = CustomGameOptions.EmergencyButtonCount;
                        GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown = CustomGameOptions.IntKillCooldown;
                        GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks = CustomGameOptions.GhostTasksCountToWin;

                        if (CustomGameOptions.AutoAdjustSettings)
                            RandomMap.AdjustSettings(readByte);

                        break;

                    case CustomRPC.Action:
                        var id6 = reader.ReadByte();

                        switch ((ActionsRPC)id6)
                        {
                            case ActionsRPC.FreezeDouse:
                                var cryomaniac = Utils.PlayerById(reader.ReadByte());
                                var freezedouseTarget = Utils.PlayerById(reader.ReadByte());
                                var cryomaniacRole = Role.GetRole<Cryomaniac>(cryomaniac);
                                cryomaniacRole.DousedPlayers.Add(freezedouseTarget.PlayerId);
                                cryomaniacRole.LastDoused = DateTime.UtcNow;
                                break;

                            case ActionsRPC.RevealerFinished:
                                var revealer2 = Utils.PlayerById(reader.ReadByte());
                                var revealerRole2 = Role.GetRole<Revealer>(revealer2);
                                revealerRole2.CompletedTasks = true;
                                break;

                            case ActionsRPC.AllFreeze:
                                var theCryomaniac = Utils.PlayerById(reader.ReadByte());
                                var theCryomaniacRole = Role.GetRole<Cryomaniac>(theCryomaniac);
                                theCryomaniacRole.FreezeUsed = true;
                                break;

                            case ActionsRPC.JanitorClean:
                                var janitorPlayer = Utils.PlayerById(reader.ReadByte());
                                var janitorRole = Role.GetRole<Janitor>(janitorPlayer);

                                foreach (var body in Object.FindObjectsOfType<DeadBody>())
                                {
                                    if (body.ParentId == reader.ReadByte())
                                        Coroutines.Start(Coroutine.CleanCoroutine(body, janitorRole));
                                }

                                break;

                            case ActionsRPC.EngineerFix:
                                var engineer = Utils.PlayerById(reader.ReadByte());
                                Role.GetRole<Engineer>(engineer).UsesLeft--;
                                break;

                            case ActionsRPC.FixLights:
                                var lights = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                                lights.ActualSwitches = lights.ExpectedSwitches;
                                break;

                            case ActionsRPC.SetExtraVotes:
                                var mayor = Utils.PlayerById(reader.ReadByte());
                                var mayorRole = Role.GetRole<Mayor>(mayor);
                                mayorRole.ExtraVotes = reader.ReadBytesAndSize().ToList();

                                if (!mayor.Is(RoleEnum.Mayor))
                                    mayorRole.VoteBank -= mayorRole.ExtraVotes.Count;

                                break;

                            case ActionsRPC.SetSwaps:
                                var readSByte = reader.ReadSByte();
                                SwapVotes.Swap1 = MeetingHud.Instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == readSByte);
                                var readSByte2 = reader.ReadSByte();
                                SwapVotes.Swap2 = MeetingHud.Instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == readSByte2);
                                Utils.LogSomething("Bytes received - " + readSByte + " - " + readSByte2);
                                break;

                            case ActionsRPC.Remember:
                                var amnesiac = Utils.PlayerById(reader.ReadByte());
                                var other = Utils.PlayerById(reader.ReadByte());
                                PerformRemember.Remember(Role.GetRole<Amnesiac>(amnesiac), other);
                                break;

                            case ActionsRPC.Steal:
                                var thief = Utils.PlayerById(reader.ReadByte());
                                var other4 = Utils.PlayerById(reader.ReadByte());
                                PerformSteal.Steal(Role.GetRole<Thief>(thief), other4);
                                break;

                            case ActionsRPC.Whisper:
                                var whisp = Utils.PlayerById(reader.ReadByte());
                                var whispRole = Role.GetRole<Whisperer>(whisp);
                                whispRole.Whisper();
                                break;

                            case ActionsRPC.RetributionistAction:
                                var retId = reader.ReadByte();

                                switch ((RetributionistActionsRPC)retId)
                                {
                                    case RetributionistActionsRPC.RetributionistReviveSet:
                                        var ret = Utils.PlayerById(reader.ReadByte());
                                        var id8 = reader.ReadByte();

                                        if (id8 == sbyte.MaxValue)
                                            break;

                                        var revived = Utils.PlayerById(reader.ReadByte());
                                        var retRole = Role.GetRole<Retributionist>(ret);

                                        if (revived != null)
                                            retRole.Revived = revived;

                                        break;

                                    case RetributionistActionsRPC.RetributionistRevive:
                                        var ret2 = Utils.PlayerById(reader.ReadByte());
                                        var revived2 = Utils.PlayerById(reader.ReadByte());
                                        var retRole2 = Role.GetRole<Retributionist>(ret2);

                                        if (retRole2.RevivedRole.RoleType != RoleEnum.Altruist)
                                            break;

                                        StartRevive.Revive(retRole2, revived2);
                                        break;

                                    case RetributionistActionsRPC.EngineerFix:
                                        var ret3 = Utils.PlayerById(reader.ReadByte());
                                        var retRole3 = Role.GetRole<Retributionist>(ret3);

                                        if (retRole3.RevivedRole.RoleType != RoleEnum.Engineer)
                                            break;

                                        retRole3.FixUsesLeft--;
                                        break;

                                    case RetributionistActionsRPC.Protect:
                                        var ret5 = Utils.PlayerById(reader.ReadByte());
                                        var shielded = Utils.PlayerById(reader.ReadByte());
                                        var retRole5 = Role.GetRole<Retributionist>(ret5);

                                        if (retRole5.RevivedRole.RoleType != RoleEnum.Medic)
                                            break;

                                        retRole5.ShieldedPlayer = shielded;
                                        retRole5.UsedAbility = true;
                                        break;

                                    case RetributionistActionsRPC.Interrogate:
                                        var ret6 = Utils.PlayerById(reader.ReadByte());
                                        var interrogated = Utils.PlayerById(reader.ReadByte());
                                        var retRole6 = Role.GetRole<Retributionist>(ret6);

                                        if (retRole6.RevivedRole.RoleType != RoleEnum.Sheriff)
                                            break;

                                        retRole6.Interrogated.Add(interrogated.PlayerId);
                                        retRole6.LastInterrogated = DateTime.UtcNow;
                                        break;

                                    case RetributionistActionsRPC.Alert:
                                        var ret7 = Utils.PlayerById(reader.ReadByte());
                                        var retRole7 = Role.GetRole<Retributionist>(ret7);

                                        if (retRole7.RevivedRole.RoleType != RoleEnum.Veteran)
                                            break;

                                        retRole7.AlertTimeRemaining = CustomGameOptions.AlertDuration;
                                        retRole7.Alert();
                                        break;

                                    case RetributionistActionsRPC.AltruistRevive:
                                        var ret8 = Utils.PlayerById(reader.ReadByte());
                                        var retRole8 = Role.GetRole<Retributionist>(ret8);

                                        if (retRole8.RevivedRole.RoleType != RoleEnum.Altruist)
                                            break;

                                        readByte = reader.ReadByte();
                                        var theDeadBodies2 = Object.FindObjectsOfType<DeadBody>();

                                        foreach (var body in theDeadBodies2)
                                        {
                                            if (body.ParentId == readByte)
                                            {
                                                if (body.ParentId == PlayerControl.LocalPlayer.PlayerId)
                                                    Coroutines.Start(Utils.FlashCoroutine(retRole8.Color));

                                                Coroutines.Start(RetRevive.RetributionistRevive(body, retRole8));
                                            }
                                        }

                                        break;

                                    case RetributionistActionsRPC.Swoop:
                                        var ret9 = Utils.PlayerById(reader.ReadByte());
                                        var retRole9 = Role.GetRole<Retributionist>(ret9);

                                        if (retRole9.RevivedRole.RoleType != RoleEnum.Chameleon)
                                            break;

                                        retRole9.SwoopTimeRemaining = CustomGameOptions.SwoopDuration;
                                        retRole9.Invis();
                                        break;

                                    case RetributionistActionsRPC.Mediate:
                                        var mediatedPlayer2 = Utils.PlayerById(reader.ReadByte());
                                        var retRole10 = Role.GetRole<Retributionist>(Utils.PlayerById(reader.ReadByte()));

                                        if (PlayerControl.LocalPlayer.PlayerId != mediatedPlayer2.PlayerId)
                                            break;

                                        retRole10.AddMediatePlayer(mediatedPlayer2.PlayerId);
                                        break;
                                }

                                break;

                            case ActionsRPC.GodfatherAction:
                                var gfId = reader.ReadByte();

                                switch ((GodfatherActionsRPC)gfId)
                                {
                                    case GodfatherActionsRPC.Declare:
                                        var gf = Utils.PlayerById(reader.ReadByte());
                                        var maf = Utils.PlayerById(reader.ReadByte());
                                        PerformDeclare.Declare(Role.GetRole<Godfather>(gf), maf);
                                        break;

                                    case GodfatherActionsRPC.JanitorClean:
                                        var gf1 = Utils.PlayerById(reader.ReadByte());
                                        var gfRole1 = Role.GetRole<Godfather>(gf1);
                                        var deadBodies2 = Object.FindObjectsOfType<DeadBody>();

                                        foreach (var body in deadBodies2)
                                        {
                                            if (body.ParentId == reader.ReadByte())
                                                Coroutines.Start(Coroutine2.CleanCoroutine(body, gfRole1));
                                        }

                                        break;

                                    case GodfatherActionsRPC.Teleport:
                                        var gf2 = Utils.PlayerById(reader.ReadByte());
                                        var gfRole2 = Role.GetRole<Godfather>(gf2);
                                        gfRole2.TeleportPoint = reader.ReadVector2();
                                        Godfather.Teleport(gf2);
                                        break;

                                    case GodfatherActionsRPC.Morph:
                                        var gf3 = Utils.PlayerById(reader.ReadByte());
                                        var morphTarget2 = Utils.PlayerById(reader.ReadByte());
                                        var gfRole3 = Role.GetRole<Godfather>(gf3);
                                        gfRole3.MorphTimeRemaining = CustomGameOptions.MorphlingDuration;
                                        gfRole3.MorphedPlayer = morphTarget2;
                                        gfRole3.Morph();
                                        break;

                                    case GodfatherActionsRPC.Disguise:
                                        var gf4 = Utils.PlayerById(reader.ReadByte());
                                        var disguiseTarget2 = Utils.PlayerById(reader.ReadByte());
                                        var disguiserForm2 = Utils.PlayerById(reader.ReadByte());
                                        var gfRole4 = Role.GetRole<Godfather>(gf4);
                                        gfRole4.DisguiserTimeRemaining2 = CustomGameOptions.TimeToDisguise;
                                        gfRole4.DisguiserTimeRemaining = CustomGameOptions.DisguiseDuration;
                                        gfRole4.MeasuredPlayer = disguiseTarget2;
                                        gfRole4.ClosestPlayer = disguiserForm2;
                                        gfRole4.Delay();
                                        break;

                                    case GodfatherActionsRPC.Blackmail:
                                        var gfRole5 = Role.GetRole<Blackmailer>(Utils.PlayerById(reader.ReadByte()));
                                        gfRole5.BlackmailedPlayer = Utils.PlayerById(reader.ReadByte());
                                        break;

                                    case GodfatherActionsRPC.Mine:
                                        var ventId2 = reader.ReadInt32();
                                        var gf6 = Utils.PlayerById(reader.ReadByte());
                                        var gfRole6 = Role.GetRole<Godfather>(gf6);
                                        var pos2 = reader.ReadVector2();
                                        var zAxis2 = reader.ReadSingle();
                                        PerformDeclare.SpawnVent(ventId2, gfRole6, pos2, zAxis2);
                                        break;

                                    case GodfatherActionsRPC.TimeFreeze:
                                        var gf7 = Utils.PlayerById(reader.ReadByte());
                                        var gfRole7 = Role.GetRole<Godfather>(gf7);
                                        Freeze.FreezeFunctions.FreezeAll();
                                        gfRole7.FreezeTimeRemaining = CustomGameOptions.FreezeDuration;
                                        gfRole7.TimeFreeze();
                                        break;

                                    case GodfatherActionsRPC.Invis:
                                        var gf8 = Utils.PlayerById(reader.ReadByte());
                                        var gfRole8 = Role.GetRole<Godfather>(gf8);
                                        gfRole8.InvisTimeRemaining = CustomGameOptions.InvisDuration;
                                        gfRole8.Invis();
                                        break;

                                    case GodfatherActionsRPC.Drag:
                                        var gf9 = Utils.PlayerById(reader.ReadByte());
                                        var gfRole9 = Role.GetRole<Godfather>(gf9);
                                        var dienerBodies2 = Object.FindObjectsOfType<DeadBody>();

                                        foreach (var body in dienerBodies2)
                                        {
                                            if (body.ParentId == reader.ReadByte())
                                                gfRole9.CurrentlyDragging = body;
                                        }

                                        break;

                                    case GodfatherActionsRPC.Drop:
                                        readByte1 = reader.ReadByte();
                                        var v2_1 = reader.ReadVector2();
                                        var v2z_1 = reader.ReadSingle();
                                        var gf10 = Utils.PlayerById(readByte1);
                                        var gfRole10 = Role.GetRole<Godfather>(gf10);
                                        var body2_1 = gfRole10.CurrentlyDragging;
                                        gfRole10.CurrentlyDragging = null;
                                        body2_1.transform.position = new Vector3(v2_1.x, v2_1.y, v2z_1);
                                        break;

                                    case GodfatherActionsRPC.Camouflage:
                                        var gf11 = Utils.PlayerById(reader.ReadByte());
                                        var gfRole11 = Role.GetRole<Godfather>(gf11);
                                        Utils.Camouflage();
                                        gfRole11.CamoTimeRemaining = CustomGameOptions.CamouflagerDuration;
                                        gfRole11.Camouflage();
                                        break;

                                    case GodfatherActionsRPC.FlashGrenade:
                                        var gf12 = Utils.PlayerById(reader.ReadByte());
                                        var gfRole12 = Role.GetRole<Godfather>(gf12);
                                        gfRole12.FlashTimeRemaining = CustomGameOptions.GrenadeDuration;
                                        gfRole12.Flash();
                                        break;
                                }

                                break;

                            case ActionsRPC.RebelAction:
                                var rebId = reader.ReadByte();

                                switch ((RebelActionsRPC)rebId)
                                {
                                    case RebelActionsRPC.Sidekick:
                                        var reb = Utils.PlayerById(reader.ReadByte());
                                        var side = Utils.PlayerById(reader.ReadByte());
                                        PerformSidekick.Sidekick(Role.GetRole<Rebel>(reb), side);
                                        break;

                                    case RebelActionsRPC.Poison:
                                        var reb1 = Utils.PlayerById(reader.ReadByte());
                                        var poisoned2 = Utils.PlayerById(reader.ReadByte());
                                        var rebRole1 = Role.GetRole<Rebel>(reb1);
                                        rebRole1.PoisonedPlayer = poisoned2;
                                        break;

                                    case RebelActionsRPC.Warp:
                                        var teleports2 = reader.ReadByte();
                                        var coordinates2 = new Dictionary<byte, Vector2>();

                                        for (var i = 0; i < teleports2; i++)
                                        {
                                            var playerId = reader.ReadByte();
                                            var location = reader.ReadVector2();
                                            coordinates2.Add(playerId, location);
                                        }

                                        Rebel.WarpPlayersToCoordinates(coordinates2);
                                        break;

                                    case RebelActionsRPC.Conceal:
                                        var reb2 = Utils.PlayerById(reader.ReadByte());
                                        var rebRole2 = Role.GetRole<Rebel>(reb2);
                                        rebRole2.ConcealTimeRemaining = CustomGameOptions.ConcealDuration;
                                        rebRole2.Conceal();
                                        Utils.Conceal();
                                        break;

                                    case RebelActionsRPC.Shapeshift:
                                        var reb3 = Utils.PlayerById(reader.ReadByte());
                                        var rebRole3 = Role.GetRole<Rebel>(reb3);
                                        rebRole3.ShapeshiftTimeRemaining = CustomGameOptions.ShapeshiftDuration;
                                        rebRole3.Shapeshift();
                                        break;

                                    case RebelActionsRPC.Confuse:
                                        var reb5 = Utils.PlayerById(reader.ReadByte());
                                        var rebRole5 = Role.GetRole<Rebel>(reb5);
                                        rebRole5.ConfuseTimeRemaining = CustomGameOptions.ConfuseDuration;
                                        rebRole5.Confuse();
                                        Reverse.ConfuseFunctions.ConfuseAll();
                                        break;
                                }

                                break;

                            case ActionsRPC.Shift:
                                var shifter = Utils.PlayerById(reader.ReadByte());
                                var other2 = Utils.PlayerById(reader.ReadByte());
                                PerformShift.Shift(Role.GetRole<Shifter>(shifter), other2);
                                break;

                            case ActionsRPC.Convert:
                                var drac = Utils.PlayerById(reader.ReadByte());
                                var other3 = Utils.PlayerById(reader.ReadByte());
                                PerformConvert.Convert(Role.GetRole<Dracula>(drac), other3);
                                break;

                            case ActionsRPC.Teleport:
                                var teleporter = Utils.PlayerById(reader.ReadByte());
                                var teleporterRole = Role.GetRole<Teleporter>(teleporter);
                                teleporterRole.TeleportPoint = reader.ReadVector2();
                                Teleporter.Teleport(teleporter);
                                break;

                            case ActionsRPC.Rewind:
                                var TimeLordPlayer = Utils.PlayerById(reader.ReadByte());
                                var TimeLordRole = Role.GetRole<TimeLord>(TimeLordPlayer);
                                StartStop.StartRewind(TimeLordRole);
                                break;

                            case ActionsRPC.Protect:
                                var medic = Utils.PlayerById(reader.ReadByte());
                                var shield = Utils.PlayerById(reader.ReadByte());
                                Role.GetRole<Medic>(medic).ShieldedPlayer = shield;
                                break;

                            case ActionsRPC.RewindRevive:
                                RecordRewind.ReviveBody(Utils.PlayerById(reader.ReadByte()));
                                break;

                            case ActionsRPC.BypassKill:
                                var killer = Utils.PlayerById(reader.ReadByte());
                                var target = Utils.PlayerById(reader.ReadByte());
                                var lunge = reader.ReadBoolean();
                                Utils.MurderPlayer(killer, target, lunge);
                                break;

                            case ActionsRPC.AssassinKill:
                                var toDie = Utils.PlayerById(reader.ReadByte());
                                var guessString = reader.ReadString();
                                var assassin = Ability.GetAbilityValue<Assassin>(AbilityEnum.Assassin);
                                AssassinKill.MurderPlayer(assassin, toDie, guessString);
                                break;

                            case ActionsRPC.GuesserKill:
                                var toDie2 = Utils.PlayerById(reader.ReadByte());
                                var guessString2 = reader.ReadString();
                                var assassin2 = Role.GetRoleValue<Guesser>(RoleEnum.Guesser);
                                GuesserKill.MurderPlayer(assassin2, toDie2, guessString2);
                                break;

                            case ActionsRPC.Interrogate:
                                var sheriff = Utils.PlayerById(reader.ReadByte());
                                var otherPlayer = Utils.PlayerById(reader.ReadByte());
                                var sheriffRole = Role.GetRole<Sheriff>(sheriff);
                                sheriffRole.Interrogated.Add(otherPlayer.PlayerId);
                                sheriffRole.LastInterrogated = DateTime.UtcNow;
                                break;

                            case ActionsRPC.Morph:
                                var morphling = Utils.PlayerById(reader.ReadByte());
                                var morphTarget = Utils.PlayerById(reader.ReadByte());
                                var morphRole = Role.GetRole<Morphling>(morphling);
                                morphRole.TimeRemaining = CustomGameOptions.MorphlingDuration;
                                morphRole.MorphedPlayer = morphTarget;
                                morphRole.Morph();
                                break;

                            case ActionsRPC.Scream:
                                var banshee2 = Utils.PlayerById(reader.ReadByte());
                                var bansheeRole2 = Role.GetRole<Banshee>(banshee2);
                                bansheeRole2.TimeRemaining = CustomGameOptions.ScreamDuration;
                                bansheeRole2.Scream();

                                foreach (var player8 in PlayerControl.AllPlayerControls)
                                {
                                    if (!player8.Data.IsDead && !player8.Data.Disconnected && !player8.Is(Faction.Syndicate))
                                    {
                                        var targetRole4 = Role.GetRole(player8);
                                        targetRole4.IsBlocked = !targetRole4.RoleBlockImmune;
                                        bansheeRole2.Blocked.Add(player8.PlayerId);
                                    }
                                }

                                break;

                            case ActionsRPC.Mark:
                                var ghoul2 = Utils.PlayerById(reader.ReadByte());
                                var marked = Utils.PlayerById(reader.ReadByte());
                                var ghoulRole2 = Role.GetRole<Ghoul>(ghoul2);
                                ghoulRole2.MarkedPlayer = marked;
                                break;

                            case ActionsRPC.Disguise:
                                var disguiser = Utils.PlayerById(reader.ReadByte());
                                var disguiseTarget = Utils.PlayerById(reader.ReadByte());
                                var disguiserForm = Utils.PlayerById(reader.ReadByte());
                                var disguiseRole = Role.GetRole<Disguiser>(disguiser);
                                disguiseRole.TimeRemaining = CustomGameOptions.DisguiseDuration;
                                disguiseRole.TimeRemaining2 = CustomGameOptions.TimeToDisguise;
                                disguiseRole.MeasuredPlayer = disguiseTarget;
                                disguiseRole.ClosestPlayer = disguiserForm;
                                disguiseRole.Delay();
                                break;

                            case ActionsRPC.Poison:
                                var poisoner = Utils.PlayerById(reader.ReadByte());
                                var poisoned = Utils.PlayerById(reader.ReadByte());
                                var poisonerRole = Role.GetRole<Poisoner>(poisoner);
                                poisonerRole.PoisonedPlayer = poisoned;
                                break;

                            case ActionsRPC.Blackmail:
                                var blackmailer = Role.GetRole<Blackmailer>(Utils.PlayerById(reader.ReadByte()));
                                blackmailer.BlackmailedPlayer = Utils.PlayerById(reader.ReadByte());
                                break;

                            case ActionsRPC.Mine:
                                var ventId = reader.ReadInt32();
                                var miner = Utils.PlayerById(reader.ReadByte());
                                var minerRole = Role.GetRole<Miner>(miner);
                                var pos = reader.ReadVector2();
                                var zAxis = reader.ReadSingle();
                                Mine.SpawnVent(ventId, minerRole, pos, zAxis);
                                break;

                            case ActionsRPC.TimeFreeze:
                                var tm = Utils.PlayerById(reader.ReadByte());
                                var tmRole = Role.GetRole<TimeMaster>(tm);
                                Freeze.FreezeFunctions.FreezeAll();
                                tmRole.TimeRemaining = CustomGameOptions.FreezeDuration;
                                tmRole.TimeFreeze();
                                break;

                            case ActionsRPC.Confuse:
                                var drunk = Utils.PlayerById(reader.ReadByte());
                                var drunkRole = Role.GetRole<Drunkard>(drunk);
                                drunkRole.TimeRemaining = CustomGameOptions.ConfuseDuration;
                                drunkRole.Confuse();
                                Reverse.ConfuseFunctions.ConfuseAll();
                                break;

                            case ActionsRPC.Invis:
                                var wraith = Utils.PlayerById(reader.ReadByte());
                                var wraithRole = Role.GetRole<Wraith>(wraith);
                                wraithRole.TimeRemaining = CustomGameOptions.InvisDuration;
                                wraithRole.Invis();
                                break;

                            case ActionsRPC.Alert:
                                var veteran = Utils.PlayerById(reader.ReadByte());
                                var veteranRole = Role.GetRole<Veteran>(veteran);
                                veteranRole.TimeRemaining = CustomGameOptions.AlertDuration;
                                veteranRole.Alert();
                                break;

                            case ActionsRPC.Vest:
                                var surv = Utils.PlayerById(reader.ReadByte());
                                var survRole = Role.GetRole<Survivor>(surv);
                                survRole.TimeRemaining = CustomGameOptions.VestDuration;
                                survRole.Vest();
                                break;

                            case ActionsRPC.Ambush:
                                var amb = Utils.PlayerById(reader.ReadByte());
                                var ambushed = Utils.PlayerById(reader.ReadByte());
                                var ambRole = Role.GetRole<Ambusher>(amb);
                                ambRole.TimeRemaining = CustomGameOptions.VestDuration;
                                ambRole.AmbushedPlayer = ambushed;
                                ambRole.Ambush();
                                break;

                            case ActionsRPC.GAProtect:
                                var ga2 = Utils.PlayerById(reader.ReadByte());
                                var ga2Role = Role.GetRole<GuardianAngel>(ga2);
                                ga2Role.TimeRemaining = CustomGameOptions.ProtectDuration;
                                ga2Role.Protect();
                                break;

                            case ActionsRPC.Transport:
                                Coroutines.Start(Transporter.TransportPlayers(reader.ReadByte(), reader.ReadByte(), reader.ReadBoolean()));
                                break;

                            case ActionsRPC.Beam:
                                Coroutines.Start(Beamer.BeamPlayers(reader.ReadByte(), reader.ReadByte()));
                                break;

                            case ActionsRPC.SetUntransportable:
                                if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
                                    Role.GetRole<Transporter>(PlayerControl.LocalPlayer).UntransportablePlayers.Add(reader.ReadByte(), DateTime.UtcNow);

                                break;

                            case ActionsRPC.SetUnbeamable:
                                if (PlayerControl.LocalPlayer.Is(RoleEnum.Beamer))
                                    Role.GetRole<Beamer>(PlayerControl.LocalPlayer).UnbeamablePlayers.Add(reader.ReadByte(), DateTime.UtcNow);

                                break;

                            case ActionsRPC.Mediate:
                                var mediatedPlayer = Utils.PlayerById(reader.ReadByte());
                                var medium = Role.GetRole<Medium>(Utils.PlayerById(reader.ReadByte()));

                                if (PlayerControl.LocalPlayer.PlayerId != mediatedPlayer.PlayerId)
                                    break;

                                medium.AddMediatePlayer(mediatedPlayer.PlayerId);
                                break;

                            case ActionsRPC.FlashGrenade:
                                var grenadier = Utils.PlayerById(reader.ReadByte());
                                var grenadierRole = Role.GetRole<Grenadier>(grenadier);
                                grenadierRole.TimeRemaining = CustomGameOptions.GrenadeDuration;
                                grenadierRole.Flash();
                                break;

                            case ActionsRPC.Maul:
                                var ww = Utils.PlayerById(reader.ReadByte());
                                var wwRole = Role.GetRole<Werewolf>(ww);
                                wwRole.Maul(wwRole.Player);
                                break;

                            case ActionsRPC.Douse:
                                var arsonist = Utils.PlayerById(reader.ReadByte());
                                var douseTarget = Utils.PlayerById(reader.ReadByte());
                                var arsonistRole = Role.GetRole<Arsonist>(arsonist);
                                arsonistRole.DousedPlayers.Add(douseTarget.PlayerId);
                                arsonistRole.LastDoused = DateTime.UtcNow;
                                arsonistRole.LastIgnited = DateTime.UtcNow;
                                break;

                            case ActionsRPC.Ignite:
                                var theArsonist = Utils.PlayerById(reader.ReadByte());
                                var theArsonistRole = Role.GetRole<Arsonist>(theArsonist);
                                theArsonistRole.Ignite();
                                theArsonistRole.LastIgnited = DateTime.UtcNow;
                                break;

                            case ActionsRPC.CannibalEat:
                                readByte1 = reader.ReadByte();
                                var cannibalPlayer = Utils.PlayerById(readByte1);
                                var cannibalRole = Role.GetRole<Cannibal>(cannibalPlayer);
                                readByte = reader.ReadByte();

                                foreach (var body in Object.FindObjectsOfType<DeadBody>())
                                {
                                    if (body.ParentId == readByte)
                                        Coroutines.Start(Eat.EatCoroutine(body, cannibalRole));
                                }

                                break;

                            case ActionsRPC.Infect:
                                var pb = Utils.PlayerById(reader.ReadByte());
                                var infected = reader.ReadByte();
                                Role.GetRole<Plaguebearer>(pb).InfectedPlayers.Add(infected);
                                break;

                            case ActionsRPC.AltruistRevive:
                                readByte1 = reader.ReadByte();
                                var altruistPlayer = Utils.PlayerById(readByte1);
                                var altruistRole = Role.GetRole<Altruist>(altruistPlayer);
                                readByte = reader.ReadByte();

                                foreach (var body in Object.FindObjectsOfType<DeadBody>())
                                {
                                    if (body.ParentId == readByte)
                                    {
                                        if (body.ParentId == PlayerControl.LocalPlayer.PlayerId)
                                            Coroutines.Start(Utils.FlashCoroutine(altruistRole.Color));

                                        Coroutines.Start(Revive.AltruistRevive(body, altruistRole));
                                    }
                                }

                                break;

                            case ActionsRPC.NecromancerResurrect:
                                readByte1 = reader.ReadByte();
                                var necroPlayer = Utils.PlayerById(readByte1);
                                var necroRole = Role.GetRole<Necromancer>(necroPlayer);
                                readByte = reader.ReadByte();

                                foreach (var body in Object.FindObjectsOfType<DeadBody>())
                                {
                                    if (body.ParentId == readByte)
                                    {
                                        if (body.ParentId == PlayerControl.LocalPlayer.PlayerId)
                                            Coroutines.Start(Utils.FlashCoroutine(necroRole.Color));

                                        Coroutines.Start(Resurrect.NecromancerResurrect(body, necroRole));
                                    }
                                }

                                break;

                            case ActionsRPC.Warp:
                                var teleports = reader.ReadByte();
                                var coordinates = new Dictionary<byte, Vector2>();

                                for (var i = 0; i < teleports; i++)
                                {
                                    var playerId = reader.ReadByte();
                                    var location = reader.ReadVector2();
                                    coordinates.Add(playerId, location);
                                }

                                Warper.WarpPlayersToCoordinates(coordinates);
                                break;

                            case ActionsRPC.Detonate:
                                var bomber = Utils.PlayerById(reader.ReadByte());
                                var bomberRole = Role.GetRole<Bomber>(bomber);
                                bomberRole.Bombs.DetonateBombs(bomber.name);
                                break;

                            case ActionsRPC.Swoop:
                                var chameleon = Utils.PlayerById(reader.ReadByte());
                                var chameleonRole = Role.GetRole<Chameleon>(chameleon);
                                chameleonRole.TimeRemaining = CustomGameOptions.SwoopDuration;
                                chameleonRole.Invis();
                                break;

                            case ActionsRPC.BarryButton:
                                var buttonBarry = Utils.PlayerById(reader.ReadByte());

                                if (AmongUsClient.Instance.AmHost)
                                {
                                    MeetingRoomManager.Instance.reporter = buttonBarry;
                                    MeetingRoomManager.Instance.target = null;
                                    AmongUsClient.Instance.DisconnectHandlers.AddUnique(MeetingRoomManager.Instance.Cast<IDisconnectHandler>());

                                    if (GameManager.Instance.CheckTaskCompletion())
                                        return;

                                    HudManager.Instance.OpenMeetingRoom(buttonBarry);
                                    buttonBarry.RpcStartMeeting(null);
                                }

                                break;

                            case ActionsRPC.BaitReport:
                                var baitKiller = Utils.PlayerById(reader.ReadByte());
                                var bait = Utils.PlayerById(reader.ReadByte());
                                baitKiller.ReportDeadBody(bait.Data);
                                break;

                            case ActionsRPC.Drag:
                                var dienerPlayer = Utils.PlayerById(reader.ReadByte());
                                var dienerRole = Role.GetRole<Undertaker>(dienerPlayer);
                                var dienerBodies = Object.FindObjectsOfType<DeadBody>();

                                foreach (var body in dienerBodies)
                                {
                                    if (body.ParentId == reader.ReadByte())
                                        dienerRole.CurrentlyDragging = body;
                                }

                                break;

                            case ActionsRPC.Drop:
                                readByte1 = reader.ReadByte();
                                var v2 = reader.ReadVector2();
                                var v2z = reader.ReadSingle();
                                var dienerPlayer2 = Utils.PlayerById(readByte1);
                                var dienerRole2 = Role.GetRole<Undertaker>(dienerPlayer2);
                                var body2 = dienerRole2.CurrentlyDragging;
                                dienerRole2.CurrentlyDragging = null;
                                body2.transform.position = new Vector3(v2.x, v2.y, v2z);
                                break;

                            case ActionsRPC.Camouflage:
                                var camouflager = Utils.PlayerById(reader.ReadByte());
                                var camouflagerRole = Role.GetRole<Camouflager>(camouflager);
                                Utils.Camouflage();
                                camouflagerRole.TimeRemaining = CustomGameOptions.CamouflagerDuration;
                                camouflagerRole.Camouflage();
                                break;

                            case ActionsRPC.EscRoleblock:
                                var escort = Utils.PlayerById(reader.ReadByte());
                                var blocked2 = Utils.PlayerById(reader.ReadByte());
                                var escortRole = Role.GetRole<Escort>(escort);
                                var targetRole = Role.GetRole(blocked2);
                                targetRole.IsBlocked = !targetRole.RoleBlockImmune;
                                escortRole.BlockTarget = blocked2;
                                escortRole.TimeRemaining = CustomGameOptions.EscRoleblockDuration;
                                escortRole.Block();
                                break;

                            case ActionsRPC.GlitchRoleblock:
                                var glitch = Utils.PlayerById(reader.ReadByte());
                                var blocked3 = Utils.PlayerById(reader.ReadByte());
                                var glitchRole = Role.GetRole<Glitch>(glitch);
                                var targetRole3 = Role.GetRole(blocked3);
                                targetRole3.IsBlocked = !targetRole3.RoleBlockImmune;
                                glitchRole.HackTarget = blocked3;
                                glitchRole.TimeRemaining = CustomGameOptions.HackDuration;
                                glitchRole.Hack();
                                break;

                            case ActionsRPC.ConsRoleblock:
                                var consort = Utils.PlayerById(reader.ReadByte());
                                var blocked = Utils.PlayerById(reader.ReadByte());
                                var consortRole = Role.GetRole<Consort>(consort);
                                var targetRole2 = Role.GetRole(blocked);
                                targetRole2.IsBlocked = !targetRole2.RoleBlockImmune;
                                consortRole.BlockTarget = blocked;
                                consortRole.TimeRemaining = CustomGameOptions.ConsRoleblockDuration;
                                consortRole.Block();
                                break;

                            case ActionsRPC.SetMimic:
                                var glitch3 = Utils.PlayerById(reader.ReadByte());
                                var mimicTarget = Utils.PlayerById(reader.ReadByte());
                                var glitchRole3 = Role.GetRole<Glitch>(glitch3);
                                glitchRole3.TimeRemaining2 = CustomGameOptions.MimicDuration;
                                glitchRole3.MimicTarget = mimicTarget;
                                glitchRole3.Mimic();
                                break;

                            case ActionsRPC.Conceal:
                                var concealer = Utils.PlayerById(reader.ReadByte());
                                var concealerRole = Role.GetRole<Concealer>(concealer);
                                concealerRole.TimeRemaining = CustomGameOptions.ConcealDuration;
                                concealerRole.Conceal();
                                Utils.Conceal();
                                break;

                            case ActionsRPC.Shapeshift:
                                var ss = Utils.PlayerById(reader.ReadByte());
                                var ssRole = Role.GetRole<Shapeshifter>(ss);
                                ssRole.TimeRemaining = CustomGameOptions.ShapeshiftDuration;
                                ssRole.Shapeshift();
                                break;

                            case ActionsRPC.Gaze:
                                var gorg = Utils.PlayerById(reader.ReadByte());
                                var stoned = reader.ReadByte();
                                var gorgon = Role.GetRole<Gorgon>(gorg);
                                gorgon.Gazed.Add(stoned);
                                break;
                        }

                        break;

                    case CustomRPC.WinLose:
                        var id7 = reader.ReadByte();

                        switch ((WinLoseRPC)id7)
                        {
                            case WinLoseRPC.CrewWin:
                                Role.CrewWin = true;
                                break;

                            case WinLoseRPC.IntruderWin:
                                Role.IntruderWin = true;
                                break;

                            case WinLoseRPC.SyndicateWin:
                                Role.SyndicateWin = true;
                                break;

                            case WinLoseRPC.UndeadWin:
                                Role.UndeadWin = true;
                                break;

                            case WinLoseRPC.ReanimatedWin:
                                Role.ReanimatedWin = true;
                                break;

                            case WinLoseRPC.SectWin:
                                Role.SectWin = true;
                                break;

                            case WinLoseRPC.CabalWin:
                                Role.CabalWin = true;
                                break;

                            case WinLoseRPC.NobodyWins:
                                Role.NobodyWins = true;
                                Objectifier.NobodyWins = true;
                                break;

                            case WinLoseRPC.AllNeutralsWin:
                                Role.AllNeutralsWin = true;
                                break;

                            case WinLoseRPC.AllNKsWin:
                                Role.NKWins = true;
                                break;

                            case WinLoseRPC.SameNKWins:
                            case WinLoseRPC.SoloNKWins:
                                var nkRole = Role.GetRole(Utils.PlayerById(reader.ReadByte()));

                                switch (nkRole.RoleType)
                                {
                                    case RoleEnum.Glitch:
                                        Role.GlitchWins = true;
                                        break;

                                    case RoleEnum.Arsonist:
                                        Role.ArsonistWins = true;
                                        break;

                                    case RoleEnum.Cryomaniac:
                                        Role.CryomaniacWins = true;
                                        break;

                                    case RoleEnum.Juggernaut:
                                        Role.JuggernautWins = true;
                                        break;

                                    case RoleEnum.Murderer:
                                        Role.MurdererWins = true;
                                        break;

                                    case RoleEnum.Werewolf:
                                        Role.WerewolfWins = true;
                                        break;

                                    case RoleEnum.SerialKiller:
                                        Role.SerialKillerWins = true;
                                        break;
                                }

                                if ((WinLoseRPC)id7 == WinLoseRPC.SameNKWins)
                                {
                                    foreach (var role in Role.GetRoles(nkRole.RoleType))
                                    {
                                        if (!role.Player.Data.Disconnected && role.NotDefective)
                                            role.Winner = true;
                                    }
                                }

                                nkRole.Winner = true;
                                break;

                            case WinLoseRPC.InfectorsWin:
                                Role.InfectorsWin = true;
                                break;

                            case WinLoseRPC.JesterWin:
                                Role.GetRole<Jester>(Utils.PlayerById(reader.ReadByte())).VotedOut = true;
                                break;

                            case WinLoseRPC.CannibalWin:
                                Role.GetRole<Cannibal>(Utils.PlayerById(reader.ReadByte())).CannibalWin = true;
                                break;

                            case WinLoseRPC.ExecutionerWin:
                                Role.GetRole<Executioner>(Utils.PlayerById(reader.ReadByte())).TargetVotedOut = true;
                                break;

                            case WinLoseRPC.BountyHunterWin:
                                Role.GetRole<BountyHunter>(Utils.PlayerById(reader.ReadByte())).TargetKilled = true;
                                break;

                            case WinLoseRPC.TrollWin:
                                Role.GetRole<Troll>(Utils.PlayerById(reader.ReadByte())).Killed = true;
                                break;

                            case WinLoseRPC.ActorWin:
                                Role.GetRole<Actor>(Utils.PlayerById(reader.ReadByte())).Guessed = true;
                                break;

                            case WinLoseRPC.GuesserWin:
                                Role.GetRole<Guesser>(Utils.PlayerById(reader.ReadByte())).TargetGuessed = true;
                                break;

                            case WinLoseRPC.CorruptedWin:
                                Objectifier.CorruptedWins = true;

                                if (reader.ReadBoolean())
                                {
                                    foreach (Corrupted corr in Objectifier.GetObjectifiers(ObjectifierEnum.Corrupted).Cast<Corrupted>())
                                        corr.Winner = true;
                                }
                                else
                                {
                                    Objectifier.GetObjectifier(Utils.PlayerById(reader.ReadByte())).Winner = true;
                                }

                                break;

                            case WinLoseRPC.LoveWin:
                                Objectifier.LoveWins = true;
                                var lover = Objectifier.GetObjectifier<Lovers>(Utils.PlayerById(reader.ReadByte()));
                                lover.Winner = true;
                                Objectifier.GetObjectifier(lover.OtherLover).Winner = true;
                                break;

                            case WinLoseRPC.OverlordWin:
                                Objectifier.OverlordWins = true;

                                foreach (Overlord ov in Objectifier.GetObjectifiers(ObjectifierEnum.Overlord).Cast<Overlord>())
                                    ov.Winner = true;

                                break;

                            case WinLoseRPC.TaskmasterWin:
                                Objectifier.TaskmasterWins = true;
                                var tm = Objectifier.GetObjectifier<Taskmaster>(Utils.PlayerById(reader.ReadByte()));
                                tm.Winner = true;
                                break;

                            case WinLoseRPC.RivalWin:
                                Objectifier.RivalWins = true;
                                var rival = Objectifier.GetObjectifier<Rivals>(Utils.PlayerById(reader.ReadByte()));
                                rival.Winner = true;
                                break;

                            case WinLoseRPC.PhantomWin:
                                Role.PhantomWins = true;
                                var phantom3 = Role.GetRole<Phantom>(Utils.PlayerById(reader.ReadByte()));
                                phantom3.CompletedTasks = true;
                                phantom3.Winner = true;
                                break;
                        }

                        Utils.EndGame();
                        break;
                }
            }
        }

        [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SelectRoles))]
        public static class RPCSetRole
        {
            public static void Postfix()
            {
                Utils.LogSomething("RPC SET ROLE");
                var infected = GameData.Instance.AllPlayers.ToArray().Where(o => o.IsImpostor());

                ResetEverything();

                Utils.LogSomething("Cleared Variables");

                var startWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Start, SendOption.Reliable);
                AmongUsClient.Instance.FinishRpcImmediately(startWriter);

                BeginRoleGen(infected.ToList());

                foreach (var player in PlayerControl.AllPlayerControls)
                    player.MaxReportDistance = CustomGameOptions.ReportDistance;
            }
        }
    }
}