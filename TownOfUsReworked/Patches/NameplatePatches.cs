using Innersloth.Assets;
using static TownOfUsReworked.Managers.CustomNameplateManager;

namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(NameplatesTab), nameof(NameplatesTab.OnEnable))]
public static class NameplatesTabOnEnablePatch
{
    private static TMP_Text Template;

    private static void CreateNameplatePackage(List<NamePlateData> nameplates, string packageName, ref float yStart, NameplatesTab __instance)
    {
        var isDefaultPackage = "Innersloth" == packageName;

        if (!isDefaultPackage)
            nameplates = [ .. nameplates.OrderBy(x => x.name) ];

        var offset = yStart;

        if (Template)
        {
            var title = UObject.Instantiate(Template, __instance.scroller.Inner);
            var material = title.GetComponent<MeshRenderer>().material;
            material.SetFloat("_StencilComp", 4f);
            material.SetFloat("_Stencil", 1f);
            title.transform.localPosition = new(2.25f, offset, -1f);
            title.transform.localScale = Vector3.one * 1.5f;
            title.fontSize *= 0.5f;
            title.enableAutoSizing = false;
            title.GetComponent<TextTranslatorTMP>().Destroy();
            title.SetText(packageName);
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
                colorChip.Button.OverrideOnMouseOverListeners(() => __instance.SelectNameplate(nameplate));
                colorChip.Button.OverrideOnMouseOutListeners(() => __instance.SelectNameplate(HatManager.Instance.GetNamePlateById(DataManager.Player.Customization.NamePlate)));
                colorChip.Button.OverrideOnClickListeners(__instance.ClickEquip);
            }
            else
                colorChip.Button.OverrideOnClickListeners(() => __instance.SelectNameplate(nameplate));

            colorChip.Button.ClickMask = __instance.scroller.Hitbox;
            colorChip.transform.localPosition = new(xpos, ypos, -1f);
            colorChip.ProductId = nameplate.ProductId;
            colorChip.Tag = nameplate;
            colorChip.SelectionHighlight.gameObject.SetActive(false);

            if (CustomNameplateViewDatas.TryGetValue(colorChip.ProductId, out var viewData))
                colorChip.GetComponent<NameplateChip>().image.sprite = viewData.Image;
            else
                DefaultNameplateCoro(__instance, colorChip.GetComponent<NameplateChip>());

            __instance.ColorChips.Add(colorChip);
            yStart = ypos;
        }

        yStart -= 1.5f;
    }

    private static void DefaultNameplateCoro(NameplatesTab __instance, NameplateChip chip) => __instance.StartCoroutine(__instance.CoLoadAssetAsync<NamePlateViewData>(HatManager.Instance
        .GetNamePlateById(chip.ProductId).ViewDataRef, (Action<NamePlateViewData>)(viewData => chip.image.sprite = viewData?.Image)));

    public static bool Prefix(NameplatesTab __instance)
    {
        __instance.PlayerPreview.gameObject.SetActive(true);

        if (__instance.HasLocalPlayer())
            __instance.PlayerPreview.UpdateFromLocalPlayer(PlayerMaterial.MaskType.None);
        else
            __instance.PlayerPreview.UpdateFromDataManager(PlayerMaterial.MaskType.None);

        __instance.scroller.Inner.DestroyChildren();
        __instance.ColorChips = new();
        var array = HatManager.Instance.GetUnlockedNamePlates();
        var packages = new Dictionary<string, List<NamePlateData>>();

        foreach (var data in array)
        {
            var ext = data.GetExtention();
            var package = "Innersloth";

            if (ext != null)
                package = ext.StreamOnly ? "Stream" : ext.Artist;

            if (IsNullEmptyOrWhiteSpace(package))
                package = "Misc";

            if (!packages.TryGetValue(package, out var value))
                packages[package] = value = [];

            value.Add(data);
        }

        Template = __instance.transform.FindChild("Text").GetComponent<TMP_Text>();
        var keys = packages.Keys.OrderBy(x => x switch
        {
            "Innersloth" => 4,
            "Stream" => 1,
            "Misc" => 3,
            _ => 2
        });
        var yOffset = __instance.YStart;
        keys.ForEach(key => CreateNameplatePackage(packages[key], key, ref yOffset, __instance));
        __instance.plateId = DataManager.Player.Customization.NamePlate;
        __instance.currentNameplateIsEquipped = true;
        __instance.scroller.ContentYBounds.max = -(yOffset + 3.8f);
        __instance.scroller.UpdateScrollBars();

        if (array.Length != 0)
            __instance.GetDefaultSelectable().PlayerEquippedForeground.SetActive(true);

        return false;
    }
}