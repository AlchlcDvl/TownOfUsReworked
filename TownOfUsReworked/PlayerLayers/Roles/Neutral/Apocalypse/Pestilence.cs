namespace TownOfUsReworked.PlayerLayers.Roles;

public class Pestilence : Neutral
{
    private CustomButton ObliterateButton { get; set; }

    public override UColor Color => ClientGameOptions.CustomNeutColors ? CustomColorManager.Pestilence : CustomColorManager.Neutral;
    public override string Name => "Pestilence";
    public override LayerEnum Type => LayerEnum.Pestilence;
    public override Func<string> StartText => () => "THE APOCALYPSE IS NIGH";
    public override Func<string> Description => () => "- You can spread a deadly disease to kill everyone";
    public override DefenseEnum DefenseVal => DefenseEnum.Invincible;

    public static readonly Dictionary<byte, int> Infected = new();

    public Pestilence() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        BaseStart();
        Objectives = () => "- Obliterate anyone who can oppose you";
        Alignment = Alignment.NeutralApoc;
        ObliterateButton = new(this, "Obliterate", AbilityTypes.Alive, "ActionSecondary", Obliterate, CustomGameOptions.ObliterateCd, Exception);
        return this;
    }

    private void Obliterate()
    {
        Interact(Player, ObliterateButton.TargetPlayer);
        ObliterateButton.StartCooldown();
    }

    private bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        Player.IsLinkedTo(player);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        ObliterateButton.Update2("OBLITERATE");
    }
}