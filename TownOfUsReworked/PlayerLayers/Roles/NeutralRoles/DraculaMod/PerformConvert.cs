using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.OperativeMod;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Classes;
using System.Linq;
using Reactor.Utilities;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.DraculaMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformConvert
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Dracula))
                return false;

            var role = Role.GetRole<Dracula>(PlayerControl.LocalPlayer);

            if (__instance == role.BiteButton)
            {
                if (!Utils.ButtonUsable(__instance))
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (role.ConvertTimer() != 0f)
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.VampireHunter), false, true, Role.GetRoleValue(RoleEnum.Pestilence));

                if (interact[3] == true && interact[0] == true)
                {
                    if (role.Converted.Count >= CustomGameOptions.AliveVampCount)
                    {
                        Utils.RpcMurderPlayer(role.Player, role.ClosestPlayer);
                        role.LastBitten = DateTime.UtcNow;
                    }
                    else
                    {
                        var writer3 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                        writer3.Write((byte)ActionsRPC.Convert);
                        writer3.Write(PlayerControl.LocalPlayer.PlayerId);
                        writer3.Write(role.ClosestPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer3);
                        Convert(role, role.ClosestPlayer);
                        role.LastBitten = DateTime.UtcNow;
                    }

                    return false;
                }
                else if (interact[1] == true)
                    role.LastBitten.AddSeconds(CustomGameOptions.ProtectKCReset);
                else if (interact[2] == true)
                    role.LastBitten.AddSeconds(CustomGameOptions.VestKCReset);

                return false;
            }

            return false;
        }

        public static void Convert(Dracula dracRole, PlayerControl other)
        {
            var role = Role.GetRole(other);
            var drac = dracRole.Player;

            var convert = other.Is(SubFaction.None);

            if (convert)
            {
                role.SubFaction = SubFaction.Undead;
                role.IsBitten = true;
                dracRole.Converted.Add(other.PlayerId);
            }
            else if (other.IsBitten())
                dracRole.Converted.Add(other.PlayerId);
            else if (other.Is(RoleEnum.Dracula))
            {
                var drac2 = (Dracula)role;
                dracRole.Converted.AddRange(drac2.Converted);
                drac2.Converted.AddRange(dracRole.Converted);
            }
            else if (!other.Is(SubFaction.None))
                Utils.RpcMurderPlayer(drac, other);

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic))
                Coroutines.Start(Utils.FlashCoroutine(dracRole.Color));
        }
    }
}
