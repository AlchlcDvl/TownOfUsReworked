using HarmonyLib;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using Object = UnityEngine.Object;
using System.Linq;
using System;
using Hazel;
using Random = UnityEngine.Random;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MediumMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformMediate
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Medium))
                return true;

            var role = Role.GetRole<Medium>(PlayerControl.LocalPlayer);

            if (__instance == role.MediateButton)
            {
                if (role.MediateTimer() != 0f)
                    return false;

                role.LastMediated = DateTime.UtcNow;
                var PlayersDead = Murder.KilledPlayers.GetRange(0, Murder.KilledPlayers.Count);

                if (CustomGameOptions.DeadRevealed == DeadRevealed.Newest)
                    PlayersDead.Reverse();

                if (CustomGameOptions.DeadRevealed != DeadRevealed.Random)
                {
                    foreach (var dead in PlayersDead)
                    {
                        if (Object.FindObjectsOfType<DeadBody>().Any(x => x.ParentId == dead.PlayerId && !role.MediatedPlayers.ContainsKey(x.ParentId)))
                        {
                            role.AddMediatePlayer(dead.PlayerId);
                            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
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
                    var dead = PlayersDead[Random.RandomRangeInt(0, PlayersDead.Count - 1)];

                    if (Object.FindObjectsOfType<DeadBody>().Any(x => x.ParentId == dead.PlayerId && !role.MediatedPlayers.ContainsKey(x.ParentId)))
                    {
                        role.AddMediatePlayer(dead.PlayerId);
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                        writer.Write((byte)ActionsRPC.Mediate);
                        writer.Write(dead.PlayerId);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                }

                return false;
            }

            return true;
        }
    }
}