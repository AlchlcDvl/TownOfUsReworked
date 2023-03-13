using System;
using UnityEngine;
using Reactor.Utilities;
using System.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Medium : CrewRole
    {
        public DateTime LastMediated;
        public Dictionary<byte, ArrowBehaviour> MediatedPlayers;
        public static Sprite Arrow => TownOfUsReworked.Arrow;
        public AbilityButton MediateButton;
        
        public Medium(PlayerControl player) : base(player)
        {
            Name = "Medium";
            StartText = "Spooky Scary Ghosties Send Shivers Down Your Spine";
            AbilitiesText = "- You can mediate which makes ghosts visible to you." + (CustomGameOptions.ShowMediumToDead ? "\n- When mediating, dead players will be able to see " +
                "you." : "");
            Color = CustomGameOptions.CustomCrewColors ? Colors.Medium : Colors.Crew;
            RoleType = RoleEnum.Medium;
            MediatedPlayers = new Dictionary<byte, ArrowBehaviour>();
            RoleAlignment = RoleAlignment.CrewInvest;
            AlignmentName = CI;
            RoleDescription = "You are a Medium! You can mediate the dead, which reveals the spirits of the dead to you! Use their movements and information to find the evils!";
            InspectorResults = InspectorResults.DifferentLens;
        }

        public float MediateTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastMediated;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.MediateCooldown) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void AddMediatePlayer(byte playerId)
        {
            var gameObj = new GameObject();
            var arrow = gameObj.AddComponent<ArrowBehaviour>();

            if (Player.PlayerId == PlayerControl.LocalPlayer.PlayerId || CustomGameOptions.ShowMediumToDead)
            {
                gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = Arrow;
                arrow.image = renderer;
                gameObj.layer = 5;
                arrow.target = Utils.PlayerById(playerId).transform.position;
            }
            
            MediatedPlayers.Add(playerId, arrow);
            Coroutines.Start(Utils.FlashCoroutine(Color));
        }
    }
}