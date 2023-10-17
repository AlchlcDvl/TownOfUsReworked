namespace TownOfUsReworked.PlayerLayers.Roles;

public class Consigliere : Intruder
{
    public List<byte> Investigated { get; set; }
    public CustomButton InvestigateButton { get; set; }
    private static string Option => CustomGameOptions.ConsigInfo == ConsigInfo.Role ? "role" : "faction";

    public override Color Color => ClientGameOptions.CustomIntColors ? Colors.Consigliere : Colors.Intruder;
    public override string Name => "Consigliere";
    public override LayerEnum Type => LayerEnum.Consigliere;
    public override Func<string> StartText => () => "See The <color=#8CFFFFFF>Crew</color> For Who They Really Are";
    public override Func<string> Description => () => $"- You can reveal a player's {Option}\n{CommonAbilities}";
    public override Func<string> Attributes => () => Player.IsAssassin() && CustomGameOptions.ConsigInfo == ConsigInfo.Role ? "\n- You cannot assassinate players you have revealed" : "";

    public Consigliere(PlayerControl player) : base(player)
    {
        Alignment = Alignment.IntruderSupport;
        Investigated = new();
        InvestigateButton = new(this, "Investigate", AbilityTypes.Target, "Secondary", Investigate, CustomGameOptions.InvestigateCd, Exception1);
    }

    public void Investigate()
    {
        var interact = Interact(Player, InvestigateButton.TargetPlayer);

        if (interact.AbilityUsed)
            Investigated.Add(InvestigateButton.TargetPlayer.PlayerId);

        if (interact.Reset)
            InvestigateButton.StartCooldown(CooldownType.Reset);
        else if (interact.Protected)
            InvestigateButton.StartCooldown(CooldownType.GuardianAngel);
    }

    public bool Exception1(PlayerControl player) => Investigated.Contains(player.PlayerId) || (((Faction is Faction.Intruder or Faction.Syndicate && player.Is(Faction)) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None)) && CustomGameOptions.FactionSeeRoles) || (Player.IsOtherLover(player) && CustomGameOptions.LoversRoles) ||
        (Player.IsOtherRival(player) && CustomGameOptions.RivalsRoles) || (player.Is(LayerEnum.Mafia) && Player.Is(LayerEnum.Mafia) && CustomGameOptions.MafiaRoles) ||
        (Player.IsOtherLink(player) && CustomGameOptions.LinkedRoles);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        InvestigateButton.Update2("INVESTIGATE");
    }
}