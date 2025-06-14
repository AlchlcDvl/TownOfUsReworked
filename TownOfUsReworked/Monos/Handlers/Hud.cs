#pragma warning disable CA1822

namespace TownOfUsReworked.Monos;

public sealed class HudHandler : MonoBehaviour
{
    private bool CommsEnabled;
    public bool CamouflagerEnabled;
    public bool IsCamoed => CommsEnabled || CamouflagerEnabled;

    public void Update()
    {
        if (IsLobby() || IsEnded() || NoPlayers() || IsHnS() || !HudManager.InstanceExists || !Ship() || HUD().IsIntroDisplayed || !LayerHandler.Handlers.ContainsKey(LocalPlayer.PlayerId))
            return;

        if (LocalBlocked() && ActiveTask())
            ActiveTask().Close();

        CustomArrow.AllArrows.ForEach(x => x.Update());
        CustomButton.AllButtons.ForEach(x => x.Timers());
        HUD().ReportButton?.ToggleVisible(!LocalPlayer.HasDied() && !LocalPlayer.Is<Coward>() && !LocalPlayer.Is(Faction.GameMode) && !Meeting() && !MapBehaviourPatches.MapActive);

        foreach (var player in UninteractablePlayers.Keys.Select(id => PlayerById(id)).Where(player => !player.HasDied()))
        {
            if (!UninteractablePlayers.TryGetValue(player.PlayerId, out var time) || !UninteractablePlayers2.TryGetValue(player.PlayerId, out var limit) || Time.time - time > limit)
                continue;

            UninteractablePlayers.Remove(player.PlayerId);
            UninteractablePlayers2.Remove(player.PlayerId);
        }
    }
}