using HarmonyLib;
using Reactor.Utilities;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Data;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch]
    public static class SubmergedPatches
    {
        [HarmonyPatch(typeof(IntroCutscene._ShowRole_d__39), nameof(IntroCutscene._ShowRole_d__39.MoveNext))]
        public static class SubmergedStartPatch
        {
            public static void Postfix()
            {
                if (SubmergedCompatibility.IsSubmerged())
                    Coroutines.Start(SubmergedCompatibility.WaitStart(() => SubmergedCompatibility.ResetTimers(false)));
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class SubmergedHudPatch
        {
            public static void Postfix(HudManager __instance)
            {
                if (SubmergedCompatibility.IsSubmerged())
                {
                    if (PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer.Is(RoleEnum.Revealer))
                        __instance.MapButton.transform.parent.Find(__instance.MapButton.name + "(Clone)").gameObject.SetActive(Role.GetRole<Revealer>(PlayerControl.LocalPlayer).Caught);
                    else if (PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer.Is(RoleEnum.Phantom))
                        __instance.MapButton.transform.parent.Find(__instance.MapButton.name + "(Clone)").gameObject.SetActive(Role.GetRole<Phantom>(PlayerControl.LocalPlayer).Caught);
                    else if (PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer.Is(RoleEnum.Ghoul))
                        __instance.MapButton.transform.parent.Find(__instance.MapButton.name + "(Clone)").gameObject.SetActive(Role.GetRole<Ghoul>(PlayerControl.LocalPlayer).Caught);
                    else if (PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer.Is(RoleEnum.Banshee))
                        __instance.MapButton.transform.parent.Find(__instance.MapButton.name + "(Clone)").gameObject.SetActive(Role.GetRole<Banshee>(PlayerControl.LocalPlayer).Caught);
                }
            }
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleAnimation))]
        [HarmonyPriority(Priority.Low)] //Mke sure it occurs after other patches
        public static class SubmergedPhysicsPatch
        {
            public static void Postfix(PlayerPhysics __instance) => SubmergedCompatibility.Ghostrolefix(__instance);
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
        [HarmonyPriority(Priority.Low)] //make sure it occurs after other patches
        public static class SubmergedLateUpdatePhysicsPatch
        {
            public static void Postfix(PlayerPhysics __instance) => SubmergedCompatibility.Ghostrolefix(__instance);
        }
    }
}