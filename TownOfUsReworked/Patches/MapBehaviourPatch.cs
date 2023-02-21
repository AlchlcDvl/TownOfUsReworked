using HarmonyLib;
using TownOfUsReworked.PlayerLayers.Roles;

namespace TownOfUsReworked.Patches
{
	[HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.FixedUpdate))]
	class MapBehaviourPatch
    {
		static void Postfix(MapBehaviour __instance)
        {
			var role = Role.GetRole(PlayerControl.LocalPlayer);

            if (role != null)
            {
                __instance.ColorControl.baseColor = role.Color;
                __instance.ColorControl.SetColor(role.Color);
            }
		}
	}
}
