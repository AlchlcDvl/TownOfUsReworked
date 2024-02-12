namespace TownOfUsReworked.Monos;

//Why is this here? well i'm glad you asked dear onlooker, it's because THE FUCKING THING INSISTS ON LOWERING THE FPS OTHERWISE
public class HudHandler : MonoBehaviour
{
    private bool CommsEnabled;
    public bool CamouflagerEnabled;
    public bool GodfatherEnabled;
    public bool IsCamoed => CommsEnabled || CamouflagerEnabled || GodfatherEnabled;

    public static HudHandler Instance { get; private set; }

    public HudHandler(IntPtr ptr) : base(ptr)
    {
        if (Instance)
            Instance.Destroy();

        Instance = this;
    }

    public void Update()
    {
        if (IsLobby || IsEnded || NoPlayers || IsHnS || !HUD || !Ship)
            return;

        HUD.KillButton.SetTarget(null);
        HUD.KillButton.gameObject.SetActive(false);

        CustomPlayer.Local.RegenTask();

        Objects.Range.AllItems.ForEach(x => x.Update());

        var vent = GetSprite("DefaultVent");

        if (CustomPlayer.Local.Is(Faction.Intruder))
            vent = GetSprite("IntruderVent");
        else if (CustomPlayer.Local.Is(Faction.Syndicate))
            vent = GetSprite("SyndicateVent");
        else if (CustomPlayer.Local.Is(Faction.Crew))
            vent = GetSprite("CrewVent");
        else if (CustomPlayer.Local.Is(Faction.Neutral))
            vent = GetSprite("NeutralVent");

        if (HUD.ImpostorVentButton.currentTarget == null || LocalBlocked)
            HUD.ImpostorVentButton.SetDisabled();
        else
            HUD.ImpostorVentButton.SetEnabled();

        HUD.ImpostorVentButton.graphic.sprite = vent;
        HUD.ImpostorVentButton.buttonLabelText.text = LocalBlocked ? "BLOCKED" : "VENT";
        HUD.ImpostorVentButton.buttonLabelText.fontSharedMaterial = HUD.SabotageButton.buttonLabelText.fontSharedMaterial;
        HUD.ImpostorVentButton.gameObject.SetActive((CustomPlayer.Local.CanVent() || CustomPlayer.Local.inVent) && !(Map && Map.IsOpen) && !ActiveTask);

        var closestDead = CustomPlayer.Local.GetClosestBody(maxDistance: CustomGameOptions.ReportDistance);

        if (closestDead == null || CustomPlayer.Local.CannotUse())
            HUD.ReportButton.SetDisabled();
        else
            HUD.ReportButton.SetEnabled();

        HUD.ReportButton.buttonLabelText.text = LocalBlocked ? "BLOCKED" : "REPORT";
        HUD.ReportButton.buttonLabelText.fontSharedMaterial = HUD.SabotageButton.buttonLabelText.fontSharedMaterial;

        if (CustomPlayer.Local.closest == null || LocalBlocked)
            HUD.UseButton.SetDisabled();
        else
            HUD.UseButton.SetEnabled();

        HUD.UseButton.buttonLabelText.text = LocalBlocked ? "BLOCKED" : "USE";
        HUD.UseButton.buttonLabelText.fontSharedMaterial = HUD.SabotageButton.buttonLabelText.fontSharedMaterial;

        if (LocalBlocked)
            HUD.PetButton.SetDisabled();
        else
            HUD.PetButton.SetEnabled();

        HUD.PetButton.buttonLabelText.text = LocalBlocked ? "BLOCKED" : "PET";
        HUD.PetButton.buttonLabelText.fontSharedMaterial = HUD.SabotageButton.buttonLabelText.fontSharedMaterial;

        if (CustomPlayer.Local.CannotUse())
            HUD.SabotageButton.SetDisabled();
        else
            HUD.SabotageButton.SetEnabled();

        var sab = GetSprite("DefaultSabotage");

        if (CustomPlayer.Local.Is(Faction.Syndicate))
            sab = GetSprite("SyndicateSabotage");
        else if (CustomPlayer.Local.Is(Faction.Intruder))
            sab = GetSprite("IntruderSabotage");

        HUD.SabotageButton.graphic.sprite = sab;
        HUD.SabotageButton.buttonLabelText.text = LocalBlocked ? "BLOCKED" : "SABOTAGE";
        HUD.SabotageButton.gameObject.SetActive(CustomPlayer.Local.CanSabotage() && !(Map && Map.IsOpen) && !ActiveTask);

        if (LocalBlocked && ActiveTask)
            ActiveTask.Close();

        if (LocalBlocked && MapPatch.MapActive)
            Map.Close();

        CustomArrow.AllArrows.Where(x => x.Owner != CustomPlayer.Local).ForEach(x => x.Update());
        PlayerLayer.LocalLayers.ForEach(x => x.UpdateHud(HUD));
        PlayerLayer.AllLayers.ForEach(x => x.TryEndEffect());
        CustomButton.AllButtons.ForEach(x => x.Timers());

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

            if (UninteractiblePlayers.ContainsKey(player.PlayerId) && player.moveable && UninteractiblePlayers[player.PlayerId].AddSeconds(6) < DateTime.UtcNow)
                UninteractiblePlayers.Remove(player.PlayerId);
        }

        if (CustomGameOptions.CamouflagedComms)
        {
            if (Ship.Systems.TryGetValue(SystemTypes.Comms, out var comms) == true)
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