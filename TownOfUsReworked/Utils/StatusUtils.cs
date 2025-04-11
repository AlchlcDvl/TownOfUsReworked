using TownOfUsReworked.Statuses;

namespace TownOfUsReworked.Utils;

public static class StatusUtils
{
    private static readonly Dictionary<Type, ParameterInfo[]> TypeToConstructorParamsMap = [];

    public static T AddStatus<T>(this PlayerControl player, params object[] args) where T : BaseStatus
    {
        var status = CreateInstance<T>(args);
        status.Start(player);
        return status;
    }

    public static bool HasStatus<T>(this PlayerControl player) where T : BaseStatus => player.HasStatuses<T>(out _);

    public static bool HasStatus<T>(this PlayerControl player, out T status) where T : BaseStatus
    {
        var result = player.HasStatuses<T>(out var statuses);
        status = statuses.FirstOrDefault();
        return result && status != null;
    }

    public static bool HasStatuses<T>(this PlayerControl player, out IEnumerable<T> statuses) where T : BaseStatus => BaseStatus.AllStatuses.OfType<T>().TryFindingAll(x => x.Player == player, out
        statuses);

    public static void RemoveStatus(this PlayerControl player, BaseStatus status)
    {
        player.GetComponent<StatusHandler>().RemoveStatus(status);
        status.End();
    }

    public static T RpcAddStatus<T>(this PlayerControl player, params object[] args) where T : BaseStatus
    {
        CallRpc(CustomRPC.Misc, [ MiscRPC.SetStatus, typeof(T), player, .. args ]);
        return player.AddStatus<T>(args);
    }

    public static void AddStatusFromRpc(NetData data)
    {
        var type = data.ReadType();
        var player = data.ReadPlayer();

        if (!TypeToConstructorParamsMap.TryGetValue(type, out var parameters))
            TypeToConstructorParamsMap[type] = parameters = type.GetConstructors(AccessTools.all)[0].GetParameters();

        BaseStatus status;

        if (parameters.Length == 0)
            status = (BaseStatus)Activator.CreateInstance(type);
        else
            status = (BaseStatus)Activator.CreateInstance(type, [ .. parameters.Select(x => data.Read(x.ParameterType)) ]);

        status.Start(player);
    }
}