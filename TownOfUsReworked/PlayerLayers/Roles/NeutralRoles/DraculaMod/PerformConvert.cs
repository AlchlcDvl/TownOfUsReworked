using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using Reactor.Utilities;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.DraculaMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformConvert
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Dracula))
                return true;

            var role = Role.GetRole<Dracula>(PlayerControl.LocalPlayer);

            if (__instance == role.BiteButton)
            {
                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (role.ConvertTimer() != 0f)
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, false, true);

                if (interact[3])
                {
                    var writer3 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer3.Write((byte)ActionsRPC.Convert);
                    writer3.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer3.Write(role.ClosestPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer3);
                    Convert(role, role.ClosestPlayer);
                }

                if (interact[0])
                    role.LastBitten = DateTime.UtcNow;
                else if (interact[1])
                    role.LastBitten.AddSeconds(CustomGameOptions.ProtectKCReset);
                else if (interact[2])
                    role.LastBitten.AddSeconds(CustomGameOptions.VestKCReset);

                return false;
            }

            return true;
        }

        public static void Convert(Dracula dracRole, PlayerControl other)
        {
            var role = Role.GetRole(other);
            var drac = dracRole.Player;

            if (dracRole.Converted.Count >= CustomGameOptions.AliveVampCount)
            {
                Utils.RpcMurderPlayer(drac, other);
                return;
            }

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
