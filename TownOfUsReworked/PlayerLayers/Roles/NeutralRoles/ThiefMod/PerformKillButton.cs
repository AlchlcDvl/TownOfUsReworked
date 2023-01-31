using HarmonyLib;
using Hazel;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Abilities.SnitchMod;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;
using System;
using AmongUs.GameOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.ThiefMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformSteal
    {
        public static Sprite Sprite => TownOfUsReworked.Arrow;
        
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton || !PlayerControl.LocalPlayer.Is(RoleEnum.Thief))
                return true;

            if (!PlayerControl.LocalPlayer.CanMove || PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            var role = Role.GetRole<Thief>(PlayerControl.LocalPlayer);

            if (__instance.isCoolingDown || !__instance.enabled)
                return false;

            var maxDistance = GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];

            if (role == null)
                return false;

            if (role.ClosestPlayer.IsInfected() || role.Player.IsInfected())
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer))
                    ((Plaguebearer)pb).RpcSpreadInfection(role.ClosestPlayer, role.Player);
            }
            else if (role.Player.IsOtherRival(role.ClosestPlayer))
            {
                role.LastKilled = DateTime.UtcNow;
                return false;
            }
            else if (!(role.ClosestPlayer.Is(Faction.Intruder) || role.ClosestPlayer.Is(Faction.Syndicate) || role.ClosestPlayer.Is(SubFaction.Cabal) ||
                role.ClosestPlayer.Is(SubFaction.Undead) || role.ClosestPlayer.Is(RoleAlignment.NeutralKill) || role.ClosestPlayer.Is(RoleAlignment.CrewKill)))
            {
                Utils.RpcMurderPlayer(role.Player, role.Player);
                return false;
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.Action, SendOption.Reliable, -1);
            writer.Write((byte)ActionsRPC.Steal);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            writer.Write(role.ClosestPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            
            //SoundManager.Instance.PlaySound(TownOfUsReworked.StealSound, false, 0.4f);

            Steal(role, role.ClosestPlayer);
            Utils.RpcMurderPlayer(role.Player, role.ClosestPlayer);
            return false;
        }

        public static void Steal(Thief thiefRole, PlayerControl other)
        {
            var role = Role.GetRole(other);
            var roleType = role.RoleType;
            var ability = (Ability.GetAbility(other))?.AbilityType;
            var thief = thiefRole.Player;
            Role newRole;

            switch (roleType)
            {
                case RoleEnum.Anarchist:
                    newRole = new Anarchist(thief);
                    break;
                    
                case RoleEnum.Arsonist:
                    newRole = new Arsonist(thief);
                    break;
                    
                case RoleEnum.Blackmailer:
                    newRole = new Blackmailer(thief);
                    break;
                    
                case RoleEnum.Bomber:
                    newRole = new Bomber(thief);
                    break;
                    
                case RoleEnum.Camouflager:
                    newRole = new Camouflager(thief);
                    break;
                    
                case RoleEnum.Concealer:
                    newRole = new Concealer(thief);
                    break;
                    
                case RoleEnum.Consigliere:
                    newRole = new Consigliere(thief);
                    break;
                    
                case RoleEnum.Consort:
                    newRole = new Consort(thief);
                    break;
                    
                case RoleEnum.Cryomaniac:
                    newRole = new Cryomaniac(thief);
                    break;
                    
                case RoleEnum.Dampyr:
                    newRole = new Dampyr(thief);
                    break;
                    
                case RoleEnum.Disguiser:
                    newRole = new Disguiser(thief);
                    break;
                    
                case RoleEnum.Dracula:
                    newRole = new Dracula(thief);
                    break;
                    
                case RoleEnum.Framer:
                    newRole = new Framer(thief);
                    break;
                    
                case RoleEnum.Glitch:
                    newRole = new Glitch(thief);
                    break;
                    
                case RoleEnum.Godfather:
                    newRole = new Godfather(thief);
                    break;
                    
                case RoleEnum.Gorgon:
                    newRole = new Gorgon(thief);
                    break;
                    
                case RoleEnum.Grenadier:
                    newRole = new Grenadier(thief);
                    break;
                    
                case RoleEnum.Impostor:
                    newRole = new Impostor(thief);
                    break;
                    
                case RoleEnum.Juggernaut:
                    newRole = new Juggernaut(thief);
                    break;
                    
                case RoleEnum.Mafioso:
                    newRole = new Mafioso(thief);
                    break;
                    
                case RoleEnum.Miner:
                    newRole = new Miner(thief);
                    break;
                    
                case RoleEnum.Morphling:
                    newRole = new Morphling(thief);
                    break;
                    
                case RoleEnum.Rebel:
                    newRole = new Rebel(thief);
                    break;
                    
                case RoleEnum.Sidekick:
                    newRole = new Sidekick(thief);
                    break;
                    
                case RoleEnum.Shapeshifter:
                    newRole = new Shapeshifter(thief);
                    break;
                    
                case RoleEnum.Murderer:
                    newRole = new Murderer(thief);
                    break;
                    
                case RoleEnum.Plaguebearer:
                    newRole = new Plaguebearer(thief);
                    break;
                    
                case RoleEnum.Pestilence:
                    newRole = new Pestilence(thief);
                    break;
                    
                case RoleEnum.SerialKiller:
                    newRole = new SerialKiller(thief);
                    break;
                    
                case RoleEnum.Werewolf:
                    newRole = new Werewolf(thief);
                    break;
                    
                case RoleEnum.Janitor:
                    newRole = new Janitor(thief);
                    break;
                    
                case RoleEnum.Poisoner:
                    newRole = new Poisoner(thief);
                    break;
                    
                case RoleEnum.Teleporter:
                    newRole = new Teleporter(thief);
                    break;
                    
                case RoleEnum.TimeMaster:
                    newRole = new TimeMaster(thief);
                    break;
                    
                case RoleEnum.Thief:
                    newRole = new Thief(thief);
                    break;
                    
                case RoleEnum.Undertaker:
                    newRole = new Undertaker(thief);
                    break;
                    
                case RoleEnum.VampireHunter:
                    newRole = new VampireHunter(thief);
                    break;
                    
                case RoleEnum.Veteran:
                    newRole = new Veteran(thief);
                    break;
                    
                case RoleEnum.Vigilante:
                    newRole = new Vigilante(thief);
                    break;
                    
                case RoleEnum.Warper:
                    newRole = new Warper(thief);
                    break;
                    
                case RoleEnum.Wraith:
                    newRole = new Wraith(thief);
                    break;
                
                default:
                    newRole = new Thief(thief);
                    break;
            }
            
            newRole.RoleHistory.Add(thiefRole);
            newRole.RoleHistory.AddRange(thiefRole.RoleHistory);

            if (thief == newRole.Player && thief == PlayerControl.LocalPlayer)
                newRole.RegenTask();

            if (other.IsRecruit())
                newRole.IsRecruit = true;
            
            if (CustomGameOptions.ThiefSteals)
            {
                var newRole2 = new Thief(other);
                newRole2.RoleHistory.Add(role);
                newRole2.RoleHistory.AddRange(role.RoleHistory);

                if (PlayerControl.LocalPlayer == other && other.Is(Faction.Intruder))
                    DestroyableSingleton<HudManager>.Instance.SabotageButton.gameObject.SetActive(false);
                
                if (other.IsRecruit())
                    newRole2.IsRecruit = true;
                
                if (other == PlayerControl.LocalPlayer)
                    newRole2.RegenTask();
            }

            if (ability == AbilityEnum.Snitch)
            {
                var snitchRole = Ability.GetAbility<Snitch>(thief);
                snitchRole.ImpArrows.DestroyAll();
                snitchRole.SnitchArrows.Values.DestroyAll();
                snitchRole.SnitchArrows.Clear();
                CompleteTask.Postfix(thief);

                if (other.AmOwner)
                {
                    foreach (var player in PlayerControl.AllPlayerControls)
                        player.nameText().color = Color.white;
                }
            }

            if (thief.Is(Faction.Intruder) && (!thief.Is(ObjectifierEnum.Traitor) || CustomGameOptions.SnitchSeesTraitor))
            {
                foreach (var snitch in Ability.GetAbilities(AbilityEnum.Snitch))
                {
                    var snitchRole = (Snitch)snitch;
                    var role3 = Role.GetRole(snitchRole.Player);

                    if (role3.TasksDone && PlayerControl.LocalPlayer.Is(AbilityEnum.Snitch))
                    {
                        var gameObj = new GameObject();
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = Sprite;
                        arrow.image = renderer;
                        gameObj.layer = 5;
                        snitchRole.SnitchArrows.Add(thief.PlayerId, arrow);
                    }
                    else if (role3.TasksDone && PlayerControl.LocalPlayer == thief)
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
