
namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Consigliere)]
public sealed class Consigliere : Intruder
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number InvestigateCd = 25;

    [StringOption<ConsigInfo>]
    public static ConsigInfo ConsigInfo = ConsigInfo.Role;

    public List<byte> Investigated { get; } = [];
    private CustomButton InvestigateButton { get; set; }

    protected override UColor MainColor => CustomColorManager.Consigliere;
    public override LayerEnum Type { get; } = LayerEnum.Consigliere;
    public override Func<string> StartText { get; } = () => "See The <#8CFFFFFF>Crew</color> For Who They Really Are";
    public override Func<string> Description => () => $"- You can reveal a player's {(ConsigInfo == ConsigInfo.Role ? "role" : "faction")}{(Player.Is<Assassin>() && ConsigInfo == ConsigInfo.Role ? "\n- You cannot assassinate players you have revealed" : "")}\n{CommonAbilities}";

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Support;
        Investigated.Clear();
        InvestigateButton ??= new(this, new SpriteName("Investigate"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Investigate, new Cooldown(InvestigateCd), "INVESTIGATE",
            (PlayerBodyExclusion)Exception1);
    }

    protected override void OnTrueDeath()
    {
        if (DeadSeeEverything())
            Investigated.Clear();
    }

    private void Investigate(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            Investigated.Add(target.PlayerId);

        InvestigateButton.StartCooldown(cooldown);
    }

    private bool Exception1(PlayerControl player) => Investigated.Contains(player.PlayerId) || (((Faction is Faction.Intruder or Faction.Syndicate && player.Is(Faction)) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None)) && GameModifiers.FactionSeeRoles) || (Player.IsOtherLover(player) && Lovers.LoversRoles) || (Player.IsOtherRival(player) &&
        Rivals.RivalsRoles) || (player.Is<Mafia>() && Player.Is<Mafia>() && Mafia.MafiaRoles) || (Player.IsOtherLink(player) && Linked.LinkedRoles);

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (!Investigated.Contains(player.PlayerId) || revealed)
            return;

        revealed = true;
        var revealRole = ConsigInfo == ConsigInfo.Role;
        var role = handler.CustomRole;
        removeFromConsig = role.SubFaction == SubFaction && role.SubFaction != SubFaction.None && revealRole;
        color = revealRole ? role.Color : role.FactionColor;
        name += revealRole ? $"\n{role}" : $"\n{role.FactionName}";
    }
}