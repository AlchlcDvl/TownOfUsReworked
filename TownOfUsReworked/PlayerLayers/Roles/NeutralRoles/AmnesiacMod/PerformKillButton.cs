using HarmonyLib;
using Hazel;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Abilities.SnitchMod;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;
using Il2CppSystem.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.AmnesiacMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKillButton
    {
        public static Sprite Sprite => TownOfUsReworked.Arrow;
        
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton || !PlayerControl.LocalPlayer.Is(RoleEnum.Amnesiac) || !PlayerControl.LocalPlayer.CanMove ||
                PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            var role = Role.GetRole<Amnesiac>(PlayerControl.LocalPlayer);

            if (__instance.isCoolingDown || !__instance.enabled || role == null || role.CurrentTarget == null)
                return false;

            var maxDistance = GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];

            if (Utils.GetDistBetweenPlayers(role.Player, Utils.PlayerById(role.CurrentTarget.ParentId)) > maxDistance)
                return false;

            var playerId = role.CurrentTarget.ParentId;
            var player = Utils.PlayerById(playerId);

            if (player.IsInfected() || role.Player.IsInfected())
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer))
                    ((Plaguebearer)pb).RpcSpreadInfection(player, role.Player);
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
            writer.Write((byte)ActionsRPC.Remember);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(playerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            
            try
            {
                SoundManager.Instance.PlaySound(TownOfUsReworked.RememberSound, false, 1f);
            } catch {}

            Remember(role, player);
            return false;
        }

        public static void Remember(Amnesiac amneRole, PlayerControl other)
        {
            var role = Role.GetRole(other);
            var roleType = role.RoleType;
            var ability = (Ability.GetAbility(other))?.AbilityType;
            var amnesiac = amneRole.Player;
            List<PlayerTask> tasks1, tasks2;
            List<GameData.TaskInfo> taskinfos1, taskinfos2;
            Role newRole;

            if (PlayerControl.LocalPlayer == amnesiac)
            {
                var amnesiacRole = Role.GetRole<Amnesiac>(amnesiac);
                amnesiacRole.BodyArrows.Values.DestroyAll();
                amnesiacRole.BodyArrows.Clear();
                amnesiacRole.CurrentTarget.bodyRenderer.material.SetFloat("_Outline", 0f);
            }

            switch (roleType)
            {
                case RoleEnum.Agent:
                    newRole = new Agent(amnesiac);
                    break;

                case RoleEnum.Altruist:
                    newRole = new Altruist(amnesiac);
                    break;
                    
                case RoleEnum.Amnesiac:
                    newRole = new Amnesiac(amnesiac);
                    break;
                    
                case RoleEnum.Anarchist:
                    newRole = new Anarchist(amnesiac);
                    break;
                    
                case RoleEnum.Arsonist:
                    newRole = new Arsonist(amnesiac);
                    break;
                    
                case RoleEnum.Blackmailer:
                    newRole = new Blackmailer(amnesiac);
                    break;
                    
                case RoleEnum.Bomber:
                    newRole = new Bomber(amnesiac);
                    break;
                    
                case RoleEnum.Camouflager:
                    newRole = new Camouflager(amnesiac);
                    break;
                    
                case RoleEnum.Cannibal:
                    newRole = new Cannibal(amnesiac);
                    break;
                    
                case RoleEnum.Concealer:
                    newRole = new Concealer(amnesiac);
                    break;
                    
                case RoleEnum.Consigliere:
                    newRole = new Consigliere(amnesiac);
                    break;
                    
                case RoleEnum.Consort:
                    newRole = new Consort(amnesiac);
                    break;
                    
                case RoleEnum.Coroner:
                    newRole = new Coroner(amnesiac);
                    break;
                    
                case RoleEnum.Crewmate:
                    newRole = new Crewmate(amnesiac);
                    break;
                    
                case RoleEnum.Cryomaniac:
                    newRole = new Cryomaniac(amnesiac);
                    break;
                    
                case RoleEnum.Dampyr:
                    newRole = new Dampyr(amnesiac);
                    break;
                    
                case RoleEnum.Detective:
                    newRole = new Detective(amnesiac);
                    break;
                    
                case RoleEnum.Disguiser:
                    newRole = new Disguiser(amnesiac);
                    break;
                    
                case RoleEnum.Dracula:
                    newRole = new Dracula(amnesiac);
                    break;
                    
                case RoleEnum.Engineer:
                    newRole = new Engineer(amnesiac);
                    break;
                    
                case RoleEnum.Escort:
                    newRole = new Escort(amnesiac);
                    break;
                    
                case RoleEnum.Executioner:
                    newRole = new Executioner(amnesiac);
                    break;
                    
                case RoleEnum.Framer:
                    newRole = new Framer(amnesiac);
                    break;
                    
                case RoleEnum.Glitch:
                    newRole = new Glitch(amnesiac);
                    break;
                    
                case RoleEnum.Godfather:
                    newRole = new Godfather(amnesiac);
                    break;
                    
                case RoleEnum.Gorgon:
                    newRole = new Gorgon(amnesiac);
                    break;
                    
                case RoleEnum.Grenadier:
                    newRole = new Grenadier(amnesiac);
                    break;
                    
                case RoleEnum.GuardianAngel:
                    newRole = new GuardianAngel(amnesiac);
                    break;
                    
                case RoleEnum.Impostor:
                    newRole = new Impostor(amnesiac);
                    break;
                    
                case RoleEnum.Inspector:
                    newRole = new Inspector(amnesiac);
                    break;
                    
                case RoleEnum.Investigator:
                    newRole = new Investigator(amnesiac);
                    break;
                    
                case RoleEnum.Jackal:
                    newRole = new Jackal(amnesiac);
                    break;
                    
                case RoleEnum.Jester:
                    newRole = new Jester(amnesiac);
                    break;
                    
                case RoleEnum.Juggernaut:
                    newRole = new Juggernaut(amnesiac);
                    break;
                    
                case RoleEnum.Sheriff:
                    newRole = new Sheriff(amnesiac);
                    break;
                    
                case RoleEnum.Mayor:
                    newRole = new Mayor(amnesiac);
                    break;
                    
                case RoleEnum.Mafioso:
                    newRole = new Mafioso(amnesiac);
                    break;
                    
                case RoleEnum.Miner:
                    newRole = new Miner(amnesiac);
                    break;
                    
                case RoleEnum.Morphling:
                    newRole = new Morphling(amnesiac);
                    break;
                    
                case RoleEnum.Swapper:
                    newRole = new Swapper(amnesiac);
                    break;
                    
                case RoleEnum.Medic:
                    newRole = new Medic(amnesiac);
                    break;
                    
                case RoleEnum.Tracker:
                    newRole = new Tracker(amnesiac);
                    break;
                    
                case RoleEnum.Transporter:
                    newRole = new Transporter(amnesiac);
                    break;
                    
                case RoleEnum.Medium:
                    newRole = new Medium(amnesiac);
                    break;
                    
                case RoleEnum.Operative:
                    newRole = new Operative(amnesiac);
                    break;
                    
                case RoleEnum.Shifter:
                    newRole = new Shifter(amnesiac);
                    break;
                    
                case RoleEnum.Rebel:
                    newRole = new Rebel(amnesiac);
                    break;
                    
                case RoleEnum.Sidekick:
                    newRole = new Sidekick(amnesiac);
                    break;
                    
                case RoleEnum.Shapeshifter:
                    newRole = new Shapeshifter(amnesiac);
                    break;
                    
                case RoleEnum.Murderer:
                    newRole = new Murderer(amnesiac);
                    break;
                    
                case RoleEnum.Survivor:
                    newRole = new Survivor(amnesiac);
                    break;
                    
                case RoleEnum.Plaguebearer:
                    newRole = new Plaguebearer(amnesiac);
                    break;
                    
                case RoleEnum.Pestilence:
                    newRole = new Pestilence(amnesiac);
                    break;
                    
                case RoleEnum.SerialKiller:
                    newRole = new SerialKiller(amnesiac);
                    break;
                    
                case RoleEnum.Werewolf:
                    newRole = new Werewolf(amnesiac);
                    break;
                    
                case RoleEnum.Janitor:
                    newRole = new Janitor(amnesiac);
                    break;
                    
                case RoleEnum.Poisoner:
                    newRole = new Poisoner(amnesiac);
                    break;
                    
                case RoleEnum.TimeLord:
                    newRole = new TimeLord(amnesiac);
                    break;
                    
                case RoleEnum.Teleporter:
                    newRole = new Teleporter(amnesiac);
                    break;
                    
                case RoleEnum.TimeMaster:
                    newRole = new TimeMaster(amnesiac);
                    break;
                    
                case RoleEnum.Troll:
                    newRole = new Troll(amnesiac);
                    break;
                    
                case RoleEnum.Thief:
                    newRole = new Thief(amnesiac);
                    break;
                    
                case RoleEnum.Undertaker:
                    newRole = new Undertaker(amnesiac);
                    break;
                    
                case RoleEnum.Vampire:
                    newRole = new Vampire(amnesiac);
                    break;
                    
                case RoleEnum.VampireHunter:
                    newRole = new VampireHunter(amnesiac);
                    break;
                    
                case RoleEnum.Veteran:
                    newRole = new Veteran(amnesiac);
                    break;
                    
                case RoleEnum.Vigilante:
                    newRole = new Vigilante(amnesiac);
                    break;
                    
                case RoleEnum.Warper:
                    newRole = new Warper(amnesiac);
                    break;
                    
                case RoleEnum.Wraith:
                    newRole = new Wraith(amnesiac);
                    break;
                
                default:
                    newRole = new Amnesiac(amnesiac);
                    break;
            }
            
            newRole.RoleHistory.Add(amneRole);
            newRole.RoleHistory.AddRange(amneRole.RoleHistory);

            DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
            
            if (newRole.Player == PlayerControl.LocalPlayer)
                newRole.RegenTask();

            tasks1 = other.myTasks;
            taskinfos1 = other.Data.Tasks;
            tasks2 = amnesiac.myTasks;
            taskinfos2 = amnesiac.Data.Tasks;

            amnesiac.myTasks = tasks1;
            amnesiac.Data.Tasks = taskinfos1;
            other.myTasks = tasks2;
            other.Data.Tasks = taskinfos2;

            if (ability == AbilityEnum.Snitch)
            {
                var snitchRole = Ability.GetAbility<Snitch>(amnesiac);
                snitchRole.ImpArrows.DestroyAll();
                snitchRole.SnitchArrows.Values.DestroyAll();
                snitchRole.SnitchArrows.Clear();
                CompleteTask.Postfix(amnesiac);

                if (other.AmOwner)
                {
                    foreach (var player in PlayerControl.AllPlayerControls)
                        player.nameText().color = Color.white;
                }
            }

            if (amnesiac.Is(Faction.Intruder) && (!amnesiac.Is(ObjectifierEnum.Traitor) || CustomGameOptions.SnitchSeesTraitor))
            {
                foreach (var snitch in Ability.GetAbilities(AbilityEnum.Snitch))
                {
                    var snitchRole = (Snitch)snitch;
                    var role3 = Role.GetRole(snitch.Player);

                    if (role3.TasksDone && PlayerControl.LocalPlayer.Is(AbilityEnum.Snitch))
                    {
                        var gameObj = new GameObject();
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = Sprite;
                        arrow.image = renderer;
                        gameObj.layer = 5;
                        snitchRole.SnitchArrows.Add(amnesiac.PlayerId, arrow);
                    }
                    else if (role3.TasksDone && PlayerControl.LocalPlayer == amnesiac)
                    {
                        var gameObj = new GameObject();
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = Sprite;
                        arrow.image = renderer;
                        gameObj.layer = 5;
                        snitchRole.ImpArrows.Add(arrow);
                    }
                }
            }
        }
    }
}
