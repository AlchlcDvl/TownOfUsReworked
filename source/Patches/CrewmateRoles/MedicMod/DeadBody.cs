using System;
using System.Collections.Generic;
using TownOfUs.Extensions;

namespace TownOfUs.CrewmateRoles.MedicMod
{
    public class DeadPlayer
    {
        public byte KillerId { get; set; }
        public byte PlayerId { get; set; }
        public DateTime KillTime { get; set; }
    }

    //body report class for when medic reports a body
    public class BodyReport
    {
        public PlayerControl Killer { get; set; }
        public PlayerControl Reporter { get; set; }
        public PlayerControl Body { get; set; }
        public float KillAge { get; set; }

        public static string ParseBodyReport(BodyReport br)
        {
            //System.Console.WriteLine(br.KillAge);
            if (br.KillAge > CustomGameOptions.MedicReportColorDuration * 1000)
                return
                    $"Body Report: The corpse is too old to gain information from. (Killed {Math.Round(br.KillAge / 1000)}s ago)";

            if (br.Killer.PlayerId == br.Body.PlayerId)
                return
                    $"Body Report: The kill appears to have been a suicide! (Killed {Math.Round(br.KillAge / 1000)}s ago)";

            if (br.KillAge < CustomGameOptions.MedicReportNameDuration * 1000)
                return
                    $"Body Report: The killer appears to be {br.Killer.Data.PlayerName}! (Killed {Math.Round(br.KillAge / 1000)}s ago)";

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
            var typeOfColor = colors[br.Killer.GetDefaultOutfit().ColorId];
            return
                $"Body Report: The killer appears to be a {typeOfColor} color. (Killed {Math.Round(br.KillAge / 1000)}s ago)";
        }
    }
}
