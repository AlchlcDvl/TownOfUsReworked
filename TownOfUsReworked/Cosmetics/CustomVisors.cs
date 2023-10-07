namespace TownOfUsReworked.Cosmetics;

public static class CustomVisors
{
    private static bool SubLoaded;
    private static bool Running;
    private static Material Shader;
    private static readonly Dictionary<string, VisorExtension> CustomVisorRegistry = new();
    private static readonly Dictionary<string, VisorViewData> CustomVisorViewDatas = new();

    private static Sprite CreateVisorSprite(string path, bool fromDisk = false)
    {
        var texture = fromDisk ? LoadDiskTexture(path) : LoadResourceTexture(path);

        if (texture == null)
            return null;

        var sprite = Sprite.Create(texture, new(0f, 0f, texture.width, texture.height), new(0.5f, 0.5f), 100);

        if (sprite == null)
            return null;

        texture.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
        sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
        return sprite;
    }

    private static VisorData CreateVisorBehaviour(CustomVisor cv, bool fromDisk = false)
    {
        if (Shader == null)
            Shader = HatManager.Instance.PlayerMaterial;

        if (fromDisk)
        {
            cv.ID = TownOfUsReworked.Visors + cv.ID + ".png";

            if (cv.FlipID != null)
                cv.FlipID = TownOfUsReworked.Visors + cv.FlipID + ".png";

            if (cv.FloorID != null)
                cv.FloorID = TownOfUsReworked.Visors + cv.FloorID + ".png";

            if (cv.ClimbID != null)
                cv.ClimbID = TownOfUsReworked.Visors + cv.ClimbID + ".png";
        }

        var visor = ScriptableObject.CreateInstance<VisorData>().DontDestroy();
        var viewData = ScriptableObject.CreateInstance<VisorViewData>().DontDestroy();
        viewData.IdleFrame = CreateVisorSprite(cv.ID, fromDisk);
        viewData.FloorFrame = cv.FloorID != null ? CreateVisorSprite(cv.FloorID, fromDisk) : viewData.IdleFrame;
        viewData.LeftIdleFrame = cv.FlipID != null ? CreateVisorSprite(cv.FlipID, fromDisk) : null;
        viewData.ClimbFrame = cv.ClimbID != null ? CreateVisorSprite(cv.ClimbID, fromDisk) : null;
        visor.SpritePreview = viewData.IdleFrame;
        visor.name = cv.Name;
        visor.displayOrder = 99;
        visor.ProductId = "customVisor_" + cv.Name.Replace(' ', '_');
        visor.ChipOffset = new(0f, 0.2f);
        visor.Free = true;
        visor.behindHats = !cv.InFront;

        if (cv.Adaptive && Shader != null)
            viewData.AltShader = Shader;

        var extend = new VisorExtension()
        {
            Artist = cv.Artist ?? "Unknown",
            Condition = cv.Condition ?? "none",
            FlipImage = viewData.LeftIdleFrame
        };

        CustomVisorRegistry.TryAdd(visor.name, extend);
        CustomVisorViewDatas.TryAdd(visor.ProdId, viewData);
        visor.ViewDataRef = new(viewData.Pointer);
        visor.CreateAddressableAsset();
        return visor;
    }

    [HarmonyPatch(typeof(HatManager), nameof(HatManager.GetVisorById))]
    public static class HatManagerPatch
    {
        private static List<VisorData> allVisors = new();

        public static void Prefix(HatManager __instance)
        {
            if (Running || SubLoaded)
                return;

            Running = true;
            allVisors = __instance.allVisors.ToList();

            try
            {
                while (AssetLoader.VisorDetails.Count > 0)
                {
                    var visorData = CreateVisorBehaviour(AssetLoader.VisorDetails[0], true);
                    allVisors.Add(visorData);
                    AssetLoader.VisorDetails.RemoveAt(0);
                }

                __instance.allVisors = allVisors.ToArray();
                SubLoaded = true; //Only loaded if the operation was successful
            }
            catch (Exception e)
            {
                if (!SubLoaded)
                    LogError("Unable to add Custom Visors\n" + e);
            }
        }

        public static void Postfix() => Running = false;
    }

    [HarmonyPatch(typeof(VisorsTab), nameof(VisorsTab.OnEnable))]
    public static class VisorsTabOnEnablePatch
    {
        private static TMP_Text Template;

        public static float CreateVisorPackage(List<VisorData> visors, string packageName, float YStart, VisorsTab __instance)
        {
            visors = visors.OrderBy(x => x.name).ToList();
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

            for (var i = 0; i < visors.Count; i++)
            {
                var visor = visors[i];
                var xpos = __instance.XRange.Lerp(i % __instance.NumPerRow / (__instance.NumPerRow - 1f));
                var ypos = offset - (i / __instance.NumPerRow * __instance.YOffset);
                var colorChip = UObject.Instantiate(__instance.ColorTabPrefab, __instance.scroller.Inner);

                if (ActiveInputManager.currentControlType == ActiveInputManager.InputType.Keyboard)
                {
                    colorChip.Button.OnMouseOver.AddListener((Action)(() => __instance.SelectVisor(visor)));
                    colorChip.Button.OnMouseOut.AddListener((Action)(() => __instance.SelectVisor(HatManager.Instance.GetVisorById(DataManager.Player.Customization.Visor))));
                    colorChip.Button.OnClick.AddListener((Action)(() => __instance.ClickEquip()));
                }
                else
                    colorChip.Button.OnClick.AddListener((Action)(() => __instance.SelectVisor(visor)));

                colorChip.Button.ClickMask = __instance.scroller.Hitbox;
                colorChip.transform.localPosition = new(xpos, ypos, -1f);
                colorChip.Inner.transform.localPosition = visor.ChipOffset;
                colorChip.ProductId = visor.ProductId;
                colorChip.Tag = visor;
                __instance.UpdateMaterials(colorChip.Inner.FrontLayer, visor);
                visor.SetPreview(colorChip.Inner.FrontLayer, __instance.HasLocalPlayer() ? CustomPlayer.LocalCustom.DefaultOutfit.ColorId : DataManager.Player.Customization.Color);
                colorChip.SelectionHighlight.gameObject.SetActive(false);
                __instance.ColorChips.Add(colorChip);
            }

            return offset - ((visors.Count - 1) / __instance.NumPerRow * __instance.YOffset) - 1.75f;
        }

