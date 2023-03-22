using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.ShifterMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class PerformShift
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Shifter))
                return true;

            var role = Role.GetRole<Shifter>(PlayerControl.LocalPlayer);

            if (__instance == role.ShiftButton)
            {
                if (!Utils.ButtonUsable(role.ShiftButton))
                    return false;

                if (role.ShiftTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer);

                if (interact[3] == true)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.Shift);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.ClosestPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Shift(role, role.ClosestPlayer);
                }

                if (interact[0] == true)
                    role.LastShifted = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastShifted.AddSeconds(CustomGameOptions.ProtectKCReset);
                
                return false;
            }

            return true;
        }

        public static void Shift(Shifter shifterRole, PlayerControl other)
        {
            var role = Role.GetRole(other);
            var roleType = role.RoleType;
            var shifter = shifterRole.Player;
            Role newRole;

            if (!other.Is(Faction.Crew) || other.IsFramed())
            {
                Utils.RpcMurderPlayer(shifter, shifter, false);
                return;
            }

            switch (roleType)
            {
                case RoleEnum.Agent:
                    newRole = new Agent(shifter);
                    break;

                case RoleEnum.Altruist:
                    newRole = new Altruist(shifter);
                    break;

                case RoleEnum.Coroner:
                    newRole = new Coroner(shifter);
                    break;

                case RoleEnum.Crewmate:
                    newRole = new Crewmate(shifter);
                    break;

                case RoleEnum.Detective:
                    newRole = new Detective(shifter);
                    break;

                case RoleEnum.Engineer:
                    newRole = new Engineer(shifter);
                    break;

                case RoleEnum.Escort:
                    newRole = new Escort(shifter);
                    break;

                case RoleEnum.Inspector:
                    newRole = new Inspector(shifter);
                    break;

                case RoleEnum.Sheriff:
                    newRole = new Sheriff(shifter);
                    break;

                case RoleEnum.Mayor:
                    newRole = new Mayor(shifter);
                    break;

                case RoleEnum.Swapper:
                    newRole = new Swapper(shifter);
                    break;

                case RoleEnum.Medic:
                    newRole = new Medic(shifter);
                    break;

                case RoleEnum.Tracker:
                    newRole = new Tracker(shifter);
                    break;

                case RoleEnum.Transporter:
                    newRole = new Transporter(shifter);
                    break;

                case RoleEnum.Medium:
                    newRole = new Medium(shifter);
                    break;

                case RoleEnum.Operative:
                    newRole = new Operative(shifter);
                    break;

                case RoleEnum.TimeLord:
                    newRole = new TimeLord(shifter);
                    break;

                case RoleEnum.VampireHunter:
                    newRole = new VampireHunter(shifter);
                    break;

                case RoleEnum.Veteran:
                    newRole = new Veteran(shifter);
                    break;

                case RoleEnum.Vigilante:
                    newRole = new Vigilante(shifter);
                    break;

                case RoleEnum.Mystic:
                    newRole = new Mystic(shifter);
                    break;

                case RoleEnum.Seer:
                    newRole = new Seer(shifter);
                    break;

                case RoleEnum.Chameleon:
                    newRole = new Chameleon(shifter);
                    break;

                case RoleEnum.Retributionist:
                    newRole = new Retributionist(shifter);
                    break;

                default:
                    newRole = new Shifter(shifter);
                    break;
            }

            newRole.RoleHistory.Add(shifterRole);
            newRole.RoleHistory.AddRange(shifterRole.RoleHistory);
            shifter.RegenTask();
            Role newRole2;

            if (CustomGameOptions.ShiftedBecomes == BecomeEnum.Shifter)
                newRole2 = new Shifter(other);
            else
                newRole2 = new Crewmate(other);

            newRole2.RoleHistory.Add(role);
            newRole2.RoleHistory.AddRange(role.RoleHistory);
            newRole2.IsRecruit = role.IsRecruit;
            newRole2.IsBitten = role.IsBitten;
            newRole2.IsPersuaded = role.IsPersuaded;
            newRole2.IsResurrected = role.IsResurrected;
            newRole2.IsIntTraitor = role.IsIntTraitor;
            newRole2.IsSynFanatic = role.IsSynFanatic;
            newRole2.IsIntFanatic = role.IsIntFanatic;
            newRole2.IsSynTraitor = role.IsSynTraitor;

            other.RegenTask();
        }
    }
}
