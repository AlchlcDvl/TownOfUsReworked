using HarmonyLib;
using Reactor.Utilities;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class StopKill
    {
        public static void BreakShield(byte medicId, byte playerId, bool flag)
        {
            if (PlayerControl.LocalPlayer.PlayerId == playerId && CustomGameOptions.NotificationShield == NotificationOptions.Shielded)
                Coroutines.Start(Utils.FlashCoroutine(new Color(0f, 0.5f, 0f, 1f)));
                
                try
                {
                    AudioClip AttemptSFX = TownOfUsReworked.loadAudioClipFromResources("TownOfUsReworked.Resources.Attempt.raw");
                    SoundManager.Instance.PlaySound(AttemptSFX, false, 0.2f);
                } catch {}

            if (PlayerControl.LocalPlayer.PlayerId == medicId && CustomGameOptions.NotificationShield == NotificationOptions.Medic)
                Coroutines.Start(Utils.FlashCoroutine(new Color(0f, 0.5f, 0f, 1f)));

                try
                {
                    AudioClip AttemptSFX = TownOfUsReworked.loadAudioClipFromResources("TownOfUsReworked.Resources.Attempt.raw");
                    SoundManager.Instance.PlaySound(AttemptSFX, false, 0.2f);
                } catch {}

            if (CustomGameOptions.NotificationShield == NotificationOptions.Everyone)
                Coroutines.Start(Utils.FlashCoroutine(new Color(0f, 0.5f, 0f, 1f)));

                try
                {
                    AudioClip AttemptSFX = TownOfUsReworked.loadAudioClipFromResources("TownOfUsReworked.Resources.Attempt.raw");
                    SoundManager.Instance.PlaySound(AttemptSFX, false, 0.2f);
                } catch {}

            if (!flag)
                return;

            var player = Utils.PlayerById(playerId);

            foreach (var role in Role.GetRoles(RoleEnum.Medic))
            {
                if (((Medic) role).ShieldedPlayer.PlayerId == playerId)
                {
                    ((Medic) role).ShieldedPlayer = null;
                    ((Medic) role).exShielded = player;
                    System.Console.WriteLine(player.name + " Is Ex-Shielded");
                }
            }

            player.myRend().material.SetColor("_VisorColor", Palette.VisorColor);
            player.myRend().material.SetFloat("_Outline", 0f);
        }
    }
}