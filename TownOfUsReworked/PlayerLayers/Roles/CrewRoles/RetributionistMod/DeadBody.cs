using System;
using System.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Lobby.CustomOption;
using HarmonyLib;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod
{
    public class BodyReport
    {
        public PlayerControl Killer { get; set; }
        public PlayerControl Body { get; set; }
        public float KillAge { get; set; }

        public static string ParseBodyReport(BodyReport br)
        {
            var coronerReport = $"{br.Body.Data.PlayerName}'s Report:\n";
            var killerRole = Role.GetRole(br.Killer);
            var bodyRole = Role.GetRole(br.Body);
            var selfFlag = br.Body == br.Killer;

            coronerReport += $"They died approximately {Math.Round(br.KillAge / 1000)}s ago!\n";
            coronerReport += $"They were a {bodyRole.Name}!\n";

            if (selfFlag)
                coronerReport += "There is evidence of self-harm!\n";
            else if (!selfFlag)
            {
                if (CustomGameOptions.CoronerReportRole)
                    coronerReport += $"They were killed by a {killerRole.Name}!\n";
                else
                {
                    if (br.Killer.Is(Faction.Crew))
                        coronerReport += "The killer is from the Crew!\n";
                    else if (br.Killer.Is(Faction.Neutral))
                        coronerReport += "The killer is a Neutral!\n";
                    else if (br.Killer.Is(Faction.Intruder))
                        coronerReport += "The killer is an Intruder!\n";
                    else if (br.Killer.Is(Faction.Syndicate))
                        coronerReport += "The killer is from the Syndicate!\n";
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
                    {21, "lighter"},// beige
                    {22, "darker"},// magenta
                    {23, "lighter"},// turquoise
                    {24, "lighter"},// lilac
                    {25, "darker"},// olive
                    {26, "lighter"},// azure
                    {27, "darker"},// plum
                    {28, "darker"},// jungle
                    {29, "lighter"},// mint
                    {30, "lighter"},// chartreuse
                    {31, "darker"},// macau
                    {32, "darker"},// tawny
                    {33, "lighter"},// gold
                    {34, "lighter"},// rainbow
                };

                coronerReport += $"The killer is a {colors[br.Killer.CurrentOutfit.ColorId]} color!\n";
            
                if (CustomGameOptions.CoronerReportName && CustomGameOptions.CoronerKillerNameTime <= Math.Round(br.KillAge / 1000)) 
                    coronerReport += $"They were killed by {br.Killer.Data.PlayerName}!";
            }

            return coronerReport;
        }
    }
}
