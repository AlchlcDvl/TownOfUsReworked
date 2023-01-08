using System.Collections.Generic;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Modifiers;
using UnityEngine;
using Object = System.Object;
using System;
using InnerNet;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.Extensions
{
    public static class AmongUsExtensions
    {
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

        public static void SetImpostor(this GameData.PlayerInfo playerinfo, bool impostor)
        {
            if (playerinfo.Role != null)
                playerinfo.Role.TeamType = impostor ? RoleTeamTypes.Impostor : RoleTeamTypes.Crewmate;
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
                
                if (CustomGameOptions.PlayerNumbers)
                    playerControl.nameText().text += $" {playerControl.PlayerId}";
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

        public static TMPro.TextMeshPro nameText(this PlayerControl p) => p.cosmetics.nameText;

        public static TMPro.TextMeshPro NameText(this PoolablePlayer p) => p.cosmetics.nameText;

        public static UnityEngine.SpriteRenderer myRend(this PlayerControl p) => p.cosmetics.currentBodySprite.BodySprite;
    }
}