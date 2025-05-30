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

        (VoteArea.ColorBlindName.color, VoteArea.ColorBlindName.enabled) = (Player.Data.DefaultOutfit.ColorId.GetColor(false), DataManager.Settings.Accessibility.ColorBlindMode);

        if (Player.Data.Role is not LayerHandler handler || LocalPlayer.Data.Role is not LayerHandler localHandler)
            return;

        handler.UpdateVoteArea();
        localHandler.UpdateVoteArea(VoteArea);
        var deadSeeEverything = DeadSeeEverything();
        var amOwner = Player.AmOwner;
        var (name, color) = UpdateGameName(handler, localHandler, amOwner, deadSeeEverything, out var revealed);
        (VoteArea.NameText.text, VoteArea.NameText.color) = (Player.Data.PlayerName + name, color);
        VoteArea.NameText.fontSize = revealed ? 1.5f : 2f;
        VoteArea.NameText.transform.localPosition = new(0.3384f, revealed ? 0.0611f : 0.0311f, -0.1f);
    }
}