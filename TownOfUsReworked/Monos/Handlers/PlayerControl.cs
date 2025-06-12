namespace TownOfUsReworked.Monos;

public sealed class PlayerControlHandler : NameHandler
{
    private TextMeshPro Name { get; set; }
    private TextMeshPro Color { get; set; }

    [HideFromIl2Cpp]
    private AppearanceHandler Appearance { get; set; }

    // [HideFromIl2Cpp]
    // private StatusHandler Status { get; set; }

    public void Awake()
    {
        Player = GetComponent<PlayerControl>();
        Name = Player.NameText();
        Color = Player.ColorBlindText();
        Appearance = gameObject.AddComponent<AppearanceHandler>();
        // Status = gameObject.AddComponent<StatusHandler>();
    }

    public void Update()
    {
        if (!Player || !Player.Data || NoLobby() || Meeting())
            return;

        var visible = UpdateNameVisibility();
        Color.enabled = DataManager.Settings.Accessibility.ColorBlindMode && visible;

        if (!IsInGame() || !LayerHandler.Handlers.TryGetValue(Player.PlayerId, out var handler) || !LayerHandler.Handlers.TryGetValue(LocalPlayer.PlayerId, out var localHandler))
        {
            Name.color = UColor.white.SetAlpha(Appearance.Alpha);
            return;
        }

        var deadSeeEverything = DeadSeeEverything();
        var amOwner = Player.AmOwner;

        if (!amOwner && !deadSeeEverything && Appearance.Mimicked && LayerHandler.Handlers.TryGetValue(Appearance.Mimicked.PlayerId, out var handler1))
            handler = handler1;

        handler.UpdatePlayer();
        localHandler.UpdatePlayer(Player);
        var (extraStuff, color) = UpdateGameName(handler, localHandler, amOwner, deadSeeEverything, out var revealed);
        (Name.text, Name.color, Name.enabled) = (handler.Player.name + extraStuff, color.SetAlpha(Appearance.Alpha), visible);
        Name.transform.localPosition = new(0f, revealed ? -0.05f : -0.2f, -0.5f);
    }

    public void UpdateCurrent()
    {
        Appearance.UpdateCurrent();
        // Status.UpdateCurrent();
    }

    private bool UpdateNameVisibility()
    {
        if (IsLobby())
            return true;

        if (GameModifiers.PlayerNames == PlayerNames.NotVisible)
            return false;

        if (GameModifiers.PlayerNames != PlayerNames.Obstructed)
            return true;

        try
        {
            var local = LocalPlayer;
            var vector = Player.transform.position - local.transform.position;

            if (vector.magnitude > local.lightSource.viewDistance)
                return false;

            if (PhysicsHelpers.AnyNonTriggersBetween(local.transform.position, vector.normalized, vector.magnitude, Constants.ShipAndObjectsMask) && !Player.AmOwner && !local.HasDied())
                return false;
        } catch {}

        return true;
    }
}