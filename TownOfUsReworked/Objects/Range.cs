using Cpp2IL.Core.Extensions;

namespace TownOfUsReworked.Objects;

public class Range
{
    public static readonly List<Range> AllItems = new();

    private GameObject Item { get; }
    public Transform Transform => Item?.transform;
    public int Number { get; }
    private TextMeshPro NumberText { get; }
    public PlayerControl Owner { get; }
    public float Size { get; }

    public Range(PlayerControl owner, UColor color, float scale, string name)
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

    public void Destroy()
    {
        AllItems.Remove(this);

        if (!Item)
            return;

        NumberText.gameObject.SetActive(false);
        NumberText.gameObject.Destroy();
        Item.SetActive(false);
        Item.Destroy();
    }

    public virtual void Update()
    {
        if (!Transform)
            return;

        Transform.Rotate(Vector3.forward * 10 * Time.fixedDeltaTime);
    }

    public static void DestroyAll()
    {
        var dupe = AllItems.Clone();
        dupe.ForEach(p => p.Destroy());
        AllItems.Clear();
        dupe.Clear();
    }
}