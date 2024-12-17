namespace TownOfUsReworked.Monos;

public class DeadBodyHandler : NameHandler
{
    private DeadBody Body { get; set; }
    private Vector3 Size { get; set; }
    private int ColorId { get; set; }
    private SpriteRenderer Rend { get; set; }

    public void Awake()
    {
        Body = GetComponent<DeadBody>();
        Player = PlayerByBody(Body);
        Custom = CustomPlayer.Custom(Player);
        Rend = Body.bodyRenderers[0];
        Size = Custom.SizeFactor;
        ColorId = Player.Data.DefaultOutfit.ColorId;
    }

    public void Update()
    {
        if (HudHandler.Instance.IsCamoed)
            CustomColorManager.SetColor(Rend, 39);
        else if (SurveillancePatches.NVActive)
            CustomColorManager.SetColor(Rend, 6);
        else
            CustomColorManager.SetColor(Rend, ColorId);

        Body.transform.localScale = Size;
    }
}