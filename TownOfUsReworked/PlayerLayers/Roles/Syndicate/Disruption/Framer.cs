namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Framer : Syndicate, IFramer
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number FrameCd { get; set; } = new(25f);

    [NumberOption(MultiMenu.LayerSubOptions, 0.5f, 5f, 0.25f, Format.Distance)]
    public static Number ChaosDriveFrameRadius { get; set; } = new(1.5f);

    public CustomButton FrameButton { get; set; }
    public CustomButton RadialFrameButton { get; set; }
    public List<byte> Framed { get; } = [];

    public override UColor Color => ClientOptions.CustomSynColors ? CustomColorManager.Framer : FactionColor;
    public override LayerEnum Type => LayerEnum.Framer;
    public override Func<string> StartText => () => "Make Everyone Suspicious";
    public override Func<string> Description => () => $"- You can frame a{(HoldsDrive ? $"ll players within a {ChaosDriveFrameRadius}m radius" : " player")}\n- Till you are dead, framed " +
        $"targets will die easily to killing roles and will have the wrong investigative results\n{CommonAbilities}";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.SyndicateDisrup;
        Framed.Clear();
        FrameButton ??= new(this, new SpriteName("Frame"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Frame, new Cooldown(FrameCd), "FRAME", (UsableFunc)Usable1,
            (PlayerBodyExclusion)Exception1);
        RadialFrameButton ??= new(this, new SpriteName("RadialFrame"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)RadialFrame, new Cooldown(FrameCd), "FRAME",
            (UsableFunc)Usable2);
    }

    public void RpcFrame(PlayerControl player)
    {
        if (Exception1(player))
            return;

        Framed.Add(player.PlayerId);
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, player.PlayerId);
    }

    public void Frame(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            RpcFrame(target);

        FrameButton.StartCooldown(cooldown);
    }

    public void RadialFrame()
    {
        GetClosestPlayers(Player, ChaosDriveFrameRadius).ForEach(RpcFrame);
        RadialFrameButton.StartCooldown();
    }

    public bool Exception1(PlayerControl player) => Framed.Contains(player.PlayerId) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) &&
        SubFaction != SubFaction.None);

    public bool Usable1() => !HoldsDrive;

    public bool Usable2() => HoldsDrive;

    public override void ReadRPC(MessageReader reader) => Framed.Add(reader.ReadByte());
}