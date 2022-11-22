﻿using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Lobby.Extras.RainbowMod;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.CamouflagerMod;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TrackerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static Sprite Sprite => TownOfUsReworked.Arrow;
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton)
                return true;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Tracker))
                return true;

            var role = Role.GetRole<Tracker>(PlayerControl.LocalPlayer);

            if (!PlayerControl.LocalPlayer.CanMove | role.ClosestPlayer == null)
                return false;

            var flag2 = role.TrackerTimer() == 0f;

            if (!flag2)
                return false;

            if (!__instance.enabled)
                return false;

            var maxDistance = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];

            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(), PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance)
                return false;

            if (role.ClosestPlayer == null)
                return false;

            var target = role.ClosestPlayer;

            if (!role.ButtonUsable)
                return false;

            if (role.ClosestPlayer.IsInfected() | role.Player.IsInfected())
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer))
                    ((Plaguebearer)pb).RpcSpreadInfection(role.ClosestPlayer, role.Player);
            }

            if (role.ClosestPlayer.IsOnAlert() | role.ClosestPlayer.Is(RoleEnum.Pestilence))
            {
                if (role.Player.IsShielded())
                {
                    var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                    writer2.Write(PlayerControl.LocalPlayer.GetMedic().Player.PlayerId);
                    writer2.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer2);

                    System.Console.WriteLine(CustomGameOptions.ShieldBreaks + "- shield break");

                    if (CustomGameOptions.ShieldBreaks)
                        role.LastTracked = DateTime.UtcNow;

                    StopKill.BreakShield(PlayerControl.LocalPlayer.GetMedic().Player.PlayerId, PlayerControl.LocalPlayer.PlayerId, CustomGameOptions.ShieldBreaks);
                    return false;
                }
                else if (!role.Player.IsProtected())
                {
                    Utils.RpcMurderPlayer(role.ClosestPlayer, PlayerControl.LocalPlayer);
                    return false;
                }

                role.LastTracked = DateTime.UtcNow;
                return false;
            }

            var gameObj = new GameObject();
            var arrow = gameObj.AddComponent<ArrowBehaviour>();
            gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
            var renderer = gameObj.AddComponent<SpriteRenderer>();
            renderer.sprite = Sprite;

            if (!CamouflageUnCamouflage.IsCamoed)
            {
                if (RainbowUtils.IsRainbow(target.GetDefaultOutfit().ColorId))
                    renderer.color = RainbowUtils.Rainbow;
                else
                    renderer.color = Palette.PlayerColors[target.GetDefaultOutfit().ColorId];
            }
            else
                renderer.color = Color.gray;

            arrow.image = renderer;
            gameObj.layer = 5;
            arrow.target = target.transform.position;
            role.TrackerArrows.Add(target.PlayerId, arrow);
            role.UsesLeft--;
            role.LastTracked = DateTime.UtcNow;
            SoundManager.Instance.PlaySound(TownOfUsReworked.TrackSound, false, 0.4f);
            
            return false;
        }
    }
}
