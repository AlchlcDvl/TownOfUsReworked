namespace TownOfUsReworked.PlayerLayers.Roles;

public class Consigliere : Intruder
{
    public List<byte> Investigated { get; set; }
    public CustomButton InvestigateButton { get; set; }
    public DateTime LastInvestigated { get; set; }
    private static string Option => CustomGameOptions.ConsigInfo == ConsigInfo.Role ? "role" : "faction";
    private string CanAssassinate => Player.IsAssassin() && CustomGameOptions.ConsigInfo == ConsigInfo.Role ? ("\n- You cannot assassinate players " +
        "you have revealed") : "";

    public override Color Color => ClientGameOptions.CustomIntColors ? Colors.Consigliere : Colors.Intruder;
    public override string Name => "Consigliere";
    public override LayerEnum Type => LayerEnum.Consigliere;
    public override Func<string> StartText => () => "See The <color=#8CFFFFFF>Crew</color> For Who They Really Are";
    public override Func<string> Description => () => $"- You can reveal a player's {Option}{CanAssassinate}\n{CommonAbilities}";
    public override InspectorResults InspectorResults => InspectorResults.GainsInfo;
    public float Timer => ButtonUtils.Timer(Player, LastInvestigated, CustomGameOptions.InvestigateCd);

    public Consigliere(PlayerControl player) : base(player)
    {
        Alignment = Alignment.IntruderSupport;
        Investigated = new();
        InvestigateButton = new(this, "Investigate", AbilityTypes.Direct, "Secondary", Investigate, Exception1);
    }

    public void Investigate()
    {
        if (Timer != 0f || IsTooFar(Player, InvestigateButton.TargetPlayer) || Investigated.Contains(InvestigateButton.TargetPlayer.PlayerId))
            return;

        var interact = Interact(Player, InvestigateButton.TargetPlayer);

        if (interact.AbilityUsed)
            Investigated.Add(InvestigateButton.TargetPlayer.PlayerId);

        if (interact.Reset)
            LastInvestigated = DateTime.UtcNow;
        else if (interact.Protected)
            LastInvestigated.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    public bool Exception1(PlayerControl player) => Investigated.Contains(player.PlayerId) || (((Faction is Faction.Intruder or Faction.Syndicate && player.Is(Faction)) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None)) && CustomGameOptions.FactionSeeRoles) || (Player.IsOtherLover(player) && CustomGameOptions.LoversRoles) ||
        (Player.IsOtherRival(player) && CustomGameOptions.RivalsRoles) || (player.Is(LayerEnum.Mafia) && Player.Is(LayerEnum.Mafia) && CustomGameOptions.MafiaRoles) ||
        (Player.IsOtherLink(player) && CustomGameOptions.LinkedRoles);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        InvestigateButton.Update("INVESTIGATE", Timer, CustomGameOptions.InvestigateCd);
    }
}