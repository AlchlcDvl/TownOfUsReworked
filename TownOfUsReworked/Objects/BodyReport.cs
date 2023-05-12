namespace TownOfUsReworked.Objects
{
    [HarmonyPatch]
    public class BodyReport
    {
        public PlayerControl Killer;
        public PlayerControl Body;
        public PlayerControl Reporter;
        public float KillAge;

        public string ParseBodyReport()
        {
            var report = $"{Body.Data.PlayerName}'s Report:\n";
            var killerRole = Role.GetRole(Killer);
            var bodyRole = Role.GetRole(Body);
            var selfFlag = Body == Killer;

            var flashed = Role.GetRoles<Grenadier>(RoleEnum.Grenadier).Any(x => x.Flashed && x.FlashedPlayers.Contains(Reporter)) ||
                Role.GetRoles<PromotedGodfather>(RoleEnum.PromotedGodfather).Any(x => x.OnEffect && x.FlashedPlayers.Contains(Reporter));

            if (!flashed)
            {
                report += $"They died approximately {Math.Round(KillAge / 1000)}s ago!\n";
                report += $"They were a {bodyRole.Name}!\n";

                if (selfFlag)
                    report += "There is evidence of self-harm!";
                else if (!selfFlag)
                {
                    if (CustomGameOptions.CoronerReportRole)
                        report += $"They were killed by a {killerRole.Name}!\n";
                    else
                    {
                        if (Killer.Is(Faction.Crew))
                            report += "The killer is from the Crew!\n";
                        else if (Killer.Is(Faction.Neutral))
                            report += "The killer is a Neutral!\n";
                        else if (Killer.Is(Faction.Intruder))
                            report += "The killer is an Intruder!\n";
                        else if (Killer.Is(Faction.Syndicate))
                            report += "The killer is from the Syndicate!\n";
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
                        {33, "lighter"},// gold
                        {34, "lighter"},// panda
                        {35, "darker"},// contrast
                        {36, "lighter"},// chroma
                        {37, "darker"},// mantle
                        {38, "lighter"},// fire
                        {39, "lighter"},// galaxy
                        {40, "lighter"},// monochrome
                        {41, "lighter"},// rainbow
                    };

                    report += $"The killer is a {colors[Killer.CurrentOutfit.ColorId]} color!\n";

                    if (CustomGameOptions.CoronerReportName && CustomGameOptions.CoronerKillerNameTime <= Math.Round(KillAge / 1000))
                        report += $"They were killed by {Killer.Data.PlayerName}!";
                }
            }
            else
                report += "You have been blinded so you cannot tell what happened to the body!";

            return report;
        }
    }
}