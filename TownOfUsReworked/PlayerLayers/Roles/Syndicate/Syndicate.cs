namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Syndicate : Role, IPromoter
{
    private CustomButton KillButton;
    public Rebel Promoter;
    public bool IsUnderling { get; set; }
    public bool IsPromoted { get; set; }
    public Layer UnderlingType => Layer.Sidekick;
    public Layer PromoterType => Layer.Rebel;
    public float PromotionModifier { get; } = Rebel.RebPromotionCdDecrease;
    protected string CommonAbilities => "<#008000FF>" + (KillUsable() ? "- You can kill players directly" : "") + (Player.CanSabotage() ? "\n- You can sabotage the systems to distract the <#8CFFFFFF>Crew</color>" : "") + "</color>";
    public bool HoldsDrive => Player == DriveHolder || (SyndicateSettings.GlobalDrive && SyndicateHasChaosDrive);

    protected override UColor MainColor => CustomColorManager.Syndicate;
    protected override UColor LayerColor => CustomColorManager.Syndicate;
    public override Attack Attack => KillUsable() ? Attack.Basic : Attack.None;
    public override bool AffectedByLights => false;
    public override bool CanVent
    {
        get
        {
            var part = Handler.CurrentFaction switch
            {
                Faction.Pandorica => PandoricaSettings.PandoricaVent,
                Faction.Illuminati => IlluminatiSettings.IlluminatiVent,
                _ => true
            };
            return ((HoldsDrive && (int)SyndicateSettings.SyndicateVent is 1) || (int)SyndicateSettings.SyndicateVent is 0) && part;
        }
    }
    protected override bool UseMainColor => ClientOptions.CustomSynColors;
    public override Faction BaseFaction => BadGuysSettings.IlluminatiUnleashed && BadGuysSettings.IlluminatiMembers == IlluminatiType.Syndicate
        ? Faction.Illuminati
        : (BadGuysSettings.PandoricaOpens && BadGuysSettings.PandoricaMembers == PandoricaType.Syndicate
            ? Faction.Pandorica : Faction.Syndicate);
    public override Alignment Alignment => IsPromoted
        ? Alignment.Head
        : (IsUnderling
            ? Alignment.Utility
            : ActualAlignment);

    protected abstract Alignment ActualAlignment { get; }

    public static bool SyndicateHasChaosDrive { get; set; }

    public static PlayerControl DriveHolder
    {
        get;
        set
        {
            if (field?.Is<Syndicate>(out var syndicate1) == true)
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

    public override void Init()
    {
        base.Init();
        KillButton ??= new(this, (SpriteFunc)GetSpriteName, AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Kill, new Cooldown(SyndicateSettings.CdKillCd), "KILL",
            (PlayerBodyExclusion)Exception, (UsableFunc)KillUsable, FactionColor);
    }

    private string GetSpriteName() => $"{Handler.CurrentFaction}Kill";

    public override List<PlayerControl> Team()
    {
        var team = base.Team();
        team.AddRange(AllPlayers().Where(x => x != Player && x.Is(Handler.CurrentFaction)));
        return team;
    }

    public override void OnDeath(DeathReasonEnum reason, PlayerControl killer)
    {
        if (Player == DriveHolder)
            AssignChaosDrive();
    }

    public override void UpdatePlayer()
    {
        base.UpdatePlayer();

        if (IsPromoted || !IsUnderling || (!(Promoter?.Dead ?? false)))
            return;

        IsPromoted = true;
        IsUnderling = false;
        Promoter = null;
        Name = TranslationManager.Translate("Layer.Rebel");
        Handler.History.Add((Layer.Sidekick, Handler.CurrentFaction));
    }

    protected virtual void OnDriveReceivedLocal() {}

    protected virtual void OnDriveReceived() {}

    protected virtual void OnDriveLostLocal() {}

    protected virtual void OnDriveLost() {}

    private void Kill(PlayerControl target) => KillButton.StartCooldown(Interact(Player, target, true));

    private bool Exception(PlayerControl player) => Player.IsBuddyWith(player, Handler.CurrentFaction);

    private bool KillUsable() => ((HoldsDrive && Alignment != Alignment.Killing) || Type is Layer.Anarchist || IsPromoted) && !IsUnderling;
}