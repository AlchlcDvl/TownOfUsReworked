namespace TownOfUsReworked.Monos;

public sealed class FootprintHandler : MonoBehaviour
{
    private PlayerControl Player;
    private bool IsEven;
    private float Time2;

    public void Awake()
    {
        Player = GetComponent<PlayerControl>();
        SpawnFootprint();
    }

    public void Update()
    {
        if (Player.HasDied() || Meeting())
            return;

        Time2 += Time.deltaTime;

        if (Time2 < Detective.FootprintInterval)
            return;

        Time2 = 0f;
        SpawnFootprint();
    }

    private void SpawnFootprint()
    {
        var print = new GameObject("Footprint") { layer = LayerMask.NameToLayer("Players") }.AddComponent<Footprint>();
        print.Player = Player;
        print.IsEven = IsEven = !IsEven;
    }
}