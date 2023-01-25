using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Linq;
using System;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MediumMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.CannotUseButton(PlayerControl.LocalPlayer, RoleEnum.Medium))
                return false;

            var role = Role.GetRole<Medium>(PlayerControl.LocalPlayer);

            if (Utils.CannotUseButton(PlayerControl.LocalPlayer, RoleEnum.Escort, null, __instance) || __instance != role.MediateButton)
                return false;

            if (role.MediateTimer() != 0f && __instance == role.MediateButton)
                return false;

            role.LastMediated = DateTime.UtcNow;
            List<DeadPlayer> PlayersDead = Murder.KilledPlayers.GetRange(0, Murder.KilledPlayers.Count);

            if (CustomGameOptions.DeadRevealed == DeadRevealed.Newest)
                PlayersDead.Reverse();

            foreach (var dead in Murder.KilledPlayers)
            {
                if (Object.FindObjectsOfType<DeadBody>().Any(x => x.ParentId == dead.PlayerId && !role.MediatedPlayers.Keys.Contains(x.ParentId)))
                {
                    role.AddMediatePlayer(dead.PlayerId);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                    writer.Write((byte)ActionsRPC.Mediate);
                    writer.Write(dead.PlayerId);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    if (CustomGameOptions.DeadRevealed != DeadRevealed.All)
                        return false;
                }
            }
            
            //SoundManager.Instance.PlaySound(TownOfUsReworked.MediateSound, false, 0.4f);
            return false;
        }
    }
}