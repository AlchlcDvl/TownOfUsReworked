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
                if (ModCompatibility.IsSubmerged)
                    Coroutines.Start(ModCompatibility.WaitStart(() => ButtonUtils.ResetCustomTimers(false)));
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class SubmergedHudPatch
        {
            public static void Postfix(HudManager __instance)
            {
                if (ModCompatibility.IsSubmerged && PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer.IsPostmortal())
                    __instance.MapButton.transform.parent.Find(__instance.MapButton.name + "(Clone)").gameObject.SetActive(PlayerControl.LocalPlayer.Caught());
            }
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleAnimation))]
        [HarmonyPriority(Priority.Low)] //Make sure it occurs after other patches
        public static class SubmergedPhysicsPatch
        {
            public static void Postfix(PlayerPhysics __instance) => ModCompatibility.Ghostrolefix(__instance);
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.LateUpdate))]
        [HarmonyPriority(Priority.Low)] //Make sure it occurs after other patches
        public static class SubmergedLateUpdatePhysicsPatch
        {
            public static void Postfix(PlayerPhysics __instance) => ModCompatibility.Ghostrolefix(__instance);
        }
    }
}