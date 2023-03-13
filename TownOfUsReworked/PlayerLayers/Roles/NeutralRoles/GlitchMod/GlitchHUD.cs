using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GlitchMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class GlitchHUD
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Glitch))
                return;

            var role = Role.GetRole<Glitch>(PlayerControl.LocalPlayer);

            if (role.KillButton == null)
                role.KillButton = Utils.InstantiateButton();

            if (role.HackButton == null)
                role.HackButton = Utils.InstantiateButton();

            if (role.MimicButton == null)
                role.MimicButton = Utils.InstantiateButton();

            role.MimicButton.UpdateButton(role, "MIMIC", role.MimicTimer(), CustomGameOptions.MimicCooldown, TownOfUsReworked.MimicSprite, AbilityTypes.Effect, role.IsUsingMimic,
                role.TimeRemaining, CustomGameOptions.MimicDuration, true, !role.IsUsingMimic);
            role.HackButton.UpdateButton(role, "HACK", role.HackTimer(), CustomGameOptions.HackCooldown, TownOfUsReworked.Placeholder, AbilityTypes.Direct, role.IsUsingHack,
                role.TimeRemaining, CustomGameOptions.HackDuration, true, !role.IsUsingHack);
            role.KillButton.UpdateButton(role, "NEUTRALISE", role.KillTimer(), CustomGameOptions.GlitchKillCooldown, TownOfUsReworked.EraseDataSprite, AbilityTypes.Direct);

            if (role.MimicList != null)
            {
                if (Minigame.Instance)
                    Minigame.Instance.Close();

                if (!role.MimicList.IsOpen || MeetingHud.Instance || PlayerControl.LocalPlayer.Data.IsDead)
                {
                    role.MimicList.Toggle();
                    role.MimicList.SetVisible(false);
                    role.MimicList = null;
                }
                else
                {
                    foreach (var bubble in role.MimicList.chatBubPool.activeChildren)
                    {
                        if (!role.IsUsingMimic && role.MimicList != null)
                        {
                            Vector2 ScreenMin = Camera.main.WorldToScreenPoint(bubble.Cast<ChatBubble>().Background.bounds.min);
                            Vector2 ScreenMax = Camera.main.WorldToScreenPoint(bubble.Cast<ChatBubble>().Background.bounds.max);

                            if (Input.mousePosition.x > ScreenMin.x && Input.mousePosition.x < ScreenMax.x)
                            {
                                if (Input.mousePosition.y > ScreenMin.y && Input.mousePosition.y < ScreenMax.y)
                                {
                                    if (!Input.GetMouseButtonDown(0) && role.LastMouse)
                                    {
                                        role.LastMouse = false;
                                        role.MimicList.Toggle();
                                        role.MimicList.SetVisible(false);
                                        role.MimicList = null;
                                        role.MimicTarget = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.Data.PlayerName == bubble.Cast<ChatBubble>().NameText.text);
                                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                                        writer.Write((byte)ActionsRPC.SetMimic);
                                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                                        writer.Write(role.ClosestPlayer.PlayerId);
                                        role.Mimic();
                                        break;
                                    }
                                    
                                    role.LastMouse = Input.GetMouseButtonDown(0);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}