namespace TownOfUsReworked.Objects;

public class Range
{
    public static readonly List<Range> AllItems = new();
    private readonly GameObject Item;
    public Transform Transform => Item?.transform;
    public readonly int Number;
    private readonly TextMeshPro NumberText;
    public readonly PlayerControl Owner;
    public readonly float Size;

    public Range(PlayerControl owner, Color color, float scale, string name)
    {
        Owner = owner;
        Item = new(name) { layer = 11 };
        Item.AddSubmergedComponent("ElevatorMover");
        var position = Owner.GetTruePosition();
        Transform.position = new(position.x, position.y, (position.y / 1000f) + 0.001f);
        Transform.localScale = new(scale * 0.25f, scale * 0.25f, 1f);
        var rend = Item.AddComponent<SpriteRenderer>();
        rend.sprite = GetSprite("Range");
        rend.color = color;
        Item.SetActive(true);
        Number = AllItems.Count + 1;
        NumberText = UObject.Instantiate(HUD.KillButton.cooldownTimerText, Transform);
        NumberText.text = $"<size=300%>{Number}</size>";
        NumberText.fontStyle = FontStyles.Bold;
        NumberText.name = $"{name}Number{Number}";
        NumberText.gameObject.SetActive(true);
        Size = scale;
        AllItems.Add(this);
    }

    public void Destroy(bool remove = true)
    {
        if (Item == null)
            return;

        NumberText.gameObject.SetActive(false);
        NumberText.gameObject.Destroy();
        Item.SetActive(false);
        Item.Destroy();
        Stop();

        if (remove)
            AllItems.Remove(this);
    }

    public virtual IEnumerator Timer() => null;

    public virtual void Update()
    {
        if (Transform == null)
            return;

        Transform.Rotate(Vector3.forward * 10 * Time.fixedDeltaTime);
    }

    public virtual void Stop() => Coroutines.Stop(Timer());

    public static void DestroyAll()
    {
        AllItems.ForEach(p => p.Destroy(false));
        AllItems.Clear();
    }
}