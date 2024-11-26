namespace TownOfUsReworked.Monos;

public class FootprintHandler : MonoBehaviour
{
    private PlayerControl Player { get; set; }
    private bool IsEven { get; set; }
    private float Time2 { get; set; }

    public void Awake()
    {
        Player = GetComponent<PlayerControl>();
        SpawnFootprint();
    }

    public void Update()
    {
        if (Player.HasDied() || Player.AmOwner || Meeting())
            return;

        Time2 += Time.deltaTime;

        if (Time2 < Detective.FootprintInterval)
            return;

        Time2 = 0f;
        SpawnFootprint();
    }

    private void SpawnFootprint()
    {
        var gameObject = new GameObject("Footprint") { layer = 11 };
        var print = gameObject.AddComponent<FootprintB>();
        print.Player = Player;
        print.IsEven = IsEven = !IsEven;
    }
}