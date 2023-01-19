using HarmonyLib;
using Hazel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.NeutralsMod;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers;
using UD = TownOfUsReworked.PlayerLayers.Abilities.UnderdogMod;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.CamouflagerMod;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Modifiers;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using Object = UnityEngine.Object;
using Reactor.Utilities;
using Random = UnityEngine.Random;
using Il2CppInterop.Runtime.InteropTypes;

namespace TownOfUsReworked.Extensions
{
    [HarmonyPatch]
    public static class Utils
    {
        internal static bool ShowDeadBodies = false;
        private static GameData.PlayerInfo voteTarget = null;
        public static Dictionary<PlayerControl, Color> oldColors = new Dictionary<PlayerControl, Color>();
        public static List<WinningPlayerData> potentialWinners = new List<WinningPlayerData>();
        public static Sprite LockSprite = TownOfUsReworked.LockSprite;

        public static void Invis(PlayerControl player)
        {
            var color = new Color32(0, 0, 0, 0);

            if (PlayerControl.LocalPlayer.Is(Faction.Intruder) || PlayerControl.LocalPlayer.Data.IsDead)
                color.a = 26;

            if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Invis)
            {
                player.SetOutfit(CustomPlayerOutfitType.Invis, new GameData.PlayerOutfit()
                {
                    ColorId = player.CurrentOutfit.ColorId,
                    HatId = "",
                    SkinId = "",
                    VisorId = "",
                    PlayerName = " "
                });

                player.myRend().color = color;
                player.nameText().color = new Color32(0, 0, 0, 0);
                player.cosmetics.colorBlindText.color = new Color32(0, 0, 0, 0);
            }
        }

        /*public static void SendMessage(string text, byte sendTo = byte.MaxValue)
        {
            if (!AmongUsClient.Instance.AmHost)
                return;

            TownOfUsReworked.MessagesToSend.Add((text, sendTo));
        }*/

        public static InnerNet.ClientData GetClient(this PlayerControl player)
        {
            var client = AmongUsClient.Instance.allClients.ToArray().Where(cd => cd.Character.PlayerId == player.PlayerId).FirstOrDefault();
            return client;
        }
        
        public static int GetClientId(this PlayerControl player)
        {
            var client = player.GetClient();
            return client == null ? -1 : client.Id;
        }

