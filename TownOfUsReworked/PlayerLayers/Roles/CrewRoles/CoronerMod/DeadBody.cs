using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.CoronerMod
{
    public class DeadPlayer
    {
        public byte KillerId { get; set; }
        public byte PlayerId { get; set; }
        public DateTime KillTime { get; set; }
    }

    public class BodyReport
    {
        public PlayerControl Killer { get; set; }
        public PlayerControl Reporter { get; set; }
        public PlayerControl Body { get; set; }
        public float KillAge { get; set; }

        public static string ParseBodyReport(BodyReport br)
        {
            var coronerReport = "";
            var killerRole = Role.GetRole(br.Killer);
            var bodyRole = Role.GetRole(br.Body);

            if (br.Body == br.Killer)
                coronerReport += $"{br.Body.Data.PlayerName}'s Report: There are evident marks of self-harm!\n";
            
            coronerReport += $"{br.Body.Data.PlayerName}'s Report: {bodyRole.CoronerDeadReport}\n";
            coronerReport += $"{br.Body.Data.PlayerName}'s Report: They were killed {Math.Round(br.KillAge / 1000)}s ago!\n";

            if (CustomGameOptions.CoronerReportRole)
                coronerReport += $"{br.Body.Data.PlayerName}'s Report: {killerRole.CoronerKillerReport}\n";
            else
            {
                if (br.Killer.Is(Faction.Crew))
                    coronerReport += $"{br.Body.Data.PlayerName}'s Report: The killer is from the Crew!\n";
                else if (br.Killer.Is(Faction.Neutral))
                    coronerReport += $"{br.Body.Data.PlayerName}'s Report: The killer is a Neutral!\n";
                else if (br.Killer.Is(Faction.Intruders))
                    coronerReport += $"{br.Body.Data.PlayerName}'s Report: The killer is an Intruder!\n";
                else
                    coronerReport += $"{br.Body.Data.PlayerName}'s Report: This is a bit odd.\n";
            }
            
            var colors = new Dictionary<int, string>
                {
                    {0, "darker"},// red
                    {1, "darker"},// blue
                    {2, "darker"},// green
                    {3, "lighter"},// pink
                    {4, "lighter"},// orange
                    {5, "lighter"},// yellow
                    {6, "darker"},// black
                    {7, "lighter"},// white
                    {8, "darker"},// purple
                    {9, "darker"},// brown
                    {10, "lighter"},// cyan
                    {11, "lighter"},// lime
                    {12, "darker"},// maroon
                    {13, "lighter"},// rose
                    {14, "lighter"},// banana
                    {15, "darker"},// gray
                    {16, "darker"},// tan
                    {17, "lighter"},// coral
                    {18, "darker"},// watermelon
                    {19, "darker"},// chocolate
                    {20, "lighter"},// sky blue
                    {21, "darker"},// beige
                    {22, "lighter"},// hot pink
                    {23, "lighter"},// turquoise
                    {24, "lighter"},// lilac
                    {25, "darker"},// olive
                    {26, "lighter"},// azure
                    {27, "lighter"},// rainbow
                    {27, "lighter"}, //Tomato
                    {28, "darker"}, //backrooms
                    {29, "darker"}, //Gold
                    {30, "darker"}, //Space
                    {31, "lighter"}, //Ice
                    {32, "lighter"}, //Mint
                    {33, "darker"}, //BTS
                    {34, "darker"}, //Forest Green
                    {35, "lighter"}, //Donation
                    {36, "darker"}, //Cherry
                    {37, "lighter"}, //Toy
                    {38, "lighter"}, //Pizzaria
                    {39, "lighter"}, //Starlight
                    {40, "lighter"}, //Softball
                    {41, "darker"}, //Dark Jester
                    {42, "darker"}, //FRESH
                    {43, "darker"}, //Goner
                    {44, "lighter"}, //Psychic Friend
                    {45, "lighter"}, //Frost
                    {46, "darker"}, //Abyss Green
                    {47, "darker"}, //Midnight
                    {48, "darker"}, //<3
                    {49, "lighter"}, //Heat From Fire
                    {50, "lighter"}, //Fire From Heat
                    {51, "lighter"}, //Determination
                    {52, "lighter"}, //Patience
                    {53, "darker"}, //Bravery
                    {54, "darker"}, //Integrity
                    {55, "darker"}, //Perserverance
                    {56, "darker"}, //Kindness
                    {57, "lighter"}, //Bravery
                    {58, "darker"}, //Purple Plumber
                    {59, "lighter"}, //Rainbow
                };

            coronerReport += $"{br.Body.Data.PlayerName}'s Report: The killer is a {colors[br.Killer.CurrentOutfit.ColorId]} color!\n";
            
            if (CustomGameOptions.CoronerReportName /*&& CustomGameOptions.CoronerKillerNameTime <= Math.Round(br.KillAge / 1000)*/) 
                coronerReport += $"{br.Body.Data.PlayerName}'s Report: They were killed by {br.Killer.Data.PlayerName}!";
                
            return coronerReport;
        }
    }
}
