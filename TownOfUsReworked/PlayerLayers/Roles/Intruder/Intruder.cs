namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Intruder : Role, IPromoter
{
    public bool IsUnderling { get; set; }
    public bool IsPromoted { get; set; }
    public Layer UnderlingType => Layer.Mafioso;
    public Layer PromoterType => Layer.Godfather;
    public float PromotionModifier { get; } = Godfather.GfPromotionCdDecrease;

    protected CustomButton KillButton;
    public Godfather Promoter;

    protected string CommonAbilities => $"<#{FactionColor.ToHtmlStringRGBA()}>- You can kill players</color>" + (Player.CanSabotage() ?
        "\n- You can call sabotages to distract the <#8CFFFFFF>Crew</color>" : "");

    protected override UColor MainColor => CustomColorManager.Intruder;
    protected override UColor LayerColor => CustomColorManager.Intruder;
    public override Attack Attack => Attack.Basic;
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
            return IntruderSettings.IntrudersVent && part;
        }
    }
    protected override bool UseMainColor => ClientOptions.CustomIntColors;
    public override Faction BaseFaction => BadGuysSettings.IlluminatiUnleashed && BadGuysSettings.IlluminatiMembers == IlluminatiType.Intruders
        ? Faction.Illuminati
        : (BadGuysSettings.PandoricaOpens && BadGuysSettings.PandoricaMembers == PandoricaType.Intruders
            ? Faction.Pandorica : Faction.Intruder);
    public override Alignment Alignment => IsPromoted
        ? Alignment.Head
        : (IsUnderling
            ? Alignment.Utility
            : ActualAlignment);

    protected abstract Alignment ActualAlignment { get; }

    public override void Init()
    {
        base.Init();
        KillButton ??= new(this, (SpriteFunc)GetKillSprite, AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Kill, new Cooldown(IntruderSettings.IntKillCd), "KILL",
            (PlayerBodyExclusion)Exception, FactionColor, (UsableFunc)KillUsable);
    }

    private string GetKillSprite() => $"{Handler.CurrentFaction}Kill";

    public override List<PlayerControl> Team()
    {
        var team = base.Team();
        team.AddRange(AllPlayers().Where(x => x != Player && x.Is(Handler.CurrentFaction)));
        return team;
    }

    public override void UpdatePlayer()
    {
        base.UpdatePlayer();

        if (IsPromoted || !IsUnderling || (!(Promoter?.Dead ?? false)))
            return;

        IsPromoted = true;
        IsUnderling = false;
        Promoter = null;
        Name = TranslationManager.Translate("Layer.Godfather");
        Handler.History.Add((Layer.Mafioso, Handler.CurrentFaction));
    }

    protected virtual void Kill(PlayerControl target) => KillButton.StartCooldown(Interact(Player, target, true));

    private bool Exception(PlayerControl player) => Player.IsBuddyWith(player, Handler.CurrentFaction);

    private bool KillUsable() => !IsUnderling;
}