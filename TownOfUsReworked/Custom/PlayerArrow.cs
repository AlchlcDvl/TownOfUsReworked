namespace TownOfUsReworked.Custom;

public class PlayerArrow : CustomArrow
{
    public PlayerControl TargetPlayer { get; set; }
    private bool SkipBody { get; set; }

    public PlayerArrow(PlayerControl owner, PlayerControl target, UColor color, float interval = 0f, bool skipBody = false) : base(owner, color, null, interval)
    {
        TargetPlayer = target;
        SkipBody = skipBody;
        Target = TargetFunc;
    }

    private Vector3 TargetFunc()
    {
        var pos = TargetPlayer.transform.position;

        if (!SkipBody)
        {
            var body = BodyByPlayer(TargetPlayer);

            if (!body && TargetPlayer.Data.IsDead)
            {
                Arrow.target = Owner.transform.position;
                Disable();
                return Vector3.zero;
            }
            else if (body)
                pos = body.transform.position;
        }

        return pos;
    }
}