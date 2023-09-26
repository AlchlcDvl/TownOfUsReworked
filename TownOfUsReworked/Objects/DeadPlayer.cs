namespace TownOfUsReworked.Objects;

public class DeadPlayer
{
    public readonly byte KillerId;
    public readonly byte PlayerId;
    public readonly DateTime KillTime;

    public PlayerControl Killer => PlayerById(KillerId);
    public PlayerControl Body => PlayerById(PlayerId);

    public PlayerControl Reporter { get; set; }
    public float KillAge { get; set; }

    public DeadPlayer(byte killer, byte player)
    {
        PlayerId = player;
        KillerId = killer;
        KillTime = DateTime.UtcNow;
    }

    public string ParseBodyReport()
    {
        var report = $"{Body.name}'s Report:\n";
        var killerRole = Role.GetRole(Killer);
        var bodyRole = Role.GetRole(Body);

        if (!Reporter.IsFlashed())
        {
            report += $"They died approximately {Math.Round(KillAge / 1000f)}s ago!\n";
            report += $"They were a {bodyRole.Name}!\n";

            if (Body == Killer)
                report += "There is evidence of self-harm!";
            else
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

                report += $"The killer is a {CustomColors.LightDarkColors[Killer.CurrentOutfit.ColorId].ToLower()} color!\n";

                if (CustomGameOptions.CoronerReportName && CustomGameOptions.CoronerKillerNameTime <= Math.Round(KillAge / 1000))
                    report += $"They were killed by {Killer.name}!";
            }
        }
        else
            report += "You have been blinded so you cannot tell what happened to the body!";

        return report;
    }
}