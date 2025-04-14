namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Intruder : Role, IPromoter
{
    protected CustomButton KillButton { get; private set; }
    public bool IsUnderling { get; set; }
    public bool IsPromoted { get; set; }
    public Godfather Promoter { get; set; }
    public LayerEnum UnderlingType { get; } = LayerEnum.Mafioso;
    public LayerEnum PromoterType { get; } = LayerEnum.Godfather;
    public float PromotionModifier { get; } = Godfather.GfPromotionCdDecrease;

    protected string CommonAbilities => "<#FF1919FF>- You can kill players</color>" + (Player.CanSabotage() ? "\n- You can call sabotages to distract the <#8CFFFFFF>Crew</color>" : "");

    protected override UColor MainColor => CustomColorManager.Intruder;
    public override AttackEnum AttackVal => AttackEnum.Basic;
    public override bool AffectedByLights => false;
    public override float VisionRange => IntruderSettings.IntruderVision;
    public override bool CanVent => IntruderSettings.IntrudersVent;
    protected override bool UseMainColor => ClientOptions.CustomIntColors;

    protected override void Init()
    {
        base.Init();
        Faction = GameModifiers.IlluminatiUnleashed ? Faction.Illuminati : (GameModifiers.PandoricaOpens ? Faction.Pandorica : Faction.Intruder);
        KillButton ??= new(this, new SpriteName("IntruderKill"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Kill, new Cooldown(IntruderSettings.IntKillCd), "KILL",
            (PlayerBodyExclusion)Exception, FactionColor, (UsableFunc)KillUsable);
    }

    public override List<PlayerControl> Team()
    {
        var team = base.Team();
        team.AddRange(AllPlayers().Where(x => x != Player && x.Is(Faction)));
        return team;
    }

    public override void UpdatePlayer()
    {
        base.UpdatePlayer();

        if (!IsPromoted && IsUnderling && (Promoter?.Dead ?? false))
        {
            IsPromoted = true;
            IsUnderling = false;
            Promoter = null;
            Name = TranslationManager.Translate("Layer.Godfather");
            Alignment = Alignment.Head;
            RoleHistory.Add(LayerEnum.Mafioso);
        }
    }

    protected virtual void Kill(PlayerControl target) => KillButton.StartCooldown(Interact(Player, target, true));

    private bool Exception(PlayerControl player) => (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) && SubFaction != SubFaction.None) ||
        Player.IsLinkedTo(player);

    private bool KillUsable() => !IsUnderling;
}