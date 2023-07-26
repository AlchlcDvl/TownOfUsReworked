namespace TownOfUsReworked.Monos
{
    public class InteractableBehaviour : MonoBehaviour
    {
        public InteractableBehaviour(IntPtr ptr) : base(ptr) {}

        public static readonly List<GameObject> AllCustomPlateform = new();
        public byte Id;
        public Action OnClick;
        public static InteractableBehaviour NearestTask;
        private readonly SpriteRenderer renderer;

        [HideFromIl2Cpp]
        public float CanUse(GameData.PlayerInfo pc, out bool canUse)
        {
            var player = pc.Object;
            var truePosition = player.GetTruePosition();
            canUse = !pc.IsDead && player.CanMove && !CallPlateform.PlateformIsUsed && !FindObjectOfType<MovingPlatformBehaviour>().InUse;
            var distance = float.MaxValue;

            if (canUse)
            {
                distance = Vector2.Distance(truePosition, transform.position);
                canUse &= distance <= CustomGameOptions.InteractionDistance;
            }

            return distance;
        }

        public void Use() => OnClick();

        [HideFromIl2Cpp]
        public void SetOutline(bool on)
        {
            if (renderer)
            {
                renderer.material.SetFloat("_Outline", on ? 1 : 0);
                renderer.material.SetColor("_OutlineColor", UColor.white);
                renderer.material.SetColor("_AddColor", on ? UColor.white : Color.clear);
            }
        }

        public static void CreateThisTask(Vector3 position, Vector3 rotation, Action onClick)
        {
            var callPlateform = new GameObject("CallPlateform");
            callPlateform.transform.position = position;
            callPlateform.transform.localRotation = Quaternion.Euler(rotation);
            callPlateform.transform.localScale = new(1f, 1f, 2f);
            callPlateform.layer = 12;
            callPlateform.SetActive(true);

            var callPlateformTasks = callPlateform.AddComponent<InteractableBehaviour>();
            callPlateformTasks.Id = 1;
            callPlateformTasks.OnClick = onClick;
            AllCustomPlateform.Add(callPlateform);
        }

        public static void ClosestTasks(PlayerControl player)
        {
            NearestTask = null;

            foreach (var customElectrical in AllCustomPlateform)
            {
                var component = customElectrical.GetComponent<InteractableBehaviour>();
                component.SetOutline(false);

                if (component && ((!player.Data.IsDead && (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) && player.CanMove) || !player.inVent || !player.inMovingPlat ||
                    !player.onLadder))
                {
                    var distance = component.CanUse(player.Data, out var canUse);

                    if (canUse)
                    {
                        NearestTask = component;
                        component.SetOutline(true);
                    }
                }
            }
        }
    }
}