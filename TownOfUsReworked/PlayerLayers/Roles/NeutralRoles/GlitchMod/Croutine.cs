using Hazel;
using InnerNet;
using System;
using System.Collections;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GlitchMod
{
    public static class Block
    {
        public static IEnumerator Mimic(Glitch __instance, PlayerControl mimicPlayer)
        {
            unchecked
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Mimic,
                    SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(mimicPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }

            Utils.Morph(__instance.Player, mimicPlayer);

            var mimicActivation = DateTime.UtcNow;

            while (true)
            {
                __instance.MimicTarget = mimicPlayer;
                var totalMimickTime = (DateTime.UtcNow - mimicActivation).TotalMilliseconds / 1000;

                if (__instance.Player.Data.IsDead)
                    totalMimickTime = CustomGameOptions.MimicDuration;

                if (totalMimickTime > CustomGameOptions.MimicDuration || PlayerControl.LocalPlayer.Data.IsDead || AmongUsClient.Instance.GameState ==
                    InnerNetClient.GameStates.Ended)
                {
                    __instance.LastMimic = DateTime.UtcNow;
                    __instance.MimicTarget = null;
                    Utils.DefaultOutfit(__instance.Player);
                    yield break;
                }

                Utils.Morph(__instance.Player, mimicPlayer);
                __instance.GlitchButton.SetCoolDown(__instance.TimeRemaining, CustomGameOptions.MimicDuration);

                yield return null;
            }
        }
    }
}