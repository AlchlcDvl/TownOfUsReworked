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
                    else if (Killer.Is(Faction.Crew))
                        report += "The killer is from the Crew!\n";
                    else if (Killer.Is(Faction.Neutral))
                        report += "The killer is a Neutral!\n";
                    else if (Killer.Is(Faction.Intruder))
                        report += "The killer is an Intruder!\n";
                    else if (Killer.Is(Faction.Syndicate))
                        report += "The killer is from the Syndicate!\n";

                    report += $"The killer is a {Role.LightDarkColors[Killer.CurrentOutfit.ColorId]} color!\n";

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