namespace TownOfUsReworked.Monos;

// FIXME: The drag doesn't appear to happen
public sealed class DragHandler : MonoBehaviour
{
    public static readonly Dictionary<byte, DeadBody> Dragging = [];

    public static void StartDrag(PlayerControl player, DeadBody body) => Dragging[player.PlayerId] = body;

    public static void StopDrag(PlayerControl player)
    {
        var position = player.transform.position;

        if (IsSubmerged())
            position.z = position.y > -7f ? 0.0208f : -0.0273f;

        position.y -= 0.4f;
        Dragging[player.PlayerId].transform.position = position;
        Dragging.Remove(player.PlayerId);

        if (player.Is<IDragger>(out var dragger))
            dragger.CurrentlyDragging = null;
    }

    private static void StopDrag(byte id) => StopDrag(PlayerById(id));

    #pragma warning disable CA1822 // Method can be marked static
    public void Update()
    #pragma warning restore CA1822 // Method can be marked static
    {
        if (!IsInGame() || IsHnS() || (!PlayerLayer.GetLayers<Janitor>().Any() && !PlayerLayer.GetLayers<PromotedGodfather>().Any(x => x.IsJani)))
            return;

        var toRemove = new List<byte>();

        foreach (var (playerid, body) in Dragging)
        {
            var player = PlayerById(playerid);

            if (player.HasDied())
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