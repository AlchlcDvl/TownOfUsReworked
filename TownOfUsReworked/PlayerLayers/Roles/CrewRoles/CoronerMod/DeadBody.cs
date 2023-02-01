using System;
using System.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using HarmonyLib;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.CoronerMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
    public class MeetingBodyReport
    {
        private static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] GameData.PlayerInfo info)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || !PlayerControl.LocalPlayer.Is(RoleEnum.Coroner))
                return;

            Role.GetRole<Coroner>(PlayerControl.LocalPlayer).Reported.Add(info.PlayerId);
        }
    }

    public class BodyReport
    {
        public PlayerControl Killer { get; set; }
        public PlayerControl Reporter { get; set; }
        public PlayerControl Body { get; set; }
        public float KillAge { get; set; }

        public static string ParseBodyReport(BodyReport br)
        {
            var coronerReport = $"{br.Body.Data.PlayerName}'s Report:\n";
            var killerRole = Role.GetRole(br.Killer);
            var bodyRole = Role.GetRole(br.Body);
            var selfFlag = br.Body == br.Killer;

            if (selfFlag)
                coronerReport += "There are evident marks of self-harm!\n";
            
            coronerReport += $"They were a {bodyRole.Name}!\n";

            if (!selfFlag)
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
            }

            coronerReport += $"They died approximately {Math.Round(br.KillAge / 1000)}s ago!\n";
            
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

            if (!selfFlag)
                coronerReport += $"The killer is a {colors[br.Killer.CurrentOutfit.ColorId]} color!\n";
            
            if (CustomGameOptions.CoronerReportName && CustomGameOptions.CoronerKillerNameTime <= Math.Round(br.KillAge / 1000) && !selfFlag) 
                coronerReport += $"They were killed by {br.Killer.Data.PlayerName}!";
                
            return coronerReport;
        }
    }
}
