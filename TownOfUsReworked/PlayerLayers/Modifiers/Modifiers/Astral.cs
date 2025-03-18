namespace TownOfUsReworked.PlayerLayers.Modifiers;

public sealed class Astral : Modifier
{
    public Vector3 LastPosition { get; set; }

    public override UColor MainColor => CustomColorManager.Astral;
    public override LayerEnum Type => LayerEnum.Astral;
    public override Func<string> Description => () => "- You will not teleport to the meeting button";

    protected override void Init() => LastPosition = Vector3.zero;

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