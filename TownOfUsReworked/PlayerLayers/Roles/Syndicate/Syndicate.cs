namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Syndicate : Role
{
    public CustomButton KillButton { get; set; }
    public string CommonAbilities => "<color=#008000FF>" + (Type is not LayerEnum.Anarchist and not LayerEnum.Sidekick && Alignment != Alignment.SyndicateKill && HoldsDrive ? ("- You can "
        + "kill players directly") : "- You can kill") + (Player.CanSabotage() ? "\n- You can sabotage the systems to distract the <color=#8CFFFFFF>Crew</color>" : "") + "</color>";
    public bool HoldsDrive => Player == DriveHolder || (SyndicateSettings.GlobalDrive && SyndicateHasChaosDrive) || GetLayers<PromotedRebel>().Any(x => x.HoldsDrive && IsPromoted);
    public bool IsPromoted;

    public override UColor Color => CustomColorManager.Syndicate;
    public override Faction BaseFaction => Faction.Syndicate;
    public override AttackEnum AttackVal => HoldsDrive ? AttackEnum.Basic : AttackEnum.Basic;

    public override void Init()
    {
        base.Init();
        Faction = Faction.Syndicate;
        Objectives = () => SyndicateWinCon;
        KillButton ??= new(this, new SpriteName("SyndicateKill"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Kill, new Cooldown(SyndicateSettings.CDKillCd), "KILL",
            (PlayerBodyExclusion)Exception, (UsableFunc)KillUsable, FactionColor);
        IsPromoted = false;
    }

    public override List<PlayerControl> Team()
    {
        var team = base.Team();

        if (IsRecruit)
        {
            var jackal = Player.GetJackal();
            team.Add(jackal.Player);
            team.AddRange(jackal.GetOtherRecruits(Player));
        }

        foreach (var player in AllPlayers())
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
            foreach (var player in AllPlayers())
            {
                if (player != Player && player.Is(LayerEnum.Mafia))
                    team.Add(player);
            }
        }

        return team;
    }

    public void Kill() => KillButton.StartCooldown(Interact(Player, KillButton.GetTarget<PlayerControl>(), true));

    public bool Exception(PlayerControl player) => (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction != SubFaction.None) ||
        Player.IsLinkedTo(player);

    public bool KillUsable() => (HoldsDrive && Alignment != Alignment.SyndicateKill) || Type is LayerEnum.Anarchist or LayerEnum.Sidekick or LayerEnum.Rebel;
}