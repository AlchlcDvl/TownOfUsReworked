namespace TownOfUsReworked.Custom;

public sealed class CustomGuessingMenu(PlayerControl owner, RoleSelect click) : CustomMenu(owner, MenuType.Guessing)
{
    private RoleSelect Click { get; } = click;
    private PlayerControl Selected { get; set; }
    public ShapeshifterPanel SelectedPanel { get; set; }

    public HashSet<Layer> Mapping { get; } = [];

    private void Clicked(ShapeshifterPanel selectedPanel, PlayerControl player, Layer role)
    {
        Click(selectedPanel, player, role);
        IsActive = false;

        if (!SelectedPanel)
            Menu.Close();
    }

    public void Open(PlayerControl selected)
    {
        Selected = selected;
        Open();
    }

    private void SetRole(ShapeshifterPanel panel, int index, Layer layer, Action onClick)
    {
        panel.shapeshift = onClick;
        // TODO: Replace the player icon with a layer icon because that looks nicer
        panel.PlayerIcon.SetFlipX(false);
        panel.PlayerIcon.ToggleName(false);
        panel.GetComponentsInChildren<SpriteRenderer>().Do(x => x.material.SetInt(PlayerMaterial.MaskLayer, index + 2));
        panel.PlayerIcon.SetMaskLayer(index + 2);
        panel.PlayerIcon.cosmetics.SetMaskType(PlayerMaterial.MaskType.ComplexUI);
        panel.PlayerIcon.Hands.Do(x => x.sharedMaterial = CosmeticsLayer.GetBodyMaterial(PlayerMaterial.MaskType.ComplexUI));
        panel.PlayerIcon.OtherBodySprites.Do(x => x.sharedMaterial = CosmeticsLayer.GetBodyMaterial(PlayerMaterial.MaskType.ComplexUI));
        var dictEntry = LayerDictionary[layer];
        PlayerMaterial.SetColors(dictEntry.Color, panel.PlayerIcon.cosmetics.currentBodySprite.BodySprite);
        panel.LevelNumberText.transform.parent.gameObject.SetActive(false);
        panel.NameText.text = layer == Layer.Miner && MapPatches.CurrentMap == 5 ? TranslationManager.Translate("Layer.Herbalist") : dictEntry.Name;
        panel.NameText.color = dictEntry.Color;
        panel.name = $"Guess{panel.NameText.text.Replace(" ", "")}";

        if (panel == SelectedPanel)
            panel.Background.color = dictEntry.Color.Alternate(0.4f);
    }

    public override ISystem.List<UiElement> CreateMenu(ShapeshifterMinigame __instance)
    {
        __instance.potentialVictims = new();
        var list2 = new ISystem.List<UiElement>();

        foreach (var (i, role) in Mapping.Indexed())
        {
            var shapeshifterPanel = UObject.Instantiate(__instance.PanelPrefab, __instance.transform);
            SetRole(shapeshifterPanel, i, role, () => Clicked(shapeshifterPanel, Selected, role));
            Menu.potentialVictims.Add(shapeshifterPanel);
            list2.Add(shapeshifterPanel.Button);
        }

        return list2;
    }

    public void Close()
    {
        Selected = null;
        SelectedPanel = null;
        Mapping.Clear();

        if (Menu)
            Menu.Close();

        IsActive = false;
    }
}