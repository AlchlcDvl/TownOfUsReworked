namespace TownOfUsReworked.PlayerLayers;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
[HarmonyPriority(Priority.First)]
public static class DoUndo
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
        var Vent = GetSprite("DefaultVent");

        if (CustomPlayer.Local.Is(Faction.Intruder))
            Vent = GetSprite("IntruderVent");
        else if (CustomPlayer.Local.Is(Faction.Syndicate))
            Vent = GetSprite("SyndicateVent");
        else if (CustomPlayer.Local.Is(Faction.Crew))
            Vent = GetSprite("CrewVent");
        else if (CustomPlayer.Local.Is(Faction.Neutral))
            Vent = GetSprite("NeutralVent");

        if (__instance.ImpostorVentButton.currentTarget == null || LocalBlocked)
            __instance.ImpostorVentButton.SetDisabled();
        else
            __instance.ImpostorVentButton.SetEnabled();

        __instance.ImpostorVentButton.graphic.sprite = Vent;
        __instance.ImpostorVentButton.buttonLabelText.text = LocalBlocked ? "BLOCKED" : "VENT";
        __instance.ImpostorVentButton.buttonLabelText.fontSharedMaterial = __instance.SabotageButton.buttonLabelText.fontSharedMaterial;
        __instance.ImpostorVentButton.gameObject.SetActive((CustomPlayer.Local.CanVent() || CustomPlayer.Local.inVent) && !(Map && Map.IsOpen));

        var closestDead = CustomPlayer.Local.GetClosestBody(CustomGameOptions.ReportDistance);

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

        var Sabo = GetSprite("DefaultSabotage");

        if (CustomPlayer.Local.Is(Faction.Syndicate))
            Sabo = GetSprite("SyndicateSabotage");
        else if (CustomPlayer.Local.Is(Faction.Intruder))
            Sabo = GetSprite("Sabotage");

        __instance.SabotageButton.graphic.sprite = Sabo;
        __instance.SabotageButton.buttonLabelText.text = LocalBlocked ? "BLOCKED" : "SABOTAGE";
        __instance.SabotageButton.gameObject.SetActive(CustomPlayer.Local.CanSabotage() && !(Map && Map.IsOpen));

        if (LocalBlocked && Minigame.Instance)
            Minigame.Instance.Close();

        if (Map && LocalBlocked)
            Map.Close();

        CustomArrow.AllArrows.Where(x => x.Owner != CustomPlayer.Local).ForEach(x => x.Update());
        PlayerLayer.LocalLayers.ForEach(x => x?.UpdateHud(__instance));
        CamouflagerEnabled = false;
        CommsEnabled = false;

        foreach (var role in Role.GetRoles<Chameleon>(LayerEnum.Chameleon))
        {
            if (role.IsSwooped)
                role.Invis();
            else if (role.Enabled)
                role.Uninvis();
        }

        foreach (var role in Role.GetRoles<Escort>(LayerEnum.Escort))
        {
            if (role.Blocking)
                role.Block();
            else if (role.Enabled)
                role.UnBlock();
        }

        foreach (var alt in Role.GetRoles<Altruist>(LayerEnum.Altruist))
        {
            if (alt.IsReviving)
                alt.Revive();
            else if (alt.Enabled)
                alt.UnRevive();
        }

        foreach (var ret in Role.GetRoles<Retributionist>(LayerEnum.Retributionist))
        {
            if (ret.OnEffect)
                ret.Effect();
            else if (ret.Enabled)
                ret.UnEffect();
        }

        foreach (var veteran in Role.GetRoles<Veteran>(LayerEnum.Veteran))
        {
            if (veteran.OnAlert)
                veteran.Alert();
            else if (veteran.Enabled)
                veteran.UnAlert();
        }

        foreach (var amb in Role.GetRoles<Ambusher>(LayerEnum.Ambusher))
        {
            if (amb.OnAmbush)
                amb.Ambush();
            else if (amb.Enabled)
                amb.UnAmbush();
        }

        foreach (var camouflager in Role.GetRoles<Camouflager>(LayerEnum.Camouflager))
        {
            if (camouflager.Camouflaged)
                camouflager.Camouflage();
            else if (camouflager.Enabled)
                camouflager.UnCamouflage();
        }

        foreach (var gf in Role.GetRoles<PromotedGodfather>(LayerEnum.PromotedGodfather))
        {
            if (gf.DelayActive)
                gf.Delay();
            else if (gf.OnEffect)
                gf.Effect();
            else if (gf.Enabled)
                gf.UnEffect();
        }

        foreach (var consort in Role.GetRoles<Consort>(LayerEnum.Consort))
        {
            if (consort.Blocking)
                consort.Block();
            else if (consort.Enabled)
                consort.UnBlock();
        }

        foreach (var disguiser in Role.GetRoles<Disguiser>(LayerEnum.Disguiser))
        {
            if (disguiser.DelayActive)
                disguiser.Delay();
            else if (disguiser.Disguised)
                disguiser.Disguise();
            else if (disguiser.Enabled)
                disguiser.UnDisguise();
        }

        foreach (var enf in Role.GetRoles<Enforcer>(LayerEnum.Enforcer))
        {
            if (enf.DelayActive)
                enf.Delay();
            else if (enf.Bombing)
                enf.Boom();
            else if (enf.Enabled)
                enf.Unboom();
        }

        foreach (var grenadier in Role.GetRoles<Grenadier>(LayerEnum.Grenadier))
        {
            if (grenadier.Flashed)
                grenadier.Flash();
            else if (grenadier.Enabled)
                grenadier.UnFlash();
        }

        foreach (var morphling in Role.GetRoles<Morphling>(LayerEnum.Morphling))
        {
            if (morphling.Morphed)
                morphling.Morph();
            else if (morphling.Enabled)
                morphling.Unmorph();
        }

        foreach (var wraith in Role.GetRoles<Wraith>(LayerEnum.Wraith))
        {
            if (wraith.IsInvis)
                wraith.Invis();
            else if (wraith.Enabled)
                wraith.Uninvis();
        }

        foreach (var glitch in Role.GetRoles<Glitch>(LayerEnum.Glitch))
        {
            if (glitch.IsUsingMimic)
                glitch.Mimic();
            else if (glitch.MimicEnabled)
                glitch.UnMimic();

            if (glitch.IsUsingHack)
                glitch.Hack();
            else if (glitch.HackEnabled)
                glitch.UnHack();
        }

        foreach (var ga in Role.GetRoles<GuardianAngel>(LayerEnum.GuardianAngel))
        {
            if (ga.Protecting)
                ga.Protect();
            else if (ga.Enabled)
                ga.UnProtect();
        }

        foreach (var necro in Role.GetRoles<Necromancer>(LayerEnum.Necromancer))
        {
            if (necro.IsResurrecting)
                necro.Resurrect();
            else if (necro.Resurrecting)
                necro.UnResurrect();
        }

        foreach (var sk in Role.GetRoles<SerialKiller>(LayerEnum.SerialKiller))
        {
            if (sk.Lusted)
                sk.Bloodlust();
            else if (sk.Enabled)
                sk.Unbloodlust();
        }

        foreach (var surv in Role.GetRoles<Survivor>(LayerEnum.Survivor))
        {
            if (surv.Vesting)
                surv.Vest();
            else if (surv.Enabled)
                surv.UnVest();
        }

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

            if (banshee.Screaming)
                banshee.Scream();
            else if (banshee.Enabled)
                banshee.UnScream();
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

        foreach (var concealer in Role.GetRoles<Concealer>(LayerEnum.Concealer))
        {
            if (concealer.Concealed)
                concealer.Conceal();
            else if (concealer.Enabled)
                concealer.UnConceal();
        }

        foreach (var crus in Role.GetRoles<Crusader>(LayerEnum.Crusader))
        {
            if (crus.OnCrusade)
                crus.Crusade();
            else if (crus.Enabled)
                crus.UnCrusade();
        }

        foreach (var pois in Role.GetRoles<Poisoner>(LayerEnum.Poisoner))
        {
            if (pois.Poisoned)
                pois.Poison();
            else if (pois.Enabled)
                pois.PoisonKill();
        }

        foreach (var tk in Role.GetRoles<TimeKeeper>(LayerEnum.TimeKeeper))
        {
            if (tk.Controlling)
                tk.Control();
            else if (tk.Enabled)
                tk.UnControl();
        }

        foreach (var coll in Role.GetRoles<Roles.Collider>(LayerEnum.Collider))
        {
            if (coll.Charged)
                coll.ChargeSelf();
            else if (coll.Enabled)
                coll.DischargeSelf();
        }

        foreach (var drunk in Role.GetRoles<Drunkard>(LayerEnum.Drunkard))
        {
            if (drunk.Confused)
                drunk.Confuse();
            else if (drunk.Enabled)
                drunk.UnConfuse();
        }

        foreach (var reb in Role.GetRoles<PromotedRebel>(LayerEnum.PromotedRebel))
        {
            if (reb.OnEffect)
                reb.Effect();
            else if (reb.Enabled)
                reb.UnEffect();
        }

        foreach (var ss in Role.GetRoles<Shapeshifter>(LayerEnum.Shapeshifter))
        {
            if (ss.Shapeshifted)
                ss.Shapeshift();
            else if (ss.Enabled)
                ss.UnShapeshift();
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