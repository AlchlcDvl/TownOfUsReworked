using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GodfatherMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static class DragBody
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Godfather))
                return;

            var role = Role.GetRole<Godfather>(__instance);
            var body = role.CurrentlyDragging;

            if (body == null)
                return;

            if (__instance.Data.IsDead)
            {
                role.CurrentlyDragging = null;
                body.bodyRenderer.material.SetFloat("_Outline", 0f);
                return;
            }

            var currentPosition = __instance.GetTruePosition();
            var velocity = __instance.gameObject.GetComponent<Rigidbody2D>().velocity.normalized;
            Vector3 newPos = ((Vector2)__instance.transform.position) - (velocity / 3) + body.myCollider.offset;

            //WHY ARE THERE DIFFERENT LOCAL Z INDEXS FOR DIFFERENT DECALS ON DIFFERENT LEVELS?!?!?!
            //AD: idk ¯\_(ツ)_/¯
            if (SubmergedCompatibility.IsSubmerged())
            {
                if (newPos.y > -7f)
                    newPos.z = 0.0208f;
                else
                    newPos.z = -0.0273f;
            }

            if (!PhysicsHelpers.AnythingBetween(currentPosition, newPos, Constants.ShipAndObjectsMask, false))
                body.transform.position = newPos;

            if (!__instance.AmOwner)
                return;

            var material = body.bodyRenderer.material;
            material.SetColor("_OutlineColor", Color.green);
            material.SetFloat("_Outline", 1f);
            __instance.moveable = true;
        }
    }
}
