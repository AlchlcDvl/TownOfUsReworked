namespace TownOfUsReworked.PlayerLayers.Modifiers;

public sealed class Astral : Modifier, IPositioner
{
    public Vector3 LastPosition { get; set; }

    protected override UColor MainColor => CustomColorManager.Astral;
    public override Layer Type => Layer.Astral;
    public override string Description => "- You will not teleport to the meeting button";

    public override void Init() => LastPosition = Vector3.zero;

    public override void BeforeMeeting()
    {
        if (!UninteractablePlayers.ContainsKey(PlayerId))
            LastPosition = Player.transform.position;
    }

    public void SetPosition()
    {
        if (Dead || LastPosition == Vector3.zero)
            return;

        Player.RpcCustomSnapTo(LastPosition);

        if (IsSubmerged())
            ChangeFloor(LastPosition.y > -7);
    }
}