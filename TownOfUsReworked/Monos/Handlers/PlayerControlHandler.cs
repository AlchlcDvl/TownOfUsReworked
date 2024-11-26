namespace TownOfUsReworked.Monos;

public class PlayerControlHandler : NameHandler
{
    [HideFromIl2Cpp]
    public CustomPlayer Custom { get; set; }

    public void Awake()
    {
        Player = gameObject.GetComponent<PlayerControl>();
        Custom = CustomPlayer.Custom(Player);
        Player.ColorBlindText().transform.localPosition = new(0f, -1.5f, 0f);
    }

    public void Update()
    {
        if (!Player)
            return;

        PlayerNames[Player.PlayerId] = Player.Data?.PlayerName;
        ColorNames[Player.PlayerId] = Player.Data?.ColorName?.Replace("(", "")?.Replace(")", "");

        if (Meeting())
            return;

        var cb = Player.ColorBlindText();
        (cb.text, cb.color) = UpdateColorblind(Player);
        var revealed = false;
        var name = Player.NameText();

        if (IsInGame() && Player.Data?.Role is LayerHandler handler && CustomPlayer.Local.Data?.Role is LayerHandler localHandler)
        {
            handler.UpdatePlayer();
            localHandler.UpdatePlayer(Player);
            (name.text, name.color) = UpdateGameName(handler, localHandler, out revealed);
            Player.transform.localScale = Custom.SizeFactor;
        }

        name.transform.localPosition = new(0f, revealed ? -0.05f : -0.2f, -0.5f);
    }
}