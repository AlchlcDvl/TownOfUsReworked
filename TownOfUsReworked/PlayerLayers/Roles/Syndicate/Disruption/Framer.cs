namespace TownOfUsReworked.PlayerLayers.Roles;

public class Framer : Syndicate
{
    public CustomButton FrameButton { get; set; }
    public CustomButton RadialFrameButton { get; set; }
    public List<byte> Framed { get; set; }

    public override UColor Color => ClientGameOptions.CustomSynColors ? CustomColorManager.Framer : CustomColorManager.Syndicate;
    public override string Name => "Framer";
    public override LayerEnum Type => LayerEnum.Framer;
    public override Func<string> StartText => () => "Make Everyone Suspicious";
    public override Func<string> Description => () => $"- You can frame {(HoldsDrive ? $"all players within a {CustomGameOptions.ChaosDriveFrameRadius}m radius" : "a player")}\n- Till you " +
        $"are dead, framed targets will die easily to killing roles and have the wrong investigative results\n{CommonAbilities}";

    public Framer() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        BaseStart();
        Alignment = Alignment.SyndicateDisrup;
        Framed = new();
        FrameButton = new(this, "Frame", AbilityTypes.Alive, "Secondary", Frame, CustomGameOptions.FrameCd, Exception1);
        RadialFrameButton = new(this, "RadialFrame", AbilityTypes.Targetless, "Secondary", RadialFrame, CustomGameOptions.FrameCd);
        return this;
    }

    public void RpcFrame(PlayerControl player)
    {
        if (Exception1(player))
            return;

        Framed.Add(player.PlayerId);
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, player.PlayerId);
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

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        FrameButton.Update2("FRAME", !HoldsDrive);
        RadialFrameButton.Update2("FRAME", HoldsDrive);
    }

    public override void ReadRPC(MessageReader reader) => Framed.Add(reader.ReadByte());
}