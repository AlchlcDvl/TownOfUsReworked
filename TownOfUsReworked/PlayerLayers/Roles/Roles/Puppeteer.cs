using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Puppeteer : Role
    {
        public KillButton _possessButton;
        public PlayerControl ClosestPlayer;
        public PlayerControl PossessPlayer;
        public DateTime PossStart;
        public float PossessTime;
        public float duration;
        public DateTime lastPossess;
        public bool possessStarting = false; 
        public bool Enabled;
        
        public static Sprite PossessSprite => TownOfUsReworked.Placeholder; //TODO .PossessSprite;
        public static Sprite UnPossessSprite => TownOfUsReworked.Placeholder; // TODO .ReleaseSprite;

        public Puppeteer(PlayerControl player) : base(player)
        {
            Name = "Puppeteer";
            ImpostorText = () => "Control Crew to Kill";
            TaskText = () => "Control Crew to Kill";
            Color = CustomGameOptions.CustomSynColors ? Colors.Puppeteer : Colors.Syndicate;
            SubFaction = SubFaction.None;
            RoleType = RoleEnum.Puppeteer;
            Faction = Faction.Syndicate;
            FactionName = "Syndicate";
            FactionColor = Colors.Syndicate;
            RoleAlignment = RoleAlignment.SyndicateChaos;
            AlignmentName = () => "Syndicate (Chaos)";
            IntroText = "Cause choas and murder your opposition";
            PossessPlayer = null;
            ClosestPlayer = null;
            _possessButton = null;
            lastPossess = DateTime.UtcNow;
            Results = InspResults.GAExeMedicPup;
            AddToRoleHistory(RoleType);
        }

        public KillButton PossessButton
        {
            get => _possessButton;
            set
            {
                _possessButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public void UnPossess()
        {
            PossessPlayer = null;
            Player.moveable = true;

            if (PlayerControl.LocalPlayer == Player)
                PossessButton.graphic.sprite = PossessSprite;

            PossessTime -= Time.deltaTime;
        }

        public void KillUnPossess()
        {
            if (PlayerControl.LocalPlayer == Player)
                Player.SetKillTimer(PlayerControl.GameOptions.KillCooldown);

            UnPossess();
        }

        public void Possess()
        {
            Enabled = true;
            lastPossess = DateTime.UtcNow;
        }

        public float PossessTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - lastPossess;
            var num = CustomGameOptions.PossessCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}