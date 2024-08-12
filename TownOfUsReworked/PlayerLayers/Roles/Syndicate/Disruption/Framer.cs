namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Framer : Syndicate
{
    public CustomButton FrameButton { get; set; }
    public CustomButton RadialFrameButton { get; set; }
    public List<byte> Framed { get; set; }

    public override UColor Color => ClientOptions.CustomSynColors ? CustomColorManager.Framer : CustomColorManager.Syndicate;
    public override string Name => "Framer";
    public override LayerEnum Type => LayerEnum.Framer;
    public override Func<string> StartText => () => "Make Everyone Suspicious";
    public override Func<string> Description => () => $"- You can frame a{(HoldsDrive ? $"ll players within a {CustomGameOptions.ChaosDriveFrameRadius}m radius" : " player")}\n- Till you " +
        $"are dead, framed targets will die easily to killing roles and have the wrong investigative results\n{CommonAbilities}";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.SyndicateDisrup;
        Framed = [];
        FrameButton = CreateButton(this, new SpriteName("Frame"), AbilityTypes.Alive, KeybindType.Secondary, (OnClick)Frame, new Cooldown(CustomGameOptions.FrameCd), "FRAME",
            (PlayerBodyExclusion)Exception1, (UsableFunc)Usable1);
        RadialFrameButton = CreateButton(this, new SpriteName("RadialFrame"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)RadialFrame, new Cooldown(CustomGameOptions.FrameCd),
            "FRAME", (UsableFunc)Usable2);
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
        GetClosestPlayers(Player.transform.position, CustomGameOptions.ChaosDriveFrameRadius).ForEach(RpcFrame);
        RadialFrameButton.StartCooldown();
    }

    public bool Exception1(PlayerControl player) => Framed.Contains(player.PlayerId) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || (player.Is(SubFaction) &&
        SubFaction != SubFaction.None);

    public bool Usable1() => !HoldsDrive;

    public bool Usable2() => HoldsDrive;

    public override void ReadRPC(MessageReader reader) => Framed.Add(reader.ReadByte());
}