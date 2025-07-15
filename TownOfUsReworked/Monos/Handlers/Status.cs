// using TownOfUsReworked.Statuses;

// namespace TownOfUsReworked.Monos;

// public sealed class StatusHandler : MonoBehaviour
// {
//     public static readonly Dictionary<byte, StatusHandler> Handlers = [];

//     private PlayerControl Player;
//     private readonly List<BaseStatus> Statuses = [];

//     public void Awake() => Player = GetComponent<PlayerControl>();

//     public void Update()
//     {
//         foreach (var status in Statuses)
//         {
//             if (!status.Active)
//                 continue;

//             if (status is BaseTimedStatus { TimerActive: true } timed)
//             {
//                 timed.Timer -= Time.deltaTime;

//                 if (timed.Timer <= 0f)
//                     timed.TimerEnded();
//             }

//             status.OnPlayerUpdate();

//             if (Player.AmOwner)
//                 status.OnLocalUpdate();
//         }
//     }

//     public void UpdateCurrent() => Handlers[Player.PlayerId] = this;

//     [HideFromIl2Cpp]
//     public void AddStatus(BaseStatus status)
//     {
//         Statuses.Add(status);
//         status.OnAdd();

//         if (status is BaseTimedStatus { AutoStart: true } timed)
//             timed.Timer = timed.Duration;
//     }

//     [HideFromIl2Cpp]
//     public void RemoveStatus(BaseStatus status)
//     {
//         Statuses.Remove(status);
//         status.OnRemove();
//     }
// }