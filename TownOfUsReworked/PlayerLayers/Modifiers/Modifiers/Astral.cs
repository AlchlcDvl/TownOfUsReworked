namespace TownOfUsReworked.PlayerLayers.Modifiers;

public class Astral : Modifier
{
    public Vector3 LastPosition { get; set; }

    public override UColor Color => ClientOptions.CustomModColors ? CustomColorManager.Astral : CustomColorManager.Modifier;
    public override string Name => "Astral";
    public override LayerEnum Type => LayerEnum.Astral;
    public override Func<string> Description => () => "- You will not teleport to the meeting button";

    public override void Init() => LastPosition = Vector3.zero;

    public void SetPosition()
    {
        if (LastPosition == Vector3.zero)
            return;

        Player.RpcCustomSnapTo(LastPosition);

        if (IsSubmerged())
            ChangeFloor(LastPosition.y > -7);
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);

        if (!UninteractiblePlayers.ContainsKey(PlayerId))
            LastPosition = CustomPlayer.LocalCustom.Position;
    }
}