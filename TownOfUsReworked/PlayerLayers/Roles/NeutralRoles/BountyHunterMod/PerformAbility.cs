using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using Reactor.Utilities;
using UnityEngine;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.BountyHunterMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static Sprite Guess => TownOfUsReworked.Placeholder;
        public static Sprite Hunt => TownOfUsReworked.WhisperSprite;

        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.BountyHunter))
                return false;

            var role = Role.GetRole<BountyHunter>(PlayerControl.LocalPlayer);

            if (__instance == role.GuessButton)
            {
                if (!Utils.ButtonUsable(__instance))
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (role.CheckTimer() != 0f)
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence), role.TargetFound && role.GuessButton.graphic.sprite == Hunt);

                if (interact[3] == true)
                {
                    if (role.GuessButton.graphic.sprite == Guess)
                    {
                        if (role.ClosestPlayer != role.TargetPlayer)
                        {
                            Coroutines.Start(Utils.FlashCoroutine(Color.red));
                            role.UsesLeft--;
                        }
                        else
                        {
                            role.TargetFound = true;
                            Coroutines.Start(Utils.FlashCoroutine(Color.green));
                        }
                    }
                }

                if (interact[0] == true)
                    role.LastChecked = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastChecked.AddSeconds(CustomGameOptions.ProtectKCReset);
                else if (interact[2] == true)
                    role.LastChecked.AddSeconds(CustomGameOptions.VestKCReset);

                if (role.GuessButton.graphic.sprite == Hunt && role.TargetFound)
                {
                    Utils.RpcMurderPlayer(role.Player, role.ClosestPlayer);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.BountyHunterWin);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.TargetKilled = true;
                    return false;
                }

                return false;
            }
            
            return false;
        }
    }
}