namespace TownOfUsReworked.Monos;

public class PlayerControlHandler : NameHandler
{
    [HideFromIl2Cpp]
    private CustomPlayer Custom { get; set; }

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
    }

    public void Update()
    {
        if (!Player || !Player.Data)
            return;

        PlayerNames[Player.PlayerId] = Player.Data.PlayerName;
        ColorNames[Player.PlayerId] = Player.Data.ColorName.Replace("(", "").Replace(")", "");

        if (Meeting())
            return;

        (Color.text, Color.color) = UpdateColorblind(Player);

        if (IsInGame() && Player.Data.Role is LayerHandler handler && CustomPlayer.Local.Data.Role is LayerHandler localHandler)
        {
            handler.UpdatePlayer();
            localHandler.UpdatePlayer(Player);
            (Name.text, Name.color) = UpdateGameName(handler, localHandler, out var revealed);
            Name.transform.localPosition = new(0f, revealed ? -0.05f : -0.2f, -0.5f);
            Player.transform.localScale = Custom.SizeFactor;
        }
    }
}