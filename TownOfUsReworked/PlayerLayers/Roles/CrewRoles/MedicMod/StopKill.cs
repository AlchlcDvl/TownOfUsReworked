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
            var role = Role.GetRole<Medic>(Utils.PlayerById(medicId));

            if (PlayerControl.LocalPlayer.PlayerId == playerId && CustomGameOptions.NotificationShield == NotificationOptions.Shielded)
                Coroutines.Start(Utils.FlashCoroutine(role.Color));

            if (PlayerControl.LocalPlayer.PlayerId == medicId && CustomGameOptions.NotificationShield == NotificationOptions.Medic)
                Coroutines.Start(Utils.FlashCoroutine(role.Color));
            
            if (CustomGameOptions.NotificationShield == NotificationOptions.Everyone)
                Coroutines.Start(Utils.FlashCoroutine(role.Color));

            try
            {
                SoundManager.Instance.PlaySound(TownOfUsReworked.AttemptSound, false, 1f);
            } catch {}

            if (!flag)
                return;

            var player = Utils.PlayerById(playerId);

            foreach (var role2 in Role.GetRoles(RoleEnum.Medic))
            {
                if (((Medic)role2).ShieldedPlayer.PlayerId == playerId)
                {
                    ((Medic)role2).ShieldedPlayer = null;
                    ((Medic)role2).exShielded = player;
                    System.Console.WriteLine(player.name + " Is Ex-Shielded");
                }
            }

            player.myRend().material.SetColor("_VisorColor", Palette.VisorColor);
            player.myRend().material.SetFloat("_Outline", 0f);
        }
    }
}