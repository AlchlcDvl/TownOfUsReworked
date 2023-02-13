using HarmonyLib;
using Hazel;
using UnityEngine;
using System.Collections.Generic;
using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Lobby.CustomOption;
using Reactor.Utilities;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.WhispererMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformWhisper
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Whisperer))
                return false;

            var role = Role.GetRole<Whisperer>(PlayerControl.LocalPlayer);

            if (__instance == role.WhisperButton)
            {
                if (!Utils.ButtonUsable(__instance))
                    return false;

                if (role.WhisperTimer() != 0f)
                    return false;

                role.LastWhispered = DateTime.UtcNow;
                Whisper(role);
                return false;
            }

            return false;
        }

        public static void Whisper(Whisperer role)
        {
            Vector2 truePosition = role.Player.GetTruePosition();
            var closestPlayers = Utils.GetClosestPlayers(truePosition, CustomGameOptions.WhisperRadius);
            closestPlayers.Remove(role.Player);

            if (role.PlayerConversion.Count == 0)
                role.PlayerConversion = role.GetPlayers();

            var oldStats = role.PlayerConversion;
            role.PlayerConversion = new List<(PlayerControl, int)>();

            foreach (var conversionRate in oldStats)
            {
                var player = conversionRate.Item1;
                var stats = conversionRate.Item2;

                if (closestPlayers.Contains(player))
                    stats -= (int)role.WhisperConversion;

                if (!player.Data.IsDead)
                    role.PlayerConversion.Add((player, stats));
            }

            role.WhisperCount++;
            var removals = new List<(PlayerControl, int)>();

            foreach (var playerConversion in role.PlayerConversion)
            {
                if (playerConversion.Item2 <= 0)
                {
                    role.ConversionCount++;

                    if (CustomGameOptions.WhisperRateDecreases)
                        role.WhisperConversion -= CustomGameOptions.WhisperRateDecrease;

                    if (role.WhisperConversion < 2.5f)
                        role.WhisperConversion = 2.5f;

                    if (playerConversion.Item1.Is(SubFaction.None))
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                        writer.Write((byte)ActionsRPC.WhisperConvert);
                        writer.Write(playerConversion.Item1.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        removals.Add(playerConversion);
                        var targetRole = Role.GetRole(playerConversion.Item1);
                        targetRole.SubFaction = SubFaction.Sect;
                        targetRole.IsPersuaded = true;
                        role.Persuaded.Add(playerConversion.Item1.PlayerId);

                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic))
                            Coroutines.Start(Utils.FlashCoroutine(role.Color));
                    }
                    else if (!playerConversion.Item1.Is(SubFaction.Sect))
                        Utils.RpcMurderPlayer(role.Player, playerConversion.Item1);
                }
            }
            
            foreach (var removal in removals)
                role.PlayerConversion.Remove(removal);
        }
    }
}