        public static void Conceal()
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player != PlayerControl.LocalPlayer)
                {
                    var color = Colors.Clear;
                    var playerName = " ";

                    if (player.Is(Faction.Syndicate) || player.Data.IsDead)
                    {
                        color.a = 26;
                        playerName = player.name;
                    }

                    if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Invis &&
                        player.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage &&
                        player.GetCustomOutfitType() != CustomPlayerOutfitType.PlayerNameOnly)
                    {
                        player.SetOutfit(CustomPlayerOutfitType.Invis, new GameData.PlayerOutfit()
                        {
                            ColorId = player.CurrentOutfit.ColorId,
                            HatId = "",
                            SkinId = "",
                            VisorId = "",
                            PlayerName = playerName
                        });
                        PlayerMaterial.SetColors(color, player.myRend());
                        player.nameText().color = color;
                        player.cosmetics.colorBlindText.color = color;
                    }
                }
            }
        }

        public static void Morph(PlayerControl player, PlayerControl MorphedPlayer)
        {
            if (CamouflageUnCamouflage.IsCamoed)
                return;

            if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Morph)
                player.SetOutfit(CustomPlayerOutfitType.Morph, MorphedPlayer.Data.DefaultOutfit);
            
            foreach (var player2 in PlayerControl.AllPlayerControls)
            {
                if (player2 == PlayerControl.LocalPlayer)
                    player.SetOutfit(CustomPlayerOutfitType.Default);
            }
        }

        public static void DefaultOutfit(PlayerControl player)
        {
            player.SetOutfit(CustomPlayerOutfitType.Default);
        }

        public static void Shapeshift()
        {
            var allPlayers = PlayerControl.AllPlayerControls;

            foreach (var player in allPlayers)
            {
                if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage &&
                    player.GetCustomOutfitType() != CustomPlayerOutfitType.Invis &&
                    player.GetCustomOutfitType() != CustomPlayerOutfitType.PlayerNameOnly)
                {
                    int random;

                    while (true)
                    {
                        random = Random.RandomRangeInt(0, allPlayers.Count);

                        if (player != allPlayers[random])
                            break;
                    }

                    var otherPlayer = allPlayers[random];

                    Morph(player, otherPlayer);
                }
            }
        }

        public static void Camouflage()
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage &&
                    player.GetCustomOutfitType() != CustomPlayerOutfitType.Invis &&
                    player.GetCustomOutfitType() != CustomPlayerOutfitType.PlayerNameOnly)
                {
                    player.SetOutfit(CustomPlayerOutfitType.Camouflage, new GameData.PlayerOutfit()
                    {
                        ColorId = player.GetDefaultOutfit().ColorId,
                        HatId = "",
                        SkinId = "",
                        VisorId = "",
                        PlayerName = " "
                    });
                    PlayerMaterial.SetColors(Color.grey, player.myRend());
                    player.nameText().color = Color.clear;
                    player.cosmetics.colorBlindText.color = Color.clear;

                    if (CustomGameOptions.CamoHideSize)
                        player.GetAppearance().SizeFactor.Set(1f, 1f, 1f);

                    if (CustomGameOptions.CamoHideSpeed)
                        player.MyPhysics.body.velocity.Set(CustomGameOptions.PlayerSpeed, CustomGameOptions.PlayerSpeed);
                }
            }
        }

        public static void DefaultOutfitAll()
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player != null)
                    DefaultOutfit(player);
            }
        }

        public static void AddUnique<T>(this Il2CppSystem.Collections.Generic.List<T> self, T item) where T : IDisconnectHandler
        {
            if (!self.Contains(item))
                self.Add(item);
        }

        public static bool Is(this PlayerControl player, RoleEnum roleType)
        {
            return Role.GetRole(player)?.RoleType == roleType;
        }

        public static bool Is(this PlayerControl player, Role role)
        {
            return Role.GetRole(player) == role;
        }

        public static bool Is(this PlayerControl player, SubFaction subFaction)
        {
            return Role.GetRole(player)?.SubFaction == subFaction;
        }

        public static bool Is(this PlayerControl player, ModifierEnum modifierType)
        {
            return Modifier.GetModifier(player)?.ModifierType == modifierType;
        }

        public static bool Is(this PlayerControl player, ObjectifierEnum abilityType)
        {
            return Objectifier.GetObjectifier(player)?.ObjectifierType == abilityType;
        }

        public static bool Is(this PlayerControl player, AbilityEnum ability)
        {
            return Ability.GetAbility(player)?.AbilityType == ability;
        }

        public static bool Is(this PlayerControl player, InspResults results)
        {
            return Role.GetRole(player)?.Results == results;
        }

        public static bool Is(this PlayerControl player, Faction faction)
        {
            return Role.GetRole(player)?.Faction == faction;
        }

        public static bool Is(this PlayerControl player, RoleAlignment alignment)
        {
            return Role.GetRole(player)?.RoleAlignment == alignment;
        }

        public static bool Is(this PlayerControl player, AttackEnum attack)
        {
            return Role.GetRole(player)?.Attack == attack;
        }

        public static bool Is(this PlayerControl player, DefenseEnum defense)
        {
            return Role.GetRole(player)?.Defense == defense;
        }

        public static Faction GetFaction(this PlayerControl player)
        {
            if (player == null)
                return Faction.None;

            var role = Role.GetRole(player);

            if (role == null)
                return player.Data.IsImpostor() ? Faction.Intruder : Faction.Crew;

            return role.Faction;
        }

        public static List<PlayerControl> GetCrewmates(List<PlayerControl> impostors)
        {
            return PlayerControl.AllPlayerControls.ToArray().Where(player => !impostors.Any(imp => imp.PlayerId == player.PlayerId)).ToList();
        }

        public static List<PlayerControl> GetImpostors(List<GameData.PlayerInfo> infected)
        {
            var impostors = new List<PlayerControl>();

            foreach (var impData in infected)
                impostors.Add(impData.Object);

            return impostors;
        }

        public static bool IsRecruit(this PlayerControl player)
        {
            if (player == null)
                return false;
                
            var role = Role.GetRole(player);

            if (role == null)
                return false;

            return role.IsRecruit;
        }

        public static bool IsBase(this PlayerControl player)
        {
            if (player == null)
                return false;
                
            var role = Role.GetRole(player);

            if (role == null)
                return true;

            return role.Base;
        }

        public static bool IsGATarget(this PlayerControl player)
        {
            if (player == null)
                return false;
            
            bool flag = false;
                
            foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
            {
                if (ga.TargetPlayer == null)
                    continue;

                if (player.PlayerId == ga.TargetPlayer.PlayerId)
                {
                    flag = true;
                    break;
                }
            }

            return flag;
        }

        public static bool IsExeTarget(this PlayerControl player)
        {
            if (player == null)
                return false;
            
            bool flag = false;
                
            foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
            {
                if (exe.TargetPlayer == null)
                    continue;

                if (player.PlayerId == exe.TargetPlayer.PlayerId)
                {
                    flag = true;
                    break;
                }
            }

            return flag;
        }

        public static bool CanDoTasks(this PlayerControl player)
        {
            if (player == null)
                return false;
                
            var phantomflag = player.Is(RoleEnum.Phantom);
            
            var crewflag = player.Is(Faction.Crew);
            var neutralflag = player.Is(Faction.Neutral);

            var loverflag = player.Is(ObjectifierEnum.Lovers);
            var rivalflag = player.Is(ObjectifierEnum.Rivals);
            var taskmasterflag = player.Is(ObjectifierEnum.Taskmaster);
            var corruptedflag = player.Is(ObjectifierEnum.Corrupted);
            var recruitflag = player.IsRecruit();

            var isdead = player.Data.IsDead;

            var flag1 = crewflag && !(loverflag || rivalflag || corruptedflag);
            var flag2 = neutralflag && (taskmasterflag || (phantomflag && isdead));
            var flag = (flag1 || flag2) && !recruitflag;

            return flag;
        }

        public static Jackal GetJackal(this PlayerControl player)
        {
            if (player == null)
                return null;
            
            if (!player.IsRecruit())
                return null;
                
            return Role.GetRoles(RoleEnum.Jackal).FirstOrDefault(role =>
            {
                var goodRecruit = ((Jackal)role).GoodRecruit;
                var evilRecruit = ((Jackal)role).EvilRecruit;
                var backupRecruit = ((Jackal)role).BackupRecruit;

                var goodFlag = goodRecruit != null && player.PlayerId == goodRecruit.PlayerId;
                var evilFlag = evilRecruit != null && player.PlayerId == evilRecruit.PlayerId;
                var backupFlag = backupRecruit != null && player.PlayerId == backupRecruit.PlayerId;
                return goodFlag || evilFlag || backupFlag;
            }) as Jackal;
        }
        
        public static DefenseEnum GetDefense(PlayerControl player)
        {
            if (player == null)
                return DefenseEnum.None;
            
            var role = Role.GetRole(player);

            if (role == null)
                return DefenseEnum.None;

            return role.Defense;
        }
        
        public static AttackEnum GetAttack(PlayerControl player)
        {
            if (player == null)
                return AttackEnum.None;
                
            var role = Role.GetRole(player);

            if (role == null)
                return AttackEnum.None;

            return role.Attack;
        }

        public static bool IsStronger(PlayerControl attacker, PlayerControl target)
        {
            var flag = false;
            var attack = GetAttack(attacker);
            var defense = GetDefense(target);

            if ((byte)defense < (byte)attack)
                flag = true;

            return flag;
        }

        public static RoleEnum GetRole(PlayerControl player)
        {
            if (player == null || player.Data == null)
                return RoleEnum.None;

            var role = Role.GetRole(player);

            if (role != null)
                return role.RoleType;

            return player.Data.IsImpostor() ? RoleEnum.Impostor : RoleEnum.Crewmate;
        }

        public static AbilityEnum GetAbility(PlayerControl player)
        {
            if (player == null || player.Data == null)
                return AbilityEnum.None;

            var ability = Ability.GetAbility(player);

            if (ability == null)
                return AbilityEnum.None;

            return ability.AbilityType;
        }

        public static ObjectifierEnum GetObjectifier(PlayerControl player)
        {
            if (player == null || player.Data == null)
                return ObjectifierEnum.None;

            var objectifier = Objectifier.GetObjectifier(player);

            if (objectifier == null)
                return ObjectifierEnum.None;

            return objectifier.ObjectifierType;
        }

        public static ModifierEnum GetModifier(PlayerControl player)
        {
            if (player == null || player.Data == null)
                return ModifierEnum.None;

            var modifier = Modifier.GetModifier(player);

            if (modifier == null)
                return ModifierEnum.None;

            return modifier.ModifierType;
        }

        public static PlayerControl PlayerById(byte id)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.PlayerId == id)
                    return player;
            }

            return null;
        }

        public static PlayerControl PlayerByPool(this PoolablePlayer player)
        {
            foreach (var player2 in PlayerControl.AllPlayerControls)
            {
                if (player2.nameText() == player.NameText())
                    return player2;
            }

            return null;
        }

        public static bool IsShielded(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Medic).Any(role =>
            {
                var shieldedPlayer = ((Medic)role).ShieldedPlayer;
                return shieldedPlayer != null && player.PlayerId == shieldedPlayer.PlayerId;
            });
        }

        public static Medic GetMedic(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Medic).FirstOrDefault(role =>
            {
                var shieldedPlayer = ((Medic)role).ShieldedPlayer;
                var shieldedFlag = shieldedPlayer != null && player == shieldedPlayer;
                return shieldedFlag;
            }) as Medic;
        }

        public static bool IsOnAlert(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Veteran).Any(role =>
            {
                var veteran = (Veteran)role;
                return veteran != null && veteran.OnAlert && player == veteran.Player;
            });
        }

        public static bool IsVesting(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Survivor).Any(role =>
            {
                var surv = (Survivor)role;
                return surv != null && surv.Vesting && player == surv.Player;
            });
        }

        public static bool IsProtected(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.GuardianAngel).Any(role =>
            {
                var gaTarget = ((GuardianAngel)role).TargetPlayer;
                var ga = (GuardianAngel)role;
                return gaTarget != null && ga.Protecting && player == gaTarget;
            });
        }

        public static bool IsInfected(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Plaguebearer).Any(role =>
            {
                var plaguebearer = (Plaguebearer)role;

                return plaguebearer != null && (plaguebearer.InfectedPlayers.Contains(player.PlayerId) || player == plaguebearer.Player);
            });
        }

        public static bool IsFramed(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Framer).Any(role =>
            {
                var framer = (Framer)role;

                return framer != null && framer.Framed.Contains(player.PlayerId);
            });
        }

        public static bool IsWinningRival(this PlayerControl player)
        {
            if (player == null || player.Data == null)
                return false;

            if (!player.Is(ObjectifierEnum.Rivals))
                return false;
            
            var rival = Objectifier.GetObjectifier<Rivals>(player);

            return rival.RivalDead();
        }

        public static bool IsTurnedTraitor(this PlayerControl player)
        {
            return player.IsIntTraitor() || player.IsSynTraitor();
        }

        public static bool IsIntTraitor(this PlayerControl player)
        {
            if (player == null || player.Data == null)
                return false;

            if (!player.Is(ObjectifierEnum.Traitor))
                return false;
            
            var traitor = Role.GetRole(player);

            return traitor.IsIntTraitor;
        }

        public static bool IsSynTraitor(this PlayerControl player)
        {
            if (player == null || player.Data == null)
                return false;

            if (!player.Is(ObjectifierEnum.Traitor))
                return false;
            
            var traitor = Role.GetRole(player);

            return traitor.IsSynTraitor;
        }

        public static bool IsOtherRival(this PlayerControl player, PlayerControl refPlayer)
        {
            if (player == null || player.Data == null)
                return false;

            if (!player.Is(ObjectifierEnum.Rivals) || !refPlayer.Is(ObjectifierEnum.Rivals))
                return false;
            
            var rival1 = Objectifier.GetObjectifier<Rivals>(player);
            var rival2 = Objectifier.GetObjectifier<Rivals>(refPlayer);

            return rival1.OtherRival == refPlayer && rival2.OtherRival == player;
        }

        public static bool CrewWins()
        {
            var flag = (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.IsRecruit() || x.Is(ObjectifierEnum.Lovers) ||
                x.Is(Faction.Intruder) || x.Is(RoleAlignment.NeutralKill) || x.Is(RoleAlignment.NeutralNeo) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralPros) ||
                x.IsTurnedTraitor() || x.Is(ObjectifierEnum.Corrupted) || x.IsWinningRival())) == 0) || TasksDone();

            return flag;
        }

        public static bool IntrudersWin()
        {
            var flag = (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.IsRecruit() || x.Is(ObjectifierEnum.Lovers) ||
                x.Is(Faction.Crew) || x.Is(RoleAlignment.NeutralKill) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralNeo) || x.Is(RoleAlignment.NeutralPros) ||
                x.IsWinningRival())) == 0) || Sabotaged();

            return flag;
        }

        public static bool AllNeutralsWin()
        {
            var flag = (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.IsRecruit() || x.Is(ObjectifierEnum.Lovers) ||
                x.Is(Faction.Crew) || x.Is(Faction.Syndicate) || x.IsWinningRival() || x.Is(Faction.Intruder))) == 0) && CustomGameOptions.NoSolo == NoSolo.AllNeutrals;

            return flag;
        }

        public static bool NKWins(RoleEnum nk)
        {
            var flag = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || (x.Is(RoleAlignment.NeutralKill) &&
                !x.Is(nk)) || x.Is(RoleAlignment.NeutralNeo) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralPros) || x.Is(Faction.Crew) || x.Is(ObjectifierEnum.Lovers) ||
                x.IsWinningRival())) == 0;

            return flag;
        }

        public static bool AllNKsWin()
        {
            var flag = (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(RoleAlignment.NeutralNeo) ||
                x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralPros) || x.Is(Faction.Crew) || x.Is(ObjectifierEnum.Lovers) || x.IsWinningRival())) == 0) && CustomGameOptions.NoSolo
                == NoSolo.AllNKs;

            return flag;
        }

        public static bool CorruptedWin()
        {
            var flag = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(ObjectifierEnum.Corrupted)) == 0;
            return flag;
        }

        public static bool LoversWin(ObjectifierEnum obj)
        {
            var flag1 = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) == 3;
            var flag2 = false;

            if (obj == ObjectifierEnum.Lovers)
            {
                var lover = Objectifier.GetObjectifierValue<Lovers>(obj);
                flag2 = lover.OtherLover != null && lover.Player != null && !lover.OtherLover.Data.IsDead && !lover.Player.Data.IsDead &&
                    !lover.OtherLover.Data.Disconnected && !lover.Player.Data.Disconnected;
            }

            var flag = flag1 && flag2;
            return flag;
        }

        public static bool RivalsWin(ObjectifierEnum obj)
        {
            var flag1 = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) == 2;
            var flag2 = false;

            if (obj == ObjectifierEnum.Rivals)
            {
                var rival = Objectifier.GetObjectifierValue<Rivals>(obj);
                flag2 = rival.RivalDead();
            }

            var flag = flag1 && flag2;
            return flag;
        }

        public static bool SyndicateWins()
        {
            var flag = (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.IsRecruit() || x.Is(RoleAlignment.NeutralKill) ||
                x.Is(Faction.Intruder) || x.Is(RoleAlignment.NeutralNeo) || x.Is(Faction.Crew) || x.Is(RoleAlignment.NeutralPros) || x.Is(ObjectifierEnum.Lovers) ||
                x.IsWinningRival())) == 0) || (CustomGameOptions.AltImps && Sabotaged());

            return flag;
        }

        public static bool UndeadWin()
        {
            var flag = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(RoleAlignment.NeutralKill) ||
                (x.Is(RoleAlignment.NeutralNeo) && !x.Is(RoleEnum.Dracula)) || x.Is(Faction.Syndicate) || x.Is(Faction.Crew) || x.IsRecruit() || x.Is(ObjectifierEnum.Lovers) ||
                (x.Is(RoleAlignment.NeutralPros) && !(x.Is(RoleEnum.Dampyr) || x.Is(RoleEnum.Vampire))) || x.IsWinningRival())) == 0;

            return flag;
        }

        public static bool CabalWin()
        {
            var flag = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(RoleAlignment.NeutralKill) ||
                (x.Is(RoleAlignment.NeutralNeo) && !x.Is(RoleEnum.Jackal)) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralPros) || x.Is(Faction.Crew) ||
                x.Is(ObjectifierEnum.Lovers) || x.IsWinningRival()) && !x.IsRecruit()) == 0;

            return flag;
        }

        public static bool IsWinner(this string playerName)
        {
            var flag = false;
            var winners = TempData.winners;

            foreach (var win in winners)
            {
                if (win.PlayerName == playerName)
                {
                    flag = true;
                    break;
                }
            }

            return flag;
        }

        public static string DeathReason(this PlayerControl player)
        {
            if (player == null)
                return "";
            
            var role = Role.GetRole(player);

            if (role == null)
                return " ERROR";

            var die = "";
            var killedBy = "";
            var result = "";

            if (role.DeathReason == DeathReasonEnum.Killed)
                die = "Killed";
            else if (role.DeathReason == DeathReasonEnum.Ejected)
                die = "Ejected";
            else if (role.DeathReason == DeathReasonEnum.Disconnected)
                die = "Disconnected";
            else if (role.DeathReason == DeathReasonEnum.Guessed)
                die = "Guessed";
            else if (role.DeathReason == DeathReasonEnum.Alive)
                die = "Alive";
            else if (role.DeathReason == DeathReasonEnum.Revived)
                die = "Revived";
            else if (role.DeathReason == DeathReasonEnum.Suicide)
                die = "Suicide";
            
            if (role.DeathReason != DeathReasonEnum.Alive && role.DeathReason != DeathReasonEnum.Ejected && role.DeathReason != DeathReasonEnum.Suicide)
                killedBy = role.KilledBy;
            
            result = die + killedBy;

            return result;
        }

        public static PlayerControl GetClosestPlayer(PlayerControl refPlayer, List<PlayerControl> AllPlayers)
        {
            var num = double.MaxValue;
            var refPosition = refPlayer.GetTruePosition();
            PlayerControl result = null;

            foreach (var player in AllPlayers)
            {
                if (player.Data.IsDead || player.PlayerId == refPlayer.PlayerId || !player.Collider.enabled)
                    continue;

                var playerPosition = player.GetTruePosition();
                var distBetweenPlayers = Vector2.Distance(refPosition, playerPosition);
                var isClosest = distBetweenPlayers < num;

                if (!isClosest)
                    continue;

                var vector = playerPosition - refPosition;

                if (PhysicsHelpers.AnyNonTriggersBetween(refPosition, vector.normalized, vector.magnitude, Constants.ShipAndObjectsMask))
                    continue;

                num = distBetweenPlayers;
                result = player;
            }

            return result;
        }

        public static PlayerControl GetClosestPlayer(PlayerControl refplayer)
        {
            return GetClosestPlayer(refplayer, PlayerControl.AllPlayerControls.ToArray().ToList());
        }

        public static string GetRoleColor(this string roleName)
        {
            if (roleName == null || roleName.Length == 0)
                return "";
            
            var color = new Color32(255, 255, 255, 255);

            switch (roleName)
            {
                case "Agent":
                    color = Colors.Agent;
                    break;

                case "Altruist":
                    color = Colors.Altruist;
                    break;

                case "Amnesiac":
                    color = Colors.Amnesiac;
                    break;

                case "Anarchist":
                    color = Colors.Syndicate;
                    break;

                case "Arsonist":
                    color = Colors.Arsonist;
                    break;

                case "Blackmailer":
                    color = Colors.Blackmailer;
                    break;

                case "Bomber":
                    color = Colors.Bomber;
                    break;

                case "Camouflager":
                    color = Colors.Camouflager;
                    break;

                case "Cannibal":
                    color = Colors.Cannibal;
                    break;

                case "Concealer":
                    color = Colors.Concealer;
                    break;

                case "Consigliere":
                    color = Colors.Consigliere;
                    break;

                case "Consort":
                    color = Colors.Consort;
                    break;

                case "Coroner":
                    color = Colors.Coroner;
                    break;

                case "Crewmate":
                    color = Colors.Crew;
                    break;

                case "Cryomaniac":
                    color = Colors.Cryomaniac;
                    break;

                case "Dampyr":
                    color = Colors.Dampyr;
                    break;

                case "Detective":
                    color = Colors.Detective;
                    break;

                case "Disguiser":
                    color = Colors.Disguiser;
                    break;

                case "Dracula":
                    color = Colors.Dracula;
                    break;

                case "Engineer":
                    color = Colors.Engineer;
                    break;

                case "Escort":
                    color = Colors.Escort;
                    break;

                case "Executioner":
                    color = Colors.Executioner;
                    break;

                case "Framer":
                    color = Colors.Framer;
                    break;

                case "Glitch":
                    color = Colors.Glitch;
                    break;

                case "Godfather":
                    color = Colors.Godfather;
                    break;

                case "Gorgon":
                    color = Colors.Gorgon;
                    break;

                case "Grenadier":
                    color = Colors.Grenadier;
                    break;

                case "Guardian Angel":
                    color = Colors.GuardianAngel;
                    break;

                case "Impostor":
                    color = Colors.Intruder;
                    break;

                case "Inspector":
                    color = Colors.Inspector;
                    break;

                case "Investigator":
                    color = Colors.Investigator;
                    break;

                case "Jackal":
                    color = Colors.Jackal;
                    break;

                case "Janitor":
                    color = Colors.Janitor;
                    break;

                case "Jester":
                    color = Colors.Jester;
                    break;

                case "Juggernaut":
                    color = Colors.Juggernaut;
                    break;

                case "Mafioso":
                    color = Colors.Mafioso;
                    break;

                case "Mayor":
                    color = Colors.Mayor;
                    break;

                case "Medic":
                    color = Colors.Medic;
                    break;

                case "Medium":
                    color = Colors.Medium;
                    break;

                case "Miner":
                    color = Colors.Miner;
                    break;

                case "Morphling":
                    color = Colors.Morphling;
                    break;

                case "Murderer":
                    color = Colors.Murderer;
                    break;

                case "Operative":
                    color = Colors.Operative;
                    break;

                case "Pestilence":
                    color = Colors.Pestilence;
                    break;

                case "Plaguebearer":
                    color = Colors.Plaguebearer;
                    break;

                case "Poisoner":
                    color = Colors.Poisoner;
                    break;

                case "Rebel":
                    color = Colors.Rebel;
                    break;

                case "Serial Killer":
                    color = Colors.SerialKiller;
                    break;

                case "Shapeshifter":
                    color = Colors.Shapeshifter;
                    break;

                case "Sheriff":
                    color = Colors.Sheriff;
                    break;

                case "Shifter":
                    color = Colors.Shifter;
                    break;

                case "Sidekick":
                    color = Colors.Sidekick;
                    break;

                case "Survivor":
                    color = Colors.Survivor;
                    break;

                case "Swapper":
                    color = Colors.Swapper;
                    break;

                case "Teleporter":
                    color = Colors.Teleporter;
                    break;

                case "Thief":
                    color = Colors.Thief;
                    break;

                case "Time Lord":
                    color = Colors.TimeLord;
                    break;

                case "Time Master":
                    color = Colors.TimeMaster;
                    break;

                case "Tracker":
                    color = Colors.Tracker;
                    break;

                case "Transporter":
                    color = Colors.Transporter;
                    break;

                case "Troll":
                    color = Colors.Troll;
                    break;

                case "Undertaker":
                    color = Colors.Undertaker;
                    break;

                case "Vampire":
                    color = Colors.Vampire;
                    break;

                case "Vampire Hunter":
                    color = Colors.VampireHunter;
                    break;

                case "Veteran":
                    color = Colors.Veteran;
                    break;

                case "Vigilante":
                    color = Colors.Vigilante;
                    break;

                case "Warper":
                    color = Colors.Warper;
                    break;

                case "Werewolf":
                    color = Colors.Werewolf;
                    break;

                case "Wraith":
                    color = Colors.Wraith;
                    break;

                default:
                    color = new Color32(255, 255, 255, 255);
                    break;
            }

            var role = Role.AllRoles.FirstOrDefault(x => x.Name == roleName);

            if (role == null)
                return "";

            if (role.Faction == Faction.Intruder && !CustomGameOptions.CustomIntColors)
                color = Colors.Intruder;
            else if (role.Faction == Faction.Syndicate && !CustomGameOptions.CustomSynColors)
                color = Colors.Syndicate;
            else if (role.Faction == Faction.Neutral && !CustomGameOptions.CustomNeutColors)
                color = Colors.Neutral;
            else if (role.Faction == Faction.Crew && !CustomGameOptions.CustomCrewColors)
                color = Colors.Crew;

            var colorString = "<color=#" + color.ToHtmlStringRGBA() + ">";
            return colorString;
        }

        public static void SetTarget(ref PlayerControl closestPlayer, KillButton button, float maxDistance = float.NaN, List<PlayerControl> targets = null)
        {
            if (!button.isActiveAndEnabled)
                return;

            button.SetTarget(SetClosestPlayer(ref closestPlayer, maxDistance, targets));
        }

        public static PlayerControl SetClosestPlayer(ref PlayerControl closestPlayer, float maxDistance = float.NaN, List<PlayerControl> targets = null)
        {
            if (float.IsNaN(maxDistance))
                maxDistance = GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];

            var player = GetClosestPlayer(PlayerControl.LocalPlayer, targets ?? PlayerControl.AllPlayerControls.ToArray().ToList());
            var closeEnough = player == null || (GetDistBetweenPlayers(PlayerControl.LocalPlayer, player) < maxDistance);
            return closestPlayer = closeEnough ? player : null;
        }

        public static double GetDistBetweenPlayers(PlayerControl player, PlayerControl refplayer)
        {
            if (player == null || refplayer == null)
                return 100000000000000;

            var truePosition = refplayer.GetTruePosition();
            var truePosition2 = player.GetTruePosition();
            return Vector2.Distance(truePosition, truePosition2);
        }

        public static void RpcMurderPlayer(PlayerControl killer, PlayerControl target)
        {
            MurderPlayer(killer, target);

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BypassKill, SendOption.Reliable, -1);
            writer.Write(killer.PlayerId);
            writer.Write(target.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static void MurderPlayer(PlayerControl killer, PlayerControl target)
        {
            if (killer == null || target == null)
                return;

            var data = target.Data;

            if (data != null && !data.IsDead)
            {
                if (killer == PlayerControl.LocalPlayer)
                {
                    try
                    {
                        SoundManager.Instance.PlaySound(TownOfUsReworked.KillSFX, false, 1f);
                    } catch {}
                }

                target.gameObject.layer = LayerMask.NameToLayer("Ghost");
                target.Visible = false;

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Coroner) && !PlayerControl.LocalPlayer.Data.IsDead)
                    Coroutines.Start(FlashCoroutine(Colors.Coroner));

                if (target.AmOwner)
                {
                    try
                    {
                        if (Minigame.Instance)
                        {
                            Minigame.Instance.Close();
                            Minigame.Instance.Close();
                        }

                        if (MapBehaviour.Instance)
                        {
                            MapBehaviour.Instance.Close();
                            MapBehaviour.Instance.Close();
                        }
                    } catch {}

                    DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(killer.Data, data);
                    DestroyableSingleton<HudManager>.Instance.ShadowQuad.gameObject.SetActive(false);
                    target.nameText().GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
                    target.RpcSetScanner(false);
                    var importantTextTask = new GameObject("_Player").AddComponent<ImportantTextTask>();
                    importantTextTask.transform.SetParent(AmongUsClient.Instance.transform, false);

                    if (!PlayerControl.GameOptions.GhostsDoTasks)
                    {
                        for (var i = 0; i < target.myTasks.Count; i++)
                        {
                            var playerTask = target.myTasks.ToArray()[i];
                            playerTask.OnRemove();
                            Object.Destroy(playerTask.gameObject);
                        }

                        target.myTasks.Clear();
                        importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GhostIgnoreTasks,
                            new Il2CppReferenceArray<Il2CppSystem.Object>(0));
                    }
                    else
                    {
                        importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GhostDoTasks, new Il2CppReferenceArray<Il2CppSystem.Object>(0));
                    }

                    target.myTasks.Insert(0, importantTextTask);
                }

                var targetRole = Role.GetRole(target);
                var killerRole = Role.GetRole(killer);

                if (!killer.Is(RoleEnum.Poisoner) && !killer.Is(RoleEnum.Arsonist) && !killer.Is(RoleEnum.Gorgon))
                    killer.MyPhysics.StartCoroutine(killer.KillAnimations.Random().CoPerformKill(killer, target));
                else
                    killer.MyPhysics.StartCoroutine(killer.KillAnimations.Random().CoPerformKill(target, target));

                if (killer != target)
                {
                    targetRole.KilledBy = " By " + killerRole.PlayerName;
                    targetRole.DeathReason = DeathReasonEnum.Killed;
                }
                else
                    targetRole.DeathReason = DeathReasonEnum.Suicide;

                if (killer == target)
                    return;

                if (target.Is(RoleEnum.Troll))
                {
                    var troll = (Troll)targetRole;
                    troll.Killed = true;
                    return;
                }

                var deadBody = new DeadPlayer
                {
                    PlayerId = target.PlayerId,
                    KillerId = killer.PlayerId,
                    KillTime = DateTime.UtcNow
                };

                Murder.KilledPlayers.Add(deadBody);

                if (!killer.AmOwner)
                    return;

                if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.SerialKiller))
                {
                    var sk = Role.GetRole<SerialKiller>(killer);
                    sk.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.LustKillCd);
                    sk.Player.SetKillTimer(CustomGameOptions.LustKillCd * CustomGameOptions.DiseasedMultiplier);
                    return;
                }
                else if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Glitch))
                {
                    var glitch = Role.GetRole<Glitch>(killer);
                    glitch.LastKill = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.GlitchKillCooldown);
                    glitch.Player.SetKillTimer(CustomGameOptions.GlitchKillCooldown * CustomGameOptions.DiseasedMultiplier);
                    return;
                }
                else if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Juggernaut))
                {
                    var juggernaut = Role.GetRole<Juggernaut>(killer);
                    juggernaut.LastKill = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * (CustomGameOptions.JuggKillCooldown +
                        5.0f - CustomGameOptions.JuggKillBonus * juggernaut.JuggKills));
                    juggernaut.Player.SetKillTimer((CustomGameOptions.JuggKillCooldown + 5.0f - CustomGameOptions.JuggKillBonus * juggernaut.JuggKills) *
                        CustomGameOptions.DiseasedMultiplier);
                    return;
                }
                else if (target.Is(ModifierEnum.Diseased) && killer.Is(AbilityEnum.Underdog))
                {
                    var lowerKC = (CustomGameOptions.IntKillCooldown - CustomGameOptions.UnderdogKillBonus) *
                        CustomGameOptions.DiseasedMultiplier;
                    var normalKC = CustomGameOptions.IntKillCooldown * CustomGameOptions.DiseasedMultiplier;
                    var upperKC = (CustomGameOptions.IntKillCooldown + CustomGameOptions.UnderdogKillBonus) *
                        CustomGameOptions.DiseasedMultiplier;
                    killer.SetKillTimer(LastImp() ? lowerKC : (UD.PerformKill.IncreasedKC() ? normalKC : upperKC));
                    return;
                }
                else if (target.Is(ModifierEnum.Diseased) && killer.Is(Faction.Intruder))
                {
                    killer.SetKillTimer(CustomGameOptions.IntKillCooldown * CustomGameOptions.DiseasedMultiplier);
                    return;
                }
                else if (target.Is(ModifierEnum.Bait))
                {
                    BaitReport(killer, target);
                    return;
                }

                if (killer.Is(AbilityEnum.Underdog))
                {
                    var lowerKC = CustomGameOptions.IntKillCooldown - CustomGameOptions.UnderdogKillBonus;
                    var normalKC = CustomGameOptions.IntKillCooldown;
                    var upperKC = CustomGameOptions.IntKillCooldown + CustomGameOptions.UnderdogKillBonus;
                    killer.SetKillTimer(LastImp() ? lowerKC : (UD.PerformKill.IncreasedKC() ? normalKC : upperKC));
                    return;
                }

                if (killer.Is(Faction.Intruder))
                {
                    killer.SetKillTimer(CustomGameOptions.IntKillCooldown);
                    return;
                }
            }
        }

        public static string GetRoleList(PlayerControl inspector, PlayerControl target)
        {
            var insp = Role.GetRole<Inspector>(inspector);
            var role = Role.GetRole(target);
            var yes = "";

            switch (role.Results)
            {
                case InspResults.SherConsigInspBm:
                    break;
            }

            return yes;
        }

        public static string GetEndGameName(this string playerName)
        {
            if (playerName == null)
                return "";
            
            PlayerControl target = null;
            
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.name == playerName)
                {
                    target = player;
                    break;
                }
            }

            if (target == null)
                return "";

            var role = Role.GetRole(target);
            var colorString = role == null ? "<color=#FFFFFFFF>" : role.ColorString;
            var winString = colorString + playerName + "\n" + role.Name + "</color>";
            return winString;
        }

        public static void BaitReport(PlayerControl killer, PlayerControl target)
        {
            Coroutines.Start(BaitReportDelay(killer, target));
        }

        public static IEnumerator BaitReportDelay(PlayerControl killer, PlayerControl target)
        {
            var extraDelay = Random.RandomRangeInt(0, (int) (100 * (CustomGameOptions.BaitMaxDelay - CustomGameOptions.BaitMinDelay) + 1));

            if (CustomGameOptions.BaitMaxDelay <= CustomGameOptions.BaitMinDelay)
                yield return new WaitForSeconds(CustomGameOptions.BaitMaxDelay + 0.01f);
            else
                yield return new WaitForSeconds(CustomGameOptions.BaitMinDelay + 0.01f + extraDelay/100f);
                
            var bodies = Object.FindObjectsOfType<DeadBody>();

            if (AmongUsClient.Instance.AmHost)
            {
                foreach (var body in bodies)
                {
                    if (body.ParentId == target.PlayerId)
                    {
                        killer.ReportDeadBody(target.Data);
                        break;
                    }
                }
            }
            else
            {
                foreach (var body in bodies)
                {
                    if (body.ParentId == target.PlayerId)
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.BaitReport, SendOption.Reliable, -1);
                        writer.Write(killer.PlayerId);
                        writer.Write(target.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        break;
                    }
                }
            }
        }

        public static IEnumerator FlashCoroutine(Color color)
        {
            var waitfor = 1f;
            var alpha = 0.3f;
            color.a = alpha;

            if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
            {
                var fullscreen = DestroyableSingleton<HudManager>.Instance.FullScreen;
                fullscreen.enabled = true;
                fullscreen.gameObject.active = true;
                fullscreen.color = color;
            }

            yield return new WaitForSeconds(waitfor);

            if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
            {
                var fullscreen = DestroyableSingleton<HudManager>.Instance.FullScreen;

                if (fullscreen.color.Equals(color))
                {
                    fullscreen.color = new Color(1f, 0f, 0f, 0.3f);
                    fullscreen.enabled = false;
                }
            }
        }

        public static IEnumerable<(T1, T2)> Zip<T1, T2>(List<T1> first, List<T2> second)
        {
            return first.Zip(second, (x, y) => (x, y));
        }

        public static void DestroyAll(this IEnumerable<Component> listie)
        {
            foreach (var item in listie)
            {
                if (item == null)
                    continue;

                Object.Destroy(item);

                if (item.gameObject == null)
                    return;

                Object.Destroy(item.gameObject);
            }
        }

        public static void EndGame(GameOverReason reason = GameOverReason.ImpostorByVote, bool showAds = false)
        {
            ShipStatus.RpcEndGame(reason, showAds);
        }

        [HarmonyPatch(typeof(MedScanMinigame), nameof(MedScanMinigame.FixedUpdate))]
        class MedScanMinigameFixedUpdatePatch
        {
            static void Prefix(MedScanMinigame __instance)
            {
                if (CustomGameOptions.ParallelMedScans)
                {
                    //Allows multiple medbay scans at once
                    __instance.medscan.CurrentUser = PlayerControl.LocalPlayer.PlayerId;
                    __instance.medscan.UsersList.Clear();
                }
            }
        }
      
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
        class StartMeetingPatch
        {
            public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)]GameData.PlayerInfo meetingTarget)
            {
                voteTarget = meetingTarget;
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        class MeetingHudUpdatePatch
        {
            static void Postfix(MeetingHud __instance)
            {
                //Deactivate skip Button if skipping on emergency meetings is disabled 
                if ((voteTarget == null && CustomGameOptions.SkipButtonDisable == DisableSkipButtonMeetings.Emergency) || (CustomGameOptions.SkipButtonDisable == DisableSkipButtonMeetings.Always))
                    __instance.SkipVoteButton.gameObject.SetActive(false);
            }
        }

        //Submerged utils
        public static object TryCast(this Il2CppObjectBase self, Type type)
        {
            return AccessTools.Method(self.GetType(), nameof(Il2CppObjectBase.TryCast)).MakeGenericMethod(type).Invoke(self, Array.Empty<object>());
        }

        public static IList createList(Type myType)
        {
            Type genericListType = typeof(List<>).MakeGenericType(myType);
            return (IList)Activator.CreateInstance(genericListType);
        }

        public static void ResetCustomTimers()
        {
            #region Crew Roles
            foreach (Medium role in Role.GetRoles(RoleEnum.Medium))
                role.LastMediated = DateTime.UtcNow;

            foreach (Sheriff role in Role.GetRoles(RoleEnum.Sheriff))
                role.LastInterrogated = DateTime.UtcNow;

            foreach (Inspector role in Role.GetRoles(RoleEnum.Inspector))
                role.LastInspected = DateTime.UtcNow;

            foreach (Vigilante role in Role.GetRoles(RoleEnum.Vigilante))
                role.LastKilled = DateTime.UtcNow;

            foreach (TimeLord role in Role.GetRoles(RoleEnum.TimeLord))
            {
                role.StartRewind = DateTime.UtcNow.AddSeconds(-10.0f);
                role.FinishRewind = DateTime.UtcNow;
            }

            foreach (Tracker role in Role.GetRoles(RoleEnum.Tracker))
                role.LastTracked = DateTime.UtcNow;

            foreach (Escort role in Role.GetRoles(RoleEnum.Escort))
                role.LastBlock = DateTime.UtcNow;

            foreach (Transporter role in Role.GetRoles(RoleEnum.Transporter))
                role.LastTransported = DateTime.UtcNow;

            foreach (Veteran role in Role.GetRoles(RoleEnum.Veteran))
                role.LastAlerted = DateTime.UtcNow;

            foreach (Operative role in Role.GetRoles(RoleEnum.Operative))
                role.lastBugged = DateTime.UtcNow;

            foreach (Detective role in Role.GetRoles(RoleEnum.Detective))
                role.LastExamined = DateTime.UtcNow;

            foreach (Shifter role in Role.GetRoles(RoleEnum.Shifter))
                role.LastShifted = DateTime.UtcNow;

            foreach (VampireHunter role in Role.GetRoles(RoleEnum.VampireHunter))
                role.LastStaked = DateTime.UtcNow;
            #endregion

            #region Neutral Roles
            foreach (Survivor role in Role.GetRoles(RoleEnum.Survivor))
                role.LastVested = DateTime.UtcNow;

            foreach (GuardianAngel role in Role.GetRoles(RoleEnum.GuardianAngel))
                role.LastProtected = DateTime.UtcNow;

            foreach (Arsonist role in Role.GetRoles(RoleEnum.Arsonist))
            {
                role.LastDoused = DateTime.UtcNow;
                role.LastIgnited = DateTime.UtcNow;
            }

            foreach (Glitch role in Role.GetRoles(RoleEnum.Glitch))
            {
                role.LastHack = DateTime.UtcNow;
                role.LastKill = DateTime.UtcNow;
                role.LastMimic = DateTime.UtcNow;
            }

            foreach (Juggernaut role in Role.GetRoles(RoleEnum.Juggernaut))
                role.LastKill = DateTime.UtcNow;

            foreach (Werewolf role in Role.GetRoles(RoleEnum.Werewolf))
                role.LastMauled = DateTime.UtcNow;

            foreach (Murderer role in Role.GetRoles(RoleEnum.Murderer))
                role.LastKill = DateTime.UtcNow;

            foreach (SerialKiller role in Role.GetRoles(RoleEnum.SerialKiller))
            {
                role.LastLusted = DateTime.UtcNow;
                role.LastKilled = DateTime.UtcNow;
            }

            foreach (Cryomaniac role in Role.GetRoles(RoleEnum.Cryomaniac))
                role.LastDoused = DateTime.UtcNow;

            foreach (Plaguebearer role in Role.GetRoles(RoleEnum.Plaguebearer))
                role.LastInfected = DateTime.UtcNow;

            foreach (Pestilence role in Role.GetRoles(RoleEnum.Pestilence))
                role.LastKill = DateTime.UtcNow;

            foreach (Cannibal role in Role.GetRoles(RoleEnum.Cannibal))
                role.LastEaten = DateTime.UtcNow;

            foreach (Dracula role in Role.GetRoles(RoleEnum.Dracula))
                role.LastBitten = DateTime.UtcNow;

            foreach (Dampyr role in Role.GetRoles(RoleEnum.Dampyr))
                role.LastKill = DateTime.UtcNow;

            foreach (Thief role in Role.GetRoles(RoleEnum.Thief))
                role.LastKilled = DateTime.UtcNow;

            foreach (Troll role in Role.GetRoles(RoleEnum.Troll))
                role.LastInteracted = DateTime.UtcNow;
            #endregion

            #region Intruder Roles
            foreach (Blackmailer role in Role.GetRoles(RoleEnum.Blackmailer))
                role.LastBlackmailed = DateTime.UtcNow;

            foreach (Teleporter role in Role.GetRoles(RoleEnum.Teleporter))
                role.LastTeleport = DateTime.UtcNow;

            foreach (Grenadier role in Role.GetRoles(RoleEnum.Grenadier))
                role.LastFlashed = DateTime.UtcNow;

            foreach (Miner role in Role.GetRoles(RoleEnum.Miner))
                role.LastMined = DateTime.UtcNow;

            foreach (Morphling role in Role.GetRoles(RoleEnum.Morphling))
                role.LastMorphed = DateTime.UtcNow;

            foreach (Poisoner role in Role.GetRoles(RoleEnum.Poisoner))
                role.LastPoisoned = DateTime.UtcNow;

            foreach (Consort role in Role.GetRoles(RoleEnum.Consort))
                role.LastBlock = DateTime.UtcNow;

            foreach (Wraith role in Role.GetRoles(RoleEnum.Wraith))
                role.LastInvis = DateTime.UtcNow;

            foreach (Janitor role in Role.GetRoles(RoleEnum.Janitor))
                role.LastCleaned = DateTime.UtcNow;

            foreach (Undertaker role in Role.GetRoles(RoleEnum.Undertaker))
                role.LastDragged = DateTime.UtcNow;

            foreach (Disguiser role in Role.GetRoles(RoleEnum.Disguiser))
                role.LastDisguised = DateTime.UtcNow;

            foreach (TimeMaster role in Role.GetRoles(RoleEnum.TimeMaster))
                role.LastFrozen = DateTime.UtcNow;

            foreach (Consigliere role in Role.GetRoles(RoleEnum.Consigliere))
                role.LastInvestigated = DateTime.UtcNow;

            foreach (Camouflager role in Role.GetRoles(RoleEnum.Camouflager))
                role.LastCamouflaged = DateTime.UtcNow;

            foreach (Consort role in Role.GetRoles(RoleEnum.Consort))
                role.LastBlock = DateTime.UtcNow;
            #endregion

            #region Syndicate Roles
            foreach (Anarchist role in Role.GetRoles(RoleEnum.Anarchist))
                role.LastKill = DateTime.UtcNow;

            foreach (Concealer role in Role.GetRoles(RoleEnum.Concealer))
                role.LastConcealed = DateTime.UtcNow;

            foreach (Gorgon role in Role.GetRoles(RoleEnum.Gorgon))
                role.LastGazed = DateTime.UtcNow;

            foreach (Shapeshifter role in Role.GetRoles(RoleEnum.Shapeshifter))
                role.LastShapeshifted = DateTime.UtcNow;

            foreach (Warper role in Role.GetRoles(RoleEnum.Warper))
                role.LastWarped = DateTime.UtcNow;

            foreach (Framer role in Role.GetRoles(RoleEnum.Framer))
                role.LastFramed = DateTime.UtcNow;
            #endregion

            #region Objectifiers
            foreach (Corrupted corr in Objectifier.GetObjectifiers(ObjectifierEnum.Corrupted))
                corr.LastKilled = DateTime.UtcNow;
            #endregion
        }
        
        public static void AirKill(PlayerControl player, PlayerControl target)
        {
            Vector3 vector = target.transform.position;
            vector.z = vector.y / 1000f;
            player.transform.position = vector;
            player.NetTransform.SnapTo(vector);
        }

        public static PlayerControl SetClosestPlayerToPlayer(PlayerControl fromPlayer, ref PlayerControl closestPlayer, float maxDistance = float.NaN, List<PlayerControl> targets = null)
        {
            if (float.IsNaN(maxDistance)) 
                maxDistance = GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];
                
            var player = GetClosestPlayer(fromPlayer, targets ?? PlayerControl.AllPlayerControls.ToArray().ToList());
            var closeEnough = player == null || (GetDistBetweenPlayers(fromPlayer, player) < maxDistance);
            return closestPlayer = closeEnough ? player : null;
        }

        public static bool LastImp()
        {
            return PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(Faction.Intruder) && !(x.Data.IsDead || x.Data.Disconnected)) == 1;
        }

        public static bool LastCrew()
        {
            return PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(Faction.Crew) && !(x.Data.IsDead || x.Data.Disconnected)) == 1;
        }

        public static bool CrewDead()
        {
            return PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(Faction.Crew) && !(x.Data.IsDead || x.Data.Disconnected)) == 0;
        }

        public static bool ImpsDead()
        {
            return PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(Faction.Intruder) && !(x.Data.IsDead || x.Data.Disconnected)) == 0;
        }

        public static bool LastNeut()
        {
            return PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(Faction.Neutral) && !(x.Data.IsDead || x.Data.Disconnected)) == 1;
        }

        public static bool LastSyn()
        {
            return PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(Faction.Syndicate) && !(x.Data.IsDead || x.Data.Disconnected)) == 1;
        }

        public static bool LastNonCrew()
        {
            return LastImp() && LastNeut() && LastSyn();
        }

        public static IEnumerator Block(PlayerControl blocker, PlayerControl target)
        {
            var tickDictionary = new Dictionary<byte, DateTime>();
            GameObject[] lockImg = {null, null, null, null};

            if (!(blocker.Is(RoleEnum.Glitch) || blocker.Is(RoleEnum.Escort) || blocker.Is(RoleEnum.Consort)))
                yield break;
            
            if (target.Is(RoleEnum.SerialKiller))
            {
                RpcMurderPlayer(target, blocker);
                yield break;
            }

            if (tickDictionary.ContainsKey(target.PlayerId))
            {
                tickDictionary[target.PlayerId] = DateTime.UtcNow;
                yield break;
            }

            tickDictionary.Add(target.PlayerId, DateTime.UtcNow);

            while (true)
            {
                if (PlayerControl.LocalPlayer == target)
                {
                    if (HudManager.Instance.KillButton != null)
                    {
                        if (lockImg[0] == null)
                        {
                            lockImg[0] = new GameObject();
                            var lockImgR = lockImg[0].AddComponent<SpriteRenderer>();
                            lockImgR.sprite = LockSprite;
                        }

                        lockImg[0].layer = 5;
                        lockImg[0].transform.position = new Vector3(HudManager.Instance.KillButton.transform.position.x, HudManager.Instance.KillButton.transform.position.y, -50f);
                        HudManager.Instance.KillButton.enabled = false;
                        HudManager.Instance.KillButton.graphic.color = Palette.DisabledClear;
                        HudManager.Instance.KillButton.graphic.material.SetFloat("_Desat", 1f);
                    }

                    if (HudManager.Instance.UseButton != null)
                    {
                        if (lockImg[1] == null)
                        {
                            lockImg[1] = new GameObject();
                            var lockImgR = lockImg[1].AddComponent<SpriteRenderer>();
                            lockImgR.sprite = LockSprite;
                        }

                        lockImg[1].transform.position = new Vector3(HudManager.Instance.UseButton.transform.position.x, HudManager.Instance.UseButton.transform.position.y, -50f);
                        lockImg[1].layer = 5;
                        HudManager.Instance.UseButton.enabled = false;
                        HudManager.Instance.UseButton.graphic.color = Palette.DisabledClear;
                        HudManager.Instance.UseButton.graphic.material.SetFloat("_Desat", 1f);
                    }

                    if (HudManager.Instance.ReportButton != null)
                    {
                        if (lockImg[2] == null)
                        {
                            lockImg[2] = new GameObject();
                            var lockImgR = lockImg[2].AddComponent<SpriteRenderer>();
                            lockImgR.sprite = LockSprite;
                        }

                        lockImg[2].transform.position = new Vector3(HudManager.Instance.ReportButton.transform.position.x, HudManager.Instance.ReportButton.transform.position.y, -50f);
                        lockImg[2].layer = 5;
                        HudManager.Instance.ReportButton.enabled = false;
                        HudManager.Instance.ReportButton.SetActive(false);
                    }

                    var role = Role.GetRole(PlayerControl.LocalPlayer);

                    if (role != null)
                    {
                        if (role.ExtraButtons.Count > 0)
                        {
                            if (lockImg[3] == null)
                            {
                                lockImg[3] = new GameObject();
                                var lockImgR = lockImg[3].AddComponent<SpriteRenderer>();
                                lockImgR.sprite = LockSprite;
                            }

                            lockImg[3].transform.position = new Vector3(role.ExtraButtons[0].transform.position.x, role.ExtraButtons[0].transform.position.y, -50f);
                            lockImg[3].layer = 5;
                            role.ExtraButtons[0].enabled = false;
                            role.ExtraButtons[0].graphic.color = Palette.DisabledClear;
                            role.ExtraButtons[0].graphic.material.SetFloat("_Desat", 1f);
                        }
                    }

                    if (Minigame.Instance)
                    {
                        Minigame.Instance.Close();
                        Minigame.Instance.Close();
                    }

                    if (MapBehaviour.Instance)
                    {
                        MapBehaviour.Instance.Close();
                        MapBehaviour.Instance.Close();
                    }
                }

                var totalBlocktime = (DateTime.UtcNow - tickDictionary[target.PlayerId]).TotalMilliseconds / 1000;

                if (MeetingHud.Instance || totalBlocktime > CustomGameOptions.HackDuration || target == null || target.Data.IsDead)
                {
                    foreach (var obj in lockImg)
                    {
                        if (obj != null)
                            obj.SetActive(false);
                    }

                    if (PlayerControl.LocalPlayer == target)
                    {
                        HudManager.Instance.UseButton.enabled = true;
                        HudManager.Instance.ReportButton.enabled = true;
                        HudManager.Instance.KillButton.enabled = true;
                        HudManager.Instance.UseButton.graphic.color = Palette.EnabledColor;
                        HudManager.Instance.UseButton.graphic.material.SetFloat("_Desat", 0f);
                        var role = Role.GetRole(PlayerControl.LocalPlayer);

                        if (role != null)
                        {
                            if (role.ExtraButtons.Count > 0)
                            {
                                role.ExtraButtons[0].enabled = true;
                                role.ExtraButtons[0].graphic.color = Palette.EnabledColor;
                                role.ExtraButtons[0].graphic.material.SetFloat("_Desat", 0f);
                            }
                        }
                    }

                    tickDictionary.Remove(target.PlayerId);
                    yield break;
                }

                yield return null;
            }
        }

        public static bool TasksDone()
        {
            var allCrew = new List<PlayerControl>();
            var crewWithNoTasks = new List<PlayerControl>();

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.CanDoTasks() && player.Is(Faction.Crew))
                {
                    allCrew.Add(player);
                
                    var TasksLeft = player.Data.Tasks.ToArray().Count(x => !x.Complete);

                    if (TasksLeft <= 0)
                        crewWithNoTasks.Add(player);
                }
            }

            var flag1 = allCrew.Count == crewWithNoTasks.Count;
            var flag2 = CrewDead();
            var flag = flag1 && !flag2;

            return flag;
        }

        public static Il2CppSystem.Collections.Generic.List<PlayerControl> FindClosestPlayers(PlayerControl player, float radius)
        {
            radius *= 2;
            Il2CppSystem.Collections.Generic.List<PlayerControl> playerControlList = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            Vector2 truePosition = player.GetTruePosition();
            Il2CppSystem.Collections.Generic.List<GameData.PlayerInfo> allPlayers = GameData.Instance.AllPlayers;

            for (int index = 0; index < allPlayers.Count; ++index)
            {
                GameData.PlayerInfo playerInfo = allPlayers[index];

                if (!playerInfo.Disconnected)
                {
                    Vector2 vector2 = new Vector2(playerInfo.Object.GetTruePosition().x - truePosition.x, playerInfo.Object.GetTruePosition().y -
                        truePosition.y);
                    float magnitude = ((Vector2)vector2).magnitude;

                    if (magnitude <= radius)
                    {
                        PlayerControl playerControl = playerInfo.Object;
                        playerControlList.Add(playerControl);
                    }
                }
            }

            return playerControlList;
        }

        public static bool Sabotaged()
        {
            var result = false;

            if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.LifeSupp))
            {
                var lifeSuppSystemType = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

                if (lifeSuppSystemType.Countdown < 0f)
                    result = true;
            }

            if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Laboratory))
            {
                var reactorSystemType = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();

                if (reactorSystemType.Countdown < 0f)
                    result = true;
            }

            if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Reactor))
            {
                var reactorSystemType = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ICriticalSabotage>();

                if (reactorSystemType.Countdown < 0f)
                    result = true;
            }

            return result;
        }
    }
}
