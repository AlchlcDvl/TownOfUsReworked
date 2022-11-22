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
            var mimicText = new GameObject("_Player").AddComponent<ImportantTextTask>();
            mimicText.transform.SetParent(PlayerControl.LocalPlayer.transform, false);
            mimicText.Text = $"{__instance.ColorString}Mimicking {mimicPlayer.Data.PlayerName} ({CustomGameOptions.MimicDuration}s)</color>";
            PlayerControl.LocalPlayer.myTasks.Insert(0, mimicText);

            while (true)
            {
                __instance.MimicTarget = mimicPlayer;
                var totalMimickTime = (DateTime.UtcNow - mimicActivation).TotalMilliseconds / 1000;

                if (__instance.Player.Data.IsDead)
                    totalMimickTime = CustomGameOptions.MimicDuration;

                mimicText.Text = $"{__instance.ColorString}Mimicking {mimicPlayer.Data.PlayerName}" +
                    $" ({CustomGameOptions.MimicDuration - Math.Round(totalMimickTime)}s)</color>";

                if (totalMimickTime > CustomGameOptions.MimicDuration | PlayerControl.LocalPlayer.Data.IsDead | AmongUsClient.Instance.GameState ==
                    InnerNetClient.GameStates.Ended)
                {
                    PlayerControl.LocalPlayer.myTasks.Remove(mimicText);
                    __instance.LastMimic = DateTime.UtcNow;
                    __instance.MimicTarget = null;
                    Utils.DefaultOutfit(__instance.Player);

                    SoundManager.Instance.PlaySound(TownOfUsReworked.GlitchWin, false, 0.4f);

                    yield break;
                }

                Utils.Morph(__instance.Player, mimicPlayer);
                __instance.GlitchButton.SetCoolDown(__instance.TimeRemaining, CustomGameOptions.MimicDuration);

                yield return null;
            }
        }
    }
}