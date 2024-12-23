namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Syndicate : Role
{
    public CustomButton KillButton { get; set; }
    public string CommonAbilities => "<#008000FF>" + (Type is not LayerEnum.Anarchist and not LayerEnum.Sidekick && Alignment != Alignment.SyndicateKill && HoldsDrive ? ("- You can "
        + "kill players directly") : "- You can kill") + (Player.CanSabotage() ? "\n- You can sabotage the systems to distract the <#8CFFFFFF>Crew</color>" : "") + "</color>";
    public bool HoldsDrive => Player == DriveHolder || (SyndicateSettings.GlobalDrive && SyndicateHasChaosDrive) || GetLayers<PromotedRebel>().Any(x => x.HoldsDrive && IsPromoted);
    public bool IsPromoted;

    public override UColor Color => CustomColorManager.Syndicate;
    public override Faction BaseFaction => Faction.Syndicate;
    public override AttackEnum AttackVal => HoldsDrive ? AttackEnum.Basic : AttackEnum.Basic;

    public static bool SyndicateHasChaosDrive { get; set; }

    private static PlayerControl _driveHolder;
    public static PlayerControl DriveHolder
    {
        get => _driveHolder;
        set
        {
            _driveHolder = value;

            if (!value)
                return;

            var syndicateRole = value.GetLayer<Syndicate>();

            if (!syndicateRole)
                return;

            syndicateRole.OnDriveReceived();

            if (value.AmOwner)
                syndicateRole.OnDriveReceivedLocal();
        }
    }

    public override void Init()
    {
        base.Init();
        Faction = Faction.Syndicate;
        Objectives = () => SyndicateWinCon;
        KillButton ??= new(this, new SpriteName("SyndicateKill"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Kill, new Cooldown(SyndicateSettings.CDKillCd), "KILL",
            (PlayerBodyExclusion)Exception, (UsableFunc)KillUsable, FactionColor);
        IsPromoted = false;
    }

    public override List<PlayerControl> Team()
    {
        var team = base.Team();
        team.AddRange(AllPlayers().Where(x => x != Player && x.Is(Faction)));
        return team;
    }

    public override void OnDeath(DeathReason reason, DeathReasonEnum reason2, PlayerControl killer)
    {
        base.OnDeath(reason, reason2, killer);

        if (Player == DriveHolder)
            AssignChaosDrive();
    }

    public virtual void OnDriveReceivedLocal() {}

    public virtual void OnDriveReceived() {}

    public void Kill(PlayerControl target) => KillButton.StartCooldown(Interact(Player, target, true));

    public bool Exception(PlayerControl player) => (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction != SubFaction.None) ||
        Player.IsLinkedTo(player);

    public bool KillUsable() => (HoldsDrive && Alignment != Alignment.SyndicateKill) || Type is LayerEnum.Anarchist or LayerEnum.Sidekick or LayerEnum.Rebel;
}