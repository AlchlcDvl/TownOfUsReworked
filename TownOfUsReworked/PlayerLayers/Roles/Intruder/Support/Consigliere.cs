namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Consigliere)]
public sealed class Consigliere : ISupport
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number InvestigateCd = 25;

    [StringOption<ConsigInfo>]
    public static ConsigInfo ConsigInfo = ConsigInfo.Role;

    public readonly HashSet<byte> Investigated = [];
    private CustomButton InvestigateButton;

    protected override UColor MainColor => CustomColorManager.Consigliere;
    public override Layer Type => Layer.Consigliere;
    public override string StartText => "See The <#8CFFFFFF>Crew</color> For Who They Really Are";
    public override string Description => $"- You can reveal a player's {(ConsigInfo == ConsigInfo.Role ? "role" : "faction")}{(Player.Is<Assassin>() && ConsigInfo == ConsigInfo.Role ? "\n- You cannot assassinate players you have revealed" : "")}\n{CommonAbilities}";

    public override void Init()
    {
        base.Init();
        Investigated.Clear();
        InvestigateButton ??= new(this, new SpriteName("Investigate"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Investigate, new Cooldown(InvestigateCd), "INVESTIGATE",
            (PlayerBodyExclusion)Exception1);
    }

    protected override void OnTrueDeath(bool value)
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

    private bool Exception1(PlayerControl player) => Investigated.Contains(player.PlayerId) || (Handler.CurrentFaction.IsFactionedEvil(true) && player.Is(Handler.CurrentFaction)) ||
        Player.KnowsRoleOf(player);

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (!Investigated.Contains(player.PlayerId) || revealed)
            return;

        revealed = true;
        var revealRole = ConsigInfo == ConsigInfo.Role;
        var role = handler.CurrentRole;
        removeFromConsig = revealRole;
        color = revealRole ? role.Color : role.FactionColor;
        name += revealRole ? $"\n{role}" : $"\n{role.FactionName}";
    }
}