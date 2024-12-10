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

        CustomArrow.AllArrows.Where(x => !x.Owner.AmOwner).ForEach(x => x.Update());
        AllButtons.ForEach(x => x.Timers());
        HUD()?.ReportButton?.ToggleVisible(!CustomPlayer.Local.HasDied() && !CustomPlayer.Local.Is(LayerEnum.Coward) && !CustomPlayer.Local.Is(Faction.GameMode) && !Meeting() &&
            !MapBehaviourPatches.MapActive);

        foreach (var id in UninteractiblePlayers.Keys)
        {
            var player = PlayerById(id);

            if (player.HasDied())
                continue;

            if (UninteractiblePlayers.TryGetValue(player.PlayerId, out var time) && time.AddSeconds(UninteractiblePlayers2[player.PlayerId]) < DateTime.UtcNow)
            {
                UninteractiblePlayers.Remove(player.PlayerId);
                UninteractiblePlayers2.Remove(player.PlayerId);
            }
        }

        if (BetterSabotages.CamouflagedComms)
        {
            if (Ship().Systems.TryGetValue(SystemTypes.Comms, out var comms))
            {
                var comms1 = comms.TryCast<HudOverrideSystemType>();

                if (comms1 != null && comms1.IsActive)
                {
                    CommsEnabled = true;
                    Camouflage();
                    return;
                }

                var comms2 = comms.TryCast<HqHudSystemType>();

                if (comms2 != null && comms2.IsActive)
                {
                    CommsEnabled = true;
                    Camouflage();
                    return;
                }
            }

            if (CommsEnabled && !(CamouflagerEnabled || GodfatherEnabled))
            {
                CommsEnabled = false;
                DefaultOutfitAll();
            }
        }
    }
}