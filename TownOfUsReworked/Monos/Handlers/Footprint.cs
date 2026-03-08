namespace TownOfUsReworked.Monos;

public sealed class FootprintHandler : MonoBehaviour
{
    private PlayerControl? Player;
    private bool IsEven;
    private float ElapsedTime;

    public void Awake() => Player = GetComponent<PlayerControl>();

    public void Update()
    {
        if (Player!.HasDied() || Meeting() || Player!.MyPhysics.body.velocity.sqrMagnitude < 0.05f)
            return;

        ElapsedTime += Time.deltaTime;

        if (ElapsedTime < Detective.FootprintInterval)
            return;

        ElapsedTime = 0f;
        Footprint.Produce(Player!, IsEven = !IsEven);
    }
}