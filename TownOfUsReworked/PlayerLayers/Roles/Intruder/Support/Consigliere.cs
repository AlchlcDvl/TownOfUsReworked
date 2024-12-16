namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Consigliere : Intruder
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number InvestigateCd { get; set; } = new(25);

    [StringOption(MultiMenu.LayerSubOptions)]
    public static ConsigInfo ConsigInfo { get; set; } = ConsigInfo.Role;

    public List<byte> Investigated { get; set; }
    public CustomButton InvestigateButton { get; set; }

    public override UColor Color => ClientOptions.CustomIntColors ? CustomColorManager.Consigliere: FactionColor;
    public override string Name => "Consigliere";
    public override LayerEnum Type => LayerEnum.Consigliere;
    public override Func<string> StartText => () => "See The <#8CFFFFFF>Crew</color> For Who They Really Are";
    public override Func<string> Description => () => $"- You can reveal a player's {(ConsigInfo == ConsigInfo.Role ? "role" : "faction")}\n{CommonAbilities}";
    public override Func<string> Attributes => () => Player.IsAssassin() && ConsigInfo == ConsigInfo.Role ? "\n- You cannot assassinate players you have revealed" : "";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.IntruderSupport;
        Investigated = [];
        InvestigateButton ??= new(this, new SpriteName("Investigate"), AbilityTypes.Alive, KeybindType.Secondary, (OnClickPlayer)Investigate, new Cooldown(InvestigateCd), "INVESTIGATE",
            (PlayerBodyExclusion)Exception1);
    }

    public void Investigate(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            Investigated.Add(target.PlayerId);

        InvestigateButton.StartCooldown(cooldown);
    }

    public bool Exception1(PlayerControl player) => Investigated.Contains(player.PlayerId) || (((Faction is Faction.Intruder or Faction.Syndicate && player.Is(Faction)) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None)) && GameModifiers.FactionSeeRoles) || (Player.IsOtherLover(player) && Lovers.LoversRoles) || (Player.IsOtherRival(player) &&
        Rivals.RivalsRoles) || (player.Is(LayerEnum.Mafia) && Player.Is(LayerEnum.Mafia) && Mafia.MafiaRoles) || (Player.IsOtherLink(player) && Linked.LinkedRoles);
}