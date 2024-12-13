namespace TownOfUsReworked.Monos;

public class VoteAreaHandler : NameHandler
{
    private PlayerVoteArea VoteArea { get; set; }

    public void Awake()
    {
        VoteArea = GetComponent<PlayerVoteArea>();

        if (!VoteArea)
            return;

        Player = PlayerByVoteArea(VoteArea);
    }

    public void Update()
    {
        if (!Player || !Player.Data)
            return;

        (VoteArea.ColorBlindName.text, VoteArea.ColorBlindName.color) = UpdateColorblind(Player);

        if (Player.Data.Role is LayerHandler handler && CustomPlayer.Local.Data.Role is LayerHandler localHandler)
        {
            handler.UpdateVoteArea();
            localHandler.UpdateVoteArea(VoteArea);
            (VoteArea.NameText.text, VoteArea.NameText.color) = UpdateGameName(handler, localHandler, out var revealed);
            VoteArea.NameText.fontSize = revealed ? 1.5f : 2f;
            VoteArea.NameText.transform.localPosition = new(0.3384f, revealed ? 0.0611f : 0.0311f, -0.1f);
        }
    }
}