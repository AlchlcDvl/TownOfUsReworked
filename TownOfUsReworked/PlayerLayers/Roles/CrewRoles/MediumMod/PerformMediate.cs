using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Lobby.CustomOption;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Linq;
using System;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using Random = UnityEngine.Random;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MediumMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformMediate
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Medium))
                return false;

            var role = Role.GetRole<Medium>(PlayerControl.LocalPlayer);

            if (__instance == role.MediateButton)
            {
                if (!__instance.isActiveAndEnabled)
                    return false;

                if (role.MediateTimer() > 0f)
                    return false;

                role.LastMediated = DateTime.UtcNow;
                List<DeadPlayer> PlayersDead = Murder.KilledPlayers.GetRange(0, Murder.KilledPlayers.Count);

                if (CustomGameOptions.DeadRevealed == DeadRevealed.Newest)
                    PlayersDead.Reverse();

                if (CustomGameOptions.DeadRevealed != DeadRevealed.Random)
                {
                    foreach (var dead in PlayersDead)
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
                }
                else
                {
                    PlayersDead.Shuffle();
                    var dead = PlayersDead[Random.RandomRangeInt(0, PlayersDead.Count)];

                    if (Object.FindObjectsOfType<DeadBody>().Any(x => x.ParentId == dead.PlayerId && !role.MediatedPlayers.Keys.Contains(x.ParentId)))
                    {
                        role.AddMediatePlayer(dead.PlayerId);
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                        writer.Write((byte)ActionsRPC.Mediate);
                        writer.Write(dead.PlayerId);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                }

                return false;
            }
            
            //SoundManager.Instance.PlaySound(TownOfUsReworked.MediateSound, false, 0.4f);
            return false;
        }
    }
}