namespace TownOfUsReworked.Monos;

public sealed class PlayerControlHandler : NameHandler
{
    private TextMeshPro Name { get; set; }

    public void Awake()
    {
        Player = GetComponent<PlayerControl>();
        Name = Player.NameText();
        gameObject.AddComponent<AppearanceHandler>();
        gameObject.AddComponent<StatusHandler>();
    }

    public void Update()
    {
        if (!Player || !Player.Data || NoLobby() || Meeting())
            return;

        if (!IsInGame() || Player.Data.Role is not LayerHandler handler || LocalPlayer.Data.Role is not LayerHandler localHandler)
        {
            (Name.text, Name.color) = (Player.name, UColor.white);
            return;
        }

        handler.UpdatePlayer();
        localHandler.UpdatePlayer(Player);
        (Name.text, Name.color) = UpdateGameName(handler, localHandler, out var revealed);
        Name.transform.localPosition = new(0f, revealed ? -0.05f : -0.2f, -0.5f);
    }

    private bool UpdateColorblindVisibility()
    {
        if (!DataManager.Settings.Accessibility.ColorBlindMode || GameModifiers.PlayerNames == PlayerNames.NotVisible)
            return false;

        var local = LocalPlayer;
        var vector = Player.transform.position - local.transform.position;

        if (vector.magnitude > local.lightSource.viewDistance)
            return false;

        if (PhysicsHelpers.AnyNonTriggersBetween(local.transform.position, vector.normalized, vector.magnitude, Constants.ShipAndObjectsMask) && !Player.AmOwner && !local.HasDied() &&
            GameModifiers.PlayerNames == PlayerNames.Obstructed)
        {
            return false;
        }

        return true;
    }
}