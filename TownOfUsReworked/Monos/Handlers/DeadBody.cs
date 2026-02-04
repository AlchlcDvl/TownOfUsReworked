namespace TownOfUsReworked.Monos;

public sealed class DeadBodyHandler : NameHandler
{
    public static readonly HashSet<byte> Dragging = [];

    public DeadBody Body { get; private set; }

    private SpriteRenderer Rend;
    private PlayerControl Dragger;

    public void Awake()
    {
        Body = GetComponent<DeadBody>();
        Player = PlayerByBody(Body);
        Rend = Body.bodyRenderers[0];
    }

    public void Update()
    {
        if (!Dragger)
            return;

        transform.position = Vector3.Lerp(transform.position, Dragger.GetTruePosition(), 5f * Time.deltaTime);
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