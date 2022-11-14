using System;
using UnityEngine;
using Reactor.Utilities;
using System.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Medium : Role
    {
        public DateTime LastMediated { get; set; }
        public Dictionary<byte, ArrowBehaviour> MediatedPlayers = new Dictionary<byte, ArrowBehaviour>();
        public static Sprite Arrow => TownOfUsReworked.Arrow;
        public bool MedWin;
        
        public Medium(PlayerControl player) : base(player)
        {
            Name = "Medium";
            ImpostorText = () => "Spooky Scary Ghosties Send Shivers Down Your Spine";
            TaskText = () => "Follow ghosts to get clues from them";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Medium : Colors.Crew;
            SubFaction = SubFaction.None;
            LastMediated = DateTime.UtcNow;
            RoleType = RoleEnum.Medium;
            Faction = Faction.Crew;
            Scale = 1.4f;
            MediatedPlayers = new Dictionary<byte, ArrowBehaviour>();
            FactionName = "Medium";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewInvest;
            AlignmentName = () => "Crew (Investigative)";
            IntroText = "Eject all <color=#FF0000FF>evildoers</color>";
            Results = InspResults.CoroJaniUTMed;
            AddToRoleHistory(RoleType);
        }

        public float MediateTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastMediated;
            var num = CustomGameOptions.MediateCooldown * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void AddMediatePlayer(byte playerId)
        {
            var gameObj = new GameObject();
            var arrow = gameObj.AddComponent<ArrowBehaviour>();

            if (Player.PlayerId == PlayerControl.LocalPlayer.PlayerId | CustomGameOptions.ShowMediumToDead)
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

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }
    }
}