        public static bool Prefix(VisorsTab __instance)
        {
            for (var i = 0; i < __instance.scroller.Inner.childCount; i++)
                __instance.scroller.Inner.GetChild(i).gameObject.Destroy();

            __instance.ColorChips = new();
            var array = HatManager.Instance.GetUnlockedVisors();
            var packages = new Dictionary<string, List<VisorData>>();

            foreach (var data in array)
            {
                var ext = data.GetVisorExtension();
                var package = ext != null ? ext.Artist : "Innersloth";

                if (!packages.ContainsKey(package))
                    packages[package] = new();

                packages[package].Add(data);
            }

            var yOffset = __instance.YStart;
            Template = __instance.transform.FindChild("Text").gameObject.GetComponent<TMP_Text>();
            var keys = packages.Keys.OrderBy(x => x switch
            {
                "Innersloth" => 2,
                _ => 1
            });
            keys.ForEach(key => yOffset = CreateVisorPackage(packages[key], key, yOffset, __instance));
            __instance.scroller.ContentYBounds.max = -(yOffset + 4.1f);

            foreach (var colorChip in __instance.ColorChips)
            {
                colorChip.Inner.FrontLayer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                colorChip.Inner.BackLayer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                colorChip.SelectionHighlight.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(CosmeticsCache), nameof(CosmeticsCache.GetVisor))]
    public static class CosmeticsCacheGetVisorPatch
    {
        public static bool Prefix(CosmeticsCache __instance, string id, ref VisorViewData __result)
        {
            if (!CustomVisorViewDatas.TryGetValue(id, out __result))
                return true;

            if (__result == null)
                __result = __instance.visors["visor_EmptyVisor"].GetAsset();

            return false;
        }
    }

    [HarmonyPatch(typeof(VisorLayer), nameof(VisorLayer.UpdateMaterial))]
    public static class VisorLayerUpdateMaterialPatch
    {
        public static bool Prefix(VisorLayer __instance)
        {
            if (__instance.currentVisor == null || !__instance.currentVisor.ProductId.StartsWith("customVisor_"))
                return true;

            var asset = CustomVisorViewDatas.TryGetValue(__instance.currentVisor.ProdId, out var result) ? result : null;
            __instance.Image.sharedMaterial = asset.AltShader ?? HatManager.Instance.DefaultShader;
            PlayerMaterial.SetColors(__instance.matProperties.ColorId, __instance.Image);

            __instance.Image.maskInteraction = __instance.matProperties.MaskType switch
            {
                PlayerMaterial.MaskType.SimpleUI or PlayerMaterial.MaskType.ScrollingUI => SpriteMaskInteraction.VisibleInsideMask,
                PlayerMaterial.MaskType.Exile => SpriteMaskInteraction.VisibleOutsideMask,
                _ => SpriteMaskInteraction.None,
            };

            return false;
        }
    }

    [HarmonyPatch(typeof(VisorLayer), nameof(VisorLayer.SetFlipX))]
    public static class VisorLayerSetFlipXPatch
    {
        public static bool Prefix(VisorLayer __instance, bool flipX)
        {
            if (__instance.currentVisor == null || !__instance.currentVisor.ProductId.StartsWith("customVisor_"))
                return true;

            __instance.Image.flipX = flipX;
            var asset = CustomVisorViewDatas.TryGetValue(__instance.currentVisor.ProdId, out var result) ? result : null;
            __instance.Image.sprite = flipX && asset.LeftIdleFrame ? asset.LeftIdleFrame : asset.IdleFrame;
            return false;
        }
    }

    [HarmonyPatch(typeof(VisorLayer), nameof(VisorLayer.SetVisor), new Type[] { typeof(VisorData), typeof(VisorViewData), typeof(int) })]
    public static class VisorLayerSetVisorPositionPatch
    {
        public static bool Prefix(VisorLayer __instance, VisorData data, VisorViewData visorView, int colorId)
        {
            if (!data.ProductId.StartsWith("customVisor_"))
                return true;

            __instance.currentVisor = data;
            __instance.transform.SetLocalZ(__instance.ZIndexSpacing * (data.BehindHats ? -1.5f : -3f));
            __instance.SetFlipX(__instance.Image.flipX);
            __instance.SetMaterialColor(colorId);
            return false;
        }
    }

    [HarmonyPatch(typeof(VisorLayer), nameof(VisorLayer.SetVisor), new Type[] { typeof(VisorData), typeof(int) })]
    public static class VisorLayerSetVisorPatch
    {
        public static bool Prefix(VisorLayer __instance, VisorData data, int colorId)
        {
            if (!CustomVisorViewDatas.TryGetValue(data.ProductId, out var asset))
                return true;

            __instance.currentVisor = data;
            __instance.SetVisor(__instance.currentVisor, asset, colorId);
            return false;
        }
    }

    public static VisorExtension GetVisorExtension(this VisorData visor)
    {
        CustomVisorRegistry.TryGetValue(visor.name, out var ret);
        return ret;
    }
}