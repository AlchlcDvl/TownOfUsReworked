using TownOfUsReworked.Statuses;

namespace TownOfUsReworked.Monos;

public sealed class StatusHandler : MonoBehaviour
{
    private PlayerControl Player { get; set; }

    [HideFromIl2Cpp]
    private List<BaseStatus> Statuses { get; } = [];

    public void Awake() => Player = GetComponent<PlayerControl>();

    public void Update()
    {
        foreach (var status in Statuses)
        {
            if (status is BaseTimedStatus { Active: true } timed)
            {
                timed.Timer -= Time.deltaTime;

                if (timed.Timer <= 0f)
                    timed.OnTimerEnd();
            }

            status.OnPlayerUpdate();

            if (Player.AmOwner)
                status.OnLocalUpdate();
        }
    }

    [HideFromIl2Cpp]
    public void AddStatus(BaseStatus status)
    {
        Statuses.Add(status);
        status.OnAdd();
    }

    [HideFromIl2Cpp]
    public void RemoveStatus(BaseStatus status)
    {
        Statuses.Remove(status);
        status.OnRemove();
    }
}