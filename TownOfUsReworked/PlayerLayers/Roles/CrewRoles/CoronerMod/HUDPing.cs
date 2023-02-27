using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Linq;
using TownOfUsReworked.Patches;
using AmongUs.GameOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.CoronerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDPing
    {
        public static Sprite Arrow => TownOfUsReworked.Arrow;
        public static Sprite Placeholder => TownOfUsReworked.Placeholder;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Coroner))
                return;

            var role = Role.GetRole<Coroner>(PlayerControl.LocalPlayer);
            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            var maxDistance = GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];
            var flag = (CustomGameOptions.GhostTasksCountToWin || !data.IsDead) && (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) && PlayerControl.LocalPlayer.CanMove;
            DeadBody closestBody = null;
            var closestDistance = float.MaxValue;
            var allBodies = Object.FindObjectsOfType<DeadBody>();

            if (role.AutopsyButton == null)
            {
                role.AutopsyButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.AutopsyButton.graphic.enabled = true;
                role.AutopsyButton.graphic.sprite = Placeholder;
                role.AutopsyButton.gameObject.SetActive(false);
            }

            if (role.CompareButton == null)
            {
                role.CompareButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.CompareButton.graphic.enabled = true;
                role.CompareButton.graphic.sprite = Placeholder;
                role.CompareButton.gameObject.SetActive(false);
            }

            foreach (var body in allBodies.Where(x => Vector2.Distance(x.TruePosition, truePosition) <= maxDistance))
            {
                var distance = Vector2.Distance(truePosition, body.TruePosition);

                if (!(distance < closestDistance))
                    continue;

                closestBody = body;
                closestDistance = distance;
            }

            KillButtonTarget.SetTarget(role.AutopsyButton, closestBody, role);
            role.AutopsyButton.SetCoolDown(0f, 1f);
            role.AutopsyButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.PrimaryButton = role.AutopsyButton;
            var renderer2 = role.AutopsyButton.graphic;

            if (role.CurrentTarget != null)
            {
                renderer2.color = Palette.EnabledColor;
                renderer2.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer2.color = Palette.DisabledClear;
                renderer2.material.SetFloat("_Desat", 1f);
            }

            var renderer3 = role.CompareButton.graphic;

            if (!role.CompareButton.isCoolingDown && role.ClosestPlayer != null && role.ReferenceBody != null)
            {
                renderer3.color = Palette.EnabledColor;
                renderer3.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer3.color = Palette.DisabledClear;
                renderer3.material.SetFloat("_Desat", 1f);
            }

            if (!PlayerControl.LocalPlayer.Data.IsDead)
            {
                var validBodies = Object.FindObjectsOfType<DeadBody>().Where(x => Murder.KilledPlayers.Any(y => y.PlayerId == x.ParentId &&
                    y.KillTime.AddSeconds(CustomGameOptions.CoronerArrowDuration) > System.DateTime.UtcNow));

                foreach (var bodyArrow in role.BodyArrows.Keys)
                {
                    if (!validBodies.Any(x => x.ParentId == bodyArrow))
                        role.DestroyArrow(bodyArrow);
                }

                foreach (var body in validBodies)
                {
                    if (!role.BodyArrows.ContainsKey(body.ParentId))
                    {
                        var gameObj = new GameObject();
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = Arrow;
                        arrow.image = renderer;
                        gameObj.layer = 5;
                        role.BodyArrows.Add(body.ParentId, arrow);
                    }
                    
                    role.BodyArrows.GetValueSafe(body.ParentId).target = body.TruePosition;
                }
            }
            else
            {
                if (role.BodyArrows.Count != 0)
                {
                    role.BodyArrows.Values.DestroyAll();
                    role.BodyArrows.Clear();
                }
            }
        }
    }
}