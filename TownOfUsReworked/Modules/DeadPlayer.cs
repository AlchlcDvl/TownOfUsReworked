namespace TownOfUsReworked.Modules;

public sealed class DeadPlayer(byte killer, byte player)
{
    public readonly byte KillerId = killer;
    public readonly byte PlayerId = player;
    private readonly float KillTime = Time.time;
    private readonly PlayerControl Killer = PlayerById(killer);
    private readonly PlayerControl Body = PlayerById(player);

    public float KillAge => Time.time - KillTime;

    public string ParseBodyReport(PlayerControl reporter)
    {
        if (!reporter.Is<Coroner>())
            return "";

        var report = $"{Body.name}'s Report:";

        if (reporter.IsFlashed())
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
                    report += killerRole.Handler.CurrentFaction switch
                    {
                        Faction.Crew => "\nThe killer is from the Crew!",
                        Faction.Intruder => "\nThe killer is an Intruder!",
                        Faction.Syndicate => "\nThe killer is from the Syndicate!",
                        Faction.Apocalypse => "\nThe killer is a follower of the Apocalypse!",
                        Faction.Pandorica => "\nThe killer is from the Pandorica!",
                        Faction.Compliance => "\nThe killer is from the Compliance!",
                        Faction.Illuminati => "\nThe killer is from the Illuminati!",
                        _ when killerRole.Handler.CurrentFaction.IsOutcast() => "\nThe killer is an Outcast!",
                        _ => ""
                    };
                }

                report += $"\nThe killer is a {(Killer.Data.DefaultOutfit.ColorId.IsLighter() ? "lighter" : "darker")} color!";
            }
        }

        return report;
    }
}