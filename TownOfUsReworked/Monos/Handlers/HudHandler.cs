namespace TownOfUsReworked.Monos;

// Why is this here? well i'm glad you asked dear onlooker, it's because THE FUCKING THING INSISTS ON LOWERING THE FPS OTHERWISE
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
        if (IsLobby || IsEnded || NoPlayers || IsHnS || !HUD || !Ship || IntroCutscene.Instance)
            return;

        if (LocalBlocked && ActiveTask)
            ActiveTask.Close();

        CustomArrow.AllArrows.Where(x => x.Owner != CustomPlayer.Local).ForEach(x => x.Update());
        PlayerLayer.LocalLayers.ForEach(x => x.UpdateHud(HUD));
        CustomPlayer.Local.GetButtons().ForEach(x => x.SetActive());
        AllButtons.ForEach(x => x.Timers());
        HUD?.ReportButton?.gameObject?.SetActive(!CustomPlayer.Local.HasDied() && !CustomPlayer.Local.Is(LayerEnum.Coward) && !CustomPlayer.Local.Is(Faction.GameMode));

        foreach (var phantom in PlayerLayer.GetLayers<Phantom>())
        {
            if (!phantom.Caught)
                phantom.Fade();
            else if (phantom.Faded)
                phantom.UnFade();
        }

        foreach (var banshee in PlayerLayer.GetLayers<Banshee>())
        {
            if (!banshee.Caught)
                banshee.Fade();
            else if (banshee.Faded)
                banshee.UnFade();
        }

        foreach (var ghoul in PlayerLayer.GetLayers<Ghoul>())
        {
            if (!ghoul.Caught)
                ghoul.Fade();
            else if (ghoul.Faded)
                ghoul.UnFade();
        }

        foreach (var revealer in PlayerLayer.GetLayers<Revealer>())
        {
            if (!revealer.Caught)
                revealer.Fade();
            else if (revealer.Faded)
                revealer.UnFade();
        }

        foreach (var body in AllBodies)
        {
            var renderer = body.MyRend();
            var player = PlayerByBody(body);

            if (IsCamoed)
                PlayerMaterial.SetColors(UColor.grey, renderer);
            else if (SurveillancePatches.NVActive)
                PlayerMaterial.SetColors(UColor.green, renderer);
            else if (player)
                PlayerMaterial.SetColors(player.Data.DefaultOutfit.ColorId, renderer);

            if (player)
                body.transform.localScale = CustomPlayer.Custom(player).SizeFactor;
        }

        foreach (var id in UninteractiblePlayers.Keys)
        {
            var player = PlayerById(id);

            if (player.HasDied())
                continue;

            if (UninteractiblePlayers.TryGetValue(player.PlayerId, out var time) && time.AddSeconds(UninteractiblePlayers2[player.PlayerId]) < DateTime.UtcNow)
                UninteractiblePlayers.Remove(player.PlayerId);
        }

        if (CustomGameOptions.CamouflagedComms)
        {
            if (Ship.Systems.TryGetValue(SystemTypes.Comms, out var comms))
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