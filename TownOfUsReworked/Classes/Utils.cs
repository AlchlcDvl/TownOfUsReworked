using HarmonyLib;
using Hazel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.NeutralsMod;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.CamouflagerMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using TownOfUsReworked.PlayerLayers.Modifiers;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using Reactor.Utilities;
using Il2CppInterop.Runtime.InteropTypes;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.SerialKillerMod;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.UndertakerMod;
using TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.SyndicateMod;
using AmongUs.GameOptions;
using TownOfUsReworked.PlayerLayers;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using TownOfUsReworked.Objects;
using System.Reflection;
using Il2CppInterop.Runtime;
using System.IO;
using TMPro;

namespace TownOfUsReworked.Classes
{
    [HarmonyPatch]
    public static class Utils
    {
        public static List<WinningPlayerData> PotentialWinners = new List<WinningPlayerData>();
        public static string RoleColorString => "<color=#" + Colors.Role.ToHtmlStringRGBA() + ">";
        public static string AlignmentColorString => "<color=#" + Colors.Alignment.ToHtmlStringRGBA() + ">";
        public static string ObjectivesColorString => "<color=#" + Colors.Objectives.ToHtmlStringRGBA() + ">";
        public static string AttributesColorString => "<color=#" + Colors.Attributes.ToHtmlStringRGBA() + ">";
        public static string AbilitiesColorString => "<color=#" + Colors.Abilities.ToHtmlStringRGBA() + ">";
        public static string ObjectifierColorString => "<color=#" + Colors.Objectifier.ToHtmlStringRGBA() + ">";
        public static string ModifierColorString => "<color=#" + Colors.Modifier.ToHtmlStringRGBA() + ">";
        public static string AbilityColorString => "<color=#" + Colors.Ability.ToHtmlStringRGBA() + ">";
        private static DLoadImage _iCallLoadImage;

        private delegate bool DLoadImage(IntPtr tex, IntPtr data, bool markNonReadable);

        public static TextMeshPro NameText(this PlayerControl p) => p.cosmetics.nameText;

        public static TextMeshPro NameText(this PoolablePlayer p) => p.cosmetics.nameText;

        public static SpriteRenderer myRend(this PlayerControl p) => p.cosmetics.currentBodySprite.BodySprite;

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

        public static VisualAppearance GetDefaultAppearance(this PlayerControl player) => new VisualAppearance();

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

        public static bool IsImpostor(this GameData.PlayerInfo playerinfo) => playerinfo?.Role?.TeamType == RoleTeamTypes.Impostor;

        public static bool IsImpostor(this PlayerVoteArea playerinfo) => PlayerByVoteArea(playerinfo).Data?.Role?.TeamType == RoleTeamTypes.Impostor;

        public static GameData.PlayerOutfit GetDefaultOutfit(this PlayerControl playerControl) => playerControl.Data.DefaultOutfit;

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

            if (GameStates.IsInGame)
            {
                playerControl.NameText().color = Color.white;

                if (PlayerControl.LocalPlayer.Is(Faction.Intruder) && playerControl.Is(Faction.Intruder))
                    playerControl.NameText().color = Colors.Intruder;

                if (PlayerControl.LocalPlayer.Is(Faction.Syndicate) && playerControl.Is(Faction.Syndicate))
                    playerControl.NameText().color = Colors.Syndicate;

                if (PlayerControl.LocalPlayer.Is(SubFaction.Undead) && playerControl.Is(SubFaction.Undead))
                    playerControl.NameText().color = Colors.Undead;

                if (PlayerControl.LocalPlayer.Is(SubFaction.Cabal) && playerControl.Is(SubFaction.Cabal))
                    playerControl.NameText().color = Colors.Cabal;

            }
            else
                playerControl.NameText().color = Color.white;
        }

        public static CustomPlayerOutfitType GetCustomOutfitType(this PlayerControl playerControl) => (CustomPlayerOutfitType)playerControl.CurrentOutfitType;

        public static bool IsNullOrDestroyed(this Object obj)
        {
            if (object.ReferenceEquals(obj, null))
                return true;

            if (obj is UnityEngine.Object)
                return (obj as UnityEngine.Object) == null;

            return false;
        }

        public static Texture2D CreateEmptyTexture(int width = 0, int height = 0) => new Texture2D(width, height, TextureFormat.RGBA32, Texture.GenerateAllMips, false, IntPtr.Zero);

