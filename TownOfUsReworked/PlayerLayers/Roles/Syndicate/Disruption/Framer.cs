namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Framer : Syndicate
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number FrameCd { get; set; } = new(25f);

    [NumberOption(MultiMenu.LayerSubOptions, 0.5f, 5f, 0.25f, Format.Distance)]
    public static Number ChaosDriveFrameRadius { get; set; } = new(1.5f);

    public CustomButton FrameButton { get; set; }
    public CustomButton RadialFrameButton { get; set; }
    public List<byte> Framed { get; set; }

    public override UColor Color => ClientOptions.CustomSynColors ? CustomColorManager.Framer : CustomColorManager.Syndicate;
    public override string Name => "Framer";
    public override LayerEnum Type => LayerEnum.Framer;
    public override Func<string> StartText => () => "Make Everyone Suspicious";
    public override Func<string> Description => () => $"- You can frame a{(HoldsDrive ? $"ll players within a {ChaosDriveFrameRadius}m radius" : " player")}\n- Till you are dead, framed " +
        $"targets will die easily to killing roles and have the wrong investigative results\n{CommonAbilities}";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.SyndicateDisrup;
        Framed = [];
        FrameButton = CreateButton(this, new SpriteName("Frame"), AbilityTypes.Alive, KeybindType.Secondary, (OnClick)Frame, new Cooldown(FrameCd), "FRAME", (UsableFunc)Usable1,
            (PlayerBodyExclusion)Exception1);
        RadialFrameButton = CreateButton(this, new SpriteName("RadialFrame"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)RadialFrame, new Cooldown(FrameCd), (UsableFunc)Usable2,
            "FRAME");
    }

    public void RpcFrame(PlayerControl player)
    {
        if (Exception1(player))
            return;

        Framed.Add(player.PlayerId);
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, player.PlayerId);
    }

    public void Frame()
    {
        var cooldown = Interact(Player, FrameButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
            RpcFrame(FrameButton.TargetPlayer);

        FrameButton.StartCooldown(cooldown);
    }

    public void RadialFrame()
    {
        GetClosestPlayers(Player.transform.position, ChaosDriveFrameRadius).ForEach(RpcFrame);
        RadialFrameButton.StartCooldown();
    }

    public bool Exception1(PlayerControl player) => Framed.Contains(player.PlayerId) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) &&
        SubFaction != SubFaction.None);

    public bool Usable1() => !HoldsDrive;

    public bool Usable2() => HoldsDrive;

    public override void ReadRPC(MessageReader reader) => Framed.Add(reader.ReadByte());
}