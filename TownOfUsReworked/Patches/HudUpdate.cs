namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
[HarmonyPriority(Priority.First)]
public static class HudUpdate
{
    private static bool CommsEnabled;
    public static bool CamouflagerEnabled;
    public static bool GodfatherEnabled;
    public static bool IsCamoed => CommsEnabled || CamouflagerEnabled || GodfatherEnabled;

    public static void Postfix(HudManager __instance)
    {
        if (IsLobby || IsEnded || NoPlayers || IsHnS)
            return;

        if (!Sprites.ContainsKey("DefaultVent"))
            Sprites.Add("DefaultVent", __instance.ImpostorVentButton.graphic.sprite);

        if (!Sprites.ContainsKey("DefaultSabotage"))
            Sprites.Add("DefaultSabotage", __instance.SabotageButton.graphic.sprite);

        __instance.KillButton.SetTarget(null);
        __instance.KillButton.gameObject.SetActive(false);

        CustomPlayer.Local.RegenTask();

        var vent = GetSprite("DefaultVent");

        if (CustomPlayer.Local.Is(Faction.Intruder))
            vent = GetSprite("IntruderVent");
        else if (CustomPlayer.Local.Is(Faction.Syndicate))
            vent = GetSprite("SyndicateVent");
        else if (CustomPlayer.Local.Is(Faction.Crew))
            vent = GetSprite("CrewVent");
        else if (CustomPlayer.Local.Is(Faction.Neutral))
            vent = GetSprite("NeutralVent");

        if (__instance.ImpostorVentButton.currentTarget == null || LocalBlocked)
            __instance.ImpostorVentButton.SetDisabled();
        else
            __instance.ImpostorVentButton.SetEnabled();

        __instance.ImpostorVentButton.graphic.sprite = vent;
        __instance.ImpostorVentButton.buttonLabelText.text = LocalBlocked ? "BLOCKED" : "VENT";
        __instance.ImpostorVentButton.buttonLabelText.fontSharedMaterial = __instance.SabotageButton.buttonLabelText.fontSharedMaterial;
        __instance.ImpostorVentButton.gameObject.SetActive((CustomPlayer.Local.CanVent() || CustomPlayer.Local.inVent) && !(Map && Map.IsOpen));

        var closestDead = CustomPlayer.Local.GetClosestBody(maxDistance: CustomGameOptions.ReportDistance);

        if (closestDead == null || CustomPlayer.Local.CannotUse())
            __instance.ReportButton.SetDisabled();
        else
            __instance.ReportButton.SetEnabled();

        __instance.ReportButton.buttonLabelText.text = LocalBlocked ? "BLOCKED" : "REPORT";
        __instance.ReportButton.buttonLabelText.fontSharedMaterial = __instance.SabotageButton.buttonLabelText.fontSharedMaterial;

        if (CustomPlayer.Local.closest == null || LocalBlocked)
            __instance.UseButton.SetDisabled();
        else
            __instance.UseButton.SetEnabled();

        __instance.UseButton.buttonLabelText.text = LocalBlocked ? "BLOCKED" : "USE";
        __instance.UseButton.buttonLabelText.fontSharedMaterial = __instance.SabotageButton.buttonLabelText.fontSharedMaterial;

        if (LocalBlocked)
            __instance.PetButton.SetDisabled();
        else
            __instance.PetButton.SetEnabled();

        __instance.PetButton.buttonLabelText.text = LocalBlocked ? "BLOCKED" : "PET";
        __instance.PetButton.buttonLabelText.fontSharedMaterial = __instance.SabotageButton.buttonLabelText.fontSharedMaterial;

        if (CustomPlayer.Local.CannotUse())
            __instance.SabotageButton.SetDisabled();
        else
            __instance.SabotageButton.SetEnabled();

        var sab = GetSprite("DefaultSabotage");

        if (CustomPlayer.Local.Is(Faction.Syndicate))
            sab = GetSprite("SyndicateSabotage");
        else if (CustomPlayer.Local.Is(Faction.Intruder))
            sab = GetSprite("IntruderSabotage");

        __instance.SabotageButton.graphic.sprite = sab;
        __instance.SabotageButton.buttonLabelText.text = LocalBlocked ? "BLOCKED" : "SABOTAGE";
        __instance.SabotageButton.gameObject.SetActive(CustomPlayer.Local.CanSabotage() && !(Map && Map.IsOpen));

        if (LocalBlocked && Minigame.Instance)
            Minigame.Instance.Close();

        if (Map && LocalBlocked)
            Map.Close();

        CustomArrow.AllArrows.Where(x => x.Owner != CustomPlayer.Local).ForEach(x => x?.Update());
        PlayerLayer.LocalLayers.ForEach(x => x?.UpdateHud(__instance));
        PlayerLayer.AllLayers.ForEach(x => x?.TryEndEffect());
        CustomButton.AllButtons.ForEach(x => x?.Timers());

        foreach (var phantom in Role.GetRoles<Phantom>(LayerEnum.Phantom))
        {
            if (!phantom.Caught)
                phantom.Fade();
            else if (phantom.Faded)
                phantom.UnFade();
        }

        foreach (var banshee in Role.GetRoles<Banshee>(LayerEnum.Banshee))
        {
            if (!banshee.Caught)
                banshee.Fade();
            else if (banshee.Faded)
                banshee.UnFade();
        }

        foreach (var ghoul in Role.GetRoles<Ghoul>(LayerEnum.Ghoul))
        {
            if (!ghoul.Caught)
                ghoul.Fade();
            else if (ghoul.Faded)
                ghoul.UnFade();
        }

        foreach (var revealer in Role.GetRoles<Revealer>(LayerEnum.Revealer))
        {
            if (!revealer.Caught)
                revealer.Fade();
            else if (revealer.Faded)
                revealer.UnFade();
        }

        foreach (var body in AllBodies)
        {
            var renderer = body.MyRend();

            if (IsCamoed)
                PlayerMaterial.SetColors(UColor.grey, renderer);
            else
            {
                renderer.material.SetColor("_BackColor", PlayerByBody(body).GetShadowColor());
                renderer.material.SetColor("_BodyColor", PlayerByBody(body).GetPlayerColor());
            }
        }

        if (CustomGameOptions.CamouflagedComms)
        {
            if (ShipStatus.Instance)
            {
                switch (TownOfUsReworked.NormalOptions.MapId)
                {
                    case 0:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                        var comms5 = ShipStatus.Instance.Systems[SystemTypes.Comms]?.Cast<HudOverrideSystemType>();

                        if (comms5.IsActive)
                        {
                            CommsEnabled = true;
                            Camouflage();
                            return;
                        }

                        break;

                    case 1:
                        var comms2 = ShipStatus.Instance.Systems[SystemTypes.Comms]?.Cast<HqHudSystemType>();

                        if (comms2.IsActive)
                        {
                            CommsEnabled = true;
                            Camouflage();
                            return;
                        }

                        break;
                }
            }

            if (CommsEnabled)
            {
                CommsEnabled = false;
                CamouflagerEnabled = false;
                GodfatherEnabled = false;
                DefaultOutfitAll();
            }
        }
    }
}

[HarmonyPatch(typeof(UObject), nameof(UObject.Destroy), typeof(UObject))]
public static class HUDClose
{
    public static void Postfix(UObject obj)
    {
        if (obj == null || obj != ExileController.Instance?.gameObject)
            return;

        CustomPlayer.Local.EnableButtons();
        ButtonUtils.ResetCustomTimers(CooldownType.Meeting);
    }
}

[HarmonyPatch(typeof(IntroCutscene._CoBegin_d__33), nameof(IntroCutscene._CoBegin_d__33.MoveNext))]
public static class Start
{
    public static void Postfix()
    {
        CustomPlayer.Local.EnableButtons();
        ButtonUtils.ResetCustomTimers(CooldownType.Start);
    }
}