        public static void Morph(PlayerControl player, PlayerControl MorphedPlayer)
        {
            if (CamouflageUnCamouflage.IsCamoed)
                return;

            if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Morph)
                player.SetOutfit(CustomPlayerOutfitType.Morph, MorphedPlayer.Data.DefaultOutfit);
        }

        public static void DefaultOutfit(PlayerControl player)
        {
            player.myRend().color = new Color32(255, 255, 255, 255);
            player.SetOutfit(CustomPlayerOutfitType.Default);
        }

        public static void Camouflage()
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                var everything = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()|{}[],.<>;':\"-+=*/`~_\\ ";
                var length = Random.RandomRangeInt(0, 21);
                var position = 0;
                var name = "";

                while (position < length && length > 0)
                {
                    var random = Random.RandomRangeInt(0, everything.Length);
                    name += everything[random];
                    position++;
                }

                if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage && player.GetCustomOutfitType() != CustomPlayerOutfitType.Invis && player.GetCustomOutfitType() !=
                    CustomPlayerOutfitType.PlayerNameOnly && !player.Data.IsDead && !PlayerControl.LocalPlayer.Data.IsDead && player != PlayerControl.LocalPlayer)
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
                    player.NameText().color = Color.clear;
                    player.cosmetics.colorBlindText.color = Color.clear;

                    if (CustomGameOptions.CamoHideSize)
                        player.GetAppearance().SizeFactor.Set(1f, 1f, 1f);

                    if (CustomGameOptions.CamoHideSpeed)
                        player.MyPhysics.body.velocity.Set(CustomGameOptions.PlayerSpeed, CustomGameOptions.PlayerSpeed);
                }

                player.SetName(name);
            }
        }

        public static void Conceal()
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                var color = new Color32(0, 0, 0, 0);

                if (PlayerControl.LocalPlayer.Is(Faction.Syndicate) || PlayerControl.LocalPlayer.Data.IsDead)
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
                    player.NameText().color = new Color32(0, 0, 0, 0);
                    player.cosmetics.colorBlindText.color = new Color32(0, 0, 0, 0);
                }
            }
        }

        public static void DefaultOutfitAll()
        {
            foreach (var player in PlayerControl.AllPlayerControls)
                DefaultOutfit(player);
        }

        public static void AddUnique<T>(this Il2CppSystem.Collections.Generic.List<T> self, T item) where T : IDisconnectHandler
        {
            if (!self.Contains(item))
                self.Add(item);
        }

        public static bool Is(this PlayerControl player, RoleEnum roleType) => Role.GetRole(player)?.RoleType == roleType;

        public static bool Is(this Role role, RoleEnum roleType) => role?.RoleType == roleType;

        public static bool Is(this PlayerControl player, Role role) => Role.GetRole(player).Player == role.Player;

        public static bool Is(this PlayerControl player, SubFaction subFaction) => Role.GetRole(player)?.SubFaction == subFaction;

        public static bool Is(this PlayerControl player, ModifierEnum modifierType) => Modifier.GetModifier(player)?.ModifierType == modifierType;

        public static bool Is(this PlayerControl player, ObjectifierEnum abilityType) => Objectifier.GetObjectifier(player)?.ObjectifierType == abilityType;

        public static bool Is(this PlayerControl player, AbilityEnum ability) => Ability.GetAbility(player)?.AbilityType == ability;

        public static bool Is(this PlayerControl player, Faction faction) => Role.GetRole(player)?.Faction == faction;

        public static bool Is(this PlayerControl player, RoleAlignment alignment) => Role.GetRole(player)?.RoleAlignment == alignment;

        public static bool Is(this PlayerVoteArea player, RoleEnum roleType) => PlayerByVoteArea(player).Is(roleType);

        public static bool Is(this PlayerVoteArea player, Role role) => PlayerByVoteArea(player).Is(role);

        public static bool Is(this PlayerVoteArea player, SubFaction subFaction) => PlayerByVoteArea(player).Is(subFaction);

        public static bool Is(this PlayerVoteArea player, ModifierEnum modifierType) => PlayerByVoteArea(player).Is(modifierType);

        public static bool Is(this PlayerVoteArea player, ObjectifierEnum abilityType) => PlayerByVoteArea(player).Is(abilityType);

        public static bool Is(this PlayerVoteArea player, AbilityEnum ability) => PlayerByVoteArea(player).Is(ability);

        public static bool Is(this PlayerVoteArea player, Faction faction) => PlayerByVoteArea(player).Is(faction);

        public static bool Is(this PlayerVoteArea player, RoleAlignment alignment) => PlayerByVoteArea(player).Is(alignment);

        public static Faction GetFaction(this PlayerControl player)
        {
            if (player == null)
                return Faction.None;

            var role = Role.GetRole(player);

            if (role == null)
                return player.Data.IsImpostor() ? Faction.Intruder : Faction.Crew;

            return role.Faction;
        }

        public static SubFaction GetSubFaction(this PlayerControl player)
        {
            if (player == null)
                return SubFaction.None;

            var role = Role.GetRole(player);

            if (role == null)
                return SubFaction.None;

            return role.SubFaction;
        }

        public static Faction GetFaction(this PlayerVoteArea player) => PlayerByVoteArea(player).GetFaction();

        public static SubFaction GetSubFaction(this PlayerVoteArea player) => PlayerByVoteArea(player).GetSubFaction();

        public static List<PlayerControl> GetCrewmates(List<PlayerControl> impostors) => PlayerControl.AllPlayerControls.ToArray().Where(player => !impostors.Any(imp => imp ==
            player)).ToList();

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

        public static bool IsBitten(this PlayerControl player)
        {
            if (player == null)
                return false;

            var role = Role.GetRole(player);

            if (role == null)
                return false;

            return role.IsBitten;
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

        public static bool IsRecruit(this PlayerVoteArea player) => PlayerByVoteArea(player).IsRecruit();

        public static bool IsBitten(this PlayerVoteArea player) => PlayerByVoteArea(player).IsBitten();

        public static bool IsPersuaded(this PlayerVoteArea player) => PlayerByVoteArea(player).IsPersuaded();

        public static bool IsResurrected(this PlayerVoteArea player) => PlayerByVoteArea(player).IsResurrected();

        public static bool NotOnTheSameSide(this PlayerControl player)
        {
            if (player == null)
                return false;

            var traitorFlag = player.IsTurnedTraitor();
            var fanaticFlag = player.IsTurnedFanatic();
            var recruitFlag = player.IsRecruit();
            var bittenFlag = player.IsBitten();
            var sectFlag = player.IsPersuaded();
            var revivedFlag = player.IsResurrected();
            var rivalFlag = player.IsWinningRival();
            var corruptedFlag = player.Is(ObjectifierEnum.Corrupted);

            return traitorFlag || recruitFlag || sectFlag || revivedFlag || rivalFlag || fanaticFlag || corruptedFlag || bittenFlag;
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

        public static bool IsBHTarget(this PlayerControl player)
        {
            if (player == null)
                return false;

            bool flag = false;

            foreach (BountyHunter bh in Role.GetRoles(RoleEnum.BountyHunter))
            {
                if (bh.TargetPlayer == null)
                    continue;

                if (player.PlayerId == bh.TargetPlayer.PlayerId)
                {
                    flag = true;
                    break;
                }
            }

            return flag;
        }

        public static bool IsGuessTarget(this PlayerControl player)
        {
            if (player == null)
                return false;

            bool flag = false;

            foreach (Guesser guess in Role.GetRoles(RoleEnum.Guesser))
            {
                if (guess.TargetPlayer == null)
                    continue;

                if (player.PlayerId == guess.TargetPlayer.PlayerId)
                {
                    flag = true;
                    break;
                }
            }

            return flag;
        }

        public static bool IsGATarget(this PlayerVoteArea player) => PlayerByVoteArea(player).IsGATarget();

        public static bool IsExeTarget(this PlayerVoteArea player) => PlayerByVoteArea(player).IsExeTarget();

        public static bool IsBHTarget(this PlayerVoteArea player) => PlayerByVoteArea(player).IsBHTarget();

        public static bool IsGuessTarget(this PlayerVoteArea player) => PlayerByVoteArea(player).IsGuessTarget();

        public static bool IsTarget(this PlayerControl player) => player.IsBHTarget() || player.IsGuessTarget() || player.IsGATarget() || player.IsExeTarget();

        public static bool IsTarget(this PlayerVoteArea player) => player.IsBHTarget() || player.IsGuessTarget() || player.IsGATarget() || player.IsExeTarget();

        public static bool CanDoTasks(this PlayerControl player)
        {
            if (player == null)
                return false;

            if (Role.GetRole(player) == null)
                return !player.Data.IsImpostor();

            var crewflag = player.Is(Faction.Crew);
            var neutralflag = player.Is(Faction.Neutral);
            var intruderflag = player.Is(Faction.Intruder);
            var syndicateflag = player.Is(Faction.Syndicate);

            var phantomflag = player.Is(RoleEnum.Phantom);

            var NotOnTheSameSide = player.NotOnTheSameSide();
            var taskmasterflag = player.Is(ObjectifierEnum.Taskmaster);

            var isdead = player.Data.IsDead;

            var flag1 = crewflag && !NotOnTheSameSide;
            var flag2 = neutralflag && (taskmasterflag || (phantomflag && isdead));
            var flag3 = intruderflag && taskmasterflag;
            var flag4 = syndicateflag && taskmasterflag;
            var flag = flag1 || flag2 || flag3 || flag4;

            return flag;
        }

        public static bool CanDoTasks(this PlayerVoteArea player) => PlayerByVoteArea(player).CanDoTasks();

        public static Jackal GetJackal(this PlayerControl player)
        {
            if (player == null)
                return null;

            if (!player.IsRecruit())
                return null;

            return Role.GetRoles(RoleEnum.Jackal).FirstOrDefault(role => ((Jackal)role).Recruited.Contains(player.PlayerId)) as Jackal;
        }

        public static Necromancer GetNecromancer(this PlayerControl player)
        {
            if (player == null)
                return null;

            if (!player.IsResurrected())
                return null;

            return Role.GetRoles(RoleEnum.Necromancer).FirstOrDefault(role => ((Necromancer)role).Resurrected.Contains(player.PlayerId)) as Necromancer;
        }

        public static Dracula GetDracula(this PlayerControl player)
        {
            if (player == null)
                return null;

            if (!player.Is(SubFaction.Undead))
                return null;

            return Role.GetRoles(RoleEnum.Dracula).FirstOrDefault(role => ((Dracula)role).Converted.Contains(player.PlayerId)) as Dracula;
        }

        public static Whisperer GetWhisperer(this PlayerControl player)
        {
            if (player == null)
                return null;

            if (!player.IsPersuaded())
                return null;

            return Role.GetRoles(RoleEnum.Whisperer).FirstOrDefault(role => ((Whisperer)role).Persuaded.Contains(player.PlayerId)) as Whisperer;
        }

        public static Jackal GetJackal(this PlayerVoteArea player) => PlayerByVoteArea(player).GetJackal();

        public static Necromancer GetNecromancer(this PlayerVoteArea player) => PlayerByVoteArea(player).GetNecromancer();

        public static Dracula GetDracula(this PlayerVoteArea player) => PlayerByVoteArea(player).GetDracula();

        public static Whisperer GetWhisperer(this PlayerVoteArea player) => PlayerByVoteArea(player).GetWhisperer();

        public static PlayerControl PlayerById(byte id)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.PlayerId == id)
                    return player;
            }

            return null;
        }

        public static PlayerControl PlayerByVoteArea(PlayerVoteArea state) => PlayerById(state.TargetPlayerId);

        public static bool IsShielded(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Medic).Any(role =>
            {
                var shieldedPlayer = ((Medic)role).ShieldedPlayer;
                return shieldedPlayer != null && player.PlayerId == shieldedPlayer.PlayerId;
            });
        }

        public static bool IsRetShielded(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Retributionist).Any(role =>
            {
                var shieldedPlayer = ((Retributionist)role).ShieldedPlayer;
                return shieldedPlayer != null && player.PlayerId == shieldedPlayer.PlayerId && ((Retributionist)role).RevivedRole?.RoleType == RoleEnum.Medic;
            });
        }

        public static bool IsShielded(this PlayerVoteArea player) => PlayerByVoteArea(player).IsShielded();

        public static bool IsRetShielded(this PlayerVoteArea player) => PlayerByVoteArea(player).IsRetShielded();

        public static Medic GetMedic(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Medic).FirstOrDefault(role =>
            {
                var shieldedPlayer = ((Medic)role).ShieldedPlayer;
                return shieldedPlayer != null && player == shieldedPlayer;
            }) as Medic;
        }

        public static Retributionist GetRetMedic(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Medic).FirstOrDefault(role =>
            {
                var shieldedPlayer = ((Retributionist)role).ShieldedPlayer;
                return shieldedPlayer != null && player == shieldedPlayer && ((Retributionist)role).RevivedRole?.RoleType == RoleEnum.Medic;
            }) as Retributionist;
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

        public static bool IsInfected(this PlayerVoteArea player) => PlayerByVoteArea(player).IsInfected();

        public static bool IsFramed(this PlayerVoteArea player) => PlayerByVoteArea(player).IsFramed();

        public static bool IsWinningRival(this PlayerControl player)
        {
            if (player == null || player.Data == null)
                return false;

            if (!player.Is(ObjectifierEnum.Rivals))
                return false;

            var rival = Objectifier.GetObjectifier<Rivals>(player);

            return rival.RivalDead();
        }

        public static bool IsTurnedTraitor(this PlayerControl player) => player.IsIntTraitor() || player.IsSynTraitor();

        public static bool IsTurnedFanatic(this PlayerControl player) => player.IsIntFanatic() || player.IsSynFanatic();

        public static bool IsTurnedTraitor(this PlayerVoteArea player) => PlayerByVoteArea(player).IsTurnedTraitor();

        public static bool IsTurnedFanatic(this PlayerVoteArea player) => PlayerByVoteArea(player).IsTurnedFanatic();

        public static bool IsUnturnedFanatic(this PlayerControl player)
        {
            if (!player.Is(ObjectifierEnum.Fanatic))
                return false;

            var fanatic = Objectifier.GetObjectifier<Fanatic>(player);
            return !fanatic.Turned;
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

        public static bool IsCrewAlly(this PlayerControl player)
        {
            if (player == null || player.Data == null)
                return false;

            if (!player.Is(ObjectifierEnum.Allied))
                return false;

            var traitor = Role.GetRole(player);
            return traitor.IsCrewAlly;
        }

        public static bool IsSynAlly(this PlayerControl player)
        {
            if (player == null || player.Data == null)
                return false;

            if (!player.Is(ObjectifierEnum.Allied))
                return false;

            var traitor = Role.GetRole(player);
            return traitor.IsSynAlly;
        }

        public static bool IsIntAlly(this PlayerControl player)
        {
            if (player == null || player.Data == null)
                return false;

            if (!player.Is(ObjectifierEnum.Allied))
                return false;

            var traitor = Role.GetRole(player);
            return traitor.IsIntAlly;
        }

        public static bool IsAlly(this PlayerControl player) => player.IsCrewAlly() || player.IsIntAlly() || player.IsSynAlly();

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
            var flag = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(RoleAlignment.NeutralKill) ||
                x.Is(RoleAlignment.NeutralNeo) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralPros) || x.NotOnTheSameSide()) && !x.IsCrewAlly()) == 0;

            return flag;
        }

        public static bool IntrudersWin()
        {
            var flag = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Crew) || x.Is(RoleAlignment.NeutralKill) ||
                x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralNeo) || x.Is(RoleAlignment.NeutralPros) || x.NotOnTheSameSide()) && !x.IsIntAlly()) == 0;

            return flag;
        }

        public static bool SyndicateWins()
        {
            var flag = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(RoleAlignment.NeutralKill) || x.Is(Faction.Intruder) ||
                x.Is(RoleAlignment.NeutralNeo) || x.Is(Faction.Crew) || x.Is(RoleAlignment.NeutralPros) || x.NotOnTheSameSide()) && !x.IsSynAlly()) == 0;

            return flag;
        }

        public static bool AllNeutralsWin()
        {
            var flag = (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.NotOnTheSameSide() || x.Is(Faction.Crew) ||
                x.Is(Faction.Syndicate) || x.Is(Faction.Intruder))) == 0) && CustomGameOptions.NoSolo == NoSolo.AllNeutrals;

            return flag;
        }

        public static bool NKWins(RoleEnum nk)
        {
            var flag = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || (x.Is(RoleAlignment.NeutralKill) &&
                !x.Is(nk)) || x.Is(RoleAlignment.NeutralNeo) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralPros) || x.Is(ObjectifierEnum.Allied) || x.Is(Faction.Crew) ||
                x.NotOnTheSameSide())) == 0;

            return flag;
        }

        public static bool PestOrPBWins()
        {
            var flag = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || (x.Is(RoleAlignment.NeutralKill) &&
                !x.Is(RoleEnum.Plaguebearer)) || x.Is(RoleAlignment.NeutralNeo) || x.Is(Faction.Syndicate) || (x.Is(RoleAlignment.NeutralPros) && !x.Is(RoleEnum.Pestilence)) ||
                x.Is(ObjectifierEnum.Allied) || x.Is(Faction.Crew) || x.NotOnTheSameSide())) == 0;

            return flag;
        }

        public static bool AllNKsWin()
        {
            var flag = (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(RoleAlignment.NeutralNeo) ||
                x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralPros) || x.Is(Faction.Crew) || x.Is(ObjectifierEnum.Allied) || x.NotOnTheSameSide())) == 0) &&
                CustomGameOptions.NoSolo == NoSolo.AllNKs;

            return flag;
        }

        public static bool NoOneWins() => PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) == 0;

        public static bool CorruptedWin() => PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(ObjectifierEnum.Corrupted)) == 0;

        public static bool LoversWin(PlayerControl player)
        {
            var flag1 = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) == 3;

            var lover = Objectifier.GetObjectifier<Lovers>(player);
            var flag2 = lover.OtherLover != null && lover.Player != null && !lover.OtherLover.Data.IsDead && !lover.Player.Data.IsDead && !lover.OtherLover.Data.Disconnected &&
                !lover.Player.Data.Disconnected;

            var flag = flag1 && flag2;
            return flag;
        }

        public static bool RivalsWin(PlayerControl player)
        {
            var flag1 = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) == 2;

            var rival = Objectifier.GetObjectifier<Rivals>(player);
            var flag2 = rival.RivalDead();

            var flag = flag1 && flag2;
            return flag;
        }

        public static bool CabalWin()
        {
            var flag = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(RoleAlignment.NeutralKill) ||
                (x.Is(RoleAlignment.NeutralNeo) && !x.Is(RoleEnum.Jackal)) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralPros) || x.Is(Faction.Crew) ||
                x.Is(ObjectifierEnum.Lovers) || x.IsWinningRival() || x.IsResurrected() || x.IsPersuaded() || x.IsBitten()) && !x.IsRecruit()) == 0;

            return flag;
        }

        public static bool UndeadWin()
        {
            var flag = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(RoleAlignment.NeutralKill) ||
                (x.Is(RoleAlignment.NeutralNeo) && !x.Is(RoleEnum.Dracula)) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralPros) || x.Is(Faction.Crew) ||
                x.Is(ObjectifierEnum.Lovers) || x.IsWinningRival() || x.IsResurrected() || x.IsRecruit() || x.IsPersuaded()) && !x.IsBitten()) == 0;

            return flag;
        }

        public static bool SectWin()
        {
            var flag = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(RoleAlignment.NeutralKill) ||
                (x.Is(RoleAlignment.NeutralNeo) && !x.Is(RoleEnum.Whisperer)) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralPros) || x.Is(Faction.Crew) ||
                x.Is(ObjectifierEnum.Lovers) || x.IsWinningRival() || x.IsRecruit() || x.IsResurrected() || x.IsBitten()) && !x.IsPersuaded()) == 0;

            return flag;
        }

        public static bool ReanimatedWin()
        {
            var flag = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(RoleAlignment.NeutralKill) ||
                (x.Is(RoleAlignment.NeutralNeo) && !x.Is(RoleEnum.Necromancer)) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralPros) || x.Is(Faction.Crew) ||
                x.Is(ObjectifierEnum.Lovers) || x.IsWinningRival() || x.IsPersuaded() || x.IsRecruit() || x.IsBitten()) && !x.IsResurrected()) == 0;

            return flag;
        }

        public static bool IsWinner(this string playerName)
        {
            var winners = TempData.winners;

            foreach (var win in winners)
            {
                if (win.PlayerName == playerName)
                    return true;
            }

            return false;
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
            var truePosition = refPlayer.GetTruePosition();
            var maxDistance = CustomGameOptions.InteractionDistance;
            var closestDistance = double.MaxValue;
            PlayerControl closestPlayer = null;

            foreach (var player in AllPlayers)
            {
                if (player.Data.IsDead || player == refPlayer || !player.Collider.enabled || player.inMovingPlat || (player.inVent && !CustomGameOptions.VentTargetting))
                    continue;

                var distance = Vector2.Distance(truePosition, player.GetTruePosition());

                if (distance > closestDistance)
                    continue;

                if (distance > maxDistance)
                    continue;

                closestPlayer = player;
                closestDistance = distance;
            }

            return closestPlayer;
        }

        public static void SetTarget(ref PlayerControl closestPlayer, KillButton button, List<PlayerControl> targets = null)
        {
            if (!ButtonUsable(button))
                return;

            var target = SetClosestPlayer(ref closestPlayer, targets);

            if (target != null)
                button.SetTarget(target);
        }

        public static bool CanInteractWithDead(this PlayerControl player)
        {
            var flag = player != null && !MeetingHud.Instance && (player.Is(RoleEnum.Altruist) || player.Is(RoleEnum.Amnesiac) || player.Is(RoleEnum.Cannibal) ||
                player.Is(RoleEnum.Necromancer) || player.Is(RoleEnum.Janitor) || player.Is(RoleEnum.Godfather) || player.Is(RoleEnum.Coroner) || player.Is(RoleEnum.Undertaker) ||
                player.Is(RoleEnum.Retributionist));

            return flag;
        }

        public static void SetDeadTarget(this AbilityButton button, Role role, bool condition = true)
        {
            if (!ButtonUsable(button) || !CanInteractWithDead(role.Player))
                return;

            var target = GetClosestDeadPlayer(role.Player);
            DeadBody oldTarget = null;

            switch (role.RoleType)
            {
                case RoleEnum.Amnesiac:
                    var amne = (Amnesiac)role;
                    oldTarget = amne.CurrentTarget;
                    amne.CurrentTarget = target;
                    break;

                case RoleEnum.Altruist:
                    var alt = (Altruist)role;
                    oldTarget = alt.CurrentTarget;
                    alt.CurrentTarget = target;
                    break;

                case RoleEnum.Retributionist:
                    var ret = (Retributionist)role;
                    var revivedRole = ret.RevivedRole?.RoleType;

                    if (revivedRole != RoleEnum.Altruist && revivedRole != RoleEnum.Coroner)
                        return;

                    oldTarget = ret.CurrentTarget;
                    ret.CurrentTarget = target;
                    break;

                case RoleEnum.Cannibal:
                    var cann = (Cannibal)role;
                    oldTarget = cann.CurrentTarget;
                    cann.CurrentTarget = target;
                    break;

                case RoleEnum.Necromancer:
                    var necro = (Necromancer)role;
                    oldTarget = necro.CurrentTarget;
                    necro.CurrentTarget = target;
                    break;

                case RoleEnum.Coroner:
                    var cor = (Coroner)role;
                    oldTarget = cor.CurrentTarget;
                    cor.CurrentTarget = target;
                    break;

                case RoleEnum.Janitor:
                    var jani = (Janitor)role;
                    oldTarget = jani.CurrentTarget;
                    jani.CurrentTarget = target;
                    break;

                case RoleEnum.Undertaker:
                    var ut = (Undertaker)role;
                    oldTarget = ut.CurrentTarget;
                    ut.CurrentTarget = target;
                    break;

                case RoleEnum.Godfather:
                    var gf = (Godfather)role;
                    var formerRole = gf.FormerRole?.RoleType;

                    if (formerRole != RoleEnum.Janitor && formerRole != RoleEnum.Undertaker)
                        return;

                    oldTarget = gf.CurrentTarget;
                    gf.CurrentTarget = target;
                    break;
            }

            var component = target?.bodyRenderer;
            var oldComponent = oldTarget?.bodyRenderer;

            if (target != oldTarget && oldTarget != null)
                oldComponent?.material.SetFloat("_Outline", 1f);

            if (target != null && !role.IsBlocked && !button.isCoolingDown && condition && !role.Player.inVent && !role.Player.inMovingPlat)
            {
                component.material.SetFloat("_Outline", 1f);
                component.material.SetColor("_OutlineColor", role.Color);
                button.SetEnabled();
            }
            else
                button.SetDisabled();
        }

        public static bool CanInteract(PlayerControl player)
        {
            var flag = player != null && !MeetingHud.Instance && (player.Is(RoleAlignment.NeutralKill) || player.Is(RoleEnum.Thief) || player.Is(Faction.Intruder) ||
                player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.Seer) || player.Is(RoleEnum.Detective) || player.Is(RoleEnum.VampireHunter) || player.Is(RoleEnum.Medic) ||
                player.Is(RoleEnum.Shifter) || player.Is(RoleEnum.Mystic) || player.Is(RoleAlignment.NeutralNeo) || player.Is(RoleEnum.Tracker) || player.Is(RoleEnum.Vigilante) ||
                player.Is(Faction.Syndicate) || player.Is(RoleEnum.Inspector) || player.Is(RoleEnum.Escort) || player.Is(RoleEnum.Troll) || player.Is(RoleEnum.BountyHunter) ||
                player.Is(ObjectifierEnum.Corrupted) || player.Is(RoleEnum.Retributionist) || player.Is(RoleEnum.Coroner) || player.Is(RoleEnum.Pestilence) ||
                player.Is(RoleEnum.Jester) || player.Is(RoleEnum.Executioner) || player.Is(RoleEnum.Betrayer));
            return flag;
        }

        public static void SetAliveTarget(this AbilityButton button, Role role, bool condition = true, List<PlayerControl> targets = null)
        {
            if (!ButtonUsable(button) || !CanInteract(role.Player))
                return;

            if (targets == null)
                targets = PlayerControl.AllPlayerControls.ToArray().ToList();
            
            var toberemoved = new List<PlayerControl>();
            
            foreach (var player in targets)
            {
                if (player.Data.IsDead || player.Data.Disconnected)
                    toberemoved.Add(player);
            }

            foreach (var player in toberemoved)
                targets.Remove(player);

            var target = GetClosestPlayer(role.Player, targets);
            PlayerControl oldTarget = null;
            PlayerControl oldTarget1 = null;
            PlayerControl oldTarget2 = null;
            PlayerControl oldTarget3 = null;

            switch (role.Faction)
            {
                case Faction.Intruder:
                    if (role.Player.Is(ObjectifierEnum.Allied) || role.Player.Is(ObjectifierEnum.Fanatic) || role.Player.Is(ObjectifierEnum.Traitor))
                        break;

                    var intr = (IntruderRole)role;
                    oldTarget = intr.ClosestPlayer;
                    intr.ClosestPlayer = target;
                    break;

                case Faction.Syndicate:
                    if (role.Player.Is(ObjectifierEnum.Allied) || role.Player.Is(ObjectifierEnum.Fanatic) || role.Player.Is(ObjectifierEnum.Traitor))
                        break;

                    var syn = (SyndicateRole)role;
                    oldTarget = syn.ClosestPlayer;
                    syn.ClosestPlayer = target;
                    break;
            }

            var oldComponent = oldTarget?.myRend();

            if (target != oldTarget && oldTarget != null && target != null)
                oldComponent?.material.SetFloat("_Outline", 0f);

            switch (role.RoleType)
            {
                case RoleEnum.Thief:
                    var thief = (Thief)role;
                    oldTarget = thief.ClosestPlayer;
                    thief.ClosestPlayer = target;
                    break;

                case RoleEnum.Sheriff:
                    var sher = (Sheriff)role;
                    oldTarget = sher.ClosestPlayer;
                    sher.ClosestPlayer = target;
                    break;

                case RoleEnum.Detective:
                    var det = (Detective)role;
                    oldTarget = det.ClosestPlayer;
                    det.ClosestPlayer = target;
                    break;

                case RoleEnum.Coroner:
                    var cor = (Coroner)role;
                    oldTarget = cor.ClosestPlayer;
                    cor.ClosestPlayer = target;
                    break;

                case RoleEnum.Seer:
                    var seer = (Seer)role;
                    oldTarget = seer.ClosestPlayer;
                    seer.ClosestPlayer = target;
                    break;

                case RoleEnum.VampireHunter:
                    var vh = (VampireHunter)role;
                    oldTarget = vh.ClosestPlayer;
                    vh.ClosestPlayer = target;
                    break;

                case RoleEnum.Medic:
                    var medic = (Medic)role;
                    oldTarget = medic.ClosestPlayer;
                    medic.ClosestPlayer = target;
                    break;

                case RoleEnum.Shifter:
                    var shift = (Shifter)role;
                    oldTarget = shift.ClosestPlayer;
                    shift.ClosestPlayer = target;
                    break;

                case RoleEnum.Mystic:
                    var mys = (Mystic)role;
                    oldTarget = mys.ClosestPlayer;
                    mys.ClosestPlayer = target;
                    break;

                case RoleEnum.Dracula:
                    var drac = (Dracula)role;
                    oldTarget = drac.ClosestPlayer;
                    drac.ClosestPlayer = target;
                    break;

                case RoleEnum.Jackal:
                    var jack = (Jackal)role;
                    oldTarget = jack.ClosestPlayer;
                    jack.ClosestPlayer = target;
                    break;

                case RoleEnum.Retributionist:
                    var ret = (Retributionist)role;
                    var revivedRole = ret.RevivedRole?.RoleType;

                    if (revivedRole != RoleEnum.Sheriff && revivedRole != RoleEnum.Coroner && revivedRole != RoleEnum.Detective && revivedRole != RoleEnum.Seer && revivedRole !=
                        RoleEnum.VampireHunter && revivedRole != RoleEnum.Medic && revivedRole != RoleEnum.Mystic && revivedRole != RoleEnum.Vigilante && revivedRole != RoleEnum.Tracker &&
                        revivedRole != RoleEnum.Inspector)
                        return;

                    oldTarget = ret.ClosestPlayer;
                    ret.ClosestPlayer = target;
                    break;

                case RoleEnum.Necromancer:
                    var necro = (Necromancer)role;
                    oldTarget = necro.ClosestPlayer;
                    necro.ClosestPlayer = target;
                    break;

                case RoleEnum.Tracker:
                    var track = (Tracker)role;
                    oldTarget = track.ClosestPlayer;
                    track.ClosestPlayer = target;
                    break;

                case RoleEnum.Vigilante:
                    var vig = (Vigilante)role;
                    oldTarget = vig.ClosestPlayer;
                    vig.ClosestPlayer = target;
                    break;

                case RoleEnum.BountyHunter:
                    var bh = (BountyHunter)role;
                    oldTarget = bh.ClosestPlayer;
                    bh.ClosestPlayer = target;
                    break;

                case RoleEnum.Inspector:
                    var insp = (Inspector)role;
                    oldTarget = insp.ClosestPlayer;
                    insp.ClosestPlayer = target;
                    break;

                case RoleEnum.Escort:
                    var esc = (Escort)role;
                    oldTarget = esc.ClosestPlayer;
                    esc.ClosestPlayer = target;
                    break;

                case RoleEnum.Troll:
                    var troll = (Troll)role;
                    oldTarget = troll.ClosestPlayer;
                    troll.ClosestPlayer = target;
                    break;

                case RoleEnum.Pestilence:
                    var pest = (Pestilence)role;
                    oldTarget = pest.ClosestPlayer;
                    pest.ClosestPlayer = target;
                    break;

                case RoleEnum.Ambusher:
                    var amb = (Ambusher)role;
                    oldTarget = amb.ClosestAmbush;
                    amb.ClosestAmbush = target;
                    break;

                case RoleEnum.Blackmailer:
                    var bm = (Blackmailer)role;
                    oldTarget = bm.ClosestBlackmail;
                    bm.ClosestBlackmail = target;
                    break;

                case RoleEnum.Consigliere:
                    var consig = (Consigliere)role;
                    oldTarget = consig.ClosestTarget;
                    consig.ClosestTarget = target;
                    break;

                case RoleEnum.Consort:
                    var cons = (Consort)role;
                    oldTarget = cons.ClosestTarget;
                    cons.ClosestTarget = target;
                    break;

                case RoleEnum.Disguiser:
                    var disg = (Disguiser)role;
                    oldTarget = disg.ClosestTarget;
                    disg.ClosestTarget = target;
                    break;

                case RoleEnum.Godfather:
                    var gf = (Godfather)role;
                    oldTarget = gf.ClosestTarget;
                    oldTarget1 = gf.ClosestBlackmail;
                    oldTarget2 = gf.ClosestIntruder;
                    oldTarget3 = gf.ClosestAmbush;
                    gf.ClosestTarget = target;
                    gf.ClosestBlackmail = target;
                    gf.ClosestIntruder = target;
                    gf.ClosestAmbush = target;
                    break;

                case RoleEnum.Morphling:
                    var morph = (Morphling)role;
                    oldTarget = morph.ClosestTarget;
                    morph.ClosestTarget = target;
                    break;

                case RoleEnum.Arsonist:
                    var arso = (Arsonist)role;
                    oldTarget = arso.ClosestPlayer;
                    arso.ClosestPlayer = target;
                    break;

                case RoleEnum.Cryomaniac:
                    var cryo = (Cryomaniac)role;
                    oldTarget = cryo.ClosestPlayer;
                    cryo.ClosestPlayer = target;
                    break;

                case RoleEnum.Glitch:
                    var gli = (Glitch)role;
                    oldTarget = gli.ClosestPlayer;
                    gli.ClosestPlayer = target;
                    break;

                case RoleEnum.Juggernaut:
                    var jugg = (Juggernaut)role;
                    oldTarget = jugg.ClosestPlayer;
                    jugg.ClosestPlayer = target;
                    break;

                case RoleEnum.Murderer:
                    var murd = (Murderer)role;
                    oldTarget = murd.ClosestPlayer;
                    murd.ClosestPlayer = target;
                    break;

                case RoleEnum.Plaguebearer:
                    var pb = (Plaguebearer)role;
                    oldTarget = pb.ClosestPlayer;
                    pb.ClosestPlayer = target;
                    break;

                case RoleEnum.SerialKiller:
                    var sk = (SerialKiller)role;
                    oldTarget = sk.ClosestPlayer;
                    sk.ClosestPlayer = target;
                    break;

                case RoleEnum.Werewolf:
                    var ww = (Werewolf)role;
                    oldTarget = ww.ClosestPlayer;
                    ww.ClosestPlayer = target;
                    break;

                case RoleEnum.Crusader:
                    var crus = (Crusader)role;
                    oldTarget = crus.ClosestCrusade;
                    crus.ClosestCrusade = target;
                    break;

                case RoleEnum.Framer:
                    var frame = (Framer)role;
                    oldTarget = frame.ClosestFrame;
                    frame.ClosestFrame = target;
                    break;

                case RoleEnum.Gorgon:
                    var gorg = (Gorgon)role;
                    oldTarget = gorg.ClosestGaze;
                    gorg.ClosestGaze = target;
                    break;

                case RoleEnum.Poisoner:
                    var pois = (Poisoner)role;
                    oldTarget = pois.ClosestPoison;
                    pois.ClosestPoison = target;
                    break;

                case RoleEnum.Rebel:
                    var reb = (Rebel)role;
                    oldTarget = reb.ClosestSyndicate;
                    oldTarget1 = reb.ClosestFrame;
                    oldTarget2 = reb.ClosestCrusade;
                    reb.ClosestSyndicate = target;
                    reb.ClosestFrame = target;
                    reb.ClosestCrusade = target;
                    break;

                case RoleEnum.Jester:
                    var jest = (Rebel)role;
                    oldTarget = jest.ClosestPlayer;
                    jest.ClosestPlayer = target;
                    break;

                case RoleEnum.Executioner:
                    var exe = (Executioner)role;
                    oldTarget = exe.ClosestPlayer;
                    exe.ClosestPlayer = target;
                    break;

                case RoleEnum.Betrayer:
                    var bet = (Betrayer)role;
                    oldTarget = bet.ClosestPlayer;
                    bet.ClosestPlayer = target;
                    break;
            }

            var component = target?.myRend();
            oldComponent = oldTarget?.myRend();

            if (target != oldTarget && oldTarget != null)
                oldComponent?.material.SetFloat("_Outline", 0f);

            oldComponent = oldTarget1?.myRend();

            if (target != oldTarget && oldTarget != null && target != null)
                oldComponent?.material.SetFloat("_Outline", 0f);

            oldComponent = oldTarget2?.myRend();

            if (target != oldTarget && oldTarget != null && target != null)
                oldComponent?.material.SetFloat("_Outline", 0f);

            oldComponent = oldTarget3?.myRend();

            if (target != oldTarget && oldTarget != null && target != null)
                oldComponent?.material.SetFloat("_Outline", 0f);

            if (target != null && !role.IsBlocked && !button.isCoolingDown && condition && !role.Player.inVent && !role.Player.inMovingPlat)
            {
                component.material.SetFloat("_Outline", 1f);
                component.material.SetColor("_OutlineColor", role.Color);
                button.SetEnabled();
            }
            else
                button.SetDisabled();
        }

        public static void SetAliveTarget(this AbilityButton button, Objectifier obj, List<PlayerControl> targets = null, bool condition = true)
        {
            if (!ButtonUsable(button) || !CanInteract(obj.Player))
                return;

            if (targets == null)
                targets = PlayerControl.AllPlayerControls.ToArray().ToList();

            var target = GetClosestPlayer(obj.Player, targets);
            PlayerControl oldTarget = null;

            var oldComponent = oldTarget?.myRend();

            if (target != oldTarget && oldTarget != null)
                oldComponent?.material.SetFloat("_Outline", 0f);

            switch (obj.ObjectifierType)
            {
                case ObjectifierEnum.Corrupted:
                    var cor = (Corrupted)obj;
                    oldTarget = cor.ClosestPlayer;
                    cor.ClosestPlayer = target;
                    break;
            }

            var component = target?.myRend();
            oldComponent = oldTarget?.myRend();

            if (target != oldTarget && oldTarget != null && target != null)
                oldComponent?.material.SetFloat("_Outline", 0f);

            if (target != null && !obj.Player.IsBlocked() && !button.isCoolingDown && condition && !obj.Player.inVent && !obj.Player.inMovingPlat)
            {
                component.material.SetFloat("_Outline", 1f);
                component.material.SetColor("_OutlineColor", obj.Color);
                button.SetEnabled();
            }
            else
                button.SetDisabled();
        }

        public static bool HasEffect(PlayerControl player)
        {
            var flag = player != null && !MeetingHud.Instance && (player.Is(RoleEnum.Camouflager) || player.Is(RoleEnum.Shapeshifter) || player.Is(RoleEnum.Chameleon) ||
                player.Is(RoleEnum.Engineer) || player.Is(RoleEnum.Medium) || player.Is(RoleEnum.Operative) || player.Is(RoleEnum.Retributionist) || player.Is(RoleEnum.TimeLord) ||
                player.Is(RoleEnum.Transporter) || player.Is(RoleEnum.Veteran) || player.Is(RoleEnum.Morphling) || player.Is(RoleEnum.Godfather) || player.Is(RoleEnum.Grenadier) ||
                player.Is(RoleEnum.Miner) || player.Is(RoleEnum.Teleporter) || player.Is(RoleEnum.TimeMaster) || player.Is(RoleEnum.Wraith) || player.Is(RoleEnum.GuardianAngel) ||
                player.Is(RoleEnum.Survivor) || player.Is(RoleEnum.Whisperer) || player.Is(RoleEnum.Bomber) || player.Is(RoleEnum.Concealer) || player.Is(RoleEnum.Warper) ||
                player.Is(RoleEnum.Framer) || player.Is(RoleEnum.Drunkard) || player.Is(RoleEnum.Rebel) || player.Is(AbilityEnum.ButtonBarry));
            return flag;
        }

        public static void SetEffectTarget(this AbilityButton button, Role role, bool condition = true)
        {
            if (!ButtonUsable(button) || !HasEffect(role.Player))
                return;

            if (!role.IsBlocked && !button.isCoolingDown && condition && !role.Player.inVent && !role.Player.inMovingPlat)
                button.SetEnabled();
            else
                button.SetDisabled();
        }

        public static void SetEffectTarget(this AbilityButton button, Ability ability, bool condition = true)
        {
            if (!ButtonUsable(button) || !HasEffect(ability.Player))
                return;

            if (!ability.Player.IsBlocked() && !button.isCoolingDown && condition && !ability.Player.inVent && !ability.Player.inMovingPlat)
                button.SetEnabled();
            else
                button.SetDisabled();
        }

        public static void UpdateButton(this AbilityButton button, Role role, string label, float timer, float maxTimer, Sprite sprite, AbilityTypes type, List<PlayerControl> targets =
            null, bool usable = true, bool condition = true, bool effectActive = false, float effectTimer = 0f, float maxDuration = 1f, bool usesActive = false, int uses = 0, bool
            postDeath = false)
        {
            if (button == null)
                return;

            if (type == AbilityTypes.Direct)
                button.SetAliveTarget(role, condition, targets);
            else if (type == AbilityTypes.Dead)
                button.SetDeadTarget(role, condition);
            else if (type == AbilityTypes.Effect)
                button.SetEffectTarget(role, condition);
            else
                return;

            button.graphic.sprite = role.IsBlocked ? TownOfUsReworked.Blocked : (sprite ?? HudManager.Instance.AbilityButton.graphic.sprite);
            var difference = GetUnderdogChange(role.Player);
            button.SetCoolDown(effectActive ? effectTimer : timer, effectActive ? maxDuration : (maxTimer + difference));
            button.buttonLabelText.text = label;
            button.commsDown?.gameObject?.SetActive(false);

            if (role.Faction == Faction.Intruder && ((IntruderRole)role).KillButton == button)
                button.buttonLabelText.SetOutlineColor(role.FactionColor);
            else if (role.Faction == Faction.Syndicate && ((SyndicateRole)role).KillButton == button)
                button.buttonLabelText.SetOutlineColor(role.FactionColor);
            else
                button.buttonLabelText.SetOutlineColor(role.Color);

            if (!usesActive)
            {
                button.usesRemainingSprite.gameObject.SetActive(false);
                button.usesRemainingText.gameObject.SetActive(false);
            }
            else
                button.usesRemainingText.text = $"{uses}";

            if (MeetingHud.Instance)
                button.gameObject.SetActive(false);
            else if (postDeath)
                button.gameObject.SetActive(role.Player.Data.IsDead && !MeetingHud.Instance && !GameStates.IsLobby && usable);
            else
                button.gameObject.SetActive(SetActive(role.Player, role.RoleType, usable));
        }

        public static void UpdateButton(this AbilityButton button, Role role, string label, float timer, float maxTimer, Sprite sprite, AbilityTypes type, List<PlayerControl> targets,
            bool usesActive, int uses, bool usable = true, bool condition = true) => button.UpdateButton(role, label, timer, maxTimer, sprite, type, targets, usable, condition, false, 0,
            1, usesActive, uses);

        public static void UpdateButton(this AbilityButton button, Role role, string label, float timer, float maxTimer, Sprite sprite, AbilityTypes type, bool usesActive, int uses, bool
            usable = true, bool condition = true) => button.UpdateButton(role, label, timer, maxTimer, sprite, type, null, usable, condition, false, 0, 1, usesActive, uses);

        public static void UpdateButton(this AbilityButton button, Role role, string label, float timer, float maxTimer, Sprite sprite, AbilityTypes type, bool effectActive, float
            effectTimer, float effectDuration, bool usable = true, bool condition = true) => button.UpdateButton(role, label, timer, maxTimer, sprite, type, null, usable, condition,
            effectActive, effectTimer, effectDuration);

        public static void UpdateButton(this AbilityButton button, Role role, string label, float timer, float maxTimer, Sprite sprite, AbilityTypes type, bool usable, bool condition) =>
            button.UpdateButton(role, label, timer, maxTimer, sprite, type, null, usable, condition);

        public static void UpdateButton(this AbilityButton button, Role role, string label, float timer, float maxTimer, Sprite sprite, AbilityTypes type, bool usable) =>
            button.UpdateButton(role, label, timer, maxTimer, sprite, type, null, usable);
        
        public static AbilityButton InstantiateButton()
        {
            var button = Object.Instantiate(HudManager.Instance.AbilityButton, HudManager.Instance.AbilityButton.transform.parent);
            button.graphic.enabled = true;
            button.buttonLabelText.enabled = true;
            button.usesRemainingText.enabled = true;
            button.usesRemainingSprite.enabled = true;
            button.commsDown?.gameObject?.SetActive(false);
            return button;
        }

        public static void UpdateButton(this AbilityButton button, Objectifier obj, float timer, float maxTimer, string label, Sprite sprite, AbilityTypes type,
            List<PlayerControl> targets = null, bool condition = true, bool usesActive = false, int uses = 0)
        {
            if (CanInteract(obj.Player))
                button.SetAliveTarget(obj, targets, condition);

            button.graphic.sprite = obj.Player.IsBlocked() ? TownOfUsReworked.Blocked : (sprite ?? HudManager.Instance.AbilityButton.graphic.sprite);
            button.buttonLabelText.text = label;
            button.buttonLabelText.SetOutlineColor(obj.Color);
            button.commsDown?.gameObject?.SetActive(false);
            var difference = GetUnderdogChange(obj.Player);
            button.SetCoolDown(timer, (maxTimer + difference));

            if (!usesActive)
            {
                button.usesRemainingSprite.gameObject.SetActive(false);
                button.usesRemainingText.gameObject.SetActive(false);
            }
            else
                button.usesRemainingText.text = $"{uses}";

            if (MeetingHud.Instance)
                button.gameObject.SetActive(false);
            else
                button.gameObject.SetActive(SetActive(obj.Player, obj.ObjectifierType, condition));
        }

        public static void UpdateButton(this AbilityButton button, Ability ability, Sprite sprite, string label, AbilityTypes type, bool condition, float timer,
            float maxTimer, bool usesActive = false, int uses = 0)
        {
            if (HasEffect(ability.Player))
                button.SetEffectTarget(ability, condition);

            button.graphic.sprite = ability.Player.IsBlocked() ? TownOfUsReworked.Blocked : (sprite ?? HudManager.Instance.AbilityButton.graphic.sprite);
            button.buttonLabelText.text = label;
            button.buttonLabelText.SetOutlineColor(ability.Color);
            button.commsDown?.gameObject?.SetActive(false);
            var difference = GetUnderdogChange(ability.Player);
            button.SetCoolDown(timer, (maxTimer + difference));

            if (!usesActive)
            {
                button.usesRemainingSprite.gameObject.SetActive(false);
                button.usesRemainingText.gameObject.SetActive(false);
            }
            else
                button.usesRemainingText.text = $"{uses}";

            if (MeetingHud.Instance)
                button.gameObject.SetActive(false);
            else
                button.gameObject.SetActive(SetActive(ability.Player, ability.AbilityType, condition));
        }

        public static DeadBody GetClosestDeadPlayer(PlayerControl player, float maxDistance = 0f)
        {
            var truePosition = player.GetTruePosition();
            var closestDistance = double.MaxValue;
            DeadBody closestBody = null;

            if (maxDistance == 0f)
                maxDistance = CustomGameOptions.InteractionDistance;

            foreach (var body in Object.FindObjectsOfType<DeadBody>().Where(x => Vector2.Distance(x.TruePosition, truePosition) <= maxDistance))
            {
                var distance = Vector2.Distance(truePosition, body.TruePosition);

                if (distance > closestDistance)
                    continue;

                closestBody = body;
                closestDistance = distance;
            }

            return closestBody;
        }

        public static Vector2 GetSize()
        {
            var vents = Object.FindObjectsOfType<Vent>();
            var size = Vector2.Scale(vents[0].GetComponent<BoxCollider2D>().size, vents[0].transform.localScale) * 0.75f;
            return size;
        }

        public static PlayerControl SetClosestPlayer(ref PlayerControl closestPlayer, List<PlayerControl> targets = null)
        {
            if (targets == null)
                targets = PlayerControl.AllPlayerControls.ToArray().ToList();

            var maxDistance = CustomGameOptions.InteractionDistance;
            var player = GetClosestPlayer(PlayerControl.LocalPlayer, targets.Where(x => !x.inVent || (x.inVent && CustomGameOptions.VentTargetting) || !(x.Data.IsDead ||
                x.Data.Disconnected)).ToList());
            var closeEnough = player == null || (GetDistBetweenPlayers(PlayerControl.LocalPlayer, player) < maxDistance);
            return closestPlayer = closeEnough ? player : null;
        }

        public static double GetDistBetweenPlayers(PlayerControl player, PlayerControl refplayer)
        {
            if (player == null || refplayer == null)
                return double.MaxValue;

            var truePosition = refplayer.GetTruePosition();
            var truePosition2 = player.GetTruePosition();
            return Vector2.Distance(truePosition, truePosition2);
        }

        public static double GetDistBetweenPlayers(PlayerControl player, DeadBody refplayer)
        {
            if (player == null || refplayer == null)
                return double.MaxValue;

            var truePosition = refplayer.TruePosition;
            var truePosition2 = player.GetTruePosition();
            return Vector2.Distance(truePosition, truePosition2);
        }

        public static void RpcMurderPlayer(PlayerControl killer, PlayerControl target, bool lunge)
        {
            if (killer == null || target == null)
                return;

            MurderPlayer(killer, target, lunge);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.BypassKill);
            writer.Write(killer.PlayerId);
            writer.Write(target.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static void MurderPlayer(PlayerControl killer, PlayerControl target, bool lunge)
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
                
                if (target.Is(ModifierEnum.VIP))
                    Coroutines.Start(FlashCoroutine(targetRole.Color));

                var killerRole = Role.GetRole(killer);

                if (target.AmOwner)
                {
                    try
                    {
                        if (Minigame.Instance)
                            Minigame.Instance.Close();

                        if (MapBehaviour.Instance)
                            MapBehaviour.Instance.Close();
                    } catch {}

                    HudManager.Instance.KillOverlay.ShowKillAnimation(killer.Data, data);
                    HudManager.Instance.ShadowQuad.gameObject.SetActive(false);
                    target.NameText().GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
                    target.RpcSetScanner(false);
                }

                target.RegenTask();
                killer.RegenTask();

                if (lunge)
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

                var deadBody = new DeadPlayer
                {
                    PlayerId = target.PlayerId,
                    KillerId = killer.PlayerId,
                    KillTime = DateTime.UtcNow
                };

                if (killer == target)
                    return;

                Murder.KilledPlayers.Add(deadBody);

                if (!killer.AmOwner)
                    return;

                if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.SerialKiller))
                {
                    var sk = Role.GetRole<SerialKiller>(killer);
                    sk.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.LustKillCd);
                }
                else if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Glitch))
                {
                    var glitch = Role.GetRole<Glitch>(killer);
                    glitch.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.GlitchKillCooldown);
                }
                else if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Juggernaut))
                {
                    var juggernaut = Role.GetRole<Juggernaut>(killer);
                    juggernaut.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * (CustomGameOptions.JuggKillCooldown -
                        (CustomGameOptions.JuggKillBonus * juggernaut.JuggKills)));
                }
                else if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Werewolf))
                {
                    var pest = Role.GetRole<Pestilence>(killer);
                    pest.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.PestKillCd);
                }
                else if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Murderer))
                {
                    var pest = Role.GetRole<Pestilence>(killer);
                    pest.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.MurdKCD);
                }
                else if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Vigilante))
                {
                    var pest = Role.GetRole<Pestilence>(killer);
                    pest.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.PestKillCd);
                }
                else if (target.Is(ModifierEnum.Diseased) && killer.Is(AbilityEnum.Underdog))
                {
                    var cooldown = killer.Is(Faction.Intruder) ? CustomGameOptions.IntKillCooldown : CustomGameOptions.ChaosDriveKillCooldown;
                    var last = killer.Is(Faction.Intruder) ? LastImp() : LastSyn();
                    var lowerKC = (cooldown - CustomGameOptions.UnderdogKillBonus) * (CustomGameOptions.DiseasedMultiplier - 1f);
                    var normalKC = cooldown * (CustomGameOptions.DiseasedMultiplier - 1f);
                    var upperKC = (cooldown + CustomGameOptions.UnderdogKillBonus) * (CustomGameOptions.DiseasedMultiplier - 1f);
                    var role = Role.GetRole(killer);

                    switch (role.Faction)
                    {
                        case Faction.Syndicate:
                            var role2 = (SyndicateRole)role;
                            role2.LastKilled = DateTime.UtcNow.AddSeconds(last ? lowerKC : (CustomGameOptions.UnderdogIncreasedKC ? upperKC : normalKC));
                            break;

                        case Faction.Intruder:
                            var role3 = (IntruderRole)role;
                            role3.LastKilled = DateTime.UtcNow.AddSeconds(last ? lowerKC : (CustomGameOptions.UnderdogIncreasedKC ? upperKC : normalKC));
                            break;
                    }
                }
                else if (target.Is(ModifierEnum.Bait))
                    BaitReport(killer, target);
            }
        }

        public static void BaitReport(PlayerControl killer, PlayerControl target) => Coroutines.Start(BaitReportDelay(killer, target));

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
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
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
                var fullscreen = HudManager.Instance.FullScreen;
                fullscreen.enabled = true;
                fullscreen.gameObject.active = true;
                fullscreen.color = color;
            }

            yield return new WaitForSeconds(waitfor);

            if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
            {
                var fullscreen = HudManager.Instance.FullScreen;

                if (fullscreen.color.Equals(color))
                {
                    fullscreen.color = new Color(1f, 0f, 0f, 0.3f);
                    fullscreen.enabled = false;
                }
            }
        }

        public static IEnumerable<(T1, T2)> Zip<T1, T2>(List<T1> first, List<T2> second) => first.Zip(second, (x, y) => (x, y));

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

        public static void EndGame(GameOverReason reason = GameOverReason.ImpostorByVote, bool showAds = false) => GameManager.Instance.RpcEndGame(reason, showAds);

        //Submerged utils
        public static object TryCast(this Il2CppObjectBase self, Type type) => AccessTools.Method(self.GetType(), nameof(Il2CppObjectBase.TryCast)).MakeGenericMethod(type).Invoke(self,
            Array.Empty<object>());

        public static IList CreateList(Type myType) => (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(myType));

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

            foreach (Thief role in Role.GetRoles(RoleEnum.Thief))
                role.LastStolen = DateTime.UtcNow;

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

        public static bool LastImp() => PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(Faction.Intruder) && !(x.Data.IsDead || x.Data.Disconnected)) == 1;

        public static bool LastSyn() => PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(Faction.Syndicate) && !(x.Data.IsDead || x.Data.Disconnected)) == 1;

        public static bool Last(PlayerControl player)
        {
            if (player.Is(Faction.Intruder))
                return LastImp();
            else if (player.Is(Faction.Syndicate))
                return LastSyn();

            return false;
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

                    if (Role.GetRole(player).TasksDone)
                        crewWithNoTasks.Add(player);
                }
            }

            return allCrew.Count == crewWithNoTasks.Count;
        }

        public static bool Sabotaged()
        {
            if (ShipStatus.Instance.Systems != null)
            {
                if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.LifeSupp))
                {
                    var lifeSuppSystemType = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

                    if (lifeSuppSystemType.Countdown < 0f)
                        return true;
                }

                if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Laboratory))
                {
                    var reactorSystemType = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();

                    if (reactorSystemType.Countdown < 0f)
                        return true;
                }

                if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Reactor))
                {
                    var reactorSystemType = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ICriticalSabotage>();

                    if (reactorSystemType.Countdown < 0f)
                        return true;
                }
            }

            return false;
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
            else if (player.IsResurrected())
                mainflag = CustomGameOptions.ResurrectVent;
            else if (player.IsPersuaded())
                mainflag = CustomGameOptions.PersuadedVent;
            else if (player.IsBitten())
                mainflag = CustomGameOptions.UndeadVent;
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
            else if (player.Is(Faction.Crew) && !player.Is(RoleEnum.Revealer))
            {
                if (player.Is(AbilityEnum.Tunneler) && !player.Is(RoleEnum.Engineer))
                {
                    var tunneler = Role.GetRole(player);
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
                    (player.Is(RoleEnum.Survivor) && CustomGameOptions.SurvVent) || (player.Is(RoleEnum.Actor) && CustomGameOptions.ActorVent) ||
                    (player.Is(RoleEnum.GuardianAngel) && CustomGameOptions.GAVent) || (player.Is(RoleEnum.Amnesiac) && CustomGameOptions.AmneVent) ||
                    (player.Is(RoleEnum.Werewolf) && CustomGameOptions.WerewolfVent) || (player.Is(RoleEnum.Jackal) && CustomGameOptions.JackalVent) ||
                    (player.Is(RoleEnum.BountyHunter) && CustomGameOptions.BHVent) || player.Is(RoleEnum.Phantom);

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
            else if (player.Is(RoleEnum.Revealer) || player.Is(RoleEnum.Phantom))
                mainflag = true;
            else
                mainflag = false;

            return mainflag;
        }

        public static List<bool> Interact(PlayerControl player, PlayerControl target, bool toKill = false, bool toConvert = false)
        {
            if (!CanInteract(player))
                return new List<bool>() { false, false, false, false };

            bool fullCooldownReset = false;
            bool gaReset = false;
            bool survReset = false;
            bool abilityUsed = false;

            Spread(player, target);

            if ((target.IsOnAlert() || target.Is(RoleEnum.Pestilence) || (target.Is(RoleEnum.VampireHunter) && player.Is(SubFaction.Undead)) || (target.Is(RoleEnum.SerialKiller) &&
                (player.Is(RoleEnum.Escort) || player.Is(RoleEnum.Consort) || (player.Is(RoleEnum.Glitch) && !toKill))) && !player.Is(AbilityEnum.Ruthless)))
            {
                if (player.Is(RoleEnum.Pestilence))
                {
                    if (target.IsShielded() && (toKill || toConvert))
                    {
                        var medic = target.GetMedic().Player.PlayerId;
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
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
                else if (player.IsShielded() && !target.Is(AbilityEnum.Ruthless))
                {
                    var medic = player.GetMedic().Player.PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                    writer.Write(medic);
                    writer.Write(player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    if (CustomGameOptions.ShieldBreaks)
                        fullCooldownReset = true;

                    StopKill.BreakShield(medic, player.PlayerId, CustomGameOptions.ShieldBreaks);
                }
                else if (player.IsProtected() && !target.Is(AbilityEnum.Ruthless))
                    gaReset = true;
                else
                    RpcMurderPlayer(target, player, !target.Is(AbilityEnum.Ninja));

                if (target.IsShielded() && (toKill || toConvert))
                {
                    var medic = target.GetMedic().Player.PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
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
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
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
                {
                    if (target.Is(ObjectifierEnum.Fanatic) && (player.Is(Faction.Intruder) || player.Is(Faction.Syndicate)) && target.IsUnturnedFanatic())
                    {
                        var role = Role.GetRole(player);
                        Fanatic.TurnFanatic(target, role.Faction);

                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                        writer.Write((byte)TurnRPC.TurnFanatic);
                        writer.Write(player.PlayerId);
                        writer.Write(target.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                    else
                    {
                        RpcMurderPlayer(player, target, !player.Is(AbilityEnum.Ninja));

                        if (target.Is(RoleEnum.Troll))
                        {
                            var troll = Role.GetRole<Troll>(target);
                            troll.Killed = true;
                            RpcMurderPlayer(target, player, false);

                            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                            writer.Write((byte)WinLoseRPC.TrollWin);
                            writer.Write(target.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                        }
                    }
                }

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

        public static InspectorResults GetInspResults(this PlayerControl player)
        {
            if (player == null)
                return InspectorResults.None;

            var role = Role.GetRole(player);

            if (role == null)
                return InspectorResults.None;

            return role.InspectorResults;
        }

        public static InspectorResults GetInspResults(this PlayerVoteArea player) => PlayerByVoteArea(player).GetInspResults();

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
                    var vector2 = new Vector2(playerInfo.Object.GetTruePosition().x - truePosition.x, playerInfo.Object.GetTruePosition().y - truePosition.y);
                    float magnitude = vector2.magnitude;

                    if (magnitude <= lightRadius)
                    {
                        var playerControl = playerInfo.Object;
                        playerControlList.Add(playerControl);
                    }
                }
            }

            return playerControlList;
        }

        public static bool IsTooFar(PlayerControl player, PlayerControl target)
        {
            if (player == null || target == null)
                return true;

            var maxDistance = CustomGameOptions.InteractionDistance;
            return GetDistBetweenPlayers(player, target) > maxDistance;
        }

        public static bool IsTooFar(PlayerControl player, DeadBody target)
        {
            if (player == null || target == null)
                return true;

            var maxDistance = CustomGameOptions.InteractionDistance;
            return GetDistBetweenPlayers(player, target) > maxDistance;
        }

        public static bool SetActive(PlayerControl target, RoleEnum role, bool condition = true) => target != null && target.Data != null && !target.Data.IsDead &&
            GameStates.IsRoaming && (HudManager.Instance.UseButton.isActiveAndEnabled || HudManager.Instance.PetButton.isActiveAndEnabled) && target.Is(role) && target ==
            PlayerControl.LocalPlayer && condition;

        public static bool SetDeadActive(PlayerControl target, RoleEnum role, bool condition = true) => target != null && target.Data != null && GameStates.IsRoaming &&
            condition && (HudManager.Instance.UseButton.isActiveAndEnabled || HudManager.Instance.PetButton.isActiveAndEnabled) && target.Is(role) && target == PlayerControl.LocalPlayer;

        public static bool SetActive(PlayerControl target, ObjectifierEnum obj, bool condition = true) => target != null && target.Data != null && !target.Data.IsDead &&
            GameStates.IsRoaming && (HudManager.Instance.UseButton.isActiveAndEnabled || HudManager.Instance.PetButton.isActiveAndEnabled) && target.Is(obj) && target ==
            PlayerControl.LocalPlayer && condition;

        public static bool SetActive(PlayerControl target, AbilityEnum ability, bool condition = true) => target != null && target.Data != null && !target.Data.IsDead &&
            GameStates.IsRoaming && (HudManager.Instance.UseButton.isActiveAndEnabled || HudManager.Instance.PetButton.isActiveAndEnabled) && target.Is(ability) && target ==
            PlayerControl.LocalPlayer && condition;

        public static bool NoButton(PlayerControl target, RoleEnum role) => PlayerControl.AllPlayerControls.Count <= 1 || target == null || target.Data == null || !target.CanMove ||
            !target.Is(role) || !GameStates.IsRoaming || MeetingHud.Instance;

        public static bool NoButton(PlayerControl target, Faction faction) => PlayerControl.AllPlayerControls.Count <= 1 || target == null || target.Data == null || !target.CanMove ||
            !target.Is(faction) || !GameStates.IsRoaming || MeetingHud.Instance;

        public static bool NoButton(PlayerControl target, ObjectifierEnum obj) => PlayerControl.AllPlayerControls.Count <= 1 || target == null || target.Data == null || !target.CanMove ||
            !target.Is(obj) || !GameStates.IsRoaming || MeetingHud.Instance;

        public static bool NoButton(PlayerControl target, AbilityEnum ability) => PlayerControl.AllPlayerControls.Count <= 1 || target == null || target.Data == null || !target.CanMove ||
            !target.Is(ability) || !GameStates.IsRoaming || MeetingHud.Instance;

        public static bool IsBlocked(this PlayerControl player)
        {
            if (player == null)
                return false;

            var role = Role.GetRole(player);

            if (role == null)
                return false;

            return role.IsBlocked;
        }

        public static bool SeemsEvil(this PlayerControl player)
        {
            var intruderFlag = player.Is(Faction.Intruder) && !player.Is(ObjectifierEnum.Traitor) && !player.Is(ObjectifierEnum.Fanatic) && !player.Is(RoleEnum.Godfather);
            var syndicateFlag = player.Is(Faction.Syndicate) && !player.Is(ObjectifierEnum.Traitor) && !player.Is(ObjectifierEnum.Fanatic) && !player.Is(RoleEnum.Rebel);
            var traitorFlag = player.IsTurnedTraitor() && CustomGameOptions.TraitorColourSwap;
            var fanaticFlag = player.IsTurnedFanatic() && CustomGameOptions.FanaticColourSwap;
            var nkFlag = player.Is(RoleAlignment.NeutralKill) && !CustomGameOptions.NeutKillingRed;
            var neFlag = player.Is(RoleAlignment.NeutralEvil) && !CustomGameOptions.NeutEvilRed;
            var framedFlag = player.IsFramed();

            return intruderFlag || syndicateFlag || traitorFlag || nkFlag || neFlag || framedFlag;
        }

        public static bool SeemsGood(this PlayerControl player) => !SeemsEvil(player);

        public static bool SeemsEvil(this PlayerVoteArea player) => PlayerByVoteArea(player).SeemsEvil();

        public static bool SeemsGood(this PlayerVoteArea player) => PlayerByVoteArea(player).SeemsGood();

        public static bool IsBlockImmune(PlayerControl player) => (bool)Role.GetRole(player)?.RoleBlockImmune;

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

        public static float GetModifiedCooldown(float cooldown, float difference = 0f, float factor = 1f) => (cooldown * factor) + difference;

        public static float GetUnderdogChange(PlayerControl player)
        {
            if (!player.Is(AbilityEnum.Underdog))
                return 0f;

            var last = player.Is(Faction.Intruder) ? LastImp() : LastSyn();
            var lowerKC = -CustomGameOptions.UnderdogKillBonus;
            var upperKC = CustomGameOptions.UnderdogKillBonus;

            if (CustomGameOptions.UnderdogIncreasedKC && !last)
                return upperKC;
            else if (last)
                return lowerKC;
            else
                return 0f;
        }

        public static bool Check(int probability)
        {
            if (probability == 0)
                return false;

            if (probability == 100)
                return true;

            var num = Random.RandomRangeInt(1, 100);
            return num <= probability;
        }

        public static bool HasTarget(this Role role) => role.RoleType == RoleEnum.Executioner || role.RoleType == RoleEnum.GuardianAngel || role.RoleType == RoleEnum.Guesser ||
            role.RoleType == RoleEnum.BountyHunter;

        public static bool ButtonUsable(ActionButton button) => button.isActiveAndEnabled && !button.isCoolingDown && !PlayerControl.LocalPlayer.inVent &&
            !PlayerControl.LocalPlayer.inMovingPlat;

        public static List<object> AllPlayerInfo(this PlayerControl player)
        {
            var role = Role.GetRole(player);
            var modifier = Modifier.GetModifier(player);
            var ability = Ability.GetAbility(player);
            var objectifier = Objectifier.GetObjectifier(player);

            var info = new List<object>();

            info.Add(role); //0
            info.Add(modifier); //1
            info.Add(ability); //2
            info.Add(objectifier); //3

            return info;
        }

        public static List<object> AllPlayerInfo(this PlayerVoteArea player) => PlayerByVoteArea(player).AllPlayerInfo();

        public static PlayerControl GetOtherLover(this PlayerControl player)
        {
            if (!player.Is(ObjectifierEnum.Lovers))
                return null;

            return Objectifier.GetObjectifier<Lovers>(player).OtherLover;
        }

        public static PlayerControl GetOtherRival(this PlayerControl player)
        {
            if (!player.Is(ObjectifierEnum.Rivals))
                return null;

            return Objectifier.GetObjectifier<Rivals>(player).OtherRival;
        }

        public static string GetEndGameName(string name)
        {
            foreach (var role in Role.AllRoles)
            {
                if (role.PlayerName == name)
                    return $"{role.ColorString}<size=75%>{role.Name}</size>\n<size=90%>{name}</size></color>";
            }

            return "";
        }

        public static bool NeutralHasUnfinishedBusiness(PlayerControl player)
        {
            if (player.Is(RoleEnum.GuardianAngel))
            {
                var ga = Role.GetRole<GuardianAngel>(player);
                return ga.TargetAlive;
            }
            else if (player.Is(RoleEnum.Executioner))
            {
                var exe = Role.GetRole<Executioner>(player);
                return exe.TargetVotedOut;
            }
            else if (player.Is(RoleEnum.Jester))
            {
                var jest = Role.GetRole<Jester>(player);
                return jest.VotedOut;
            }
            else if (player.Is(RoleEnum.Guesser))
            {
                var guess = Role.GetRole<Guesser>(player);
                return guess.TargetGuessed;
            }
            else if (player.Is(RoleEnum.BountyHunter))
            {
                var bh = Role.GetRole<BountyHunter>(player);
                return bh.TargetKilled;
            }
            else if (player.Is(RoleEnum.Actor))
            {
                var act = Role.GetRole<Actor>(player);
                return act.Guessed;
            }

            return false;
        }

        public static string GetTaskList(this PlayerControl player)
        {
            var info = player.AllPlayerInfo();

            var role = info[0] as Role;
            var modifier = info[1] as Modifier;
            var ability = info[2] as Ability;
            var objectifier = info[3] as Objectifier;

            string objectives = $"{ObjectivesColorString}Objectives:";
            string abilities = $"{AbilitiesColorString}Abilities:";
            string attributes = $"{AttributesColorString}Attributes:";
            string roleName = $"{RoleColorString}Role: ";
            string objectifierName = $"{ObjectifierColorString}Objectifier: ";
            string abilityName = $"{AbilityColorString}Ability: ";
            string modifierName = $"{ModifierColorString}Modifier: ";
            string alignment = $"{AlignmentColorString}Alignment: ";

            if (info[0] != null)
            {
                roleName += $"{role.ColorString}{role.Name}</color>";
                objectives += $"\n{role.ColorString}{role.Objectives}</color>";
                alignment += $"{role.AlignmentName}";
            }
            else
            {
                roleName += "None";
                objectives += "\n- None.";
                alignment += "None";
            }

            roleName += "</color>";
            alignment += "</color>";

            if (info[3] != null)
            {
                if (!objectifier.Hidden)
                {
                    objectives += $"\n{objectifier.ColorString}{objectifier.TaskText}</color>";
                    objectifierName += $"{objectifier.ColorString}{objectifier.Name}</color>";
                }
            }
            else
                objectifierName += "None";

            objectifierName += "</color>";

            if (info[2] != null)
            {
                if (!ability.Hidden)
                    abilityName += $"{ability.ColorString}{ability.Name}</color>";
            }
            else
                abilityName += "None";

            abilityName += "</color>";

            if (info[1] != null)
            {
                if (!modifier.Hidden)
                    modifierName += $"{modifier.ColorString}{modifier.Name}</color>";
            }
            else
                modifierName += "None";

            modifierName += "</color>";

            if (player.IsRecruit())
            {
                var jackal = player.GetJackal();
                objectives += $"\n<color=#" + Colors.Cabal.ToHtmlStringRGBA() + $">- You are a member of the Cabal. Help {jackal.PlayerName} in taking over the mission.</color>";
            }
            else if (player.IsResurrected())
            {
                var necromancer = player.GetNecromancer();
                objectives += $"\n<color=#" + Colors.Reanimated.ToHtmlStringRGBA() + $">- You are a member of the Reanimated. Help {necromancer.PlayerName} in taking over the mission.</color>";
            }
            else if (player.IsPersuaded())
            {
                var whisperer = player.GetWhisperer();
                objectives += $"\n<color=#" + Colors.Sect.ToHtmlStringRGBA() + $">- You are a member of the Sect. Help {whisperer.PlayerName} in taking over the mission.</color>";
            }
            else if (player.IsBitten())
            {
                var dracula = player.GetDracula();
                objectives += $"\n<color=#" + Colors.Dracula.ToHtmlStringRGBA() + $">- You are a member of the Undead. Help {dracula.PlayerName} in taking over the mission." + 
                    "\n- Attempting to interact with a <color=#C0C0C0FF>Vampire Hunter</color> will force them to kill you.</color>";
            }

            objectives += "</color>";

            var hassomething = false;

            if (info[0] != null)
            {
                abilities += $"\n{role.ColorString}{role.AbilitiesText}</color>";
                hassomething = true;
            }

            if (info[2] != null)
            {
                if (!ability.Hidden)
                {
                    abilities += $"\n{ability.ColorString}{ability.TaskText}</color>";
                    hassomething = true;
                }
            }

            if (!hassomething)
                abilities += $"\n- None.";

            abilities += "</color>";

            var hasnothing = true;

            if (info[1] != null)
            {
                if (!modifier.Hidden)
                {
                    attributes += $"\n{modifier.ColorString}{modifier.TaskText}</color>";
                    hasnothing = false;
                }
            }

            if (player.IsGuessTarget() && CustomGameOptions.GuesserTargetKnows)
            {
                attributes += "\n<color=#EEE5BEFF>- Someone wants to guess you.</color>";
                hasnothing = false;
            }

            if (player.IsExeTarget() && CustomGameOptions.ExeTargetKnows)
            {
                attributes += "\n<color=#CCCCCCFF>- Someone wants you ejected.</color>";
                hasnothing = false;
            }

            if (player.IsGATarget() && CustomGameOptions.GATargetKnows)
            {
                attributes += "\n<color=#FFFFFFFF>- Someone wants to protect you.</color>";
                hasnothing = false;
            }

            if (player.IsBHTarget())
            {
                attributes += "\n<color=#B51E39FF>- There is a bounty on your head.</color>";
                hasnothing = false;
            }

            if (player.Data.IsDead)
            {
                attributes += "\n<color=#FF0000FF>- You are dead.</color>";
                hasnothing = false;
            }

            if (!player.CanDoTasks())
            {
                attributes += "\n- Your tasks are fake.";
                hasnothing = false;
            }

            if (hasnothing)
                attributes += "\n- None.";
            
            attributes += "</color>";

            var tasks = $"{roleName}\n{objectifierName}\n{abilityName}\n{modifierName}\n{alignment}\n{objectives}\n{abilities}\n{attributes}\n<color=#FFFFFFFF>Tasks:</color>";
            return tasks;
        }

        public static void RegenTask(this PlayerControl player)
        {
            try
            {
                foreach (var task2 in player.myTasks.ToArray())
                {
                    var importantTextTask = task2.Cast<ImportantTextTask>();

                    if (importantTextTask.Text.Contains("Sabotage and kill everyone") || importantTextTask.Text.Contains("Fake Tasks") || importantTextTask.Text.Contains("Role") ||
                        importantTextTask.Text.Contains("tasks to win!"))
                        player.myTasks.Remove(importantTextTask);
                }
            } catch {}

            var task = new GameObject("DetailTask").AddComponent<ImportantTextTask>();
            task.transform.SetParent(player.transform, false);
            task.Text = player.GetTaskList();
            player.myTasks.Insert(0, task);
        }

        public static Sprite CreateSprite(string name)
        {
            var pixelsPerUnit = 100f;
            var pivot = new Vector2(0.5f, 0.5f);
            var assembly = Assembly.GetExecutingAssembly();
            var tex = CreateEmptyTexture();
            var imageStream = assembly.GetManifestResourceStream(name);
            var img = imageStream.ReadFully();
            LoadImage(tex, img, true);
            tex.DontDestroy();
            var sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), pivot, pixelsPerUnit);
            sprite.DontDestroy();
            return sprite;
        }

        public static void LoadImage(Texture2D tex, byte[] data, bool markNonReadable)
        {
            _iCallLoadImage ??= IL2CPP.ResolveICall<DLoadImage>("UnityEngine.ImageConversion::LoadImage");
            var il2CPPArray = (Il2CppStructArray<byte>) data;
            _iCallLoadImage.Invoke(tex.Pointer, il2CPPArray.Pointer, markNonReadable);
        }

        public static AudioClip CreateAudio(string path)
        {
            try
            {
                var name = path;
                name.Replace($"{TownOfUsReworked.Sounds}", "");
                name.Replace(".raw", "");
                var executingAssembly = Assembly.GetExecutingAssembly();
                var manifestResourceStream = executingAssembly.GetManifestResourceStream(path);
                var array = new byte[manifestResourceStream.Length];
                manifestResourceStream.Read(array, 0, (int)manifestResourceStream.Length);
                var array2 = new float[array.Length / 4];

                for (int i = 0; i < array2.Length; i++)
                {
                    int startIndex = i * 4;
                    array2[i] = (float)BitConverter.ToInt32(array, startIndex) / Int32.MaxValue;
                }

                var audioClip = AudioClip.Create(name, array2.Length, 2, 48000, false).DontDestroy();
                audioClip.SetData(array2, 0);
                return audioClip;
            }
            catch
            {
                return null;
            }
        }

        public static void LogSomething(object message) => PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage(message);

        public static string CreateText(string itemName, string folder = "", string subfolder = "")
        {
            var resourceName = "";

            if (subfolder != "" && folder != "")
                resourceName = $"{TownOfUsReworked.Resources}{folder}.{subfolder}.{itemName}";
            else if (subfolder == "" && folder != "")
                resourceName = $"{TownOfUsReworked.Resources}{folder}.{itemName}";
            else 
                resourceName = $"{TownOfUsReworked.Resources}{itemName}";

            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(resourceName);
            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public static bool NKHasWon() => Role.AllRoles.Any(x => x.RoleAlignment == RoleAlignment.NeutralKill && NKWins(x.RoleType));

        public static bool LoverRivalWins() => Objectifier.AllObjectifiers.Any(x => (x.ObjectifierType == ObjectifierEnum.Rivals && ((Rivals)x).RivalWins) || (x.ObjectifierType ==
            ObjectifierEnum.Lovers && ((Lovers)x).LoveWins));

        public static bool GameHasEnded() => SyndicateWins() || IntrudersWin() || Sabotaged() || TasksDone() || CrewWins() || PestOrPBWins() || NKHasWon() || CabalWin() || SectWin() ||
            ReanimatedWin() || UndeadWin() || AllNeutralsWin() || AllNKsWin() || CorruptedWin() || LoverRivalWins();

        public static bool IsInRange(this float num, float min, float max, bool minInclusive = false, bool maxInclusive = false)
        {
            if (minInclusive && maxInclusive)
                return num >= min && num <= max;
            else if (minInclusive)
                return num >= min && num < max;
            else if (maxInclusive)
                return num > min && num <= max;
            else
                return num > min && num < max;
        }

        public static PlayerControl GetTarget(this PlayerControl player)
        {
            var role = Role.GetRole(player);

            if (!role.HasTarget())
                return null;

            if (player.Is(RoleEnum.Executioner))
                return ((Executioner)role).TargetPlayer;
            else if (player.Is(RoleEnum.GuardianAngel))
                return ((GuardianAngel)role).TargetPlayer;
            else if (player.Is(RoleEnum.Guesser))
                return ((Guesser)role).TargetPlayer;

            return null;
        }

        public static void RoleUpdate(this Role newRole, Role former)
        {
            newRole.RoleHistory.Add(former);
            newRole.RoleHistory.AddRange(former.RoleHistory);
            newRole.Faction = former.Faction;
            newRole.SubFaction = former.SubFaction;
            newRole.FactionColor = former.FactionColor;
            newRole.SubFactionColor = former.SubFactionColor;
            newRole.DeathReason = former.DeathReason;
            newRole.KilledBy = former.KilledBy;
            newRole.IsBitten = former.IsBitten;
            newRole.IsRecruit = former.IsRecruit;
            newRole.IsResurrected = former.IsResurrected;
            newRole.IsPersuaded = former.IsPersuaded;
            newRole.IsIntFanatic = former.IsIntFanatic;
            newRole.IsIntTraitor = former.IsIntTraitor;
            newRole.IsSynFanatic = former.IsSynFanatic;
            newRole.IsSynTraitor = former.IsSynTraitor;
            newRole.IsIntAlly = former.IsIntAlly;
            newRole.IsSynAlly = former.IsSynAlly;
            newRole.IsCrewAlly = former.IsCrewAlly;
        }
    
        public static void ShareGameVersion()
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.VersionHandshake, Hazel.SendOption.Reliable, -1);
            writer.Write((byte)TownOfUsReworked.Version.Major);
            writer.Write((byte)TownOfUsReworked.Version.Minor);
            writer.Write((byte)TownOfUsReworked.Version.Build);
            writer.Write(AmongUsClient.Instance.AmHost ? Patches.GameStartManagerPatch.timer : -1f);
            writer.WritePacked(AmongUsClient.Instance.ClientId);
            writer.Write((byte)(TownOfUsReworked.Version.Revision < 0 ? 0xFF : TownOfUsReworked.Version.Revision));
            writer.Write(Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId.ToByteArray());
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            VersionHandshake(TownOfUsReworked.Version.Major, TownOfUsReworked.Version.Minor, TownOfUsReworked.Version.Build, TownOfUsReworked.Version.Revision,
                Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId, AmongUsClient.Instance.ClientId);
        }

        public static void VersionHandshake(int major, int minor, int build, int revision, Guid guid, int clientId)
        {
            Version ver;

            if (revision < 0) 
                ver = new Version(major, minor, build);
            else 
                ver = new Version(major, minor, build, revision);

            GameStartManagerPatch.PlayerVersions[clientId] = new GameStartManagerPatch.PlayerVersion(ver, guid);
        }

        public static IEnumerable<T> GetFastEnumerator<T>(this Il2CppSystem.Collections.Generic.List<T> list) where T : Il2CppSystem.Object => new Il2CppListEnumerable<T>(list);

        public static string GetRandomisedName()
        {
            var everything = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()|{}[],.<>;':\"-+=*/`~_\\ ⟡☆♡♧♤ø▶❥✔εΔΓικνστυφψΨωχӪζδ♠♥βαµ♣✚Ξρλς§π★ηΛγΣΦΘξ";
            var length = Random.RandomRangeInt(1, 11);
            var position = 0;
            var name = "";

            while (position < length)
            {
                var random = Random.RandomRangeInt(0, everything.Length);
                name += everything[random];
                position++;
            }

            return name;
        }
    }
}