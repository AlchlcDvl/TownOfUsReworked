namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Intruder : Role
{
    public CustomButton KillButton { get; set; }
    public string CommonAbilities => "<color=#FF0000FF>- You can kill players" + (CustomGameOptions.IntrudersCanSabotage || (IsDead && CustomGameOptions.GhostsCanSabotage) ? ("\n- You can " +
        "call sabotages to distract the <color=#8CFFFFFF>Crew</color>") : "") + "</color>";

    public override Color Color => Colors.Intruder;
    public override Faction BaseFaction => Faction.Intruder;

    protected Intruder(PlayerControl player) : base(player)
    {
        Faction = Faction.Intruder;
        FactionColor = Colors.Intruder;
        Objectives = () => IntrudersWinCon;
        KillButton = new(this, "IntruderKill", AbilityTypes.Target, "ActionSecondary", Kill, CustomGameOptions.IntKillCd, Exception);
        Player.Data.SetImpostor(true);
    }

    public override List<PlayerControl> Team()
    {
        var team = base.Team();

        if (IsRecruit)
        {
            var jackal = Player.GetJackal();
            team.Add(jackal.Player);
            team.Add(jackal.GoodRecruit);
        }

        foreach (var player in CustomPlayer.AllPlayers)
        {
            if (player.Is(Faction) && player != Player)
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

        return team;
    }

    public void Kill()
    {
        var interact = Interact(Player, KillButton.TargetPlayer, true);

        if (Player.Is(LayerEnum.Janitor))
        {
            var jani = (Janitor)this;

            if (CustomGameOptions.JaniCooldownsLinked)
            {
                if (interact.AbilityUsed || interact.Reset)
                    jani.CleanButton.StartCooldown(CooldownType.Start);
                else if (interact.Protected)
                    jani.CleanButton.StartCooldown(CooldownType.GuardianAngel);
                else if (interact.Vested)
                    jani.CleanButton.StartCooldown(CooldownType.Survivor);
            }
        }
        else if (Player.Is(LayerEnum.PromotedGodfather))
        {
            var gf = (PromotedGodfather)this;

            if (CustomGameOptions.JaniCooldownsLinked && gf.IsJani)
            {
                if (interact.AbilityUsed || interact.Reset)
                    gf.CleanButton.StartCooldown(CooldownType.Start);
                else if (interact.Protected)
                    gf.CleanButton.StartCooldown(CooldownType.GuardianAngel);
                else if (interact.Vested)
                    gf.CleanButton.StartCooldown(CooldownType.Survivor);
            }
        }

        if (interact.AbilityUsed || interact.Reset)
            KillButton.StartCooldown(CooldownType.Reset);
        else if (interact.Protected)
            KillButton.StartCooldown(CooldownType.GuardianAngel);
        else if (interact.Vested)
            KillButton.StartCooldown(CooldownType.Survivor);
    }

    public bool Exception(PlayerControl player) =>  (player.Is(Faction) && Faction != Faction.Crew) || (player.Is(SubFaction) && SubFaction != SubFaction.None) ||  Player.IsLinkedTo(player);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        KillButton.Update2("KILL");
    }
}