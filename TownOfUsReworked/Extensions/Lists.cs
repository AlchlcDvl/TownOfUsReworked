using System.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Modifiers;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using UnityEngine;
using HarmonyLib;

namespace TownOfUsReworked.Extensions
{
    [HarmonyPatch]
    public static class Lists
    {
        public static List<Role> NeutralKillers;
        public static List<Role> NeutralNeophytes;
        public static List<Role> NeutralProselytes;
        public static List<Role> NeutralEvils;
        public static List<Role> NeutralBenigns;
        public static List<Role> Neutrals;

        public static List<Role> Undead;
        public static List<Role> Cabal;

        public static List<Role> IntruderKillers;
        public static List<Role> IntruderSupporters;
        public static List<Role> IntruderDecievers;
        public static List<Role> IntruderConcealers;
        public static List<Role> IntruderUtility;
        public static List<Role> Intruders;

        public static List<Role> CrewUtility;
        public static List<Role> CrewInvestigators;
        public static List<Role> CrewSupporters;
        public static List<Role> CrewSovereigners;
        public static List<Role> CrewKillers;
        public static List<Role> CrewProtectors;
        public static List<Role> CrewAuditors;
        public static List<Role> Crew;

        public static List<Role> SyndicateUtility;
        public static List<Role> SyndicatePowers;
        public static List<Role> SyndicateKillers;
        public static List<Role> SyndicateSupporters;
        public static List<Role> SyndicateDisruptors;
        public static List<Role> Syndicate;

        public static List<Role> AllRoles;
        public static List<Modifier> AllModifiers;
        public static List<Ability> AllAbilities;
        public static List<Objectifier> AllObjectifiers;
        
        public static void DefinedLists()
        {
            NeutralKillers = new List<Role>();
            NeutralNeophytes = new List<Role>();
            NeutralProselytes = new List<Role>();
            NeutralEvils = new List<Role>();
            NeutralBenigns = new List<Role>();
            Neutrals = new List<Role>();

            Undead = new List<Role>();
            Cabal = new List<Role>();

            IntruderKillers = new List<Role>();
            IntruderSupporters = new List<Role>();
            IntruderDecievers = new List<Role>();
            IntruderConcealers = new List<Role>();
            IntruderUtility = new List<Role>();
            Intruders = new List<Role>();

            CrewUtility = new List<Role>();
            CrewInvestigators = new List<Role>();
            CrewSupporters = new List<Role>();
            CrewSovereigners = new List<Role>();
            CrewKillers = new List<Role>();
            CrewProtectors = new List<Role>();
            CrewAuditors = new List<Role>();
            Crew = new List<Role>();

            SyndicateUtility = new List<Role>();
            SyndicatePowers = new List<Role>();
            SyndicateKillers = new List<Role>();
            SyndicateSupporters = new List<Role>();
            SyndicateDisruptors = new List<Role>();
            Syndicate = new List<Role>();

            AllRoles = new List<Role>();
            AllModifiers = new List<Modifier>();
            AllAbilities = new List<Ability>();
            AllObjectifiers = new List<Objectifier>();

            foreach (var role in Role.AllRoles)
            {
                if (role.Faction == Faction.Crew)
                {
                    if (role.RoleAlignment == RoleAlignment.CrewSupport)
                        CrewSupporters.Add(role);
                    else if (role.RoleAlignment == RoleAlignment.CrewAudit)
                        CrewAuditors.Add(role);
                    else if (role.RoleAlignment == RoleAlignment.CrewUtil)
                        CrewUtility.Add(role);
                    else if (role.RoleAlignment == RoleAlignment.CrewInvest)
                        CrewInvestigators.Add(role);
                    else if (role.RoleAlignment == RoleAlignment.CrewSov)
                        CrewSovereigners.Add(role);
                    else if (role.RoleAlignment == RoleAlignment.CrewProt)
                        CrewProtectors.Add(role);
                    else if (role.RoleAlignment == RoleAlignment.CrewKill)
                        CrewKillers.Add(role);
                    
                    Crew.Add(role);
                }
                else if (role.Faction == Faction.Neutral)
                {
                    if (role.RoleAlignment == RoleAlignment.NeutralBen)
                        NeutralBenigns.Add(role);
                    else if (role.RoleAlignment == RoleAlignment.NeutralNeo)
                        NeutralNeophytes.Add(role);
                    else if (role.RoleAlignment == RoleAlignment.NeutralEvil)
                        NeutralEvils.Add(role);
                    else if (role.RoleAlignment == RoleAlignment.NeutralKill)
                        NeutralKillers.Add(role);
                    else if (role.RoleAlignment == RoleAlignment.NeutralPros)
                        NeutralProselytes.Add(role);
                    
                    if (role.SubFaction == SubFaction.Undead)
                        Undead.Add(role);
                    else if (role.SubFaction == SubFaction.Cabal)
                        Cabal.Add(role);
                    
                    Neutrals.Add(role);
                }
                else if (role.Faction == Faction.Syndicate)
                {
                    if (role.RoleAlignment == RoleAlignment.SyndicateDisruption)
                        SyndicateDisruptors.Add(role);
                    else if (role.RoleAlignment == RoleAlignment.SyndicateKill)
                        SyndicateKillers.Add(role);
                    else if (role.RoleAlignment == RoleAlignment.SyndicatePower)
                        SyndicatePowers.Add(role);
                    else if (role.RoleAlignment == RoleAlignment.SyndicateSupport)
                        SyndicateSupporters.Add(role);
                    else if (role.RoleAlignment == RoleAlignment.SyndicateUtil)
                        SyndicateUtility.Add(role);
                    
                    Syndicate.Add(role);
                }
                else if (role.Faction == Faction.Intruder)
                {
                    if (role.RoleAlignment == RoleAlignment.IntruderConceal)
                        IntruderConcealers.Add(role);
                    else if (role.RoleAlignment == RoleAlignment.IntruderDecep)
                        IntruderDecievers.Add(role);
                    else if (role.RoleAlignment == RoleAlignment.IntruderKill)
                        IntruderKillers.Add(role);
                    else if (role.RoleAlignment == RoleAlignment.IntruderSupport)
                        IntruderSupporters.Add(role);
                    else if (role.RoleAlignment == RoleAlignment.IntruderUtil)
                        IntruderUtility.Add(role);
                    
                    Intruders.Add(role);
                }

                AllRoles.Add(role);
            }

            foreach (var modifier in Modifier.AllModifiers)
                AllModifiers.Add(modifier);

            foreach (var ability in Ability.AllAbilities)
                AllAbilities.Add(ability);

            foreach (var objectifier in Objectifier.AllObjectifiers)
                AllObjectifiers.Add(objectifier);
        }
    }
}