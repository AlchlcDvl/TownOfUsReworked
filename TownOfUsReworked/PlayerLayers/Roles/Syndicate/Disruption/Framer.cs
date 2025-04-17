namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Framer)]
public sealed class Framer : Syndicate
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number FrameCd = 25f;

    [NumberOption(0.5f, 5f, 0.25f, Format.Distance)]
    public static Number ChaosDriveFrameRadius = 1.5f;

    private CustomButton FrameButton { get; set; }
    private CustomButton RadialFrameButton { get; set; }
    public List<byte> Framed { get; } = [];

    protected override UColor MainColor => CustomColorManager.Framer;
    public override LayerEnum Type { get; } = LayerEnum.Framer;
    public override Func<string> StartText { get; } = () => "Make Everyone Suspicious";
    public override Func<string> Description => () => $"- You can frame a{(HoldsDrive ? $"ll players within a {ChaosDriveFrameRadius}m radius" : " player")}\n- Till you are dead, framed " +
        $"targets will die easily to killing roles and will have the wrong investigative results\n{CommonAbilities}";

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Disruption;
        Framed.Clear();
        FrameButton ??= new(this, new SpriteName("Frame"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Frame, new Cooldown(FrameCd), "FRAME", (UsableFunc)Usable1,
            (PlayerBodyExclusion)Exception1);
        RadialFrameButton ??= new(this, new SpriteName("RadialFrame"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)RadialFrame, new Cooldown(FrameCd), "FRAME",
            (UsableFunc)Usable2);
    }

    public override void Reset(bool meeting, bool start)
    {
    }

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (Framed.Contains(player.PlayerId))
            name += " <#00FFFFFF>ς</color>";
    }

    private void RpcFrame(PlayerControl player)
    {
        if (Exception1(player))
            return;

        Framed.Add(player.PlayerId);
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, player.PlayerId);
    }

    private void Frame(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            RpcFrame(target);

        FrameButton.StartCooldown(cooldown);
    }

    private void RadialFrame()
    {
        GetClosestPlayers(Player, ChaosDriveFrameRadius).ForEach(RpcFrame);
        RadialFrameButton.StartCooldown();
    }

    private bool Exception1(PlayerControl player) => Framed.Contains(player.PlayerId) || (player.Is(Faction) && Faction is not (Faction.Crew or Faction.Neutral)) || (player.Is(SubFaction) &&
        SubFaction != SubFaction.None);

    private bool Usable1() => !HoldsDrive;

    private bool Usable2() => HoldsDrive;

    public override void ReadRPC(NetData reader) => Framed.Add(reader.ReadByte());

    protected override void OnTrueDeath() => Framed.Clear();
}