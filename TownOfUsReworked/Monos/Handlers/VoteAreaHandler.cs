namespace TownOfUsReworked.Monos;

public class VoteAreaHandler : NameHandler
{
    public static Vector3? NamePos;

    private PlayerVoteArea VoteArea { get; set; }

    public void Awake()
    {
        VoteArea = gameObject.GetComponent<PlayerVoteArea>();
        Player = PlayerByVoteArea(VoteArea);
        NamePos ??= VoteArea.ColorBlindName.transform.localPosition;
    }

    public void Update()
    {
        if (!Player)
            return;

        (VoteArea.ColorBlindName.text, VoteArea.ColorBlindName.color) = UpdateColorblind(Player);

        if (Player.Data?.Role is LayerHandler handler && CustomPlayer.Local.Data?.Role is LayerHandler localHandler)
        {
            handler.UpdateVoteArea();
            localHandler.UpdateVoteArea(VoteArea);

            (VoteArea.NameText.text, VoteArea.NameText.color) = UpdateGameName(handler, localHandler, out var revealed);
            VoteArea.ColorBlindName.transform.localPosition = revealed ? new(-0.93f, 0f, -0.1f) : (NamePos ?? default);
            VoteArea.ColorBlindName.color = VoteArea.NameText.color;
        }
    }
}