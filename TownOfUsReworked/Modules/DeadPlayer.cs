namespace TownOfUsReworked.Modules;

public class DeadPlayer(byte killer, byte player)
{
    public byte KillerId { get; } = killer;
    public byte PlayerId { get; } = player;
    public DateTime KillTime { get; } = DateTime.UtcNow;

    public PlayerControl Killer => PlayerById(KillerId);
    public PlayerControl Body => PlayerById(PlayerId);

    public PlayerControl Reporter { get; set; }
    public float KillAge { get; set; }

    public string ParseBodyReport()
    {
        var report = $"{Body.name}'s Report:";
        var killerRole = Killer.GetRole();
        var bodyRole = Body.GetRole();

        if (!Reporter.IsFlashed())
        {
            report += $"\nThey died approximately {Math.Round(KillAge / 1000f)}s ago!";
            report += $"\nThey were a {bodyRole.Name}!";

            if (Body == Killer)
                report += "\nThere is evidence of self-harm!";
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

                if (Coroner.CoronerReportName && Coroner.CoronerKillerNameTime <= (KillAge / 1000))
                    report += $"\nThey were killed by {Killer.name}!";
            }
        }
        else
            report += "\nYou have been blinded so you cannot tell what happened to the body!";

        return report;
    }
}