namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Ghoul)]
public sealed class Ghoul : Intruder, IGhosty
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number GhoulMarkCd = 25;

    private CustomButton MarkButton { get; set; }
    public bool Caught { get; set; }
    public PlayerControl MarkedPlayer { get; set; }
    public Vector3 LastPosition { get; set; }

    protected override UColor MainColor => CustomColorManager.Ghoul;
    public override LayerEnum Type => LayerEnum.Ghoul;
    public override Func<string> StartText { get; } = () => "BOO!";
    public override Func<string> Description => () => "- You can mark a player for death every round\n- Marked players will be announced to all players and will die at the end of the next" +
        " meeting if you are not clicked";

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Utility;
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
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, MarkedPlayer);
        MarkButton.StartCooldown();
    }

    private bool Usable() => !Caught && !MarkedPlayer;

    private bool Exception1(PlayerControl player) => player == MarkedPlayer || (player.Is(Faction) && Faction.IsFactionedEvil()) || Player.IsLinkedTo(player);

    public override void ReadRPC(RpcReader reader) => MarkedPlayer = reader.ReadPlayer();

    public bool CanBeClicked(PlayerControl clicker) => !clicker.Is(Faction);
}