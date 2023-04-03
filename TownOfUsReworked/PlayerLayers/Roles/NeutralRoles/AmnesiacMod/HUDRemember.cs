using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Linq;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.AmnesiacMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDRemember
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Amnesiac))
                return;

            var role = Role.GetRole<Amnesiac>(PlayerControl.LocalPlayer);

            if (role.RememberButton == null)
                role.RememberButton = CustomButtons.InstantiateButton();

            role.RememberButton.UpdateButton(role, "REMEMBER", 0, 1, AssetManager.Remember, AbilityTypes.Dead, "ActionSecondary");

            if (CustomGameOptions.RememberArrows && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var validBodies = Object.FindObjectsOfType<DeadBody>().Where(x => Murder.KilledPlayers.Any(y => y.PlayerId == x.ParentId &&
                    y.KillTime.AddSeconds(CustomGameOptions.RememberArrowDelay) < System.DateTime.UtcNow));

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
                        renderer.sprite = AssetManager.Arrow;
                        arrow.image = renderer;
                        gameObj.layer = 5;
                        role.BodyArrows.Add(body.ParentId, arrow);
                    }

                    role.BodyArrows.GetValueSafe(body.ParentId).target = body.TruePosition;
                }
            }
            else if (role.BodyArrows.Count != 0)
            {
                role.BodyArrows.Values.DestroyAll();
                role.BodyArrows.Clear();
            }
        }
    }
}