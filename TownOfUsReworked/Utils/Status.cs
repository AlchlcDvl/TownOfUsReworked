// using TownOfUsReworked.Statuses;

// namespace TownOfUsReworked.Utils;

// public static class StatusUtils
// {
//     public static T AddStatus<T>(this PlayerControl player, params object[] args) where T : BaseStatus
//     {
//         var status = CreateInstance<T>(args);
//         status.Start(player);
//         return status;
//     }

//     public static bool HasStatus<T>(this PlayerControl player) where T : BaseStatus => player.HasStatuses<T>(out _);

//     public static bool HasStatus<T>(this PlayerControl player, out T status) where T : BaseStatus
//     {
//         var result = player.HasStatuses<T>(out var statuses);
//         status = statuses.FirstOrDefault();
//         return result && status is not null;
//     }

//     public static bool HasStatuses<T>(this PlayerControl player, out IEnumerable<T> statuses) where T : BaseStatus => BaseStatus.AllStatuses.OfType<T>().TryFindingAll(x => x.Player == player, out
//         statuses);

//     public static void RemoveStatus(this PlayerControl player, BaseStatus status)
//     {
//         StatusHandler.Handlers[player.PlayerId].RemoveStatus(status);
//         status.End();
//     }

//     public static T RpcAddStatus<T>(this PlayerControl player, params object[] args) where T : BaseStatus
//     {
//         var writer = CreateWriter(CustomRPC.Misc, MiscRPC.SetStatus, typeof(T), player, (ushort)args.Length);
//         writer?.WriteWithTypeCode(args);
//         writer?.Send();
//         return player.AddStatus<T>(args);
//     }

//     public static T RpcAddStatusWithoutType<T>(this PlayerControl player, CustomRPC rpcId, object[] rpcArgs, params object[] args) where T : BaseStatus
//     {
//         var writer = CreateWriter(rpcId, [..rpcArgs, player, (ushort)args.Length]);
//         writer?.WriteWithTypeCode(args);
//         writer?.Send();
//         return player.AddStatus<T>(args);
//     }

//     public static BaseStatus AddStatusFromRpc(NetData data)
//     {
//         var type = data.ReadType();
//         var player = data.ReadPlayer();
//         var count = data.ReadUShort();
//         var args = new List<object>();

//         while (count-- > 0)
//             args.Add(data.Read());

//         var status = (BaseStatus)Activator.CreateInstance(type, [.. args]);
//         status?.Start(player);
//         return status;
//     }

//     public static T AddStatusFromRpcWithoutType<T>(NetData data) where T : BaseStatus
//     {
//         var player = data.ReadPlayer();
//         var count = data.ReadUShort();
//         var args = new List<object>();

//         while (count-- > 0)
//             args.Add(data.Read());

//         var status = CreateInstance<T>([.. args]);
//         status.Start(player);
//         return status;
//     }
// }