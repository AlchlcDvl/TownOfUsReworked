using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Linq;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.CoronerMod;
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
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Medium))
                return true;

            var role = Role.GetRole<Medium>(PlayerControl.LocalPlayer);

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            if (!__instance.enabled)
                return false;

            if (role.MediateTimer() != 0f)
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

                    unchecked
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.Mediate,
                            SendOption.Reliable, -1);
                        writer.Write(dead.PlayerId);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }

                    if (CustomGameOptions.DeadRevealed != DeadRevealed.All)
                        return false;
                }
            }
            
            //SoundManager.Instance.PlaySound(TownOfUsReworked.MediateSound, false, 0.4f);

            return false;
        }
    }
}