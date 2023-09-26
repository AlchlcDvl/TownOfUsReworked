/*namespace TownOfUsReworked.Cosmetics;

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

        texture.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
        sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
        return sprite;
    }

    private static NamePlateData CreateNameplateBehaviour(CustomNameplate cn, bool fromDisk = false)
    {
        if (fromDisk)
            cn.ID = TownOfUsReworked.Nameplates + cn.ID + ".png";

        var nameplate = ScriptableObject.CreateInstance<NamePlateData>().DontDestroy();
        var viewData = ScriptableObject.CreateInstance<NamePlateViewData>().DontDestroy();
        viewData.Image = CreateNameplateSprite(cn.ID, fromDisk);
        nameplate.SpritePreview = viewData.Image;
        nameplate.PreviewCrewmateColor = false;
        nameplate.name = cn.Name;
        nameplate.displayOrder = 99;
        nameplate.ProductId = "customNameplate_" + cn.Name.Replace(' ', '_');
        nameplate.ChipOffset = new(0f, 0.2f);
        nameplate.Free = true;

        var extend = new NameplateExtension()
        {
            Artist = cn.Artist ?? "Unknown",
            Condition = cn.Condition ?? "none"
        };

        CustomNameplateRegistry.TryAdd(nameplate.name, extend);
        CustomNameplateViewDatas.TryAdd(nameplate.name, viewData);
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
                while (AssetLoader.NameplateDetails.Count > 0)
                {
                    var nameplateData = CreateNameplateBehaviour(AssetLoader.NameplateDetails[0], true);
                    allPlates.Add(nameplateData);
                    AssetLoader.NameplateDetails.RemoveAt(0);
                }

                __instance.allNamePlates = allPlates.ToArray();
                SubLoaded = true; //Only loaded if the operation was successful
            }
            catch (Exception e)
            {
                if (!SubLoaded)
                    LogError("Unable to add Custom Nameplates\n" + e);
            }
        }

        public static void Postfix() => Running = false;
    }

    [HarmonyPatch(typeof(NameplatesTab), nameof(NameplatesTab.OnEnable))]
    public static class NameplatesTabOnEnablePatch
    {
        private static TMP_Text Template;

        public static float CreateNameplatePackage(List<NamePlateData> nameplates, string packageName, float YStart, NameplatesTab __instance)
        {
            nameplates = nameplates.OrderBy(x => x.name).ToList();
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
                var nameplate = nameplates[i];
                var xpos = __instance.XRange.Lerp(i % __instance.NumPerRow / (__instance.NumPerRow - 1f));
                var ypos = offset - (i / __instance.NumPerRow * __instance.YOffset);
                var colorChip = UObject.Instantiate(__instance.ColorTabPrefab, __instance.scroller.Inner);

                if (ActiveInputManager.currentControlType == ActiveInputManager.InputType.Keyboard)
                {
                    colorChip.Button.OnMouseOver.AddListener((Action)(() => __instance.SelectNameplate(nameplate)));
                    colorChip.Button.OnMouseOut.AddListener((Action)(() => __instance.SelectNameplate(HatManager.Instance.GetNamePlateById(DataManager.Player.Customization.NamePlate))));
                    colorChip.Button.OnClick.AddListener((Action)(() => __instance.ClickEquip()));
                }
                else
                    colorChip.Button.OnClick.AddListener((Action)(() => __instance.SelectNameplate(nameplate)));

                colorChip.Button.ClickMask = __instance.scroller.Hitbox;
                colorChip.transform.localPosition = new(xpos, ypos, -1f);
                colorChip.Inner.transform.localPosition = nameplate.ChipOffset;
                colorChip.ProductId = nameplate.ProductId;
                colorChip.Tag = nameplate;
                __instance.UpdateMaterials(colorChip.Inner.FrontLayer, nameplate);
                nameplate.SetPreview(colorChip.Inner.FrontLayer, __instance.HasLocalPlayer() ? CustomPlayer.LocalCustom.Data.DefaultOutfit.ColorId : DataManager.Player.Customization.Color);
                colorChip.SelectionHighlight.gameObject.SetActive(false);
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
            var packages = new Dictionary<string, List<NamePlateData>>();

            foreach (var data in array)
            {
                var ext = data.GetNameplateExtension();
                var package = ext != null ? ext.Artist : "Innersloth";

                if (!packages.ContainsKey(package))
                    packages[package] = new();

                packages[package].Add(data);
            }

            var YOffset = __instance.YStart;
            Template = __instance.transform.FindChild("Text").gameObject.GetComponent<TMP_Text>();
            var keys = packages.Keys.OrderBy(x => x switch
            {
                "Innersloth" => 2,
                _ => 1
            });
            keys.ForEach(key => YOffset = CreateNameplatePackage(packages[key], key, YOffset, __instance));
            __instance.scroller.ContentYBounds.max = -(YOffset + 3.8f);

            foreach (var colorChip in __instance.ColorChips)
            {
                colorChip.Inner.FrontLayer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                colorChip.Inner.BackLayer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                colorChip.SelectionHighlight.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(CosmeticsCache), nameof(CosmeticsCache.GetNameplate))]
    public static class CosmeticsCacheGetPlatePatch
    {
        public static bool Prefix(CosmeticsCache __instance, string id, ref NamePlateViewData __result)
        {
            if (!CustomNameplateViewDatas.TryGetValue(id, out __result))
                return true;

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
}*/