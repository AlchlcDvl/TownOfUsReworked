namespace TownOfUsReworked.PlayerLayers.Roles;

public class Intruder : Role
{
    public DateTime LastKilled { get; set; }
    public CustomButton KillButton { get; set; }
    public string CommonAbilities => "- You can kill players" + (CustomGameOptions.IntrudersCanSabotage || (IsDead && CustomGameOptions.GhostsCanSabotage) ? ("\n- You can call " +
        "sabotages to distract the <color=#8CFFFFFF>Crew</color>") : "");

    public override Color Color => Colors.Intruder;
    public override Faction BaseFaction => Faction.Intruder;
    public float KillTimer => ButtonUtils.Timer(Player, LastKilled, CustomGameOptions.IntKillCd);

    protected Intruder(PlayerControl player) : base(player)
    {
        Faction = Faction.Intruder;
        FactionColor = Colors.Intruder;
        Objectives = () => IntrudersWinCon;
        KillButton = new(this, "IntruderKill", AbilityTypes.Direct, "ActionSecondary", Kill, Exception);
        Player.Data.SetImpostor(true);
    }

    public override void IntroPrefix(IntroCutscene._ShowTeam_d__36 __instance)
    {
        if (!Local)
            return;

        var team = new List<PlayerControl> { CustomPlayer.Local };

        if (IsRecruit)
        {
            var jackal = Player.GetJackal();
            team.Add(jackal.Player);
            team.Add(jackal.GoodRecruit);
        }

        foreach (var player in CustomPlayer.AllPlayers)
        {
            if (player.Is(Faction) && player != CustomPlayer.Local)
                team.Add(player);
        }

        if (Player.Is(LayerEnum.Lovers))
            team.Add(Player.GetOtherLover());
        else if (Player.Is(LayerEnum.Rivals))
            team.Add(Player.GetOtherRival());
        else if (Player.Is(LayerEnum.Mafia))
        {
            foreach (var player in CustomPlayer.AllPlayers)
            {
                if (player != Player && player.Is(LayerEnum.Mafia))
                    team.Add(player);
            }
        }

        __instance.teamToShow = team.SystemToIl2Cpp();
    }

    public void Kill()
    {
        if (IsTooFar(Player, KillButton.TargetPlayer) || KillTimer != 0f)
            return;

        var interact = Interact(Player, KillButton.TargetPlayer, true);

        if (Player.Is(LayerEnum.Janitor))
        {
            var jani = (Janitor)this;

            if (interact.AbilityUsed || interact.Reset)
            {
                if (CustomGameOptions.JaniCooldownsLinked)
                    jani.LastCleaned = DateTime.UtcNow;
            }
            else if (interact.Protected)
            {
                if (CustomGameOptions.JaniCooldownsLinked)
                    jani.LastCleaned.AddSeconds(CustomGameOptions.ProtectKCReset);
            }
            else if (interact.Vested)
            {
                if (CustomGameOptions.JaniCooldownsLinked)
                    jani.LastCleaned.AddSeconds(CustomGameOptions.VestKCReset);
            }
        }
        else if (Player.Is(LayerEnum.PromotedGodfather))
        {
            var gf = (PromotedGodfather)this;

            if (interact.AbilityUsed || interact.Reset)
            {
                if (CustomGameOptions.JaniCooldownsLinked && gf.IsJani)
                    gf.LastCleaned = DateTime.UtcNow;
            }
            else if (interact.Protected)
            {
                if (CustomGameOptions.JaniCooldownsLinked && gf.IsJani)
                    gf.LastCleaned.AddSeconds(CustomGameOptions.ProtectKCReset);
            }
            else if (interact.Vested)
            {
                if (CustomGameOptions.JaniCooldownsLinked && gf.IsJani)
                    gf.LastCleaned.AddSeconds(CustomGameOptions.VestKCReset);
            }
        }

        if (interact.AbilityUsed || interact.Reset)
            LastKilled = DateTime.UtcNow;
        else if (interact.Protected)
            LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
        else if (interact.Vested)
            LastKilled.AddSeconds(CustomGameOptions.VestKCReset);
    }

    public bool Exception(PlayerControl player) =>  (player.Is(Faction) && Faction != Faction.Crew) || (player.Is(SubFaction) && SubFaction != SubFaction.None) ||
        Player.IsLinkedTo(player);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        KillButton.Update("KILL", KillTimer, CustomGameOptions.IntKillCd);
    }
}