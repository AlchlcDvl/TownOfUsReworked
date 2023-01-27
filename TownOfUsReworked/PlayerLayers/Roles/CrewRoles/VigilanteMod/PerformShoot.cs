using System;
using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using Reactor.Utilities;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.VigilanteMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public static class PerformSHoot
    {
        private static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Vigilante, true))
                return false;

            var role = Role.GetRole<Vigilante>(PlayerControl.LocalPlayer);

            if (Utils.IsTooFar(PlayerControl.LocalPlayer, role.ClosestPlayer))
                return false;

            if (!Utils.ButtonUsable(__instance))
                return false;

            if (role.KillTimer() != 0f && __instance == role.ShootButton)
                return false;

            if (__instance == role.ShootButton)
            {
                var flag4 = role.ClosestPlayer.Is(Faction.Intruder) || role.ClosestPlayer.Is(RoleAlignment.NeutralKill) || role.ClosestPlayer.Is(Faction.Syndicate) ||
                    (role.ClosestPlayer.Is(RoleEnum.Jester) && CustomGameOptions.VigiKillsJester) || (role.ClosestPlayer.Is(RoleEnum.Executioner) && CustomGameOptions.VigiKillsExecutioner) ||
                    (role.ClosestPlayer.Is(RoleEnum.Cannibal) && CustomGameOptions.VigiKillsCannibal) || (role.ClosestPlayer.Is(RoleAlignment.NeutralBen) && CustomGameOptions.VigiKillsNB) ||
                    role.ClosestPlayer.Is(RoleAlignment.NeutralNeo) || role.ClosestPlayer.Is(RoleAlignment.NeutralPros) || role.ClosestPlayer.IsRecruit() || role.ClosestPlayer.IsFramed() ||
                    PlayerControl.LocalPlayer.IsTurnedTraitor() || PlayerControl.LocalPlayer.Is(ObjectifierEnum.Corrupted) || role.ClosestPlayer.Is(RoleEnum.Troll) ||
                    role.ClosestPlayer.IsTurnedTraitor() || role.ClosestPlayer.IsResurrected()/* || role.ClosestPlayer.IsTurnedFanatic()*/;
                var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, null, flag4);

                if (interact[3] == true && interact[0] == true)
                {                
                    if (flag4)
                    {
                        role.KilledInno = false;
                        return false;
                    }
                    else
                    {
                        if (CustomGameOptions.MisfireKillsInno)
                            Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, role.ClosestPlayer);
                        
                        if (CustomGameOptions.VigiKnowsInno)
                        {
                            if (role.Player == PlayerControl.LocalPlayer && CustomGameOptions.VigiNotifOptions == VigiNotif.Flash && CustomGameOptions.VigiOptions != VigiOptions.Immediate)
                                Coroutines.Start(Utils.FlashCoroutine(role.Color));
                            else if (CustomGameOptions.VigiNotifOptions == VigiNotif.Message && CustomGameOptions.VigiOptions != VigiOptions.Immediate)
                                role.InnoMessage = true;
                        }

                        if (CustomGameOptions.VigiKillAgain)
                        {
                            role.LastKilled = DateTime.UtcNow;
                            role.KilledInno = false;
                        }
                        else
                            role.KilledInno = true;

                        if (CustomGameOptions.VigiOptions == VigiOptions.Immediate)
                            Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);
                        else if (CustomGameOptions.VigiOptions == VigiOptions.PreMeeting)
                            role.PreMeetingDie = true;
                        else if (CustomGameOptions.VigiOptions == VigiOptions.PostMeeting)
                            role.PostMeetingDie = true;
                    }

                    return false;
                }
                else if (interact[1] == true)
                    role.LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            return false;
        }
    }
}