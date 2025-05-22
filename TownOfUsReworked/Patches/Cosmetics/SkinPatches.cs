// namespace TownOfUsReworked.Patches.Cosmetics;

// [HarmonyPatch(typeof(SkinsTab))]
// public static class SkinTabOnEnablePatch
// {
//     private static TMP_Text Template;

//     private static void CreateNameplatePackage(List<SkinData> skins, string packageName, ref float yStart, SkinsTab __instance)
//     {
//         var isDefaultPackage = packageName == "Innersloth";

//         if (!isDefaultPackage)
//             skins = [ .. skins.OrderBy(x => x.name) ];

//         var offset = yStart;

//         if (Template)
//         {
//             var title = UObject.Instantiate(Template, __instance.scroller.Inner);
//             var material = title.GetComponent<MeshRenderer>().material;
//             material.SetFloat(StencilComp, 4f);
//             material.SetFloat(Stencil, 1f);
//             title.transform.localPosition = new(2.25f, offset, -1f);
//             title.transform.localScale = Vector3.one * 1.5f;
//             title.fontSize *= 0.5f;
//             title.enableAutoSizing = false;
//             title.GetComponent<TextTranslatorTMP>().Destroy();
//             title.text = packageName;
//             offset -= 0.8f * __instance.YOffset;
//         }

//         for (var i = 0; i < skins.Count; i++)
//         {
//             var skin = skins[i];
//             var xpos = __instance.XRange.Lerp(i % __instance.NumPerRow / (__instance.NumPerRow - 1f));
//             var ypos = offset - (i / __instance.NumPerRow * __instance.YOffset);
//             var colorChip = UObject.Instantiate(__instance.ColorTabPrefab, __instance.scroller.Inner);

//             if (ActiveInputManager.currentControlType == ActiveInputManager.InputType.Keyboard)
//             {
//                 colorChip.Button.OverrideOnMouseOverListeners(() => __instance.SelectSkin(skin));
//                 colorChip.Button.OverrideOnMouseOutListeners(() => __instance.SelectSkin(HatManager.Instance.GetSkinById(DataManager.Player.Customization.Skin)));
//                 colorChip.Button.OverrideOnClickListeners(__instance.ClickEquip);
//             }
//             else
//                 colorChip.Button.OverrideOnClickListeners(() => __instance.SelectSkin(skin));

//             colorChip.Button.ClickMask = __instance.scroller.Hitbox;
//             colorChip.transform.localPosition = new(xpos, ypos, -1f);
//             colorChip.ProductId = skin.ProductId;
//             colorChip.Tag = skin;
//             colorChip.SelectionHighlight.gameObject.SetActive(false);
//             skin.SetPreview(colorChip.Inner.FrontLayer, __instance.HasLocalPlayer() ? PlayerControl.LocalPlayer.Data.DefaultOutfit.ColorId : DataManager.Player.Customization.Color);

//             if (!HatManager.Instance.CheckLongModeValidCosmetic(skin.ProdId, __instance.PlayerPreview.GetIgnoreLongMode()))
//                 colorChip.SetUnavailable();

//             if (SkinLoader.CustomCosmeticRegistry.ContainsKey(skin.ProductId))
//                 colorChip.transform.FindRecursive("HatParent").localPosition = new(-0.1f, 0.4f, -2f);

//             __instance.ColorChips.Add(colorChip);
//             yStart = ypos;
//         }

//         yStart -= 1.5f;
//     }

//     [HarmonyPatch(nameof(SkinsTab.OnEnable))]
//     public static bool Prefix(SkinsTab __instance)
//     {
//         __instance.PlayerPreview.gameObject.SetActive(true);

//         if (__instance.HasLocalPlayer())
//             __instance.PlayerPreview.UpdateFromLocalPlayer(PlayerMaterial.MaskType.None);
//         else
//             __instance.PlayerPreview.UpdateFromDataManager(PlayerMaterial.MaskType.None);

//         __instance.scroller.Inner.DestroyChildren();
//         __instance.ColorChips = new();
//         var array = HatManager.Instance.GetUnlockedSkins();
//         var packages = new Dictionary<string, List<SkinData>>();

//         foreach (var data in array)
//         {
//             var package = "Innersloth";

//             if (SkinLoader.CustomCosmeticRegistry.TryGetValue(data.ProductId, out var cn))
//                 package = cn.StreamOnly ? "Stream" : cn.Artist;

//             if (IsNullEmptyOrWhiteSpace(package))
//                 package = "Misc";

//             if (!packages.TryGetValue(package, out var value))
//                 packages[package] = value = [];

//             value.Add(data);
//         }

//         Template = __instance.transform.FindChild("Text").GetComponent<TMP_Text>();
//         var keys = packages.Keys.OrderBy(x => x switch
//         {
//             "Innersloth" => 4,
//             "Stream" => 1,
//             "Misc" => 3,
//             _ => 2
//         });
//         var yOffset = __instance.YStart;
//         keys.Do(key => CreateNameplatePackage(packages[key], key, ref yOffset, __instance));
//         __instance.skinId = DataManager.Player.Customization.NamePlate;
//         __instance.currentSkinIsEquipped = true;
//         __instance.scroller.ContentYBounds.max = -(yOffset + 3.8f);
//         __instance.scroller.UpdateScrollBars();

//         if (array.Length != 0)
//             __instance.GetDefaultSelectable().PlayerEquippedForeground.SetActive(true);

//         return false;
//     }
// }