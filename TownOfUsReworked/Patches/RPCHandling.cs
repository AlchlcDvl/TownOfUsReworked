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
using TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Objectifiers.AlliedMod;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Abilities.AssassinMod;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.SwapperMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TimeLordMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RevealerMod;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.ExecutionerMod;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GuardianAngelMod;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GuesserMod;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.PhantomMod;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.ConsigliereMod;
using TownOfUsReworked.PlayerLayers.Objectifiers.TraitorMod;
using Reactor.Networking.Extensions;
using AmongUs.GameOptions;
using Coroutine = TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.JanitorMod.Coroutine;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using Eat = TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.CannibalMod.Coroutine;
using Revive = TownOfUsReworked.PlayerLayers.Roles.CrewRoles.AltruistMod.Coroutine;
using Alt = TownOfUsReworked.PlayerLayers.Roles.CrewRoles.AltruistMod.KillButtonTarget;
using PerformRemember = TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.AmnesiacMod.PerformRemember;
using PerformSteal = TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.ThiefMod.PerformSteal;
using PerformDeclare = TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GodfatherMod.PerformAbility;
using PerformSidekick = TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.RebelMod.PerformAbility;
using PerformShift = TownOfUsReworked.PlayerLayers.Roles.CrewRoles.ShifterMod.PerformShift;
using PerformConvert = TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.DraculaMod.PerformConvert;

namespace TownOfUsReworked.Patches
{
    public static class RPCHandling
    {
        private static readonly List<(Type, int, int, bool)> CrewAuditorRoles = new List<(Type, int, int, bool)>();
        private static readonly List<(Type, int, int, bool)> CrewInvestigativeRoles = new List<(Type, int, int, bool)>();
        private static readonly List<(Type, int, int, bool)> CrewKillingRoles = new List<(Type, int, int, bool)>();
        private static readonly List<(Type, int, int, bool)> CrewProtectiveRoles = new List<(Type, int, int, bool)>();
        private static readonly List<(Type, int, int, bool)> CrewSovereignRoles = new List<(Type, int, int, bool)>();
        private static readonly List<(Type, int, int, bool)> CrewSupportRoles = new List<(Type, int, int, bool)>();
        private static readonly List<(Type, int, int, bool)> CrewRoles = new List<(Type, int, int, bool)>();
        
        private static readonly List<(Type, int, int, bool)> NeutralEvilRoles = new List<(Type, int, int, bool)>();
        private static readonly List<(Type, int, int, bool)> NeutralBenignRoles = new List<(Type, int, int, bool)>();
        private static readonly List<(Type, int, int, bool)> NeutralKillingRoles = new List<(Type, int, int, bool)>();
        private static readonly List<(Type, int, int, bool)> NeutralNeophyteRoles = new List<(Type, int, int, bool)>();
        private static readonly List<(Type, int, int, bool)> NeutralRoles = new List<(Type, int, int, bool)>();

        private static readonly List<(Type, int, int, bool)> IntruderDeceptionRoles = new List<(Type, int, int, bool)>();
        private static readonly List<(Type, int, int, bool)> IntruderConcealingRoles = new List<(Type, int, int, bool)>();
        private static readonly List<(Type, int, int, bool)> IntruderKillingRoles = new List<(Type, int, int, bool)>();
        private static readonly List<(Type, int, int, bool)> IntruderSupportRoles = new List<(Type, int, int, bool)>();
        private static readonly List<(Type, int, int, bool)> IntruderRoles = new List<(Type, int, int, bool)>();

        private static readonly List<(Type, int, int, bool)> SyndicateDisruptionRoles = new List<(Type, int, int, bool)>();
        private static readonly List<(Type, int, int, bool)> SyndicateKillingRoles = new List<(Type, int, int, bool)>();
        private static readonly List<(Type, int, int, bool)> SyndicatePowerRoles = new List<(Type, int, int, bool)>();
        private static readonly List<(Type, int, int, bool)> SyndicateSupportRoles = new List<(Type, int, int, bool)>();
        private static readonly List<(Type, int, int, bool)> SyndicateRoles = new List<(Type, int, int, bool)>();

        private static readonly List<(Type, int, int)> GlobalModifiers = new List<(Type, int, int)>();
        private static readonly List<(Type, int, int)> ProfessionalModifiers = new List<(Type, int, int)>();
        private static readonly List<(Type, int, int)> BaitModifiers = new List<(Type, int, int)>();
        private static readonly List<(Type, int, int)> DiseasedModifiers = new List<(Type, int, int)>();
        private static readonly List<(Type, int, int)> AllModifiers = new List<(Type, int, int)>();

        private static readonly List<(Type, int, int)> GlobalAbilityGet = new List<(Type, int, int)>();
        private static readonly List<(Type, int, int)> IntruderAbilityGet = new List<(Type, int, int)>();
        private static readonly List<(Type, int, int)> CrewAbilityGet = new List<(Type, int, int)>();
        private static readonly List<(Type, int, int)> TunnelerAbilityGet = new List<(Type, int, int)>();
        private static readonly List<(Type, int, int)> NonEvilAbilityGet = new List<(Type, int, int)>();
        private static readonly List<(Type, int, int)> TaskedAbilityGet = new List<(Type, int, int)>();
        private static readonly List<(Type, int, int)> SnitchAbilityGet = new List<(Type, int, int)>();
        private static readonly List<(Type, int, int)> NonVentingAbilityGet = new List<(Type, int, int)>();
        private static readonly List<(Type, int, int)> EvilAbilityGet = new List<(Type, int, int)>();
        private static readonly List<(Type, int, int)> SyndicateAbilityGet = new List<(Type, int, int)>();
        private static readonly List<(Type, int, int)> NeutralAbilityGet = new List<(Type, int, int)>();
        private static readonly List<(Type, int, int)> AllAbilities = new List<(Type, int, int)>();

        private static readonly List<(Type, int, int)> CrewObjectifierGet = new List<(Type, int, int)>();
        private static readonly List<(Type, int, int)> CorruptedObjectifierGet = new List<(Type, int, int)>();
        private static readonly List<(Type, int, int)> NeutralObjectifierGet = new List<(Type, int, int)>();
        private static readonly List<(Type, int, int)> OverlordObjectifierGet = new List<(Type, int, int)>();
        private static readonly List<(Type, int, int)> LoverRivalObjectifierGet = new List<(Type, int, int)>();
        private static readonly List<(Type, int, int)> GlobalObjectifierGet = new List<(Type, int, int)>();
        private static readonly List<(Type, int, int)> AllObjectifiers = new List<(Type, int, int)>();

        private static readonly bool IsAA = CustomGameOptions.GameMode == GameMode.AllAny;
        private static readonly bool IsCustom = CustomGameOptions.GameMode == GameMode.Custom;
        private static readonly bool IsClassic = CustomGameOptions.GameMode == GameMode.Classic;
        private static readonly bool IsKilling = CustomGameOptions.GameMode == GameMode.KillingOnly;

        private static bool PhantomOn;
        private static bool RevealerOn;

        public static void Shuffle<T>(this List<T> list)
        {
            var count = list.Count;
            var last = count - 1;
            
            for (var i = 0; i <= last; ++i)
            {
                var r = Random.Range(i, count);
                var tmp = list[i];
                list[i] = list[r];
                list[r] = tmp;
            }
        }

        public static T TakeFirst<T>(this List<T> list)
        {
            list.Shuffle();
            var item = list[0];

            while (item == null)
            {
                list.Shuffle();
                item = list[0];
            }

            list.RemoveAt(0);
            return item;
        }

        private static void SortThings(List<(Type, int, int, bool)> items, int max, int min)
        {
            if (items.Count == 0)
                return;

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
                var a_ = a.Item2 == 100 ? 0 : 100;
                var b_ = b.Item2 == 100 ? 0 : 100;
                return a_.CompareTo(b_);
            });

            var certainItems = 0;
            var odds = 0;

            foreach (var role in items)
            {
                if (role.Item2 == 100)
                    certainItems += 1;
                else
                    odds += role.Item2;
            }

