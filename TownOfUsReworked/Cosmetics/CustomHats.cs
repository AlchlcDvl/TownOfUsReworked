namespace TownOfUsReworked.Cosmetics
{
    [HarmonyPatch]
    public static class CustomHats
    {
        private static bool Loaded;
        private static bool Running;
        private static Material Shader;
        public readonly static Dictionary<string, HatExtension> CustomHatRegistry = new();

        #pragma warning disable
        public static HatExtension TestExt;
        #pragma warning restore

        public class HatExtension
        {
            public string Artist { get; set; }
            public string Condition { get; set; }
            public Sprite FlipImage { get; set; }
            public Sprite BackFlipImage { get; set; }
        }

        public class CustomHat
        {
            public string Artist { get; set; }
            public string Condition { get; set; }
            public string Name { get; set; }
            public string ID { get; set; }
            public string FlipID { get; set; }
            public string BackflipID { get; set; }
            public string BackID { get; set; }
            public string ClimbID { get; set; }
            public bool NoBouce { get; set; }
            public bool Adaptive { get; set; }
            public bool Behind { get; set; }
        }

        private static List<CustomHat> CreateCustomHatDetails(string[] hats, bool fromDisk = false)
        {
            var fronts = new Dictionary<string, CustomHat>();
            var backs = new Dictionary<string, string>();
            var flips = new Dictionary<string, string>();
            var backflips = new Dictionary<string, string>();
            var climbs = new Dictionary<string, string>();

            foreach (var h in hats)
            {
                var s = fromDisk ? h[(h.LastIndexOf("\\") + 1)..].Split('.')[0] : h.Split('.')[3];
                var p = s.Split('_');
                var options = new HashSet<string>();

                for (var j = 1; j < p.Length; j++)
                    options.Add(p[j]);

                if (options.Contains("back") && options.Contains("flip"))
                    backflips.Add(p[0], h);
                else if (options.Contains("climb"))
                    climbs.Add(p[0], h);
                else if (options.Contains("back"))
                    backs.Add(p[0], h);
                else if (options.Contains("flip"))
                    flips.Add(p[0], h);
                else
                {
                    var custom = new CustomHat
                    {
                        ID = h,
                        Name = p[0].Replace('-', ' '),
                        NoBouce = options.Contains("nobounce"),
                        Adaptive = options.Contains("adaptive"),
                        Behind = options.Contains("behind")
                    };

                    fronts.Add(p[0], custom);
                }
            }

            var customhats = new List<CustomHat>();

            foreach (var k in fronts.Keys)
            {
                var hat = fronts[k];
                backs.TryGetValue(k, out var br);
                climbs.TryGetValue(k, out var cr);
                flips.TryGetValue(k, out var fr);
                backflips.TryGetValue(k, out var bfr);

                if (br != null)
                    hat.BackID = br;

                if (cr != null)
                    hat.ClimbID = cr;

                if (fr != null)
                    hat.FlipID = fr;

                if (bfr != null)
                    hat.BackflipID = bfr;

                if (hat.BackID != null)
                    hat.Behind = true;

                customhats.Add(hat);
            }

            return customhats;
        }

        private static Sprite CreateHatSprite(string path, bool fromDisk = false)
        {
            var texture = fromDisk ? AssetManager.LoadDiskTexture(path) : AssetManager.LoadResourceTexture(path);

            if (texture == null)
                return null;

            var sprite = Sprite.Create(texture, new(0, 0, texture.width, texture.height), new(0.5f, 0.5f), 100);

            if (sprite == null)
                return null;

            texture.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
            sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
            return sprite;
        }

        private static HatData CreateHatBehaviour(CustomHat ch, bool fromDisk = false, bool testOnly = false)
        {
            if (Shader == null)
                Shader = HatManager.Instance.PlayerMaterial;

            if (fromDisk)
            {
                var filePath = Path.GetDirectoryName(Application.dataPath) + "\\CustomHats\\";
                ch.ID = filePath + ch.ID + ".png";

                if (ch.BackID != null)
                    ch.BackID = filePath + ch.BackID + ".png";

                if (ch.ClimbID != null)
                    ch.ClimbID = filePath + ch.ClimbID + ".png";

                if (ch.FlipID != null)
                    ch.FlipID = filePath + ch.FlipID + ".png";

                if (ch.BackflipID != null)
                    ch.BackflipID = filePath + ch.BackflipID + ".png";
            }

            var hat = ScriptableObject.CreateInstance<HatData>();
            hat.hatViewData.viewData = ScriptableObject.CreateInstance<HatViewData>();
            hat.hatViewData.viewData.MainImage = CreateHatSprite(ch.ID, fromDisk);

            if (ch.BackID != null)
            {
                hat.hatViewData.viewData.BackImage = CreateHatSprite(ch.BackID, fromDisk);
                ch.Behind = true; // Required to view backresource
            }

            if (ch.ClimbID != null)
                hat.hatViewData.viewData.ClimbImage = CreateHatSprite(ch.ClimbID, fromDisk);

            hat.name = ch.Name;
            hat.displayOrder = 99;
            hat.ProductId = "hat_" + ch.Name.Replace(' ', '_');
            hat.InFront = !ch.Behind;
            hat.NoBounce = ch.NoBouce;
            hat.ChipOffset = new(0f, 0.2f);
            hat.Free = true;

            if (ch.Adaptive && Shader != null)
                hat.hatViewData.viewData.AltShader = Shader;

            var extend = new HatExtension
            {
                Artist = ch.Artist ?? "Misc",
                Condition = ch.Condition ?? "none"
            };

            if (ch.FlipID != null)
                extend.FlipImage = CreateHatSprite(ch.FlipID, fromDisk);

            if (ch.BackflipID != null)
                extend.BackFlipImage = CreateHatSprite(ch.BackflipID, fromDisk);

            if (testOnly)
            {
                TestExt = extend;
                TestExt.Condition = hat.name;
            }
            else if (!CustomHatRegistry.ContainsKey(hat.name))
                CustomHatRegistry.Add(hat.name, extend);

            return hat;
        }

        [HarmonyPatch(typeof(HatManager), nameof(HatManager.GetHatById))]
        public static class HatManagerPatch
        {
            private static List<HatData> AllHats;

            public static void Prefix(HatManager __instance)
            {
                if (Running)
                    return;

                Running = true; // prevent simultanious execution
                AllHats = __instance.allHats.ToList();

                try
                {
                    while (CosmeticsLoader.HatDetails.Count > 0)
                    {
                        var hatData = CreateHatBehaviour(CosmeticsLoader.HatDetails[0], true);
                        AllHats.Add(hatData);
                        CosmeticsLoader.HatDetails.RemoveAt(0);
                    }

                    __instance.allHats = AllHats.ToArray();
                }
                catch (Exception e)
                {
                    if (!Loaded)
                        Utils.LogSomething("Unable to add Custom Hats\n" + e);
                }

                Loaded = true;
            }

            public static void Postfix() => Running = false;
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleAnimation))]
        public static class PlayerPhysicsHandleAnimationPatch
        {
            public static void Postfix(PlayerPhysics __instance)
            {
                var currentAnimation = __instance.Animations.Animator.GetCurrentAnimation();

                if (currentAnimation == __instance.Animations.group.ClimbUpAnim || currentAnimation == __instance.Animations.group.ClimbDownAnim)
                    return;

                var hp = __instance.myPlayer.cosmetics.hat;

                if (hp.Hat == null)
                    return;

                var extend = hp.Hat.GetHatExtension();

                if (extend == null)
                    return;

                if (extend.FlipImage != null)
                    hp.FrontLayer.sprite = __instance.FlipX ? extend.FlipImage : hp.hatView.MainImage;

                if (extend.BackFlipImage != null)
                    hp.BackLayer.sprite = __instance.FlipX ? extend.BackFlipImage : hp.hatView.BackImage;
            }
        }

        [HarmonyPatch]
        public static class FreeplayHatTestingPatches
        {
            [HarmonyPatch(typeof(HatParent), nameof(HatParent.SetHat), typeof(int))]
            public static class HatParentSetHatPatchColor
            {
                public static void Prefix(HatParent __instance)
                {
                    if (TutorialManager.InstanceExists)
                    {
                        try
                        {
                            var filePath = Path.GetDirectoryName(Application.dataPath) + "\\CustomHats\\Test";

                            if (!Directory.Exists(filePath))
                                Directory.CreateDirectory(filePath);

                            var d = new DirectoryInfo(filePath);
                            var filePaths = d.GetFiles("*.png").Select(x => x.FullName).ToArray(); // Getting Text files
                            var hats = CreateCustomHatDetails(filePaths, true);

                            if (hats.Count > 0)
                                __instance.Hat = CreateHatBehaviour(hats[0], true, true);
                        }
                        catch (Exception e)
                        {
                            Utils.LogSomething("Unable to create test hat\n" + e);
                        }
                    }
                }
            }

            [HarmonyPatch(typeof(HatParent), nameof(HatParent.SetHat), typeof(HatData), typeof(HatViewData), typeof(int))]
            public static class HatParentSetHatPatchExtra
            {
                public static bool Prefix(HatParent __instance, HatData hat, HatViewData hatViewData, int color)
                {
                    if (!TutorialManager.InstanceExists)
                        return true;

                    try
                    {
                        __instance.Hat = hat;
                        __instance.hatView = hatViewData;
                        var filePath = Path.GetDirectoryName(Application.dataPath) + "\\CustomHats\\Test";

                        if (!Directory.Exists(filePath))
                            return true;

                        var d = new DirectoryInfo(filePath);
                        var filePaths = d.GetFiles("*.png").Select(x => x.FullName).ToArray();
                        var hats = CreateCustomHatDetails(filePaths, true);

                        if (hats.Count > 0)
                        {
                            __instance.Hat = CreateHatBehaviour(hats[0], true, true);
                            __instance.hatView = __instance.Hat.hatViewData.viewData;
                        }
                    }
                    catch (Exception e)
                    {
                        Utils.LogSomething("Unable to create test hat\n" + e);
                        return true;
                    }

                    __instance.PopulateFromHatViewData();
                    __instance.SetMaterialColor(color);
                    return false;
                }
            }
        }

        [HarmonyPatch(typeof(HatsTab), nameof(HatsTab.OnEnable))]
        public static class HatsTabOnEnablePatch
        {
            private const string InnerslothPackageName = "Innersloth";
            private static TMP_Text Template;

            public static float CreateHatPackage(List<Tuple<HatData, HatExtension>> hats, string packageName, float YStart, HatsTab __instance)
            {
                var isDefaultPackage = InnerslothPackageName == packageName;

                if (!isDefaultPackage)
                    hats = hats.OrderBy(x => x.Item1.name).ToList();

                var offset = YStart;

                if (Template != null)
                {
                    var title = UObject.Instantiate(Template, __instance.scroller.Inner);
                    title.transform.localPosition = new(2.25f, YStart, -1f);
                    title.transform.localScale = Vector3.one * 1.5f;
                    title.fontSize *= 0.5f;
                    title.enableAutoSizing = false;
                    __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>(_ => title.SetText(packageName))));
                    offset -= 0.8f * __instance.YOffset;
                }

                for (var i = 0; i < hats.Count; i++)
                {
                    var hat = hats[i].Item1;
                    var ext = hats[i].Item2;

                    var xpos = __instance.XRange.Lerp(i % __instance.NumPerRow / (__instance.NumPerRow - 1f));
                    var ypos = offset - (i / __instance.NumPerRow * (isDefaultPackage ? 1f : 1.5f) * __instance.YOffset);
                    var colorChip = UObject.Instantiate(__instance.ColorTabPrefab, __instance.scroller.Inner);

                    if (ActiveInputManager.currentControlType == ActiveInputManager.InputType.Keyboard)
                    {
                        colorChip.Button.OnMouseOver.AddListener((Action)(() => __instance.SelectHat(hat)));
                        colorChip.Button.OnMouseOut.AddListener((Action)(() => __instance.SelectHat(HatManager.Instance.GetHatById(DataManager.Player.Customization.Hat))));
                        colorChip.Button.OnClick.AddListener((Action)(() => __instance.ClickEquip()));
                    }
                    else
                        colorChip.Button.OnClick.AddListener((Action)(() => __instance.SelectHat(hat)));

                    colorChip.Button.ClickMask = __instance.scroller.Hitbox;
                    var background = colorChip.transform.FindChild("Background");
                    var foreground = colorChip.transform.FindChild("ForeGround");

                    if (ext != null)
                    {
                        if (background != null)
                        {
                            background.localPosition = Vector3.down * 0.243f;
                            background.localScale = new(background.localScale.x, 0.8f, background.localScale.y);
                        }

                        if (foreground != null)
                            foreground.localPosition = Vector3.down * 0.243f;

                        if (Template != null)
                        {
                            var description = UObject.Instantiate(Template, colorChip.transform);
                            description.transform.localPosition = new(0f, -0.65f, -1f);
                            description.alignment = TextAlignmentOptions.Center;
                            description.transform.localScale = Vector3.one * 0.65f;
                            __instance.StartCoroutine(Effects.Lerp(0.1f, new Action<float>(_ => description.SetText($"{hat.name}"))));
                        }
                    }

                    colorChip.transform.localPosition = new(xpos, ypos, -1f);
                    colorChip.Inner.SetHat(hat, __instance.HasLocalPlayer() ? PlayerControl.LocalPlayer.Data.DefaultOutfit.ColorId : DataManager.Player.Customization.Color);
                    colorChip.Inner.transform.localPosition = hat.ChipOffset;
                    colorChip.Tag = hat;
                    colorChip.SelectionHighlight.gameObject.SetActive(false);
                    __instance.ColorChips.Add(colorChip);
                }

                return offset - ((hats.Count - 1) / __instance.NumPerRow * (isDefaultPackage ? 1f : 1.5f) * __instance.YOffset) - 1.75f;
            }

            public static bool Prefix(HatsTab __instance)
            {
                for (var i = 0; i < __instance.scroller.Inner.childCount; i++)
                    __instance.scroller.Inner.GetChild(i).gameObject.Destroy();

                __instance.ColorChips = new();

                var unlockedHats = HatManager.Instance.GetUnlockedHats();
                var packages = new Dictionary<string, List<Tuple<HatData, HatExtension>>>();

                foreach (var hatBehaviour in unlockedHats)
                {
                    var ext = hatBehaviour.GetHatExtension();

                    if (ext != null)
                    {
                        if (!packages.ContainsKey(ext.Artist))
                            packages[ext.Artist] = new();

                        packages[ext.Artist].Add(new(hatBehaviour, ext));
                    }
                    else
                    {
                        if (!packages.ContainsKey(InnerslothPackageName))
                            packages[InnerslothPackageName] = new();

                        packages[InnerslothPackageName].Add(new(hatBehaviour, null));
                    }
                }

                var YOffset = __instance.YStart;
                Template = GameObject.Find("HatsGroup").transform.FindChild("Text").GetComponent<TMP_Text>();

                var orderedKeys = packages.Keys.OrderBy(x =>
                {
                    if (x == InnerslothPackageName)
                        return 1000;

                    if (x == "Developer Hats")
                        return 0;

                    return 500;
                });

                foreach (var key in orderedKeys)
                {
                    var value = packages[key];
                    YOffset = CreateHatPackage(value, key, YOffset, __instance);
                }

                foreach (var colorChip in __instance.ColorChips)
                {
                    colorChip.Inner.FrontLayer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                    colorChip.Inner.BackLayer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                    colorChip.SelectionHighlight.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                }

                __instance.scroller.ContentYBounds.max = -(YOffset + 4.1f);
                return false;
            }
        }

        public static HatExtension GetHatExtension(this HatData hat)
        {
            if (TestExt?.Condition.Equals(hat.name) == true)
                return TestExt;

            CustomHatRegistry.TryGetValue(hat.name, out var ret);
            return ret;
        }
    }
}