namespace TownOfUsReworked.Monos
{
    public class Tasks : MonoBehaviour
    {
        public Tasks(IntPtr ptr) : base(ptr) {}

        public readonly static List<GameObject> AllCustomPlateform = new();
        public byte Id;
        public Action OnClick;

        public static Tasks NearestTask;
        private readonly SpriteRenderer renderer = null;

        [HideFromIl2Cpp]
        public float CanUse(GameData.PlayerInfo PC, out bool CanUse)
        {
            var Player = PC.Object;
            var truePosition = Player.GetTruePosition();
            CanUse = !PC.IsDead && Player.CanMove && !CallPlateform.PlateformIsUsed && !FindObjectOfType<MovingPlatformBehaviour>().InUse;
            var Distance = float.MaxValue;

            if (CanUse)
            {
                Distance = Vector2.Distance(truePosition, transform.position);
                CanUse &= Distance <= CustomGameOptions.InteractionDistance;
            }

            return Distance;
        }

        public void Use() => OnClick();

        [HideFromIl2Cpp]
        public void SetOutline(bool On)
        {
            if (renderer)
            {
                renderer.material.SetFloat("_Outline", On ? 1 : 0);
                renderer.material.SetColor("_OutlineColor", Color.white);
                renderer.material.SetColor("_AddColor", On ? Color.white : Color.clear);
            }
        }

        public static void CreateThisTask(Vector3 Position, Vector3 Rotation, Action OnClick)
        {
            var callPlateform = new GameObject("CallPlateform");
            callPlateform.transform.position = Position;
            callPlateform.transform.localRotation = Quaternion.Euler(Rotation);
            callPlateform.transform.localScale = new(1f, 1f, 2f);
            callPlateform.layer = 12;
            callPlateform.SetActive(true);

            var callPlateformTasks = callPlateform.AddComponent<Tasks>();
            callPlateformTasks.Id = 1;
            callPlateformTasks.OnClick = OnClick;
            AllCustomPlateform.Add(callPlateform);
        }

        public static void ClosestTasks(PlayerControl Player)
        {
            NearestTask = null;

            foreach (var CustomElectrical in AllCustomPlateform)
            {
                var component = CustomElectrical.GetComponent<Tasks>();
                component.SetOutline(false);

                if (component != null && ((!Player.Data.IsDead && (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) && Player.CanMove) || !Player.inVent))
                {
                    var Distance = component.CanUse(Player.Data, out bool CanUse);

                    if (CanUse && Distance <= CustomGameOptions.InteractionDistance)
                    {
                        NearestTask = component;
                        component.SetOutline(true);
                    }
                }
            }
        }
    }
}