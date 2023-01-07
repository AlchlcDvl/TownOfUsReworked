using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Linq;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Patches;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.CoronerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDPing
    {
        public static Sprite Arrow => TownOfUsReworked.Arrow;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Coroner))
                return;

            var role = Role.GetRole<Coroner>(PlayerControl.LocalPlayer);
            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();

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