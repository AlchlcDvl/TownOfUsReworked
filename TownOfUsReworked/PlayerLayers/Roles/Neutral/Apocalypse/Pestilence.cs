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

    public static readonly Dictionary<byte, int> Infected = [];

    public override void Init()
    {
        BaseStart();
        Objectives = () => "- Obliterate anyone who can oppose you";
        Alignment = Alignment.NeutralApoc;
        ObliterateButton = CreateButton(this, new SpriteName("Obliterate"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Obliterate, (PlayerBodyExclusion)Exception, "OBLITERATE",
            new Cooldown(CustomGameOptions.ObliterateCd));
    }

    private void Obliterate()
    {
        Interact(Player, ObliterateButton.TargetPlayer);
        ObliterateButton.StartCooldown();
    }

    private bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        Player.IsLinkedTo(player);
}