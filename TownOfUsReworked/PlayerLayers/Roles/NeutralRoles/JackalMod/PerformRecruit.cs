using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;

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
                if (!__instance.isActiveAndEnabled)
                    return false;

                if (role.RecruitTimer() != 0f)
                    return false;
                
                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence), false, true);

                if (interact[3] == true)
                {
                    role.BackupRecruit = role.ClosestPlayer;
                    role.HasRecruited = true;
                    var targetRole = Role.GetRole(role.ClosestPlayer);
                    targetRole.SubFaction = SubFaction.Cabal;
                    targetRole.IsRecruit = true;
                    targetRole.Faction = Faction.Neutral;
                    role.RecruitButton.gameObject.SetActive(false);

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
    }
}
