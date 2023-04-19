using HarmonyLib;
using TownOfUsReworked.Data;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.First)]
    public static class DoUndo
    {
        private static bool CommsEnabled;
        private static bool CamouflagerEnabled;
        public static bool IsCamoed => CommsEnabled || CamouflagerEnabled;

        public static void Postfix(HudManager __instance)
        {
            if (ConstantVariables.IsLobby || ConstantVariables.IsEnded || ConstantVariables.Inactive)
                return;

            foreach (var layer in PlayerLayer.GetLayers(PlayerControl.LocalPlayer))
                layer.UpdateHud(__instance);

            foreach (var role in Role.GetRoles<Chameleon>(RoleEnum.Chameleon))
            {
                if (role.IsSwooped)
                    role.Invis();
                else if (role.Enabled)
                    role.Uninvis();
            }

            foreach (var role in Role.GetRoles<Escort>(RoleEnum.Escort))
            {
                if (role.Blocking)
                    role.Block();
                else if (role.Enabled)
                    role.UnBlock();
            }

            foreach (var ret in Role.GetRoles<Retributionist>(RoleEnum.Retributionist))
            {
                if (ret.RevivedRole == null)
                    continue;

                if (ret.OnEffect)
                {
                    if (ret.IsCham)
                        ret.Invis();
                    else if (ret.IsVet)
                        ret.Alert();
                    else if (ret.IsEsc)
                        ret.Block();
                }
                else if (ret.Enabled)
                {
                    if (ret.IsCham)
                        ret.Uninvis();
                    else if (ret.IsVet)
                        ret.UnAlert();
                    else if (ret.IsEsc)
                        ret.UnBlock();
                }
            }

            foreach (var veteran in Role.GetRoles<Veteran>(RoleEnum.Veteran))
            {
                if (veteran.OnAlert)
                    veteran.Alert();
                else if (veteran.Enabled)
                    veteran.UnAlert();
            }

            foreach (var amb in Role.GetRoles<Ambusher>(RoleEnum.Ambusher))
            {
                if (amb.OnAmbush)
                    amb.Ambush();
                else if (amb.Enabled)
                    amb.UnAmbush();
            }

            CamouflagerEnabled = false;
            CommsEnabled = false;

            foreach (var camouflager in Role.GetRoles<Camouflager>(RoleEnum.Camouflager))
            {
                if (camouflager.Camouflaged)
                {
                    camouflager.Camouflage();
                    CamouflagerEnabled = true;
                }
                else if (camouflager.Enabled)
                {
                    camouflager.UnCamouflage();
                    CamouflagerEnabled = false;
                }
            }

            foreach (var gf in Role.GetRoles<PromotedGodfather>(RoleEnum.Godfather))
            {
                if (gf.FormerRole == null || gf.FormerRole?.RoleType == RoleEnum.Impostor)
                    continue;

                if (gf.DelayActive)
                {
                    if (gf.IsEnf)
                        gf.BombDelay();
                    else if (gf.IsDisg)
                        gf.DisgDelay();
                }
                else if (gf.OnEffect)
                {
                    if (gf.IsGren)
                        gf.Flash();
                    else if (gf.IsDisg)
                        gf.Disguise();
                    else if (gf.IsMorph)
                        gf.Morph();
                    else if (gf.IsTM)
                        gf.TimeFreeze();
                    else if (gf.IsWraith)
                        gf.Invis();
                    else if (gf.IsCons)
                        gf.Block();
                    else if (gf.IsCamo)
                    {
                        gf.Camouflage();
                        CamouflagerEnabled = true;
                    }
                }
                else if (gf.Enabled)
                {
                    if (gf.IsGren)
                        gf.UnFlash();
                    else if (gf.IsDisg)
                        gf.UnDisguise();
                    else if (gf.IsMorph)
                        gf.Unmorph();
                    else if (gf.IsTM)
                        gf.Unfreeze();
                    else if (gf.IsWraith)
                        gf.Uninvis();
                    else if (gf.IsCons)
                        gf.UnBlock();
                    else if (gf.IsCamo)
                    {
                        gf.UnCamouflage();
                        CamouflagerEnabled = false;
                    }
                }
            }

            foreach (var consort in Role.GetRoles<Consort>(RoleEnum.Consort))
            {
                if (consort.Blocking)
                    consort.Block();
                else if (consort.Enabled)
                    consort.UnBlock();
            }

            foreach (var disguiser in Role.GetRoles<Disguiser>(RoleEnum.Disguiser))
            {
                if (disguiser.DelayActive)
                    disguiser.Delay();
                else if (disguiser.Disguised)
                    disguiser.Disguise();
                else if (disguiser.Enabled)
                    disguiser.UnDisguise();
            }

            foreach (var enf in Role.GetRoles<Enforcer>(RoleEnum.Enforcer))
            {
                if (enf.DelayActive)
                    enf.Delay();
                else if (enf.Bombing)
                    enf.Boom();
                else if (enf.Enabled)
                    enf.Unboom();
            }

            foreach (var grenadier in Role.GetRoles<Grenadier>(RoleEnum.Grenadier))
            {
                if (grenadier.Flashed)
                    grenadier.Flash();
                else if (grenadier.Enabled)
                    grenadier.UnFlash();
            }

            foreach (var morphling in Role.GetRoles<Morphling>(RoleEnum.Morphling))
            {
                if (morphling.Morphed)
                    morphling.Morph();
                else if (morphling.Enabled)
                    morphling.Unmorph();
            }

            foreach (var tm in Role.GetRoles<TimeMaster>(RoleEnum.TimeMaster))
            {
                if (tm.Frozen)
                    tm.TimeFreeze();
                else if (tm.Enabled)
                    tm.Unfreeze();
            }

            foreach (var wraith in Role.GetRoles<Wraith>(RoleEnum.Wraith))
            {
                if (wraith.IsInvis)
                    wraith.Invis();
                else if (wraith.Enabled)
                    wraith.Uninvis();
            }

            foreach (var glitch in Role.GetRoles<Glitch>(RoleEnum.Glitch))
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

            foreach (var ga in Role.GetRoles<GuardianAngel>(RoleEnum.GuardianAngel))
            {
                if (ga.Protecting)
                    ga.Protect();
                else if (ga.Enabled)
                    ga.UnProtect();
            }

            foreach (var necro in Role.GetRoles<Necromancer>(RoleEnum.Necromancer))
            {
                if (necro.IsResurrecting)
                    necro.Resurrect();
                else if (necro.Resurrecting)
                    necro.UnResurrect();
            }

            foreach (var sk in Role.GetRoles<SerialKiller>(RoleEnum.SerialKiller))
            {
                if (sk.Lusted)
                    sk.Bloodlust();
                else if (sk.Enabled)
                    sk.Unbloodlust();
            }

            foreach (var surv in Role.GetRoles<Survivor>(RoleEnum.Survivor))
            {
                if (surv.Vesting)
                    surv.Vest();
                else if (surv.Enabled)
                    surv.UnVest();
            }

            foreach (var phantom in Role.GetRoles<Phantom>(RoleEnum.Phantom))
            {
                if (phantom.Player.Data.Disconnected)
                    continue;

                var caught = phantom.Caught;

                if (!caught)
                    phantom.Fade();
                else if (phantom.Faded)
                {
                    Utils.DefaultOutfit(phantom.Player);
                    phantom.Player.MyRend().color = Color.white;
                    phantom.Player.gameObject.layer = LayerMask.NameToLayer("Ghost");
                    phantom.Faded = false;
                    phantom.Player.MyPhysics.ResetMoveState();
                }
            }

            foreach (var banshee in Role.GetRoles<Banshee>(RoleEnum.Banshee))
            {
                if (banshee.Player.Data.Disconnected)
                    continue;

                var caught = banshee.Caught;

                if (!caught)
                    banshee.Fade();
                else if (banshee.Faded)
                {
                    Utils.DefaultOutfit(banshee.Player);
                    banshee.Player.MyRend().color = Color.white;
                    banshee.Player.gameObject.layer = LayerMask.NameToLayer("Ghost");
                    banshee.Faded = false;
                    banshee.Player.MyPhysics.ResetMoveState();
                }

                if (banshee.Screaming)
                    banshee.Scream();
                else if (banshee.Enabled)
                    banshee.UnScream();
            }

            foreach (var ghoul in Role.GetRoles<Ghoul>(RoleEnum.Ghoul))
            {
                if (ghoul.Player.Data.Disconnected)
                    continue;

                var caught = ghoul.Caught;

                if (!caught)
                    ghoul.Fade();
                else if (ghoul.Faded)
                {
                    Utils.DefaultOutfit(ghoul.Player);
                    ghoul.Player.MyRend().color = Color.white;
                    ghoul.Player.gameObject.layer = LayerMask.NameToLayer("Ghost");
                    ghoul.Faded = false;
                    ghoul.Player.MyPhysics.ResetMoveState();
                }
            }

            foreach (var haunter in Role.GetRoles<Revealer>(RoleEnum.Revealer))
            {
                if (haunter.Player.Data.Disconnected)
                    return;

                var caught = haunter.Caught;

                if (!caught)
                    haunter.Fade();
                else if (haunter.Faded)
                {
                    Utils.DefaultOutfit(haunter.Player);
                    haunter.Player.MyRend().color = Color.white;
                    haunter.Player.gameObject.layer = LayerMask.NameToLayer("Ghost");
                    haunter.Faded = false;
                    haunter.Player.MyPhysics.ResetMoveState();
                }
            }

            foreach (var concealer in Role.GetRoles<Concealer>(RoleEnum.Concealer))
            {
                if (concealer.Concealed)
                    concealer.Conceal();
                else if (concealer.Enabled)
                    concealer.UnConceal();
            }

            foreach (var crus in Role.GetRoles<Crusader>(RoleEnum.Crusader))
            {
                if (crus.OnCrusade)
                    crus.Crusade();
                else if (crus.Enabled)
                    crus.UnCrusade();
            }

            foreach (var pois in Role.GetRoles<Poisoner>(RoleEnum.Poisoner))
            {
                if (pois.Poisoned)
                    pois.Poison();
                else if (pois.Enabled)
                    pois.PoisonKill();
            }

            foreach (var reb in Role.GetRoles<PromotedRebel>(RoleEnum.PromotedRebel))
            {
                if (reb.FormerRole == null || reb.FormerRole?.RoleType == RoleEnum.Anarchist)
                    continue;

                if (reb.OnEffect)
                {
                    if (reb.IsConc)
                        reb.Conceal();
                    else if (reb.IsPois)
                        reb.Poison();
                    else if (reb.IsDrunk)
                        reb.Confuse();
                    else if (reb.IsSS)
                        reb.Shapeshift();
                }
                else if (reb.Enabled)
                {
                    if (reb.IsConc)
                        reb.UnConceal();
                    else if (reb.IsPois)
                        reb.PoisonKill();
                    else if (reb.IsDrunk)
                        reb.Unconfuse();
                    else if (reb.IsSS)
                        reb.UnShapeshift();
                }
            }

            foreach (var ss in Role.GetRoles<Shapeshifter>(RoleEnum.Shapeshifter))
            {
                if (ss.Shapeshifted)
                    ss.Shapeshift();
                else if (ss.Enabled)
                    ss.UnShapeshift();
            }

            if (CustomGameOptions.ColourblindComms)
            {
                if (ShipStatus.Instance)
                {
                    switch (GameOptionsManager.Instance.currentNormalGameOptions.MapId)
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
                                Utils.Camouflage();
                                return;
                            }

                            break;

                        case 1:
                            var comms2 = ShipStatus.Instance.Systems[SystemTypes.Comms]?.Cast<HqHudSystemType>();

                            if (comms2.IsActive)
                            {
                                CommsEnabled = true;
                                Utils.Camouflage();
                                return;
                            }

                            break;
                    }
                }

                if (CommsEnabled)
                {
                    CommsEnabled = false;
                    CamouflagerEnabled = false;
                    Utils.DefaultOutfitAll();
                }
            }
        }
    }
}