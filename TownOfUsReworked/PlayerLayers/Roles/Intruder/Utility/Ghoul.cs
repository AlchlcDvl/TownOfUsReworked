namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Ghoul)]
public sealed class Ghoul : IUtility, IGhosty
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number GhoulMarkCd = 25;

    private CustomButton MarkButton;
    public PlayerControl MarkedPlayer;

    public bool Caught { get; set; }
    public Vector3 LastPosition { get; set; }

    protected override UColor MainColor => CustomColorManager.Ghoul;
    public override Layer Type => Layer.Ghoul;
    public override string StartText => "BOO!";
    public override string Description => "- You can mark a player for death every round\n- Marked players will be announced to all players and will die at the end of the next" +
        " meeting if you are not clicked";

    public override void Init()
    {
        base.Init();
        MarkedPlayer = null;
        MarkButton ??= new(this, new SpriteName("GhoulMark"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Mark, new Cooldown(GhoulMarkCd), "MARK", new PostDeath(true),
            (PlayerBodyExclusion)Exception1, (UsableFunc)Usable);
        Player.gameObject.layer = LayerMask.NameToLayer("Players");
    }

    public override void BeforeMeeting()
    {
        if (!UninteractablePlayers.ContainsKey(PlayerId))
            LastPosition = Player.transform.position;
    }

    private void Mark(PlayerControl target)
    {
        CallRpc(ActionsRpc.LayerAction, this, MarkedPlayer);
        MarkButton.StartCooldown();
    }

    private bool Usable() => !Caught && !MarkedPlayer;

    private bool Exception1(PlayerControl player) => player == MarkedPlayer || Player.IsBuddyWith(player, Faction);

    public override void ReadRPC(RpcReader reader) => MarkedPlayer = reader.ReadPlayer();

    public bool CanBeClicked(PlayerControl clicker) => !clicker.Is(Faction);
}