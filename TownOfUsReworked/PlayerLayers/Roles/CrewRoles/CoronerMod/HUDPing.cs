using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Linq;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Modules;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.CoronerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDPing
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Coroner))
                return;

            var role = Role.GetRole<Coroner>(PlayerControl.LocalPlayer);

            if (role.AutopsyButton == null)
                role.AutopsyButton = CustomButtons.InstantiateButton();

            role.AutopsyButton.UpdateButton(role, "AUTOPSY", role.AutopsyTimer(), 10, AssetManager.Placeholder, AbilityTypes.Dead, "ActionSecondary");

            if (role.CompareButton == null)
                role.CompareButton = CustomButtons.InstantiateButton();

            role.CompareButton.UpdateButton(role, "COMPARE", role.CompareTimer(), CustomGameOptions.CompareCooldown, AssetManager.Placeholder, AbilityTypes.Direct, "Secondary", null,
                true, role.UsesLeft, role.ReferenceBody != null, role.ButtonUsable);

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