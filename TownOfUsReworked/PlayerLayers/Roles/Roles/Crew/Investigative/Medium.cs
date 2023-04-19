using System;
using UnityEngine;
using System.Collections.Generic;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;
using TownOfUsReworked.Custom;
using HarmonyLib;
using System.Linq;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using TownOfUsReworked.Patches;
using Hazel;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Medium : CrewRole
    {
        public DateTime LastMediated;
        public Dictionary<byte, ArrowBehaviour> MediatedPlayers = new();
        public CustomButton MediateButton;
        public CustomButton SeanceButton;

        public Medium(PlayerControl player) : base(player)
        {
            Name = "Medium";
            StartText = "Spooky Scary Ghosties Send Shivers Down Your Spine";
            AbilitiesText = "- You can mediate which makes ghosts visible to you" + (CustomGameOptions.ShowMediumToDead ? "\n- When mediating, dead players will be able to see " +
                "you" : "");
            Color = CustomGameOptions.CustomCrewColors ? Colors.Medium : Colors.Crew;
            RoleType = RoleEnum.Medium;
            MediatedPlayers = new();
            RoleAlignment = RoleAlignment.CrewInvest;
            AlignmentName = CI;
            InspectorResults = InspectorResults.DifferentLens;
            Type = LayerEnum.Medium;
            MediateButton = new(this, AssetManager.Mediate, AbilityTypes.Effect, "ActionSecondary", Mediate);
        }

        public float MediateTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastMediated;
            var num = Player.GetModifiedCooldown(CustomGameOptions.MediateCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void AddMediatePlayer(byte playerId)
        {
            var gameObj = new GameObject();
            var arrow = gameObj.AddComponent<ArrowBehaviour>();

            if (Player == PlayerControl.LocalPlayer || CustomGameOptions.ShowMediumToDead)
            {
                gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = AssetManager.Arrow;
                arrow.image = renderer;
                gameObj.layer = 5;
                arrow.target = Utils.PlayerById(playerId).transform.position;
                Utils.Flash(Color);
            }

            MediatedPlayers.Add(playerId, arrow);
        }

        public override void OnLobby()
        {
            MediatedPlayers.Values.DestroyAll();
            MediatedPlayers.Clear();
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            MediateButton.Update("MEDIATE", MediateTimer(), CustomGameOptions.MediateCooldown);

            if (!PlayerControl.LocalPlayer.Data.IsDead)
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (MediatedPlayers.ContainsKey(player.PlayerId))
                    {
                        MediatedPlayers.GetValueSafe(player.PlayerId).target = player.transform.position;
                        player.Visible = true;

                        if (!CustomGameOptions.ShowMediatePlayer)
                        {
                            player.SetOutfit(CustomPlayerOutfitType.Camouflage, new GameData.PlayerOutfit()
                            {
                                ColorId = player.GetDefaultOutfit().ColorId,
                                HatId = "",
                                SkinId = "",
                                VisorId = "",
                                PlayerName = " "
                            });

                            PlayerMaterial.SetColors(new Color32(128, 128, 128, 255), player.MyRend());
                        }
                    }
                }
            }
        }

        public void Mediate()
        {
            if (MediateTimer() != 0f)
                return;

            LastMediated = DateTime.UtcNow;
            var PlayersDead = Murder.KilledPlayers.GetRange(0, Murder.KilledPlayers.Count);

            if (PlayersDead.Count == 0)
                return;

            if (CustomGameOptions.DeadRevealed == DeadRevealed.Newest)
                PlayersDead.Reverse();

            if (CustomGameOptions.DeadRevealed != DeadRevealed.Random)
            {
                foreach (var dead in PlayersDead)
                {
                    if (Object.FindObjectsOfType<DeadBody>().Any(x => x.ParentId == dead.PlayerId && !MediatedPlayers.ContainsKey(x.ParentId)))
                    {
                        AddMediatePlayer(dead.PlayerId);
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                        writer.Write((byte)ActionsRPC.Mediate);
                        writer.Write(dead.PlayerId);
                        writer.Write(Player.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);

                        if (CustomGameOptions.DeadRevealed != DeadRevealed.All)
                            break;
                    }
                }
            }
            else
            {
                PlayersDead.Shuffle();
                var dead = PlayersDead[Random.RandomRangeInt(0, PlayersDead.Count - 1)];

                if (Object.FindObjectsOfType<DeadBody>().Any(x => x.ParentId == dead.PlayerId && !MediatedPlayers.ContainsKey(x.ParentId)))
                {
                    AddMediatePlayer(dead.PlayerId);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.Mediate);
                    writer.Write(dead.PlayerId);
                    writer.Write(Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
        }
    }
}