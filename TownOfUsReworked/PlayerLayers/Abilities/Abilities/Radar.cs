namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Radar : Ability
    {
        public List<ArrowBehaviour> RadarArrow = new();
        public SpriteRenderer Point;

        public Radar(PlayerControl player) : base(player)
        {
            Name = "Radar";
            TaskText = "- You are aware of those close to you";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Radar : Colors.Ability;
            AbilityType = AbilityEnum.Radar;
            RadarArrow = new();
            Type = LayerEnum.Radar;
        }

        public override void OnLobby()
        {
            base.OnLobby();
            RadarArrow.DestroyAll();
            ClearPoint();
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);

            if (IsDead)
            {
                OnLobby();
                return;
            }

            if (RadarArrow.Count == 0)
            {
                var gameObj = new GameObject("RadarArrow") { layer = 5 };
                var arrow = gameObj.AddComponent<ArrowBehaviour>();
                gameObj.transform.parent = Player.gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = AssetManager.GetSprite("Arrow");
                renderer.color = Color;
                arrow.image = renderer;
                arrow.target = Player.transform.position;
                RadarArrow.Add(arrow);
            }

            foreach (var arrow in RadarArrow)
                arrow.target = Player.GetClosestPlayer(null, float.MaxValue).transform.position;
        }

        public override void UpdateMap(MapBehaviour __instance)
        {
            base.UpdateMap(__instance);

            if (IsDead || MeetingHud.Instance)
                return;

            var v = RadarArrow[Player.GetClosestPlayer(null, float.MaxValue).PlayerId].target;
            v /= ShipStatus.Instance.MapScale;
            v.x *= Mathf.Sign(ShipStatus.Instance.transform.localScale.x);
            v.z = -1f;

            if (Point)
                Point.transform.localPosition = v;
            else
            {
                var Point = UObject.Instantiate(__instance.HerePoint, __instance.HerePoint.transform.parent, true);
                Point.enabled = true;
                Point.color = Color;
                Point.transform.localPosition = v;
            }
        }

        public void ClearPoint() => Point.Destroy();
    }
}