using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.InvestigatorMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.OperativeMod;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using System.Linq;

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

            if (!Utils.ButtonUsable(__instance))
                return false;

            if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                return false;

            if (role.ConvertTimer() != 0f && __instance == role.BiteButton)
                return false;

            if (__instance == role.BiteButton)
            {
                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.VampireHunter), false, true, Role.GetRoleValue(RoleEnum.Pestilence));

                if (interact[3] == true && interact[0] == true)
                {
                    var vampCount = PlayerControl.AllPlayerControls.ToArray().ToList().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(SubFaction.Undead));

                    if (vampCount == CustomGameOptions.AliveVampCount)
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

            var convert = false;
            var convertNeut = false;
            var convertNK = false;
            var convertCK = false;
            var alreadyVamp = false;

            switch (role.RoleType)
            {
                case RoleEnum.Vampire:
                case RoleEnum.Dampyr:
                case RoleEnum.Dracula:

                    alreadyVamp = true;
                    break;

                case RoleEnum.Sheriff:
                case RoleEnum.Engineer:
                case RoleEnum.Mayor:
                case RoleEnum.Swapper:
                case RoleEnum.Investigator:
                case RoleEnum.TimeLord:
                case RoleEnum.Medic:
                case RoleEnum.Agent:
                case RoleEnum.Altruist:
                case RoleEnum.Crewmate:
                case RoleEnum.Tracker:
                case RoleEnum.Transporter:
                case RoleEnum.Medium:
                case RoleEnum.Coroner:
                case RoleEnum.Operative:
                case RoleEnum.Detective:
                case RoleEnum.Shifter:
                case RoleEnum.Inspector:
                case RoleEnum.Escort:

                    convert = true;
                    break;

                case RoleEnum.Vigilante:
                case RoleEnum.Veteran:

                    convertCK = true;
                    convert = true;
                    break;

                case RoleEnum.Amnesiac:
                case RoleEnum.Survivor:
                case RoleEnum.Jester:
                case RoleEnum.Cannibal:
                case RoleEnum.Executioner:
                case RoleEnum.Troll:

                    convertNeut = true;
                    break;

                case RoleEnum.Cryomaniac:
                case RoleEnum.Thief:
                case RoleEnum.Glitch:
                case RoleEnum.Plaguebearer:
                case RoleEnum.Werewolf:
                case RoleEnum.Murderer:
                case RoleEnum.SerialKiller:
                case RoleEnum.Arsonist:
                case RoleEnum.Guesser:

                    convertNK = true;
                    convertNeut = true;
                    break;
            }

            if (alreadyVamp)
            {
                dracRole.Converted.Add(other);
                dracRole.LastBitten = DateTime.UtcNow;

                if (role == dracRole)
                {
                    var drac2 = (Dracula)role;
                    dracRole.Converted.AddRange(drac2.Converted);
                    return;
                }
            }
            else if (convertNeut && CustomGameOptions.DraculaConvertNeuts)
            {
                Role newRole;
                dracRole.Converted.Add(other);
                
                if (!convertNK)
                {
                    if (role.RoleType == RoleEnum.Amnesiac)
                    {
                        var amne = Role.GetRole<Amnesiac>(other);
                        amne.BodyArrows.Values.DestroyAll();
                        amne.BodyArrows.Clear();
                        amne.CurrentTarget.bodyRenderer.material.SetFloat("_Outline", 0f);
                    }
                    else if (role.RoleType == RoleEnum.Cannibal)
                    {
                        var can = Role.GetRole<Cannibal>(other);
                        can.BodyArrows.Values.DestroyAll();
                        can.BodyArrows.Clear();
                        can.CurrentTarget.bodyRenderer.material.SetFloat("_Outline", 0f);
                    }

                    newRole = new Vampire(other);
                }
                else
                    newRole = new Dampyr(other);

                newRole.RoleHistory.Add(role);
                newRole.RoleHistory.AddRange(role.RoleHistory);
            }
            else if (convert)
            {
                Role newRole;
                dracRole.Converted.Add(other);

                if (!convertCK)
                {
                    if (role.RoleType == RoleEnum.Investigator)
                    {
                        var invRole = Role.GetRole<Investigator>(other);
                        Footprint.DestroyAll(invRole);
                    }
                    else if (role.RoleType == RoleEnum.Tracker)
                    {
                        var trackerRole = Role.GetRole<Tracker>(other);
                        trackerRole.TrackerArrows.Values.DestroyAll();
                        trackerRole.TrackerArrows.Clear();
                    }
                    else if (role.RoleType == RoleEnum.Coroner)
                    {
                        var coronerRole = Role.GetRole<Coroner>(other);
                        coronerRole.BodyArrows.Values.DestroyAll();
                        coronerRole.BodyArrows.Clear();
                    }
                    else if (role.RoleType == RoleEnum.Operative)
                    {
                        var opRole = Role.GetRole<Operative>(other);
                        opRole.BuggedPlayers.Clear();
                        opRole.Bugs.ClearBugs();
                    }
                    else if (role.RoleType == RoleEnum.Medic)
                    {
                        var medicRole = Role.GetRole<Medic>(other);
                        medicRole.ShieldedPlayer = null;
                    }

                    newRole = new Vampire(other);
                }
                else
                    newRole = new Dampyr(other);

                newRole.RoleHistory.Add(role);
                newRole.RoleHistory.AddRange(role.RoleHistory);
            }
            else if (!other.Is(SubFaction.Undead))
                Utils.RpcMurderPlayer(drac, other);

            dracRole.LastBitten = DateTime.UtcNow;

            foreach (var role2 in Role.GetRoles(RoleEnum.Dampyr))
            {
                var dampyr = (Dampyr)role2;
                dampyr.LastKilled = DateTime.UtcNow;

                if (dampyr.Player == PlayerControl.LocalPlayer)
                    dampyr.RegenTask();
            }

            foreach (var role2 in Role.GetRoles(RoleEnum.Vampire))
            {
                var vampire = (Vampire)role2;

                if (vampire.Player == PlayerControl.LocalPlayer)
                    vampire.RegenTask();
            }
        }
    }
}
