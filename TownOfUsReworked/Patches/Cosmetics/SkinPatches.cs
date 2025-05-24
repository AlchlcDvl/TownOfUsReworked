// namespace TownOfUsReworked.Patches.Cosmetics;

// [HarmonyPatch(typeof(SkinsTab), nameof(SkinsTab.OnEnable))]
// public static class SkinTabOnEnablePatch
// {
//     public static bool Prefix(SkinsTab __instance)
//     {
//         __instance.BaseOnEnable();
//         var array = HatManager.Instance.GetUnlockedSkins();
//         var packages = CosmeticTabPatches.GeneratePackages<SkinData, CustomSkin, SkinViewData>(array, SkinLoader.CustomCosmeticRegistry);
//         __instance.CreatePackages(packages, false, __instance.SelectSkin, () => __instance.SelectSkin(HatManager.Instance.GetSkinById(DataManager.Player.Customization.Skin)), out var yOffset);
//         __instance.skinId = DataManager.Player.Customization.Skin;
//         __instance.currentSkinIsEquipped = true;
//         __instance.EndOnEnable(yOffset, array);
//         return false;
//     }
// }