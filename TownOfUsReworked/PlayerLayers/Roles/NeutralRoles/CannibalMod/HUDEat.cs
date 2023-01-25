using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Linq;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using AmongUs.GameOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.CannibalMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDEat
    {
        public static Sprite Arrow => TownOfUsReworked.Arrow;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Cannibal))
                return;

            var role = Role.GetRole<Cannibal>(PlayerControl.LocalPlayer);
            
            if (role.EatButton == null)
            {
                role.EatButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.EatButton.graphic.enabled = true;
                role.EatButton.gameObject.SetActive(false);
            }

            role.EatButton.GetComponent<AspectPosition>().Update();
            role.EatButton.graphic.sprite = TownOfUsReworked.CannibalEat;
            role.EatButton.gameObject.SetActive(!MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead && !LobbyBehaviour.Instance);

            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            var maxDistance = GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];
            var flag = (CustomGameOptions.GhostTasksCountToWin || !data.IsDead) && (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) &&
                PlayerControl.LocalPlayer.CanMove;
            var allocs = Physics2D.OverlapCircleAll(truePosition, maxDistance, LayerMask.GetMask(new[] {"Players", "Ghost"}));
            var killButton = role.EatButton;
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

            KillButtonTarget.SetTarget(killButton, closestBody, role);
            killButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance && !LobbyBehaviour.Instance);
            role.EatButton.SetCoolDown(role.EatTimer(), CustomGameOptions.CannibalCd);

            if (CustomGameOptions.EatArrows && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var validBodies = Object.FindObjectsOfType<DeadBody>().Where(x => Murder.KilledPlayers.Any(y => y.PlayerId == x.ParentId &&
                    y.KillTime.AddSeconds(CustomGameOptions.EatArrowDelay) < System.DateTime.UtcNow));

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