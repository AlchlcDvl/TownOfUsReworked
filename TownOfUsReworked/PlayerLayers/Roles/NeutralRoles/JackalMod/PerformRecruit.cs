using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using Reactor.Utilities;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.JackalMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformRecruit
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Jackal))
                return false;

            var role = Role.GetRole<Jackal>(PlayerControl.LocalPlayer);

            if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                return false;

            if (__instance == role.RecruitButton)
            {
                if (!Utils.ButtonUsable(__instance))
                    return false;

                if (role.RecruitTimer() != 0f)
                    return false;
                
                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence), !role.ClosestPlayer.Is(SubFaction.None),
                    role.ClosestPlayer.Is(SubFaction.None));

                if (interact[3] == true)
                {
                    Recruit(role, role.ClosestPlayer);
                    var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetBackupRecruit, SendOption.Reliable, -1);
                    writer2.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer2.Write(role.ClosestPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer2);
                    return false;
                }
                else if (interact[0] == true)
                    role.LastRecruited = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastRecruited.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            return false;
        }

        public static void Recruit(Jackal jackRole, PlayerControl other)
        {
            var role = Role.GetRole(other);
            var jack = jackRole.Player;

            var convert = other.Is(SubFaction.None);

            if (convert)
            {
                jackRole.BackupRecruit = other;
                role.SubFaction = SubFaction.Cabal;
                role.IsRecruit = true;
                jackRole.Recruited.Add(other.PlayerId);
            }
            else if (other.IsRecruit())
                jackRole.Recruited.Add(other.PlayerId);
            else if (other.Is(RoleEnum.Jackal))
            {
                var jackal = (Jackal)role;
                jackRole.Recruited.AddRange(jackal.Recruited);
                jackal.Recruited.AddRange(jackRole.Recruited);
            }
            else if (!other.Is(SubFaction.None))
                Utils.RpcMurderPlayer(jack, other);

            jackRole.HasRecruited = true;
            jackRole.RecruitButton.gameObject.SetActive(false);

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic))
                Coroutines.Start(Utils.FlashCoroutine(jackRole.Color));
        }
    }
}
