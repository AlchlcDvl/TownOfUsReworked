namespace TownOfUsReworked.Monos;

public abstract class Range : MonoBehaviour
{
    private static readonly List<GameObject> AllItems = [];

    protected static int Number { get; private set; }

    public PlayerControl? Owner;
    public float Size;

    public virtual void Update() => transform.Rotate(Vector3.forward * (10 * Time.deltaTime));

    protected static GameObject CreateRange(UColor color, float scale, string name, Vector2 position)
    {
        var item = new GameObject(name) { layer = LayerMask.NameToLayer("Players") };
        var transform = item.transform;
        var rend = item.AddComponent<SpriteRenderer>();
        rend.sprite = GetSprite("Range");
        rend.color = color;
        Number = AllItems.Count + 1;
        var numberText = Instantiate(HUD()!.KillButton.cooldownTimerText, transform);
        numberText.text = $"<size=300%>{Number}</size>";
        numberText.fontStyle = FontStyles.Bold;
        numberText.name = $"{name}Number{Number}";
        numberText.gameObject.SetActive(true);
        transform.position = new(position.x, position.y, (position.y / 1000f) + 0.001f);

        if (IsSubmerged())
            item.AddSubmergedComponent("ElevatorMover");

        AllItems.Add(item);
        Coroutines.Start(PerformTimedAction(1f, p => transform.localScale = Vector3.one * (0.25f * scale * p)));
        return item;
    }

    public static void Clear()
    {
        foreach (var item in AllItems)
        {
            if (item)
                item.Destroy();
        }

        AllItems.Clear();
    }
}