            while (certainItems < amount)
            {
                var num = certainItems;
                var random = Random.RandomRangeInt(0, odds);
                var rolePicked = false;

                while (num < items.Count && rolePicked == false)
                {
                    random -= items[num].Item2;

                    if (random < 0)
                    {
                        odds -= items[num].Item2;
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

        private static void SortThings(List<(Type, int, int)> items, int max, int min)
        {
            if (items.Count == 0)
                return;
                
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
                var a_ = a.Item2 == 100 ? 0 : 100;
                var b_ = b.Item2 == 100 ? 0 : 100;
                return a_.CompareTo(b_);
            });

            var certainItems = 0;
            var odds = 0;

            foreach (var role in items)
            {
                if (role.Item2 == 100)
                    certainItems += 1;
                else
                    odds += role.Item2;
            }

            while (certainItems < amount)
            {
                var num = certainItems;
                var random = Random.RandomRangeInt(0, odds);
                var rolePicked = false;

                while (num < items.Count && rolePicked == false)
                {
                    random -= items[num].Item2;

                    if (random < 0)
                    {
                        odds -= items[num].Item2;
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

        internal static bool Check(int probability)
        {
            if (probability == 0)
                return false;

            if (probability == 100)
                return true;

            var num = Random.RandomRangeInt(1, 101);
            return num <= probability;
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

                        while (maxNeut >= crewmates.Count)
                            maxNeut--;
                        
                        while (minNeut >= crewmates.Count)
                            minNeut--;

                        SortThings(NeutralRoles, maxNeut, minNeut);
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

                        while (maxSyn > CustomGameOptions.SyndicateCount || maxSyn >= crewmates.Count - NeutralRoles.Count)
                            maxSyn--;

                        while (minSyn > CustomGameOptions.SyndicateCount || minSyn >= crewmates.Count - NeutralRoles.Count)
                            minSyn--;

                        SortThings(SyndicateRoles, maxSyn, minSyn);

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
                            CrewRoles.Add((typeof(Crewmate), 100, 20, false));
                        
                        CrewRoles.Shuffle();
                        
                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Classic/Custom Role Sort Done");
                    }
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

                    AllModifiers.AddRange(GlobalModifiers);
                    AllModifiers.AddRange(ProfessionalModifiers);
                    AllModifiers.AddRange(BaitModifiers);
                    AllModifiers.AddRange(DiseasedModifiers);

                    AllAbilities.AddRange(GlobalAbilityGet);
                    AllAbilities.AddRange(IntruderAbilityGet);
                    AllAbilities.AddRange(CrewAbilityGet);
                    AllAbilities.AddRange(TunnelerAbilityGet);
                    AllAbilities.AddRange(NonEvilAbilityGet);
                    AllAbilities.AddRange(TaskedAbilityGet);
                    AllAbilities.AddRange(SnitchAbilityGet);
                    AllAbilities.AddRange(NonVentingAbilityGet);
                    AllAbilities.AddRange(EvilAbilityGet);
                    AllAbilities.AddRange(SyndicateAbilityGet);
                    AllAbilities.AddRange(NeutralAbilityGet);
                    
                    AllObjectifiers.AddRange(CrewObjectifierGet);
                    AllObjectifiers.AddRange(CorruptedObjectifierGet);
                    AllObjectifiers.AddRange(NeutralObjectifierGet);
                    AllObjectifiers.AddRange(OverlordObjectifierGet);
                    AllObjectifiers.AddRange(GlobalObjectifierGet);
                    AllObjectifiers.AddRange(LoverRivalObjectifierGet);

                    PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("All Any Role Sort Done");
                }

                var NonIntruderRoles = new List<(Type, int, int, bool)>();

                NonIntruderRoles.AddRange(CrewRoles);
                NonIntruderRoles.AddRange(NeutralRoles);

                if (!CustomGameOptions.AltImps)
                    NonIntruderRoles.AddRange(SyndicateRoles);
                else
                {
                    IntruderRoles.Clear();
                    IntruderRoles.AddRange(SyndicateRoles);
                }

                var nonIntruderRoles = new List<(Type, int, int, bool)>();
                var impRoles = new List<(Type, int, int, bool)>();

                if (IsAA)
                {
                    while (nonIntruderRoles.Count <= crewmates.Count && NonIntruderRoles.Count > 0)
                    {
                        NonIntruderRoles.Shuffle();
                        nonIntruderRoles.Add(NonIntruderRoles[0]);

                        if (NonIntruderRoles[0].Item4 == true && CustomGameOptions.EnableUniques)
                            NonIntruderRoles.Remove(NonIntruderRoles[0]);
                    }

                    PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("All Any Role List 1 Done");

                    while (impRoles.Count <= impostors.Count && IntruderRoles.Count > 0)
                    {
                        IntruderRoles.Shuffle();                            
                        impRoles.Add(IntruderRoles[0]);

                        if (IntruderRoles[0].Item4 == true && CustomGameOptions.EnableUniques)
                            IntruderRoles.Remove(IntruderRoles[0]);
                    }

                    PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("All Any Role List 2 Done");
                }

                nonIntruderRoles.Shuffle();
                impRoles.Shuffle();

                if (IsAA)
                {
                    while (crewmates.Count > 0 && nonIntruderRoles.Count > 0)
                    {
                        var (type, _, id, unique) = nonIntruderRoles.TakeFirst();
                        Role.GenRole<Role>(type, crewmates.TakeFirst(), id);
                    }

                    PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("All Any Roles Spawn 1 Done");

                    while (impostors.Count > 0 && impRoles.Count > 0)
                    {
                        var (type, _, id, unique) = impRoles.TakeFirst();
                        Role.GenRole<Role>(type, impostors.TakeFirst(), id);
                    }

                    PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("All Any Role Spawn 2 Done");
                }
                else
                {
                    while (crewmates.Count > 0 && NonIntruderRoles.Count > 0)
                    {
                        var (type, _, id, unique) = NonIntruderRoles.TakeFirst();
                        Role.GenRole<Role>(type, crewmates.TakeFirst(), id);
                    }

                    PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Classic/Custom Role Spawn 1 Done");

                    while (impostors.Count > 0 && IntruderRoles.Count > 0)
                    {
                        var (type, _, id, unique) = IntruderRoles.TakeFirst();
                        Role.GenRole<Role>(type, impostors.TakeFirst(), id);
                    }

                    PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Classic/Custom Role Spawn 2 Done");
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

                if (CustomGameOptions.SyndicateCount >= 3)
                    SyndicateRoles.Add((typeof(Rebel), CustomGameOptions.RebelOn, 61, CustomGameOptions.UniqueRebel));

                NeutralKillingRoles.Add((typeof(Glitch), CustomGameOptions.GlitchOn, 27, CustomGameOptions.UniqueGlitch));
                NeutralKillingRoles.Add((typeof(Werewolf), CustomGameOptions.WerewolfOn, 30, CustomGameOptions.UniqueWerewolf));
                NeutralKillingRoles.Add((typeof(SerialKiller), CustomGameOptions.SerialKillerOn, 35, CustomGameOptions.UniqueSerialKiller));
                NeutralKillingRoles.Add((typeof(Juggernaut), CustomGameOptions.JuggernautOn, 36, CustomGameOptions.UniqueJuggernaut));
                NeutralKillingRoles.Add((typeof(Murderer), CustomGameOptions.MurdererOn, 28, CustomGameOptions.UniqueMurderer));

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
                        CrewRoles.Add((typeof(Vigilante), 100, 3, false));
                        vigis -= 1;
                        CrewRoles.Add((typeof(Veteran), 100, 11, false));
                        vets -= 1;
                    }
                }

                SortThings(IntruderRoles, CustomGameOptions.IntruderCount, CustomGameOptions.IntruderCount);
                SortThings(SyndicateRoles, CustomGameOptions.SyndicateCount, CustomGameOptions.SyndicateCount);

                PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Killing Role List Sorted");

                var nonIntruderRoles = new List<(Type, int, int, bool)>();

                nonIntruderRoles.AddRange(CrewRoles);
                nonIntruderRoles.AddRange(NeutralKillingRoles);

                if (!CustomGameOptions.AltImps)
                    nonIntruderRoles.AddRange(SyndicateRoles);
                else
                {
                    IntruderRoles.Clear();
                    IntruderRoles.AddRange(SyndicateRoles);
                }
                    
                nonIntruderRoles.Shuffle();
                IntruderRoles.Shuffle();

                while (impostors.Count > 0 && IntruderRoles.Count > 0)
                {
                    var (type, _, id, unique) = IntruderRoles.TakeFirst();
                    Role.GenRole<Role>(type, impostors.TakeFirst(), id);
                }

                PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Killing Role Spawn 1 Done");

                while (crewmates.Count > 0 && nonIntruderRoles.Count > 0)
                {
                    var (type, _, id, unique) = nonIntruderRoles.TakeFirst();
                    Role.GenRole<Role>(type, crewmates.TakeFirst(), id);
                }

                PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Killing Role Spawn 2 Done");
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
            SortThings(SnitchAbilityGet, crewCount, crewCount);
            SortThings(TunnelerAbilityGet, crewCount, crewCount);
            SortThings(TaskedAbilityGet, crewCount + neutralCount, crewCount + neutralCount);
            SortThings(NeutralAbilityGet, neutralCount, neutralCount);
            SortThings(SyndicateAbilityGet, syndicateCount, syndicateCount);
            
            SortThings(CrewObjectifierGet, crewCount, crewCount);
            SortThings(CorruptedObjectifierGet, crewCount, crewCount);
            SortThings(NeutralObjectifierGet, neutralCount, neutralCount);
            SortThings(OverlordObjectifierGet, neutralCount, neutralCount);
            SortThings(GlobalObjectifierGet, allCount, allCount);
            SortThings(LoverRivalObjectifierGet, allCount, allCount);
            SortThings(EvilAbilityGet, intruderCount + syndicateCount, intruderCount + syndicateCount);
            SortThings(NonEvilAbilityGet, crewCount + neutralCount, crewCount + neutralCount);

            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Layers Sorted");

            if (CustomGameOptions.EnableObjectifiers)
            {
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
                
                if (!IsAA)
                {
                    while ((canHaveObjectifier.Count > 0 && NeutralObjectifierGet.Count > 0) || (canHaveObjectifier2.Count > 0 && CrewObjectifierGet.Count > 0) || (canHaveObjectifier3.Count >
                        0 && GlobalObjectifierGet.Count > 0) || (canHaveLoverorRival.Count > 4 && LoverRivalObjectifierGet.Count > 0))
                    {
                        var random = Random.RandomRangeInt(0, 4);

                        if (canHaveObjectifier.Count > 0 && NeutralObjectifierGet.Count > 0 && random == 0)
                        {
                            var (type, _, id) = NeutralObjectifierGet.TakeFirst();
                            Objectifier.GenObjectifier<Objectifier>(type, canHaveObjectifier.TakeFirst(), id);
                        }
                        else if (canHaveObjectifier2.Count > 0 && CrewObjectifierGet.Count > 0 && random == 1)
                        {
                            var (type, _, id) = CrewObjectifierGet.TakeFirst();
                            Objectifier.GenObjectifier<Objectifier>(type, canHaveObjectifier2.TakeFirst(), id);
                        }
                        else if (canHaveObjectifier3.Count > 0 && GlobalObjectifierGet.Count > 0 && random == 2)
                        {
                            var (type, _, id) = GlobalObjectifierGet.TakeFirst();
                            Objectifier.GenObjectifier<Objectifier>(type, canHaveObjectifier3.TakeFirst(), id);  
                        }
                        else if (canHaveLoverorRival.Count > 4 && LoverRivalObjectifierGet.Count > 0 && random == 3)
                        {
                            var (type, _, id) = LoverRivalObjectifierGet.TakeFirst();

                            if (id == 0)
                                Lovers.Gen(canHaveLoverorRival);
                            else if (id == 1)
                                Rivals.Gen(canHaveLoverorRival);
                        }
                    }
                }

                PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Objectifiers Done");
            }

            if (CustomGameOptions.EnableAbilities)
            {
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

                canHaveAbility2.RemoveAll(player => !(player.Is(RoleAlignment.NeutralNeo) || player.Is(RoleAlignment.NeutralKill) || player.Is(RoleAlignment.NeutralPros)));
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
                
                if (!IsAA)
                {
                    while ((canHaveAbility7.Count > 0 && SnitchAbilityGet.Count > 0) || (CustomGameOptions.WhoCanVent == WhoCanVentOptions.Default && canHaveAbility5.Count > 0 &&
                        TunnelerAbilityGet.Count > 0) || (canHaveAbility9.Count > 0 && NonEvilAbilityGet.Count > 0) || (canHaveAbility10.Count > 0 && EvilAbilityGet.Count > 0) ||
                        (canHaveAbility8.Count > 0 && TaskedAbilityGet.Count > 0) || (canHaveAbility3.Count > 0 && CrewAbilityGet.Count > 0) || (canHaveAbility.Count > 0 &&
                        IntruderAbilityGet.Count > 0) || (canHaveAbility2.Count > 0 && NeutralAbilityGet.Count > 0) || (canHaveAbility4.Count > 0 && SyndicateAbilityGet.Count > 0) ||
                        (canHaveAbility11.Count > 0 && GlobalAbilityGet.Count > 0))
                    {
                        var random = Random.RandomRangeInt(0, 10);

                        if (canHaveAbility7.Count > 0 && SnitchAbilityGet.Count > 0 && random == 0)
                        {
                            var (type, _, id) = SnitchAbilityGet.TakeFirst();
                            Ability.GenAbility<Ability>(type, canHaveAbility7.TakeFirst(), id);
                        }
                        else if (CustomGameOptions.WhoCanVent == WhoCanVentOptions.Default && canHaveAbility5.Count > 0 && TunnelerAbilityGet.Count > 0 && random == 1)
                        {
                            var (type, _, id) = TunnelerAbilityGet.TakeFirst();
                            Ability.GenAbility<Ability>(type, canHaveAbility5.TakeFirst(), id);
                        }
                        else if (canHaveAbility9.Count > 0 && NonEvilAbilityGet.Count > 0 && random == 2)
                        {
                            var (type, _, id) = NonEvilAbilityGet.TakeFirst();
                            Ability.GenAbility<Ability>(type, canHaveAbility9.TakeFirst(), id);
                        }
                        else if (canHaveAbility10.Count > 0 && EvilAbilityGet.Count > 0 && random == 3)
                        {
                            var (type, _, id) = EvilAbilityGet.TakeFirst();
                            Ability.GenAbility<Ability>(type, canHaveAbility10.TakeFirst(), id);
                        }
                        else if (canHaveAbility8.Count > 0 && TaskedAbilityGet.Count > 0 && random == 4)
                        {
                            var (type, _, id) = TaskedAbilityGet.TakeFirst();
                            Ability.GenAbility<Ability>(type, canHaveAbility8.TakeFirst(), id);
                        }
                        else if (canHaveAbility3.Count > 0 && CrewAbilityGet.Count > 0 && random == 5)
                        {
                            var (type, _, id) = CrewAbilityGet.TakeFirst();
                            Ability.GenAbility<Ability>(type, canHaveAbility3.TakeFirst(), id);
                        }
                        else if (canHaveAbility.Count > 0 && IntruderAbilityGet.Count > 0 && random == 6)
                        {
                            var (type, _, id) = IntruderAbilityGet.TakeFirst();
                            Ability.GenAbility<Ability>(type, canHaveAbility.TakeFirst(), id);
                        }
                        else if (canHaveAbility2.Count > 0 && NeutralAbilityGet.Count > 0 && random == 7)
                        {
                            var (type, _, id) = NeutralAbilityGet.TakeFirst();
                            Ability.GenAbility<Ability>(type, canHaveAbility2.TakeFirst(), id);
                        }
                        else if (canHaveAbility4.Count > 0 && SyndicateAbilityGet.Count > 0 && random == 8)
                        {
                            var (type, _, id) = SyndicateAbilityGet.TakeFirst();
                            Ability.GenAbility<Ability>(type, canHaveAbility4.TakeFirst(), id);
                        }
                        else if (canHaveAbility11.Count > 0 && GlobalAbilityGet.Count > 0 && random == 9)
                        {
                            var (type, _, id) = GlobalAbilityGet.TakeFirst();
                            Ability.GenAbility<Ability>(type, canHaveAbility11.TakeFirst(), id);
                        }
                    }
                }

                PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Abilities Done");
            }

            if (CustomGameOptions.EnableModifiers)
            {
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
                
                if (!IsAA)
                {
                    while ((canHaveModifier1.Count > 0 && BaitModifiers.Count > 0) || (canHaveModifier2.Count > 0 && DiseasedModifiers.Count > 0) || (canHaveModifier3.Count > 0 &&
                        ProfessionalModifiers.Count > 0) || (canHaveModifier4.Count > 0 && GlobalModifiers.Count > 0))
                    {
                        var random = Random.RandomRangeInt(0, 4);

                        if (canHaveModifier1.Count > 0 && BaitModifiers.Count > 0 && random == 0)
                        {
                            var (type, _, id) = BaitModifiers.TakeFirst();
                            Modifier.GenModifier<Modifier>(type, canHaveModifier1.TakeFirst(), id);
                        }
                        else if (canHaveModifier2.Count > 0 && DiseasedModifiers.Count > 0 && random == 1)
                        {
                            var (type, _, id) = DiseasedModifiers.TakeFirst();
                            Modifier.GenModifier<Modifier>(type, canHaveModifier2.TakeFirst(), id);
                        }
                        else if (canHaveModifier3.Count > 0 && ProfessionalModifiers.Count > 0 && random == 2)
                        {
                            var (type, _, id) = ProfessionalModifiers.TakeFirst();
                            Modifier.GenModifier<Modifier>(type, canHaveModifier3.TakeFirst(), id);
                        }
                        else if (canHaveModifier4.Count > 0 && GlobalModifiers.Count > 0 && random == 3)
                        {
                            var (type, _, id) = GlobalModifiers.TakeFirst();
                            Modifier.GenModifier<Modifier>(type, canHaveModifier4.TakeFirst(), id);
                        }
                    }
                }

                PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Modifiers Done");
            }

            if (CustomGameOptions.AlliedOn > 0)
            {
                foreach (Allied ally in Objectifier.GetObjectifiers(ObjectifierEnum.Allied))
                {
                    var alliedRole = Role.GetRole(ally.Player);

                    if (CustomGameOptions.AlliedFaction == AlliedFaction.Intruder)
                        alliedRole.Faction = Faction.Intruder;
                    else if (CustomGameOptions.AlliedFaction == AlliedFaction.Syndicate)
                        alliedRole.Faction = Faction.Syndicate;
                    else if (CustomGameOptions.AlliedFaction == AlliedFaction.Crew)
                        alliedRole.Faction = Faction.Crew;
                    else if (CustomGameOptions.AlliedFaction == AlliedFaction.Random)
                    {
                        var random = Random.RandomRangeInt(0, 3);

                        if (random == 0)
                            alliedRole.Faction = Faction.Intruder;
                        else if (random == 1)
                            alliedRole.Faction = Faction.Syndicate;
                        else if (random == 2)
                            alliedRole.Faction = Faction.Crew;
                    }

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetAlliedFaction, SendOption.Reliable, -1);
                    writer.Write(ally.Player.PlayerId);
                    writer.Write((byte)alliedRole.Faction);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Allied Faction Set Done");
            }

            if (!IsKilling)
            {
                var toChooseFromNeut = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Neutral)).ToList();

                if (PhantomOn && toChooseFromNeut.Count != 0)
                {
                    var rand = Random.RandomRangeInt(0, toChooseFromNeut.Count);
                    var pc = toChooseFromNeut[rand];

                    SetPhantom.WillBePhantom = pc;

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetPhantom, SendOption.Reliable, -1);
                    writer.Write(pc.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
                else
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetPhantom, SendOption.Reliable, -1);
                    writer.Write(byte.MaxValue);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                var toChooseFromCrew = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crew) && !x.Is(ObjectifierEnum.Traitor) &&
                    !x.Is(ObjectifierEnum.Corrupted) && !x.Is(ObjectifierEnum.Fanatic)).ToList();

