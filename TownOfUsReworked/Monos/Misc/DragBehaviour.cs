namespace TownOfUsReworked.Monos;

public class DragBehaviour : MonoBehaviour
{
    public PlayerControl Source;
    public Rigidbody2D Body;
    public Collider2D Collider;
    public DeadBody Dragged;

    public DragBehaviour(IntPtr ptr) : base(ptr) {}

    public void SetDrag(PlayerControl source, DeadBody dragged)
    {
        Source = source;
        Dragged = dragged;
        Body = Dragged?.gameObject.EnsureComponent<Rigidbody2D>();
        Collider = Dragged?.gameObject.EnsureComponent<Collider2D>();

        if (Application.targetFrameRate > 30)
            Body.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    public void Update()
    {
        UpdateInSubmerged();

        if (!Source)
            Body.velocity = Vector2.zero;
        else if (Source.Data.IsDead)
        {
            Dragged.transform.position = Source.GetTruePosition();
            this.Destroy();
        }
        else if (!Source.Data.IsDead)
        {
            var truePosition1 = Source.GetTruePosition();
            var truePosition2 = (Vector2)transform.position + (Collider.offset * 0.7f);
            var vector = Body.velocity;
            var vector2 = truePosition1 - truePosition2;
            var num = 0f;

            if (Source.CanMove)
                num = 0.2f;

            if (vector2.sqrMagnitude > num)
            {
                if (vector2.sqrMagnitude > 2f)
                {
                    Body.transform.position = transform.position = truePosition1;
                    return;
                }

                vector2 *= 5f * Source.MyPhysics.SpeedMod;
                vector = (vector * 0.8f) + (vector2 * 0.2f);
            }
            else
                vector *= 0.7f;

            Body.velocity = vector;
            Dragged.bodyRenderers.ForEach(x => x.flipX = Body.velocity.x < -0.01f);
        }
    }

    public void LateUpdate()
    {
        var localPosition = transform.localPosition;
        localPosition.z = ((localPosition.y + -0.1f) / 1000f) + 0.0002f;
        transform.localPosition = localPosition;
    }

    public void OnDestroy()
    {
        var position = Source.transform.position;

        if (IsSubmerged())
            position.z = position.y > -7f ? 0.0208f : -0.0273f;

        position.y -= 0.4f;
        Dragged.transform.position = position;
        Body.velocity = Vector2.zero;
        Body.Destroy();
        Body = null;
    }

    private void UpdateInSubmerged()
    {
        if (!IsSubmerged())
            return;

        var position1 = transform.position;
        var position2 = Source.transform.position;

        if (Mathf.Abs(position2.y - position1.y) <= 28.871400833129883f)
            return;

        position1.y += position2.y > -6.0 ? 48.119f : -48.119f;
        transform.position = position1;
    }
}