namespace TownOfUsReworked.Objects
{
    public class Ash
    {
        public static readonly List<Ash> AllPiles = new();
        public GameObject Pile { get; set; }

        public Ash(Vector2 position)
        {
            Pile = new("AshPile") { layer = 11 };
            Pile.AddSubmergedComponent("ElevatorMover");
            Pile.transform.position = new(position.x, position.y, (position.y / 1000f) + 0.001f);
            Pile.transform.localScale = Vector3.one * 0.35f;
            Pile.AddComponent<SpriteRenderer>().sprite = GetSprite("AshPile");
            Pile.SetActive(true);
            AllPiles.Add(this);
        }

        public void Destroy()
        {
            if (Pile == null)
                return;

            Pile.SetActive(false);
            Pile.Destroy();
            Pile = null;
        }

        public static void DestroyAll()
        {
            AllPiles.ForEach(x => x.Destroy());
            AllPiles.Clear();
        }
    }
}