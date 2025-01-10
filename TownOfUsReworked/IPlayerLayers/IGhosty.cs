namespace TownOfUsReworked.IPlayerLayers;

public interface IGhosty : IPlayerLayer
{
    bool Caught { get; set; }
    bool Faded { get; set; }

    public void Fade()
    {
        if (Disconnected)
            return;

        Faded = true;

        var maxDistance = Ship().MaxLightRadius * TownOfUsReworked.NormalOptions.CrewLightMod;
        var distance = (CustomPlayer.Local.GetTruePosition() - Player.GetTruePosition()).magnitude;

        var distPercent = distance / maxDistance;
        distPercent = Mathf.Max(0, distPercent - 1);

        var velocity = Player.GetComponent<Rigidbody2D>().velocity.magnitude;

        if (Player.GetCustomOutfitType() != CustomPlayerOutfitType.PlayerNameOnly)
            Player.SetOutfit(CustomPlayerOutfitType.PlayerNameOnly, BlankOutfit(Player));

        Player.cosmetics.SetPhantomRoleAlpha(Mathf.Lerp(0.07f + (velocity / Player.MyPhysics.TrueSpeed * 0.13f), 0, distPercent));
        Player.NameText().color = Player.cosmetics.colorBlindText.color = UColor.clear;

        if (Local)
            Camouflage();
    }

    public void UnFade()
    {
        Player.MyRend().color = UColor.white;
        Player.gameObject.layer = LayerMask.NameToLayer("Ghost");
        Faded = false;
        Player.MyPhysics.ResetMoveState();
        Player.MyPhysics.ResetAnimState();

        if (Local)
            DefaultOutfitAll();
    }

    public void UpdateGhost()
    {
        if (!Caught)
            Fade();
        else if (Faded)
            UnFade();
    }
}