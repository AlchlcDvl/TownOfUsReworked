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
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.CamouflagerMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Modifiers;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using Reactor.Utilities;
using Il2CppInterop.Runtime.InteropTypes;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.SerialKillerMod;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.UndertakerMod;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;
using TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.SyndicateMod;
using AmongUs.GameOptions;
using TownOfUsReworked.PlayerLayers;
using InnerNet;
using UD = TownOfUsReworked.PlayerLayers.Abilities.UnderdogMod;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace TownOfUsReworked.Classes
{
    [HarmonyPatch]
    public static class Utils
    {
        public static Dictionary<PlayerControl, Color> oldColors = new Dictionary<PlayerControl, Color>();
        public static List<WinningPlayerData> potentialWinners = new List<WinningPlayerData>();
		public static Dictionary<byte, DateTime> tickDictionary = new Dictionary<byte, DateTime>();

        public static TMPro.TextMeshPro nameText(this PlayerControl p) => p.cosmetics.nameText;

        public static TMPro.TextMeshPro NameText(this PoolablePlayer p) => p.cosmetics.nameText;

        public static UnityEngine.SpriteRenderer myRend(this PlayerControl p) => p.cosmetics.currentBodySprite.BodySprite;

        public static KeyValuePair<byte, int> MaxPair(this Dictionary<byte, int> self, out bool tie)
        {
            tie = true;
            var result = new KeyValuePair<byte, int>(byte.MaxValue, int.MinValue);

            foreach (var keyValuePair in self)
            {
                if (keyValuePair.Value > result.Value)
                {
                    result = keyValuePair;
                    tie = false;
                }
                else if (keyValuePair.Value == result.Value)
                    tie = true;
            }

            return result;
        }

        public static KeyValuePair<byte, int> MaxPair(this byte[] self, out bool tie)
        {
            tie = true;
            var result = new KeyValuePair<byte, int>(byte.MaxValue, int.MinValue);

            for (byte i = 0; i < self.Length; i++)
            {
                if (self[i] > result.Value)
                {
                    result = new KeyValuePair<byte, int>(i, self[i]);
                    tie = false;
                }
                else if (self[i] == result.Value)
                    tie = true;
            }
            
            return result;
        }

        public static VisualAppearance GetDefaultAppearance(this PlayerControl player)
        {
            return new VisualAppearance();
        }

        public static bool TryGetAppearance(this PlayerControl player, IVisualAlteration modifier, out VisualAppearance appearance)
        {
            if (modifier != null)
                return modifier.TryGetModifiedAppearance(out appearance);

            appearance = player.GetDefaultAppearance();
            return false;
        }

        public static VisualAppearance GetAppearance(this PlayerControl player)
        {
            if (player.TryGetAppearance(Role.GetRole(player) as IVisualAlteration, out var appearance))
                return appearance;
            else if (player.TryGetAppearance(Modifier.GetModifier(player) as IVisualAlteration, out appearance))
                return appearance;
            else
                return player.GetDefaultAppearance();
        }

        public static bool IsImpostor(this GameData.PlayerInfo playerinfo)
        {
            return playerinfo?.Role?.TeamType == RoleTeamTypes.Impostor;
        }

        public static GameData.PlayerOutfit GetDefaultOutfit(this PlayerControl playerControl)
        {
            return playerControl.Data.DefaultOutfit;
        }

        public static void SetOutfit(this PlayerControl playerControl, CustomPlayerOutfitType CustomOutfitType, GameData.PlayerOutfit outfit)
        {
            playerControl.Data.SetOutfit((PlayerOutfitType)CustomOutfitType, outfit);
            playerControl.SetOutfit(CustomOutfitType);
        }

        public static void SetOutfit(this PlayerControl playerControl, CustomPlayerOutfitType CustomOutfitType)
        {
            if (playerControl == null)
                return;

            var outfitType = (PlayerOutfitType)CustomOutfitType;
            
            if (!playerControl.Data.Outfits.ContainsKey(outfitType))
                return;
            
            var newOutfit = playerControl.Data.Outfits[outfitType];
            playerControl.CurrentOutfitType = outfitType;
            playerControl.RawSetName(newOutfit.PlayerName);
            playerControl.RawSetColor(newOutfit.ColorId);
            playerControl.RawSetHat(newOutfit.HatId, newOutfit.ColorId);
            playerControl.RawSetVisor(newOutfit.VisorId, newOutfit.ColorId);
            playerControl.RawSetPet(newOutfit.PetId, newOutfit.ColorId);
            playerControl.RawSetSkin(newOutfit.SkinId, newOutfit.ColorId);
            playerControl.cosmetics.colorBlindText.color = Color.white;

            if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started)
            {
                if (PlayerControl.LocalPlayer.Is(Faction.Intruder) && playerControl.Is(Faction.Intruder))
                    playerControl.nameText().color = Colors.Intruder;

                if (PlayerControl.LocalPlayer.Is(Faction.Syndicate) && playerControl.Is(Faction.Syndicate))
                    playerControl.nameText().color = Colors.Syndicate;

                if (PlayerControl.LocalPlayer.Is(SubFaction.Undead) && playerControl.Is(SubFaction.Undead))
                    playerControl.nameText().color = Colors.Undead;

                if (PlayerControl.LocalPlayer.Is(SubFaction.Cabal) && playerControl.Is(SubFaction.Cabal))
                    playerControl.nameText().color = Colors.Cabal;
            }
            else
                playerControl.nameText().color = Color.white;
        }

        public static CustomPlayerOutfitType GetCustomOutfitType(this PlayerControl playerControl)
        {
            return (CustomPlayerOutfitType)playerControl.CurrentOutfitType;
        }

        public static bool IsNullOrDestroyed(this Object obj)
        {

            if (object.ReferenceEquals(obj, null))
                return true;

            if (obj is UnityEngine.Object)
                return (obj as UnityEngine.Object) == null;

            return false;
        }

        public static Texture2D CreateEmptyTexture(int width = 0, int height = 0)
        {
            return new Texture2D(width, height, TextureFormat.RGBA32, Texture.GenerateAllMips, false, IntPtr.Zero);
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

        public static void Camouflage()
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage && player.GetCustomOutfitType() != CustomPlayerOutfitType.Invis &&
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
                DefaultOutfit(player);
                player.myRend().color = new Color32(255, 255, 255, 255);
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

        public static bool Is(this PlayerControl player, Faction faction)
        {
            return Role.GetRole(player)?.Faction == faction;
        }

        public static bool Is(this PlayerControl player, RoleAlignment alignment)
        {
            return Role.GetRole(player)?.RoleAlignment == alignment;
        }

        /*public static bool Is(this PlayerControl player, AttackEnum attack)
        {
            return Role.GetRole(player)?.Attack == attack;
        }

        public static bool Is(this PlayerControl player, DefenseEnum defense)
        {
            return Role.GetRole(player)?.Defense == defense;
        }*/

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

        public static bool HasObjectifier(this PlayerControl player)
        {
            if (player == null)
                return false;
                
            var role = Objectifier.GetObjectifier(player);

            if (role == null)
                return false;
            else
                return true;
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

        public static bool IsPersuaded(this PlayerControl player)
        {
            if (player == null)
                return false;
                
            var role = Role.GetRole(player);

            if (role == null)
                return false;

            return role.IsPersuaded;
        }

        public static bool IsResurrected(this PlayerControl player)
        {
            if (player == null)
                return false;
                
            var role = Role.GetRole(player);

            if (role == null)
                return false;

            return role.IsResurrected;
        }

        public static bool NotOnTheSameSide(this PlayerControl player)
        {
            if (player == null)
                return false;
                
            var traitorFlag = player.IsTurnedTraitor();
            //var fanaticFlag = player.IsTurnedFanatic();
            var recruitFlag = player.IsRecruit();
            var sectFlag = player.IsPersuaded();
            var revivedFlag = player.IsResurrected();
            var rivalFlag = player.IsWinningRival();

            return traitorFlag || recruitFlag || sectFlag || revivedFlag || rivalFlag;
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
            
            if (Role.GetRole(player) == null)
                return !player.Data.IsImpostor();
            
            var crewflag = player.Is(Faction.Crew);
            var neutralflag = player.Is(Faction.Neutral);

            var phantomflag = player.Is(RoleEnum.Phantom);

            var loverflag = player.Is(ObjectifierEnum.Lovers);
            var rivalflag = player.Is(ObjectifierEnum.Rivals);
            var taskmasterflag = player.Is(ObjectifierEnum.Taskmaster);
            var corruptedflag = player.Is(ObjectifierEnum.Corrupted);
            var recruitflag = player.IsRecruit();

            var isdead = player.Data.IsDead;

            var flag1 = (crewflag && !(loverflag || rivalflag || corruptedflag));
            var flag2 = neutralflag && (taskmasterflag || (phantomflag && isdead));
            var flag = (flag1 || flag2) && !recruitflag; // && ((CustomGameOptions.GhostTasksCountToWin && isdead) || (!CustomGameOptions.GhostTasksCountToWin && !isdead))

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

        /*public static DefenseEnum GetDefense(PlayerControl player)
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
        }*/

        public static PlayerControl PlayerById(byte id)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.PlayerId == id)
                    return player;
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
            var vetFlag = Role.GetRoles(RoleEnum.Veteran).Any(role =>
            {
                var veteran = (Veteran)role;
                return veteran != null && veteran.OnAlert && player == veteran.Player;
            });

            var retFlag = Role.GetRoles(RoleEnum.Retributionist).Any(role =>
            {
                var retributionist = (Retributionist)role;
                return retributionist != null && retributionist.OnAlert && player == retributionist.Player;
            });

            return vetFlag || retFlag;
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

        public static bool IsTurnedFanatic(this PlayerControl player)
        {
            return player.IsIntFanatic() || player.IsSynFanatic();
        }

        public static bool IsIntFanatic(this PlayerControl player)
        {
            if (player == null || player.Data == null)
                return false;

            if (!player.Is(ObjectifierEnum.Fanatic))
                return false;
            
            var traitor = Role.GetRole(player);
            return traitor.IsIntFanatic;
        }

        public static bool IsSynFanatic(this PlayerControl player)
        {
            if (player == null || player.Data == null)
                return false;

            if (!player.Is(ObjectifierEnum.Fanatic))
                return false;
            
            var traitor = Role.GetRole(player);
            return traitor.IsSynFanatic;
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
                x.Is(ObjectifierEnum.Corrupted) || !x.Is(SubFaction.None))) == 0) || TasksDone();

            return flag;
        }

        public static bool IntrudersWin()
        {
            var flag = (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.IsRecruit() || x.Is(ObjectifierEnum.Lovers) ||
                x.Is(Faction.Crew) || x.Is(RoleAlignment.NeutralKill) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralNeo) || x.Is(RoleAlignment.NeutralPros) ||
                x.IsWinningRival() || !x.Is(SubFaction.None))) == 0) || Sabotaged();

            return flag;
        }

        public static bool AllNeutralsWin()
        {
            var flag = (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(ObjectifierEnum.Lovers) || !x.Is(SubFaction.None) ||
                x.Is(Faction.Crew) || x.Is(Faction.Syndicate) || x.IsWinningRival() || x.Is(Faction.Intruder))) == 0) && CustomGameOptions.NoSolo == NoSolo.AllNeutrals;

            return flag;
        }

        public static bool NKWins(RoleEnum nk)
        {
            var flag = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || (x.Is(RoleAlignment.NeutralKill) &&
                !x.Is(nk)) || x.Is(RoleAlignment.NeutralNeo) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralPros) || x.Is(Faction.Crew) || x.Is(ObjectifierEnum.Lovers) ||
                x.IsWinningRival() || !x.Is(SubFaction.None))) == 0;

            return flag;
        }

        public static bool AllNKsWin()
        {
            var flag = (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(RoleAlignment.NeutralNeo) ||
                x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralPros) || x.Is(Faction.Crew) || x.Is(ObjectifierEnum.Lovers) || x.IsWinningRival())) == 0) &&
                CustomGameOptions.NoSolo == NoSolo.AllNKs;

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

            var lover = Objectifier.GetObjectifierValue<Lovers>(obj);
            var flag2 = lover.OtherLover != null && lover.Player != null && !lover.OtherLover.Data.IsDead && !lover.Player.Data.IsDead && !lover.OtherLover.Data.Disconnected &&
                !lover.Player.Data.Disconnected;

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
            var flag = (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(RoleAlignment.NeutralKill) || !x.Is(SubFaction.None) ||
                x.Is(Faction.Intruder) || x.Is(RoleAlignment.NeutralNeo) || x.Is(Faction.Crew) || x.Is(RoleAlignment.NeutralPros) || x.Is(ObjectifierEnum.Lovers) ||
                x.IsWinningRival())) == 0) || (CustomGameOptions.AltImps && Sabotaged());

            return flag;
        }

        public static bool UndeadWin()
        {
            var flag = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(RoleAlignment.NeutralKill) ||
                (x.Is(RoleAlignment.NeutralNeo) && !x.Is(RoleEnum.Dracula)) || x.Is(Faction.Syndicate) || x.Is(Faction.Crew) || x.IsRecruit() || x.IsPersuaded() || x.IsResurrected() ||
                x.Is(ObjectifierEnum.Lovers) || (x.Is(RoleAlignment.NeutralPros) && !(x.Is(RoleEnum.Dampyr) || x.Is(RoleEnum.Vampire))) || x.IsWinningRival())) == 0;

            return flag;
        }

        public static bool CabalWin()
        {
            var flag = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(RoleAlignment.NeutralKill) ||
                (x.Is(RoleAlignment.NeutralNeo) && !x.Is(RoleEnum.Jackal)) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralPros) || x.Is(Faction.Crew) ||
                x.Is(ObjectifierEnum.Lovers) || x.IsWinningRival()) && !x.IsRecruit()) == 0;

            return flag;
        }

        public static bool SectWin()
        {
            var flag = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(RoleAlignment.NeutralKill) ||
                (x.Is(RoleAlignment.NeutralNeo) && !x.Is(RoleEnum.Whisperer)) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralPros) || x.Is(Faction.Crew) ||
                x.Is(ObjectifierEnum.Lovers) || x.IsWinningRival() || x.IsRecruit() || x.IsResurrected()) && !x.IsPersuaded()) == 0;

            return flag;
        }

        public static bool ReanimatedWin()
        {
            var flag = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(RoleAlignment.NeutralKill) ||
                (x.Is(RoleAlignment.NeutralNeo) && !x.Is(RoleEnum.Necromancer)) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralPros) || x.Is(Faction.Crew) ||
                x.Is(ObjectifierEnum.Lovers) || x.IsWinningRival() || x.IsPersuaded() || x.IsRecruit()) && !x.IsResurrected()) == 0;

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
                if (player.Data.IsDead || player.PlayerId == refPlayer.PlayerId || !player.Collider.enabled || player.inMovingPlat || (player.inVent && CustomGameOptions.VentTargetting))
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

        public static void SetTarget(ref PlayerControl closestPlayer, KillButton button, List<PlayerControl> targets = null)
        {
            if (!button.isActiveAndEnabled)
                return;
            
            var target = SetClosestPlayer(ref closestPlayer, targets);

            if (target != null)
                button.SetTarget(target);
        }

        public static PlayerControl SetClosestPlayer(ref PlayerControl closestPlayer, List<PlayerControl> targets = null)
        {
            var maxDistance = GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];

            if (targets == null)
                targets = PlayerControl.AllPlayerControls.ToArray().ToList();

            var player = GetClosestPlayer(PlayerControl.LocalPlayer, targets.Where(x => !x.inVent || (x.inVent && CustomGameOptions.VentTargetting)).ToList());
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

        public static double GetDistBetweenPlayers(PlayerControl player, DeadBody refplayer)
        {
            if (player == null || refplayer == null)
                return 100000000000000;

            var truePosition = refplayer.TruePosition;
            var truePosition2 = player.GetTruePosition();
            return Vector2.Distance(truePosition, truePosition2);
        }

        public static void RpcMurderPlayer(PlayerControl killer, PlayerControl target)
        {
            MurderPlayer(killer, target);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
            writer.Write((byte)ActionsRPC.BypassKill);
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
                        SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 1f);
                    } catch {}
                }

                target.gameObject.layer = LayerMask.NameToLayer("Ghost");
                target.Visible = false;

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Coroner) && !PlayerControl.LocalPlayer.Data.IsDead)
                    Coroutines.Start(FlashCoroutine(Colors.Coroner));

                if (PlayerControl.LocalPlayer.Data.IsDead)
                    Coroutines.Start(FlashCoroutine(Colors.Stalemate));

                var targetRole = Role.GetRole(target);
                var killerRole = Role.GetRole(killer);

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

                    if (!GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks)
                    {
                        for (var i = 0; i < target.myTasks.Count; i++)
                        {
                            var playerTask = target.myTasks.ToArray()[i];
                            playerTask.OnRemove();
                            Object.Destroy(playerTask.gameObject);
                        }

                        target.myTasks.Clear();
                        importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GhostIgnoreTasks, new
                            Il2CppReferenceArray<Il2CppSystem.Object>(0));
                    }
                    else
                        importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(StringNames.GhostDoTasks, new Il2CppReferenceArray<Il2CppSystem.Object>(0));

                    target.myTasks.Insert(0, importantTextTask);
                }

                if (!killer.Is(RoleEnum.Poisoner) && !killer.Is(RoleEnum.Arsonist) && !killer.Is(RoleEnum.Gorgon) && !killer.Is(RoleEnum.Jester))
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

                var deadBody = new DeadPlayer
                {
                    PlayerId = target.PlayerId,
                    KillerId = killer.PlayerId,
                    KillTime = DateTime.UtcNow
                };

                Murder.KilledPlayers.Add(deadBody);

                if (target.Is(RoleEnum.Troll))
                {
                    var troll = (Troll)targetRole;
                    troll.Killed = true;
                    return;
                }

                if (!killer.AmOwner)
                    return;

                if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.SerialKiller))
                {
                    var sk = Role.GetRole<SerialKiller>(killer);
                    sk.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.LustKillCd);
                    sk.Player.SetKillTimer(CustomGameOptions.LustKillCd * CustomGameOptions.DiseasedMultiplier);
                }
                else if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Glitch))
                {
                    var glitch = Role.GetRole<Glitch>(killer);
                    glitch.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.GlitchKillCooldown);
                    glitch.Player.SetKillTimer(CustomGameOptions.GlitchKillCooldown * CustomGameOptions.DiseasedMultiplier);
                }
                else if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Juggernaut))
                {
                    var juggernaut = Role.GetRole<Juggernaut>(killer);
                    juggernaut.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * (CustomGameOptions.JuggKillCooldown -
                        CustomGameOptions.JuggKillBonus * juggernaut.JuggKills));
                    juggernaut.Player.SetKillTimer((CustomGameOptions.JuggKillCooldown + 5.0f - CustomGameOptions.JuggKillBonus * juggernaut.JuggKills) *
                        CustomGameOptions.DiseasedMultiplier);
                }
                else if (target.Is(ModifierEnum.Diseased) && killer.Is(AbilityEnum.Underdog))
                {
                    var cooldown = killer.Is(Faction.Intruder) ? CustomGameOptions.IntKillCooldown : CustomGameOptions.ChaosDriveKillCooldown;
                    var last = killer.Is(Faction.Intruder) ? LastImp() : LastSyn();
                    var lowerKC = (cooldown - CustomGameOptions.UnderdogKillBonus) * CustomGameOptions.DiseasedMultiplier;
                    var normalKC = cooldown * CustomGameOptions.DiseasedMultiplier;
                    var upperKC = (cooldown + CustomGameOptions.UnderdogKillBonus) * CustomGameOptions.DiseasedMultiplier;
                    var role = Role.GetRole(killer);

                    switch (role.Faction)
                    {  
                        case Faction.Syndicate:
                            switch (role.RoleType)
                            {
                                case RoleEnum.Anarchist:
                                    var role2 = (Anarchist)role;
                                    role2.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.ChaosDriveKillCooldown);
                                    break;
                                case RoleEnum.Concealer:
                                    var role3 = (Concealer)role;
                                    role3.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.ChaosDriveKillCooldown);
                                    break;
                                case RoleEnum.Framer:
                                    var role4 = (Framer)role;
                                    role4.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.ChaosDriveKillCooldown);
                                    break;
                                case RoleEnum.Poisoner:
                                    var role5 = (Poisoner)role;
                                    role5.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.ChaosDriveKillCooldown);
                                    break;
                                case RoleEnum.Shapeshifter:
                                    var role6 = (Shapeshifter)role;
                                    role6.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.ChaosDriveKillCooldown);
                                    break;
                                case RoleEnum.Bomber:
                                    var role7 = (Bomber)role;
                                    role7.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.ChaosDriveKillCooldown);
                                    break;
                                case RoleEnum.Gorgon:
                                    var role8 = (Gorgon)role;
                                    role8.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.ChaosDriveKillCooldown);
                                    break;
                                case RoleEnum.Rebel:
                                    var role10 = (Rebel)role;
                                    role10.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.ChaosDriveKillCooldown);
                                    break;
                                case RoleEnum.Warper:
                                    var role11 = (Warper)role;
                                    role11.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.ChaosDriveKillCooldown);
                                    break;
                                case RoleEnum.Sidekick:
                                    var role12 = (Sidekick)role;
                                    role12.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.ChaosDriveKillCooldown);
                                    break;
                            }

                            break;
                    }

                    killer.SetKillTimer(last ? lowerKC : (UD.PerformKill.IncreasedKC() ? normalKC : upperKC));
                }
                else if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Werewolf))
                {
                    var pest = Role.GetRole<Pestilence>(killer);
                    pest.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.PestKillCd);
                    pest.Player.SetKillTimer(CustomGameOptions.PestKillCd * CustomGameOptions.DiseasedMultiplier);
                }
                else if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Murderer))
                {
                    var pest = Role.GetRole<Pestilence>(killer);
                    pest.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.PestKillCd);
                    pest.Player.SetKillTimer(CustomGameOptions.PestKillCd * CustomGameOptions.DiseasedMultiplier);
                }
                else if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Vigilante))
                {
                    var pest = Role.GetRole<Pestilence>(killer);
                    pest.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.PestKillCd);
                    pest.Player.SetKillTimer(CustomGameOptions.PestKillCd * CustomGameOptions.DiseasedMultiplier);
                }
                else if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Dampyr))
                {
                    var pest = Role.GetRole<Pestilence>(killer);
                    pest.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.PestKillCd);
                    pest.Player.SetKillTimer(CustomGameOptions.PestKillCd * CustomGameOptions.DiseasedMultiplier);
                }
                else if (target.Is(ModifierEnum.Diseased) && killer.Is(Faction.Intruder))
                    killer.SetKillTimer(CustomGameOptions.IntKillCooldown * CustomGameOptions.DiseasedMultiplier);
                else if (target.Is(ModifierEnum.Bait))
                    BaitReport(killer, target);
            }
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
                return "<color=#FFFFFFFF>" + playerName + "</color>";

            var role = Role.GetRole(target);
            var winString = role.ColorString + playerName + "\n" + role.Name + "</color>";
            return winString;
        }

        public static void BaitReport(PlayerControl killer, PlayerControl target)
        {
            Coroutines.Start(BaitReportDelay(killer, target));
        }

        public static IEnumerator BaitReportDelay(PlayerControl killer, PlayerControl target)
        {
            if (killer == null || target == null || killer == target)
                yield break;

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
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                        writer.Write((byte)ActionsRPC.BaitReport);
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
            GameManager.Instance.RpcEndGame(reason, showAds);
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
                role.LastBugged = DateTime.UtcNow;

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
                role.LastKilled = DateTime.UtcNow;
                role.LastMimic = DateTime.UtcNow;
            }

            foreach (Juggernaut role in Role.GetRoles(RoleEnum.Juggernaut))
                role.LastKilled = DateTime.UtcNow;

            foreach (Werewolf role in Role.GetRoles(RoleEnum.Werewolf))
                role.LastMauled = DateTime.UtcNow;

            foreach (Murderer role in Role.GetRoles(RoleEnum.Murderer))
                role.LastKilled = DateTime.UtcNow;

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
                role.LastKilled = DateTime.UtcNow;

            foreach (Cannibal role in Role.GetRoles(RoleEnum.Cannibal))
                role.LastEaten = DateTime.UtcNow;

            foreach (Dracula role in Role.GetRoles(RoleEnum.Dracula))
                role.LastBitten = DateTime.UtcNow;

            foreach (Dampyr role in Role.GetRoles(RoleEnum.Dampyr))
                role.LastKilled = DateTime.UtcNow;

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
            {
                role.LastBlock = DateTime.UtcNow;
                role.LastKilled = DateTime.UtcNow;
            }

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
                role.LastKilled = DateTime.UtcNow;

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
            return PlayerControl.AllPlayerControls.ToArray().Count(x => (x.Is(Faction.Syndicate) || x.Is(Faction.Neutral) || x.Is(Faction.Intruder)) && !(x.Data.IsDead ||
                x.Data.Disconnected)) == 1;
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

        public static bool Sabotaged()
        {
            var flag1 = false;

            if (ShipStatus.Instance.Systems != null)
            {
                if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.LifeSupp))
                {
                    var lifeSuppSystemType = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

                    if (lifeSuppSystemType.Countdown < 0f)
                        flag1 = true;
                }

                if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Laboratory))
                {
                    var reactorSystemType = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();

                    if (reactorSystemType.Countdown < 0f)
                        flag1 = true;
                }

                if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Reactor))
                {
                    var reactorSystemType = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ICriticalSabotage>();

                    if (reactorSystemType.Countdown < 0f)
                        flag1 = true;
                }
            }

            var flag2 = ImpsDead();
            var flag = flag1 && !flag2;

            return flag;
        }
        
		public static IEnumerator Block(Role blocker, PlayerControl blockPlayer)
		{
            if (IsBlockImmune(blockPlayer))
                yield break;

			GameObject[] lockImg = { null, null, null, null, null, null, null, null };

			if (tickDictionary.ContainsKey(blockPlayer.PlayerId))
			{
				tickDictionary[blockPlayer.PlayerId] = DateTime.UtcNow;
				yield break;
			}

			tickDictionary.Add(blockPlayer.PlayerId, DateTime.UtcNow);

			while (true)
            {
                if (PlayerControl.LocalPlayer == blockPlayer)
                {
                    if (HudManager.Instance.KillButton != null)
                    {
                        if (lockImg[0] == null)
                        {
                            lockImg[0] = new GameObject();
                            var lockImgR = lockImg[0].AddComponent<SpriteRenderer>();
                            lockImgR.sprite = TownOfUsReworked.Lock;
                        }

                        lockImg[0].layer = 5;
                        lockImg[0].transform.position = new Vector3(HudManager.Instance.KillButton.transform.position.x, HudManager.Instance.KillButton.transform.position.y, -50f);
                        HudManager.Instance.KillButton.enabled = false;
                        HudManager.Instance.KillButton.graphic.color = Palette.DisabledClear;
                        HudManager.Instance.KillButton.graphic.material.SetFloat("_Desat", 1f);
                    }

                    if (HudManager.Instance.UseButton != null || HudManager.Instance.PetButton != null)
                    {
                        if (lockImg[1] == null)
                        {
                            lockImg[1] = new GameObject();
                            var lockImgR = lockImg[1].AddComponent<SpriteRenderer>();
                            lockImgR.sprite = TownOfUsReworked.Lock;
                        }

                        if (HudManager.Instance.UseButton != null)
                        {
                            lockImg[1].transform.position = new Vector3(HudManager.Instance.UseButton.transform.position.x, HudManager.Instance.UseButton.transform.position.y, -50f);
                            lockImg[1].layer = 5;
                            HudManager.Instance.UseButton.enabled = false;
                            HudManager.Instance.UseButton.graphic.color = Palette.DisabledClear;
                            HudManager.Instance.UseButton.graphic.material.SetFloat("_Desat", 1f);
                        }
                        else
                        {
                            lockImg[1].transform.position = new Vector3(HudManager.Instance.PetButton.transform.position.x, HudManager.Instance.PetButton.transform.position.y, -50f);
                            lockImg[1].layer = 5;
                            HudManager.Instance.PetButton.enabled = false;
                            HudManager.Instance.PetButton.graphic.color = Palette.DisabledClear;
                            HudManager.Instance.PetButton.graphic.material.SetFloat("_Desat", 1f);
                        }
                    }

                    if (HudManager.Instance.ReportButton != null)
                    {
                        if (lockImg[2] == null)
                        {
                            lockImg[2] = new GameObject();
                            var lockImgR = lockImg[2].AddComponent<SpriteRenderer>();
                            lockImgR.sprite = TownOfUsReworked.Lock;
                        }

                        lockImg[2].transform.position = new Vector3(HudManager.Instance.ReportButton.transform.position.x, HudManager.Instance.ReportButton.transform.position.y, -50f);
                        lockImg[2].layer = 5;
                        HudManager.Instance.ReportButton.enabled = false;
                        HudManager.Instance.ReportButton.SetActive(false);
                    }

                    var role = Role.GetRole(blockPlayer);
                    var newNum = 4;

                    if (role != null)
                    {
                        if (role.AbilityButtons.Count > 0)
                        {
                            var j = 0;

                            for (var i = 3; i < role.AbilityButtons.Count + 3; i++)
                            {
                                if (lockImg[i] == null)
                                {
                                    lockImg[i] = new GameObject();
                                    var lockImgR = lockImg[i].AddComponent<SpriteRenderer>();
                                    lockImgR.sprite = TownOfUsReworked.Lock;
                                }

                                var button = role.AbilityButtons[j];
                                lockImg[i].transform.position = new Vector3(button.transform.position.x, button.transform.position.y, -50f);
                                lockImg[i].layer = 5;
                                button.enabled = false;
                                button.graphic.color = Palette.DisabledClear;
                                button.graphic.material.SetFloat("_Desat", 1f);
                                j++;
                                newNum = i;
                            }
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

                var totalHacktime = (DateTime.UtcNow - tickDictionary[blockPlayer.PlayerId]).TotalMilliseconds / 1000;
                
                if (MeetingHud.Instance || (totalHacktime > CustomGameOptions.HackDuration && blocker.RoleType == RoleEnum.Glitch) || (totalHacktime >
                    CustomGameOptions.EscRoleblockDuration && blocker.RoleType == RoleEnum.Escort) || (totalHacktime > CustomGameOptions.ConsRoleblockDuration &&
                    blocker.RoleType == RoleEnum.Consort) || blockPlayer == null || blockPlayer.Data.IsDead || blocker.Player.Data.IsDead || blockPlayer.Data.Disconnected ||
                    blocker.Player.Data.Disconnected || LobbyBehaviour.Instance)
                {
                    foreach (var obj in lockImg)
                    {
                        if (obj != null)
                            obj.SetActive(false);
                    }

                    if (PlayerControl.LocalPlayer == blockPlayer)
                    {
                        if (HudManager.Instance.UseButton != null)
                        {
                            HudManager.Instance.UseButton.enabled = true;
                            HudManager.Instance.UseButton.graphic.color = Palette.EnabledColor;
                            HudManager.Instance.UseButton.graphic.material.SetFloat("_Desat", 0f);
                        }
                        else
                        {
                            HudManager.Instance.PetButton.enabled = true;
                            HudManager.Instance.PetButton.graphic.color = Palette.EnabledColor;
                            HudManager.Instance.PetButton.graphic.material.SetFloat("_Desat", 0f);
                        }

                        HudManager.Instance.ReportButton.enabled = true;
                        HudManager.Instance.KillButton.enabled = true;
                        var role = Role.GetRole(PlayerControl.LocalPlayer);

                        if (role != null)
                        {
                            if (role.AbilityButtons.Count > 0)
                            {
                                var j = 0;

                                for (var i = 0; i < role.AbilityButtons.Count; i++)
                                {
                                    var button = role.AbilityButtons[j];
                                    button.enabled = true;
                                    button.graphic.color = Palette.EnabledColor;
                                    button.graphic.material.SetFloat("_Desat", 0f);
                                    j++;
                                }
                            }
                        }
                    }

                    tickDictionary.Remove(blockPlayer.PlayerId);
                    yield break;
                }

                yield return null;
            }
        }
        
        public static bool CanVent(PlayerControl player, GameData.PlayerInfo playerInfo)
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek)
                return false;

            if (player == null || playerInfo == null || playerInfo.IsDead || playerInfo.Disconnected || CustomGameOptions.WhoCanVent == WhoCanVentOptions.Noone || LobbyBehaviour.Instance
                || MeetingHud.Instance || player.inMovingPlat)
                return false;
            else if (player.inVent || CustomGameOptions.WhoCanVent == WhoCanVentOptions.Everyone)
                return true;

            bool mainflag = false;
            var playerRole = Role.GetRole(player);
                
            if (playerRole == null)
                mainflag = playerInfo.IsImpostor();
            else if (player.IsRecruit())
                mainflag = CustomGameOptions.RecruitVent;
            else if (player.Is(Faction.Syndicate))
                mainflag = (Role.SyndicateHasChaosDrive && CustomGameOptions.SyndicateVent == SyndicateVentOptions.ChaosDrive) || CustomGameOptions.SyndicateVent ==
                    SyndicateVentOptions.Always;
            else if (player.Is(Faction.Intruder))
            {
                if (CustomGameOptions.IntrudersVent)
                {
                    var flag = (player.Is(RoleEnum.Morphling) && CustomGameOptions.MorphlingVent) || (player.Is(RoleEnum.Wraith) && CustomGameOptions.WraithVent) ||
                        (player.Is(RoleEnum.Grenadier) && CustomGameOptions.GrenadierVent) || (player.Is(RoleEnum.Teleporter) && CustomGameOptions.TeleVent);

                    if (flag)
                        mainflag = true;
                    else if (player.Is(RoleEnum.Undertaker))
                    {
                        var undertaker = (Undertaker)playerRole;
                
                        mainflag = CustomGameOptions.UndertakerVentOptions == UndertakerOptions.Always || undertaker.CurrentlyDragging != null &&
                            CustomGameOptions.UndertakerVentOptions == UndertakerOptions.Body || undertaker.CurrentlyDragging == null &&
                            CustomGameOptions.UndertakerVentOptions == UndertakerOptions.Bodyless;
                    }
                    else
                        mainflag = true;
                }
                else
                    mainflag = false;
            }
            else if (player.Is(Faction.Crew))
            {            
                if (player.Is(AbilityEnum.Tunneler) && !player.Is(RoleEnum.Engineer))
                {
                    var tunneler = Ability.GetAbility<Tunneler>(player);
                    mainflag = tunneler.TasksDone;
                }
                else if (player.Is(RoleEnum.Engineer))
                    mainflag = true;
                else if (CustomGameOptions.CrewVent)
                    mainflag = true;
                else
                    mainflag = false;
            }
            else if (player.Is(Faction.Neutral))
            {
                var flag = (player.Is(RoleEnum.Murderer) && CustomGameOptions.MurdVent) || (player.Is(RoleEnum.Glitch) && CustomGameOptions.GlitchVent) ||
                    (player.Is(RoleEnum.Juggernaut) && CustomGameOptions.JuggVent) || (player.Is(RoleEnum.Pestilence) && CustomGameOptions.PestVent) ||
                    (player.Is(RoleEnum.Jester) && CustomGameOptions.JesterVent) || (player.Is(RoleEnum.Plaguebearer) && CustomGameOptions.PBVent) ||
                    (player.Is(RoleEnum.Arsonist) && CustomGameOptions.ArsoVent) || (player.Is(RoleEnum.Executioner) &&  CustomGameOptions.ExeVent) ||
                    (player.Is(RoleEnum.Cannibal) && CustomGameOptions.CannibalVent) || (player.Is(RoleEnum.Dracula) && CustomGameOptions.DracVent) ||
                    (player.Is(RoleEnum.Vampire) && CustomGameOptions.VampVent) || (player.Is(RoleEnum.Survivor) && CustomGameOptions.SurvVent) ||
                    (player.Is(RoleEnum.GuardianAngel) && CustomGameOptions.GAVent) || (player.Is(RoleEnum.Amnesiac) && CustomGameOptions.AmneVent) ||
                    (player.Is(RoleEnum.Werewolf) && CustomGameOptions.WerewolfVent) || (player.Is(RoleEnum.Dampyr) && CustomGameOptions.DampVent) ||
                    (player.Is(RoleEnum.Jackal) && CustomGameOptions.JackalVent);

                if (flag)
                    mainflag = flag;
                else if (player.Is(RoleEnum.SerialKiller))
                {
                    var role2 = (SerialKiller)playerRole;

                    if (CustomGameOptions.SKVentOptions == SKVentOptions.Always)
                        mainflag = true;
                    else if (role2.Lusted && CustomGameOptions.SKVentOptions == SKVentOptions.Bloodlust)
                        mainflag = true;
                    else if (!role2.Lusted && CustomGameOptions.SKVentOptions == SKVentOptions.NoLust)
                        mainflag = true;
                    else
                        mainflag = false;
                }
                else
                    mainflag = false;
            }
            else
                mainflag = false;

            return mainflag;
        }
        
        public static List<bool> Interact(PlayerControl player, PlayerControl target, Role cautious = null, bool toKill = false, bool toConvert = false, Role cautious2 = null)
        {
            if (!CanInteract(player))
                return null;

            bool fullCooldownReset = false;
            bool gaReset = false;
            bool survReset = false;
            bool abilityUsed = false;

            Spread(player, target);

            if (cautious != null)
            {
                if (cautious.Player == target)
                {
                    if ((player.IsShielded() || player.IsProtected()) && !target.Is(AbilityEnum.Ruthless))
                    {
                        if (player.IsShielded())
                        {
                            var medic = player.GetMedic().Player.PlayerId;
                            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                            writer.Write(medic);
                            writer.Write(player.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);

                            if (CustomGameOptions.ShieldBreaks)
                                fullCooldownReset = true;

                            StopKill.BreakShield(medic, player.PlayerId, CustomGameOptions.ShieldBreaks);
                        }
                        else if (player.IsProtected())
                            gaReset = true;
                    }
                    else
                        RpcMurderPlayer(target, player);
                    
                    if (target.IsShielded() && (toKill || toConvert))
                    {
                        var medic = target.GetMedic().Player.PlayerId;
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                        writer.Write(medic);
                        writer.Write(target.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);

                        if (CustomGameOptions.ShieldBreaks)
                            fullCooldownReset = true;

                        StopKill.BreakShield(medic, target.PlayerId, CustomGameOptions.ShieldBreaks);
                    }
                }
            }
            else if (cautious2 != null)
            {
                if (cautious2.Player == target)
                {
                    if ((player.IsShielded() || player.IsProtected()) && !target.Is(AbilityEnum.Ruthless))
                    {
                        if (player.IsShielded())
                        {
                            var medic = player.GetMedic().Player.PlayerId;
                            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                            writer.Write(medic);
                            writer.Write(player.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);

                            if (CustomGameOptions.ShieldBreaks)
                                fullCooldownReset = true;

                            StopKill.BreakShield(medic, player.PlayerId, CustomGameOptions.ShieldBreaks);
                        }
                        else if (player.IsProtected())
                            gaReset = true;
                    }
                    else
                        RpcMurderPlayer(target, player);
                    
                    if (target.IsShielded() && (toKill || toConvert))
                    {
                        var medic = target.GetMedic().Player.PlayerId;
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                        writer.Write(medic);
                        writer.Write(target.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);

                        if (CustomGameOptions.ShieldBreaks)
                            fullCooldownReset = true;

                        StopKill.BreakShield(medic, target.PlayerId, CustomGameOptions.ShieldBreaks);
                    }
                }
            }
            else if (target.IsOnAlert() && !player.Is(AbilityEnum.Ruthless))
            {
                if (player.Is(RoleEnum.Pestilence))
                {
                    if (target.IsShielded() && (toKill || toConvert))
                    {
                        var medic = target.GetMedic().Player.PlayerId;
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                        writer.Write(medic);
                        writer.Write(target.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);

                        if (CustomGameOptions.ShieldBreaks)
                            fullCooldownReset = true;

                        StopKill.BreakShield(medic, target.PlayerId, CustomGameOptions.ShieldBreaks);
                    }
                    else if (target.IsProtected())
                        gaReset = true;
                }
                else if (player.IsShielded() && (toKill || toConvert) && !target.Is(AbilityEnum.Ruthless))
                {
                    var medic = player.GetMedic().Player.PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                    writer.Write(medic);
                    writer.Write(player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    if (CustomGameOptions.ShieldBreaks)
                        fullCooldownReset = true;

                    StopKill.BreakShield(medic, player.PlayerId, CustomGameOptions.ShieldBreaks);
                }
                else if (player.IsProtected())
                    gaReset = true;
                else
                    RpcMurderPlayer(target, player);
                
                if (target.IsShielded() && (toKill || toConvert))
                {
                    var medic = target.GetMedic().Player.PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                    writer.Write(medic);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    if (CustomGameOptions.ShieldBreaks)
                        fullCooldownReset = true;

                    StopKill.BreakShield(medic, target.PlayerId, CustomGameOptions.ShieldBreaks);
                }
            }
            else if (target.IsShielded() && (toKill || toConvert) && !player.Is(AbilityEnum.Ruthless))
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                writer.Write(target.GetMedic().Player.PlayerId);
                writer.Write(target.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                if (CustomGameOptions.ShieldBreaks)
                    fullCooldownReset = true;

                StopKill.BreakShield(target.GetMedic().Player.PlayerId, target.PlayerId, CustomGameOptions.ShieldBreaks);
            }
            else if (target.IsVesting() && (toKill || toConvert) && !player.Is(AbilityEnum.Ruthless))
                survReset = true;
            else if (target.IsProtected() && (toKill || toConvert) && !player.Is(AbilityEnum.Ruthless))
                gaReset = true;
            else if (player.IsOtherRival(target) && (toKill || toConvert))
                fullCooldownReset = true;
            else
            {
                if (toKill)
                    RpcMurderPlayer(player, target);

                abilityUsed = true;
                fullCooldownReset = true;
            }

            var reset = new List<bool>();
            reset.Add(fullCooldownReset);
            reset.Add(gaReset);
            reset.Add(survReset);
            reset.Add(abilityUsed);
            return reset;
        }

        public static Il2CppSystem.Collections.Generic.List<PlayerControl> GetClosestPlayers(Vector2 truePosition, float radius)
        {
            Il2CppSystem.Collections.Generic.List<PlayerControl> playerControlList = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            Il2CppSystem.Collections.Generic.List<GameData.PlayerInfo> allPlayers = GameData.Instance.AllPlayers;
            float lightRadius = radius * ShipStatus.Instance.MaxLightRadius;

            for (int index = 0; index < allPlayers.Count; ++index)
            {
                GameData.PlayerInfo playerInfo = allPlayers[index];

                if (!playerInfo.Disconnected)
                {
                    Vector2 vector2 = new Vector2(playerInfo.Object.GetTruePosition().x - truePosition.x, playerInfo.Object.GetTruePosition().y - truePosition.y);
                    float magnitude = ((Vector2)vector2).magnitude;

                    if (magnitude <= lightRadius)
                    {
                        PlayerControl playerControl = playerInfo.Object;
                        playerControlList.Add(playerControl);
                    }
                }
            }

            return playerControlList;
        }

        public static bool IsTooFar(PlayerControl player, PlayerControl target)
        {
            if (player == null || target == null || (player == null && target == null))
                return true;

            var maxDistance = GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];
            return (GetDistBetweenPlayers(player, target) > maxDistance);
        }

        public static bool IsTooFar(PlayerControl player, DeadBody target)
        {
            if (player == null || target == null || (player == null && target == null))
                return true;

            var maxDistance = GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];
            return (GetDistBetweenPlayers(player, target) > maxDistance);
        }

        public static bool SetActive(PlayerControl target, HudManager hud)
        {
            var buttonFlag = hud.UseButton.isActiveAndEnabled || hud.PetButton.isActiveAndEnabled;
            var meetingFlag = !MeetingHud.Instance;
            return !target.Data.IsDead && GameStates.IsInGame && meetingFlag && buttonFlag;
        }

        public static bool NoButton(PlayerControl target, RoleEnum role)
        {
            return PlayerControl.AllPlayerControls.Count <= 1 || target == null || target.Data == null || !target.Is(role);
        }

        public static bool NoButton(PlayerControl target, ObjectifierEnum obj)
        {
            return PlayerControl.AllPlayerControls.Count <= 1 || target == null || target.Data == null || !target.Is(obj);
        }

        public static bool NoButton(PlayerControl target, AbilityEnum ability)
        {
            return PlayerControl.AllPlayerControls.Count <= 1 || target == null || target.Data == null || !target.Is(ability);
        }

        public static bool SeemsEvil(this PlayerControl player)
        {
            var intruderFlag = player.Is(Faction.Intruder) && !player.Is(ObjectifierEnum.Traitor);
            var syndicateFlag = player.Is(Faction.Syndicate) && !player.Is(ObjectifierEnum.Traitor);
            var traitorFlag = player.IsTurnedTraitor() && CustomGameOptions.TraitorColourSwap;
            var nkFlag = player.Is(RoleAlignment.NeutralKill) && !CustomGameOptions.NeutKillingRed;
            var neFlag = player.Is(RoleAlignment.NeutralEvil) && !CustomGameOptions.NeutEvilRed;
            var framedFlag = player.IsFramed();

            return intruderFlag || syndicateFlag || traitorFlag || nkFlag || neFlag || framedFlag;
        }

        public static bool SeemsGood(this PlayerControl player)
        {
            return !SeemsEvil(player);
        }

        public static bool IsBlockImmune(PlayerControl player)
        {
            var role = Role.GetRole(player);
            return (bool)role?.RoleBlockImmune;
        }

        public static void Spread(PlayerControl interacter, PlayerControl target)
        {
            if (interacter.IsInfected() || target.IsInfected() || target.Is(RoleEnum.Plaguebearer))
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer))
                    ((Plaguebearer)pb).RpcSpreadInfection(interacter, target);
            }

            if (target.Is(RoleEnum.Arsonist))
            {
                foreach (var arso in Role.GetRoles(RoleEnum.Arsonist))
                    ((Arsonist)arso).RpcSpreadDouse(target, interacter);
            }

            if (target.Is(RoleEnum.Cryomaniac))
            {
                foreach (var cryo in Role.GetRoles(RoleEnum.Cryomaniac))
                    ((Cryomaniac)cryo).RpcSpreadDouse(target, interacter);
            }
        }

        public static bool CanInteract(PlayerControl player)
        {
            var flag = player != null && !MeetingHud.Instance && (player.Is(RoleAlignment.NeutralKill) || player.Is(RoleEnum.Thief) || player.Is(Faction.Intruder) ||
                player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.Altruist) || player.Is(RoleEnum.Amnesiac) || player.Is(RoleEnum.Cannibal) || player.Is(RoleEnum.Detective) ||
                player.Is(RoleEnum.Dracula) || player.Is(RoleEnum.Dampyr) || player.Is(RoleEnum.VampireHunter) || player.Is(RoleEnum.Medic) || player.Is(RoleEnum.Shifter) ||
                player.Is(RoleEnum.Tracker) || player.Is(RoleEnum.Vigilante) || player.Is(Faction.Syndicate) || player.Is(RoleEnum.Inspector) || player.Is(RoleEnum.Escort) ||
                player.Is(RoleEnum.Troll) || player.Is(RoleEnum.Jackal) || player.Is(RoleEnum.Mystic) || player.Is(RoleEnum.Seer) || player.Is(RoleEnum.Jester));
            return flag;
        }
    }
}