namespace TownOfUsReworked.Modules;

public sealed class Ash : IDisposable
{
    public static readonly List<Ash> AllPiles = [];

    private GameObject Pile { get; set; }

    private Ash(Vector2 position)
    {
        Pile = new("AshPile")
        {
            layer = LayerMask.NameToLayer("Players"),
            transform =
            {
                position = new(position.x, position.y, (position.y / 1000f) + 0.001f),
                localScale = Vector3.one * 0.35f
            }
        };
        Pile.AddComponent<SpriteRenderer>().sprite = GetSprite("AshPile");

        if (IsSubmerged())
            Pile.AddSubmergedComponent("ElevatorMover");

        Pile.SetActive(true);
        AllPiles.Add(this);
    }

    private void Destroy()
    {
        if (!Pile)
            return;

        Pile.SetActive(false);
        Pile.Destroy();
        Pile = null;
    }

    public void Dispose()
    {
        Destroy();
        GC.SuppressFinalize(this);
    }

    public static void CreateAsh(DeadBody body)
    {
        new Ash(body.TruePosition);
        FadeBody(body);
    }
}