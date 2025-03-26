namespace TownOfUsReworked.Modules;

public sealed class DeadPlayer(byte killer, byte player)
{
    public byte KillerId { get; } = killer;
    public byte PlayerId { get; } = player;
    private float KillTime { get; } = Time.time;
    private PlayerControl Killer { get; } = PlayerById(killer);
    private PlayerControl Body { get; } = PlayerById(player);

    public float KillAge => Time.time - KillTime;

    public PlayerControl Reporter { get; set; }

    public string ParseBodyReport()
    {
        if (!Reporter.Is<IExaminer>())
            return "";

        var report = $"{Body.name}'s Report:";

        if (Reporter.IsFlashed())
            report += "\nYou have been blinded so you cannot tell what happened to the body!";
        else
        {
            var killerRole = Killer.GetRole();
            report += $"\nThey died approximately {Mathf.RoundToInt(KillAge)}s ago!";

            if (Body == Killer)
                report += "\nThere is evidence of self-harm!";
            else if (Coroner.CoronerReportName && Coroner.CoronerKillerNameTime <= KillAge)
                report += $"\nThey were killed by {Killer.name}!";
            else
            {
                if (Coroner.CoronerReportRole)
                    report += $"\nThey were killed by a {killerRole.Name}!";
                else
                {
                    report += killerRole.Faction switch
                    {
                        Faction.Crew => "\nThe killer is from the Crew!",
                        Faction.Intruder => "\nThe killer is an Intruder!",
                        Faction.Syndicate => "\nThe killer is from the Syndicate!",
                        Faction.Neutral => "\nThe killer is a Neutral!",
                        Faction.Pandorica => "\nThe killer is from the Pandorica!",
                        Faction.Compliance => "\nThe killer is from the Order of Compliance!",
                        Faction.Illuminati => "\nThe killer is from the Illuminati!",
                        _ => ""
                    };
                }

                report += $"\nThe killer is a {(Killer.Data.DefaultOutfit.ColorId.IsLighter() ? "lighter" : "darker")} color!";
            }
        }

        return report;
    }
}