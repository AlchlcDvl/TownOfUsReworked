namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Syndicate : Role
{
    private CustomButton KillButton { get; set; }
    public PromotedRebel Promoted { get; set; }
    protected string CommonAbilities => "<#008000FF>" + (this is Anarchist or Sidekick || (Alignment != Alignment.Killing && HoldsDrive) ? ("- You can "
        + "kill players directly") : "") + (Player.CanSabotage() ? "\n- You can sabotage the systems to distract the <#8CFFFFFF>Crew</color>" : "") + "</color>";
    public bool HoldsDrive => Player == DriveHolder || (SyndicateSettings.GlobalDrive && SyndicateHasChaosDrive) || Promoted?.HoldsDrive == true;

    public override UColor Color => CustomColorManager.Syndicate;
    public override AttackEnum AttackVal => HoldsDrive ? AttackEnum.Basic : AttackEnum.None;
    public override float VisionRange => SyndicateSettings.SyndicateVision;
    public override bool CanVent => (HoldsDrive && (int)SyndicateSettings.SyndicateVent is 1) || (int)SyndicateSettings.SyndicateVent is 0;

    public static bool SyndicateHasChaosDrive { get; set; }

    private static PlayerControl DriveHolderPriv;
    public static PlayerControl DriveHolder
    {
        get => DriveHolderPriv;
        set
        {
            if (DriveHolderPriv && DriveHolderPriv.TryGetLayer<Syndicate>(out var syndicate1))
            {
                syndicate1.OnDriveLost();

                if (DriveHolderPriv.AmOwner)
                    syndicate1.OnDriveLostLocal();
            }

            DriveHolderPriv = value;

            if (!value || !value.TryGetLayer<Syndicate>(out var syndicateRole))
                return;

            syndicateRole.OnDriveReceived();

            if (value.AmOwner)
                syndicateRole.OnDriveReceivedLocal();
        }
    }

    protected override void Init()
    {
        base.Init();
        Faction = Faction.Syndicate;
        Objectives = () => SyndicateWinCon;
        KillButton ??= new(this, new SpriteName("SyndicateKill"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Kill, new Cooldown(SyndicateSettings.CdKillCd), "KILL",
            (PlayerBodyExclusion)Exception, (UsableFunc)KillUsable, FactionColor);
        Promoted = null;
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

    protected virtual void OnDriveReceivedLocal() {}

    protected virtual void OnDriveReceived() {}

    protected virtual void OnDriveLostLocal() {}

    protected virtual void OnDriveLost() {}

    private void Kill(PlayerControl target) => KillButton.StartCooldown(Interact(Player, target, true));

    private bool Exception(PlayerControl player) => (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction != SubFaction.None) ||
        Player.IsLinkedTo(player);

    private bool KillUsable() => (HoldsDrive && Alignment != Alignment.Killing) || Type is LayerEnum.Anarchist or LayerEnum.Sidekick or LayerEnum.Rebel;
}