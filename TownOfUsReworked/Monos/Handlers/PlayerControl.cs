namespace TownOfUsReworked.Monos;

public sealed class PlayerControlHandler : NameHandler
{
    private TextMeshPro Name { get; set; }
    private TextMeshPro Color { get; set; }

    [HideFromIl2Cpp]
    private AppearanceHandler Appearance { get; set; }

    public void Awake()
    {
        Player = GetComponent<PlayerControl>();
        Name = Player.NameText();
        Color = Player.ColorBlindText();
        Appearance = gameObject.AddComponent<AppearanceHandler>();
        gameObject.AddComponent<StatusHandler>();
    }

    public void Update()
    {
        if (!Player || !Player.Data || NoLobby() || Meeting())
            return;

        Color.enabled = UpdateNameVisibility(true);

        if (!IsInGame() || Player.Data.Role is not LayerHandler handler || LocalPlayer.Data.Role is not LayerHandler localHandler)
        {
            Name.color = UColor.white.SetAlpha(Appearance.Alpha);
            return;
        }

        var deadSeeEverything = DeadSeeEverything();
        var amOwner = Player.AmOwner;

        if (!amOwner && !deadSeeEverything && Appearance.Mimicked?.Data?.Role is LayerHandler handler1)
            handler = handler1;

        handler.UpdatePlayer();
        localHandler.UpdatePlayer(Player);
        var (name, color) = UpdateGameName(handler, localHandler, amOwner, deadSeeEverything, out var revealed);
        (Name.text, Name.color, Name.enabled) = (Player.name + name, color.SetAlpha(Appearance.Alpha), UpdateNameVisibility(true));
        Name.transform.localPosition = new(0f, revealed ? -0.05f : -0.2f, -0.5f);
    }

    private bool UpdateNameVisibility(bool colorBlind)
    {
        if (colorBlind)
        {
            if (IsLobby())
                return DataManager.Settings.Accessibility.ColorBlindMode;

            if (!DataManager.Settings.Accessibility.ColorBlindMode)
                return false;
        }

        if (GameModifiers.PlayerNames == PlayerNames.NotVisible)
            return false;

        try
        {
            var local = LocalPlayer;
            var vector = Player.transform.position - local.transform.position;

            if (vector.magnitude > local.lightSource.viewDistance)
                return false;

            if (PhysicsHelpers.AnyNonTriggersBetween(local.transform.position, vector.normalized, vector.magnitude, Constants.ShipAndObjectsMask) && !Player.AmOwner && !local.HasDied() &&
                GameModifiers.PlayerNames == PlayerNames.Obstructed)
            {
                return false;
            }
        } catch {}

        return true;
    }
}