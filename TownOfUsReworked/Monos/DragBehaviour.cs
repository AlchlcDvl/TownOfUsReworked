namespace TownOfUsReworked.Monos
{
    public class DragBehaviour : MonoBehaviour
    {
        private const float YOffset = -0.1f;
        public PlayerControl Source;
        public Rigidbody2D Body;
        public Collider2D Collider;
        public DeadBody Dragged;

        public DragBehaviour(IntPtr ptr) : base(ptr) {}

        public void Start()
        {
            if (Application.targetFrameRate <= 30)
                return;

            Body.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        public void Update()
        {
            if (!Dragged || !Body)
                return;

            UpdateInSubmerged();

            if (!Source)
                Body.velocity = Vector2.zero;
            else
            {
                var truePosition1 = Source.GetTruePosition();
                var truePosition2 = (Vector2)transform.position + (Collider.offset * 0.7f);
                var velocity = Body.velocity;
                var vector2_1 = truePosition1 - truePosition2;
                var num = 0f;

                if (Source.CanMove)
                    num = 0.2f;

                Vector2 vector2_2;

                if (vector2_1.sqrMagnitude > num)
                {
                    if (vector2_1.sqrMagnitude > 2.0)
                    {
                        transform.position = (Vector3) truePosition1;
                        return;
                    }

                    var vector2_3 = vector2_1 * (5f * Source.MyPhysics.SpeedMod);
                    vector2_2 = (velocity * 0.8f) + (vector2_3 * 0.2f);
                }
                else
                    vector2_2 = velocity * 0.7f;

                Body.velocity = vector2_2;
            }
        }

        public void LateUpdate()
        {
            var localPosition = transform.localPosition;
            localPosition.z = ((localPosition.y + YOffset) / 1000f) + 0.00019999999494757503f;
            transform.localPosition = localPosition;
        }

        public void OnDestroy()
        {
            Vector3 position = Source.GetTruePosition();

            if (ModCompatibility.IsSubmerged)
                position.z = position.y > -7f ? 0.0208f :  -0.0273f;

            position.y -= 0.3636f;
            Dragged.transform.position = position;
            Body.velocity = Vector2.zero;
            Body.Destroy();
        }

        private void UpdateInSubmerged()
        {
            if (!ModCompatibility.IsSubmerged)
                return;

            var position1 = transform.position;
            var position2 = Source.transform.position;

            if (Mathf.Abs(position2.y - position1.y) <= 28.871400833129883)
                return;

            position1.y += position2.y > -6.0 ? 48.119f : -48.119f;
            transform.position = position1;
        }
    }
}