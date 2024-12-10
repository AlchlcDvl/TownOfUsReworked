namespace TownOfUsReworked.Monos;

public class DeadBodyHandler : NameHandler
{
    private DeadBody Body { get; set; }
    private Vector3 Size { get; set; }
    private int ColorId { get; set; }

    [HideFromIl2Cpp]
    private SpriteRenderer[] Rends { get; set; }

    public void Awake()
    {
        Body = GetComponent<DeadBody>();
        Player = PlayerByBody(Body);
        Custom = CustomPlayer.Custom(Player);
        Rends = Body.bodyRenderers;
        Size = Custom.SizeFactor;
        ColorId = Player.Data.DefaultOutfit.ColorId;
    }

    public void Update()
    {
        foreach (var rend in Rends)
        {
            if (HudHandler.Instance.IsCamoed)
                PlayerMaterial.SetColors(39, rend);
            else if (SurveillancePatches.NVActive)
                PlayerMaterial.SetColors(6, rend);
            else
                PlayerMaterial.SetColors(ColorId, rend);
        }

        Body.transform.localScale = Size;
    }
}