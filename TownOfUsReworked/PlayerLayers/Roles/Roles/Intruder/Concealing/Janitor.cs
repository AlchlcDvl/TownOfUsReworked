using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.Classes;
using System;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Janitor : IntruderRole
    {
        public AbilityButton CleanButton;
        public AbilityButton DragButton;
        public AbilityButton DropButton;
        public DateTime LastDragged;
        public DeadBody CurrentlyDragging;
        public DeadBody CurrentTarget;
        public DateTime LastCleaned;

        public Janitor(PlayerControl player) : base(player)
        {
            Name = "Janitor";
            StartText = "Sanitise The Ship, By Any Means Neccessary";
            AbilitiesText = $"- You can clean up dead bodies, making them disappear from sight\n- You can drag bodies away to prevent them from getting reported.\n{AbilitiesText}";
            Color = CustomGameOptions.CustomIntColors ? Colors.Janitor : Colors.Intruder;
            RoleType = RoleEnum.Janitor;
            RoleAlignment = RoleAlignment.IntruderConceal;
            AlignmentName = IC;
            InspectorResults = InspectorResults.MeddlesWithDead;
            CurrentlyDragging = null;
        }

        public float CleanTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastCleaned;
            var num = CustomButtons.GetModifiedCooldown(ConstantVariables.LastImp && CustomGameOptions.SoloBoost ? (CustomGameOptions.JanitorCleanCd - CustomGameOptions.UnderdogKillBonus) :
                CustomGameOptions.JanitorCleanCd, CustomButtons.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public float DragTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastDragged;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.DragCd, CustomButtons.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public static void DragBody(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Janitor))
                return;

            var role = GetRole<Janitor>(__instance);
            var body = role.CurrentlyDragging;

            if (body == null)
                return;

            if (__instance.Data.IsDead)
            {
                role.CurrentlyDragging = null;

                foreach (var component in body?.bodyRenderers)
                    component.material.SetFloat("_Outline", 0f);

                return;
            }

            var truePosition = __instance.GetTruePosition();
            var velocity = __instance.gameObject.GetComponent<Rigidbody2D>().velocity.normalized;
            Vector3 newPos = ((Vector2)__instance.transform.position) - (velocity / 3f) + body.myCollider.offset;
            newPos.z = 0.02f;

            //WHY ARE THERE DIFFERENT LOCAL Z INDEXS FOR DIFFERENT DECALS ON DIFFERENT LEVELS?!?!?!
            //AD: idk ¯\_(ツ)_/¯
            if (SubmergedCompatibility.IsSubmerged())
            {
                if (newPos.y > -7f)
                    newPos.z = 0.0208f;
                else
                    newPos.z = -0.0273f;
            }

            if (!PhysicsHelpers.AnythingBetween(truePosition, newPos, Constants.ShipAndObjectsMask, false))
                body.transform.position = newPos;

            if (!__instance.AmOwner)
                return;

            foreach (var component in body?.bodyRenderers)
            {
                component.material.SetColor("_OutlineColor", UnityEngine.Color.green);
                component.material.SetFloat("_Outline", 1f);
            }

            __instance.moveable = true;
        }
    }
}