namespace TownOfUsReworked.Monos;

public class DragHandler : MonoBehaviour
{
    public static DragHandler Instance { get; private set; }
    public readonly Dictionary<byte, byte> Dragging = [];

    public DragHandler(IntPtr ptr) : base(ptr) => Instance = this;

    public void StartDrag(PlayerControl player, DeadBody body) => Dragging[player.PlayerId] = body.ParentId;

    public void StopDrag(PlayerControl player)
    {
        var body = BodyById(Dragging[player.PlayerId]);
        var position = player.transform.position;

        if (IsSubmerged())
            position.z = position.y > -7f ? 0.0208f : -0.0273f;

        position.y -= 0.4f;
        body.transform.position = position;
        Dragging.Remove(player.PlayerId);
    }

    public void StopDrag(byte id) => StopDrag(PlayerById(id));

    public void Update()
    {
        if (!IsInGame || IsHnS || (!PlayerLayer.GetLayers<Janitor>().Any() && !PlayerLayer.GetLayers<PromotedGodfather>().Any(x => x.IsJani)))
            return;

        var toRemove = new List<byte>();

        foreach (var (playerid, bodyid) in Dragging)
        {
            var player = PlayerById(playerid);
            var body = BodyById(bodyid);

            if (!body || !player || player.Data.IsDead)
                toRemove.Add(playerid);
            else
            {
                body.transform.position = Vector3.Lerp(body.transform.position, player.GetTruePosition(), 5f * Time.deltaTime);
                body.bodyRenderers.ForEach(x => x.flipX = !player.MyRend().flipX);
            }
        }

        if (toRemove.Any())
            toRemove.ForEach(StopDrag);
    }
}