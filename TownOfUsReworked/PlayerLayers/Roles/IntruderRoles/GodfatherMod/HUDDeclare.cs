using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GodfatherMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDDeclare
    {
        public static Sprite Promote => TownOfUsReworked.Placeholder;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Godfather))
                return;

            var role = Role.GetRole<Godfather>(PlayerControl.LocalPlayer);

            if (role.DeclareButton == null)
            {
                role.DeclareButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.DeclareButton.graphic.enabled = true;
                role.DeclareButton.graphic.sprite = Promote;
                role.DeclareButton.gameObject.SetActive(false);
            }

            if (role.KillButton == null)
            {
                role.KillButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.KillButton.graphic.enabled = true;
                role.KillButton.gameObject.SetActive(false);
            }

            var notImp = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Intruder)).ToList();
            var Imp = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Intruder)).ToList();

            role.DeclareButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && !role.HasDeclared && !role.WasMafioso);
            Utils.SetTarget(ref role.ClosestPlayer, role.DeclareButton, Imp);
        }
    }
}