using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Classes;
using UnityEngine;
using AmongUs.GameOptions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.UndertakerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDDrag
    {
        public static Sprite Drag => TownOfUsReworked.DragSprite;
        public static Sprite Drop => TownOfUsReworked.DropSprite;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Undertaker))
                return;

            var role = Role.GetRole<Undertaker>(PlayerControl.LocalPlayer);

            if (role.DragDropButton == null)
            {
                role.DragDropButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.DragDropButton.graphic.enabled = true;
                role.DragDropButton.graphic.sprite = Drag;
                role.DragDropButton.gameObject.SetActive(false);
            }

            if ((role.DragDropButton.graphic.sprite != Drag && role.DragDropButton.graphic.sprite != Drop) || (role.DragDropButton.graphic.sprite == Drop && role.CurrentlyDragging == null))
                role.DragDropButton.graphic.sprite = Drag;

            role.DragDropButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));

            if (role.DragDropButton.graphic.sprite == Drag)
            {
                var data = PlayerControl.LocalPlayer.Data;
                var isDead = data.IsDead;
                var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                var maxDistance = GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];
                var flag = GameStates.IsInGame && !GameStates.IsMeeting && PlayerControl.LocalPlayer.CanMove;
                var allocs = Physics2D.OverlapCircleAll(truePosition, maxDistance, LayerMask.GetMask(new[] {"Players", "Ghost"}));
                DeadBody closestBody = null;
                var closestDistance = float.MaxValue;

                foreach (var collider2D in allocs)
                {
                    if (!flag || isDead || collider2D.tag != "DeadBody")
                        continue;

                    var component = collider2D.GetComponent<DeadBody>();

                    if (!(Vector2.Distance(truePosition, component.TruePosition) <= maxDistance))
                        continue;

                    var distance = Vector2.Distance(truePosition, component.TruePosition);

                    if (!(distance < closestDistance))
                        continue;

                    closestBody = component;
                    closestDistance = distance;
                }

                KillButtonTarget.SetTarget(role.DragDropButton, closestBody, role);
            }

            if (role.DragDropButton.graphic.sprite == Drag)
            {
                role.DragDropButton.SetCoolDown(role.DragTimer(), CustomGameOptions.DragCd);

                var renderer1 = role.DragDropButton.graphic;

                if (role.CurrentTarget != null && !role.DragDropButton.isCoolingDown)
                {
                    renderer1.color = Palette.EnabledColor;
                    renderer1.material.SetFloat("_Desat", 0f);
                }
                else
                {
                    renderer1.color = Palette.DisabledClear;
                    renderer1.material.SetFloat("_Desat", 1f);
                }
            }
            else
            {
                role.DragDropButton.SetCoolDown(0f, 1f);
                role.DragDropButton.SetTarget(null);
                role.DragDropButton.graphic.color = Palette.EnabledColor;
                role.DragDropButton.graphic.material.SetFloat("_Desat", 0f);
            }

            if (role.KillButton == null)
            {
                role.KillButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.KillButton.graphic.enabled = true;
                role.KillButton.gameObject.SetActive(false);
            }

            var notImp = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Intruder)).ToList();
            role.KillButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.IntKillCooldown);
            Utils.SetTarget(ref role.ClosestPlayer, role.KillButton, notImp);
            var renderer2 = role.KillButton.graphic;

            if (role.ClosestPlayer != null && !role.KillButton.isCoolingDown)
            {
                renderer2.color = Palette.EnabledColor;
                renderer2.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer2.color = Palette.DisabledClear;
                renderer2.material.SetFloat("_Desat", 1f);
            }
        }
    }
}