using HarmonyLib;
using Reactor.Utilities;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.Patches
{
    public class SubmergedPatches
    {
        [HarmonyPatch(typeof(IntroCutscene._ShowRole_d__35), nameof(IntroCutscene._ShowRole_d__35.MoveNext))]
        public static class SubmergedStartPatch
        {
            public static void Postfix(IntroCutscene._ShowRole_d__35 __instance)
            {
                if (SubmergedCompatibility.isSubmerged())
                    Coroutines.Start(SubmergedCompatibility.waitStart(SubmergedCompatibility.resetTimers));
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class SubmergedHudPatch
        {
            public static void Postfix(HudManager __instance)
            {
                if (SubmergedCompatibility.isSubmerged())
                {
                    if (PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer.Is(RoleEnum.Revealer))
                    {
                        if (!Role.GetRole<Revealer>(PlayerControl.LocalPlayer).Caught)
                            __instance.MapButton.transform.parent.Find(__instance.MapButton.name + "(Clone)").gameObject.SetActive(false);
                        else
                            __instance.MapButton.transform.parent.Find(__instance.MapButton.name + "(Clone)").gameObject.SetActive(true);
                    }
                    else if (PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer.Is(RoleEnum.Phantom))
                    {
                        if (!Role.GetRole<Phantom>(PlayerControl.LocalPlayer).Caught)
                            __instance.MapButton.transform.parent.Find(__instance.MapButton.name + "(Clone)").gameObject.SetActive(false);
                        else 
                            __instance.MapButton.transform.parent.Find(__instance.MapButton.name + "(Clone)").gameObject.SetActive(true);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleAnimation))]
        [HarmonyPriority(Priority.Low)] //Mke sure it occurs after other patches
        public static class SubmergedPhysicsPatch
        {
            public static void Postfix(PlayerPhysics __instance)
            {
                SubmergedCompatibility.Ghostrolefix(__instance);
            }
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
        [HarmonyPriority(Priority.Low)] //make sure it occurs after other patches
        public static class SubmergedLateUpdatePhysicsPatch
        {
            public static void Postfix(PlayerPhysics __instance)
            {
                SubmergedCompatibility.Ghostrolefix(__instance);
            }
        }
    }
}