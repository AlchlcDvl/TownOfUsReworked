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
    public override Func<string> Description => () => $"- You can frame {(HoldsDrive ? $"all players within a {CustomGameOptions.ChaosDriveFrameRadius}m radius" : "a player")}\n- Framed " +
        $"players will die very easily to killing roles and will appear to have the wrong results to investigative roles till you are dead\n{CommonAbilities}";

    public Framer(PlayerControl player) : base(player)
    {
        Alignment = Alignment.SyndicateDisrup;
        Framed = new();
        FrameButton = new(this, "Frame", AbilityTypes.Alive, "Secondary", Frame, CustomGameOptions.FrameCd, Exception1);
        RadialFrameButton = new(this, "RadialFrame", AbilityTypes.Targetless, "Secondary", RadialFrame, CustomGameOptions.FrameCd);
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