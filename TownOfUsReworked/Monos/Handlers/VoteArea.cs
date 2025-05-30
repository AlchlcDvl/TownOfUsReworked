namespace TownOfUsReworked.Monos;

public sealed class VoteAreaHandler : NameHandler
{
    private PlayerVoteArea VoteArea { get; set; }

    public void Awake()
    {
        VoteArea = GetComponent<PlayerVoteArea>();
        Player = PlayerByVoteArea(VoteArea);
    }

    public void Update()
    {
        if (!Player || !Player.Data)
            return;

        (VoteArea.ColorBlindName.text, VoteArea.ColorBlindName.color) = UpdateColorblind();

        if (Player.Data.Role is not LayerHandler handler || LocalPlayer.Data.Role is not LayerHandler localHandler)
            return;

        handler.UpdateVoteArea();
        localHandler.UpdateVoteArea(VoteArea);
        (VoteArea.NameText.text, VoteArea.NameText.color) = UpdateGameName(handler, localHandler, out var revealed);
        VoteArea.NameText.fontSize = revealed ? 1.5f : 2f;
        VoteArea.NameText.transform.localPosition = new(0.3384f, revealed ? 0.0611f : 0.0311f, -0.1f);
    }
}