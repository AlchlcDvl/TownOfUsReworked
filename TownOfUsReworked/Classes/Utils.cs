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
using TownOfUsReworked.PlayerLayers;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using TownOfUsReworked.Objects;
using System.Reflection;
using Il2CppInterop.Runtime;
using System.IO;
using TMPro;
using AmongUs.GameOptions;

namespace TownOfUsReworked.Classes
{
    [HarmonyPatch]
    public static class Utils
    {
        public readonly static List<WinningPlayerData> PotentialWinners = new();
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

        public static SpriteRenderer MyRend(this PlayerControl p) => p.cosmetics.currentBodySprite.BodySprite;

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
                {
                    tie = true;
                }
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
                {
                    tie = true;
                }
            }

            return result;
        }

        public static VisualAppearance GetDefaultAppearance() => new();

        public static bool TryGetAppearance(IVisualAlteration modifier, out VisualAppearance appearance)
        {
            if (modifier != null)
                return modifier.TryGetModifiedAppearance(out appearance);

            appearance = GetDefaultAppearance();
            return false;
        }

        public static VisualAppearance GetAppearance(this PlayerControl player)
        {
            if (TryGetAppearance(Role.GetRole(player) as IVisualAlteration, out var appearance))
                return appearance;
            else if (TryGetAppearance(Modifier.GetModifier(player) as IVisualAlteration, out appearance))
                return appearance;
            else
                return GetDefaultAppearance();
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
            {
                playerControl.NameText().color = Color.white;
            }
        }

        public static CustomPlayerOutfitType GetCustomOutfitType(this PlayerControl playerControl) => (CustomPlayerOutfitType)playerControl.CurrentOutfitType;

        public static bool IsNullOrDestroyed(this Object obj)
        {
            if (obj is null)
                return true;

            if (obj is not null)
                return obj is not null;

            return false;
        }

        public static Texture2D CreateEmptyTexture(int width = 0, int height = 0) => new(width, height, TextureFormat.RGBA32, Texture.GenerateAllMips, false, IntPtr.Zero);

        public static void Morph(PlayerControl player, PlayerControl MorphedPlayer)
        {
            if (CamouflageUnCamouflage.IsCamoed)
                return;

            if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Morph)
                player.SetOutfit(CustomPlayerOutfitType.Morph, MorphedPlayer.Data.DefaultOutfit);
        }

        public static void DefaultOutfit(PlayerControl player)
        {
            player.MyRend().color = new Color32(255, 255, 255, 255);
            player.SetOutfit(CustomPlayerOutfitType.Default);
        }

        public static void Camouflage()
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
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

                    PlayerMaterial.SetColors(Color.grey, player.MyRend());
                    player.NameText().color = Color.clear;
                    player.cosmetics.colorBlindText.color = Color.clear;

                    if (CustomGameOptions.CamoHideSize)
                        player.GetAppearance().SizeFactor.Set(1f, 1f, 1f);

                    if (CustomGameOptions.CamoHideSpeed)
                        player.MyPhysics.body.velocity.Set(CustomGameOptions.PlayerSpeed, CustomGameOptions.PlayerSpeed);
                }
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

                    player.MyRend().color = color;
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

            foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel).Cast<GuardianAngel>())
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

            foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner).Cast<Executioner>())
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

            foreach (BountyHunter bh in Role.GetRoles(RoleEnum.BountyHunter).Cast<BountyHunter>())
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

            foreach (Guesser guess in Role.GetRoles(RoleEnum.Guesser).Cast<Guesser>())
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
            return Role.GetRoles(RoleEnum.Retributionist).FirstOrDefault(role =>
            {
                var shieldedPlayer = ((Retributionist)role).ShieldedPlayer;
                return shieldedPlayer != null && player == shieldedPlayer && ((Retributionist)role).RevivedRole?.RoleType == RoleEnum.Medic;
            }) as Retributionist;
        }

        public static Crusader GetCrusader(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Crusader).FirstOrDefault(role =>
            {
                var crusaded = ((Crusader)role).CrusadedPlayer;
                return crusaded != null && player == crusaded;
            }) as Crusader;
        }

        public static bool IsOnAlert(this PlayerControl player)
        {
            var vetFlag = Role.GetRoles(RoleEnum.Veteran).Any(role =>
            {
                var veteran = (Veteran)role;
                return veteran?.OnAlert == true && player == veteran.Player;
            });

            var retFlag = Role.GetRoles(RoleEnum.Retributionist).Any(role =>
            {
                var retributionist = (Retributionist)role;
                return retributionist?.OnAlert == true && player == retributionist.Player;
            });

            return vetFlag || retFlag;
        }

        public static bool IsVesting(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Survivor).Any(role =>
            {
                var surv = (Survivor)role;
                return surv?.Vesting == true && player == surv.Player;
            });
        }

        public static bool IsMarked(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Ghoul).Any(role =>
            {
                var ghoul = (Ghoul)role;
                return ghoul?.MarkedPlayer != null && player == ghoul.MarkedPlayer;
            });
        }

        public static bool IsAmbushed(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Ambusher).Any(role =>
            {
                var amb = (Ambusher)role;
                return (bool)amb?.OnAmbush && player == amb.AmbushedPlayer;
            });
        }

        public static bool IsCrusaded(this PlayerControl player)
        {
            var crusFlag = Role.GetRoles(RoleEnum.Crusader).Any(role =>
            {
                var crus = (Crusader)role;
                return (bool)crus?.OnCrusade && player == crus.CrusadedPlayer;
            });

            var rebFlag = Role.GetRoles(RoleEnum.Rebel).Any(role =>
            {
                var reb = (Rebel)role;
                return (bool)reb?.OnCrusade && player == reb.CrusadedPlayer;
            });

            return crusFlag || rebFlag;
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
                return (bool)plaguebearer?.InfectedPlayers.Contains(player.PlayerId) || player == plaguebearer?.Player;
            });
        }

        public static bool IsFramed(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Framer).Any(role =>
            {
                var framer = (Framer)role;
                return (bool)framer?.Framed.Contains(player.PlayerId);
            });
        }

        public static bool IsInfected(this PlayerVoteArea player) => PlayerByVoteArea(player).IsInfected();

        public static bool IsFramed(this PlayerVoteArea player) => PlayerByVoteArea(player).IsFramed();

        public static bool IsMarked(this PlayerVoteArea player) => PlayerByVoteArea(player).IsMarked();

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

        public static bool IsOtherLover(this PlayerControl player, PlayerControl refPlayer)
        {
            if (player == null || player.Data == null)
                return false;

            if (!player.Is(ObjectifierEnum.Lovers) || !refPlayer.Is(ObjectifierEnum.Lovers))
                return false;

            var lover1 = Objectifier.GetObjectifier<Lovers>(player);
            var lover2 = Objectifier.GetObjectifier<Lovers>(refPlayer);
            return lover1.OtherLover == refPlayer && lover2.OtherLover == player;
        }

        public static bool CrewWins() => !PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(Faction.Syndicate) ||
            x.Is(RoleAlignment.NeutralKill) || x.Is(RoleAlignment.NeutralNeo) || x.Is(RoleAlignment.NeutralPros) || x.NotOnTheSameSide()) && !x.IsCrewAlly());

        public static bool IntrudersWin() => !PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Crew) || x.NotOnTheSameSide() ||
            x.Is(RoleAlignment.NeutralKill) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralNeo) || x.Is(RoleAlignment.NeutralPros)) && !x.IntruderSided());

        public static bool SyndicateWins() => !PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(RoleAlignment.NeutralKill) ||
            x.Is(Faction.Intruder) || x.Is(RoleAlignment.NeutralNeo) || x.Is(Faction.Crew) || x.Is(RoleAlignment.NeutralPros) || x.NotOnTheSameSide()) && !x.SyndicateSided());

        public static bool AllNeutralsWin() => (!PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.NotOnTheSameSide() || x.Is(Faction.Crew) ||
            x.Is(Faction.Syndicate) || x.Is(Faction.Intruder)))) && CustomGameOptions.NoSolo == NoSolo.AllNeutrals;

        public static bool SameNKWins(RoleEnum nk)
        {
            if (nk == RoleEnum.Plaguebearer)
                return false;

            return !PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || (x.Is(RoleAlignment.NeutralKill) &&
                !x.Is(nk)) || x.Is(RoleAlignment.NeutralNeo) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralPros) || x.Is(ObjectifierEnum.Allied) || x.Is(Faction.Crew) ||
                x.NotOnTheSameSide())) && CustomGameOptions.NoSolo == NoSolo.SameNKs;
        }

        public static bool SoloNKWins(RoleEnum nk, PlayerControl player)
        {
            if (nk == RoleEnum.Plaguebearer)
                return false;

            return !PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || (x.Is(RoleAlignment.NeutralKill) &&
                x != player) || x.Is(RoleAlignment.NeutralNeo) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralPros) || x.Is(ObjectifierEnum.Allied) || x.Is(Faction.Crew) ||
                x.NotOnTheSameSide())) && CustomGameOptions.NoSolo == NoSolo.Never;
        }

        public static bool PestOrPBWins() => !PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) ||
            (x.Is(RoleAlignment.NeutralKill) && !x.Is(RoleEnum.Plaguebearer)) || x.Is(RoleAlignment.NeutralNeo) || x.Is(Faction.Syndicate) || (x.Is(RoleAlignment.NeutralPros) &&
            !x.Is(RoleEnum.Pestilence)) || x.Is(ObjectifierEnum.Allied) || x.Is(Faction.Crew) || x.NotOnTheSameSide()));

        public static bool AllNKsWin() => (!PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) ||
            x.Is(RoleAlignment.NeutralNeo) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralPros) || x.Is(Faction.Crew) || x.Is(ObjectifierEnum.Allied) || x.NotOnTheSameSide()))) &&
            CustomGameOptions.NoSolo == NoSolo.AllNKs;

        public static bool NoOneWins() => !PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected);

        public static bool CorruptedWin(PlayerControl player)
        {
            if (CustomGameOptions.AllCorruptedWin)
            {
                return !PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(ObjectifierEnum.Corrupted));
            }
            else
            {
                if (!player.Is(ObjectifierEnum.Corrupted))
                    return false;

                return !PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(ObjectifierEnum.Corrupted) && x != player);
            }
        }

        public static bool LoversWin(PlayerControl player)
        {
            if (!player.Is(ObjectifierEnum.Lovers))
                return false;

            var flag1 = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= 3;
            var flag2 = Objectifier.GetObjectifier<Lovers>(player).LoversAlive();
            return flag1 && flag2;
        }

        public static bool RivalsWin(PlayerControl player)
        {
            if (!player.Is(ObjectifierEnum.Rivals))
                return false;

            var flag1 = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= 2;
            var flag2 = Objectifier.GetObjectifier<Rivals>(player).RivalDead();
            return flag1 && flag2;
        }

        public static bool CabalWin() => !PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.IsBitten() ||
            x.Is(RoleAlignment.NeutralKill) || (x.Is(RoleAlignment.NeutralNeo) && !x.Is(RoleEnum.Jackal)) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralPros) ||
            x.Is(Faction.Crew) || x.Is(ObjectifierEnum.Lovers) || x.IsWinningRival() || x.IsResurrected() || x.IsPersuaded()) && !x.IsRecruit());

        public static bool UndeadWin() => !PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(Faction.Crew) ||
            x.Is(RoleAlignment.NeutralKill) || (x.Is(RoleAlignment.NeutralNeo) && !x.Is(RoleEnum.Dracula)) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralPros) ||
            x.Is(ObjectifierEnum.Lovers) || x.IsWinningRival() || x.IsResurrected() || x.IsRecruit() || x.IsPersuaded()) && !x.IsBitten());

        public static bool SectWin() => !PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.Is(Faction.Crew) ||
            x.Is(RoleAlignment.NeutralKill) || (x.Is(RoleAlignment.NeutralNeo) && !x.Is(RoleEnum.Whisperer)) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralPros) ||
            x.Is(ObjectifierEnum.Lovers) || x.IsWinningRival() || x.IsRecruit() || x.IsResurrected() || x.IsBitten()) && !x.IsPersuaded());

        public static bool ReanimatedWin() => !PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruder) || x.IsPersuaded() ||
            x.Is(RoleAlignment.NeutralKill) || (x.Is(RoleAlignment.NeutralNeo) && !x.Is(RoleEnum.Necromancer)) || x.Is(Faction.Syndicate) || x.Is(RoleAlignment.NeutralPros) ||
            x.Is(Faction.Crew) || x.Is(ObjectifierEnum.Lovers) || x.IsWinningRival() || x.IsRecruit() || x.IsBitten()) && !x.IsResurrected());

        public static bool SyndicateSided(this PlayerControl player) => player.IsSynTraitor() || player.IsSynFanatic() || player.IsSynAlly();

        public static bool IntruderSided(this PlayerControl player) => player.IsIntTraitor() || player.IsIntAlly() || player.IsIntFanatic();

        public static bool IsWinner(this string playerName)
        {
            foreach (var win in TempData.winners)
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

            return die + killedBy;
        }

        public static PlayerControl GetClosestPlayer(PlayerControl refPlayer, List<PlayerControl> AllPlayers = null, float maxDistance = 0f)
        {
            var truePosition = refPlayer.GetTruePosition();
            var closestDistance = double.MaxValue;
            PlayerControl closestPlayer = null;
            var targets = AllPlayers ?? PlayerControl.AllPlayerControls.ToArray().ToList();

            if (maxDistance == 0f)
                maxDistance = CustomGameOptions.InteractionDistance;

            foreach (var player in targets)
            {
                if (player.Data.IsDead || player == refPlayer || !player.Collider.enabled || player.inMovingPlat || player.onLadder || (player.inVent &&
                    !CustomGameOptions.VentTargetting))
                {
                    continue;
                }

                var distance = Vector2.Distance(truePosition, player.GetTruePosition());

                if (distance > closestDistance || distance > maxDistance)
                    continue;

                closestPlayer = player;
                closestDistance = distance;
            }

            return closestPlayer;
        }

        public static Vent GetClosestVent(PlayerControl refPlayer)
        {
            var truePosition = refPlayer.GetTruePosition();
            var maxDistance = CustomGameOptions.InteractionDistance / 2;
            var closestDistance = double.MaxValue;
            var allVents = Object.FindObjectsOfType<Vent>();
            Vent closestVent = null;

            foreach (var vent in allVents)
            {
                var distance = Vector2.Distance(truePosition, new Vector2(vent.transform.position.x, vent.transform.position.y));

                if (distance > maxDistance || distance > closestDistance)
                    continue;

                closestVent = vent;
                closestDistance = distance;
            }

            return closestVent;
        }

        public static bool CanInteractWithDead(this PlayerControl player) => player != null && !MeetingHud.Instance && (player.Is(RoleEnum.Altruist) || player.Is(RoleEnum.Amnesiac) ||
            player.Is(RoleEnum.Cannibal) || player.Is(RoleEnum.Necromancer) || player.Is(RoleEnum.Janitor) || player.Is(RoleEnum.Godfather) || player.Is(RoleEnum.Coroner) ||
            player.Is(RoleEnum.Undertaker) || player.Is(RoleEnum.Retributionist));

        public static void SetDeadTarget(this AbilityButton button, Role role, bool condition = true)
        {
            if (role.Player != PlayerControl.LocalPlayer)
                return;

            if (!ButtonUsable(button) || !CanInteractWithDead(PlayerControl.LocalPlayer))
                return;

            var target = GetClosestDeadPlayer(PlayerControl.LocalPlayer);
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

            if (target != null && !button.isCoolingDown && condition && !CannotUse(PlayerControl.LocalPlayer))
            {
                component.material.SetFloat("_Outline", 1f);
                component.material.SetColor("_OutlineColor", role.Color);
                button.SetEnabled();
            }
            else
            {
                button.SetDisabled();
            }
        }

        public static bool CanInteract(PlayerControl player) => player != null && !MeetingHud.Instance && (player.Is(RoleAlignment.NeutralKill) || player.Is(RoleEnum.Thief) ||
            player.Is(Faction.Intruder) || player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.Seer) || player.Is(RoleEnum.Detective) || player.Is(RoleEnum.VampireHunter) ||
            player.Is(RoleEnum.Medic) || player.Is(RoleEnum.Shifter) || player.Is(RoleEnum.Mystic) || player.Is(RoleAlignment.NeutralNeo) || player.Is(RoleEnum.Tracker) ||
            player.Is(RoleEnum.Vigilante) || player.Is(Faction.Syndicate) || player.Is(RoleEnum.Inspector) || player.Is(RoleEnum.Escort) || player.Is(RoleEnum.Troll) ||
            player.Is(RoleEnum.BountyHunter) || player.Is(ObjectifierEnum.Corrupted) || player.Is(RoleEnum.Retributionist) || player.Is(RoleEnum.Coroner) || player.Is(RoleEnum.Ghoul) ||
            player.Is(RoleEnum.Pestilence) || player.Is(RoleEnum.Jester) || player.Is(RoleEnum.Executioner) || player.Is(RoleEnum.Betrayer));

        public static void SetAliveTarget(this AbilityButton button, Role role, bool condition = true, List<PlayerControl> targets = null)
        {
            if (role.Player != PlayerControl.LocalPlayer)
                return;

            if (!ButtonUsable(button) || !CanInteract(PlayerControl.LocalPlayer))
                return;

            var target = GetClosestPlayer(PlayerControl.LocalPlayer, targets ?? PlayerControl.AllPlayerControls.ToArray().ToList());
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

            var oldComponent = oldTarget?.MyRend();

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
                    {
                        break;
                    }

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
                    oldTarget3 = reb.ClosestPoison;
                    reb.ClosestSyndicate = target;
                    reb.ClosestFrame = target;
                    reb.ClosestCrusade = target;
                    reb.ClosestPoison = target;
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

                case RoleEnum.Ghoul:
                    var ghoul = (Ghoul)role;
                    oldTarget = ghoul.ClosestMark;
                    ghoul.ClosestMark = target;
                    break;
            }

            oldComponent = oldTarget?.MyRend();

            if (target != oldTarget && oldTarget != null)
                oldComponent?.material.SetFloat("_Outline", 0f);

            oldComponent = oldTarget1?.MyRend();

            if (target != oldTarget1 && oldTarget1 != null)
                oldComponent?.material.SetFloat("_Outline", 0f);

            oldComponent = oldTarget2?.MyRend();

            if (target != oldTarget2 && oldTarget2 != null)
                oldComponent?.material.SetFloat("_Outline", 0f);

            oldComponent = oldTarget3?.MyRend();

            if (target != oldTarget3 && oldTarget3 != null)
                oldComponent?.material.SetFloat("_Outline", 0f);

            if (target != null && !button.isCoolingDown && condition && !CannotUse(PlayerControl.LocalPlayer))
            {
                var component = target?.MyRend();
                component.material.SetFloat("_Outline", 1f);
                component.material.SetColor("_OutlineColor", role.Color);
                button.SetEnabled();
            }
            else
            {
                button.SetDisabled();
            }
        }

        public static void SetAliveTarget(this AbilityButton button, Objectifier obj, List<PlayerControl> targets = null, bool condition = true)
        {
            if (obj.Player != PlayerControl.LocalPlayer)
                return;

            if (!ButtonUsable(button) || !CanInteract(PlayerControl.LocalPlayer))
                return;

            var target = GetClosestPlayer(PlayerControl.LocalPlayer, targets ?? PlayerControl.AllPlayerControls.ToArray().ToList());
            PlayerControl oldTarget = null;

            var oldComponent = oldTarget?.MyRend();

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

            var component = target?.MyRend();
            oldComponent = oldTarget?.MyRend();

            if (target != oldTarget && oldTarget != null && target != null)
                oldComponent?.material.SetFloat("_Outline", 0f);

            if (target != null && !button.isCoolingDown && condition && !CannotUse(PlayerControl.LocalPlayer))
            {
                component.material.SetFloat("_Outline", 1f);
                component.material.SetColor("_OutlineColor", obj.Color);
                button.SetEnabled();
            }
            else
            {
                button.SetDisabled();
            }
        }

        public static bool HasEffect(PlayerControl player) => player != null && !MeetingHud.Instance && (player.Is(RoleEnum.Camouflager) || player.Is(RoleEnum.Shapeshifter) ||
            player.Is(RoleEnum.Chameleon) || player.Is(RoleEnum.Engineer) || player.Is(RoleEnum.Medium) || player.Is(RoleEnum.Operative) || player.Is(RoleEnum.Retributionist) ||
            player.Is(RoleEnum.TimeLord) || player.Is(RoleEnum.Transporter) || player.Is(RoleEnum.Veteran) || player.Is(RoleEnum.Morphling) || player.Is(RoleEnum.Godfather) ||
            player.Is(RoleEnum.Grenadier) || player.Is(RoleEnum.Miner) || player.Is(RoleEnum.Teleporter) || player.Is(RoleEnum.TimeMaster) || player.Is(RoleEnum.Wraith) ||
            player.Is(RoleEnum.GuardianAngel) || player.Is(RoleEnum.Survivor) || player.Is(RoleEnum.Whisperer) || player.Is(RoleEnum.Bomber) || player.Is(RoleEnum.Concealer) ||
            player.Is(RoleEnum.Warper) || (player.Is(RoleEnum.Framer) && Role.SyndicateHasChaosDrive) || player.Is(RoleEnum.Drunkard) || player.Is(RoleEnum.Rebel) ||
            player.Is(AbilityEnum.ButtonBarry));

        public static void SetEffectTarget(this AbilityButton button, bool condition = true)
        {
            if (!ButtonUsable(button) || !HasEffect(PlayerControl.LocalPlayer))
                return;

            if (!button.isCoolingDown && condition && !CannotUse(PlayerControl.LocalPlayer))
                button.SetEnabled();
            else
                button.SetDisabled();
        }

        public static bool CannotUse(PlayerControl player) => player.onLadder || player.IsBlocked() || player.inVent || player.inMovingPlat;

        public static void UpdateButton(this AbilityButton button, Role role, string label, float timer, float maxTimer, Sprite sprite, AbilityTypes type, string keybind,
            List<PlayerControl> targets = null, bool usable = true, bool condition = true, bool effectActive = false, float effectTimer = 0f, float maxDuration = 1f, bool usesActive =
            false, int uses = 0, bool postDeath = false)
        {
            if (button == null)
                return;

            if (type == AbilityTypes.Direct)
                button.SetAliveTarget(role, condition && !effectActive, targets);
            else if (type == AbilityTypes.Dead)
                button.SetDeadTarget(role, condition && !effectActive);
            else if (type == AbilityTypes.Effect)
                button.SetEffectTarget(condition && !effectActive);
            else
                return;

            button.graphic.sprite = role.IsBlocked ? AssetManager.Blocked : (sprite ?? AssetManager.Placeholder);
            var difference = GetUnderdogChange(PlayerControl.LocalPlayer);
            button.SetCoolDown(effectActive ? effectTimer : timer, effectActive ? maxDuration : (maxTimer + difference));
            button.buttonLabelText.text = label;
            button.commsDown?.gameObject?.SetActive(false);

            if (role.BaseFaction == Faction.Intruder && ((IntruderRole)role).KillButton == button)
                button.buttonLabelText.SetOutlineColor(role.FactionColor);
            else if (role.BaseFaction == Faction.Syndicate && ((SyndicateRole)role).KillButton == button)
                button.buttonLabelText.SetOutlineColor(role.FactionColor);
            else
                button.buttonLabelText.SetOutlineColor(role.Color);

            if (!usesActive)
                button.SetInfiniteUses();
            else
                button.SetUsesRemaining(uses);

            if (MeetingHud.Instance)
                button.gameObject.SetActive(false);
            else if (postDeath && PlayerControl.LocalPlayer.Data.IsDead)
                button.gameObject.SetActive(!MeetingHud.Instance && !GameStates.IsLobby && usable);
            else
                button.gameObject.SetActive(SetActive(PlayerControl.LocalPlayer, role.RoleType, usable));

            if (Rewired.ReInput.players.GetPlayer(0).GetButtonDown(keybind) && !effectActive)
                button.DoClick();
        }

        public static void UpdateButton(this AbilityButton button, Role role, string label, float timer, float maxTimer, Sprite sprite, AbilityTypes type, string keybind,
            List<PlayerControl> targets, bool usesActive, int uses, bool usable = true, bool condition = true) => button.UpdateButton(role, label, timer, maxTimer, sprite, type, keybind,
            targets, usable, condition, false, 0, 1, usesActive, uses);

        public static void UpdateButton(this AbilityButton button, Role role, string label, float timer, float maxTimer, Sprite sprite, AbilityTypes type, string keybind, bool usesActive,
            int uses, bool usable = true, bool condition = true) => button.UpdateButton(role, label, timer, maxTimer, sprite, type, keybind, null, usable, condition, false, 0, 1,
            usesActive, uses);

        public static void UpdateButton(this AbilityButton button, Role role, string label, float timer, float maxTimer, Sprite sprite, AbilityTypes type, string keybind, bool effectActive,
            float effectTimer, float effectDuration, bool usable = true, bool condition = true) => button.UpdateButton(role, label, timer, maxTimer, sprite, type, keybind, null,
            usable, condition, effectActive, effectTimer, effectDuration);

        public static void UpdateButton(this AbilityButton button, Role role, string label, float timer, float maxTimer, Sprite sprite, AbilityTypes type, string keybind, bool usable,
            bool condition) => button.UpdateButton(role, label, timer, maxTimer, sprite, type, keybind, null, usable, condition);

        public static void UpdateButton(this AbilityButton button, Role role, string label, float timer, float maxTimer, Sprite sprite, AbilityTypes type, string keybind, bool usable) =>
            button.UpdateButton(role, label, timer, maxTimer, sprite, type, keybind, null, usable);

        public static AbilityButton InstantiateButton()
        {
            var button = Object.Instantiate(HudManager.Instance.AbilityButton, HudManager.Instance.AbilityButton.transform.parent);
            button.graphic.enabled = true;
            button.buttonLabelText.enabled = true;
            button.buttonLabelText.fontSharedMaterial = HudManager.Instance.SabotageButton.buttonLabelText.fontSharedMaterial;
            button.usesRemainingText.enabled = true;
            button.usesRemainingSprite.enabled = true;
            button.commsDown?.gameObject?.SetActive(false);
            return button;
        }

        public static void UpdateButton(this AbilityButton button, Objectifier obj, float timer, float maxTimer, string label, Sprite sprite, AbilityTypes type, string keybind,
            List<PlayerControl> targets = null, bool condition = true, bool usesActive = false, int uses = 0)
        {
            if (button == null)
                return;

            if (CanInteract(PlayerControl.LocalPlayer) && type == AbilityTypes.Direct)
                button.SetAliveTarget(obj, targets, condition);

            button.graphic.sprite = obj.Player.IsBlocked() ? AssetManager.Blocked : (sprite ?? AssetManager.Placeholder);
            button.buttonLabelText.text = label;
            button.buttonLabelText.SetOutlineColor(obj.Color);
            button.commsDown?.gameObject?.SetActive(false);
            var difference = GetUnderdogChange(PlayerControl.LocalPlayer);
            button.SetCoolDown(timer, maxTimer + difference);

            if (!usesActive)
                button.SetInfiniteUses();
            else
                button.SetUsesRemaining(uses);

            if (MeetingHud.Instance)
                button.gameObject.SetActive(false);
            else
                button.gameObject.SetActive(SetActive(PlayerControl.LocalPlayer, obj.ObjectifierType, condition));

            if (Rewired.ReInput.players.GetPlayer(0).GetButtonDown(keybind))
                button.DoClick();
        }

        public static void UpdateButton(this AbilityButton button, Ability ability, Sprite sprite, string label, AbilityTypes type, bool condition, float timer, float maxTimer,
            string keybind, bool usesActive = false, int uses = 0)
        {
            if (button == null)
                return;

            if (HasEffect(PlayerControl.LocalPlayer) && type == AbilityTypes.Effect)
                button.SetEffectTarget(condition);

            button.graphic.sprite = ability.Player.IsBlocked() ? AssetManager.Blocked : (sprite ?? AssetManager.Placeholder);
            button.buttonLabelText.text = label;
            button.buttonLabelText.SetOutlineColor(ability.Color);
            button.commsDown?.gameObject?.SetActive(false);
            var difference = GetUnderdogChange(PlayerControl.LocalPlayer);
            button.SetCoolDown(timer, maxTimer + difference);

            if (!usesActive)
                button.SetInfiniteUses();
            else
                button.SetUsesRemaining(uses);

            if (MeetingHud.Instance)
                button.gameObject.SetActive(false);
            else
                button.gameObject.SetActive(SetActive(PlayerControl.LocalPlayer, ability.AbilityType, condition));

            if (Rewired.ReInput.players.GetPlayer(0).GetButtonDown(keybind))
                button.DoClick();
        }

        public static DeadBody GetClosestDeadPlayer(PlayerControl player, float maxDistance = 0f)
        {
            var truePosition = player.GetTruePosition();
            var closestDistance = double.MaxValue;
            var allBodies = Object.FindObjectsOfType<DeadBody>();
            DeadBody closestBody = null;

            if (maxDistance == 0f)
                maxDistance = CustomGameOptions.InteractionDistance;

            foreach (var body in allBodies)
            {
                var distance = Vector2.Distance(truePosition, body.TruePosition);

                if (distance > maxDistance || distance > closestDistance)
                    continue;

                closestBody = body;
                closestDistance = distance;
            }

            return closestBody;
        }

        public static Vector2 GetSize() => Vector2.Scale(Object.FindObjectsOfType<Vent>()[0].GetComponent<BoxCollider2D>().size, Object.FindObjectsOfType<Vent>()[0].transform.localScale)
            * 0.75f;

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

        public static void RpcMurderPlayer(PlayerControl killer, PlayerControl target, bool lunge = true)
        {
            if (killer == null || target == null)
                return;

            MurderPlayer(killer, target, lunge);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.BypassKill);
            writer.Write(killer.PlayerId);
            writer.Write(target.PlayerId);
            writer.Write(lunge);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static void MurderPlayer(PlayerControl killer, PlayerControl target, bool lunge = true)
        {
            if (killer == null || target == null)
                return;

            var data = target.Data;
            lunge = !killer.Is(AbilityEnum.Ninja) && lunge && killer != target;

            if (data?.IsDead == false)
            {
                if (killer == PlayerControl.LocalPlayer)
                    SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 1f);

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
                    if (Minigame.Instance)
                        Minigame.Instance.Close();

                    if (MapBehaviour.Instance)
                        MapBehaviour.Instance.Close();

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
                {
                    targetRole.DeathReason = DeathReasonEnum.Suicide;
                }

                var deadBody = new DeadPlayer
                {
                    PlayerId = target.PlayerId,
                    KillerId = killer.PlayerId,
                    KillTime = DateTime.UtcNow
                };

                if (killer == target)
                    return;

                Murder.KilledPlayers.Add(deadBody);
                target.RpcSetRole(target.Data.IsImpostor() ? RoleTypes.ImpostorGhost : RoleTypes.CrewmateGhost);

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
                {
                    BaitReport(killer, target);
                }
            }
        }

        public static void BaitReport(PlayerControl killer, PlayerControl target) => Coroutines.Start(BaitReportDelay(killer, target));

        public static IEnumerator BaitReportDelay(PlayerControl killer, PlayerControl target)
        {
            if (killer == null || target == null || killer == target)
                yield break;

            var extraDelay = Random.RandomRangeInt(0, (int)((100 * (CustomGameOptions.BaitMaxDelay - CustomGameOptions.BaitMinDelay)) + 1));

            if (CustomGameOptions.BaitMaxDelay <= CustomGameOptions.BaitMinDelay)
                yield return new WaitForSeconds(CustomGameOptions.BaitMaxDelay + 0.01f);
            else
                yield return new WaitForSeconds(CustomGameOptions.BaitMinDelay + 0.01f + (extraDelay / 100f));

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
            color.a = 0.3f;

            if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
            {
                var fullscreen = HudManager.Instance.FullScreen;
                fullscreen.enabled = true;
                fullscreen.gameObject.active = true;
                fullscreen.color = color;
            }

            yield return new WaitForSeconds(1f);

            if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
            {
                var fullscreen = HudManager.Instance.FullScreen;

                if (fullscreen.color.Equals(color))
                {
                    fullscreen.color = new Color(1f, 0f, 0f, 0.3f);
                    var fs = false;

                    switch (GameOptionsManager.Instance.currentNormalGameOptions.MapId)
                    {
                        case 0:
                        case 3:
                            var reactor1 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

                            if (reactor1.IsActive)
                                fs = true;

                            var oxygen1 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

                            if (oxygen1.IsActive)
                                fs = true;

                            break;

                        case 1:
                            var reactor2 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

                            if (reactor2.IsActive)
                                fs = true;

                            var oxygen2 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

                            if (oxygen2.IsActive)
                                fs = true;

                            break;

                        case 2:
                            var seismic = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();

                            if (seismic.IsActive)
                                fs = true;

                            break;

                        case 4:
                            var reactor = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<HeliSabotageSystem>();

                            if (reactor.IsActive)
                                fs = true;

                            break;

                        case 5:
                            if (!SubmergedCompatibility.Loaded)
                                break;

                            var reactor5 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

                            if (reactor5.IsActive)
                                fs = true;

                            break;

                        case 6:
                            var reactor6 = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();

                            if (reactor6.IsActive)
                                fs = true;

                            var oxygen6 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

                            if (oxygen6.IsActive)
                                fs = true;

                            break;
                    }

                    fullscreen.enabled = fs;
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
            var local = PlayerControl.LocalPlayer;
            var role = Role.GetRole(local);

            if (role == null)
                return;

            local.RegenTask();

            //Crew cooldowns
            if (local.Is(RoleEnum.Chameleon))
            {
                var role2 = (Chameleon)role;
                role2.LastSwooped = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Detective))
            {
                var role2 = (Detective)role;
                role2.LastExamined = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Escort))
            {
                var role2 = (Escort)role;
                role2.LastBlock = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Inspector))
            {
                var role2 = (Inspector)role;
                role2.LastInspected = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Coroner))
            {
                var role2 = (Coroner)role;
                role2.LastCompared = DateTime.UtcNow;
                role2.LastAutopsied = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Medium))
            {
                var role2 = (Medium)role;
                role2.LastMediated = DateTime.UtcNow;
                role2.MediatedPlayers.Values.DestroyAll();
                role2.MediatedPlayers.Clear();
            }
            else if (local.Is(RoleEnum.Operative))
            {
                var role2 = (Operative)role;
                role2.LastBugged = DateTime.UtcNow;
                role2.BuggedPlayers.Clear();

                if (CustomGameOptions.BugsRemoveOnNewRound)
                    role2.Bugs.ClearBugs();
            }
            else if (local.Is(RoleEnum.Sheriff))
            {
                var role2 = (Sheriff)role;
                role2.LastInterrogated = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Shifter))
            {
                var role2 = (Shifter)role;
                role2.LastShifted = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.TimeLord))
            {
                var role2 = (TimeLord)role;
                role2.FinishRewind = DateTime.UtcNow;
                role2.StartRewind = DateTime.UtcNow.AddSeconds(-10.0f);
            }
            else if (local.Is(RoleEnum.Tracker))
            {
                var role2 = (Tracker)role;
                role2.LastTracked = DateTime.UtcNow;

                if (CustomGameOptions.ResetOnNewRound)
                {
                    role2.UsesLeft = CustomGameOptions.MaxTracks;
                    role2.TrackerArrows.Values.DestroyAll();
                    role2.TrackerArrows.Clear();
                }
            }
            else if (local.Is(RoleEnum.Transporter))
            {
                var role2 = (Transporter)role;
                role2.LastTransported = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.VampireHunter))
            {
                var role2 = (VampireHunter)role;
                role2.LastStaked = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Veteran))
            {
                var role2 = (Veteran)role;
                role2.LastAlerted = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Vigilante))
            {
                var role2 = (Vigilante)role;
                role2.FirstRound = false;
                role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Retributionist))
            {
                var role2 = (Retributionist)role;

                if (role2.RevivedRole == null)
                    return;

                switch (role2.RevivedRole.RoleType)
                {
                    case RoleEnum.Chameleon:
                        role2.LastSwooped = DateTime.UtcNow;
                        break;

                    case RoleEnum.Detective:
                        role2.LastExamined = DateTime.UtcNow;
                        break;

                    case RoleEnum.Vigilante:
                        role2.LastKilled = DateTime.UtcNow;
                        break;

                    case RoleEnum.VampireHunter:
                        role2.LastStaked = DateTime.UtcNow;
                        break;

                    case RoleEnum.Veteran:
                        role2.LastAlerted = DateTime.UtcNow;
                        break;

                    case RoleEnum.Tracker:
                        role2.LastTracked = DateTime.UtcNow;

                        if (CustomGameOptions.ResetOnNewRound)
                        {
                            role2.TrackUsesLeft = CustomGameOptions.MaxTracks;
                            role2.TrackerArrows.Values.DestroyAll();
                            role2.TrackerArrows.Clear();
                        }

                        break;

                    case RoleEnum.Sheriff:
                        role2.LastInterrogated = DateTime.UtcNow;
                        break;

                    case RoleEnum.Medium:
                        role2.LastMediated = DateTime.UtcNow;
                        role2.MediatedPlayers.Values.DestroyAll();
                        role2.MediatedPlayers.Clear();
                        break;

                    case RoleEnum.Operative:
                        role2.LastBugged = DateTime.UtcNow;
                        role2.BuggedPlayers.Clear();

                        if (CustomGameOptions.BugsRemoveOnNewRound)
                            role2.Bugs.ClearBugs();

                        break;

                    case RoleEnum.Inspector:
                        role2.LastInspected = DateTime.UtcNow;
                        break;

                    default:
                        role2.RevivedRole = null;
                        break;
                }
            }
            else if (local.Is(RoleEnum.Blackmailer))
            {
                var role2 = (Blackmailer)role;

                if (role2.Player == PlayerControl.LocalPlayer)
                    role2.BlackmailedPlayer?.MyRend().material.SetFloat("_Outline", 0f);

                role2.BlackmailedPlayer = null;
                role2.LastBlackmailed = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Camouflager))
            {
                var role2 = (Camouflager)role;
                role2.LastCamouflaged = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Consigliere))
            {
                var role2 = (Consigliere)role;
                role2.LastInvestigated = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Consort))
            {
                var role2 = (Consort)role;
                role2.LastBlock = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Disguiser))
            {
                var role2 = (Disguiser)role;
                role2.LastDisguised = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
                role2.LastMeasured = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Godfather))
            {
                var role2 = (Godfather)role;
                role2.LastKilled = DateTime.UtcNow;

                if (!role2.HasDeclared)
                    role2.LastDeclared = DateTime.UtcNow;

                if (role2.FormerRole == null || role2.FormerRole?.RoleType == RoleEnum.Impostor || !role2.WasMafioso)
                    return;

                switch (role2.FormerRole.RoleType)
                {
                    case RoleEnum.Blackmailer:
                        if (role2.Player == PlayerControl.LocalPlayer)
                            role2.BlackmailedPlayer?.MyRend().material.SetFloat("_Outline", 0f);

                        role2.BlackmailedPlayer = null;
                        role2.LastBlackmailed = DateTime.UtcNow;
                        role2.LastKilled = DateTime.UtcNow;
                        break;

                    case RoleEnum.Camouflager:
                        role2.LastCamouflaged = DateTime.UtcNow;
                        break;

                    case RoleEnum.Consigliere:
                        role2.LastInvestigated = DateTime.UtcNow;
                        break;

                    case RoleEnum.Disguiser:
                        role2.LastDisguised = DateTime.UtcNow;
                        role2.LastMeasured = DateTime.UtcNow;
                        break;

                    case RoleEnum.Grenadier:
                        role2.LastFlashed = DateTime.UtcNow;
                        break;

                    case RoleEnum.Miner:
                        role2.LastMined = DateTime.UtcNow;
                        break;

                    case RoleEnum.Janitor:
                        role2.LastCleaned = DateTime.UtcNow;
                        break;

                    case RoleEnum.Morphling:
                        role2.LastMorphed = DateTime.UtcNow;
                        role2.LastSampled = DateTime.UtcNow;
                        break;

                    case RoleEnum.Teleporter:
                        role2.LastTeleport = DateTime.UtcNow;
                        break;

                    case RoleEnum.Undertaker:
                        role2.CurrentlyDragging = null;
                        role2.LastDragged = DateTime.UtcNow;
                        break;

                    case RoleEnum.TimeMaster:
                        role2.LastFrozen = DateTime.UtcNow;
                        break;

                    case RoleEnum.Wraith:
                        role2.LastInvis = DateTime.UtcNow;
                        break;
                }
            }
            else if (local.Is(RoleEnum.Grenadier))
            {
                var role2 = (Grenadier)role;
                role2.LastFlashed = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Impostor))
            {
                var role2 = (Impostor)role;
                role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Mafioso))
            {
                var role2 = (Mafioso)role;
                role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Janitor))
            {
                var role2 = (Janitor)role;
                role2.LastCleaned = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Miner))
            {
                var role2 = (Miner)role;
                role2.LastMined = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Morphling))
            {
                var role2 = (Morphling)role;
                role2.LastMorphed = DateTime.UtcNow;
                role2.LastSampled = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Teleporter))
            {
                var role2 = (Teleporter)role;
                role2.LastTeleport = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Undertaker))
            {
                var role2 = (Undertaker)role;
                role2.CurrentlyDragging = null;
                role2.LastDragged = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.TimeMaster))
            {
                var role2 = (TimeMaster)role;
                role2.LastFrozen = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Wraith))
            {
                var role2 = (Wraith)role;
                role2.LastInvis = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Ambusher))
            {
                var role2 = (Ambusher)role;
                role2.LastAmbushed = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Ghoul))
            {
                var role2 = (Ghoul)role;

                if (!role2.Caught)
                    role2.LastMarked = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Anarchist))
            {
                var role2 = (Anarchist)role;
                role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Concealer))
            {
                var role2 = (Concealer)role;
                role2.LastConcealed = DateTime.UtcNow;

                if (Role.SyndicateHasChaosDrive)
                    role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Bomber))
            {
                var role2 = (Bomber)role;
                role2.LastDetonated = DateTime.UtcNow;
                role2.LastPlaced = DateTime.UtcNow;

                if (CustomGameOptions.BombsRemoveOnNewRound)
                    role2.Bombs.ClearBombs();

                if (Role.SyndicateHasChaosDrive)
                    role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Framer))
            {
                var role2 = (Framer)role;
                role2.LastFramed = DateTime.UtcNow;

                if (Role.SyndicateHasChaosDrive)
                    role2.LastKilled = DateTime.UtcNow;

                if (role2.Player.Data.IsDead || role2.Player.Data.Disconnected)
                    role2.Framed.Clear();
            }
            else if (local.Is(RoleEnum.Gorgon))
            {
                var role2 = (Gorgon)role;
                role2.LastGazed = DateTime.UtcNow;
                role2.Gazed.Clear();

                if (Role.SyndicateHasChaosDrive)
                    role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Crusader))
            {
                var role2 = (Crusader)role;
                role2.LastCrusaded = DateTime.UtcNow;
                role2.CrusadedPlayer = null;

                if (Role.SyndicateHasChaosDrive)
                    role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Poisoner))
            {
                var role2 = (Poisoner)role;
                role2.LastPoisoned = DateTime.UtcNow;

                if (Role.SyndicateHasChaosDrive)
                    role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Rebel))
            {
                var role2 = (Rebel)role;

                if (Role.SyndicateHasChaosDrive)
                    role2.LastKilled = DateTime.UtcNow;

                if (!role2.HasDeclared)
                    role2.LastDeclared = DateTime.UtcNow;

                if (role2.FormerRole == null || role2.FormerRole?.RoleType == RoleEnum.Anarchist || !role2.WasSidekick)
                    return;

                switch (role2.FormerRole.RoleType)
                {
                    case RoleEnum.Concealer:
                        role2.LastConcealed = DateTime.UtcNow;
                        break;

                    case RoleEnum.Framer:
                        role2.LastFramed = DateTime.UtcNow;
                        break;

                    case RoleEnum.Poisoner:
                        role2.LastPoisoned = DateTime.UtcNow;
                        break;

                    case RoleEnum.Shapeshifter:
                        role2.LastShapeshifted = DateTime.UtcNow;
                        break;

                    case RoleEnum.Warper:
                        role2.LastWarped = DateTime.UtcNow;
                        break;

                    case RoleEnum.Drunkard:
                        role2.LastConfused = DateTime.UtcNow;
                        break;
                }
            }
            else if (local.Is(RoleEnum.Shapeshifter))
            {
                var role2 = (Shapeshifter)role;
                role2.LastShapeshifted = DateTime.UtcNow;

                if (Role.SyndicateHasChaosDrive)
                    role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Sidekick))
            {
                var role2 = (Sidekick)role;
                role2.LastKilled = DateTime.UtcNow;

                if (Role.SyndicateHasChaosDrive)
                    role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Warper))
            {
                var role2 = (Warper)role;
                role2.LastWarped = DateTime.UtcNow;

                if (Role.SyndicateHasChaosDrive)
                    role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Banshee))
            {
                var role2 = (Banshee)role;

                if (!role2.Caught)
                    role2.LastScreamed = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Arsonist))
            {
                var role2 = (Arsonist)role;
                role2.LastDoused = DateTime.UtcNow;
                role2.LastIgnited = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Cannibal))
            {
                var role2 = (Cannibal)role;
                role2.LastEaten = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Cryomaniac))
            {
                var role2 = (Cryomaniac)role;
                role2.LastDoused = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Dracula))
            {
                var role2 = (Dracula)role;
                role2.LastBitten = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Glitch))
            {
                var role2 = (Glitch)role;
                role2.LastMimic = DateTime.UtcNow;
                role2.LastHack = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.GuardianAngel))
            {
                var role2 = (GuardianAngel)role;
                role2.LastProtected = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Jackal))
            {
                var role2 = (Jackal)role;
                role2.LastRecruited = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Necromancer))
            {
                var role2 = (Necromancer)role;
                role2.LastResurrected = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Jester))
            {
                var role2 = (Jester)role;

                if (role2.VotedOut)
                    role2.LastHaunted = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Juggernaut))
            {
                var role2 = (Juggernaut)role;
                role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Murderer))
            {
                var role2 = (Murderer)role;
                role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Pestilence))
            {
                var role2 = (Pestilence)role;
                role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Plaguebearer))
            {
                var role2 = (Plaguebearer)role;
                role2.LastInfected = DateTime.UtcNow;

                if (role2.Player.Data.IsDead || role2.Player.Data.Disconnected)
                    role2.InfectedPlayers.Clear();
            }
            else if (local.Is(RoleEnum.SerialKiller))
            {
                var role2 = (SerialKiller)role;
                role2.LastLusted = DateTime.UtcNow;
                role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Survivor))
            {
                var role2 = (Survivor)role;
                role2.LastVested = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Thief))
            {
                var role2 = (Thief)role;
                role2.LastStolen = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Troll))
            {
                var role2 = (Troll)role;
                role2.LastInteracted = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Werewolf))
            {
                var role2 = (Werewolf)role;
                role2.LastMauled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Whisperer))
            {
                var role2 = (Whisperer)role;
                role2.LastWhispered = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.BountyHunter))
            {
                var role2 = (BountyHunter)role;
                role2.LastChecked = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Executioner))
            {
                var role2 = (Executioner)role;

                if (role2.TargetVotedOut)
                    role2.LastDoomed = DateTime.UtcNow;
            }

            var obj = Objectifier.GetObjectifier(local);

            if (obj == null)
                return;

            if (local.Is(ObjectifierEnum.Corrupted))
            {
                var obj2 = (Corrupted)obj;
                obj2.LastKilled = DateTime.UtcNow;
            }

            var ab = Ability.GetAbility(local);

            if (ab == null)
                return;

            if (local.Is(AbilityEnum.ButtonBarry))
            {
                var ab2 = (ButtonBarry)ab;
                ab2.LastButtoned = DateTime.UtcNow;
            }
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
            if (GameStates.IsHnS)
                return !playerInfo.IsImpostor();

            if (player == null || playerInfo == null || (playerInfo.IsDead && !(player.Is(RoleEnum.Revealer) || player.Is(RoleEnum.Phantom) || player.Is(RoleEnum.Ghoul) ||
                player.Is(RoleEnum.Banshee))) || playerInfo.Disconnected || CustomGameOptions.WhoCanVent == WhoCanVentOptions.Noone || !GameStates.IsRoaming || player.inMovingPlat)
            {
                return false;
            }
            else if (player.inVent || CustomGameOptions.WhoCanVent == WhoCanVentOptions.Everyone || player.Is(RoleEnum.Revealer) || player.Is(RoleEnum.Phantom) ||
                player.Is(RoleEnum.Ghoul) || player.Is(RoleEnum.Banshee))
            {
                return true;
            }

            var playerRole = Role.GetRole(player);
            bool mainflag;

            if (playerRole == null)
            {
                mainflag = playerInfo.IsImpostor();
            }
            else if (playerRole.IsBlocked)
            {
                mainflag = false;
            }
            else if (player.IsRecruit())
            {
                mainflag = CustomGameOptions.RecruitVent;
            }
            else if (player.IsResurrected())
            {
                mainflag = CustomGameOptions.ResurrectVent;
            }
            else if (player.IsPersuaded())
            {
                mainflag = CustomGameOptions.PersuadedVent;
            }
            else if (player.IsBitten())
            {
                mainflag = CustomGameOptions.UndeadVent;
            }
            else if (player.Is(Faction.Syndicate))
            {
                mainflag = (Role.SyndicateHasChaosDrive && CustomGameOptions.SyndicateVent == SyndicateVentOptions.ChaosDrive) || CustomGameOptions.SyndicateVent ==
                                SyndicateVentOptions.Always;
            }
            else if (player.Is(Faction.Intruder))
            {
                if (CustomGameOptions.IntrudersVent)
                {
                    var flag = (player.Is(RoleEnum.Morphling) && CustomGameOptions.MorphlingVent) || (player.Is(RoleEnum.Wraith) && CustomGameOptions.WraithVent) ||
                        (player.Is(RoleEnum.Grenadier) && CustomGameOptions.GrenadierVent) || (player.Is(RoleEnum.Teleporter) && CustomGameOptions.TeleVent);

                    if (flag)
                    {
                        mainflag = true;
                    }
                    else if (player.Is(RoleEnum.Undertaker))
                    {
                        var undertaker = (Undertaker)playerRole;

                        mainflag = CustomGameOptions.UndertakerVentOptions == UndertakerOptions.Always || (undertaker.CurrentlyDragging != null &&
                            CustomGameOptions.UndertakerVentOptions == UndertakerOptions.Body) || (undertaker.CurrentlyDragging == null &&
                            CustomGameOptions.UndertakerVentOptions == UndertakerOptions.Bodyless);
                    }
                    else
                    {
                        mainflag = true;
                    }
                }
                else
                {
                    mainflag = false;
                }
            }
            else if (player.Is(Faction.Crew) && !player.Is(RoleEnum.Revealer))
            {
                if (player.Is(AbilityEnum.Tunneler) && !player.Is(RoleEnum.Engineer))
                {
                    var tunneler = Role.GetRole(player);
                    mainflag = tunneler.TasksDone;
                }
                else if (player.Is(RoleEnum.Engineer))
                {
                    mainflag = true;
                }
                else if (CustomGameOptions.CrewVent)
                {
                    mainflag = true;
                }
                else
                {
                    mainflag = false;
                }
            }
            else if (player.Is(Faction.Neutral))
            {
                var flag = (player.Is(RoleEnum.Murderer) && CustomGameOptions.MurdVent) || (player.Is(RoleEnum.Glitch) && CustomGameOptions.GlitchVent) ||
                    (player.Is(RoleEnum.Juggernaut) && CustomGameOptions.JuggVent) || (player.Is(RoleEnum.Pestilence) && CustomGameOptions.PestVent) ||
                    (player.Is(RoleEnum.Jester) && CustomGameOptions.JesterVent) || (player.Is(RoleEnum.Plaguebearer) && CustomGameOptions.PBVent) ||
                    (player.Is(RoleEnum.Arsonist) && CustomGameOptions.ArsoVent) || (player.Is(RoleEnum.Executioner) && CustomGameOptions.ExeVent) ||
                    (player.Is(RoleEnum.Cannibal) && CustomGameOptions.CannibalVent) || (player.Is(RoleEnum.Dracula) && CustomGameOptions.DracVent) ||
                    (player.Is(RoleEnum.Survivor) && CustomGameOptions.SurvVent) || (player.Is(RoleEnum.Actor) && CustomGameOptions.ActorVent) ||
                    (player.Is(RoleEnum.GuardianAngel) && CustomGameOptions.GAVent) || (player.Is(RoleEnum.Amnesiac) && CustomGameOptions.AmneVent) ||
                    (player.Is(RoleEnum.Werewolf) && CustomGameOptions.WerewolfVent) || (player.Is(RoleEnum.Jackal) && CustomGameOptions.JackalVent) ||
                    (player.Is(RoleEnum.BountyHunter) && CustomGameOptions.BHVent) || player.Is(RoleEnum.Phantom);

                if (flag)
                {
                    mainflag = flag;
                }
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
                {
                    mainflag = false;
                }
            }
            else if (player.Is(RoleEnum.Revealer) || player.Is(RoleEnum.Phantom))
            {
                mainflag = true;
            }
            else
            {
                mainflag = false;
            }

            return mainflag;
        }

        public static List<bool> Interact(PlayerControl player, PlayerControl target, bool toKill = false, bool toConvert = false, bool bypass = false)
        {
            if (!CanInteract(player))
            {
                return new List<bool>()
                {
                    false,
                    false,
                    false,
                    false
                };
            }

            var fullCooldownReset = false;
            var gaReset = false;
            var survReset = false;
            var abilityUsed = false;
            bypass = bypass || player.Is(AbilityEnum.Ruthless);

            Spread(player, target);

            if (target.IsOnAlert() || target.IsAmbushed() || target.Is(RoleEnum.Pestilence) || (target.Is(RoleEnum.VampireHunter) && player.Is(SubFaction.Undead)) ||
                (target.Is(RoleEnum.SerialKiller) && (player.Is(RoleEnum.Escort) || player.Is(RoleEnum.Consort) || (player.Is(RoleEnum.Glitch) && !toKill)) && !bypass))
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
                    RpcMurderPlayer(target, player);

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
            else if (target.IsCrusaded() && !bypass)
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
                else if (Role.SyndicateHasChaosDrive)
                    Crusader.RadialCrusade(target);
                else
                    RpcMurderPlayer(target, player);

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
            else if (target.IsShielded() && (toKill || toConvert) && !bypass)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                writer.Write(target.GetMedic().Player.PlayerId);
                writer.Write(target.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                if (CustomGameOptions.ShieldBreaks)
                    fullCooldownReset = true;

                StopKill.BreakShield(target.GetMedic().Player.PlayerId, target.PlayerId, CustomGameOptions.ShieldBreaks);
            }
            else if (target.IsVesting() && (toKill || toConvert) && !bypass)
                survReset = true;
            else if (target.IsProtected() && (toKill || toConvert) && !bypass)
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
                        RpcMurderPlayer(player, target);

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

            return new List<bool>
            {
                fullCooldownReset,
                gaReset,
                survReset,
                abilityUsed
            };
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
            var playerControlList = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            var allPlayers = GameData.Instance.AllPlayers;
            var lightRadius = radius * ShipStatus.Instance.MaxLightRadius;

            for (var index = 0; index < allPlayers.Count; ++index)
            {
                var playerInfo = allPlayers[index];

                if (!playerInfo.Disconnected)
                {
                    var vector2 = new Vector2(playerInfo.Object.GetTruePosition().x - truePosition.x, playerInfo.Object.GetTruePosition().y - truePosition.y);
                    var magnitude = vector2.magnitude;

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

        public static bool SetActive(PlayerControl target, RoleEnum role, bool condition = true) => target?.Data?.IsDead == false && target.Is(role) && condition &&
            GameStates.IsRoaming && (HudManager.Instance.UseButton.isActiveAndEnabled || HudManager.Instance.PetButton.isActiveAndEnabled) && target == PlayerControl.LocalPlayer;

        public static bool SetDeadActive(PlayerControl target, RoleEnum role, bool condition = true) => target?.Data != null && target.Is(role) && GameStates.IsRoaming &&
            condition && (HudManager.Instance.UseButton.isActiveAndEnabled || HudManager.Instance.PetButton.isActiveAndEnabled) && target == PlayerControl.LocalPlayer;

        public static bool SetActive(PlayerControl target, ObjectifierEnum obj, bool condition = true) => target?.Data?.IsDead == false && target.Is(obj) && condition &&
            GameStates.IsRoaming && (HudManager.Instance.UseButton.isActiveAndEnabled || HudManager.Instance.PetButton.isActiveAndEnabled) && target == PlayerControl.LocalPlayer;

        public static bool SetActive(PlayerControl target, AbilityEnum ability, bool condition = true) => target?.Data?.IsDead == false && GameStates.IsRoaming && condition &&
            (HudManager.Instance.UseButton.isActiveAndEnabled || HudManager.Instance.PetButton.isActiveAndEnabled) && target.Is(ability) && target == PlayerControl.LocalPlayer;

        public static bool Inactive() => PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null ||
            !PlayerControl.LocalPlayer.CanMove || !GameStates.IsRoaming;

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

            return intruderFlag || syndicateFlag || traitorFlag || nkFlag || neFlag || framedFlag || fanaticFlag;
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

        public static bool ButtonUsable(ActionButton button) => button != null && button?.isActiveAndEnabled == true && button.isCoolingDown && !CannotUse(PlayerControl.LocalPlayer);

        public static List<object> AllPlayerInfo(this PlayerControl player)
        {
            var role = Role.GetRole(player);
            var modifier = Modifier.GetModifier(player);
            var ability = Ability.GetAbility(player);
            var objectifier = Objectifier.GetObjectifier(player);

            return new List<object>
            {
                role, //0
                modifier, //1
                ability, //2
                objectifier //3
            };
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
            else if (player.Is(RoleEnum.Troll))
            {
                var troll = Role.GetRole<Troll>(player);
                return troll.Killed;
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
            {
                objectifierName += "None";
            }

            objectifierName += "</color>";

            if (info[2] != null)
            {
                if (!ability.Hidden)
                    abilityName += $"{ability.ColorString}{ability.Name}</color>";
            }
            else
            {
                abilityName += "None";
            }

            abilityName += "</color>";

            if (info[1] != null)
            {
                if (!modifier.Hidden)
                    modifierName += $"{modifier.ColorString}{modifier.Name}</color>";
            }
            else
            {
                modifierName += "None";
            }

            modifierName += "</color>";

            if (player.IsRecruit())
            {
                var jackal = player.GetJackal();
                objectives += $"\n<color=#{Colors.Cabal.ToHtmlStringRGBA()}>- You are a member of the Cabal. Help {jackal.PlayerName} in taking over the mission.</color>";
            }
            else if (player.IsResurrected())
            {
                var necromancer = player.GetNecromancer();
                objectives += $"\n<color=#{Colors.Reanimated.ToHtmlStringRGBA()}>- You are a member of the Reanimated. Help {necromancer.PlayerName} in taking over the mission.</color>";
            }
            else if (player.IsPersuaded())
            {
                var whisperer = player.GetWhisperer();
                objectives += $"\n<color=#{Colors.Sect.ToHtmlStringRGBA()}>- You are a member of the Sect. Help {whisperer.PlayerName} in taking over the mission.</color>";
            }
            else if (player.IsBitten())
            {
                var dracula = player.GetDracula();
                objectives += $"\n<color=#{Colors.Dracula.ToHtmlStringRGBA()}>- You are a member of the Undead. Help {dracula.PlayerName} in taking over the mission." +
                    "\n- Attempting to interact with a <color=#C0C0C0FF>Vampire Hunter</color> will force them to kill you.</color>";
            }

            objectives += "</color>";

            var hassomething = false;

            if (info[0] != null)
            {
                abilities += $"\n{role.ColorString}{role.AbilitiesText}</color>";
                hassomething = true;
            }

            if (info[2] != null && !ability.Hidden)
            {
                abilities += $"\n{ability.ColorString}{ability.TaskText}</color>";
                hassomething = true;
            }

            if (!hassomething)
                abilities += "\n- None.";

            abilities += "</color>";

            var hasnothing = true;

            if (info[1] != null && !modifier.Hidden)
            {
                attributes += $"\n{modifier.ColorString}{modifier.TaskText}</color>";
                hasnothing = false;
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
            return $"{roleName}\n{objectifierName}\n{abilityName}\n{modifierName}\n{alignment}\n{objectives}\n{abilities}\n{attributes}\n<color=#FFFFFFFF>Tasks:</color>";
        }

        public static void RegenTask(this PlayerControl player)
        {
            try
            {
                foreach (var task2 in player.myTasks.ToArray())
                {
                    var importantTextTask = task2.Cast<ImportantTextTask>();

                    if (importantTextTask.Text.Contains("Sabotage and kill everyone") || importantTextTask.Text.Contains("Fake Tasks") || importantTextTask.Text.Contains("Role") ||
                        importantTextTask.Text.Contains("tasks to win"))
                    {
                        player.myTasks.Remove(importantTextTask);
                    }
                }
            } catch {}

            var task = new GameObject("DetailTask").AddComponent<ImportantTextTask>();
            task.transform.SetParent(player.transform, false);
            task.Text = player.GetTaskList();
            player.myTasks.Insert(0, task);
        }

        public static Sprite CreateSprite(string name)
        {
            name += ".png";
            var assembly = Assembly.GetExecutingAssembly();
            var tex = CreateEmptyTexture();
            var imageStream = assembly.GetManifestResourceStream(name);
            var img = imageStream.ReadFully();
            LoadImage(tex, img, true);
            tex.DontDestroy();
            var sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
            sprite.DontDestroy();
            return sprite;
        }

        public static void LoadImage(Texture2D tex, byte[] data, bool markNonReadable)
        {
            _iCallLoadImage ??= IL2CPP.ResolveICall<DLoadImage>("UnityEngine.ImageConversion::LoadImage");
            var il2CPPArray = (Il2CppStructArray<byte>) data;
            _iCallLoadImage.Invoke(tex.Pointer, il2CPPArray.Pointer, markNonReadable);
        }

        public static void LogSomething(object message) => PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage(message);

        public static string CreateText(string itemName, string folder = "", string subfolder = "")
        {
            string resourceName;

            if (subfolder != "" && folder != "")
                resourceName = $"{TownOfUsReworked.Resources}{folder}.{subfolder}.{itemName}";
            else if (subfolder?.Length == 0 && folder != "")
                resourceName = $"{TownOfUsReworked.Resources}{folder}.{itemName}";
            else
                resourceName = TownOfUsReworked.Resources + itemName;

            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(resourceName);
            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public static bool GameHasEnded() => Role.SyndicateWin || Role.CrewWin || Role.NobodyWins || Role.IntruderWin || Role.AllNeutralsWin || Role.NKWins || Role.PhantomWins ||
            Role.ArsonistWins || Role.CabalWin || Role.SectWin || Role.UndeadWin || Role.ReanimatedWin || Role.CryomaniacWins || Role.JuggernautWins || Role.WerewolfWins ||
            Role.MurdererWins || Role.SerialKillerWins || Role.GlitchWins || Role.InfectorsWin || Objectifier.NobodyWins || Objectifier.LoveWins || Objectifier.RivalWins ||
            Objectifier.TaskmasterWins || Objectifier.OverlordWins || Objectifier.CorruptedWins;

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

        public static Role GetLeader(this PlayerControl player)
        {
            if (!player.Is(RoleEnum.Mafioso) && !player.Is(RoleEnum.Sidekick))
                return null;

            var role = Role.GetRole(player);

            if (role == null)
                return null;

            if (player.Is(RoleEnum.Mafioso))
                return ((Mafioso)role).Godfather;
            else if (player.Is(RoleEnum.Sidekick))
                return ((Sidekick)role).Rebel;

            return null;
        }

        public static InspectorResults GetActorList(this PlayerControl player)
        {
            if (!player.Is(RoleEnum.Actor))
                return InspectorResults.None;

            var role = Role.GetRole(player);

            if (role == null)
                return InspectorResults.None;

            if (player.Is(RoleEnum.Actor))
                return ((Actor)role).PretendRoles;

            return InspectorResults.None;
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
            newRole.Player.RegenTask();
        }

        public static void ShareGameVersion()
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.VersionHandshake, Hazel.SendOption.Reliable);
            writer.Write((byte)TownOfUsReworked.Version.Major);
            writer.Write((byte)TownOfUsReworked.Version.Minor);
            writer.Write((byte)TownOfUsReworked.Version.Build);
            writer.Write(AmongUsClient.Instance.AmHost ? GameStartManagerPatch.timer : -1f);
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
                ver = new(major, minor, build);
            else
                ver = new(major, minor, build, revision);

            GameStartManagerPatch.PlayerVersions[clientId] = new GameStartManagerPatch.PlayerVersion(ver, guid);
        }

        public static IEnumerable<T> GetFastEnumerator<T>(this Il2CppSystem.Collections.Generic.List<T> list) where T : Il2CppSystem.Object => new Il2CppListEnumerable<T>(list);

        public static string GetRandomisedName()
        {
            const string everything = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()|{}[],.<>;':\"-+=*/`~_\\ ⟡☆♡♧♤ø▶❥✔εΔΓικνστυφψΨωχӪζδ♠♥βαµ♣✚Ξρλς§π★ηΛγΣΦΘξ";
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