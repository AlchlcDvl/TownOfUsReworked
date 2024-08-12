namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Phantom : Neutral
{
    public bool Caught { get; set; }
    public bool Faded { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Phantom : CustomColorManager.Neutral;
    public override string Name => "Phantom";
    public override LayerEnum Type => LayerEnum.Phantom;
    public override Func<string> StartText => () => "Peek-A-Boo!";
    public override Func<string> Description => () => "- You end the game upon finishing your objective";

    public override void Init()
    {
        BaseStart();
        Objectives = () => "- Finish your tasks without getting clicked";
        Alignment = Alignment.NeutralPros;
    }

    public void Fade()
    {
        if (Disconnected)
            return;

        Faded = true;
        var color = new UColor(1f, 1f, 1f, 0f);

        var maxDistance = Ship.MaxLightRadius * TownOfUsReworked.NormalOptions.CrewLightMod;
        var distance = (CustomPlayer.Local.GetTruePosition() - Player.GetTruePosition()).magnitude;

        var distPercent = distance / maxDistance;
        distPercent = Mathf.Max(0, distPercent - 1);

        var velocity = Player.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude;
        color.a = 0.07f + (velocity / Player.MyPhysics.TrueSpeed * 0.13f);
        color.a = Mathf.Lerp(color.a, 0, distPercent);

        if (Player.GetCustomOutfitType() != CustomPlayerOutfitType.PlayerNameOnly)
            Player.SetOutfit(CustomPlayerOutfitType.PlayerNameOnly, BlankOutfit(Player));

        Player.MyRend().color = color;
        Player.NameText().color = new(0f, 0f, 0f, 0f);
        Player.cosmetics.colorBlindText.color = new(0f, 0f, 0f, 0f);

        if (Local)
            Camouflage();
    }

    public void UnFade()
    {
        DefaultOutfit(Player);
        Player.MyRend().color = UColor.white;
        Player.gameObject.layer = LayerMask.NameToLayer("Ghost");
        Faded = false;
        Player.MyPhysics.ResetMoveState();

        if (Local)
            DefaultOutfitAll();
    }
}