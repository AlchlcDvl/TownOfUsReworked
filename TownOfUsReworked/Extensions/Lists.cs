using System.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles;
using UnityEngine;

namespace TownOfUsReworked.Extensions
{
    public static class Lists
    {
        public static List<Role> NeutralKillers = new List<Role>();
        public static List<Role> NeutralNeophytes = new List<Role>();
        public static List<Role> NeutralProselytes = new List<Role>();
        public static List<Role> NeutralEvils = new List<Role>();
        public static List<Role> NeutralBenigns = new List<Role>();
        public static List<Role> Neutrals = new List<Role>();

        public static List<Role> Vampires = new List<Role>();

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

        public static List<Role> AllRoles = new List<Role>();
        
        public static List<AudioClip> Sounds = new List<AudioClip>();
        
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
                    else if (role.RoleAlignment == RoleAlignment.NeutralNeo)
                        NeutralNeophytes.Add(role);
                    else if (role.RoleAlignment == RoleAlignment.NeutralEvil)
                        NeutralEvils.Add(role);
                    else if (role.RoleAlignment == RoleAlignment.NeutralKill)
                        NeutralKillers.Add(role);
                    else if (role.RoleAlignment == RoleAlignment.NeutralPros)
                        NeutralProselytes.Add(role);
                    
                    if (role.SubFaction == SubFaction.Vampires)
                        Vampires.Add(role);
                    
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

                AllRoles.Add(role);
            }

            Sounds.Add(TownOfUsReworked.AlertSound);
            Sounds.Add(TownOfUsReworked.ArsonistWin);
            Sounds.Add(TownOfUsReworked.JuggernautWin);
            Sounds.Add(TownOfUsReworked.AttemptSound);
            Sounds.Add(TownOfUsReworked.CleanSound);
            Sounds.Add(TownOfUsReworked.DouseSound);
            Sounds.Add(TownOfUsReworked.FixSound);
            Sounds.Add(TownOfUsReworked.EngineerIntro);
            Sounds.Add(TownOfUsReworked.FlashSound);
            Sounds.Add(TownOfUsReworked.GlitchWin);
            Sounds.Add(TownOfUsReworked.HackSound);
            Sounds.Add(TownOfUsReworked.TrollWin);
            Sounds.Add(TownOfUsReworked.MedicIntro);
            Sounds.Add(TownOfUsReworked.MineSound);
            Sounds.Add(TownOfUsReworked.MorphSound);
            Sounds.Add(TownOfUsReworked.PhantomWin);
            Sounds.Add(TownOfUsReworked.PlaguebearerWin);
            Sounds.Add(TownOfUsReworked.PoisonSound);
            Sounds.Add(TownOfUsReworked.ProtectSound);
            Sounds.Add(TownOfUsReworked.RememberSound);
            Sounds.Add(TownOfUsReworked.ReviveSound);
            Sounds.Add(TownOfUsReworked.RewindSound);
            Sounds.Add(TownOfUsReworked.SampleSound);
            Sounds.Add(TownOfUsReworked.RevealSound);
            Sounds.Add(TownOfUsReworked.ShieldSound);
            Sounds.Add(TownOfUsReworked.VestSound);
            Sounds.Add(TownOfUsReworked.TrackSound);
            Sounds.Add(TownOfUsReworked.TransportSound);
            Sounds.Add(TownOfUsReworked.WerewolfWin);
            Sounds.Add(TownOfUsReworked.NBWin);
            Sounds.Add(TownOfUsReworked.CrewmateIntro);
            Sounds.Add(TownOfUsReworked.ImpostorIntro);
            Sounds.Add(TownOfUsReworked.IntruderWin);
            Sounds.Add(TownOfUsReworked.CrewWin);
            Sounds.Add(TownOfUsReworked.MorphlingIntro);
            Sounds.Add(TownOfUsReworked.AgentIntro);
            Sounds.Add(TownOfUsReworked.AmnesiacIntro);
            Sounds.Add(TownOfUsReworked.BloodlustSound);
            Sounds.Add(TownOfUsReworked.GlitchIntro);
            Sounds.Add(TownOfUsReworked.WarperIntro);
            Sounds.Add(TownOfUsReworked.GodfatherIntro);
            Sounds.Add(TownOfUsReworked.CoronerIntro);
            Sounds.Add(TownOfUsReworked.ShifterIntro);
            Sounds.Add(TownOfUsReworked.StabSound);
            Sounds.Add(TownOfUsReworked.IgniteSound);
            Sounds.Add(TownOfUsReworked.InteractSound);
            Sounds.Add(TownOfUsReworked.ShootingSound);
            Sounds.Add(TownOfUsReworked.TimeFreezeSound);
            Sounds.Add(TownOfUsReworked.KillSFX);
        }
    }
}