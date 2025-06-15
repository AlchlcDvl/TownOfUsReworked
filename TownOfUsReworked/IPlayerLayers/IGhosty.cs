namespace TownOfUsReworked.IPlayerLayers;

public interface IGhosty : IPositioner
{
    bool Caught { get; set; }

    public bool CanBeClicked(PlayerControl clicker);

    public void OnStart()
    {
        Player.OverrideOutfit(BlankOutfit(Player), CustomPlayerOutfitType.Ghostly, -1, CheckFaded, Fade);
        Player.MyPhysics.ResetMoveState();
        Player.MyPhysics.ResetAnimState();
    }

    private void Fade()
    {
        if (Disconnected)
            return;

        var maxDistance = LocalPlayer.lightSource.viewDistance;
        var distance = (LocalPlayer.GetTruePosition() - Player.GetTruePosition()).magnitude;

        var distPercent = distance / maxDistance;
        distPercent = Mathf.Max(0, distPercent - 1);

        var velocity = Player.GetComponent<Rigidbody2D>().velocity.magnitude;

        Player.MyRend().SetAlpha(Mathf.Lerp(0.07f + (velocity / Player.MyPhysics.TrueSpeed * 0.13f), 0, distPercent));
    }

    private bool CheckFaded() => Caught;

    public void Catch(PlayerControl clicker)
    {
        if (clicker.AmOwner)
            CallRpc(ReworkedRpc.Misc, MiscRpc.Catch, Player, clicker);

        Player.CustomDie(DeathReasonEnum.Caught, clicker);
        Player.gameObject.layer = LayerMask.NameToLayer("Ghost");
        Player.MyPhysics.ResetMoveState();
        Player.MyPhysics.ResetAnimState();
        Caught = true;
    }
}