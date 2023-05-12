namespace TownOfUsReworked.Cosmetics
{
    [HarmonyPatch]
    public static class CustomNameplates
    {
        private static bool Loaded;
        private static bool Running;
        public readonly static Dictionary<string, NameplateExtension> CustomNameplateRegistry = new();

        private static Sprite CreateNameplateSprite(string path, bool fromDisk = false)
        {
            var texture = fromDisk ? AssetManager.LoadDiskTexture(path) : AssetManager.LoadResourceTexture(path);

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
            {
                var filePath = Path.GetDirectoryName(Application.dataPath) + "\\CustomNameplates\\";
                cn.ID = filePath + cn.ID + ".png";
            }

            var nameplate = ScriptableObject.CreateInstance<NamePlateData>();
            nameplate.viewData.viewData = ScriptableObject.CreateInstance<NamePlateViewData>();
            nameplate.viewData.viewData.Image = CreateNameplateSprite(cn.ID, fromDisk);
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

            return nameplate;
        }

        public class NameplateExtension
        {
            public string Artist { get; set; }
            public string Condition { get; set; }
        }

        public class CustomNameplate
        {
            public string Artist { get; set; }
            public string Condition { get; set; }
            public string Name { get; set; }
            public string ID { get; set; }
        }

        [HarmonyPatch(typeof(HatManager), nameof(HatManager.GetNamePlateById))]
        public static class HatManagerPatch
        {
            private static List<NamePlateData> allPlates = new();

            public static void Prefix(HatManager __instance)
            {
                if (Running)
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
                }
                catch (Exception e)
                {
                    if (!Loaded)
                        Utils.LogSomething("Unable to add Custom Nameplates\n" + e);
                }

                Loaded = true;
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
                    var ext = nameplates[i].Item2;
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
                    __instance.StartCoroutine(nameplate.CoLoadViewData(new Action<NamePlateViewData>(x =>
                    {
                        colorChip.gameObject.GetComponent<NameplateChip>().image.sprite = x.Image;
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

                foreach (string key in keys)
                    YOffset = CreateNameplatePackage(packages[key], key, YOffset, __instance);

                __instance.scroller.ContentYBounds.max = -(YOffset + 3.8f);
                return false;
            }
        }

        public static NameplateExtension GetNameplateExtension(this NamePlateData Nameplate)
        {
            CustomNameplateRegistry.TryGetValue(Nameplate.name, out var ret);
            return ret;
        }
    }
}