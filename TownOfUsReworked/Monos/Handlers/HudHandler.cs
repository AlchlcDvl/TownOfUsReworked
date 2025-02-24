namespace TownOfUsReworked.Monos;

public class HudHandler : MonoBehaviour
{
    private bool CommsEnabled;
    public bool CamouflagerEnabled;
    public bool IsCamoed => CommsEnabled || CamouflagerEnabled;

    public void Update()
    {
        if (IsLobby() || IsEnded() || NoPlayers() || IsHnS() || !HudManager.InstanceExists || !Ship() || IntroCutscene.Instance)
            return;

        if (LocalBlocked() && ActiveTask())
            ActiveTask().Close();

        CustomArrow.AllArrows.ForEach(x => x.Update());
        CustomButton.AllButtons.ForEach(x => x.Timers());
        HUD().ReportButton?.ToggleVisible(!CustomPlayer.Local.HasDied() && !CustomPlayer.Local.Is<Coward>() && !CustomPlayer.Local.Is(Faction.GameMode) && !Meeting() &&
            !MapBehaviourPatches.MapActive);

        foreach (var player in UninteractablePlayers.Keys.Select(id => PlayerById(id)).Where(player => !player.HasDied()))
        {
            if (!UninteractablePlayers.TryGetValue(player.PlayerId, out var time) || !UninteractablePlayers2.TryGetValue(player.PlayerId, out var limit) || Time.time - time > limit)
                continue;

            UninteractablePlayers.Remove(player.PlayerId);
            UninteractablePlayers2.Remove(player.PlayerId);
        }

        if (!BetterSabotages.CamouflagedComms)
            return;

        if (Ship().Systems.TryGetValue(SystemTypes.Comms, out var sab) && sab.TryCast<IActivatable>(out var comms) && comms.IsActive)
        {
            CommsEnabled = true;
            Camouflage();
        }
        else if (CommsEnabled && !CamouflagerEnabled)
        {
            CommsEnabled = false;
            DefaultOutfitAll();
        }
    }
}