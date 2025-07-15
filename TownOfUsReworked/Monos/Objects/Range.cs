namespace TownOfUsReworked.Monos;

public abstract class Range : MonoBehaviour
{
    public static readonly List<GameObject> AllItems = [];
    protected static int Number { get; private set; }

    public PlayerControl Owner;
    public float Size;

    public virtual void Update() => transform.Rotate(Vector3.forward * (10 * Time.deltaTime));

    public void OnDestroy() => AllItems.Remove(gameObject);

    protected static GameObject CreateRange(UColor color, float scale, string name)
    {
        var item = new GameObject(name) { layer = LayerMask.NameToLayer("Players") };
        var transform = item.transform;
        var rend = item.AddComponent<SpriteRenderer>();
        rend.sprite = GetSprite("Range");
        rend.color = color;
        Number = AllItems.Count + 1;
        var numberText = Instantiate(HUD().KillButton.cooldownTimerText, transform);
        numberText.text = $"<size=300%>{Number}</size>";
        numberText.fontStyle = FontStyles.Bold;
        numberText.name = $"{name}Number{Number}";
        numberText.gameObject.SetActive(true);

        if (IsSubmerged())
            item.AddSubmergedComponent("ElevatorMover");

        AllItems.Add(item);
        Coroutines.Start(PerformTimedAction(1f, p => item.transform.localScale = new Vector3(scale * 0.25f, scale * 0.25f, 1f) * p));
        return item;
    }
}