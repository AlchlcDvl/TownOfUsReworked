using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Linq;
using TownOfUs.CrewmateRoles.MedicMod;
using System;
using Hazel;

namespace TownOfUs.CrewmateRoles.MediumMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Medium)) return true;
            var role = Role.GetRole<Medium>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!__instance.enabled) return false;
            if (role.MediateTimer() != 0f) return false;

            role.LastMediated = DateTime.UtcNow;

            List<DeadPlayer> PlayersDead = Murder.KilledPlayers.GetRange(0, Murder.KilledPlayers.Count);
            if (CustomGameOptions.DeadRevealed == DeadRevealed.Newest) PlayersDead.Reverse();
            foreach (var dead in Murder.KilledPlayers)
            {
                if (Object.FindObjectsOfType<DeadBody>().Any(x => x.ParentId == dead.PlayerId && !role.MediatedPlayers.Keys.Contains(x.ParentId)))
                {
                    role.AddMediatePlayer(dead.PlayerId);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte) CustomRPC.Mediate, SendOption.Reliable, -1);
                    writer.Write(dead.PlayerId);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    if (CustomGameOptions.DeadRevealed != DeadRevealed.All) return false;
                }
            }
            try {
                AudioClip MediumSFX = TownOfUs.loadAudioClipFromResources("TownOfUs.Resources.Seer.raw");
                SoundManager.Instance.PlaySound(MediumSFX, false, 0.4f);
            } catch {
            }

            return false;
        }
    }
}