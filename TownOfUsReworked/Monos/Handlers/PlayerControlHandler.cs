namespace TownOfUsReworked.Modules;

public class PlayerControlHandler : NameHandler
{
    public static Vector3? NamePos;

    public void Awake()
    {
        Player = gameObject.GetComponent<PlayerControl>();
        // Custom = CustomPlayer.Custom(Player);
        NamePos ??= Player?.NameText()?.transform?.localPosition;
    }

    public void Update()
    {
        if (!Player)
            return;

        PlayerNames[Player.PlayerId] = Player.Data?.PlayerName;
        ColorNames[Player.PlayerId] = Player.Data?.ColorName?.Replace("(", "")?.Replace(")", "");

        if (Player.Data?.Role is LayerHandler handler && CustomPlayer.Local.Data?.Role is LayerHandler localHandler)
        {
            handler.UpdatePlayer();
            localHandler.UpdatePlayer(Player);

            if (IsInGame())
            {
                (Player.NameText().text, Player.NameText().color) = UpdateGameName(handler, localHandler, out var revealed);
                Player.NameText().transform.localPosition = revealed ? new(0f, 0.15f, -0.5f) : (NamePos ?? default);
                (Player.ColorBlindText().text, Player.ColorBlindText().color) = UpdateColorblind(Player);
                Player.transform.localScale = CustomPlayer.Custom(Player).SizeFactor;
            }
        }
    }
}