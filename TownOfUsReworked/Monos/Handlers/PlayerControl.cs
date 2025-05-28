namespace TownOfUsReworked.Monos;

public sealed class PlayerControlHandler : NameHandler
{
    private TextMeshPro Name { get; set; }
    // private TextMeshPro Color { get; set; }

    public void Awake()
    {
        Player = GetComponent<PlayerControl>();
        // Custom = CustomPlayer.Custom(Player);
        Name = Player.NameText();
        // Color = Player.ColorBlindText();
        // Size = Player.transform.localScale;
        gameObject.AddComponent<AppearanceHandler>();
        gameObject.AddComponent<StatusHandler>();
    }

    public void Update()
    {
        if (!Player || !Player.Data || NoLobby())
            return;

        // PlayerNames[Player.PlayerId] = Player.name;
        ColorNames[Player.PlayerId] = Player.Data.ColorName;

        if (Meeting())
            return;

        // (Color.text, Color.color) = UpdateColorblind(Player);

        if (!IsInGame() || Player.Data.Role is not LayerHandler handler || CustomPlayer.Local.Data.Role is not LayerHandler localHandler)
        {
            (Name.text, Name.color) = (Player.name, UColor.white);
            return;
        }

        handler.UpdatePlayer();
        localHandler.UpdatePlayer(Player);
        (Name.text, Name.color) = UpdateGameName(handler, localHandler, out var revealed);
        Name.transform.localPosition = new(0f, revealed ? -0.05f : -0.2f, -0.5f);
    }

    public void OnDestroy() => CustomPlayer.AllCustomPlayers.RemoveAll([HideFromIl2Cpp] (x) => x.Player == Player || !x.Player);
}