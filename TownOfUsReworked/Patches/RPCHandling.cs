using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Hazel;
using Reactor.Utilities;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Modifiers;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;
using TownOfUsReworked.PlayerLayers.Modifiers.Modifiers;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Objectifiers.PhantomMod;
using TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Abilities.AssassinMod;
using TownOfUsReworked.PlayerLayers.Abilities.RevealerMod;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.SwapperMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TimeLordMod;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.ExecutionerMod;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GuardianAngelMod;
using UnityEngine;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.ConsigliereMod;
using Coroutine = TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.JanitorMod.Coroutine;
using Eat = TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.CannibalMod.Coroutine;
using Revive = TownOfUsReworked.PlayerLayers.Roles.CrewRoles.AltruistMod.Coroutine;
using PerformKillButton = TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.AmnesiacMod.PerformKillButton;
using PerformDeclareButton = TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GodfatherMod.PerformKill;
using PerformSidekickButton = TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.RebelMod.PerformKill;
using PerformShiftButton = TownOfUsReworked.PlayerLayers.Roles.CrewRoles.ShifterMod.PerformShiftButton;
using PerformConvertButton = TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.DraculaMod.PerformConvertButton;
using Turn = TownOfUsReworked.PlayerLayers.Objectifiers.TraitorMod;
using Reveal = TownOfUsReworked.PlayerLayers.Abilities.RevealerMod;
using Reactor.Networking.Extensions;

namespace TownOfUsReworked.Patches
{
    public static class RPCHandling
    {
        private static readonly List<(Type, CustomRPC, int, bool)> CrewAuditorRoles = new List<(Type, CustomRPC, int, bool)>();
        private static readonly List<(Type, CustomRPC, int, bool)> CrewInvestigativeRoles = new List<(Type, CustomRPC, int, bool)>();
        private static readonly List<(Type, CustomRPC, int, bool)> CrewKillingRoles = new List<(Type, CustomRPC, int, bool)>();
        private static readonly List<(Type, CustomRPC, int, bool)> CrewProtectiveRoles = new List<(Type, CustomRPC, int, bool)>();
        private static readonly List<(Type, CustomRPC, int, bool)> CrewSovereignRoles = new List<(Type, CustomRPC, int, bool)>();
        private static readonly List<(Type, CustomRPC, int, bool)> CrewSupportRoles = new List<(Type, CustomRPC, int, bool)>();
        private static readonly List<(Type, CustomRPC, int, bool)> CrewRoles = new List<(Type, CustomRPC, int, bool)>();
        
        private static readonly List<(Type, CustomRPC, int, bool)> NeutralEvilRoles = new List<(Type, CustomRPC, int, bool)>();
        private static readonly List<(Type, CustomRPC, int, bool)> NeutralBenignRoles = new List<(Type, CustomRPC, int, bool)>();
        private static readonly List<(Type, CustomRPC, int, bool)> NeutralKillingRoles = new List<(Type, CustomRPC, int, bool)>();
        private static readonly List<(Type, CustomRPC, int, bool)> NeutralNeophyteRoles = new List<(Type, CustomRPC, int, bool)>();
        private static readonly List<(Type, CustomRPC, int, bool)> NeutralRoles = new List<(Type, CustomRPC, int, bool)>();

        private static readonly List<(Type, CustomRPC, int, bool)> IntruderDeceptionRoles = new List<(Type, CustomRPC, int, bool)>();
        private static readonly List<(Type, CustomRPC, int, bool)> IntruderConcealingRoles = new List<(Type, CustomRPC, int, bool)>();
        private static readonly List<(Type, CustomRPC, int, bool)> IntruderKillingRoles = new List<(Type, CustomRPC, int, bool)>();
        private static readonly List<(Type, CustomRPC, int, bool)> IntruderSupportRoles = new List<(Type, CustomRPC, int, bool)>();
        private static readonly List<(Type, CustomRPC, int, bool)> IntruderRoles = new List<(Type, CustomRPC, int, bool)>();

        private static readonly List<(Type, CustomRPC, int, bool)> SyndicateDisruptionRoles = new List<(Type, CustomRPC, int, bool)>();
        private static readonly List<(Type, CustomRPC, int, bool)> SyndicateKillingRoles = new List<(Type, CustomRPC, int, bool)>();
        private static readonly List<(Type, CustomRPC, int, bool)> SyndicatePowerRoles = new List<(Type, CustomRPC, int, bool)>();
        private static readonly List<(Type, CustomRPC, int, bool)> SyndicateSupportRoles = new List<(Type, CustomRPC, int, bool)>();
        private static readonly List<(Type, CustomRPC, int, bool)> SyndicateRoles = new List<(Type, CustomRPC, int, bool)>();

        private static readonly List<(Type, CustomRPC, int)> GlobalModifiers = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> ProfessionalModifiers = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> BaitModifiers = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> DiseasedModifiers = new List<(Type, CustomRPC, int)>();

        private static readonly List<(Type, CustomRPC, int)> GlobalAbilityGet = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> IntruderAbilityGet = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> CrewAbilityGet = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> TunnelerAbilityGet = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> NonEvilAbilityGet = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> TaskedAbilityGet = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> SnitchRevealerAbilityGet = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> NonVentingAbilityGet = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> EvilAbilityGet = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> SyndicateAbilityGet = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> NeutralAbilityGet = new List<(Type, CustomRPC, int)>();

        private static readonly List<(Type, CustomRPC, int)> CrewObjectifierGet = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> CorruptedObjectifierGet = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> NeutralObjectifierGet = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> OverlordObjectifierGet = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> LoverRivalObjectifierGet = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> GlobalObjectifierGet = new List<(Type, CustomRPC, int)>();

        private static readonly bool IsAA = CustomGameOptions.GameMode == GameMode.AllAny;
        private static readonly bool IsCustom = CustomGameOptions.GameMode == GameMode.Custom;
        private static readonly bool IsClassic = CustomGameOptions.GameMode == GameMode.Classic;
        private static readonly bool IsKilling = CustomGameOptions.GameMode == GameMode.KillingOnly;

        private static void SortThings(List<(Type, CustomRPC, int, bool)> items, int max, int min)
        {
            items.Shuffle();

            if (items.Count < max)
                max = items.Count;

            if (min > max)
            {
                var temp = min;
                min = max;
                max = temp;
            }

            if (items.Count < max)
                max = items.Count;

            var amount = Random.RandomRangeInt(min, max + 1);

            if (amount < 0)
                amount = 0;

            items.Sort((a, b) =>
            {
                var a_ = a.Item3 == 100 ? 0 : 100;
                var b_ = b.Item3 == 100 ? 0 : 100;
                return a_.CompareTo(b_);
            });

            var certainItems = 0;
            var odds = 0;

            foreach (var role in items)
            {
                if (role.Item3 == 100)
                    certainItems += 1;
                else
                    odds += role.Item3;
            }

            while (certainItems < amount)
            {
                var num = certainItems;
                var random = Random.RandomRangeInt(0, odds);
                var rolePicked = false;

                while (num < items.Count && rolePicked == false)
                {
                    random -= items[num].Item3;

                    if (random < 0)
                    {
                        odds -= items[num].Item3;
                        var role = items[num];
                        items.Remove(role);
                        items.Insert(0, role);
                        certainItems += 1;
                        rolePicked = true;
                    }

                    num += 1;
                }
            }

            while (items.Count > amount && amount > 0)
                items.RemoveAt(items.Count - 1);
            
            items.Shuffle();
        }

        private static void SortThings(List<(Type, CustomRPC, int)> items, int max, int min)
        {
            items.Shuffle();

            if (items.Count < max)
                max = items.Count;

            if (min > max)
            {
                var temp = min;
                min = max;
                max = temp;
            }

            if (items.Count < max)
                max = items.Count;

            var amount = Random.RandomRangeInt(min, max + 1);

            if (amount < 0)
                amount = 0;

            items.Sort((a, b) =>
            {
                var a_ = a.Item3 == 100 ? 0 : 100;
                var b_ = b.Item3 == 100 ? 0 : 100;
                return a_.CompareTo(b_);
            });

            var certainItems = 0;
            var odds = 0;

            foreach (var role in items)
            {
                if (role.Item3 == 100)
                    certainItems += 1;
                else
                    odds += role.Item3;
            }

            while (certainItems < amount)
            {
                var num = certainItems;
                var random = Random.RandomRangeInt(0, odds);
                var rolePicked = false;

                while (num < items.Count && rolePicked == false)
                {
                    random -= items[num].Item3;

                    if (random < 0)
                    {
                        odds -= items[num].Item3;
                        var role = items[num];
                        items.Remove(role);
                        items.Insert(0, role);
                        certainItems += 1;
                        rolePicked = true;
                    }

                    num += 1;
                }
            }

            while (items.Count > amount && amount > 0)
                items.RemoveAt(items.Count - 1);
            
            items.Shuffle();
        }

        private static void RoleGen(List<GameData.PlayerInfo> infected)
        {
            var impostors = Utils.GetImpostors(infected);
            var crewmates = Utils.GetCrewmates(impostors);

            crewmates.Shuffle();
            impostors.Shuffle();

            if (!IsKilling)
            {
                if (IsClassic || IsCustom)
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
                            var random = Random.RandomRangeInt(0, 4);

                            switch (random)
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
                                
                                default:
                                    break;
                            }

                            maxIntSum = maxIC + maxID + maxIK + maxIS;
                        }

                        while (minIntSum > minInt)
                        {
                            var random = Random.RandomRangeInt(0, 4);

                            switch (random)
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
                                
                                default:
                                    break;
                            }

                            minIntSum = minIC + minID + minIK + minIS;
                        }

                        SortThings(IntruderConcealingRoles, maxIC, minIC);
                        SortThings(IntruderDeceptionRoles, maxID, minID);
                        SortThings(IntruderKillingRoles, maxIK, minIK);
                        SortThings(IntruderSupportRoles, maxIS, minIS);

                        IntruderRoles.AddRange(IntruderConcealingRoles);
                        IntruderRoles.AddRange(IntruderDeceptionRoles);
                        IntruderRoles.AddRange(IntruderKillingRoles);
                        IntruderRoles.AddRange(IntruderSupportRoles);

                        while (maxInt > CustomGameOptions.IntruderCount)
                            maxInt--;

                        while (minInt > CustomGameOptions.IntruderCount)
                            minInt--;

                        SortThings(IntruderRoles, maxInt, minInt);

                        while (IntruderRoles.Count < CustomGameOptions.IntruderCount)
                            IntruderRoles.Add((typeof(Impostor), CustomRPC.SetImpostor, 100, false));
                        
