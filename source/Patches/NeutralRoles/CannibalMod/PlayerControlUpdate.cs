using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.NeutralRoles.CannibalMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static ArrowBehaviour Arrow;
        public static DeadBody Target;
        public static Sprite Arrows => TownOfUs.Arrow;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Cannibal)) return;

            var role = Role.GetRole<Cannibal>(PlayerControl.LocalPlayer);
            var eatButton = __instance.KillButton;
            
            if (role.EatButton == null)
            {
                role.EatButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.EatButton.graphic.enabled = true;
                role.EatButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUs.ButtonPosition;
                role.EatButton.gameObject.SetActive(false);
            }

            if (PlayerControl.LocalPlayer.Data.IsDead)
            {
                    eatButton.gameObject.SetActive(false);
                    //eatButton.isActive = false;
            }
            else
            {
                eatButton.gameObject.SetActive(!MeetingHud.Instance);
                //eatButton.isActive = !MeetingHud.Instance;
                eatButton.graphic.sprite = TownOfUs.CannibalEat;
            }

            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            var maxDistance = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];
            var flag = (PlayerControl.GameOptions.GhostsDoTasks || !data.IsDead) && (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) &&
                       PlayerControl.LocalPlayer.CanMove;
            var killButton = role.EatButton;
            DeadBody closestBody = null;
            var closestDistance = float.MaxValue;
            var allBodies = Object.FindObjectsOfType<DeadBody>();
            var allocs = Physics2D.OverlapCircleAll(truePosition, 10, LayerMask.GetMask(new[] {"Players", "Ghost"}));

            foreach (var collider2D in allocs)
            {
                if (!flag || PlayerControl.LocalPlayer.Data.IsDead || collider2D.tag != "DeadBody") continue;
                var component = collider2D.GetComponent<DeadBody>();
                var distance = Vector2.Distance(truePosition, component.TruePosition);
                if (distance <= 10 && Arrow == null) Target = component;
                else if (distance > 10 && Target == component) Target = null;
                if (!(distance <= maxDistance)) continue;  
                if (!(distance < closestDistance)) continue;

                closestBody = component;
                closestDistance = distance;
            }

            KillButtonTarget.SetTarget(killButton, closestBody, role);
            eatButton.SetCoolDown(role.CannibalTimer(), CustomGameOptions.CannibalCd);

            if (Target != null && Arrow == null)
            {
                var gameObj = new GameObject();
                Arrow = gameObj.AddComponent<ArrowBehaviour>();
                gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = TownOfUs.Arrow;
                renderer.color = role.Color;
                Arrow.image = renderer;
                gameObj.layer = 5;
            }
        }
    }
}