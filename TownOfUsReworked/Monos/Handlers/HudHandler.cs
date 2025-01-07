namespace TownOfUsReworked.Monos;

public class HudHandler : MonoBehaviour
{
    private bool CommsEnabled;
    public bool CamouflagerEnabled;
    public bool GodfatherEnabled;
    public bool IsCamoed => CommsEnabled || CamouflagerEnabled || GodfatherEnabled;

    public static HudHandler Instance { get; private set; }

    public HudHandler(IntPtr ptr) : base(ptr) => Instance = this;

    public void Update()
    {
        if (IsLobby() || IsEnded() || NoPlayers() || IsHnS() || !HUD() || !Ship() || IntroCutscene.Instance)
            return;

        if (LocalBlocked() && ActiveTask())
            ActiveTask().Close();

        CustomArrow.AllArrows.ForEach(x => x.Update());
        AllButtons.ForEach(x => x.Timers());
        HUD()?.ReportButton?.ToggleVisible(!CustomPlayer.Local.HasDied() && !CustomPlayer.Local.Is<Coward>() && !CustomPlayer.Local.Is(Faction.GameMode) && !Meeting() &&
            !MapBehaviourPatches.MapActive);

        foreach (var id in UninteractiblePlayers.Keys)
        {
            var player = PlayerById(id);

            if (player.HasDied())
                continue;

            if (UninteractiblePlayers.TryGetValue(player.PlayerId, out var time) && UninteractiblePlayers2.TryGetValue(player.PlayerId, out var limit) && Time.time - time < limit)
            {
                UninteractiblePlayers.Remove(player.PlayerId);
                UninteractiblePlayers2.Remove(player.PlayerId);
            }
        }

        if (BetterSabotages.CamouflagedComms)
        {
            if (Ship().Systems.TryGetValue(SystemTypes.Comms, out var sab) && sab.TryCast<IActivatable>(out var comms) && comms.IsActive)
            {
                CommsEnabled = true;
                Camouflage();
            }
            else if (CommsEnabled && !(CamouflagerEnabled || GodfatherEnabled))
            {
                CommsEnabled = false;
                DefaultOutfitAll();
            }
        }
    }
}