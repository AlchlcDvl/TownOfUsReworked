namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Syndicate : Role
{
    public CustomButton KillButton { get; set; }
    public string CommonAbilities => "<color=#008000FF>" + (Type is not LayerEnum.Anarchist and not LayerEnum.Sidekick && Alignment != Alignment.SyndicateKill && HoldsDrive ? ("- You can "
        + "kill players directly") : "- You can kill") + (Player.CanSabotage() ? "\n- You can sabotage the systems to distract the <color=#8CFFFFFF>Crew</color>" : "") + "</color>";
    public bool HoldsDrive => Player == DriveHolder || (CustomGameOptions.GlobalDrive && SyndicateHasChaosDrive) || GetLayers<PromotedRebel>().Any(x => x.HoldsDrive && IsPromoted);
    public bool IsPromoted;

    public override Color Color => Colors.Syndicate;
    public override Faction BaseFaction => Faction.Syndicate;

    protected Syndicate(PlayerControl player) : base(player)
    {
        Faction = Faction.Syndicate;
        FactionColor = Colors.Syndicate;
        Objectives = () => SyndicateWinCon;
        KillButton = new(this, "SyndicateKill", AbilityTypes.Target, "ActionSecondary", Kill, CustomGameOptions.CDKillCd, Exception);
        Data.SetImpostor(true);
        IsPromoted = false;
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

        if (interact.AbilityUsed || interact.Reset)
            KillButton.StartCooldown();
        else if (interact.Protected)
            KillButton.StartCooldown(CooldownType.GuardianAngel);
        else if (interact.Vested)
            KillButton.StartCooldown(CooldownType.Survivor);
    }

    public bool Exception(PlayerControl player) => (player.Is(Faction) && Faction != Faction.Crew) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || Player.IsLinkedTo(player);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        KillButton.Update2("KILL", (HoldsDrive && Alignment != Alignment.SyndicateKill) || Type is LayerEnum.Anarchist or LayerEnum.Sidekick or LayerEnum.Rebel);
    }
}