                if (RevealerOn && toChooseFromCrew.Count != 0)
                {
                    var rand = Random.RandomRangeInt(0, toChooseFromCrew.Count);
                    var pc = toChooseFromCrew[rand];

                    SetRevealer.WillBeRevealer = pc;

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetRevealer, SendOption.Reliable, -1);
                    writer.Write(pc.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
                else
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetRevealer, SendOption.Reliable, -1);
                    writer.Write(byte.MaxValue);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }

                var exeTargets = new List<PlayerControl>();
                var gaTargets = new List<PlayerControl>();
                var guessTargets = new List<PlayerControl>();
                var goodRecruits = new List<PlayerControl>();
                var evilRecruits = new List<PlayerControl>();
                
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Is(Faction.Crew))
                    {
                        if (!player.Is(RoleEnum.Altruist))
                            gaTargets.Add(player);
                        
                        if (!player.HasObjectifier())
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
                            {
                                if (!player.HasObjectifier())
                                    evilRecruits.Add(player);
                            }
                            else if (player.Is(RoleAlignment.NeutralEvil))
                            {
                                if (!player.HasObjectifier())
                                    goodRecruits.Add(player);
                            }
                        }

                        if (CustomGameOptions.ExeCanHaveNeutralTargets)
                            exeTargets.Add(player);
                    }
                    else if (player.Is(Faction.Intruder) || player.Is(Faction.Syndicate))
                    {
                        gaTargets.Add(player);
                        
                        if (!player.HasObjectifier())
                            evilRecruits.Add(player);

                        if ((player.Is(Faction.Intruder) && CustomGameOptions.ExeCanHaveIntruderTargets) || (player.Is(Faction.Syndicate) && CustomGameOptions.ExeCanHaveSyndicateTargets))
                            exeTargets.Add(player);
                    }

                    guessTargets.Add(player);
                }

                PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Targets Set");

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
                            //PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage($"Exe Target = {exe.TargetPlayer.name}");
                        }
                    }

                    PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Exe Target Set");
                }

                if (CustomGameOptions.GuesserOn > 0)
                {
                    foreach (Guesser guess in Role.GetRoles(RoleEnum.Guesser))
                    {
                        guess.TargetPlayer = null;

                        if (guessTargets.Count > 0)
                        {
                            while (guess.TargetPlayer == null || guess.TargetPlayer == guess.Player)
                            {
                                guessTargets.Shuffle();
                                var guessNum = Random.RandomRangeInt(0, guessTargets.Count - 1);
                                guess.TargetPlayer = guessTargets[guessNum];
                            }

                            guessTargets.Remove(guess.TargetPlayer);

                            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetGuessTarget, SendOption.Reliable, -1);
                            writer.Write(guess.Player.PlayerId);
                            writer.Write(guess.TargetPlayer.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            //PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage($"Exe Target = {exe.TargetPlayer.name}");
                        }
                    }

                    PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Guess Target Set");
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
                            //PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage($"GA Target = {ga.TargetPlayer.name}");
                        }
                    }

                    PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("GA Target Set");
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
                            //PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage($"Good Recruit = {jackal.GoodRecruit.name}");
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
                            //PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage($"Evil Recruit = {jackal.EvilRecruit.name}");
                        }
                    }

                    PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Jackal Recruits Set");
                }
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
                    case CustomRPC.SetRole:
                        var player = Utils.PlayerById(reader.ReadByte());
                        var id = reader.ReadInt32();

                        switch (id)
                        {
                            case 0:
                                new Mayor(player);
                                break;
                            case 1:
                                new Sheriff(player);
                                break;
                            case 2:
                                new Inspector(player);
                                break;
                            case 3:
                                new Vigilante(player);
                                break;
                            case 4:
                                new Engineer(player);
                                break;
                            case 5:
                                new Swapper(player);
                                break;
                            case 6:
                                new Investigator(player);
                                break;
                            case 7:
                                new TimeLord(player);
                                break;
                            case 8:
                                new Medic(player);
                                break;
                            case 9:
                                new Agent(player);
                                break;
                            case 10:
                                new Altruist(player);
                                break;
                            case 11:
                                new Veteran(player);
                                break;
                            case 12:
                                new Tracker(player);
                                break;
                            case 13:
                                new Transporter(player);
                                break;
                            case 14:
                                new Medium(player);
                                break;
                            case 15:
                                new Coroner(player);
                                break;
                            case 16:
                                new Operative(player);
                                break;
                            case 17:
                                new Detective(player);
                                break;
                            case 18:
                                new Escort(player);
                                break;
                            case 19:
                                new Shifter(player);
                                break;
                            case 20:
                                new Crewmate(player);
                                break;
                            case 21:
                                new VampireHunter(player);
                                break;
                            case 22:
                                new Jester(player);
                                break;
                            case 23:
                                new Amnesiac(player);
                                break;
                            case 24:
                                new Executioner(player);
                                break;
                            case 25:
                                new Survivor(player);
                                break;
                            case 26:
                                new GuardianAngel(player);
                                break;
                            case 27:
                                new Glitch(player);
                                break;
                            case 28:
                                new Murderer(player);
                                break;
                            case 29:
                                new Cryomaniac(player);
                                break;
                            case 30:
                                new Werewolf(player);
                                break;
                            case 31:
                                new Arsonist(player);
                                break;
                            case 32:
                                new Jackal(player);
                                break;
                            case 33:
                                new Plaguebearer(player);
                                break;
                            case 34:
                                new Pestilence(player);
                                break;
                            case 35:
                                new SerialKiller(player);
                                break;
                            case 36:
                                new Juggernaut(player);
                                break;
                            case 37:
                                new Cannibal(player);
                                break;
                            case 38:
                                new Thief(player);
                                break;
                            case 39:
                                new Dracula(player);
                                break;
                            case 40:
                                new Troll(player);
                                break;
                            case 41:
                                new Undertaker(player);
                                break;
                            case 42:
                                new Morphling(player);
                                break;
                            case 43:
                                new Blackmailer(player);
                                break;
                            case 44:
                                new Miner(player);
                                break;
                            case 45:
                                new Teleporter(player);
                                break;
                            case 46:
                                new Wraith(player);
                                break;
                            case 47:
                                new Consort(player);
                                break;
                            case 48:
                                new Janitor(player);
                                break;
                            case 49:
                                new Camouflager(player);
                                break;
                            case 50:
                                new Grenadier(player);
                                break;
                            case 51:
                                new Poisoner(player);
                                break;
                            case 52:
                                new Impostor(player);
                                break;
                            case 53:
                                new Consigliere(player);
                                break;
                            case 54:
                                new Disguiser(player);
                                break;
                            case 55:
                                new TimeMaster(player);
                                break;
                            case 56:
                                new Godfather(player);
                                break;
                            case 57:
                                new Anarchist(player);
                                break;
                            case 58:
                                new Shapeshifter(player);
                                break;
                            case 59:
                                new Gorgon(player);
                                break;
                            case 60:
                                new Framer(player);
                                break;
                            case 61:
                                new Rebel(player);
                                break;
                            case 62:
                                new Concealer(player);
                                break;
                            case 63:
                                new Warper(player);
                                break;
                            case 64:
                                new Bomber(player);
                                break;
                            case 65:
                                new Chameleon(player);
                                break;
                            case 66:
                                new Guesser(player);
                                break;
                            case 67:
                                new Whisperer(player);
                                break;
                        }

                        break;

                    case CustomRPC.SetModifier:
                        var player2 = Utils.PlayerById(reader.ReadByte());
                        var id2 = reader.ReadInt32();

                        switch (id2)
                        {
                            case 0:
                                new Diseased(player2);
                                break;
                            case 1:
                                new Bait(player2);
                                break;
                            case 2:
                                new Dwarf(player2);
                                break;
                            case 3:
                                new VIP(player2);
                                break;
                            case 4:
                                new Shy(player2);
                                break;
                            case 5:
                                new Giant(player2);
                                break;
                            case 6:
                                new Drunk(player2);
                                break;
                            case 7:
                                new Flincher(player2);
                                break;
                            case 8:
                                new Coward(player2);
                                break;
                            case 9:
                                new Volatile(player2);
                                break;
                            case 10:
                                new Professional(player2);
                                break;
                        }

                        break;

                    case CustomRPC.SetAbility:
                        var player3 = Utils.PlayerById(reader.ReadByte());
                        var id3 = reader.ReadInt32();

                        switch (id3)
                        {
                            case 0:
                                new Assassin(player3);
                                break;
                            case 1:
                                new Snitch(player3);
                                break;
                            case 2:
                                new Insider(player3);
                                break;
                            case 3:
                                new Lighter(player3);
                                break;
                            case 4:
                                new Multitasker(player3);
                                break;
                            case 5:
                                new Radar(player3);
                                break;
                            case 6:
                                new Tiebreaker(player3);
                                break;
                            case 7:
                                new Torch(player3);
                                break;
                            case 8:
                                new Underdog(player3);
                                break;
                            case 9:
                                new Tunneler(player3);
                                break;
                        }

                        break;

                    case CustomRPC.SetObjectifier:
                        var player4 = Utils.PlayerById(reader.ReadByte());
                        var id4 = reader.ReadInt32();

                        switch (id4)
                        {
                            case 2:
                                new Fanatic(player4);
                                break;
                            case 3:
                                new Corrupted(player4);
                                break;
                            case 4:
                                new Overlord(player4);
                                break;
                            case 5:
                                new Allied(player4);
                                break;
                            case 6:
                                new Traitor(player4);
                                break;
                            case 7:
                                new Taskmaster(player4);
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
                            case TurnRPC.TurnPestilence:
                                Role.GetRole<Plaguebearer>(Utils.PlayerById(reader.ReadByte())).TurnPestilence();
                                break;

                            case TurnRPC.TurnVigilante:
                                Role.GetRole<VampireHunter>(Utils.PlayerById(reader.ReadByte())).TurnVigilante();
                                break;

                            case TurnRPC.TurnGodfather:
                                Role.GetRole<Mafioso>(Utils.PlayerById(reader.ReadByte())).TurnGodfather();
                                break;

                            case TurnRPC.TurnRebel:
                                Role.GetRole<Sidekick>(Utils.PlayerById(reader.ReadByte())).TurnRebel();
                                break;

                            case TurnRPC.ExeToJest:
                                TargetColor.ExeToJest(Utils.PlayerById(reader.ReadByte()));
                                break;

                            case TurnRPC.GAToSurv:
                                GATargetColor.GAToSurv(Utils.PlayerById(reader.ReadByte()));
                                break;

                            case TurnRPC.GuessToAct:
                                GuessTargetColor.GuessToAct(Utils.PlayerById(reader.ReadByte()));
                                break;

                            case TurnRPC.TurnTraitor:
                                var traitorObj = Objectifier.GetObjectifier<Traitor>(Utils.PlayerById(reader.ReadByte()));

                                if (traitorObj.Turned)
                                    SetTraitor.TurnTraitor(Utils.PlayerById(reader.ReadByte()));

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

                    case CustomRPC.RevealerDied:
                        var revealer = Utils.PlayerById(reader.ReadByte());
                        var former = Role.GetRole(revealer);
                        Role.RoleDictionary.Remove(revealer.PlayerId);
                        var revealerRole = new Revealer(revealer);
                        revealerRole.RegenTask();
                        revealerRole.RoleHistory.Add(former);
                        revealerRole.RoleHistory.AddRange(former.RoleHistory);
                        revealer.gameObject.layer = LayerMask.NameToLayer("Players");
                        SetRevealer.RemoveTasks(revealer);

                        if (!PlayerControl.LocalPlayer.Is(RoleEnum.Phantom))
                            PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
                            
                        break;

                    case CustomRPC.PhantomDied:
                        var phantom = Utils.PlayerById(reader.ReadByte());
                        var phantomFormer = Role.GetRole(phantom);
                        Role.RoleDictionary.Remove(phantom.PlayerId);
                        var phantomRole = new Phantom(phantom);
                        phantomRole.RegenTask();
                        phantomRole.RoleHistory.Add(phantomFormer);
                        phantomRole.RoleHistory.AddRange(phantomFormer.RoleHistory);
                        phantom.gameObject.layer = LayerMask.NameToLayer("Players");
                        SetPhantom.RemoveTasks(phantom);

                        if (!PlayerControl.LocalPlayer.Is(RoleEnum.Revealer))
                            PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
                            
                        break;

                    case CustomRPC.CatchPhantom:
                        Role.GetRole<Phantom>(Utils.PlayerById(reader.ReadByte())).Caught = true;
                        break;

                    case CustomRPC.Start:
                        //Utils.ShowDeadBodies = false;
                        Role.NobodyWins = false;
                        Role.CrewWin = false;
                        Role.SyndicateWin = false;
                        Role.IntruderWin = false;
                        Role.AllNeutralsWin = false;
                        Role.UndeadWin = false;
                        Role.CabalWin = false;
                        Role.NKWins = false;
                        Role.SectWin = false;
                        Role.ReanimatedWin = false;
                        Role.SyndicateHasChaosDrive = false;
                        Role.ChaosDriveMeetingTimerCount = 0;
                        ExileControllerPatch.lastExiled = null;
                        RecordRewind.points.Clear();
                        Murder.KilledPlayers.Clear();
                        Role.Buttons.Clear();
                        Role.SetColors();
                        Lists.DefinedLists();
                        Alt.DontRevive = byte.MaxValue;
                        break;

                    case CustomRPC.AttemptSound:
                        var medicId = reader.ReadByte();
                        readByte = reader.ReadByte();
                        StopKill.BreakShield(medicId, readByte, CustomGameOptions.ShieldBreaks);
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
                    
                    case CustomRPC.SendChat:
                        string report = reader.ReadString();
                        DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, report);
                        break;

                    case CustomRPC.CatchRevealer:
                        Role.GetRole<Revealer>(Utils.PlayerById(reader.ReadByte())).Caught = true;
                        break;

                    case CustomRPC.AddMayorVoteBank:
                        Role.GetRole<Mayor>(Utils.PlayerById(reader.ReadByte())).VoteBank += reader.ReadInt32();
                        break;

                    case CustomRPC.RemoveAllBodies:
                        var buggedBodies = Object.FindObjectsOfType<DeadBody>();

                        foreach (var body in buggedBodies)
                            body.gameObject.Destroy();

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

                    case CustomRPC.SetAlliedFaction:
                        var player6 = Utils.PlayerById(reader.ReadByte());
                        var alliedRole = Role.GetRole(player6);
                        var faction = (Faction)reader.ReadByte();
                        alliedRole.Faction = faction;
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
                        //GameOptionsManager.Instance.currentNormalGameOptions.MaxPlayers = CustomGameOptions.LobbySize;

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
                                HighlightImpostors.UpdateMeeting(MeetingHud.Instance);
                                break;

                            case ActionsRPC.AllFreeze:
                                var theCryomaniac = Utils.PlayerById(reader.ReadByte());
                                var theCryomaniacRole = Role.GetRole<Cryomaniac>(theCryomaniac);
                                theCryomaniacRole.FreezeUsed = true;
                                break;

                            case ActionsRPC.JanitorClean:
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

                            case ActionsRPC.EngineerFix:
                                var engineer = Utils.PlayerById(reader.ReadByte());
                                Role.GetRole<Engineer>(engineer).UsedThisRound = true;
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
                                readSByte = reader.ReadSByte();
                                SwapVotes.Swap1 = MeetingHud.Instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == readSByte);
                                readSByte2 = reader.ReadSByte();
                                SwapVotes.Swap2 = MeetingHud.Instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == readSByte2);
                                PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Bytes received - " + readSByte + " - " + readSByte2);
                                break;

                            case ActionsRPC.Remember:
                                var amnesiac = Utils.PlayerById(reader.ReadByte());
                                var other = Utils.PlayerById(reader.ReadByte());
                                PerformKillButton.Remember(Role.GetRole<Amnesiac>(amnesiac), other);
                                break;

                            case ActionsRPC.Steal:
                                var thief = Utils.PlayerById(reader.ReadByte());
                                var other4 = Utils.PlayerById(reader.ReadByte());
                                PerformStealButton.Steal(Role.GetRole<Thief>(thief), other4);
                                break;

                            case ActionsRPC.Declare:
                                readByte1 = reader.ReadByte();
                                readByte2 = reader.ReadByte();
                                var gf = Utils.PlayerById(readByte1);
                                var maf = Utils.PlayerById(readByte2);
                                PerformDeclareButton.Declare(Role.GetRole<Godfather>(gf), maf);
                                break;

                            case ActionsRPC.Sidekick:
                                readByte1 = reader.ReadByte();
                                readByte2 = reader.ReadByte();
                                var reb = Utils.PlayerById(readByte1);
                                var side = Utils.PlayerById(readByte2);
                                PerformSidekickButton.Sidekick(Role.GetRole<Rebel>(reb), side);
                                break;

                            case ActionsRPC.Shift:
                                readByte1 = reader.ReadByte();
                                readByte2 = reader.ReadByte();
                                var shifter = Utils.PlayerById(readByte1);
                                var other2 = Utils.PlayerById(readByte2);
                                PerformShiftButton.Shift(Role.GetRole<Shifter>(shifter), other2);
                                break;

                            case ActionsRPC.Convert:
                                var drac = Utils.PlayerById(reader.ReadByte());
                                var other3 = Utils.PlayerById(reader.ReadByte());
                                PerformConvertButton.Convert(Role.GetRole<Dracula>(drac), other3);
                                break;

                            case ActionsRPC.Teleport:
                                var teleporter = Utils.PlayerById(reader.ReadByte());
                                var teleporterRole = Role.GetRole<Teleporter>(teleporter);
                                var teleportPos = reader.ReadVector2();
                                teleporterRole.TeleportPoint = teleportPos;
                                Teleporter.Teleport(teleporter);
                                break;

                            case ActionsRPC.Rewind:
                                readByte = reader.ReadByte();
                                var TimeLordPlayer = Utils.PlayerById(readByte);
                                var TimeLordRole = Role.GetRole<TimeLord>(TimeLordPlayer);
                                StartStop.StartRewind(TimeLordRole);
                                break;

                            case ActionsRPC.Protect:
                                readByte1 = reader.ReadByte();
                                readByte2 = reader.ReadByte();
                                var medic = Utils.PlayerById(readByte1);
                                var shield = Utils.PlayerById(readByte2);
                                Role.GetRole<Medic>(medic).ShieldedPlayer = shield;
                                Role.GetRole<Medic>(medic).UsedAbility = true;
                                break;

                            case ActionsRPC.RewindRevive:
                                readByte = reader.ReadByte();
                                RecordRewind.ReviveBody(Utils.PlayerById(readByte));
                                break;

                            case ActionsRPC.BypassKill:
                                var killer = Utils.PlayerById(reader.ReadByte());
                                var TargetPlayer = Utils.PlayerById(reader.ReadByte());
                                Utils.MurderPlayer(killer, TargetPlayer);
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

                            case ActionsRPC.Mimic:
                                var glitchPlayer = Utils.PlayerById(reader.ReadByte());
                                var mimicPlayer = Utils.PlayerById(reader.ReadByte());
                                var glitchRole = Role.GetRole<Glitch>(glitchPlayer);
                                glitchRole.TimeRemaining = CustomGameOptions.MimicDuration;
                                glitchRole.MimicTarget = mimicPlayer;
                                break;

                            case ActionsRPC.UnMimic:
                                var glitchPlayer3 = Utils.PlayerById(reader.ReadByte());
                                var glitchRole2 = Role.GetRole<Glitch>(glitchPlayer3);
                                glitchRole2.UnMimic();
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
                                break;

                            case ActionsRPC.Disguise:
                                var disguiser = Utils.PlayerById(reader.ReadByte());
                                var disguiseTarget = Utils.PlayerById(reader.ReadByte());
                                var disguiseRole = Role.GetRole<Disguiser>(disguiser);
                                disguiseRole.TimeRemaining = CustomGameOptions.DisguiseDuration;
                                disguiseRole.DisguisedPlayer = disguiseTarget;
                                disguiseRole.LastDisguised = DateTime.UtcNow;
                                break;

                            case ActionsRPC.Poison:
                                var poisoner = Utils.PlayerById(reader.ReadByte());
                                var poisoned = Utils.PlayerById(reader.ReadByte());
                                var poisonerRole = Role.GetRole<Poisoner>(poisoner);
                                poisonerRole.PoisonedPlayer = poisoned;
                                break;

                            case ActionsRPC.Blackmail:
                                var blackmailer = Role.GetRole<Blackmailer>(Utils.PlayerById(reader.ReadByte()));
                                blackmailer.Blackmailed = Utils.PlayerById(reader.ReadByte());
                                break;

                            case ActionsRPC.Mine:
                                var ventId = reader.ReadInt32();
                                var miner = Utils.PlayerById(reader.ReadByte());
                                var minerRole = Role.GetRole<Miner>(miner);
                                var pos = reader.ReadVector2();
                                var zAxis = reader.ReadSingle();
                                PlayerLayers.Roles.IntruderRoles.MinerMod.PerformKill.SpawnVent(ventId, minerRole, pos, zAxis);
                                break;

                            case ActionsRPC.TimeFreeze:
                                var tm = Utils.PlayerById(reader.ReadByte());
                                var tmRole = Role.GetRole<TimeMaster>(tm);
                                tmRole.TimeRemaining = CustomGameOptions.FreezeDuration;
                                tmRole.TimeFreeze();
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

                            case ActionsRPC.GAProtect:
                                var ga2 = Utils.PlayerById(reader.ReadByte());
                                var ga2Role = Role.GetRole<GuardianAngel>(ga2);
                                ga2Role.TimeRemaining = CustomGameOptions.ProtectDuration;
                                ga2Role.Protect();
                                break;

                            case ActionsRPC.Transport:
                                Coroutines.Start(Transporter.TransportPlayers(reader.ReadByte(), reader.ReadByte(), reader.ReadBoolean()));
                                break;

                            case ActionsRPC.SetUntransportable:
                                if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
                                    Role.GetRole<Transporter>(PlayerControl.LocalPlayer).UntransportablePlayers.Add(reader.ReadByte(), DateTime.UtcNow);

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
                                var deads = Object.FindObjectsOfType<DeadBody>();

                                foreach (var body in deads)
                                {
                                    if (body.ParentId == readByte)
                                        Coroutines.Start(Eat.EatCoroutine(body, cannibalRole));
                                }

                                cannibalRole.LastEaten = DateTime.UtcNow;
                                break;

                            case ActionsRPC.Infect:
                                Role.GetRole<Plaguebearer>(Utils.PlayerById(reader.ReadByte())).InfectedPlayers.Add(reader.ReadByte());
                                break;

                            case ActionsRPC.AltruistRevive:
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

                            case ActionsRPC.Warp:
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

                                    DestroyableSingleton<HudManager>.Instance.OpenMeetingRoom(buttonBarry);
                                    buttonBarry.RpcStartMeeting(null);
                                }

                                break;

                            case ActionsRPC.BaitReport:
                                var baitKiller = Utils.PlayerById(reader.ReadByte());
                                var bait = Utils.PlayerById(reader.ReadByte());
                                baitKiller.ReportDeadBody(bait.Data);
                                break;

                            case ActionsRPC.Drag:
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
                                camouflagerRole.TimeRemaining = CustomGameOptions.CamouflagerDuration;
                                Utils.Camouflage();
                                break;

                            case ActionsRPC.EscRoleblock:
                                var targetPlayer = Utils.PlayerById(reader.ReadByte());

                                if (targetPlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                                {
                                    var escort = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Escort && ((Escort)x).BlockTarget == targetPlayer);
                                    ((Escort)escort)?.SetBlocked(targetPlayer);
                                }

                                break;

                            case ActionsRPC.ConsRoleblock:
                                var targetPlayer2 = Utils.PlayerById(reader.ReadByte());

                                if (targetPlayer2.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                                {
                                    var consort = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Consort && ((Consort)x).BlockTarget == targetPlayer2);
                                    ((Consort)consort)?.SetBlocked(targetPlayer2);
                                }

                                break;

                            case ActionsRPC.GlitchRoleblock:
                                var targetPlayer3 = Utils.PlayerById(reader.ReadByte());

                                if (targetPlayer3.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                                {
                                    var glitch = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Glitch && ((Glitch)x).HackTarget == targetPlayer3);
                                    ((Glitch)glitch)?.SetHacked(targetPlayer3);
                                }

                                break;

                            case ActionsRPC.Conceal:
                                var concealer = Utils.PlayerById(reader.ReadByte());
                                var concealerRole = Role.GetRole<Concealer>(concealer);
                                concealerRole.TimeRemaining = CustomGameOptions.ConcealDuration;
                                Utils.Conceal();
                                break;

                            case ActionsRPC.Shapeshift:
                                var ss = Utils.PlayerById(reader.ReadByte());
                                var ssRole = Role.GetRole<Concealer>(ss);
                                ssRole.TimeRemaining = CustomGameOptions.ConcealDuration;
                                Utils.Shapeshift();
                                break;

                            case ActionsRPC.Gaze:
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
                        }

                        break;

                    case CustomRPC.SyncCustomSettings:
                        Rpc.ReceiveRpc(reader);
                        break;
                    
                    case CustomRPC.WinLose:
                        var id7 = reader.ReadByte();

                        switch ((WinLoseRPC)id7)
                        {
                            case WinLoseRPC.CrewWin:
                                var crew = Utils.PlayerById(reader.ReadByte());
                                var crewRole = Role.GetRole(crew);
                                crewRole?.Wins();
                                Role.CrewWin = true;
                                break;
                            
                            case WinLoseRPC.CrewLose:
                                var crew2 = Utils.PlayerById(reader.ReadByte());
                                var crewRole2 = Role.GetRole(crew2);
                                crewRole2?.Loses();
                                Role.CrewWin = false;
                                break;
                            
                            case WinLoseRPC.IntruderWin:
                                var imp = Utils.PlayerById(reader.ReadByte());
                                var impRole = Role.GetRole(imp);
                                impRole?.Wins();
                                Role.IntruderWin = true;
                                break;
                            
                            case WinLoseRPC.IntruderLose:
                                var imp2 = Utils.PlayerById(reader.ReadByte());
                                var impRole2 = Role.GetRole(imp2);
                                impRole2?.Loses();
                                Role.IntruderWin = false;
                                break;
                            
                            case WinLoseRPC.SyndicateWin:
                                var syn = Utils.PlayerById(reader.ReadByte());
                                var synRole = Role.GetRole(syn);
                                synRole?.Wins();
                                Role.SyndicateWin = true;
                                break;
                            
                            case WinLoseRPC.SyndicateLose:
                                var syn2 = Utils.PlayerById(reader.ReadByte());
                                var synRole2 = Role.GetRole(syn2);
                                synRole2?.Loses();
                                Role.SyndicateWin = false;
                                break;
                            
                            case WinLoseRPC.UndeadWin:
                                var und = Utils.PlayerById(reader.ReadByte());
                                var undRole = Role.GetRole(und);
                                undRole?.Wins();
                                Role.UndeadWin = true;
                                break;
                            
                            case WinLoseRPC.UndeadLose:
                                var und2 = Utils.PlayerById(reader.ReadByte());
                                var undRole2 = Role.GetRole(und2);
                                undRole2?.Loses();
                                Role.UndeadWin = false;
                                break;
                            
                            case WinLoseRPC.ReanimatedWin:
                                var rean = Utils.PlayerById(reader.ReadByte());
                                var reanRole = Role.GetRole(rean);
                                reanRole?.Wins();
                                Role.ReanimatedWin = true;
                                break;
                            
                            case WinLoseRPC.ReanimatedLose:
                                var rean2 = Utils.PlayerById(reader.ReadByte());
                                var reanRole2 = Role.GetRole(rean2);
                                reanRole2?.Loses();
                                Role.ReanimatedWin = false;
                                break;
                            
                            case WinLoseRPC.SectWin:
                                var sect = Utils.PlayerById(reader.ReadByte());
                                var sectRole = Role.GetRole(sect);
                                sectRole?.Wins();
                                Role.SectWin = true;
                                break;
                            
                            case WinLoseRPC.SectLose:
                                var sect2 = Utils.PlayerById(reader.ReadByte());
                                var sectRole2 = Role.GetRole(sect2);
                                sectRole2?.Loses();
                                Role.SectWin = false;
                                break;
                            
                            case WinLoseRPC.CabalWin:
                                var cab = Utils.PlayerById(reader.ReadByte());
                                var cabRole = Role.GetRole(cab);
                                cabRole?.Wins();
                                Role.CabalWin = true;
                                break;
                            
                            case WinLoseRPC.CabalLose:
                                var cab2 = Utils.PlayerById(reader.ReadByte());
                                var cabRole2 = Role.GetRole(cab2);
                                cabRole2?.Loses();
                                Role.CabalWin = false;
                                break;
                            
                            case WinLoseRPC.JesterWin:
                                var jest = Utils.PlayerById(reader.ReadByte());
                                var jestRole = Role.GetRole<Jester>(jest);
                                jestRole?.Wins();
                                break;
                            
                            case WinLoseRPC.JesterLose:
                                var jest2 = Utils.PlayerById(reader.ReadByte());
                                var jestRole2 = Role.GetRole<Jester>(jest2);
                                jestRole2?.Loses();
                                break;
                            
                            case WinLoseRPC.AmnesiacLose:
                                var amne = Utils.PlayerById(reader.ReadByte());
                                var amneRole = Role.GetRole<Amnesiac>(amne);
                                amneRole?.Loses();
                                break;
                            
                            case WinLoseRPC.ThiefLose:
                                var thief2 = Utils.PlayerById(reader.ReadByte());
                                var thiefRole2 = Role.GetRole<Thief>(thief2);
                                thiefRole2?.Loses();
                                break;
                            
                            case WinLoseRPC.ArsonistWin:
                                var arso = Utils.PlayerById(reader.ReadByte());
                                var arsoRole = Role.GetRole<Arsonist>(arso);
                                arsoRole?.Wins();
                                break;
                            
                            case WinLoseRPC.ArsonistLose:
                                var arso2 = Utils.PlayerById(reader.ReadByte());
                                var arsoRole2 = Role.GetRole<Arsonist>(arso2);
                                arsoRole2?.Loses();
                                break;
                            
                            case WinLoseRPC.CannibalWin:
                                var cann = Utils.PlayerById(reader.ReadByte());
                                var cannRole = Role.GetRole<Cannibal>(cann);
                                cannRole?.Wins();
                                break;
                            
                            case WinLoseRPC.CannibalLose:
                                var cann2 = Utils.PlayerById(reader.ReadByte());
                                var cannRole2 = Role.GetRole<Cannibal>(cann2);
                                cannRole2?.Loses();
                                break;
                            
                            case WinLoseRPC.CryomaniacWin:
                                var cryo = Utils.PlayerById(reader.ReadByte());
                                var cryoRole = Role.GetRole<Cryomaniac>(cryo);
                                cryoRole?.Wins();
                                break;
                            
                            case WinLoseRPC.CryomaniacLose:
                                var cryo2 = Utils.PlayerById(reader.ReadByte());
                                var cryoRole2 = Role.GetRole<Cryomaniac>(cryo2);
                                cryoRole2?.Loses();
                                break;
                            
                            case WinLoseRPC.ExecutionerWin:
                                var exe2 = Utils.PlayerById(reader.ReadByte());
                                var exeRole2 = Role.GetRole<Executioner>(exe2);
                                exeRole2?.Wins();
                                break;
                            
                            case WinLoseRPC.ExecutionerLose:
                                var exe3 = Utils.PlayerById(reader.ReadByte());
                                var exeRole3 = Role.GetRole<Executioner>(exe3);
                                exeRole3?.Loses();
                                break;
                            
                            case WinLoseRPC.GlitchWin:
                                var gli = Utils.PlayerById(reader.ReadByte());
                                var gliRole = Role.GetRole<Glitch>(gli);
                                gliRole?.Wins();
                                break;
                            
                            case WinLoseRPC.GlitchLose:
                                var gli2 = Utils.PlayerById(reader.ReadByte());
                                var gliRole2 = Role.GetRole<Glitch>(gli2);
                                gliRole2?.Loses();
                                break;
                            
                            case WinLoseRPC.GuardianAngelWin:
                                var ga3 = Utils.PlayerById(reader.ReadByte());
                                var gaRole3 = Role.GetRole<GuardianAngel>(ga3);
                                gaRole3?.Wins();
                                break;
                            
                            case WinLoseRPC.GuardianAngelLose:
                                var ga4 = Utils.PlayerById(reader.ReadByte());
                                var gaRole4 = Role.GetRole<GuardianAngel>(ga4);
                                gaRole4?.Loses();
                                break;
                            
                            case WinLoseRPC.JuggernautWin:
                                var jugg = Utils.PlayerById(reader.ReadByte());
                                var juggRole = Role.GetRole<Juggernaut>(jugg);
                                juggRole?.Wins();
                                break;
                            
                            case WinLoseRPC.JuggernautLose:
                                var jugg2 = Utils.PlayerById(reader.ReadByte());
                                var juggRole2 = Role.GetRole<Juggernaut>(jugg2);
                                juggRole2?.Loses();
                                break;
                            
                            case WinLoseRPC.MurdererWin:
                                var murd = Utils.PlayerById(reader.ReadByte());
                                var murdRole = Role.GetRole<Murderer>(murd);
                                murdRole?.Wins();
                                break;
                            
                            case WinLoseRPC.MurdererLose:
                                var murd2 = Utils.PlayerById(reader.ReadByte());
                                var murdRole2 = Role.GetRole<Murderer>(murd2);
                                murdRole2?.Loses();
                                break;
                            
                            case WinLoseRPC.PestilenceWin:
                                var pest = Utils.PlayerById(reader.ReadByte());
                                var pestRole = Role.GetRole<Pestilence>(pest);
                                pestRole?.Wins();
                                break;
                            
                            case WinLoseRPC.PestilenceLose:
                                var pest2 = Utils.PlayerById(reader.ReadByte());
                                var pestRole2 = Role.GetRole<Pestilence>(pest2);
                                pestRole2?.Loses();
                                break;
                            
                            case WinLoseRPC.PlaguebearerWin:
                                var pb = Utils.PlayerById(reader.ReadByte());
                                var pbRole = Role.GetRole<Plaguebearer>(pb);
                                pbRole?.Wins();
                                break;
                            
                            case WinLoseRPC.PlaguebearerLose:
                                var pb2 = Utils.PlayerById(reader.ReadByte());
                                var pbRole2 = Role.GetRole<Plaguebearer>(pb2);
                                pbRole2?.Loses();
                                break;
                            
                            case WinLoseRPC.SerialKillerWin:
                                var sk = Utils.PlayerById(reader.ReadByte());
                                var skRole = Role.GetRole<SerialKiller>(sk);
                                skRole?.Wins();
                                break;
                            
                            case WinLoseRPC.SerialKillerLose:
                                var sk2 = Utils.PlayerById(reader.ReadByte());
                                var skRole2 = Role.GetRole<SerialKiller>(sk2);
                                skRole2?.Loses();
                                break;
                            
                            case WinLoseRPC.SurvivorWin:
                                var surv2 = Utils.PlayerById(reader.ReadByte());
                                var survRole2 = Role.GetRole<Survivor>(surv2);
                                survRole2?.Wins();
                                break;
                            
                            case WinLoseRPC.SurvivorLose:
                                var surv3 = Utils.PlayerById(reader.ReadByte());
                                var survRole3 = Role.GetRole<Survivor>(surv3);
                                survRole3?.Loses();
                                break;
                            
                            case WinLoseRPC.TrollWin:
                                var tro = Utils.PlayerById(reader.ReadByte());
                                var troRole = Role.GetRole<Troll>(tro);
                                troRole?.Wins();
                                break;
                            
                            case WinLoseRPC.TrollLose:
                                var tro2 = Utils.PlayerById(reader.ReadByte());
                                var troRole2 = Role.GetRole<Troll>(tro2);
                                troRole2?.Loses();
                                break;
                            
                            case WinLoseRPC.GuesserWin:
                                var guess2 = Utils.PlayerById(reader.ReadByte());
                                var guessRole2 = Role.GetRole<Guesser>(guess2);
                                guessRole2?.Wins();
                                break;
                            
                            case WinLoseRPC.GuesserLose:
                                var guess3 = Utils.PlayerById(reader.ReadByte());
                                var guessRole3 = Role.GetRole<Guesser>(guess3);
                                guessRole3?.Loses();
                                break;
                            
                            case WinLoseRPC.WerewolfWin:
                                var ww2 = Utils.PlayerById(reader.ReadByte());
                                var wwRole2 = Role.GetRole<Werewolf>(ww2);
                                wwRole2?.Wins();
                                break;
                            
                            case WinLoseRPC.WerewolfLose:
                                var ww3 = Utils.PlayerById(reader.ReadByte());
                                var wwRole3 = Role.GetRole<Werewolf>(ww3);
                                wwRole3?.Loses();
                                break;
                            
                            case WinLoseRPC.CorruptedWin:
                                var corr = Utils.PlayerById(reader.ReadByte());
                                var corrObj = Objectifier.GetObjectifier<Corrupted>(corr);
                                var corrRole = Role.GetRole(corr);
                                corrRole?.Wins();
                                corrObj?.Wins();
                                break;
                            
                            case WinLoseRPC.CorruptedLose:
                                var corr2 = Utils.PlayerById(reader.ReadByte());
                                var corrObj2 = Objectifier.GetObjectifier<Corrupted>(corr2);
                                var corrRole2 = Role.GetRole(corr2);
                                corrRole2?.Loses();
                                corrObj2?.Loses();
                                break;

                            case WinLoseRPC.LoveWin:
                                var winnerlover = Utils.PlayerById(reader.ReadByte());
                                Objectifier.GetObjectifier<Lovers>(winnerlover)?.Wins();
                                break;

                            case WinLoseRPC.OverlordWin:
                                var winnerov = Utils.PlayerById(reader.ReadByte());
                                Objectifier.GetObjectifier<Overlord>(winnerov)?.Wins();
                                break;

                            case WinLoseRPC.TaskmasterWin:
                                var winnertask = Utils.PlayerById(reader.ReadByte());
                                Objectifier.GetObjectifier<Taskmaster>(winnertask)?.Wins();
                                break;

                            case WinLoseRPC.RivalWin:
                                var winnerRival = Utils.PlayerById(reader.ReadByte());
                                Objectifier.GetObjectifier<Rivals>(winnerRival)?.Wins();
                                break;

                            case WinLoseRPC.PhantomWin:
                                Role.GetRole<Phantom>(Utils.PlayerById(reader.ReadByte())).CompletedTasks = true;
                                break;

                            case WinLoseRPC.NobodyWins:
                                Role.NobodyWins = true;
                                break;

                            case WinLoseRPC.AllNeutralsWin:
                                Role.AllNeutralsWin = true;
                                break;

                            case WinLoseRPC.AllNKsWin:
                                Role.NKWins = true;
                                break;
                        }

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

                //Utils.ShowDeadBodies = false;

                Role.NobodyWins = false;

                Role.CrewWin = false;
                Role.SyndicateWin = false;
                Role.IntruderWin = false;
                Role.AllNeutralsWin = false;
                        
                Role.UndeadWin = false;
                Role.CabalWin = false;
                Role.SectWin = false;
                Role.ReanimatedWin = false;

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
                SnitchAbilityGet.Clear();
                NonVentingAbilityGet.Clear();
                EvilAbilityGet.Clear();
                SyndicateAbilityGet.Clear();
                NeutralAbilityGet.Clear();

                CrewObjectifierGet.Clear();
                NeutralObjectifierGet.Clear();
                CorruptedObjectifierGet.Clear();
                OverlordObjectifierGet.Clear();
                GlobalObjectifierGet.Clear();
                LoverRivalObjectifierGet.Clear();

                RecordRewind.points.Clear();
                Murder.KilledPlayers.Clear();

                Role.Buttons.Clear();
                Role.SetColors();

                Lists.DefinedLists();

                PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Cleared Lists");
                
                ExileControllerPatch.lastExiled = null;

                Alt.DontRevive = byte.MaxValue;

                PhantomOn = false;
                RevealerOn = false;

                PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Cleared Functions");

                var startWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Start, SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(startWriter);
                
                if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek)
                    return;
                
                int num = 0;

                if (!IsKilling)
                {
                    PhantomOn = Check(CustomGameOptions.PhantomOn);
                    RevealerOn = Check(CustomGameOptions.RevealerOn);

                    #region Crew Roles
                    if (CustomGameOptions.MayorOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.MayorCount : 1;

                        while (num > 0)
                        {
                            CrewSovereignRoles.Add((typeof(Mayor), CustomGameOptions.MayorOn, 0, CustomGameOptions.UniqueMayor));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Mayor Done");
                    }

                    if (CustomGameOptions.SheriffOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.SheriffCount : 1;

                        while (num > 0)
                        {
                            CrewInvestigativeRoles.Add((typeof(Sheriff), CustomGameOptions.SheriffOn, 1, CustomGameOptions.UniqueSheriff));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Sheriff Done");
                    }

                    if (CustomGameOptions.InspectorOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.InspectorCount : 1;

                        while (num > 0)
                        {
                            CrewInvestigativeRoles.Add((typeof(Inspector), CustomGameOptions.InspectorOn, 2, CustomGameOptions.UniqueInspector));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Inspector Done");
                    }

                    if (CustomGameOptions.VigilanteOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.VigilanteCount : 1;

                        while (num > 0)
                        {
                            CrewKillingRoles.Add((typeof(Vigilante), CustomGameOptions.VigilanteOn, 3, CustomGameOptions.UniqueVigilante));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Vigilante Done");
                    }

                    if (CustomGameOptions.EngineerOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.EngineerCount : 1;

                        while (num > 0)
                        {
                            CrewSupportRoles.Add((typeof(Engineer), CustomGameOptions.EngineerOn, 4, CustomGameOptions.UniqueEngineer));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Engineer Done");
                    }

                    if (CustomGameOptions.SwapperOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.SwapperCount : 1;

                        while (num > 0)
                        {
                            CrewSovereignRoles.Add((typeof(Swapper), CustomGameOptions.SwapperOn, 5, CustomGameOptions.UniqueSwapper));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Swapper Done");
                    }

                    if (CustomGameOptions.InvestigatorOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.InvestigatorCount : 1;

                        while (num > 0)
                        {
                            CrewInvestigativeRoles.Add((typeof(Investigator), CustomGameOptions.InvestigatorOn, 6, CustomGameOptions.UniqueInvestigator));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Investigator Done");
                    }

                    if (CustomGameOptions.TimeLordOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.TimeLordCount : 1;

                        while (num > 0)
                        {
                            CrewProtectiveRoles.Add((typeof(TimeLord), CustomGameOptions.TimeLordOn, 7, CustomGameOptions.UniqueTimeLord));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Time Lord Done");
                    }

                    if (CustomGameOptions.MedicOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.MedicCount : 1;

                        while (num > 0)
                        {
                            CrewProtectiveRoles.Add((typeof(Medic), CustomGameOptions.MedicOn, 8, CustomGameOptions.UniqueMedic));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Medic Done");
                    }

                    if (CustomGameOptions.AgentOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.AgentCount : 1;

                        while (num > 0)
                        {
                            CrewInvestigativeRoles.Add((typeof(Agent), CustomGameOptions.AgentOn, 9, CustomGameOptions.UniqueAgent));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Agent Done");
                    }

                    if (CustomGameOptions.AltruistOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.AltruistCount : 1;

                        while (num > 0)
                        {
                            CrewProtectiveRoles.Add((typeof(Altruist), CustomGameOptions.AltruistOn, 10, CustomGameOptions.UniqueAltruist));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Altruist Done");
                    }

                    if (CustomGameOptions.VeteranOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.VeteranCount : 1;

                        while (num > 0)
                        {
                            CrewKillingRoles.Add((typeof(Veteran), CustomGameOptions.VeteranOn, 11, CustomGameOptions.UniqueVeteran));
                            num--;
                        } 

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Veteran Done");
                    }

                    if (CustomGameOptions.TrackerOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.TrackerCount : 1;

                        while (num > 0) 
                        {
                            CrewInvestigativeRoles.Add((typeof(Tracker), CustomGameOptions.TrackerOn, 12, CustomGameOptions.UniqueTracker));
                            num--;
                        } 

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Tracker Done");
                    }

                    if (CustomGameOptions.TransporterOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.TransporterCount : 1;

                        while (num > 0)
                        {
                            CrewSupportRoles.Add((typeof(Transporter), CustomGameOptions.TransporterOn, 13, CustomGameOptions.UniqueTransporter));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Transporter Done");
                    }

                    if (CustomGameOptions.MediumOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.MediumCount : 1;

                        while (num > 0)
                        {
                            CrewInvestigativeRoles.Add((typeof(Medium), CustomGameOptions.MediumOn, 14, CustomGameOptions.UniqueMedium));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Medium Done");
                    }

                    if (CustomGameOptions.CoronerOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.CoronerCount : 1;

                        while (num > 0)
                        {
                            CrewInvestigativeRoles.Add((typeof(Coroner), CustomGameOptions.CoronerOn, 15, CustomGameOptions.UniqueCoroner));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Coroner Done");
                    }

                    if (CustomGameOptions.OperativeOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.OperativeCount : 1;

                        while (num > 0)
                        {
                            CrewInvestigativeRoles.Add((typeof(Operative), CustomGameOptions.OperativeOn, 16, CustomGameOptions.UniqueOperative));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Operative Done");
                    }

                    if (CustomGameOptions.DetectiveOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.DetectiveCount : 1;

                        while (num > 0)
                        {
                            CrewInvestigativeRoles.Add((typeof(Detective), CustomGameOptions.DetectiveOn, 17, CustomGameOptions.UniqueDetective));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Detective Done");
                    }

                    if (CustomGameOptions.EscortOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.EscortCount : 1;

                        while (num > 0)
                        {
                            CrewSupportRoles.Add((typeof(Escort), CustomGameOptions.EscortOn, 18, CustomGameOptions.UniqueEscort));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Escort Done");
                    }

                    if (CustomGameOptions.ShifterOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.ShifterCount : 1;

                        while (num > 0)
                        {
                            CrewSupportRoles.Add((typeof(Shifter), CustomGameOptions.ShifterOn, 19, CustomGameOptions.UniqueShifter));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Shifter Done");
                    }

                    if (CustomGameOptions.ChameleonOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.ChameleonCount : 1;

                        while (num > 0)
                        {
                            CrewSupportRoles.Add((typeof(Chameleon), CustomGameOptions.ChameleonOn, 65, CustomGameOptions.UniqueChameleon));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Chameleon Done");
                    }

                    if (CustomGameOptions.CrewmateOn > 0 && IsCustom)
                    {
                        num = CustomGameOptions.CrewCount;

                        while (num > 0)
                        {
                            CrewRoles.Add((typeof(Crewmate), CustomGameOptions.CrewmateOn, 20, false));
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
                            CrewAuditorRoles.Add((typeof(VampireHunter), CustomGameOptions.VampireHunterOn, 21, CustomGameOptions.UniqueVampireHunter));
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
                            NeutralEvilRoles.Add((typeof(Jester), CustomGameOptions.JesterOn, 22, CustomGameOptions.UniqueJester));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Jester Done");
                    }

                    if (CustomGameOptions.AmnesiacOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.AmnesiacCount : 1;

                        while (num > 0)
                        {
                            NeutralBenignRoles.Add((typeof(Amnesiac), CustomGameOptions.AmnesiacOn, 23, CustomGameOptions.UniqueAmnesiac));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Amnesiac Done");
                    }

                    if (CustomGameOptions.ExecutionerOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.ExecutionerCount : 1;

                        while (num > 0)
                        {
                            NeutralEvilRoles.Add((typeof(Executioner), CustomGameOptions.ExecutionerOn, 24, CustomGameOptions.UniqueExecutioner));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Executioner Done");
                    }

                    if (CustomGameOptions.SurvivorOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.SurvivorCount : 1;

                        while (num > 0)
                        {
                            NeutralBenignRoles.Add((typeof(Survivor), CustomGameOptions.SurvivorOn, 25, CustomGameOptions.UniqueSurvivor));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Survivor Done");
                    }

                    if (CustomGameOptions.GuardianAngelOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.GuardianAngelCount : 1;

                        while (num > 0)
                        {
                            NeutralBenignRoles.Add((typeof(GuardianAngel), CustomGameOptions.GuardianAngelOn, 26, CustomGameOptions.UniqueGuardianAngel));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Guardian Angel Done");
                    }

                    if (CustomGameOptions.GlitchOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.GlitchCount : 1;

                        while (num > 0)
                        {
                            NeutralKillingRoles.Add((typeof(Glitch), CustomGameOptions.GlitchOn, 27, CustomGameOptions.UniqueGlitch));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Glitch Done");
                    }

                    if (CustomGameOptions.MurdererOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.MurdCount : 1;

                        while (num > 0)
                        {
                            NeutralKillingRoles.Add((typeof(Murderer), CustomGameOptions.MurdererOn, 28, CustomGameOptions.UniqueMurderer));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Murderer Done");
                    }

                    if (CustomGameOptions.CryomaniacOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.CryomaniacCount : 1;

                        while (num > 0)
                        {
                            NeutralKillingRoles.Add((typeof(Cryomaniac), CustomGameOptions.CryomaniacOn, 29, CustomGameOptions.UniqueCryomaniac));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Cryomaniac Done");
                    }

                    if (CustomGameOptions.WerewolfOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.WerewolfCount : 1;

                        while (num > 0)
                        {
                            NeutralKillingRoles.Add((typeof(Werewolf), CustomGameOptions.WerewolfOn, 30, CustomGameOptions.UniqueWerewolf));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Werewolf Done");
                    }

                    if (CustomGameOptions.ArsonistOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.ArsonistCount : 1;

                        while (num > 0)
                        {
                            NeutralKillingRoles.Add((typeof(Arsonist), CustomGameOptions.ArsonistOn, 31, CustomGameOptions.UniqueArsonist));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Arsonist Done");
                    }

                    if (CustomGameOptions.JackalOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.JackalCount : 1;

                        while (num > 0)
                        {
                            NeutralNeophyteRoles.Add((typeof(Jackal), CustomGameOptions.JackalOn, 32, CustomGameOptions.UniqueJackal));
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
                                NeutralKillingRoles.Add((typeof(Pestilence), CustomGameOptions.PlaguebearerOn, 33, CustomGameOptions.UniquePlaguebearer));
                            else
                                NeutralKillingRoles.Add((typeof(Plaguebearer), CustomGameOptions.PlaguebearerOn, 34, CustomGameOptions.UniquePlaguebearer));

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
                            NeutralKillingRoles.Add((typeof(SerialKiller), CustomGameOptions.SerialKillerOn, 35, CustomGameOptions.UniqueSerialKiller));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Serial Killer Done");
                    }

                    if (CustomGameOptions.JuggernautOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.JuggernautCount : 1;

                        while (num > 0)
                        {
                            NeutralKillingRoles.Add((typeof(Juggernaut), CustomGameOptions.JuggernautOn, 36, CustomGameOptions.UniqueJuggernaut));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Juggeraut Done");
                    }

                    if (CustomGameOptions.CannibalOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.CannibalCount : 1;

                        while (num > 0)
                        {
                            NeutralEvilRoles.Add((typeof(Cannibal), CustomGameOptions.CannibalOn, 37, CustomGameOptions.UniqueCannibal));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Cannibal Done");
                    }

                    if (CustomGameOptions.GuesserOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.GuesserCount : 1;

                        while (num > 0)
                        {
                            NeutralEvilRoles.Add((typeof(Guesser), CustomGameOptions.GuesserOn, 66, CustomGameOptions.UniqueGuesser));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Guesser Done");
                    }

                    if (CustomGameOptions.ThiefOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.ThiefCount : 1;

                        while (num > 0)
                        {
                            NeutralBenignRoles.Add((typeof(Thief), CustomGameOptions.ThiefOn, 38, CustomGameOptions.UniqueThief));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Thief Done");
                    }

                    if (CustomGameOptions.DraculaOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.DraculaCount : 1;

                        while (num > 0)
                        {
                            NeutralNeophyteRoles.Add((typeof(Dracula), CustomGameOptions.DraculaOn, 39, CustomGameOptions.UniqueDracula));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Dracula Done");
                    }

                    if (CustomGameOptions.WhispererOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.WhispererCount : 1;

                        while (num > 0)
                        {
                            NeutralNeophyteRoles.Add((typeof(Whisperer), CustomGameOptions.WhispererOn, 67, CustomGameOptions.UniqueWhisperer));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Whisperer Done");
                    }

                    if (CustomGameOptions.TrollOn > 0 && CustomGameOptions.GameMode != GameMode.Classic)
                    {
                        num = IsCustom ? CustomGameOptions.TrollCount : 1;

                        while (num > 0)
                        {
                            NeutralEvilRoles.Add((typeof(Troll), CustomGameOptions.TrollOn, 40, CustomGameOptions.UniqueTroll));
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
                                IntruderConcealingRoles.Add((typeof(Undertaker), CustomGameOptions.UndertakerOn, 41, CustomGameOptions.UniqueUndertaker));
                                num--;
                            }

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Undertaker Done");
                        }

                        if (CustomGameOptions.MorphlingOn > 0)
                        {
                            num = IsCustom ? CustomGameOptions.MorphlingCount : 1;

                            while (num > 0)
                            {
                                IntruderDeceptionRoles.Add((typeof(Morphling), CustomGameOptions.MorphlingOn, 42, CustomGameOptions.UniqueMorphling));
                                num--;
                            }

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Morphling Done");
                        }

                        if (CustomGameOptions.BlackmailerOn > 0)
                        {
                            num = IsCustom ? CustomGameOptions.BlackmailerCount : 1;

                            while (num > 0)
                            {
                                IntruderConcealingRoles.Add((typeof(Blackmailer), CustomGameOptions.BlackmailerOn, 43, CustomGameOptions.UniqueBlackmailer));
                                num--;
                            }

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Blackmailer Done");
                        }

                        if (CustomGameOptions.MinerOn > 0)
                        {
                            num = IsCustom ? CustomGameOptions.MinerCount : 1;

                            while (num > 0)
                            {
                                IntruderSupportRoles.Add((typeof(Miner), CustomGameOptions.MinerOn, 44, CustomGameOptions.UniqueMiner));
                                num--;
                            }

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Miner Done");
                        }

                        if (CustomGameOptions.TeleporterOn > 0)
                        {
                            num = IsCustom ? CustomGameOptions.TeleporterCount : 1;

                            while (num > 0)
                            {
                                IntruderSupportRoles.Add((typeof(Teleporter), CustomGameOptions.TeleporterOn, 45, CustomGameOptions.UniqueTeleporter));
                                num--;
                            }

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Teleporter Done");
                        }

                        if (CustomGameOptions.WraithOn > 0)
                        {
                            num = IsCustom ? CustomGameOptions.WraithCount : 1;

                            while (num > 0)
                            {
                                IntruderDeceptionRoles.Add((typeof(Wraith), CustomGameOptions.WraithOn, 46, CustomGameOptions.UniqueWraith));
                                num--;
                            }

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Wraith Done");
                        }

                        if (CustomGameOptions.ConsortOn > 0)
                        {
                            num = IsCustom ? CustomGameOptions.ConsortCount : 1;

                            while (num > 0)
                            {
                                IntruderSupportRoles.Add((typeof(Consort), CustomGameOptions.ConsortOn, 47, CustomGameOptions.UniqueConsort));
                                num--;
                            }

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Consort Done");
                        }

                        if (CustomGameOptions.JanitorOn > 0)
                        {
                            num = IsCustom ? CustomGameOptions.JanitorCount : 1;

                            while (num > 0)
                            {
                                IntruderConcealingRoles.Add((typeof(Janitor), CustomGameOptions.JanitorOn, 48, CustomGameOptions.UniqueJanitor));
                                num--;
                            }

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Janitor Done");
                        }

                        if (CustomGameOptions.CamouflagerOn > 0)
                        {
                            num = IsCustom ? CustomGameOptions.CamouflagerCount : 1;

                            while (num > 0)
                            {
                                IntruderConcealingRoles.Add((typeof(Camouflager), CustomGameOptions.CamouflagerOn, 49, CustomGameOptions.UniqueCamouflager));
                                num--;
                            }

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Camouflager Done");
                        }

                        if (CustomGameOptions.GrenadierOn > 0)
                        {
                            num = IsCustom ? CustomGameOptions.GrenadierCount : 1;

                            while (num > 0)
                            {
                                IntruderConcealingRoles.Add((typeof(Grenadier), CustomGameOptions.GrenadierOn, 50, CustomGameOptions.UniqueGrenadier));
                                num--;
                            }

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Grenadier Done");
                        }

                        if (CustomGameOptions.ImpostorOn > 0 && IsCustom)
                        {
                            num = CustomGameOptions.ImpCount;

                            while (num > 0)
                            {
                                IntruderRoles.Add((typeof(Impostor), CustomGameOptions.ImpostorOn, 52, false));
                                num--;
                            }

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Impostor Done");
                        }

                        if (CustomGameOptions.ConsigliereOn > 0)
                        {
                            num = IsCustom ? CustomGameOptions.ConsigliereCount : 1;

                            while (num > 0)
                            {
                                IntruderSupportRoles.Add((typeof(Consigliere), CustomGameOptions.ConsigliereOn, 53, CustomGameOptions.UniqueConsigliere));
                                num--;
                            }

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Consigliere Done");
                        }

                        if (CustomGameOptions.DisguiserOn > 0)
                        {
                            num = IsCustom ? CustomGameOptions.DisguiserCount : 1;

                            while (num > 0)
                            {
                                IntruderDeceptionRoles.Add((typeof(Disguiser), CustomGameOptions.DisguiserOn, 54, CustomGameOptions.UniqueDisguiser));
                                num--;
                            }

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Disguiser Done");
                        }

                        if (CustomGameOptions.TimeMasterOn > 0)
                        {
                            num = IsCustom ? CustomGameOptions.TimeMasterCount : 1;

                            while (num > 0)
                            {
                                IntruderSupportRoles.Add((typeof(TimeMaster), CustomGameOptions.TimeMasterOn, 55, CustomGameOptions.UniqueTimeMaster));
                                num--;
                            }

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Time Master Done");
                        }

                        if (CustomGameOptions.GodfatherOn > 0 && CustomGameOptions.IntruderCount >= 3)
                        {
                            num = IsCustom ? CustomGameOptions.GodfatherCount : 1;
                                
                            while (num > 0)
                            {
                                IntruderSupportRoles.Add((typeof(Godfather), CustomGameOptions.GodfatherOn, 56, CustomGameOptions.UniqueGodfather));
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
                            SyndicateRoles.Add((typeof(Anarchist), CustomGameOptions.AnarchistOn, 57, false));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Anarchist Done");
                    }

                    if (CustomGameOptions.ShapeshifterOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.ShapeshifterCount : 1;

                        while (num > 0)
                        {
                            SyndicateDisruptionRoles.Add((typeof(Shapeshifter), CustomGameOptions.ShapeshifterOn, 58, CustomGameOptions.UniqueShapeshifter));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Shapeshifter Done");
                    }

                    if (CustomGameOptions.GorgonOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.GorgonCount : 1;

                        while (num > 0)
                        {
                            SyndicateKillingRoles.Add((typeof(Gorgon), CustomGameOptions.GorgonOn, 59, CustomGameOptions.UniqueGorgon));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Gorgon Done");
                    }

                    if (CustomGameOptions.FramerOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.FramerCount : 1;

                        while (num > 0)
                        {
                            SyndicateDisruptionRoles.Add((typeof(Framer), CustomGameOptions.FramerOn, 60, CustomGameOptions.UniqueFramer));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Framer Done");
                    }

                    if (CustomGameOptions.RebelOn > 0 && CustomGameOptions.SyndicateCount >= 3)
                    {
                        num = IsCustom ? CustomGameOptions.RebelCount : 1;
                                
                        while (num > 0)
                        {
                            SyndicateSupportRoles.Add((typeof(Rebel), CustomGameOptions.RebelOn, 61, CustomGameOptions.UniqueRebel));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Rebel Done");
                    }

                    if (CustomGameOptions.PoisonerOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.PoisonerCount : 1;

                        while (num > 0)
                        {
                            SyndicateDisruptionRoles.Add((typeof(Poisoner), CustomGameOptions.PoisonerOn, 51, CustomGameOptions.UniquePoisoner));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Poisoner Done");
                    }

                    if (CustomGameOptions.ConcealerOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.ConcealerCount : 1;

                        while (num > 0)
                        {
                            SyndicateSupportRoles.Add((typeof(Concealer), CustomGameOptions.ConcealerOn, 62, CustomGameOptions.UniqueConcealer));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Concealer Done");
                    }

                    if (CustomGameOptions.WarperOn > 0 && GameOptionsManager.Instance.currentNormalGameOptions.MapId != 4 && GameOptionsManager.Instance.currentNormalGameOptions.MapId != 5)
                    {
                        num = IsCustom ? CustomGameOptions.WarperCount : 1;

                        while (num > 0)
                        {
                            SyndicateSupportRoles.Add((typeof(Warper), CustomGameOptions.WarperOn, 63, CustomGameOptions.UniqueWarper));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Warper Done");
                    }

                    if (CustomGameOptions.BomberOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.BomberCount : 1;

                        while (num > 0)
                        {
                            SyndicateSupportRoles.Add((typeof(Bomber), CustomGameOptions.BomberOn, 64, CustomGameOptions.UniqueBomber));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Bomber Done");
                    }
                    #endregion

                    #region Modifiers
                    if (CustomGameOptions.DiseasedOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.DiseasedCount : 1;
                        
                        while (num > 0)
                        {
                            DiseasedModifiers.Add((typeof(Diseased), CustomGameOptions.DiseasedOn, 0));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Diseased Done");
                    }

                    if (CustomGameOptions.BaitOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.BaitCount : 1;
                        
                        while (num > 0)
                        {
                            BaitModifiers.Add((typeof(Bait), CustomGameOptions.BaitOn, 1));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Bait Done");
                    }
                    
                    if (CustomGameOptions.DwarfOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.DwarfCount : 1;
                        
                        while (num > 0)
                        {
                            GlobalModifiers.Add((typeof(Dwarf), CustomGameOptions.DwarfOn, 2));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Dwarf Done");
                    }
                    
                    if (CustomGameOptions.VIPOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.VIPCount : 1;
                        
                        while (num > 0)
                        {
                            GlobalModifiers.Add((typeof(VIP), CustomGameOptions.VIPOn, 3));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("VIP Done");
                    }
                    
                    if (CustomGameOptions.ShyOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.ShyCount : 1;
                        
                        while (num > 0)
                        {
                            GlobalModifiers.Add((typeof(Shy), CustomGameOptions.ShyOn, 4));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Shy Done");
                    }

                    if (CustomGameOptions.GiantOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.GiantCount : 1;
                        
                        while (num > 0)
                        {
                            GlobalModifiers.Add((typeof(Giant), CustomGameOptions.GiantOn, 5));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Giant Done");
                    }
                    
                    if (CustomGameOptions.DrunkOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.DrunkCount : 1;
                        
                        while (num > 0)
                        {
                            GlobalModifiers.Add((typeof(Drunk), CustomGameOptions.DrunkOn, 6));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Drunk Done");
                    }

                    if (CustomGameOptions.FlincherOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.FlincherCount : 1;
                        
                        while (num > 0)
                        {
                            GlobalModifiers.Add((typeof(Flincher), CustomGameOptions.FlincherOn, 7));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Flincher Done");
                    }

                    if (CustomGameOptions.CowardOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.CowardCount : 1;
                        
                        while (num > 0)
                        {
                            GlobalModifiers.Add((typeof(Coward), CustomGameOptions.CowardOn, 8));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Coward Done");
                    }

                    if (CustomGameOptions.VolatileOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.VolatileCount : 1;
                        
                        while (num > 0)
                        {
                            GlobalModifiers.Add((typeof(Volatile), CustomGameOptions.VolatileOn, 9));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Volatile Done");
                    }

                    if (CustomGameOptions.ProfessionalOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.ProfessionalCount : 1;
                        
                        while (num > 0)
                        {
                            ProfessionalModifiers.Add((typeof(Professional), CustomGameOptions.ProfessionalOn, 10));
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
                            SyndicateAbilityGet.Add((typeof(Assassin), CustomGameOptions.AssassinOn, 0));
                            num--;
                        }

                        num = CustomGameOptions.NumberOfNeutralAssassins;

                        while (num > 0)
                        {
                            NeutralAbilityGet.Add((typeof(Assassin), CustomGameOptions.AssassinOn, 0));
                            num--;
                        }

                        num = CustomGameOptions.NumberOfCrewAssassins;

                        while (num > 0)
                        {
                            CrewAbilityGet.Add((typeof(Assassin), CustomGameOptions.AssassinOn, 0));
                            num--;
                        }

                        num = CustomGameOptions.NumberOfImpostorAssassins;

                        while (num > 0)
                        {
                            IntruderAbilityGet.Add((typeof(Assassin), CustomGameOptions.AssassinOn, 0));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Assassin Done");
                    }

                    if (CustomGameOptions.SnitchOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.SnitchCount : 1;
                        
                        while (num > 0)
                        {
                            SnitchAbilityGet.Add((typeof(Snitch), CustomGameOptions.SnitchOn, 1));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Snitch Done");
                    }

                    if (CustomGameOptions.InsiderOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.InsiderCount : 1;
                        
                        while (num > 0)
                        {
                            TaskedAbilityGet.Add((typeof(Insider), CustomGameOptions.InsiderOn, 2));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Insider Done");
                    }

                    if (CustomGameOptions.LighterOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.LighterCount : 1;
                        
                        while (num > 0)
                        {
                            CrewAbilityGet.Add((typeof(Lighter), CustomGameOptions.LighterOn, 3));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Lighter Done");
                    }

                    if (CustomGameOptions.MultitaskerOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.MultitaskerCount : 1;
                        
                        while (num > 0)
                        {
                            TaskedAbilityGet.Add((typeof(Multitasker), CustomGameOptions.MultitaskerOn, 4));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Multitasker Done");
                    }

                    if (CustomGameOptions.RadarOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.RadarCount : 1;
                        
                        while (num > 0)
                        {
                            GlobalAbilityGet.Add((typeof(Radar), CustomGameOptions.RadarOn, 5));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Radar Done");
                    }

                    if (CustomGameOptions.TiebreakerOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.TiebreakerCount : 1;
                        
                        while (num > 0)
                        {
                            GlobalAbilityGet.Add((typeof(Tiebreaker), CustomGameOptions.TiebreakerOn, 6));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Tiebreaker Done");
                    }

                    if (CustomGameOptions.TorchOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.TorchCount : 1;
                        
                        while (num > 0)
                        {
                            NonEvilAbilityGet.Add((typeof(Torch), CustomGameOptions.TorchOn, 7));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Torch Done");
                    }

                    if (CustomGameOptions.UnderdogOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.UnderdogCount : 1;
                        
                        while (num > 0)
                        {
                            EvilAbilityGet.Add((typeof(Underdog), CustomGameOptions.UnderdogOn, 8));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Underdog Done");
                    }

                    if (CustomGameOptions.TunnelerOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.TunnelerCount : 1;
                        
                        while (num > 0)
                        {
                            TunnelerAbilityGet.Add((typeof(Tunneler), CustomGameOptions.TunnelerOn, 9));
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
                            LoverRivalObjectifierGet.Add((typeof(Lovers), CustomGameOptions.LoversOn, 0));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Lovers Done");
                    }

                    if (CustomGameOptions.RivalsOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.RivalsCount : 1;
                        
                        while (num > 0)
                        {
                            LoverRivalObjectifierGet.Add((typeof(Rivals), CustomGameOptions.RivalsOn, 1));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Rivals Done");
                    }

                    if (CustomGameOptions.FanaticOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.FanaticCount : 1;
                        
                        while (num > 0)
                        {
                            CrewObjectifierGet.Add((typeof(Fanatic), CustomGameOptions.FanaticOn, 2));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Fanatic Done");
                    }

                    if (CustomGameOptions.CorruptedOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.CorruptedCount : 1;
                        
                        while (num > 0)
                        {
                            CrewObjectifierGet.Add((typeof(Corrupted), CustomGameOptions.CorruptedOn, 3));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Corrupted Done");
                    }

                    if (CustomGameOptions.OverlordOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.OverlordCount : 1;
                        
                        while (num > 0)
                        {
                            NeutralObjectifierGet.Add((typeof(Overlord), CustomGameOptions.OverlordOn, 4));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Overlord Done");
                    }

                    if (CustomGameOptions.AlliedOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.AlliedCount : 1;
                        
                        while (num > 0)
                        {
                            NeutralObjectifierGet.Add((typeof(Allied), CustomGameOptions.AlliedOn, 5));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Allied Done");
                    }

                    if (CustomGameOptions.TraitorOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.TraitorCount : 1;
                        
                        while (num > 0)
                        {
                            CrewObjectifierGet.Add((typeof(Traitor), CustomGameOptions.TraitorOn, 6));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Traitor Done");
                    }

                    if (CustomGameOptions.TaskmasterOn > 0)
                    {
                        num = IsCustom ? CustomGameOptions.TaskmasterCount : 1;
                        
                        while (num > 0)
                        {
                            NeutralObjectifierGet.Add((typeof(Taskmaster), CustomGameOptions.TaskmasterOn, 7));
                            num--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Taskmaster Done");
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