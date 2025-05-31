namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Syndicate : Role, IPromoter
{
    private CustomButton KillButton { get; set; }
    public Rebel Promoter { get; set; }
    public bool IsUnderling { get; set; }
    public bool IsPromoted { get; set; }
    public LayerEnum UnderlingType { get; } = LayerEnum.Sidekick;
    public LayerEnum PromoterType { get; } = LayerEnum.Rebel;
    public float PromotionModifier { get; } = Rebel.RebPromotionCdDecrease;
    protected string CommonAbilities => "<#008000FF>" + (KillUsable() ? "- You can kill players directly" : "") + (Player.CanSabotage() ? "\n- You can sabotage the systems to distract the <#8CFFFFFF>Crew</color>" : "") + "</color>";
    public bool HoldsDrive => Player == DriveHolder || (SyndicateSettings.GlobalDrive && SyndicateHasChaosDrive);

    protected override UColor MainColor => CustomColorManager.Syndicate;
    protected override UColor LayerColor => CustomColorManager.Syndicate;
    public override AttackEnum AttackVal => KillUsable() ? AttackEnum.Basic : AttackEnum.None;
    public override bool AffectedByLights => false;
    public override bool CanVent
    {
        get
        {
            var part = Faction switch
            {
                Faction.Pandorica => PandoricaSettings.PandoricaVent,
                Faction.Illuminati => IlluminatiSettings.IlluminatiVent,
                _ => true
            };
            return ((HoldsDrive && (int)SyndicateSettings.SyndicateVent is 1) || (int)SyndicateSettings.SyndicateVent is 0) && part;
        }
    }
    protected override bool UseMainColor => ClientOptions.CustomSynColors;

    public static bool SyndicateHasChaosDrive { get; set; }

    public static PlayerControl DriveHolder
    {
        get;
        set
        {
            if (field && field.Is<Syndicate>(out var syndicate1))
            {
                syndicate1.OnDriveLost();

                if (field.AmOwner || TownOfUsReworked.MciActive)
                    syndicate1.OnDriveLostLocal();
            }

            field = value;

            if (!value || !value.Is<Syndicate>(out var syndicateRole))
                return;

            syndicateRole.OnDriveReceived();

            if (value.AmOwner || TownOfUsReworked.MciActive)
                syndicateRole.OnDriveReceivedLocal();
        }
    }

    protected override void Init()
    {
        base.Init();
        Faction = BadGuysSettings.IlluminatiUnleashed && BadGuysSettings.IlluminatiMembers == IlluminatiType.Syndicate
            ? Faction.Illuminati
            : (BadGuysSettings.PandoricaOpens && BadGuysSettings.PandoricaMembers == PandoricaType.Syndicate
                ? Faction.Pandorica : Faction.Syndicate);
        KillButton ??= new(this, new SpriteName($"{Faction}Kill"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Kill, new Cooldown(SyndicateSettings.CdKillCd), "KILL",
            (PlayerBodyExclusion)Exception, (UsableFunc)KillUsable, FactionColor);
    }

    public override List<PlayerControl> Team()
    {
        var team = base.Team();
        team.AddRange(AllPlayers().Where(x => x != Player && x.Is(Faction)));
        return team;
    }

    public override void OnDeath(DeathReasonEnum reason, PlayerControl killer)
    {
        base.OnDeath(reason, killer);

        if (Player == DriveHolder)
            AssignChaosDrive();
    }

    public override void UpdatePlayer()
    {
        base.UpdatePlayer();

        if (!IsPromoted && IsUnderling && (Promoter?.Dead ?? false))
        {
            IsPromoted = true;
            IsUnderling = false;
            Promoter = null;
            Name = TranslationManager.Translate("Layer.Rebel");
            Alignment = Alignment.Power;
            RoleHistory.Add(LayerEnum.Sidekick);
        }
    }

    protected virtual void OnDriveReceivedLocal() {}

    protected virtual void OnDriveReceived() {}

    protected virtual void OnDriveLostLocal() {}

    protected virtual void OnDriveLost() {}

    private void Kill(PlayerControl target) => KillButton.StartCooldown(Interact(Player, target, true));

    private bool Exception(PlayerControl player) => (player.Is(Faction) && Faction is not (Faction.Crew or Faction.Neutral)) || (player.Is(SubFaction) && SubFaction != SubFaction.None) ||
        Player.IsLinkedTo(player);

    private bool KillUsable() => ((HoldsDrive && Alignment != Alignment.Killing) || Type is LayerEnum.Anarchist || IsPromoted) && !IsUnderling;
}