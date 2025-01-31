namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Ghoul : Intruder, IGhosty
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number GhoulMarkCd = 25;

    public CustomButton MarkButton { get; set; }
    public bool Caught { get; set; }
    public bool Faded { get; set; }
    public PlayerControl MarkedPlayer { get; set; }

    public override UColor Color => ClientOptions.CustomIntColors ? CustomColorManager.Ghoul : FactionColor;
    public override LayerEnum Type => LayerEnum.Ghoul;
    public override Func<string> StartText => () => "BOO!";
    public override Func<string> Description => () => "- You can mark a player for death every round\n- Marked players will be announced to all players and will die at the end of the next" +
        " meeting if you are not clicked";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Utility;
        MarkedPlayer = null;
        MarkButton ??= new(this, new SpriteName("GhoulMark"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Mark, new Cooldown(GhoulMarkCd), "MARK", new PostDeath(true),
            (PlayerBodyExclusion)Exception1, (UsableFunc)Usable);
        Player.gameObject.layer = LayerMask.NameToLayer("Players");
    }

    public void Mark(PlayerControl target)
    {
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, MarkedPlayer);
        MarkButton.StartCooldown();
    }

    public bool Usable() => !Caught && !MarkedPlayer;

    public bool Exception1(PlayerControl player) => player == MarkedPlayer || player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None);

    public override void ReadRPC(MessageReader reader) => MarkedPlayer = reader.ReadPlayer();

    public override void UpdatePlayer() => (this as IGhosty).UpdateGhost();
}