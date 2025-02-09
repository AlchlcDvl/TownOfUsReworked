namespace TownOfUsReworked.Monos;

public class Range : MonoBehaviour
{
    public static readonly List<GameObject> AllItems = [];
    public static int Number { get; set; }

    public PlayerControl Owner { get; set; }
    public float Size { get; set; }

    public virtual void Update() => transform.Rotate(Vector3.forward * 10 * Time.fixedDeltaTime);

    public void OnDestroy() => AllItems.Remove(gameObject);

    public static GameObject CreateRange(UColor color, float scale, string name)
    {
        var item = new GameObject(name) { layer = 11 };
        item.AddSubmergedComponent("ElevatorMover");
        var transform = item.transform;
        var rend = item.AddComponent<SpriteRenderer>();
        rend.sprite = GetSprite("Range");
        rend.color = color;
        item.SetActive(true);
        Number = AllItems.Count + 1;
        var numberText = Instantiate(HUD().KillButton.cooldownTimerText, transform);
        numberText.text = $"<size=300%>{Number}</size>";
        numberText.fontStyle = FontStyles.Bold;
        numberText.name = $"{name}Number{Number}";
        numberText.gameObject.SetActive(true);
        AllItems.Add(item);
        Coroutines.Start(PerformTimedAction(1f, p => item.transform.localScale = new Vector3(scale * 0.25f, scale * 0.25f, 1f) * p));
        return item;
    }
}