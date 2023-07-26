/*using Innersloth.Assets;

namespace TownOfUsReworked.Cosmetics
{
    [HarmonyPatch]
    public static class CustomNameplates
    {
        private static bool SubLoaded;
        private static bool Running;
        public static readonly Dictionary<string, NameplateExtension> CustomNameplateRegistry = new();
        public static readonly Dictionary<string, NamePlateViewData> CustomNameplateViewDatas = new();

        private static Sprite CreateNameplateSprite(string path, bool fromDisk = false)
        {
            var texture = fromDisk ? LoadDiskTexture(path) : LoadResourceTexture(path);

            if (texture == null)
                return null;

            var sprite = Sprite.Create(texture, new(0f, 0f, texture.width, texture.height), new(0.5f, 0.5f), texture.width * 0.375f);

            if (sprite == null)
                return null;

            texture.hideFlags |= HideFlags.HideAndDontSave;
            sprite.hideFlags |= HideFlags.HideAndDontSave;
            return sprite;
        }

        private static NamePlateData CreateNameplateBehaviour(CustomNameplate cn, bool fromDisk = false)
        {
            if (fromDisk)
                cn.ID = TownOfUsReworked.Nameplates + cn.ID + ".png";

            var nameplate = ScriptableObject.CreateInstance<NamePlateData>();
            var viewData = ScriptableObject.CreateInstance<NamePlateViewData>();
            viewData.Image = CreateNameplateSprite(cn.ID, fromDisk);
            nameplate.name = cn.Name;
            nameplate.displayOrder = 99;
            nameplate.ProductId = "nameplate_" + cn.Name.Replace(' ', '_');
            nameplate.ChipOffset = new(0f, 0.2f);
            nameplate.Free = true;

            var extend = new NameplateExtension
            {
                Artist = cn.Artist ?? "Unknown",
                Condition = cn.Condition ?? "none"
            };

            if (!CustomNameplateRegistry.ContainsKey(nameplate.name))
                CustomNameplateRegistry.Add(nameplate.name, extend);

            if (!CustomNameplateViewDatas.ContainsKey(nameplate.name))
                CustomNameplateViewDatas.Add(nameplate.name, viewData);

            nameplate.ViewDataRef = new(viewData.Pointer);
            nameplate.CreateAddressableAsset();
            return nameplate;
        }

        [HarmonyPatch(typeof(HatManager), nameof(HatManager.GetNamePlateById))]
        public static class HatManagerPatch
        {
            private static List<NamePlateData> allPlates = new();

            public static void Prefix(HatManager __instance)
            {
                if (Running || SubLoaded)
                    return;

                Running = true;
                allPlates = __instance.allNamePlates.ToList();

                try
                {
                    while (CosmeticsLoader.NameplateDetails.Count > 0)
                    {
                        var nameplateData = CreateNameplateBehaviour(CosmeticsLoader.NameplateDetails[0], true);
                        allPlates.Add(nameplateData);
                        CosmeticsLoader.NameplateDetails.RemoveAt(0);
                    }

                    __instance.allNamePlates = allPlates.ToArray();
                    SubLoaded = true; //Only loaded if the operation was successful
                }
                catch (Exception e)
                {
                    if (!SubLoaded)
                        LogSomething("Unable to add Custom Nameplates\n" + e);
                }
            }

            public static void Postfix() => Running = false;
        }

        [HarmonyPatch(typeof(NameplatesTab), nameof(NameplatesTab.OnEnable))]
        public static class NameplatesTabOnEnablePatch
        {
            public const string InnerslothPackageName = "Innersloth";
            private static TMP_Text Template;

            public static float CreateNameplatePackage(List<Tuple<NamePlateData, NameplateExtension>> nameplates, string packageName, float YStart, NameplatesTab __instance)
            {
                if (packageName != InnerslothPackageName)
                    nameplates = nameplates.OrderBy(x => x.Item1.name).ToList();

                var offset = YStart;

                if (Template != null)
                {
                    var title = UObject.Instantiate(Template, __instance.scroller.Inner);
                    var material = title.GetComponent<MeshRenderer>().material;
                    material.SetFloat("_StencilComp", 4f);
                    material.SetFloat("_Stencil", 1f);
                    title.transform.localPosition = new(2.25f, YStart, -1f);
                    title.transform.localScale = Vector3.one * 1.5f;
                    title.fontSize *= 0.5f;
                    title.enableAutoSizing = false;
                    __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>(_ => title.SetText(packageName, true))));
                    offset -= 0.8f * __instance.YOffset;
                }

                for (var i = 0; i < nameplates.Count; i++)
                {
                    var nameplate = nameplates[i].Item1;
                    var xpos = __instance.XRange.Lerp(i % __instance.NumPerRow / (__instance.NumPerRow - 1f));
                    var ypos = offset - (i / __instance.NumPerRow * __instance.YOffset);
                    var colorChip = UObject.Instantiate(__instance.ColorTabPrefab, __instance.scroller.Inner);

                    if (ActiveInputManager.currentControlType == ActiveInputManager.InputType.Keyboard)
                    {
                        colorChip.Button.OnMouseOver.AddListener((Action)(() => __instance.SelectNameplate(nameplate)));
                        colorChip.Button.OnMouseOut.AddListener((Action)(() =>
                            __instance.SelectNameplate(HatManager.Instance.GetNamePlateById(DataManager.Player.Customization.NamePlate))));
                        colorChip.Button.OnClick.AddListener((Action)(() => __instance.ClickEquip()));
                    }
                    else
                        colorChip.Button.OnClick.AddListener((Action)(() => __instance.SelectNameplate(nameplate)));

                    colorChip.Button.ClickMask = __instance.scroller.Hitbox;
                    colorChip.transform.localPosition = new(xpos, ypos, -1f);
                    __instance.StartCoroutine(nameplate.CoLoadIcon(new Action<Sprite, AddressableAsset>((_, _) =>
                    {
                        colorChip.gameObject.GetComponent<NameplateChip>().image.sprite = CustomNameplateViewDatas.TryGetValue(nameplate.name, out var data) ? data.Image :
                            ShipStatus.Instance.CosmeticsCache.GetNameplate(nameplate.ProdId).Image;
                        colorChip.gameObject.GetComponent<NameplateChip>().ProductId = nameplate.ProductId;
                    })));
                    __instance.ColorChips.Add(colorChip);
                }

                return offset - ((nameplates.Count - 1) / __instance.NumPerRow * __instance.YOffset) - 1.5f;
            }

            public static bool Prefix(NameplatesTab __instance)
            {
                for (var i = 0; i < __instance.scroller.Inner.childCount; i++)
                    __instance.scroller.Inner.GetChild(i).gameObject.Destroy();

                __instance.ColorChips = new();
                var array = HatManager.Instance.GetUnlockedNamePlates();
                var packages = new Dictionary<string, List<Tuple<NamePlateData, NameplateExtension>>>();

                foreach (var data in array)
                {
                    var ext = data.GetNameplateExtension();

                    if (ext != null)
                    {
                        if (!packages.ContainsKey(ext.Artist))
                            packages[ext.Artist] = new();

                        packages[ext.Artist].Add(new(data, ext));
                    }
                    else
                    {
                        if (!packages.ContainsKey(InnerslothPackageName))
                            packages[InnerslothPackageName] = new();

                        packages[InnerslothPackageName].Add(new(data, null));
                    }
                }

                var YOffset = __instance.YStart;
                Template = __instance.transform.FindChild("Text").gameObject.GetComponent<TMP_Text>();
                var keys = packages.Keys.OrderBy(x =>
                {
                    if (x == InnerslothPackageName)
                        return 1000;

                    if (x == "Developer Visors")
                        return 0;

                    return 500;
                });

                foreach (var key in keys)
                    YOffset = CreateNameplatePackage(packages[key], key, YOffset, __instance);

                __instance.scroller.ContentYBounds.max = -(YOffset + 3.8f);
                return false;
            }
        }

        [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.PreviewNameplate))]
        public static class PreviewNameplatePatch
        {
            public static void Postfix(PlayerVoteArea __instance, string plateID)
            {
                if (!CustomNameplateViewDatas.ContainsKey(plateID))
                    return;

                var npvd = CustomNameplateViewDatas[plateID];

                if (npvd != null)
                    __instance.Background.sprite = npvd.Image;
            }
        }

        [HarmonyPatch(typeof(CosmeticsCache), nameof(CosmeticsCache.GetNameplate))]
        public static class CosmeticsCacheGetPlatePatch
        {
            public static bool Prefix(CosmeticsCache __instance, string id, ref NamePlateViewData __result)
            {
                if (!CustomNameplateViewDatas.ContainsKey(id))
                    return true;

                __result = CustomNameplateViewDatas[id];

                if (__result == null)
                    __result = __instance.nameplates["nameplate_NoPlate"].GetAsset();

                return false;
            }
        }

        public static NameplateExtension GetNameplateExtension(this NamePlateData Nameplate)
        {
            CustomNameplateRegistry.TryGetValue(Nameplate.name, out var ret);
            return ret;
        }
    }
}*/