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
        if (!Player || !Player.Data || NoLobby())
            return;

        ColorNames[Player.PlayerId] = Player.Data.ColorName;

        if (Meeting())
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
}