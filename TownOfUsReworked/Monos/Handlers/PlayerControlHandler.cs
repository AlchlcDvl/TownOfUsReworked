namespace TownOfUsReworked.Monos;

public class PlayerControlHandler : NameHandler
{
    private TextMeshPro Name { get; set; }
    private TextMeshPro Color { get; set; }

    public void Awake()
    {
        Player = GetComponent<PlayerControl>();
        Custom = CustomPlayer.Custom(Player);
        Name = Player.NameText();
        Color = Player.ColorBlindText();
        Color.transform.localPosition = new(0f, -1.5f, -0.5f);
        Name.transform.localPosition = new(0f, -0.2f, -0.5f);
        Size = Player.transform.localScale;
    }

    public void Update()
    {
        if (!Player || !Player.Data || NoLobby())
            return;

        PlayerNames[Player.PlayerId] = Player.name;
        ColorNames[Player.PlayerId] = Player.Data.ColorName;

        if (Meeting())
            return;

        (Color.text, Color.color) = UpdateColorblind(Player);

        if (IsInGame() && Player.Data.Role is LayerHandler handler && CustomPlayer.Local.Data.Role is LayerHandler localHandler)
        {
            handler.UpdatePlayer();
            localHandler.UpdatePlayer(Player);
            (Name.text, Name.color) = UpdateGameName(handler, localHandler, out var revealed);
            Name.transform.localPosition = new(0f, revealed ? -0.05f : -0.2f, -0.5f);
            Player.transform.localScale = Size * Custom.Size;
        }
    }

    public void OnDestroy() => CustomPlayer.AllCustomPlayers.RemoveAll([HideFromIl2Cpp] (x) => x.Player == Player || !x.Player);
}