                        IntruderRoles.Shuffle();
                    }

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
                        var random = Random.RandomRangeInt(0, 4);

                        switch (random)
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
                            
                            default:
                                break;
                        }

                        maxNeutSum = maxNE + maxNB + maxNK + maxNN;
                    }

                    while (minNeutSum > minNeut)
                    {
                        var random = Random.RandomRangeInt(0, 4);

                        switch (random)
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
                                
                            default:
                                break;
                        }

                        minNeutSum = minNE + minNB + minNK + minNN;
                    }

                    SortThings(NeutralBenignRoles, maxNB, minNB);
                    SortThings(NeutralEvilRoles, maxNE, minNE);
                    SortThings(NeutralKillingRoles, maxNK, minNK);
                    SortThings(NeutralNeophyteRoles, maxNN, minNN);

                    NeutralRoles.AddRange(NeutralBenignRoles);
                    NeutralRoles.AddRange(NeutralEvilRoles);
                    NeutralRoles.AddRange(NeutralKillingRoles);
                    NeutralRoles.AddRange(NeutralNeophyteRoles);

                    SortThings(NeutralRoles, maxNeut, minNeut);

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
                        var random = Random.RandomRangeInt(0, 4);

                        switch (random)
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
                            
                            default:
                                break;
                        }

                        maxSynSum = maxSSu + maxSD + maxSyK + maxSP;
                    }

                    while (minSynSum > minSyn)
                    {
                        var random = Random.RandomRangeInt(0, 4);

                        switch (random)
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
                                
                            default:
                                break;
                        }

                        minSynSum = minSSu + minSD + minSyK + minSP;
                    }

                    SortThings(SyndicateSupportRoles, maxSSu, minSSu);
                    SortThings(SyndicateDisruptionRoles, maxSD, minSD);
                    SortThings(SyndicateKillingRoles, maxSyK, minSyK);
                    SortThings(SyndicatePowerRoles, maxSP, minSP);

                    SyndicateRoles.AddRange(SyndicateSupportRoles);
                    SyndicateRoles.AddRange(SyndicateDisruptionRoles);
                    SyndicateRoles.AddRange(SyndicateKillingRoles);
                    SyndicateRoles.AddRange(SyndicatePowerRoles);

                    while (maxSyn > CustomGameOptions.SyndicateCount)
                        maxSyn--;

                    while (minSyn > CustomGameOptions.SyndicateCount)
                        minSyn--;

                    SortThings(SyndicateRoles, maxSyn, minSyn);

                    while (SyndicateRoles.Count < CustomGameOptions.SyndicateCount)
                        SyndicateRoles.Add((typeof(Anarchist), CustomRPC.SetAnarchist, 100, false));
                    
                    SyndicateRoles.Shuffle();
                    
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
                        var random = Random.RandomRangeInt(0, 6);

                        switch (random)
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
                            
                            default:
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
                            
                            default:
                                break;
                        }

                        minCrewSum = minCA + minCI + minCK + minCP + minCS + minCSv;
                    }

                    SortThings(CrewAuditorRoles, maxCA, minCA);
                    SortThings(CrewInvestigativeRoles, maxCI, minCI);
                    SortThings(CrewKillingRoles, maxCK, minCK);
                    SortThings(CrewProtectiveRoles, maxCP, minCP);
                    SortThings(CrewSupportRoles, maxCS, minCS);
                    SortThings(CrewSovereignRoles, maxCSv, minCSv);

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

                    SortThings(CrewRoles, maxCrew, minCrew);

                    while (CrewRoles.Count < crewmates.Count - CustomGameOptions.SyndicateCount - NeutralRoles.Count)
                        CrewRoles.Add((typeof(Crewmate), CustomRPC.SetCrewmate, 100, false));
                    
                    CrewRoles.Shuffle();
                }
                else if (IsAA)
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
                }

                var NonIntruderRoles = new List<(Type, CustomRPC, int, bool)>();

                NonIntruderRoles.AddRange(CrewRoles);
                NonIntruderRoles.AddRange(NeutralRoles);

                if (!CustomGameOptions.AltImps)
                    NonIntruderRoles.AddRange(SyndicateRoles);
                else
                {
                    IntruderRoles.Clear();
                    IntruderRoles.AddRange(SyndicateRoles);
                }

                var nonIntruderRoles = new List<(Type, CustomRPC, int, bool)>();
                var impRoles = new List<(Type, CustomRPC, int, bool)>();

                if (IsAA)
                {
                    while (nonIntruderRoles.Count <= crewmates.Count && NonIntruderRoles.Count > 0)
                    {
                        NonIntruderRoles.Shuffle();
                        nonIntruderRoles.Add(NonIntruderRoles[0]);

                        if (NonIntruderRoles[0].Item4 == true && CustomGameOptions.EnableUniques)
                            NonIntruderRoles.Remove(NonIntruderRoles[0]);
                    }

                    while (impRoles.Count <= impostors.Count && IntruderRoles.Count > 0)
                    {
                        IntruderRoles.Shuffle();
                        impRoles.Add(IntruderRoles[0]);

                        if (IntruderRoles[0].Item4 == true && CustomGameOptions.EnableUniques)
                            IntruderRoles.Remove(IntruderRoles[0]);
                    }
                }

                nonIntruderRoles.Shuffle();
                impRoles.Shuffle();

                if (IsAA)
                {
                    foreach (var (type, rpc, _, unique) in nonIntruderRoles)
                        Role.Gen<Role>(type, crewmates, rpc);
                        
                    foreach (var (type, rpc, _, unique) in impRoles)
                        Role.Gen<Role>(type, impostors, rpc);
                }
                else
                {
                    foreach (var (type, rpc, _, unique) in NonIntruderRoles)
                        Role.Gen<Role>(type, crewmates, rpc);
                        
                    foreach (var (type, rpc, _, unique) in IntruderRoles)
                        Role.Gen<Role>(type, impostors, rpc);
                }

                var crewCount = CrewRoles.Count;
                var neutralCount = NeutralRoles.Count;
                var intruderCount = CustomGameOptions.AltImps ? 0 : IntruderRoles.Count;
                var syndicateCount = SyndicateRoles.Count;
                var allCount = crewmates.Count + impostors.Count;

                SortThings(GlobalModifiers, allCount, allCount);
                SortThings(ProfessionalModifiers, allCount, allCount);
                SortThings(DiseasedModifiers, allCount, allCount);
                SortThings(BaitModifiers, allCount, allCount);

                SortThings(IntruderAbilityGet, intruderCount, intruderCount);
                SortThings(CrewAbilityGet, crewCount, crewCount);
                SortThings(SnitchRevealerAbilityGet, crewCount, crewCount);
                SortThings(TunnelerAbilityGet, crewCount, crewCount);
                SortThings(TaskedAbilityGet, crewCount + neutralCount, crewCount + neutralCount);
                SortThings(NeutralAbilityGet, neutralCount, neutralCount);
                SortThings(SyndicateAbilityGet, syndicateCount, syndicateCount);
                
                SortThings(CrewObjectifierGet, crewCount, crewCount);
                SortThings(CorruptedObjectifierGet, crewCount, crewCount);
                SortThings(NeutralObjectifierGet, neutralCount, neutralCount);
                SortThings(OverlordObjectifierGet, neutralCount, neutralCount);
                SortThings(GlobalObjectifierGet, allCount, allCount);
                SortThings(EvilAbilityGet, intruderCount + syndicateCount, intruderCount + syndicateCount);
                SortThings(NonEvilAbilityGet, crewCount + neutralCount, crewCount + neutralCount);

                var canHaveLoverorRival = PlayerControl.AllPlayerControls.ToArray().ToList();
                var canHaveObjectifier = PlayerControl.AllPlayerControls.ToArray().ToList();
                var canHaveObjectifier2 = PlayerControl.AllPlayerControls.ToArray().ToList();
                var canHaveObjectifier3 = PlayerControl.AllPlayerControls.ToArray().ToList();

                canHaveLoverorRival.RemoveAll(player => player.Is(RoleEnum.Altruist) || player.Is(RoleEnum.Troll) || player.Is(RoleEnum.Jester) || player.Is(RoleEnum.Shifter));
                canHaveLoverorRival.Shuffle();

                canHaveObjectifier.RemoveAll(player => !player.Is(Faction.Neutral));
                canHaveObjectifier.Shuffle();

                canHaveObjectifier2.RemoveAll(player => !player.Is(Faction.Crew));
                canHaveObjectifier2.Shuffle();
                
                while (canHaveObjectifier.Count > 0 && NeutralObjectifierGet.Count > 0)
                {
                    var (type, rpc, _) = NeutralObjectifierGet.TakeFirst();
                    Role.Gen<Objectifier>(type, canHaveObjectifier.TakeFirst(), rpc);
                }
                
                while (canHaveObjectifier2.Count > 0 && CrewObjectifierGet.Count > 0)
                {
                    var (type, rpc, _) = CrewObjectifierGet.TakeFirst();
                    Role.Gen<Objectifier>(type, canHaveObjectifier2.TakeFirst(), rpc);
                }

                while (canHaveLoverorRival.Count > 0 && GlobalObjectifierGet.Count > 0)
                {
                    var (type, rpc, _) = GlobalObjectifierGet.TakeFirst();

                    if (rpc == CustomRPC.SetCouple && canHaveLoverorRival.Count >= 5)
                        Lovers.Gen(canHaveLoverorRival);
                    else if (rpc == CustomRPC.SetDuo && canHaveLoverorRival.Count >= 5)
                        Rivals.Gen(canHaveLoverorRival);
                    else
                        Role.Gen<Objectifier>(type, canHaveObjectifier3.TakeFirst(), rpc);  
                }

                var canHaveAbility = PlayerControl.AllPlayerControls.ToArray().ToList();
                var canHaveAbility2 = PlayerControl.AllPlayerControls.ToArray().ToList();
                var canHaveAbility3 = PlayerControl.AllPlayerControls.ToArray().ToList();
                var canHaveAbility4 = PlayerControl.AllPlayerControls.ToArray().ToList();
                var canHaveAbility5 = PlayerControl.AllPlayerControls.ToArray().ToList();
                var canHaveAbility6 = PlayerControl.AllPlayerControls.ToArray().ToList();
                var canHaveAbility7 = PlayerControl.AllPlayerControls.ToArray().ToList();
                var canHaveAbility8 = PlayerControl.AllPlayerControls.ToArray().ToList();
                var canHaveAbility9 = PlayerControl.AllPlayerControls.ToArray().ToList();
                var canHaveAbility10 = PlayerControl.AllPlayerControls.ToArray().ToList();
                var canHaveAbility11 = PlayerControl.AllPlayerControls.ToArray().ToList();

                canHaveAbility.RemoveAll(player => !player.Is(Faction.Intruder) || (player.Is(RoleEnum.Consigliere) && CustomGameOptions.ConsigInfo == ConsigInfo.Role));
                canHaveAbility.Shuffle();

                canHaveAbility2.RemoveAll(player => !(player.Is(RoleAlignment.NeutralNeo) || player.Is(RoleAlignment.NeutralKill)));
                canHaveAbility2.Shuffle();

                canHaveAbility3.RemoveAll(player => !player.Is(Faction.Crew));
                canHaveAbility3.Shuffle();

                canHaveAbility4.RemoveAll(player => !player.Is(Faction.Syndicate));
                canHaveAbility4.Shuffle();

                canHaveAbility5.RemoveAll(player => !player.Is(Faction.Crew) || player.Is(RoleEnum.Engineer));
                canHaveAbility5.Shuffle();

                canHaveAbility6.RemoveAll(player => !player.Is(Faction.Neutral));
                canHaveAbility6.Shuffle();

                canHaveAbility7.RemoveAll(player => !player.Is(Faction.Crew) || player.Is(ObjectifierEnum.Traitor));
                canHaveAbility7.Shuffle();

                canHaveAbility8.RemoveAll(player => !player.CanDoTasks());
                canHaveAbility8.Shuffle();

                canHaveAbility9.RemoveAll(player => !(player.Is(Faction.Crew) || player.Is(RoleAlignment.NeutralBen) || player.Is(RoleAlignment.NeutralEvil)));
                canHaveAbility9.Shuffle();

                canHaveAbility10.RemoveAll(player => !(player.Is(Faction.Intruder) || player.Is(Faction.Syndicate)));
                canHaveAbility10.Shuffle();
                
                while (canHaveAbility7.Count > 0 && SnitchRevealerAbilityGet.Count > 0)
                {
                    var (type, rpc, _) = SnitchRevealerAbilityGet.TakeFirst();
                    Role.Gen<Ability>(type, canHaveAbility7.TakeFirst(), rpc);
                }

                if (CustomGameOptions.WhoCanVent == WhoCanVentOptions.Default)
                {
                    while (canHaveAbility5.Count > 0 && TunnelerAbilityGet.Count > 0)
                    {
                        var (type, rpc, _) = TunnelerAbilityGet.TakeFirst();
                        Role.Gen<Ability>(type, canHaveAbility5.TakeFirst(), rpc);
                    }
                }
                
                while (canHaveAbility9.Count > 0 && NonEvilAbilityGet.Count > 0)
                {
                    var (type, rpc, _) = NonEvilAbilityGet.TakeFirst();
                    Role.Gen<Ability>(type, canHaveAbility9.TakeFirst(), rpc);
                }
                
                while (canHaveAbility10.Count > 0 && EvilAbilityGet.Count > 0)
                {
                    var (type, rpc, _) = EvilAbilityGet.TakeFirst();
                    Role.Gen<Ability>(type, canHaveAbility10.TakeFirst(), rpc);
                }
                
                while (canHaveAbility8.Count > 0 && TaskedAbilityGet.Count > 0)
                {
                    var (type, rpc, _) = TaskedAbilityGet.TakeFirst();
                    Role.Gen<Ability>(type, canHaveAbility8.TakeFirst(), rpc);
                }
                
                while (canHaveAbility3.Count > 0 && CrewAbilityGet.Count > 0)
                {
                    var (type, rpc, _) = CrewAbilityGet.TakeFirst();
                    Role.Gen<Ability>(type, canHaveAbility3.TakeFirst(), rpc);
                }
                
                while (canHaveAbility.Count > 0 && IntruderAbilityGet.Count > 0)
                {
                    var (type, rpc, _) = IntruderAbilityGet.TakeFirst();
                    Role.Gen<Ability>(type, canHaveAbility.TakeFirst(), rpc);
                }
                
                while (canHaveAbility2.Count > 0 && NeutralAbilityGet.Count > 0)
                {
                    var (type, rpc, _) = NeutralAbilityGet.TakeFirst();
                    Role.Gen<Ability>(type, canHaveAbility2.TakeFirst(), rpc);
                }
                
                while (canHaveAbility4.Count > 0 && SyndicateAbilityGet.Count > 0)
                {
                    var (type, rpc, _) = SyndicateAbilityGet.TakeFirst();
                    Role.Gen<Ability>(type, canHaveAbility4.TakeFirst(), rpc);
                }

                while (canHaveAbility11.Count > 0 && GlobalAbilityGet.Count > 0)
                {
                    var (type, rpc, _) = GlobalAbilityGet.TakeFirst();
                    Role.Gen<Ability>(type, canHaveAbility11.TakeFirst(), rpc);
                }

                var canHaveModifier1 = PlayerControl.AllPlayerControls.ToArray().ToList();
                var canHaveModifier2 = PlayerControl.AllPlayerControls.ToArray().ToList();
                var canHaveModifier3 = PlayerControl.AllPlayerControls.ToArray().ToList();
                var canHaveModifier4 = PlayerControl.AllPlayerControls.ToArray().ToList();

                canHaveModifier1.RemoveAll(player => player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.Shifter) || player.Is(RoleEnum.Altruist) || player.Is(RoleEnum.Troll));
                canHaveModifier1.Shuffle();

                canHaveModifier2.RemoveAll(player => player.Is(RoleEnum.Altruist) || player.Is(RoleEnum.Troll));
                canHaveModifier2.Shuffle();

                canHaveModifier3.RemoveAll(player => !player.Is(AbilityEnum.Assassin));
                canHaveModifier3.Shuffle();
                
                while (canHaveModifier1.Count > 0 && BaitModifiers.Count > 0)
                {
                    var (type, rpc, _) = BaitModifiers.TakeFirst();
                    Role.Gen<Modifier>(type, canHaveModifier1.TakeFirst(), rpc);
                }

                while (canHaveModifier2.Count > 0 && DiseasedModifiers.Count > 0)
                {
                    var (type, rpc, _) = DiseasedModifiers.TakeFirst();
                    Role.Gen<Modifier>(type, canHaveModifier2.TakeFirst(), rpc);
                }

                while (canHaveModifier3.Count > 0 && ProfessionalModifiers.Count > 0)
                {
                    var (type, rpc, _) = ProfessionalModifiers.TakeFirst();
                    Role.Gen<Modifier>(type, canHaveModifier3.TakeFirst(), rpc);
                }

                while (canHaveModifier4.Count > 0 && GlobalModifiers.Count > 0)
                {
                    var (type, rpc, _) = GlobalModifiers.TakeFirst();
                    Role.Gen<Modifier>(type, canHaveModifier4.TakeFirst(), rpc);
                }

                var exeTargets = new List<PlayerControl>();
                var gaTargets = new List<PlayerControl>();
                var goodRecruits = new List<PlayerControl>();
                var evilRecruits = new List<PlayerControl>();
                
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Is(Faction.Crew))
                    {
                        if (!player.Is(RoleEnum.Altruist))
                            gaTargets.Add(player);
                        
                        if (!player.Is(ObjectifierEnum.Traitor))
                            goodRecruits.Add(player);
                            
                        if (!player.Is(RoleAlignment.CrewSov) && !player.Is(ObjectifierEnum.Traitor))
                            exeTargets.Add(player);
                    }
                    else if (player.Is(Faction.Neutral))
                    {
                        if (!player.Is(RoleEnum.Executioner) && !player.Is(RoleEnum.Troll) && !player.Is(RoleEnum.GuardianAngel) && !player.Is(RoleEnum.Jester))
                        {
                            gaTargets.Add(player);
                                
                            if (player.Is(RoleAlignment.NeutralKill))
                                evilRecruits.Add(player);
                            else if (player.Is(RoleAlignment.NeutralEvil))
                                goodRecruits.Add(player);
                        }

                        if (CustomGameOptions.ExeCanHaveNeutralTargets)
                            exeTargets.Add(player);
                    }
                    else if (player.Is(Faction.Intruder) || player.Is(Faction.Syndicate))
                    {
                        gaTargets.Add(player);
                        evilRecruits.Add(player);

                        if ((player.Is(Faction.Intruder) && CustomGameOptions.ExeCanHaveIntruderTargets) || (player.Is(Faction.Syndicate) && CustomGameOptions.ExeCanHaveSyndicateTargets))
                            exeTargets.Add(player);
                    }
                }

                if (CustomGameOptions.ExecutionerOn > 0)
                {
                    foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                    {
                        exe.TargetPlayer = null;

                        if (exeTargets.Count > 0)
                        {
                            while (exe.TargetPlayer == null || exe.TargetPlayer == exe.Player)
                            {
                                exeTargets.Shuffle();
                                var exeNum = Random.RandomRangeInt(0, exeTargets.Count - 1);
                                exe.TargetPlayer = exeTargets[exeNum];
                            }

                            exeTargets.Remove(exe.TargetPlayer);

                            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetTarget, SendOption.Reliable, -1);
                            writer.Write(exe.Player.PlayerId);
                            writer.Write(exe.TargetPlayer.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage($"Exe Target = {exe.TargetPlayer.name}");
                        }
                    }
                }
                
                if (CustomGameOptions.GuardianAngelOn > 0)
                {
                    foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                    {
                        ga.TargetPlayer = null;
                        
                        if (gaTargets.Count > 0)
                        {
                            while (ga.TargetPlayer == null || ga.TargetPlayer == ga.Player)
                            {
                                gaTargets.Shuffle();
                                var gaNum = Random.RandomRangeInt(0, gaTargets.Count - 1);
                                ga.TargetPlayer = gaTargets[gaNum];
                            }

                            gaTargets.Remove(ga.TargetPlayer);

                            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetGATarget, SendOption.Reliable, -1);
                            writer.Write(ga.Player.PlayerId);
                            writer.Write(ga.TargetPlayer.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage($"GA Target = {ga.TargetPlayer.name}");
                        }
                    }
                }

                if (CustomGameOptions.JackalOn > 0)
                {
                    foreach (Jackal jackal in Role.GetRoles(RoleEnum.Jackal))
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
                            (Role.GetRole(jackal.GoodRecruit)).IsRecruit = true;

                            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetGoodRecruit, SendOption.Reliable, -1);
                            writer.Write(jackal.Player.PlayerId);
                            writer.Write(jackal.GoodRecruit.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage($"Good Recruit = {jackal.GoodRecruit.name}");
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
                            (Role.GetRole(jackal.EvilRecruit)).SubFaction = SubFaction.Cabal;
                            (Role.GetRole(jackal.EvilRecruit)).IsRecruit = true;

                            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetEvilRecruit, SendOption.Reliable, -1);
                            writer.Write(jackal.Player.PlayerId);
                            writer.Write(jackal.EvilRecruit.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage($"Evil Recruit = {jackal.EvilRecruit.name}");
                        }
                    }
                }

                if (CustomGameOptions.VampireHunterOn > 0 && CustomGameOptions.DraculaOn > 0)
                {
                    var VampsExist = false;

                    foreach (var player in PlayerControl.AllPlayerControls)
                    {
                        if (player.Is(SubFaction.Undead))
                        {
                            VampsExist = true;
                            break;
                        }
                    }

                    if (!VampsExist)
                    {
                        foreach (VampireHunter vh in Role.GetRoles(RoleEnum.VampireHunter))
                        {
                            vh.TurnVigilante();

                            unchecked
                            {
                                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.TurnVigilante, SendOption.Reliable, -1);
                                writer.Write(vh.Player.PlayerId);
                                AmongUsClient.Instance.FinishRpcImmediately(writer);
                            }
                        }
                    }
                }
            }
            else
            {
                CrewRoles.Clear();
                IntruderRoles.Clear();
                SyndicateRoles.Clear();
                NeutralKillingRoles.Clear();

                if (!CustomGameOptions.AltImps)
                {   
                    IntruderRoles.Clear();
                    IntruderRoles.Add((typeof(Undertaker), CustomRPC.SetUndertaker, CustomGameOptions.UndertakerOn, CustomGameOptions.UniqueUndertaker));
                    IntruderRoles.Add((typeof(Morphling), CustomRPC.SetMorphling, CustomGameOptions.MorphlingOn, CustomGameOptions.UniqueMorphling));
                    IntruderRoles.Add((typeof(Blackmailer), CustomRPC.SetBlackmailer, CustomGameOptions.BlackmailerOn, CustomGameOptions.UniqueBlackmailer));
                    IntruderRoles.Add((typeof(Miner), CustomRPC.SetMiner, CustomGameOptions.MinerOn, CustomGameOptions.UniqueMiner));
                    IntruderRoles.Add((typeof(Wraith), CustomRPC.SetWraith, CustomGameOptions.WraithOn, CustomGameOptions.UniqueWraith));
                    IntruderRoles.Add((typeof(Grenadier), CustomRPC.SetGrenadier, CustomGameOptions.GrenadierOn, CustomGameOptions.UniqueGrenadier));
                    IntruderRoles.Add((typeof(TimeMaster), CustomRPC.SetTimeMaster, CustomGameOptions.TimeMasterOn, CustomGameOptions.UniqueTimeMaster));
                    IntruderRoles.Add((typeof(Disguiser), CustomRPC.SetDisguiser, CustomGameOptions.DisguiserOn, CustomGameOptions.UniqueDisguiser));
                    IntruderRoles.Add((typeof(Consigliere), CustomRPC.SetConsigliere, CustomGameOptions.ConsigliereOn, CustomGameOptions.UniqueConsigliere));
                    IntruderRoles.Add((typeof(Teleporter), CustomRPC.SetTeleporter, CustomGameOptions.TeleporterOn, CustomGameOptions.UniqueTeleporter));
                    IntruderRoles.Add((typeof(Impostor), CustomRPC.SetImpostor, 5, false));

                    if (CustomGameOptions.IntruderCount >= 3)
                        IntruderRoles.Add((typeof(Godfather), CustomRPC.SetGodfather, CustomGameOptions.GodfatherOn, CustomGameOptions.UniqueGodfather));
                }

                SyndicateRoles.Add((typeof(Anarchist), CustomRPC.SetAnarchist, 5, false));
                SyndicateRoles.Add((typeof(Concealer), CustomRPC.SetConcealer, CustomGameOptions.ConcealerOn, CustomGameOptions.UniqueConcealer));
                SyndicateRoles.Add((typeof(Shapeshifter), CustomRPC.SetShapeshifter, CustomGameOptions.ShapeshifterOn, CustomGameOptions.UniqueShapeshifter));
                SyndicateRoles.Add((typeof(Warper), CustomRPC.SetWarper, CustomGameOptions.WarperOn, CustomGameOptions.UniqueWarper));
                SyndicateRoles.Add((typeof(Gorgon), CustomRPC.SetGorgon, CustomGameOptions.GorgonOn, CustomGameOptions.UniqueGorgon));
                //SyndicateRoles.Add((typeof(Bomber), CustomRPC.SetBomber, CustomGameOptions.BomberOn, CustomGameOptions.UniqueBomber));
                SyndicateRoles.Add((typeof(Framer), CustomRPC.SetFramer, CustomGameOptions.FramerOn, CustomGameOptions.UniqueFramer));

                if (CustomGameOptions.SyndicateCount >= 3)
                    SyndicateRoles.Add((typeof(Rebel), CustomRPC.SetRebel, CustomGameOptions.RebelOn, CustomGameOptions.UniqueRebel));

                SortThings(IntruderRoles, CustomGameOptions.IntruderCount, CustomGameOptions.IntruderCount);

                if (!CustomGameOptions.AltImps && CustomGameOptions.SyndicateCount > 0)
                    SortThings(SyndicateRoles, CustomGameOptions.SyndicateCount, CustomGameOptions.SyndicateCount);
                
                if (CustomGameOptions.AltImps)
                {
                    IntruderRoles.Clear();
                    IntruderRoles.AddRange(SyndicateRoles);
                }

                NeutralKillingRoles.Add((typeof(Glitch), CustomRPC.SetGlitch, CustomGameOptions.GlitchOn, CustomGameOptions.UniqueGlitch));
                NeutralKillingRoles.Add((typeof(Werewolf), CustomRPC.SetWerewolf, CustomGameOptions.WerewolfOn, CustomGameOptions.UniqueWerewolf));
                NeutralKillingRoles.Add((typeof(SerialKiller), CustomRPC.SetSerialKiller, CustomGameOptions.SerialKillerOn, CustomGameOptions.UniqueSerialKiller));
                NeutralKillingRoles.Add((typeof(Juggernaut), CustomRPC.SetJuggernaut, CustomGameOptions.JuggernautOn, CustomGameOptions.UniqueJuggernaut));
                NeutralKillingRoles.Add((typeof(Murderer), CustomRPC.SetMurderer, CustomGameOptions.MurdererOn, CustomGameOptions.UniqueMurderer));

                if (CustomGameOptions.AddArsonist)
                    NeutralKillingRoles.Add((typeof(Arsonist), CustomRPC.SetArsonist, CustomGameOptions.ArsonistOn, CustomGameOptions.UniqueArsonist));

                if (CustomGameOptions.AddCryomaniac)
                    NeutralKillingRoles.Add((typeof(Cryomaniac), CustomRPC.SetCryomaniac, CustomGameOptions.CryomaniacOn, CustomGameOptions.UniqueCryomaniac));

                if (CustomGameOptions.AddPlaguebearer)
                {
                    if (CustomGameOptions.PestSpawn)
                        NeutralKillingRoles.Add((typeof(Pestilence), CustomRPC.SetPestilence, CustomGameOptions.PlaguebearerOn, CustomGameOptions.UniquePlaguebearer));
                    else
                        NeutralKillingRoles.Add((typeof(Plaguebearer), CustomRPC.SetPlaguebearer, CustomGameOptions.PlaguebearerOn, CustomGameOptions.UniquePlaguebearer));
                }

                NeutralKillingRoles.Shuffle();

                var neutrals = 0;

                if (NeutralKillingRoles.Count < CustomGameOptions.NeutralRoles)
                    neutrals = NeutralKillingRoles.Count;
                else
                    neutrals = CustomGameOptions.NeutralRoles;

                var spareCrew = crewmates.Count - neutrals;

                if (spareCrew > 2)
                    SortThings(NeutralKillingRoles, neutrals, neutrals);
                else
                    SortThings(NeutralKillingRoles, crewmates.Count - 3, crewmates.Count - 3);

                if (CrewRoles.Count + NeutralKillingRoles.Count > crewmates.Count)
                    SortThings(CrewRoles, crewmates.Count - NeutralKillingRoles.Count, crewmates.Count - NeutralKillingRoles.Count);
                else if (CrewRoles.Count + NeutralKillingRoles.Count < crewmates.Count)
                {
                    int vigis = (crewmates.Count - NeutralKillingRoles.Count - CrewRoles.Count) / 2;
                    int vets = (crewmates.Count - NeutralKillingRoles.Count - CrewRoles.Count) / 2;

                    while (vigis > 0)
                    {
                        CrewRoles.Add((typeof(Vigilante), CustomRPC.SetVigilante, 100, false));
                        vigis -= 1;
                        CrewRoles.Add((typeof(Veteran), CustomRPC.SetVeteran, 100, false));
                        vets -= 1;
                    }
                }

                var nonIntruderRoles = new List<(Type, CustomRPC, int, bool)>();

                nonIntruderRoles.AddRange(CrewRoles);
                nonIntruderRoles.AddRange(NeutralKillingRoles);

                if (!CustomGameOptions.AltImps)
                    nonIntruderRoles.AddRange(SyndicateRoles);
                    
                nonIntruderRoles.Shuffle();
                IntruderRoles.Shuffle();

                foreach (var (type, rpc, _, unique) in IntruderRoles)
                    Role.Gen<Role>(type, impostors, rpc);

                foreach (var (type, rpc, _, unique) in nonIntruderRoles)
                    Role.Gen<Role>(type, crewmates, rpc);
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
        public static class HandleRpc
        {
            public static void Postfix([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
            {
                byte readByte, readByte1, readByte2;
                sbyte readSByte, readSByte2;

                switch ((CustomRPC)callId)
                {
                    case CustomRPC.SetMayor:
                        new Mayor(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetRebel:
                        new Rebel(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetJackal:
                        new Jackal(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetJester:
                        new Jester(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetVigilante:
                        new Vigilante(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetConsigliere:
                        new Consigliere(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetEngineer:
                        new Engineer(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetJanitor:
                        new Janitor(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetSwapper:
                        new Swapper(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetShifter:
                        new Shifter(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetCamouflager:
                        new Camouflager(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetAmnesiac:
                        new Amnesiac(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetInvestigator:
                        new Investigator(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetTimeLord:
                        new TimeLord(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetCannibal:
                        new Cannibal(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetTorch:
                        new Torch(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetLighter:
                        new Lighter(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetDiseased:
                        new Diseased(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetBait:
                        new Bait(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetDwarf:
                        new Dwarf(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetMedic:
                        new Medic(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetDracula:
                        new Dracula(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetMorphling:
                        new Morphling(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetPoisoner:
                        new Poisoner(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetDrunk:
                        new Drunk(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetConcealer:
                        new Concealer(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetTunneler:
                        new Tunneler(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetFlincher:
                        new Flincher(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetCryomaniac:
                        new Cryomaniac(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetThief:
                        new Thief(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.FreezeDouse:
                        var cryomaniac = Utils.PlayerById(reader.ReadByte());
                        var freezedouseTarget = Utils.PlayerById(reader.ReadByte());
                        var cryomaniacRole = Role.GetRole<Cryomaniac>(cryomaniac);
                        cryomaniacRole.DousedPlayers.Add(freezedouseTarget.PlayerId);
                        cryomaniacRole.LastDoused = DateTime.UtcNow;
                        break;

                    case CustomRPC.AllFreeze:
                        var theCryomaniac = Utils.PlayerById(reader.ReadByte());
                        var theCryomaniacRole = Role.GetRole<Cryomaniac>(theCryomaniac);
                        theCryomaniacRole.FreezeUsed = true;
                        break;

                    case CustomRPC.CryomaniacWin:
                        var theCryomaniacTheRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Cryomaniac && !x.IsRecruit && ((Cryomaniac)x).CryoWins);
                        ((Cryomaniac)theCryomaniacTheRole).Wins();
                        break;

                    case CustomRPC.CryomaniacLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Cryomaniac && !role.IsRecruit)
                                ((Cryomaniac)role).Loses();
                        }

                        break;

                    case CustomRPC.ThiefLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Thief && !role.IsRecruit)
                                ((Thief)role).Loses();
                        }

                        break;

                    case CustomRPC.TrollWin:
                        var theTrollTheRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Troll && ((Troll)x).Killed && !x.IsRecruit);
                        ((Troll)theTrollTheRole).Wins();
                        break;

                    case CustomRPC.TrollLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Troll && !role.IsRecruit)
                                ((Troll)role).Loses();
                        }

                        break;

                    case CustomRPC.LoveWin:
                        var winnerlover = Utils.PlayerById(reader.ReadByte());
                        Objectifier.GetObjectifier<Lovers>(winnerlover).Wins();
                        break;

                    case CustomRPC.JesterLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Jester && !role.IsRecruit)
                                ((Jester)role).Loses();
                        }

                        break;

                    case CustomRPC.GlitchLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Glitch && !role.IsRecruit)
                                ((Glitch)role).Loses();
                        }

                        break;

                    case CustomRPC.JuggernautLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Juggernaut && !role.IsRecruit)
                                ((Juggernaut)role).Loses();
                        }

                        break;

                    case CustomRPC.MurdererLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Murderer && !role.IsRecruit)
                                ((Murderer)role).Loses();
                        }
                        
                        break;

                    case CustomRPC.AmnesiacLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Amnesiac && !role.IsRecruit)
                                ((Amnesiac)role).Loses();
                        }

                        break;

                    case CustomRPC.ExecutionerLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Executioner && !role.IsRecruit)
                                ((Executioner)role).Loses();
                        }

                        break;

                    case CustomRPC.NobodyWins:
                        Role.NobodyWinsFunc();
                        break;

                    case CustomRPC.NeutralsWin:
                        Role.NeutralsOnlyWin();
                        break;

                    case CustomRPC.SetCouple:
                        var lover1 = Utils.PlayerById(reader.ReadByte());
                        var lover2 = Utils.PlayerById(reader.ReadByte());
                        var objectifierLover1 = new Lovers(lover1);
                        var objectifierLover2 = new Lovers(lover2);
                        objectifierLover1.OtherLover = objectifierLover2.Player;
                        objectifierLover2.OtherLover = objectifierLover1.Player;
                        break;

                    case CustomRPC.SetDuo:
                        var rival1 = Utils.PlayerById(reader.ReadByte());
                        var rival2 = Utils.PlayerById(reader.ReadByte());
                        var objectifierRival1 = new Rivals(rival1);
                        var objectifierRival2 = new Rivals(rival2);
                        objectifierRival1.OtherRival = objectifierRival2.Player;
                        objectifierRival2.OtherRival = objectifierRival1.Player;
                        break;

                    case CustomRPC.Start:
                        Utils.ShowDeadBodies = false;

                        Role.NobodyWins = false;
                        Role.NeutralsWin = false;

                        Role.CrewWin = false;
                        Role.SyndicateWin = false;
                        Role.IntruderWin = false;
                        Role.AllNeutralsWin = false;
                                
                        Role.UndeadWin = false;
                        Role.CabalWin = false;

                        Role.NKWins = false;

                        Role.SyndicateHasChaosDrive = false;
                        Role.ChaosDriveMeetingTimerCount = 0;

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

                        GlobalModifiers.Clear();
                        BaitModifiers.Clear();
                        DiseasedModifiers.Clear();
                        ProfessionalModifiers.Clear();

                        GlobalAbilityGet.Clear();
                        IntruderAbilityGet.Clear();
                        CrewAbilityGet.Clear();
                        TunnelerAbilityGet.Clear();
                        NonEvilAbilityGet.Clear();
                        TaskedAbilityGet.Clear();
                        SnitchRevealerAbilityGet.Clear();
                        NonVentingAbilityGet.Clear();
                        EvilAbilityGet.Clear();
                        SyndicateAbilityGet.Clear();
                        NeutralAbilityGet.Clear();

                        CrewObjectifierGet.Clear();
                        NeutralObjectifierGet.Clear();
                        CorruptedObjectifierGet.Clear();
                        OverlordObjectifierGet.Clear();
                        GlobalObjectifierGet.Clear();

                        RecordRewind.points.Clear();
                        Murder.KilledPlayers.Clear();

                        Role.Buttons.Clear();
                        Role.SetColors();

                        Lists.DefinedLists();

                        PlayerLayers.Roles.CrewRoles.AltruistMod.KillButtonTarget.DontRevive = byte.MaxValue;
                        break;

                    case CustomRPC.JanitorClean:
                        readByte1 = reader.ReadByte();
                        var janitorPlayer = Utils.PlayerById(readByte1);
                        var janitorRole = Role.GetRole<Janitor>(janitorPlayer);
                        readByte = reader.ReadByte();
                        var deadBodies = Object.FindObjectsOfType<DeadBody>();

                        foreach (var body in deadBodies)
                        {
                            if (body.ParentId == readByte)
                                Coroutines.Start(Coroutine.CleanCoroutine(body, janitorRole));
                        }

                        break;

                    case CustomRPC.EngineerFix:
                        var engineer = Utils.PlayerById(reader.ReadByte());
                        Role.GetRole<Engineer>(engineer).UsedThisRound = true;
                        break;

                    case CustomRPC.FixLights:
                        var lights = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                        lights.ActualSwitches = lights.ExpectedSwitches;
                        break;

                    case CustomRPC.SetExtraVotes:
                        var mayor = Utils.PlayerById(reader.ReadByte());
                        var mayorRole = Role.GetRole<Mayor>(mayor);
                        mayorRole.ExtraVotes = reader.ReadBytesAndSize().ToList();

                        if (!mayor.Is(RoleEnum.Mayor))
                            mayorRole.VoteBank -= mayorRole.ExtraVotes.Count;

                        break;

                    case CustomRPC.SetSwaps:
                        readSByte = reader.ReadSByte();
                        SwapVotes.Swap1 = MeetingHud.Instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == readSByte);
                        readSByte2 = reader.ReadSByte();
                        SwapVotes.Swap2 = MeetingHud.Instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == readSByte2);
                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Bytes received - " + readSByte + " - " + readSByte2);
                        break;

                    case CustomRPC.Remember:
                        readByte1 = reader.ReadByte();
                        readByte2 = reader.ReadByte();
                        var amnesiac = Utils.PlayerById(readByte1);
                        var other = Utils.PlayerById(readByte2);
                        PerformKillButton.Remember(Role.GetRole<Amnesiac>(amnesiac), other);
                        break;

                    case CustomRPC.Declare:
                        readByte1 = reader.ReadByte();
                        readByte2 = reader.ReadByte();
                        var gf = Utils.PlayerById(readByte1);
                        var maf = Utils.PlayerById(readByte2);
                        PerformDeclareButton.Declare(Role.GetRole<Godfather>(gf), maf);
                        break;

                    case CustomRPC.Sidekick:
                        readByte1 = reader.ReadByte();
                        readByte2 = reader.ReadByte();
                        var reb = Utils.PlayerById(readByte1);
                        var side = Utils.PlayerById(readByte2);
                        PerformSidekickButton.Sidekick(Role.GetRole<Rebel>(reb), side);
                        break;

                    case CustomRPC.Shift:
                        readByte1 = reader.ReadByte();
                        readByte2 = reader.ReadByte();
                        var shifter = Utils.PlayerById(readByte1);
                        var other2 = Utils.PlayerById(readByte2);
                        PerformShiftButton.Shift(Role.GetRole<Shifter>(shifter), other2);
                        break;

                    case CustomRPC.Convert:
                        var drac = Utils.PlayerById(reader.ReadByte());
                        var other3 = Utils.PlayerById(reader.ReadByte());
                        PerformConvertButton.Convert(Role.GetRole<Dracula>(drac), other3);
                        break;
                    
                    case CustomRPC.SetTeleporter:
                        new Teleporter(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.Teleport:
                        var teleporter = Utils.PlayerById(reader.ReadByte());
                        var teleporterRole = Role.GetRole<Teleporter>(teleporter);
                        var teleportPos = reader.ReadVector2();
                        teleporterRole.TeleportPoint = teleportPos;
                        Teleporter.Teleport(teleporter);
                        break;

                    case CustomRPC.Rewind:
                        readByte = reader.ReadByte();
                        var TimeLordPlayer = Utils.PlayerById(readByte);
                        var TimeLordRole = Role.GetRole<TimeLord>(TimeLordPlayer);
                        StartStop.StartRewind(TimeLordRole);
                        break;

                    case CustomRPC.Protect:
                        readByte1 = reader.ReadByte();
                        readByte2 = reader.ReadByte();
                        var medic = Utils.PlayerById(readByte1);
                        var shield = Utils.PlayerById(readByte2);
                        Role.GetRole<Medic>(medic).ShieldedPlayer = shield;
                        Role.GetRole<Medic>(medic).UsedAbility = true;
                        break;

                    case CustomRPC.RewindRevive:
                        readByte = reader.ReadByte();
                        RecordRewind.ReviveBody(Utils.PlayerById(readByte));
                        break;

                    case CustomRPC.AttemptSound:
                        var medicId = reader.ReadByte();
                        readByte = reader.ReadByte();
                        StopKill.BreakShield(medicId, readByte, CustomGameOptions.ShieldBreaks);
                        break;

                    case CustomRPC.SetGlitch:
                        new Glitch(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetJuggernaut:
                        new Juggernaut(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetMurderer:
                        new Murderer(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.BypassKill:
                        var killer = Utils.PlayerById(reader.ReadByte());
                        var TargetPlayer = Utils.PlayerById(reader.ReadByte());
                        Utils.MurderPlayer(killer, TargetPlayer);
                        break;

                    case CustomRPC.AssassinKill:
                        var toDie = Utils.PlayerById(reader.ReadByte());
                        var guess = reader.ReadString();
                        var assassin = Ability.GetAbilityValue<Assassin>(AbilityEnum.Assassin);
                        AssassinKill.MurderPlayer(assassin, toDie, guess);
                        break;

                    case CustomRPC.Mimic:
                        var glitchPlayer = Utils.PlayerById(reader.ReadByte());
                        var mimicPlayer = Utils.PlayerById(reader.ReadByte());
                        var glitchRole = Role.GetRole<Glitch>(glitchPlayer);
                        glitchRole.TimeRemaining = CustomGameOptions.MimicDuration;
                        glitchRole.MimicTarget = mimicPlayer;
                        break;

                    case CustomRPC.GlitchWin:
                        var theGlitch = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Glitch && !x.IsRecruit && ((Glitch)x).GlitchWins);
                        ((Glitch)theGlitch).Wins();
                        break;

                    case CustomRPC.JuggernautWin:
                        var juggernaut = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Juggernaut && !x.IsRecruit && ((Juggernaut)x).JuggernautWins);
                        ((Juggernaut)juggernaut).Wins();
                        break;

                    case CustomRPC.MurdererWin:
                        var murd = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Murderer && !x.IsRecruit && ((Murderer)x).MurdWins);
                        ((Murderer)murd).Wins();
                        break;

                    case CustomRPC.CrewWin:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.Faction == Faction.Crew && !role.IsRecruit && !role.IsIntTraitor && !role.IsSynTraitor && !role.Player.Is(ObjectifierEnum.Corrupted))
                                role.Wins();
                        }
                            
                        break;

                    case CustomRPC.CrewLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.Faction == Faction.Crew && !role.IsRecruit && !role.IsIntTraitor && !role.IsSynTraitor && !role.Player.Is(ObjectifierEnum.Corrupted))
                                role.Loses();
                        }

                        break;

                    case CustomRPC.UndeadWin:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.SubFaction == SubFaction.Undead && !role.IsRecruit)
                                role.Wins();
                        }
                            
                        break;

                    case CustomRPC.UndeadLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.SubFaction == SubFaction.Undead && !role.IsRecruit)
                                role.Loses();
                        }

                        break;

                    case CustomRPC.IntruderWin:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.Faction == Faction.Intruder && !role.IsRecruit)
                                role.Wins();
                        }
                            
                        break;

                    case CustomRPC.IntruderLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.Faction == Faction.Intruder && !role.IsRecruit)
                                role.Loses();
                        }

                        break;

                    case CustomRPC.SyndicateWin:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.Faction == Faction.Syndicate && !role.IsRecruit)
                                role.Wins();
                        }
                            
                        break;

                    case CustomRPC.CabalWin:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Jackal || role.IsRecruit)
                                role.Wins();
                        }
                            
                        break;

                    case CustomRPC.CabalLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Jackal || role.IsRecruit)
                                role.Loses();
                        }
                            
                        break;

                    case CustomRPC.SyndicateLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.Faction == Faction.Syndicate && !role.IsRecruit)
                                role.Loses();
                        }

                        break;

                    case CustomRPC.Hack:
                        var hackPlayer = Utils.PlayerById(reader.ReadByte());
                        var glitchPlayer2 = Utils.PlayerById(reader.ReadByte());
                        var theGlitchRole = Role.GetRole<Glitch>(glitchPlayer2);
                        theGlitchRole.HackTarget = hackPlayer;
                        break;

                    case CustomRPC.Interrogate:
                        var sheriff = Utils.PlayerById(reader.ReadByte());
                        var otherPlayer = Utils.PlayerById(reader.ReadByte());
                        var sheriffRole = Role.GetRole<Sheriff>(sheriff);
                        sheriffRole.Interrogated.Add(otherPlayer.PlayerId);
                        sheriffRole.LastInterrogated = DateTime.UtcNow;
                        sheriffRole.UsedThisRound = true;
                        break;

                    case CustomRPC.InspExamine:
                        var inspector = Utils.PlayerById(reader.ReadByte());
                        var otherPlayer1 = Utils.PlayerById(reader.ReadByte());
                        Role.GetRole<Inspector>(inspector).Examined.Add(otherPlayer1);
                        Role.GetRole<Inspector>(inspector).LastExamined = DateTime.UtcNow;
                        break;

                    case CustomRPC.SetSheriff:
                        new Sheriff(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.Morph:
                        var morphling = Utils.PlayerById(reader.ReadByte());
                        var morphTarget = Utils.PlayerById(reader.ReadByte());
                        var morphRole = Role.GetRole<Morphling>(morphling);
                        morphRole.TimeRemaining = CustomGameOptions.MorphlingDuration;
                        morphRole.MorphedPlayer = morphTarget;
                        break;

                    case CustomRPC.Disguise:
                        var disguiser = Utils.PlayerById(reader.ReadByte());
                        var disguiseTarget = Utils.PlayerById(reader.ReadByte());
                        var disguiseRole = Role.GetRole<Disguiser>(disguiser);
                        disguiseRole.TimeRemaining = CustomGameOptions.DisguiseDuration;
                        disguiseRole.DisguisedPlayer = disguiseTarget;
                        disguiseRole.LastDisguised = DateTime.UtcNow;
                        break;

                    case CustomRPC.Poison:
                        var poisoner = Utils.PlayerById(reader.ReadByte());
                        var poisoned = Utils.PlayerById(reader.ReadByte());
                        var poisonerRole = Role.GetRole<Poisoner>(poisoner);
                        poisonerRole.PoisonedPlayer = poisoned;
                        break;

                    case CustomRPC.SetExecutioner:
                        new Executioner(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetEscort:
                        new Escort(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetConsort:
                        new Consort(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetTroll:
                        new Troll(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetGuardianAngel:
                        new GuardianAngel(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.TurnTraitor:
                        var traitor = Utils.PlayerById(reader.ReadByte());
                        var traitorObj = Objectifier.GetObjectifier<Traitor>(traitor);

                        if (traitorObj.Turned)
                            Turn.SetTraitor.TurnTraitor(traitor);

                        break;

                    case CustomRPC.SetGodfather:
                        new Godfather(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetTarget:
                        var exe = Utils.PlayerById(reader.ReadByte());
                        var exeTarget = Utils.PlayerById(reader.ReadByte());
                        var exeRole = Role.GetRole<Executioner>(exe);
                        exeRole.TargetPlayer = exeTarget;
                        break;

                    case CustomRPC.SetGATarget:
                        var ga = Utils.PlayerById(reader.ReadByte());
                        var gaTarget = Utils.PlayerById(reader.ReadByte());
                        var gaRole = Role.GetRole<GuardianAngel>(ga);
                        gaRole.TargetPlayer = gaTarget;
                        break;

                    case CustomRPC.SetGoodRecruit:
                        var jackal = Utils.PlayerById(reader.ReadByte());
                        var goodRecruit = Utils.PlayerById(reader.ReadByte());
                        var jackalRole = Role.GetRole<Jackal>(jackal);
                        jackalRole.GoodRecruit = goodRecruit;
                        (Role.GetRole(goodRecruit)).SubFaction = SubFaction.Cabal;
                        (Role.GetRole(goodRecruit)).IsRecruit = true;
                        break;

                    case CustomRPC.SetBackupRecruit:
                        var jackal3 = Utils.PlayerById(reader.ReadByte());
                        var backRecruit = Utils.PlayerById(reader.ReadByte());
                        var jackalRole3 = Role.GetRole<Jackal>(jackal3);
                        jackalRole3.BackupRecruit = backRecruit;
                        jackalRole3.HasRecruited = true;
                        (Role.GetRole(backRecruit)).SubFaction = SubFaction.Cabal;
                        (Role.GetRole(backRecruit)).IsRecruit = true;
                        break;

                    case CustomRPC.SetEvilRecruit:
                        var jackal2 = Utils.PlayerById(reader.ReadByte());
                        var evilRecruit = Utils.PlayerById(reader.ReadByte());
                        var jackalRole2 = Role.GetRole<Jackal>(jackal2);
                        jackalRole2.GoodRecruit = evilRecruit;
                        (Role.GetRole(evilRecruit)).SubFaction = SubFaction.Cabal;
                        (Role.GetRole(evilRecruit)).IsRecruit = true;
                        break;

                    case CustomRPC.SetBlackmailer:
                        new Blackmailer(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetVampireHunter:
                        new VampireHunter(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.Blackmail:
                        var blackmailer = Role.GetRole<Blackmailer>(Utils.PlayerById(reader.ReadByte()));
                        blackmailer.Blackmailed = Utils.PlayerById(reader.ReadByte());
                        break;

                    case CustomRPC.SetAgent:
                        new Agent(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetTaskmaster:
                        new Taskmaster(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.ExeToJest:
                        TargetColor.ExeToJes(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.GAToSurv:
                        GATargetColor.GAToSurv(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetSnitch:
                        new Snitch(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetTracker:
                        new Tracker(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetDetective:
                        new Detective(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetSurvivor:
                        new Survivor(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetMiner:
                        new Miner(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.Mine:
                        var ventId = reader.ReadInt32();
                        var miner = Utils.PlayerById(reader.ReadByte());
                        var minerRole = Role.GetRole<Miner>(miner);
                        var pos = reader.ReadVector2();
                        var zAxis = reader.ReadSingle();
                        PlayerLayers.Roles.IntruderRoles.MinerMod.PerformKill.SpawnVent(ventId, minerRole, pos, zAxis);
                        break;

                    case CustomRPC.TimeFreeze:
                        var tm = Utils.PlayerById(reader.ReadByte());
                        var tmRole = Role.GetRole<TimeMaster>(tm);
                        tmRole.TimeRemaining = CustomGameOptions.FreezeDuration;
                        tmRole.TimeFreeze();
                        break;

                    case CustomRPC.SetTimeMaster:
                        new TimeMaster(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetWraith:
                        new Wraith(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.Invis:
                        var wraith = Utils.PlayerById(reader.ReadByte());
                        var wraithRole = Role.GetRole<Wraith>(wraith);
                        wraithRole.TimeRemaining = CustomGameOptions.InvisDuration;
                        wraithRole.Invis();
                        break;

                    case CustomRPC.Alert:
                        var veteran = Utils.PlayerById(reader.ReadByte());
                        var veteranRole = Role.GetRole<Veteran>(veteran);
                        veteranRole.TimeRemaining = CustomGameOptions.AlertDuration;
                        veteranRole.Alert();
                        break;

                    case CustomRPC.Vest:
                        var surv = Utils.PlayerById(reader.ReadByte());
                        var survRole = Role.GetRole<Survivor>(surv);
                        survRole.TimeRemaining = CustomGameOptions.VestDuration;
                        survRole.Vest();
                        break;

                    case CustomRPC.GAProtect:
                        var ga2 = Utils.PlayerById(reader.ReadByte());
                        var ga2Role = Role.GetRole<GuardianAngel>(ga2);
                        ga2Role.TimeRemaining = CustomGameOptions.ProtectDuration;
                        ga2Role.Protect();
                        break;

                    case CustomRPC.Transport:
                        Coroutines.Start(Transporter.TransportPlayers(reader.ReadByte(), reader.ReadByte(), reader.ReadBoolean()));
                        break;

                    case CustomRPC.SetUntransportable:
                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
                            Role.GetRole<Transporter>(PlayerControl.LocalPlayer).UntransportablePlayers.Add(reader.ReadByte(), DateTime.UtcNow);

                        break;

                    case CustomRPC.Mediate:
                        var mediatedPlayer = Utils.PlayerById(reader.ReadByte());
                        var medium = Role.GetRole<Medium>(Utils.PlayerById(reader.ReadByte()));

                        if (PlayerControl.LocalPlayer.PlayerId != mediatedPlayer.PlayerId)
                            break;

                        medium.AddMediatePlayer(mediatedPlayer.PlayerId);
                        break;

                    case CustomRPC.SetGrenadier:
                        new Grenadier(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetDisguiser:
                        new Disguiser(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetWerewolf:
                        new Werewolf(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.FlashGrenade:
                        var grenadier = Utils.PlayerById(reader.ReadByte());
                        var grenadierRole = Role.GetRole<Grenadier>(grenadier);
                        grenadierRole.TimeRemaining = CustomGameOptions.GrenadeDuration;
                        grenadierRole.Flash();
                        break;

                    case CustomRPC.Maul:
                        var ww = Utils.PlayerById(reader.ReadByte());
                        var wwRole = Role.GetRole<Werewolf>(ww);
                        wwRole.Maul(wwRole.Player);
                        break;

                    case CustomRPC.SetTiebreaker:
                        new Tiebreaker(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetCoward:
                        new Coward(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetVolatile:
                        new Volatile(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetArsonist:
                        new Arsonist(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetPestilence:
                        new Pestilence(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetSerialKiller:
                        new SerialKiller(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.Douse:
                        var arsonist = Utils.PlayerById(reader.ReadByte());
                        var douseTarget = Utils.PlayerById(reader.ReadByte());
                        var arsonistRole = Role.GetRole<Arsonist>(arsonist);
                        arsonistRole.DousedPlayers.Add(douseTarget.PlayerId);
                        arsonistRole.LastDoused = DateTime.UtcNow;
                        arsonistRole.LastIgnited = DateTime.UtcNow;
                        break;

                    case CustomRPC.Ignite:
                        var theArsonist = Utils.PlayerById(reader.ReadByte());
                        var theArsonistRole = Role.GetRole<Arsonist>(theArsonist);
                        theArsonistRole.Ignite();
                        theArsonistRole.LastIgnited = DateTime.UtcNow;
                        break;

                    case CustomRPC.ArsonistWin:
                        var theArsonistTheRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Arsonist && !x.IsRecruit && ((Arsonist)x).ArsonistWins);
                        ((Arsonist)theArsonistTheRole).Wins();
                        break;

                    case CustomRPC.ArsonistLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Arsonist && !role.IsRecruit)
                                ((Arsonist)role).Loses();
                        }

                        break;

                    case CustomRPC.CorruptedLose:
                        foreach (var obj in Objectifier.AllObjectifiers)
                        {
                            if (obj.ObjectifierType == ObjectifierEnum.Corrupted)
                                ((Corrupted)obj).Loses();
                        }

                        break;

                    case CustomRPC.SerialKillerWin:
                        var theSerialTheRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.SerialKiller && !x.IsRecruit && ((SerialKiller)x).SerialKillerWins);
                        ((SerialKiller)theSerialTheRole).Wins();
                        break;

                    case CustomRPC.TaskmasterWin:
                        var theTMTheObj = Objectifier.AllObjectifiers.FirstOrDefault(x => x.ObjectifierType == ObjectifierEnum.Taskmaster && ((Taskmaster)x).TaskmasterWins);
                        ((Taskmaster)theTMTheObj).Wins();
                        break;

                    case CustomRPC.OverlordWin:
                        var theOvTheObj = Objectifier.AllObjectifiers.FirstOrDefault(x => x.ObjectifierType == ObjectifierEnum.Overlord && ((Overlord)x).OverlordWins);
                        ((Overlord)theOvTheObj).Wins();
                        break;

                    case CustomRPC.CorruptedWin:
                        var theCorrTheObj = Objectifier.AllObjectifiers.FirstOrDefault(x => x.ObjectifierType == ObjectifierEnum.Corrupted && ((Corrupted)x).CorruptedWin);
                        ((Corrupted)theCorrTheObj).Wins();
                        break;

                    case CustomRPC.SerialKillerLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.SerialKiller && !role.IsRecruit)
                                ((SerialKiller)role).Loses();
                        }

                        break;

                    case CustomRPC.SurvivorLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Survivor && role.Player.Data.IsDead || role.Player.Data.Disconnected)
                                ((Survivor)role).Loses();
                        }

                        break;

                    case CustomRPC.GALose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.GuardianAngel && ((GuardianAngel)role).TargetPlayer.Data.IsDead)
                                ((GuardianAngel)role).Loses();
                        }

                        break;

                    case CustomRPC.PlaguebearerWin:
                        var thePlaguebearerTheRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Plaguebearer && !x.IsRecruit && ((Plaguebearer)x).PlaguebearerWins);
                        ((Plaguebearer)thePlaguebearerTheRole).Wins();
                        break;

                    case CustomRPC.PlaguebearerLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Plaguebearer && !role.IsRecruit)
                                ((Plaguebearer)role).Loses();
                        }

                        break;

                    case CustomRPC.CannibalEat:
                        readByte1 = reader.ReadByte();
                        var cannibalPlayer = Utils.PlayerById(readByte1);
                        var cannibalRole = Role.GetRole<Cannibal>(cannibalPlayer);
                        readByte = reader.ReadByte();
                        var deads = Object.FindObjectsOfType<DeadBody>();

                        foreach (var body in deads)
                        {
                            if (body.ParentId == readByte)
                                Coroutines.Start(Eat.EatCoroutine(body, cannibalRole));
                        }

                        cannibalRole.LastEaten = DateTime.UtcNow;
                        break;

                    case CustomRPC.CannibalWin:
                        var theCannibalRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Cannibal && !x.IsRecruit && ((Cannibal)x).CannibalWin);
                        ((Cannibal)theCannibalRole).Wins();
                        break;

                    case CustomRPC.CannibalLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Cannibal && !role.IsRecruit)
                                ((Cannibal)role).Loses();
                        }

                        break;

                    case CustomRPC.Infect:
                        Role.GetRole<Plaguebearer>(Utils.PlayerById(reader.ReadByte())).InfectedPlayers.Add(reader.ReadByte());
                        break;

                    case CustomRPC.TurnPestilence:
                        Role.GetRole<Plaguebearer>(Utils.PlayerById(reader.ReadByte())).TurnPestilence();
                        break;

                    case CustomRPC.TurnVigilante:
                        Role.GetRole<VampireHunter>(Utils.PlayerById(reader.ReadByte())).TurnVigilante();
                        break;

                    case CustomRPC.TurnRebel:
                        Role.GetRole<Sidekick>(Utils.PlayerById(reader.ReadByte())).TurnRebel();
                        break;

                    /*case CustomRPC.TurnGodfather:
                        Role.GetRole<Mafioso>(Utils.PlayerById(reader.ReadByte())).TurnGodfather();
                        break;*/

                    case CustomRPC.PestilenceWin:
                        var thePestilenceTheRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Pestilence && !x.IsRecruit && ((Pestilence)x).PestilenceWins);
                        ((Pestilence)thePestilenceTheRole).Wins();
                        break;

                    case CustomRPC.PestilenceLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Pestilence)
                                ((Pestilence)role).Loses();
                        }

                        break;

                    case CustomRPC.SetImpostor:
                        new Impostor(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetCrewmate:
                        new Crewmate(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetAnarchist:
                        new Anarchist(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetWarper:
                        new Warper(Utils.PlayerById(reader.ReadByte()));
                        break;
                        
                    case CustomRPC.SetRadar:
                        new Radar(Utils.PlayerById(reader.ReadByte()));
                        break;
                        
                    case CustomRPC.SetInsepctor:
                        new Inspector(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SyncCustomSettings:
                        Rpc.ReceiveRpc(reader);
                        break;

                    case CustomRPC.SetAltruist:
                        new Altruist(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetGiant:
                        new Giant(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.AltruistRevive:
                        readByte1 = reader.ReadByte();
                        var altruistPlayer = Utils.PlayerById(readByte1);
                        var altruistRole = Role.GetRole<Altruist>(altruistPlayer);
                        readByte = reader.ReadByte();
                        var theDeadBodies = Object.FindObjectsOfType<DeadBody>();

                        foreach (var body in theDeadBodies)
                        {
                            if (body.ParentId == readByte)
                            {
                                if (body.ParentId == PlayerControl.LocalPlayer.PlayerId)
                                    Coroutines.Start(Utils.FlashCoroutine(altruistRole.Color));

                                Coroutines.Start(Revive.AltruistRevive(body, altruistRole));
                            }
                        }

                        break;

                    case CustomRPC.Warp:
                        byte teleports = reader.ReadByte();
                        Dictionary<byte, Vector2> coordinates = new Dictionary<byte, Vector2>();

                        for (int i = 0; i < teleports; i++)
                        {
                            byte playerId = reader.ReadByte();
                            Vector2 location = reader.ReadVector2();
                            coordinates.Add(playerId, location);
                        }

                        Warper.WarpPlayersToCoordinates(coordinates);
                        break;

                    case CustomRPC.FixAnimation:
                        var player = Utils.PlayerById(reader.ReadByte());
                        player.MyPhysics.ResetMoveState();
                        player.Collider.enabled = true;
                        player.moveable = true;
                        player.NetTransform.enabled = true;
                        break;

                    case CustomRPC.SetButtonBarry:
                        new ButtonBarry(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.BarryButton:
                        var buttonBarry = Utils.PlayerById(reader.ReadByte());

                        if (AmongUsClient.Instance.AmHost)
                        {
                            MeetingRoomManager.Instance.reporter = buttonBarry;
                            MeetingRoomManager.Instance.target = null;
                            AmongUsClient.Instance.DisconnectHandlers.AddUnique(MeetingRoomManager.Instance.Cast<IDisconnectHandler>());

                            if (ShipStatus.Instance.CheckTaskCompletion())
                                return;

                            DestroyableSingleton<HudManager>.Instance.OpenMeetingRoom(buttonBarry);
                            buttonBarry.RpcStartMeeting(null);
                        }

                        break;

                    case CustomRPC.BaitReport:
                        var baitKiller = Utils.PlayerById(reader.ReadByte());
                        var bait = Utils.PlayerById(reader.ReadByte());
                        baitKiller.ReportDeadBody(bait.Data);
                        break;

                    case CustomRPC.CheckMurder:
                        var murderKiller = Utils.PlayerById(reader.ReadByte());
                        var murderTarget = Utils.PlayerById(reader.ReadByte());
                        murderKiller.CheckMurder(murderTarget);
                        break;

                    case CustomRPC.SetUndertaker:
                        new Undertaker(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.Drag:
                        readByte1 = reader.ReadByte();
                        var dienerPlayer = Utils.PlayerById(readByte1);
                        var dienerRole = Role.GetRole<Undertaker>(dienerPlayer);
                        readByte = reader.ReadByte();
                        var dienerBodies = Object.FindObjectsOfType<DeadBody>();

                        foreach (var body in dienerBodies)
                        {
                            if (body.ParentId == readByte)
                                dienerRole.CurrentlyDragging = body;
                        }

                        break;

                    case CustomRPC.Drop:
                        readByte1 = reader.ReadByte();
                        var v2 = reader.ReadVector2();
                        var v2z = reader.ReadSingle();
                        var dienerPlayer2 = Utils.PlayerById(readByte1);
                        var dienerRole2 = Role.GetRole<Undertaker>(dienerPlayer2);
                        var body2 = dienerRole2.CurrentlyDragging;
                        dienerRole2.CurrentlyDragging = null;
                        body2.transform.position = new Vector3(v2.x, v2.y, v2z);
                        break;

                    case CustomRPC.SetAssassin:
                        new Assassin(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetVeteran:
                        new Veteran(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetTransporter:
                        new Transporter(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetMedium:
                        new Medium(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetCoroner:
                        new Coroner(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetUnderdog:
                        new Underdog(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.PhantomDied:
                        var phantom = Utils.PlayerById(reader.ReadByte());
                        Role.RoleDictionary.Remove(phantom.PlayerId);
                        var phantomRole = new Phantom(phantom);
                        phantom.gameObject.layer = LayerMask.NameToLayer("Players");
                        SetPhantom.RemoveTasks(phantom);
                        SetPhantom.AddCollider(phantomRole);

                        if (!PlayerControl.LocalPlayer.Is(AbilityEnum.Revealer))
                            PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();

                        System.Console.WriteLine("Become Phantom - Users");
                        break;

                    case CustomRPC.CatchPhantom:
                        var phantomPlayer = Utils.PlayerById(reader.ReadByte());
                        Objectifier.GetObjectifier<Phantom>(phantomPlayer).Caught = true;
                        break;

                    case CustomRPC.PhantomWin:
                        Objectifier.GetObjectifier<Phantom>(Utils.PlayerById(reader.ReadByte())).CompletedTasks = true;
                        break;

                    case CustomRPC.RevealerDied:
                        var haunter = Utils.PlayerById(reader.ReadByte());
                        Role.RoleDictionary.Remove(haunter.PlayerId);
                        var haunterRole = new Revealer(haunter);
                        haunterRole.RegenTask();
                        haunter.gameObject.layer = LayerMask.NameToLayer("Players");
                        SetRevealer.RemoveTasks(haunter);
                        SetRevealer.AddCollider(haunterRole);

                        if (!PlayerControl.LocalPlayer.Is(ObjectifierEnum.Phantom))
                            PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();

                        System.Console.WriteLine("Become Revealer - Users");
                        break;

                    case CustomRPC.CatchRevealer:
                        var haunterPlayer = Utils.PlayerById(reader.ReadByte());
                        Ability.GetAbility<Revealer>(haunterPlayer).Caught = true;
                        break;

                    case CustomRPC.RevealerFinished:
                        Reveal.HighlightImpostors.UpdateMeeting(MeetingHud.Instance);
                        break;

                    case CustomRPC.SetTraitor:
                        new Traitor(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetPlaguebearer:
                        new Plaguebearer(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetInsider:
                        new Insider(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetFanatic:
                        new Fanatic(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetCorrupted:
                        new Corrupted(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetOverlord:
                        new Overlord(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetOperative:
                        new Operative(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetRevealer:
                        new Revealer(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetPhantom:
                        new Phantom(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.AddMayorVoteBank:
                        Role.GetRole<Mayor>(Utils.PlayerById(reader.ReadByte())).VoteBank += reader.ReadInt32();
                        break;

                    case CustomRPC.RemoveAllBodies:
                        var buggedBodies = Object.FindObjectsOfType<DeadBody>();

                        foreach (var body in buggedBodies)
                            body.gameObject.Destroy();

                        break;

                    case CustomRPC.SubmergedFixOxygen:
                        SubmergedCompatibility.RepairOxygen();
                        break;

                    case CustomRPC.SetPos:
                        var setplayer = Utils.PlayerById(reader.ReadByte());
                        setplayer.transform.position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), setplayer.transform.position.z);
                        break;

                    case CustomRPC.SetSettings:
                        readByte = reader.ReadByte();
                        PlayerControl.GameOptions.MapId = readByte == byte.MaxValue ? (byte)0 : readByte;
                        PlayerControl.GameOptions.RoleOptions.SetRoleRate(RoleTypes.Scientist, 0, 0);
                        PlayerControl.GameOptions.RoleOptions.SetRoleRate(RoleTypes.Engineer, 0, 0);
                        PlayerControl.GameOptions.RoleOptions.SetRoleRate(RoleTypes.GuardianAngel, 0, 0);
                        PlayerControl.GameOptions.RoleOptions.SetRoleRate(RoleTypes.Shapeshifter, 0, 0);
                        PlayerControl.GameOptions.CrewLightMod = CustomGameOptions.CrewVision;
                        PlayerControl.GameOptions.ImpostorLightMod = CustomGameOptions.IntruderVision;
                        PlayerControl.GameOptions.AnonymousVotes = CustomGameOptions.AnonymousVoting;
                        PlayerControl.GameOptions.VisualTasks = CustomGameOptions.VisualTasks;
                        PlayerControl.GameOptions.PlayerSpeedMod = CustomGameOptions.PlayerSpeed;
                        PlayerControl.GameOptions.NumImpostors = CustomGameOptions.IntruderCount;
                        PlayerControl.GameOptions.GhostsDoTasks = CustomGameOptions.GhostTasksCountToWin;
                        PlayerControl.GameOptions.TaskBarMode = (TaskBarMode)CustomGameOptions.TaskBarMode;
                        PlayerControl.GameOptions.ConfirmImpostor = CustomGameOptions.ConfirmEjects;
                        PlayerControl.GameOptions.VotingTime = CustomGameOptions.VotingTime;
                        PlayerControl.GameOptions.DiscussionTime = CustomGameOptions.DiscussionTime;
                        PlayerControl.GameOptions.KillDistance = CustomGameOptions.InteractionDistance;
                        PlayerControl.GameOptions.EmergencyCooldown = CustomGameOptions.EmergencyButtonCooldown;
                        PlayerControl.GameOptions.NumEmergencyMeetings = CustomGameOptions.EmergencyButtonCount;
                        PlayerControl.GameOptions.KillCooldown = CustomGameOptions.IntKillCooldown;
                        PlayerControl.GameOptions.GhostsDoTasks = CustomGameOptions.GhostTasksCountToWin;
                        //PlayerControl.GameOptions.MaxPlayers = CustomGameOptions.LobbySize;

                        if (CustomGameOptions.AutoAdjustSettings)
                            RandomMap.AdjustSettings(readByte);

                        break;

                    case CustomRPC.Camouflage:
                        var camouflager = Utils.PlayerById(reader.ReadByte());
                        var camouflagerRole = Role.GetRole<Camouflager>(camouflager);
                        camouflagerRole.TimeRemaining = CustomGameOptions.CamouflagerDuration;
                        Utils.Camouflage();
                        break;

                    case CustomRPC.Conceal:
                        var concealer = Utils.PlayerById(reader.ReadByte());
                        var concealerRole = Role.GetRole<Concealer>(concealer);
                        concealerRole.TimeRemaining = CustomGameOptions.ConcealDuration;
                        Utils.Conceal();
                        break;

                    case CustomRPC.Shapeshift:
                        var ss = Utils.PlayerById(reader.ReadByte());
                        var ssRole = Role.GetRole<Concealer>(ss);
                        ssRole.TimeRemaining = CustomGameOptions.ConcealDuration;
                        Utils.Shapeshift();
                        break;

                    case CustomRPC.Gaze:
                        readByte = reader.ReadByte();
                        readByte1 = reader.ReadByte();
                        var gorgon = Role.GetRole<Gorgon>(Utils.PlayerById(readByte));
                        gorgon.gazeList.Add(readByte1, 0);

                        if (readByte1 == PlayerControl.LocalPlayer.PlayerId)
                        {
                            PlayerControl.LocalPlayer.moveable = false;
                            PlayerControl.LocalPlayer.NetTransform.Halt();
                            ImportantTextTask freezeText;
                            freezeText = new GameObject("_Player").AddComponent<ImportantTextTask>();
                            freezeText.transform.SetParent(PlayerControl.LocalPlayer.transform, false);
                            freezeText.Text = "<color=#00ACC2FF>STONED</color>";
                            freezeText.Index = PlayerControl.LocalPlayer.PlayerId;
                            PlayerControl.LocalPlayer.myTasks.Insert(0, freezeText);
                            ShipStatus.Instance.StartCoroutine(Effects.SwayX(Camera.main.transform, 0.75f, 0.25f));
                        }

                        Utils.AirKill(gorgon.Player, Utils.PlayerById(readByte1));
                        break;

                    case CustomRPC.SetGorgon:
                        new Gorgon(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetShapeshifter:
                        new Shapeshifter(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetVIP:
                        new VIP(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetShy:
                        new Shy(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetProfessional:
                        new Gorgon(Utils.PlayerById(reader.ReadByte()));
                        break;
                    
                    case CustomRPC.SendChat:
                        string report = reader.ReadString();
                        DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, report);
                        break;
                }
            }
        }

        [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SelectRoles))]
        public static class RpcSetRole
        {
            public static void Postfix()
            {
                PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("RPC SET ROLE");
                var infected = GameData.Instance.AllPlayers.ToArray().Where(o => o.IsImpostor());

                Utils.ShowDeadBodies = false;

                Role.NobodyWins = false;
                Role.NeutralsWin = false;

                Role.CrewWin = false;
                Role.SyndicateWin = false;
                Role.IntruderWin = false;
                Role.AllNeutralsWin = false;
                        
                Role.UndeadWin = false;
                Role.CabalWin = false;

                Role.NKWins = false;

                Role.SyndicateHasChaosDrive = false;
                Role.ChaosDriveMeetingTimerCount = 0;

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

                GlobalModifiers.Clear();
                BaitModifiers.Clear();
                DiseasedModifiers.Clear();
                ProfessionalModifiers.Clear();

                GlobalAbilityGet.Clear();
                IntruderAbilityGet.Clear();
                CrewAbilityGet.Clear();
                TunnelerAbilityGet.Clear();
                NonEvilAbilityGet.Clear();
                TaskedAbilityGet.Clear();
                SnitchRevealerAbilityGet.Clear();
                NonVentingAbilityGet.Clear();
                EvilAbilityGet.Clear();
                SyndicateAbilityGet.Clear();
                NeutralAbilityGet.Clear();

                CrewObjectifierGet.Clear();
                NeutralObjectifierGet.Clear();
                CorruptedObjectifierGet.Clear();
                OverlordObjectifierGet.Clear();
                GlobalObjectifierGet.Clear();

                RecordRewind.points.Clear();
                Murder.KilledPlayers.Clear();

                Role.Buttons.Clear();
                Role.SetColors();

                Lists.DefinedLists();

                PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Cleared Lists");

                PlayerLayers.Roles.CrewRoles.AltruistMod.KillButtonTarget.DontRevive = byte.MaxValue;

                PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Cleared Functions");

                unchecked
                {
                    var startWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Start, SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(startWriter);
                }

                PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Unchecked Write Line");
                
                int num = 0;

                if (!IsKilling)
                {
                    #region Crew Roles
                    if (CustomGameOptions.MayorOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.MayorCount : 1;

                        while (num > 0)
                        {
                            CrewSovereignRoles.Add((typeof(Mayor), CustomRPC.SetMayor, CustomGameOptions.MayorOn, CustomGameOptions.UniqueMayor));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Mayor Done");
                    }

                    if (CustomGameOptions.SheriffOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.SheriffCount : 1;

                        while (num > 0)
                        {
                            CrewInvestigativeRoles.Add((typeof(Sheriff), CustomRPC.SetSheriff, CustomGameOptions.SheriffOn, CustomGameOptions.UniqueSheriff));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Sheriff Done");
                    }

                    if (CustomGameOptions.InspectorOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.InspectorCount : 1;

                        while (num > 0)
                        {
                            CrewInvestigativeRoles.Add((typeof(Inspector), CustomRPC.SetInsepctor, CustomGameOptions.InspectorOn, CustomGameOptions.UniqueInspector));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Inspector Done");
                    }

                    if (CustomGameOptions.VigilanteOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.VigilanteCount : 1;

                        while (num > 0)
                        {
                            CrewKillingRoles.Add((typeof(Vigilante), CustomRPC.SetVigilante, CustomGameOptions.VigilanteOn, CustomGameOptions.UniqueVigilante));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Vigilante Done");
                    }

                    if (CustomGameOptions.EngineerOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.EngineerCount : 1;

                        while (num > 0)
                        {
                            CrewSupportRoles.Add((typeof(Engineer), CustomRPC.SetEngineer, CustomGameOptions.EngineerOn, CustomGameOptions.UniqueEngineer));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Engineer Done");
                    }

                    if (CustomGameOptions.SwapperOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.SwapperCount : 1;

                        while (num > 0)
                        {
                            CrewSovereignRoles.Add((typeof(Swapper), CustomRPC.SetSwapper, CustomGameOptions.SwapperOn, CustomGameOptions.UniqueSwapper));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Swapper Done");
                    }

                    if (CustomGameOptions.InvestigatorOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.InvestigatorCount : 1;

                        while (num > 0)
                        {
                            CrewInvestigativeRoles.Add((typeof(Investigator), CustomRPC.SetInvestigator, CustomGameOptions.InvestigatorOn, CustomGameOptions.UniqueInvestigator));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Investigator Done");
                    }

                    if (CustomGameOptions.TimeLordOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.TimeLordCount : 1;

                        while (num > 0)
                        {
                            CrewProtectiveRoles.Add((typeof(TimeLord), CustomRPC.SetTimeLord, CustomGameOptions.TimeLordOn, CustomGameOptions.UniqueTimeLord));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Time Lord Done");
                    }

                    if (CustomGameOptions.MedicOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.MedicCount : 1;

                        while (num > 0)
                        {
                            CrewProtectiveRoles.Add((typeof(Medic), CustomRPC.SetMedic, CustomGameOptions.MedicOn, CustomGameOptions.UniqueMedic));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Medic Done");
                    }

                    if (CustomGameOptions.AgentOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.AgentCount : 1;

                        while (num > 0)
                        {
                            CrewInvestigativeRoles.Add((typeof(Agent), CustomRPC.SetAgent, CustomGameOptions.AgentOn, CustomGameOptions.UniqueAgent));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Agent Done");
                    }

                    if (CustomGameOptions.AltruistOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.AltruistCount : 1;

                        while (num > 0)
                        {
                            CrewProtectiveRoles.Add((typeof(Altruist), CustomRPC.SetAltruist, CustomGameOptions.AltruistOn, CustomGameOptions.UniqueAltruist));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Altruist Done");
                    }

                    if (CustomGameOptions.VeteranOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.VeteranCount : 1;

                        while (num > 0)
                        {
                            CrewKillingRoles.Add((typeof(Veteran), CustomRPC.SetVeteran, CustomGameOptions.VeteranOn, CustomGameOptions.UniqueVeteran));
                            num--;
                        } 

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Veteran Done");
                    }

                    if (CustomGameOptions.TrackerOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.TrackerCount : 1;

                        while (num > 0) 
                        {
                            CrewInvestigativeRoles.Add((typeof(Tracker), CustomRPC.SetTracker, CustomGameOptions.TrackerOn, CustomGameOptions.UniqueTracker));
                            num--;
                        } 

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Tracker Done");
                    }

                    if (CustomGameOptions.TransporterOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.TransporterCount : 1;

                        while (num > 0)
                        {
                            CrewSupportRoles.Add((typeof(Transporter), CustomRPC.SetTransporter, CustomGameOptions.TransporterOn, CustomGameOptions.UniqueTransporter));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Transporter Done");
                    }

                    if (CustomGameOptions.MediumOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.MediumCount : 1;

                        while (num > 0)
                        {
                            CrewInvestigativeRoles.Add((typeof(Medium), CustomRPC.SetMedium, CustomGameOptions.MediumOn, CustomGameOptions.UniqueMedium));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Medium Done");
                    }

                    if (CustomGameOptions.CoronerOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.CoronerCount : 1;

                        while (num > 0)
                        {
                            CrewInvestigativeRoles.Add((typeof(Coroner), CustomRPC.SetCoroner, CustomGameOptions.CoronerOn, CustomGameOptions.UniqueCoroner));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Coroner Done");
                    }

                    if (CustomGameOptions.OperativeOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.OperativeCount : 1;

                        while (num > 0)
                        {
                            CrewInvestigativeRoles.Add((typeof(Operative), CustomRPC.SetOperative, CustomGameOptions.OperativeOn, CustomGameOptions.UniqueOperative));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Operative Done");
                    }

                    if (CustomGameOptions.DetectiveOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.DetectiveCount : 1;

                        while (num > 0)
                        {
                            CrewInvestigativeRoles.Add((typeof(Detective), CustomRPC.SetDetective, CustomGameOptions.DetectiveOn, CustomGameOptions.UniqueDetective));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Detective Done");
                    }

                    if (CustomGameOptions.EscortOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.EscortCount : 1;

                        while (num > 0)
                        {
                            CrewSupportRoles.Add((typeof(Escort), CustomRPC.SetEscort, CustomGameOptions.EscortOn, CustomGameOptions.UniqueEscort));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Escort Done");
                    }

                    if (CustomGameOptions.ShifterOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.ShifterCount : 1;

                        while (num > 0)
                        {
                            CrewSupportRoles.Add((typeof(Shifter), CustomRPC.SetShifter, CustomGameOptions.ShifterOn, CustomGameOptions.UniqueShifter));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Shifter Done");
                    }

                    if (CustomGameOptions.CrewmateOn > 0 && IsCustom)
                    {
                        num = CustomGameOptions.CrewCount;

                        while (num > 0)
                        {
                            CrewRoles.Add((typeof(Crewmate), CustomRPC.SetCrewmate, CustomGameOptions.CrewmateOn, false));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Crewmate Done");
                    }

                    //It is crucial VH comes with Dracula does, since I want VH to only spawn if there's a Dracula in the role spawning set
                    //Future AD here, I realised that when the game begins, the VH will automatically turn into Vig so I don't need to take any extra steps
                    //Future future AD here, VH does in fact spawn with no Dracula and refuses to convert to Vig on game start for no good reason, I'll fix it once I've gotten plenty mad over it
                    if (CustomGameOptions.VampireHunterOn > 0 && CustomGameOptions.DraculaOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.VampireHunterCount : 1;

                        while (num > 0)
                        {
                            CrewAuditorRoles.Add((typeof(VampireHunter), CustomRPC.SetVampireHunter, CustomGameOptions.VampireHunterOn, CustomGameOptions.UniqueVampireHunter));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Vampire Hunter Done");
                    }
                    #endregion

                    #region Neutral Roles
                    if (CustomGameOptions.JesterOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.JesterCount : 1;

                        while (num > 0)
                        {
                            NeutralEvilRoles.Add((typeof(Jester), CustomRPC.SetJester, CustomGameOptions.JesterOn, CustomGameOptions.UniqueJester));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Jester Done");
                    }

                    if (CustomGameOptions.AmnesiacOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.AmnesiacCount : 1;

                        while (num > 0)
                        {
                            NeutralBenignRoles.Add((typeof(Amnesiac), CustomRPC.SetAmnesiac, CustomGameOptions.AmnesiacOn, CustomGameOptions.UniqueAmnesiac));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Amnesiac Done");
                    }

                    if (CustomGameOptions.ExecutionerOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.ExecutionerCount : 1;

                        while (num > 0)
                        {
                            NeutralEvilRoles.Add((typeof(Executioner), CustomRPC.SetExecutioner, CustomGameOptions.ExecutionerOn, CustomGameOptions.UniqueExecutioner));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Executioner Done");
                    }

                    if (CustomGameOptions.SurvivorOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.SurvivorCount : 1;

                        while (num > 0)
                        {
                            NeutralBenignRoles.Add((typeof(Survivor), CustomRPC.SetSurvivor, CustomGameOptions.SurvivorOn, CustomGameOptions.UniqueSurvivor));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Survivor Done");
                    }

                    if (CustomGameOptions.GuardianAngelOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.GuardianAngelCount : 1;

                        while (num > 0)
                        {
                            NeutralBenignRoles.Add((typeof(GuardianAngel), CustomRPC.SetGuardianAngel, CustomGameOptions.GuardianAngelOn, CustomGameOptions.UniqueGuardianAngel));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Guardian Angel Done");
                    }

                    if (CustomGameOptions.GlitchOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.GlitchCount : 1;

                        while (num > 0)
                        {
                            NeutralKillingRoles.Add((typeof(Glitch), CustomRPC.SetGlitch, CustomGameOptions.GlitchOn, CustomGameOptions.UniqueGlitch));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Glitch Done");
                    }

                    if (CustomGameOptions.MurdererOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.MurdCount : 1;

                        while (num > 0)
                        {
                            NeutralKillingRoles.Add((typeof(Murderer), CustomRPC.SetMurderer, CustomGameOptions.MurdererOn, CustomGameOptions.UniqueMurderer));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Murderer Done");
                    }

                    if (CustomGameOptions.CryomaniacOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.CryomaniacCount : 1;

                        while (num > 0)
                        {
                            NeutralKillingRoles.Add((typeof(Cryomaniac), CustomRPC.SetCryomaniac, CustomGameOptions.CryomaniacOn, CustomGameOptions.UniqueCryomaniac));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Cryomaniac Done");
                    }

                    if (CustomGameOptions.WerewolfOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.WerewolfCount : 1;

                        while (num > 0)
                        {
                            NeutralKillingRoles.Add((typeof(Werewolf), CustomRPC.SetWerewolf, CustomGameOptions.WerewolfOn, CustomGameOptions.UniqueWerewolf));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Werewolf Done");
                    }

                    if (CustomGameOptions.ArsonistOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.ArsonistCount : 1;

                        while (num > 0)
                        {
                            NeutralKillingRoles.Add((typeof(Arsonist), CustomRPC.SetArsonist, CustomGameOptions.ArsonistOn, CustomGameOptions.UniqueArsonist));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Arsonist Done");
                    }

                    if (CustomGameOptions.JackalOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.JackalCount : 1;

                        while (num > 0)
                        {
                            NeutralNeophyteRoles.Add((typeof(Jackal), CustomRPC.SetJackal, CustomGameOptions.JackalOn, CustomGameOptions.UniqueJackal));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Jackal Done");
                    }

                    if (CustomGameOptions.PlaguebearerOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.PlaguebearerCount : 1;

                        while (num > 0)
                        {
                            if (CustomGameOptions.PestSpawn)
                                NeutralKillingRoles.Add((typeof(Pestilence), CustomRPC.SetPestilence, CustomGameOptions.PlaguebearerOn, CustomGameOptions.UniquePlaguebearer));
                            else
                                NeutralKillingRoles.Add((typeof(Plaguebearer), CustomRPC.SetPlaguebearer, CustomGameOptions.PlaguebearerOn, CustomGameOptions.UniquePlaguebearer));

                            num--;
                        }

                        var PBorPest = CustomGameOptions.PestSpawn ? "Pestilence" : "Plaguebearer";

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage($"{PBorPest} Done");
                    }

                    if (CustomGameOptions.SerialKillerOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.SKCount : 1;

                        while (num > 0)
                        {
                            NeutralKillingRoles.Add((typeof(SerialKiller), CustomRPC.SetSerialKiller, CustomGameOptions.SerialKillerOn, CustomGameOptions.UniqueSerialKiller));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Serial Killer Done");
                    }

                    if (CustomGameOptions.JuggernautOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.JuggernautCount : 1;

                        while (num > 0)
                        {
                            NeutralKillingRoles.Add((typeof(Juggernaut), CustomRPC.SetJuggernaut, CustomGameOptions.JuggernautOn, CustomGameOptions.UniqueJuggernaut));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Juggeraut Done");
                    }

                    if (CustomGameOptions.CannibalOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.CannibalCount : 1;

                        while (num > 0)
                        {
                            NeutralEvilRoles.Add((typeof(Cannibal), CustomRPC.SetCannibal, CustomGameOptions.CannibalOn, CustomGameOptions.UniqueCannibal));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Cannibal Done");
                    }

                    if (CustomGameOptions.ThiefOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.ThiefCount : 1;

                        while (num > 0)
                        {
                            NeutralBenignRoles.Add((typeof(Thief), CustomRPC.SetThief, CustomGameOptions.ThiefOn, CustomGameOptions.UniqueThief));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Thief Done");
                    }

                    if (CustomGameOptions.DraculaOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.DraculaCount : 1;

                        while (num > 0)
                        {
                            NeutralNeophyteRoles.Add((typeof(Dracula), CustomRPC.SetDracula, CustomGameOptions.DraculaOn, CustomGameOptions.UniqueDracula));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Dracula Done");
                    }

                    if (CustomGameOptions.TrollOn > 0 && CustomGameOptions.GameMode != GameMode.Classic)
                    {
                        num = IsCustom ? CustomGameOptions.TrollCount : 1;

                        while (num > 0)
                        {
                            NeutralEvilRoles.Add((typeof(Troll), CustomRPC.SetTroll, CustomGameOptions.TrollOn, CustomGameOptions.UniqueTroll));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Troll Done");
                    }
                    #endregion

                    #region Intruder Roles
                    if (!CustomGameOptions.AltImps)
                    {
                        if (CustomGameOptions.UndertakerOn > 0)
                        {
                            num = IsCustom ? CustomGameOptions.UndertakerCount : 1;

                            while (num > 0)
                            {
                                IntruderConcealingRoles.Add((typeof(Undertaker), CustomRPC.SetUndertaker, CustomGameOptions.UndertakerOn, CustomGameOptions.UniqueUndertaker));
                                num--;
                            }

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Undertaker Done");
                        }

                        if (CustomGameOptions.MorphlingOn > 0)
                        {
                            num = IsCustom ? CustomGameOptions.MorphlingCount : 1;

                            while (num > 0)
                            {
                                IntruderDeceptionRoles.Add((typeof(Morphling), CustomRPC.SetMorphling, CustomGameOptions.MorphlingOn, CustomGameOptions.UniqueMorphling));
                                num--;
                            }

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Morphling Done");
                        }

                        if (CustomGameOptions.BlackmailerOn > 0)
                        {
                            num = IsCustom ? CustomGameOptions.BlackmailerCount : 1;

                            while (num > 0)
                            {
                                IntruderConcealingRoles.Add((typeof(Blackmailer), CustomRPC.SetBlackmailer, CustomGameOptions.BlackmailerOn, CustomGameOptions.UniqueBlackmailer));
                                num--;
                            }

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Blackmailer Done");
                        }

                        if (CustomGameOptions.MinerOn > 0)
                        {
                            num = IsCustom ? CustomGameOptions.MinerCount : 1;

                            while (num > 0)
                            {
                                IntruderSupportRoles.Add((typeof(Miner), CustomRPC.SetMiner, CustomGameOptions.MinerOn, CustomGameOptions.UniqueMiner));
                                num--;
                            }

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Miner Done");
                        }

                        if (CustomGameOptions.TeleporterOn > 0)
                        {
                            num = IsCustom ? CustomGameOptions.TeleporterCount : 1;

                            while (num > 0)
                            {
                                IntruderSupportRoles.Add((typeof(Teleporter), CustomRPC.SetTeleporter, CustomGameOptions.TeleporterOn, CustomGameOptions.UniqueTeleporter));
                                num--;
                            }

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Teleporter Done");
                        }

                        if (CustomGameOptions.WraithOn > 0)
                        {
                            num = IsCustom ? CustomGameOptions.WraithCount : 1;

                            while (num > 0)
                            {
                                IntruderDeceptionRoles.Add((typeof(Wraith), CustomRPC.SetWraith, CustomGameOptions.WraithOn, CustomGameOptions.UniqueWraith));
                                num--;
                            }

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Wraith Done");
                        }

                        if (CustomGameOptions.ConsortOn > 0)
                        {
                            num = IsCustom ? CustomGameOptions.ConsortCount : 1;

                            while (num > 0)
                            {
                                IntruderSupportRoles.Add((typeof(Consort), CustomRPC.SetConsort, CustomGameOptions.ConsortOn, CustomGameOptions.UniqueConsort));
                                num--;
                            }

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Consort Done");
                        }

                        if (CustomGameOptions.JanitorOn > 0)
                        {
                            num = IsCustom ? CustomGameOptions.JanitorCount : 1;

                            while (num > 0)
                            {
                                IntruderConcealingRoles.Add((typeof(Janitor), CustomRPC.SetJanitor, CustomGameOptions.JanitorOn, CustomGameOptions.UniqueJanitor));
                                num--;
                            }

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Janitor Done");
                        }

                        if (CustomGameOptions.CamouflagerOn > 0)
                        {
                            num = IsCustom ? CustomGameOptions.CamouflagerCount : 1;

                            while (num > 0)
                            {
                                IntruderConcealingRoles.Add((typeof(Camouflager), CustomRPC.SetCamouflager, CustomGameOptions.CamouflagerOn, CustomGameOptions.UniqueCamouflager));
                                num--;
                            }

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Camouflager Done");
                        }

                        if (CustomGameOptions.GrenadierOn > 0)
                        {
                            num = IsCustom ? CustomGameOptions.GrenadierCount : 1;

                            while (num > 0)
                            {
                                IntruderConcealingRoles.Add((typeof(Grenadier), CustomRPC.SetGrenadier, CustomGameOptions.GrenadierOn, CustomGameOptions.UniqueGrenadier));
                                num--;
                            }

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Grenadier Done");
                        }

                        if (CustomGameOptions.PoisonerOn > 0)
                        {
                            num = IsCustom ? CustomGameOptions.PoisonerCount : 1;

                            while (num > 0)
                            {
                                IntruderDeceptionRoles.Add((typeof(Poisoner), CustomRPC.SetPoisoner, CustomGameOptions.PoisonerOn, CustomGameOptions.UniquePoisoner));
                                num--;
                            }

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Poisoner Done");
                        }

                        if (CustomGameOptions.ImpostorOn > 0 && IsCustom)
                        {
                            num = CustomGameOptions.ImpCount;

                            while (num > 0)
                            {
                                IntruderRoles.Add((typeof(Impostor), CustomRPC.SetImpostor, CustomGameOptions.ImpostorOn, false));
                                num--;
                            }

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Impostor Done");
                        }

                        if (CustomGameOptions.ConsigliereOn > 0)
                        {
                            num = IsCustom ? CustomGameOptions.ConsigliereCount : 1;

                            while (num > 0)
                            {
                                IntruderSupportRoles.Add((typeof(Consigliere), CustomRPC.SetConsigliere, CustomGameOptions.ConsigliereOn, CustomGameOptions.UniqueConsigliere));
                                num--;
                            }

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Consigliere Done");
                        }

                        if (CustomGameOptions.DisguiserOn > 0)
                        {
                            num = IsCustom ? CustomGameOptions.DisguiserCount : 1;

                            while (num > 0)
                            {
                                IntruderDeceptionRoles.Add((typeof(Disguiser), CustomRPC.SetDisguiser, CustomGameOptions.DisguiserOn, CustomGameOptions.UniqueDisguiser));
                                num--;
                            }

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Disguiser Done");
                        }

                        if (CustomGameOptions.TimeMasterOn > 0)
                        {
                            num = IsCustom ? CustomGameOptions.TimeMasterCount : 1;

                            while (num > 0)
                            {
                                IntruderSupportRoles.Add((typeof(TimeMaster), CustomRPC.SetTimeMaster, CustomGameOptions.TimeMasterOn, CustomGameOptions.UniqueTimeMaster));
                                num--;
                            }

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Time Master Done");
                        }

                        if (CustomGameOptions.GodfatherOn > 0 && CustomGameOptions.IntruderCount >= 3)
                        {
                            num = IsCustom ? CustomGameOptions.GodfatherCount : 1;
                                
                            while (num > 0)
                            {
                                IntruderSupportRoles.Add((typeof(Godfather), CustomRPC.SetGodfather, CustomGameOptions.GodfatherOn, CustomGameOptions.UniqueGodfather));
                                num--;
                            }

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Godfather Done");
                        }
                    }
                    #endregion

                    #region Syndicate Roles
                    if (CustomGameOptions.AnarchistOn > 0 && IsCustom)
                    {
                        num = CustomGameOptions.AnarchistCount;

                        while (num > 0)
                        {
                            SyndicateRoles.Add((typeof(Anarchist), CustomRPC.SetAnarchist, CustomGameOptions.AnarchistOn, false));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Anarchist Done");
                    }

                    if (CustomGameOptions.ShapeshifterOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.ShapeshifterCount : 1;

                        while (num > 0)
                        {
                            SyndicateDisruptionRoles.Add((typeof(Shapeshifter), CustomRPC.SetShapeshifter, CustomGameOptions.ShapeshifterOn, CustomGameOptions.UniqueShapeshifter));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Shapeshifter Done");
                    }

                    if (CustomGameOptions.GorgonOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.GorgonCount : 1;

                        while (num > 0)
                        {
                            SyndicateKillingRoles.Add((typeof(Gorgon), CustomRPC.SetGorgon, CustomGameOptions.GorgonOn, CustomGameOptions.UniqueGorgon));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Gorgon Done");
                    }

                    if (CustomGameOptions.FramerOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.FramerCount : 1;

                        while (num > 0)
                        {
                            SyndicateDisruptionRoles.Add((typeof(Framer), CustomRPC.SetFramer, CustomGameOptions.FramerOn, CustomGameOptions.UniqueFramer));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Framer Done");
                    }

                    if (CustomGameOptions.RebelOn > 0 && CustomGameOptions.SyndicateCount >= 3)
                    {
                        num = IsCustom ? CustomGameOptions.RebelCount : 1;
                                
                        while (num > 0)
                        {
                            SyndicateSupportRoles.Add((typeof(Rebel), CustomRPC.SetRebel, CustomGameOptions.RebelOn, CustomGameOptions.UniqueRebel));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Rebel Done");
                    }

                    if (CustomGameOptions.ConcealerOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.ConcealerCount : 1;

                        while (num > 0)
                        {
                            SyndicateSupportRoles.Add((typeof(Concealer), CustomRPC.SetConcealer, CustomGameOptions.ConcealerOn, CustomGameOptions.UniqueConcealer));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Concealer Done");
                    }

                    if (CustomGameOptions.WarperOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.WarperCount : 1;

                        while (num > 0)
                        {
                            SyndicateSupportRoles.Add((typeof(Warper), CustomRPC.SetWarper, CustomGameOptions.WarperOn, CustomGameOptions.UniqueWarper));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Warper Done");
                    }
                    #endregion

                    #region Modifiers
                    if (CustomGameOptions.DiseasedOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.DiseasedCount : 1;
                        
                        while (num > 0)
                        {
                            DiseasedModifiers.Add((typeof(Diseased), CustomRPC.SetDiseased, CustomGameOptions.DiseasedOn));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Diseased Done");
                    }

                    if (CustomGameOptions.BaitOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.BaitCount : 1;
                        
                        while (num > 0)
                        {
                            BaitModifiers.Add((typeof(Bait), CustomRPC.SetBait, CustomGameOptions.BaitOn));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Bait Done");
                    }
                    
                    if (CustomGameOptions.DwarfOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.DwarfCount : 1;
                        
                        while (num > 0)
                        {
                            GlobalModifiers.Add((typeof(Dwarf), CustomRPC.SetDwarf, CustomGameOptions.DwarfOn));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Dwarf Done");
                    }
                    
                    if (CustomGameOptions.VIPOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.VIPCount : 1;
                        
                        while (num > 0)
                        {
                            GlobalModifiers.Add((typeof(VIP), CustomRPC.SetVIP, CustomGameOptions.VIPOn));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("VIP Done");
                    }
                    
                    if (CustomGameOptions.ShyOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.ShyCount : 1;
                        
                        while (num > 0)
                        {
                            GlobalModifiers.Add((typeof(Shy), CustomRPC.SetShy, CustomGameOptions.ShyOn));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Shy Done");
                    }

                    if (CustomGameOptions.GiantOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.GiantCount : 1;
                        
                        while (num > 0)
                        {
                            GlobalModifiers.Add((typeof(Giant), CustomRPC.SetGiant, CustomGameOptions.GiantOn));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Giant Done");
                    }
                    
                    if (CustomGameOptions.DrunkOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.DrunkCount : 1;
                        
                        while (num > 0)
                        {
                            GlobalModifiers.Add((typeof(Drunk), CustomRPC.SetDrunk, CustomGameOptions.DrunkOn));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Drunk Done");
                    }

                    if (CustomGameOptions.FlincherOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.FlincherCount : 1;
                        
                        while (num > 0)
                        {
                            GlobalModifiers.Add((typeof(Flincher), CustomRPC.SetFlincher, CustomGameOptions.FlincherOn));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Flincher Done");
                    }

                    if (CustomGameOptions.CowardOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.CowardCount : 1;
                        
                        while (num > 0)
                        {
                            GlobalModifiers.Add((typeof(Coward), CustomRPC.SetCoward, CustomGameOptions.CowardOn));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Coward Done");
                    }

                    if (CustomGameOptions.VolatileOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.VolatileCount : 1;
                        
                        while (num > 0)
                        {
                            GlobalModifiers.Add((typeof(Volatile), CustomRPC.SetVolatile, CustomGameOptions.VolatileOn));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Volatile Done");
                    }

                    if (CustomGameOptions.ProfessionalOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.ProfessionalCount : 1;
                        
                        while (num > 0)
                        {
                            ProfessionalModifiers.Add((typeof(Professional), CustomRPC.SetProfessional, CustomGameOptions.ProfessionalOn));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Professional Done");
                    }
                    #endregion

                    #region Abilities
                    if (CustomGameOptions.AssassinOn > 0)
                    {
                       num = CustomGameOptions.NumberOfSyndicateAssassins;

                        while (num > 0)
                        {
                            SyndicateAbilityGet.Add((typeof(Assassin), CustomRPC.SetAssassin, CustomGameOptions.AssassinOn));
                            num--;
                        }

                        num = CustomGameOptions.NumberOfNeutralAssassins;

                        while (num > 0)
                        {
                            NeutralAbilityGet.Add((typeof(Assassin), CustomRPC.SetAssassin, CustomGameOptions.AssassinOn));
                            num--;
                        }

                        num = CustomGameOptions.NumberOfCrewAssassins;

                        while (num > 0)
                        {
                            CrewAbilityGet.Add((typeof(Assassin), CustomRPC.SetAssassin, CustomGameOptions.AssassinOn));
                            num--;
                        }

                        num = CustomGameOptions.NumberOfImpostorAssassins;

                        while (num > 0)
                        {
                            IntruderAbilityGet.Add((typeof(Assassin), CustomRPC.SetAssassin, CustomGameOptions.AssassinOn));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Assassin Done");
                    }

                    if (CustomGameOptions.RevealerOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.RevealerCount : 1;
                        
                        while (num > 0)
                        {
                            SnitchRevealerAbilityGet.Add((typeof(Revealer), CustomRPC.SetRevealer, CustomGameOptions.RevealerOn));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Revealer Done");
                    }

                    if (CustomGameOptions.SnitchOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.SnitchCount : 1;
                        
                        while (num > 0)
                        {
                            SnitchRevealerAbilityGet.Add((typeof(Snitch), CustomRPC.SetSnitch, CustomGameOptions.SnitchOn));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Snitch Done");
                    }

                    if (CustomGameOptions.InsiderOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.InsiderCount : 1;
                        
                        while (num > 0)
                        {
                            CrewAbilityGet.Add((typeof(Insider), CustomRPC.SetInsider, CustomGameOptions.InsiderOn));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Insider Done");
                    }

                    if (CustomGameOptions.LighterOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.LighterCount : 1;
                        
                        while (num > 0)
                        {
                            CrewAbilityGet.Add((typeof(Lighter), CustomRPC.SetLighter, CustomGameOptions.LighterOn));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Lighter Done");
                    }

                    if (CustomGameOptions.MultitaskerOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.MultitaskerCount : 1;
                        
                        while (num > 0)
                        {
                            TaskedAbilityGet.Add((typeof(Multitasker), CustomRPC.SetMultitasker, CustomGameOptions.MultitaskerOn));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Multitasker Done");
                    }

                    if (CustomGameOptions.RadarOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.RadarCount : 1;
                        
                        while (num > 0)
                        {
                            GlobalAbilityGet.Add((typeof(Radar), CustomRPC.SetRadar, CustomGameOptions.RadarOn));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Radar Done");
                    }

                    if (CustomGameOptions.TiebreakerOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.TiebreakerCount : 1;
                        
                        while (num > 0)
                        {
                            GlobalAbilityGet.Add((typeof(Tiebreaker), CustomRPC.SetTiebreaker, CustomGameOptions.TiebreakerOn));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Tiebreaker Done");
                    }

                    if (CustomGameOptions.TorchOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.TorchCount : 1;
                        
                        while (num > 0)
                        {
                            NonEvilAbilityGet.Add((typeof(Torch), CustomRPC.SetTorch, CustomGameOptions.TorchOn));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Torch Done");
                    }

                    if (CustomGameOptions.UnderdogOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.UnderdogCount : 1;
                        
                        while (num > 0)
                        {
                            EvilAbilityGet.Add((typeof(Underdog), CustomRPC.SetUnderdog, CustomGameOptions.UnderdogOn));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Underdog Done");
                    }

                    if (CustomGameOptions.TunnelerOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.TunnelerCount : 1;
                        
                        while (num > 0)
                        {
                            TunnelerAbilityGet.Add((typeof(Tunneler), CustomRPC.SetTunneler, CustomGameOptions.TunnelerOn));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Tunneler Done");
                    }
                    #endregion

                    #region Objectifiers
                    if (CustomGameOptions.LoversOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.LoversCount : 1;
                        
                        while (num > 0)
                        {
                            LoverRivalObjectifierGet.Add((typeof(Lovers), CustomRPC.SetCouple, CustomGameOptions.LoversOn));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Lovers Done");
                    }

                    if (CustomGameOptions.RivalsOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.RivalsCount : 1;
                        
                        while (num > 0)
                        {
                            LoverRivalObjectifierGet.Add((typeof(Rivals), CustomRPC.SetDuo, CustomGameOptions.RivalsOn));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Rivals Done");
                    }

                    if (CustomGameOptions.FanaticOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.FanaticCount : 1;
                        
                        while (num > 0)
                        {
                            CrewObjectifierGet.Add((typeof(Fanatic), CustomRPC.SetFanatic, CustomGameOptions.FanaticOn));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Fanatic Done");
                    }

                    if (CustomGameOptions.CorruptedOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.CorruptedCount : 1;
                        
                        while (num > 0)
                        {
                            CrewObjectifierGet.Add((typeof(Corrupted), CustomRPC.SetCorrupted, CustomGameOptions.CorruptedOn));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Corrupted Done");
                    }

                    if (CustomGameOptions.OverlordOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.OverlordCount : 1;
                        
                        while (num > 0)
                        {
                            NeutralObjectifierGet.Add((typeof(Overlord), CustomRPC.SetOverlord, CustomGameOptions.OverlordOn));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Overlord Done");
                    }

                    if (CustomGameOptions.TraitorOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.TraitorCount : 1;
                        
                        while (num > 0)
                        {
                            CrewObjectifierGet.Add((typeof(Traitor), CustomRPC.SetTraitor, CustomGameOptions.TraitorOn));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Traitor Done");
                    }

                    if (CustomGameOptions.TaskmasterOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.TaskmasterCount : 1;
                        
                        while (num > 0)
                        {
                            NeutralObjectifierGet.Add((typeof(Taskmaster), CustomRPC.SetTaskmaster, CustomGameOptions.TaskmasterOn));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Taskmaster Done");
                    }

                    if (CustomGameOptions.PhantomOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.PhantomCount : 1;
                        
                        while (num > 0)
                        {
                            NeutralObjectifierGet.Add((typeof(Phantom), CustomRPC.SetPhantom, CustomGameOptions.PhantomOn));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Phantom Done");
                    }
                    #endregion
                }

                PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Role Gen Start");

                RoleGen(infected.ToList());

                PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Role Gen Done");
            }
        }
    }
}