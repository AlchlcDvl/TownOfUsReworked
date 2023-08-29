namespace TownOfUsReworked.PlayerLayers.Roles;

public class Framer : Syndicate
{
    public CustomButton FrameButton { get; set; }
    public CustomButton RadialFrameButton { get; set; }
    public List<byte> Framed { get; set; }
    public DateTime LastFramed { get; set; }

    public override Color32 Color => ClientGameOptions.CustomSynColors ? Colors.Framer : Colors.Syndicate;
    public override string Name => "Framer";
    public override LayerEnum Type => LayerEnum.Framer;
    public override Func<string> StartText => () => "Make Everyone Suspicious";
    public override Func<string> Description => () => $"- You can frame {(HoldsDrive ? $"all players within a {CustomGameOptions.ChaosDriveFrameRadius}m radius" : "a player")}\n- " +
        $"Framed players will die very easily to killing roles and will appear to have the wrong results to investigative roles till you are dead\n{CommonAbilities}";
    public override InspectorResults InspectorResults => InspectorResults.Manipulative;
    public float Timer => ButtonUtils.Timer(Player, LastFramed, CustomGameOptions.FrameCd);

    public Framer(PlayerControl player) : base(player)
    {
        RoleAlignment = RoleAlignment.SyndicateDisrup;
        Framed = new();
        FrameButton = new(this, "Frame", AbilityTypes.Direct, "Secondary", Frame, Exception1);
        RadialFrameButton = new(this, "RadialFrame", AbilityTypes.Effect, "Secondary", RadialFrame);
    }

    public void RpcFrame(PlayerControl player)
    {
        if ((player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) || Framed.Contains(player.PlayerId))
            return;

        Framed.Add(player.PlayerId);
        CallRpc(CustomRPC.Action, ActionsRPC.Frame, this, player);
    }

    public void Frame()
    {
        if (Timer != 0f || IsTooFar(Player, FrameButton.TargetPlayer) || HoldsDrive)
            return;

        var interact = Interact(Player, FrameButton.TargetPlayer);

        if (interact[3])
            RpcFrame(FrameButton.TargetPlayer);

        if (interact[0])
            LastFramed = DateTime.UtcNow;
        else if (interact[1])
            LastFramed.AddSeconds(CustomGameOptions.ProtectKCReset);
    }

    public void RadialFrame()
    {
        if (Timer != 0f || !HoldsDrive)
            return;

        GetClosestPlayers(CustomPlayer.Local.GetTruePosition(), CustomGameOptions.ChaosDriveFrameRadius).ForEach(RpcFrame);
        LastFramed = DateTime.UtcNow;
    }

    public bool Exception1(PlayerControl player) => Framed.Contains(player.PlayerId) || player.Is(Faction) || (player.Is(SubFaction) && SubFaction != SubFaction.None);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        FrameButton.Update("FRAME", Timer, CustomGameOptions.FrameCd, true, !HoldsDrive);
        RadialFrameButton.Update("FRAME", Timer, CustomGameOptions.FrameCd, true, HoldsDrive);
    }
}