using HarmonyLib;
using Hazel;
using System.Collections.Generic;
using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using Reactor.Utilities;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.WhispererMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class PerformWhisper
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Whisperer))
                return true;

            var role = Role.GetRole<Whisperer>(PlayerControl.LocalPlayer);

            if (__instance == role.WhisperButton)
            {
                if (role.WhisperTimer() != 0f)
                    return false;

                role.LastWhispered = DateTime.UtcNow;
                Whisper(role);
                return false;
            }

            return true;
        }

        public static void Whisper(Whisperer role)
        {
            var truePosition = role.Player.GetTruePosition();
            var closestPlayers = Utils.GetClosestPlayers(truePosition, CustomGameOptions.WhisperRadius);
            closestPlayers.Remove(role.Player);

            if (role.PlayerConversion.Count == 0)
                role.PlayerConversion = role.GetPlayers();

            var oldStats = role.PlayerConversion;
            role.PlayerConversion = new List<(byte, int)>();

            foreach (var conversionRate in oldStats)
            {
                var player = conversionRate.Item1;
                var stats = conversionRate.Item2;

                if (closestPlayers.Contains(Utils.PlayerById(player)))
                    stats -= (int)role.WhisperConversion;

                if (!Utils.PlayerById(player).Data.IsDead)
                    role.PlayerConversion.Add((player, stats));
            }

            role.WhisperCount++;
            var removals = new List<(byte, int)>();

            foreach (var playerConversion in role.PlayerConversion)
            {
                if (playerConversion.Item2 <= 0)
                {
                    role.ConversionCount++;

                    if (CustomGameOptions.WhisperRateDecreases)
                        role.WhisperConversion -= CustomGameOptions.WhisperRateDecrease;

                    if (role.WhisperConversion < 2.5f)
                        role.WhisperConversion = 2.5f;

                    if (Utils.PlayerById(playerConversion.Item1).Is(SubFaction.None))
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                        writer.Write((byte)ActionsRPC.WhisperConvert);
                        writer.Write(playerConversion.Item1);
                        writer.Write(role.Player.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        removals.Add(playerConversion);
                        var targetRole = Role.GetRole(Utils.PlayerById(playerConversion.Item1));
                        targetRole.SubFaction = SubFaction.Sect;
                        targetRole.IsPersuaded = true;
                        role.Persuaded.Add(playerConversion.Item1);

                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic))
                            Coroutines.Start(Utils.FlashCoroutine(role.Color));
                    }
                    else if (!Utils.PlayerById(playerConversion.Item1).Is(SubFaction.Sect))
                        Utils.RpcMurderPlayer(role.Player, Utils.PlayerById(playerConversion.Item1), false);
                    else  if (Utils.PlayerById(playerConversion.Item1).Is(SubFaction.Sect))
                    {
                        if (Utils.PlayerById(playerConversion.Item1).Is(RoleEnum.Whisperer))
                        {
                            var targetRole = Role.GetRole<Whisperer>(Utils.PlayerById(playerConversion.Item1));
                            role.Persuaded.AddRange(targetRole.Persuaded);
                            targetRole.Persuaded.AddRange(role.Persuaded);
                        }
                        else
                            role.Persuaded.Add(playerConversion.Item1);

                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                        writer.Write((byte)ActionsRPC.AddConversions);
                        writer.Write(role.Player.PlayerId);
                        writer.Write(playerConversion.Item1);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                }
            }
            
            foreach (var removal in removals)
                role.PlayerConversion.Remove(removal);
        }
    }
}