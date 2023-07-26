namespace TownOfUsReworked.Objects
{
    public class Range
    {
        public static readonly List<Range> AllItems = new();
        public GameObject Item;
        public Transform Transform => Item?.transform;

        public Range(Vector2 position, Color color, float scale, string name)
        {
            Item = new(name) { layer = 11 };
            Item.AddSubmergedComponent("ElevatorMover");
            Transform.position = new(position.x, position.y, (position.y / 1000f) + 0.001f);
            Transform.localScale = new(scale * 0.25f, scale * 0.25f, 1f);
            var rend = Item.AddComponent<SpriteRenderer>();
            rend.sprite = GetSprite("Range");
            rend.color = color;
            Item.SetActive(true);
            AllItems.Add(this);
        }

        public void Destroy(bool remove = true)
        {
            if (Item == null)
                return;

            Item.SetActive(false);
            Item.Destroy();
            Stop();

            if (remove)
                AllItems.Remove(this);
        }

        public virtual IEnumerator Timer() => null;

        public virtual void Update() => Transform?.Rotate(Vector3.forward * 6 * Time.fixedDeltaTime);

        public virtual void Stop() => Coroutines.Stop(Timer());

        public static void DestroyAll()
        {
            foreach (var pile in AllItems)
                pile.Destroy(false);

            AllItems.Clear();
        }
    }
}