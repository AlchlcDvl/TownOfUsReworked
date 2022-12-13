using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;
using UnityEngine;
using TownOfUsReworked.Extensions;
using Hazel;
using System.Linq;
using Il2CppSystem.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Puppeteer : Role
    {
        private KillButton _possessButton;
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
            StartText = "Control Crew to Kill";
            AbilitiesText = "Control Crew to Kill";
            Color = CustomGameOptions.CustomSynColors ? Colors.Puppeteer : Colors.Syndicate;
            SubFaction = SubFaction.None;
            RoleType = RoleEnum.Puppeteer;
            Faction = Faction.Syndicate;
            FactionName = "Syndicate";
            FactionColor = Colors.Syndicate;
            RoleAlignment = RoleAlignment.SyndicateChaos;
            AlignmentName = "Syndicate (Chaos)";
            IntroText = "Cause choas and murder your opposition";
            PossessPlayer = null;
            ClosestPlayer = null;
            _possessButton = null;
            lastPossess = DateTime.UtcNow;
            Results = InspResults.GAExeMedicPup;
            Attack = AttackEnum.Basic;
            Defense = DefenseEnum.None;
            AttackString = "Basic";
            DefenseString = "None";
            IntroSound = null;
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

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            var intTeam = new List<PlayerControl>();

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(Faction.Syndicate))
                    intTeam.Add(player);
            }
            __instance.teamToShow = intTeam;
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
                Player.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);

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

        public override void Wins()
        {
            SyndicateWin = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead | Player.Data.Disconnected)
                return true;

            if ((PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Crew) |
                x.Is(RoleAlignment.NeutralKill) | x.Is(Faction.Intruders) | x.Is(RoleAlignment.NeutralNeo) | x.Is(RoleAlignment.NeutralPros))) == 0))
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SyndicateWin,
                    SendOption.Reliable, -1);
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }
    }
}