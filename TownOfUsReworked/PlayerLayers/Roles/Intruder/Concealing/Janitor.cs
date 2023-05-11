using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.Classes;
using System;
using TownOfUsReworked.Data;
using TownOfUsReworked.Custom;
using Hazel;
using Reactor.Utilities;
using HarmonyLib;
using Reactor.Networking.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Janitor : IntruderRole
    {
        public CustomButton CleanButton;
        public CustomButton DragButton;
        public CustomButton DropButton;
        public DateTime LastDragged;
        public DeadBody CurrentlyDragging;
        public DateTime LastCleaned;

        public Janitor(PlayerControl player) : base(player)
        {
            Name = "Janitor";
            StartText = "Sanitise The Ship, By Any Means Neccessary";
            AbilitiesText = $"- You can clean up dead bodies, making them disappear from sight\n- You can drag bodies away to prevent them from getting reported\n{AbilitiesText}";
            Color = CustomGameOptions.CustomIntColors ? Colors.Janitor : Colors.Intruder;
            RoleType = RoleEnum.Janitor;
            RoleAlignment = RoleAlignment.IntruderConceal;
            AlignmentName = IC;
            InspectorResults = InspectorResults.DealsWithDead;
            CurrentlyDragging = null;
            Type = LayerEnum.Janitor;
            CleanButton = new(this, "Clean", AbilityTypes.Dead, "Secondary", Clean);
            DragButton = new(this, "Drag", AbilityTypes.Dead, "Tertiary", Drag);
            DropButton = new(this, "Drop", AbilityTypes.Effect, "Tertiary", Drop);
        }

        public float CleanTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastCleaned;
            var num = Player.GetModifiedCooldown(CustomGameOptions.JanitorCleanCd, ConstantVariables.LastImp && CustomGameOptions.SoloBoost ? -CustomGameOptions.UnderdogKillBonus : 0) *
                1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public float DragTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastDragged;
            var num = Player.GetModifiedCooldown(CustomGameOptions.DragCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
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
            if (SubmergedCompatibility.IsSubmerged)
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

        public void Clean()
        {
            if (CleanTimer() != 0f || Utils.IsTooFar(Player, CleanButton.TargetBody))
                return;

            var playerId = CleanButton.TargetBody.ParentId;
            var player = Utils.PlayerById(playerId);
            Utils.Spread(Player, player);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.FadeBody);
            writer.Write(playerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            Coroutines.Start(Utils.FadeBody(CleanButton.TargetBody));
            LastCleaned = DateTime.UtcNow;

            if (CustomGameOptions.JaniCooldownsLinked)
                LastKilled = DateTime.UtcNow;
        }

        public void Drag()
        {
            if (Utils.IsTooFar(Player, DragButton.TargetBody) || CurrentlyDragging != null)
                return;

            var playerId = DragButton.TargetBody.ParentId;
            var player = Utils.PlayerById(playerId);
            Utils.Spread(Player, player);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.Drag);
            writer.Write(PlayerId);
            writer.Write(playerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            CurrentlyDragging = DragButton.TargetBody;
        }

        public void Drop()
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.Drop);
            writer.Write(PlayerId);
            Vector3 position = PlayerControl.LocalPlayer.GetTruePosition();

            if (SubmergedCompatibility.IsSubmerged)
            {
                if (position.y > -7f)
                    position.z = 0.0208f;
                else
                    position.z = -0.0273f;
            }

            position.y -= 0.3636f;
            writer.Write(position);
            writer.Write(position.z);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            foreach (var component in CurrentlyDragging?.bodyRenderers)
                component.material.SetFloat("_Outline", 0f);

            CurrentlyDragging.transform.position = position;
            CurrentlyDragging = null;
            LastDragged = DateTime.UtcNow;
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            CleanButton.Update("CLEAN", CleanTimer(), CustomGameOptions.JanitorCleanCd, true, CurrentlyDragging == null);
            DragButton.Update("DRAG", DragTimer(), CustomGameOptions.DragCd, true, CurrentlyDragging == null);
            DropButton.Update("DROP", 0, 1, true, CurrentlyDragging != null);
        }
    }
}