using System;
using UnityEngine;
using System.Collections.Generic;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Medium : CrewRole
    {
        public DateTime LastMediated;
        public Dictionary<byte, ArrowBehaviour> MediatedPlayers = new();
        public AbilityButton MediateButton;

        public Medium(PlayerControl player) : base(player)
        {
            Name = "Medium";
            StartText = "Spooky Scary Ghosties Send Shivers Down Your Spine";
            AbilitiesText = "- You can mediate which makes ghosts visible to you." + (CustomGameOptions.ShowMediumToDead ? "\n- When mediating, dead players will be able to see " +
                "you." : "");
            Color = CustomGameOptions.CustomCrewColors ? Colors.Medium : Colors.Crew;
            Type = RoleEnum.Medium;
            MediatedPlayers = new();
            RoleAlignment = RoleAlignment.CrewInvest;
            AlignmentName = CI;
            InspectorResults = InspectorResults.DifferentLens;
        }

        public float MediateTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastMediated;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.MediateCooldown) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public void AddMediatePlayer(byte playerId)
        {
            var gameObj = new GameObject();
            var arrow = gameObj.AddComponent<ArrowBehaviour>();

            if (Player.PlayerId == PlayerControl.LocalPlayer.PlayerId || CustomGameOptions.ShowMediumToDead)
            {
                gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = AssetManager.Arrow;
                arrow.image = renderer;
                gameObj.layer = 5;
                arrow.target = Utils.PlayerById(playerId).transform.position;
            }

            MediatedPlayers.Add(playerId, arrow);
            Utils.Flash(Color, "There's a mediate in progress!");
        }

        public override void OnLobby()
        {
            MediatedPlayers.Values.DestroyAll();
            MediatedPlayers.Clear();
        }
    }
}