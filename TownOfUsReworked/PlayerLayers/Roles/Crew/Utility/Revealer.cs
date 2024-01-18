namespace TownOfUsReworked.PlayerLayers.Roles;

public class Revealer : Crew
{
    public bool Caught { get; set; }
    public bool Revealed { get; set; }
    public bool Faded { get; set; }
    public Role FormerRole { get; set; }

    public override UColor Color => ClientGameOptions.CustomCrewColors ? CustomColorManager.Revealer : CustomColorManager.Crew;
    public override string Name => "Revealer";
    public override LayerEnum Type => LayerEnum.Revealer;
    public override Func<string> StartText => () => "OOOOOOO";
    public override Func<string> Description => () => "- You can reveal evils players to the <color=#8CFFFFFF>Crew</color> once you finish your tasks without getting clicked.";

    public Revealer(PlayerControl player) : base(player) => Alignment = Alignment.CrewUtil;

    public void Fade()
    {
        if (Disconnected)
            return;

        Faded = true;
        Player.Visible = true;
        var color = new Color(1f, 1f, 1f, 0f);

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
        Player.MyRend().color = UColor.white;
        Player.gameObject.layer = LayerMask.NameToLayer("Ghost");
        Faded = false;
        Player.MyPhysics.ResetMoveState();

        if (Local)
            DefaultOutfitAll();
    }
}