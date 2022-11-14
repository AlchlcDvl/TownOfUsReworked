using System.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles;

namespace TownOfUsReworked.Lists
{
    public static class DefinedLists
    {
        public static List<Role> NeutralKillers = new List<Role>();
        public static List<Role> NeutralNeophytes = new List<Role>();
        public static List<Role> NeutralProselytes = new List<Role>();
        public static List<Role> NeutralEvils = new List<Role>();
        public static List<Role> NeutralBenigns = new List<Role>();
        public static List<Role> Neutrals = new List<Role>();
        public static List<Role> IntruderKillers = new List<Role>();
        public static List<Role> IntruderSupporters = new List<Role>();
        public static List<Role> IntruderDecievers = new List<Role>();
        public static List<Role> IntruderConcealers = new List<Role>();
        public static List<Role> IntruderUtility = new List<Role>();
        public static List<Role> Intruders = new List<Role>();
        public static List<Role> CrewUtility = new List<Role>();
        public static List<Role> CrewInvestigators = new List<Role>();
        public static List<Role> CrewSupporters = new List<Role>();
        public static List<Role> CrewSovereigners = new List<Role>();
        public static List<Role> CrewKillers = new List<Role>();
        public static List<Role> CrewProtectors = new List<Role>();
        public static List<Role> CrewCheckers = new List<Role>();
        public static List<Role> Crew = new List<Role>();
        public static List<Role> SyndicateUtility = new List<Role>();
        public static List<Role> SyndicatePowers = new List<Role>();
        public static List<Role> SyndicateKillers = new List<Role>();
        public static List<Role> SyndicateSupporters = new List<Role>();
        public static List<Role> SyndicateDisruptors = new List<Role>();
        public static List<Role> Syndicate = new List<Role>();
        public static List<Role> Others = new List<Role>();
        
        public static void Prefix()
        {
            foreach (var role in Role.AllRoles)
            {
                if (role.Faction == Faction.Crew)
                {
                    if (role.RoleAlignment == RoleAlignment.CrewSupport)
                        CrewSupporters.Add(role);
                    else if (role.RoleAlignment == RoleAlignment.CrewCheck)
                        CrewCheckers.Add(role);
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
                    else if (role.RoleAlignment == RoleAlignment.NeutralChaos)
                        NeutralNeophytes.Add(role);
                    else if (role.RoleAlignment == RoleAlignment.NeutralEvil)
                        NeutralEvils.Add(role);
                    else if (role.RoleAlignment == RoleAlignment.NeutralKill)
                        NeutralKillers.Add(role);
                    else if (role.RoleAlignment == RoleAlignment.NeutralPower)
                        NeutralProselytes.Add(role);
                    
                    Neutrals.Add(role);
                }
                else if (role.Faction == Faction.Syndicate)
                {
                    if (role.RoleAlignment == RoleAlignment.SyndicateChaos)
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
                else if (role.Faction == Faction.Intruders)
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
                else if (role.Faction == Faction.Other)
                    Others.Add(role);
            }
        }
    }
}