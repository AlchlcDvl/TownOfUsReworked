namespace TownOfUsReworked.Custom;

public class CustomRolesMenu(PlayerControl owner, CustomRolesMenu.Select click) : CustomMenu(owner, "Guessing")
{
    public Select Click { get; } = click;
    public List<LayerEnum> Mapping { get; set; } = [];
    public PlayerControl Selected { get; set; }
    public ShapeshifterPanel SelectedPanel { get; set; }

    public delegate void Select(ShapeshifterPanel selectedPanel, PlayerControl player, LayerEnum role);

    public void Clicked(ShapeshifterPanel selectedPanel, PlayerControl player, LayerEnum role)
    {
        Click(selectedPanel, player, role);

        if (!SelectedPanel)
            Menu.Close();
    }

    public void Open(PlayerControl selected, List<LayerEnum> mapping)
    {
        Selected = selected;
        Mapping = mapping;
        Open();
    }

    private void SetRole(ShapeshifterPanel panel, int index, LayerEnum layer, Action onClick)
    {
        panel.shapeshift = onClick;
        panel.PlayerIcon.SetFlipX(false);
        panel.PlayerIcon.ToggleName(false);
        panel.GetComponentsInChildren<SpriteRenderer>().ForEach(x => x.material.SetInt(PlayerMaterial.MaskLayer, index + 2));
        panel.PlayerIcon.SetMaskLayer(index + 2);
        panel.PlayerIcon.cosmetics.SetMaskType(PlayerMaterial.MaskType.ComplexUI);
        panel.PlayerIcon.Hands.ForEach(x => x.sharedMaterial = CosmeticsLayer.GetBodyMaterial(PlayerMaterial.MaskType.ComplexUI));
        panel.PlayerIcon.OtherBodySprites.ForEach(x => x.sharedMaterial = CosmeticsLayer.GetBodyMaterial(PlayerMaterial.MaskType.ComplexUI));
        var dictEntry = LayerDictionary[layer];
        PlayerMaterial.SetColors(dictEntry.Color, panel.PlayerIcon.cosmetics.currentBodySprite.BodySprite);
        panel.LevelNumberText.transform.parent.gameObject.SetActive(false);
        panel.NameText.text = dictEntry.Name;
        panel.NameText.color = dictEntry.Color;
        panel.name = $"Guess{layer}";
        // panel.transform.GetChild()

        if (panel == SelectedPanel)
            panel.Background.color = dictEntry.Color.Alternate(0.4f);
    }

    public override ISystem.List<UiElement> CreateMenu(ShapeshifterMinigame __instance)
    {
        __instance.potentialVictims = new();
        var list2 = new ISystem.List<UiElement>();

        for (var i = 0; i < Mapping.Count; i++)
        {
            var role = Mapping[i];
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
    }
}