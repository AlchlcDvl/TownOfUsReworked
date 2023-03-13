using System;
using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using Reactor.Utilities;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.VigilanteMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformSHoot
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Vigilante))
                return true;

            var role = Role.GetRole<Vigilante>(PlayerControl.LocalPlayer);

            if (role.IsBlocked)
                return false;

            if (__instance == role.ShootButton)
            {
                if (!Utils.ButtonUsable(role.ShootButton))
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (role.KillTimer() != 0f)
                    return false;

                var flag4 = role.ClosestPlayer.Is(Faction.Intruder) || role.ClosestPlayer.Is(RoleAlignment.NeutralKill) || role.ClosestPlayer.Is(Faction.Syndicate) ||
                    (role.ClosestPlayer.Is(RoleEnum.Jester) && CustomGameOptions.VigiKillsJester) || (role.ClosestPlayer.Is(RoleEnum.Executioner) && CustomGameOptions.VigiKillsExecutioner) ||
                    (role.ClosestPlayer.Is(RoleEnum.Cannibal) && CustomGameOptions.VigiKillsCannibal) || (role.ClosestPlayer.Is(RoleAlignment.NeutralBen) && CustomGameOptions.VigiKillsNB) ||
                    role.ClosestPlayer.Is(RoleAlignment.NeutralNeo) || role.ClosestPlayer.Is(RoleAlignment.NeutralPros) || role.ClosestPlayer.IsFramed() ||
                    role.Player.Is(ObjectifierEnum.Corrupted) || role.ClosestPlayer.Is(RoleEnum.Troll) || role.Player.NotOnTheSameSide() || role.ClosestPlayer.NotOnTheSameSide() ||
                    (role.ClosestPlayer.Is(RoleEnum.Actor) && CustomGameOptions.VigiKillsActor) || (role.ClosestPlayer.Is(RoleEnum.BountyHunter) && CustomGameOptions.VigiKillsBH);
                var interact = Utils.Interact(role.Player, role.ClosestPlayer, flag4);

                if (interact[3] == true)
                {                
                    if (flag4 && !role.Player.IsFramed())
                    {
                        role.KilledInno = false;
                        role.LastKilled = DateTime.UtcNow;
                        return false;
                    }
                    else
                    {
                        if (CustomGameOptions.MisfireKillsInno || role.ClosestPlayer.IsFramed())
                            Utils.RpcMurderPlayer(role.Player, role.ClosestPlayer, !role.Player.Is(AbilityEnum.Ninja));
                        
                        if (role.Player == PlayerControl.LocalPlayer && CustomGameOptions.VigiNotifOptions == VigiNotif.Flash && CustomGameOptions.VigiOptions != VigiOptions.Immediate)
                            Coroutines.Start(Utils.FlashCoroutine(role.Color));
                        else if (CustomGameOptions.VigiNotifOptions == VigiNotif.Message && CustomGameOptions.VigiOptions != VigiOptions.Immediate)
                            role.InnoMessage = true;

                        if (CustomGameOptions.VigiKillAgain)
                        {
                            role.LastKilled = DateTime.UtcNow;
                            role.KilledInno = false;
                        }
                        else
                            role.KilledInno = true;

                        if (CustomGameOptions.VigiOptions == VigiOptions.Immediate)
                            Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer, false);
                        else if (CustomGameOptions.VigiOptions == VigiOptions.PreMeeting)
                            role.PreMeetingDie = true;
                        else if (CustomGameOptions.VigiOptions == VigiOptions.PostMeeting)
                            role.PostMeetingDie = true;
                    }
                }
                else if (interact[0] == true)
                    role.LastKilled = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            return true;
        }
    }
}