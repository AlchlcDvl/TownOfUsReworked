namespace TownOfUsReworked.Monos;

public sealed class DeadBodyHandler : NameHandler
{
    public static readonly List<byte> Dragging = [];

    public DeadBody Body { get; set; }

    private int ColorId { get; set; }
    private SpriteRenderer Rend { get; set; }
    private PlayerControl Dragger { get; set; }

    public void Awake()
    {
        Body = GetComponent<DeadBody>();
        Player = PlayerByBody(Body);
        Custom = CustomPlayer.Custom(Player);
        Rend = Body.bodyRenderers[0];
        Size = Body.transform.localScale;
        ColorId = Player.Data.DefaultOutfit.ColorId;
    }

    public void Update()
    {
        if (Hud.Instance.IsCamoed)
            CustomColorManager.SetColor(Rend, 39);
        else if (SurveillancePatches.NvActive)
            CustomColorManager.SetColor(Rend, 6);
        else
            CustomColorManager.SetColor(Rend, ColorId);

        // Body.transform.localScale = Size * Custom.Size;

        if (!Dragger)
            return;

        Body.transform.position = Vector3.Lerp(Body.transform.position, Dragger.GetTruePosition(), 5f * Time.deltaTime);
        Rend.flipX = Dragger.MyRend().flipX;
    }

    public void StartDrag(PlayerControl player)
    {
        Dragger = player;
        Dragging.Add(player.PlayerId);

        if (player.Is<Janitor>(out var jani))
            jani.CurrentlyDragging = this;
    }

    public void StopDrag()
    {
        if (Dragger.Is<Janitor>(out var jani))
            jani.CurrentlyDragging = null;

        Dragging.Remove(Dragger.PlayerId);
        Dragger = null;
    }
}