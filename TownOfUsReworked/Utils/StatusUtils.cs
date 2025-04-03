using TownOfUsReworked.Statuses;

namespace TownOfUsReworked.Utils;

public static class StatusUtils
{
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
}