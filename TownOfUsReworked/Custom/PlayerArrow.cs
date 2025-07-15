namespace TownOfUsReworked.Custom;

public sealed class PlayerArrow : CustomArrow
{
    private readonly PlayerControl TargetPlayer;
    private readonly bool SkipBody;

    public PlayerArrow(PlayerControl owner, PlayerControl target, UColor color, float interval = 0f, bool skipBody = false) : base(owner, color, null, interval)
    {
        TargetPlayer = target;
        SkipBody = skipBody;
        Target = TargetFunc;
    }

    private Vector3 TargetFunc()
    {
        var pos = TargetPlayer.transform.position;

        if (SkipBody)
            return pos;

        var body = BodyByPlayer(TargetPlayer);

        if (!body && TargetPlayer.Data.IsDead)
        {
            Arrow.target = Owner.transform.position;
            Disable();
            return Vector3.zero;
        }

        if (body)
            pos = body.transform.position;

        return pos;
    }
}