namespace TownOfUsReworked.Modules;

public class DeadPlayer(byte killer, byte player)
{
    public byte KillerId { get; } = killer;
    public byte PlayerId { get; } = player;
    public float KillTime { get; } = Time.time;
    public PlayerControl Killer { get; } = PlayerById(killer);
    public PlayerControl Body { get; } = PlayerById(player);

    public float KillAge => Time.time - KillTime;

    public PlayerControl Reporter { get; set; }

    public string ParseBodyReport()
    {
        if (!Reporter.IIs<IExaminer>())
            return "";

        var report = $"{Body.name}'s Report:";

        if (Reporter.IsFlashed())
            report += "\nYou have been blinded so you cannot tell what happened to the body!";
        else
        {
            var killerRole = Killer.GetRole();
            var bodyRole = Body.GetRole();
            report += $"\nThey died approximately {Mathf.RoundToInt(KillAge)}s ago!";

            if (Body == Killer)
                report += "\nThere is evidence of self-harm!";
            else if (Coroner.CoronerReportName && Coroner.CoronerKillerNameTime <= KillAge)
                report += $"\nThey were killed by {Killer.name}!";
            else
            {
                if (Coroner.CoronerReportRole)
                    report += $"\nThey were killed by a {killerRole.Name}!";
                else if (Killer.Is(Faction.Crew))
                    report += "\nThe killer is from the Crew!";
                else if (Killer.Is(Faction.Neutral))
                    report += "\nThe killer is a Neutral!";
                else if (Killer.Is(Faction.Intruder))
                    report += "\nThe killer is an Intruder!";
                else if (Killer.Is(Faction.Syndicate))
                    report += "\nThe killer is from the Syndicate!";

                report += $"\nThe killer is a {(Killer.Data.DefaultOutfit.ColorId.IsLighter() ? "lighter" : "darker")} color!";
            }
        }

        return report;
    }
}