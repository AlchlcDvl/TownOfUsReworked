namespace TownOfUsReworked.PlayerLayers.Modifiers;

public class Astral : Modifier
{
    public Vector3 LastPosition { get; set; }

    public override UColor Color => ClientGameOptions.CustomModColors ? CustomColorManager.Astral : CustomColorManager.Modifier;
    public override string Name => "Astral";
    public override LayerEnum Type => LayerEnum.Astral;
    public override Func<string> Description => () => "- You will not teleport to the meeting button";

    public Astral(PlayerControl player) : base(player) => LastPosition = Vector3.zero;

    public void SetPosition()
    {
        if (LastPosition == Vector3.zero)
            return;

        Player.NetTransform.RpcSnapTo(LastPosition);

        if (IsSubmerged())
            ChangeFloor(LastPosition.y > -7);
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);

        if (!UninteractiblePlayers.ContainsKey(Player.PlayerId))
            LastPosition = CustomPlayer.LocalCustom.Position;
    }
}