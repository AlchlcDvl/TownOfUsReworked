namespace TownOfUsReworked.Monos;

/// <summary>
/// Simple ash object that's formed when an <c><see cref="Arsonist"/></c> ignites and cremates bodies.
/// </summary>
public sealed class Ash : MonoBehaviour
{
    private static readonly List<GameObject> AllPiles = [];

    /// <summary>
    /// Awake event.
    /// </summary>
    public void Awake()
    {
        this.AddComponent<SpriteRenderer>().sprite = GetSprite("AshPile");

        if (IsSubmerged())
            gameObject.AddSubmergedComponent("ElevatorMover");
    }

    public static void CreateAsh(DeadBody body)
    {
        var position = body.TruePosition;
        var go = new GameObject($"AshPile{body.ParentId}")
        {
            layer = LayerMask.NameToLayer("Players"),
            transform =
            {
                position = new(position.x, position.y, (position.y / 1000f) + 0.001f),
                localScale = Vector3.one * 0.35f
            }
        };
        go.AddComponent<Ash>();
        AllPiles.Add(go);
        FadeBody(body);
    }

    public static void Clear()
    {
        foreach (var pile in AllPiles)
        {
            if (pile)
                pile.Destroy();
        }

        AllPiles.Clear();
    }
}