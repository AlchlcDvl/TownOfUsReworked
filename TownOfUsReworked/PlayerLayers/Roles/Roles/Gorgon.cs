using TownOfUsReworked.Enums;
using System.Collections.Generic;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using Hazel;
using System.Linq;
using System;
using TownOfUsReworked.Lobby.CustomOption;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Gorgon : Role
    {
        private KillButton _gazeButton;
        public Dictionary<byte, float> gazeList = new Dictionary<byte, float>();
        public PlayerControl ClosestPlayer;
        public DateTime LastGazed;
        public bool Enabled = false;
        public float TimeRemaining;
        public PlayerControl StonedPlayer;
        public bool Stoned => TimeRemaining > 0f;

        public Gorgon(PlayerControl player) : base(player)
        {
            Name = "Gorgon";
            Base = false;
            IsRecruit = false;
            StartText = "Turn The <color=#8BFDFD>Crew</color> Into Sculptures";
            AbilitiesText = "- You can stone gaze players, that forces them to stand still till a meeting is called.";
            AttributesText = "- Stoned players cannot move and will die when a meeting is called.";
            Color = CustomGameOptions.CustomSynColors ? Colors.Gorgon : Colors.Syndicate;
            RoleType = RoleEnum.Gorgon;
            Faction = Faction.Syndicate;
            Attack = AttackEnum.Basic;
            Defense = DefenseEnum.None;
            SubFaction = SubFaction.None;
            FactionName = "Syndicate";
            FactionColor = Colors.Syndicate;
            CoronerDeadReport = "The body has strange snake hair! They must be a Gorgon!";
            CoronerKillerReport = "The sculpture is eerily similar to a real person. They were killed by a Gorgon!";
            Results = InspResults.ConcealGorg;
            AttackString = "Basic";
            DefenseString = "None";
            IntroSound = null;
            RoleAlignment = RoleAlignment.SyndicateKill;
            AlignmentName = "Syndicate (Killing)";
            FactionDescription = "Your faction is the Syndicate! Your faction has low killing power and is instead geared towards delaying the wins of other factions" +
                " and causing some good old chaos. After a certain number of meeting, one of you will recieve the \"Chaos Drive\" which will enhance your powers and " +
                "give you the ability to kill, if you didn't already.";         
            Objectives = "- Kill: <color=#FF0000FF>Intruders</color>, <color=#8BFDFD>Crew</color> and <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color>," +
                " <color=#1D7CF2FF>Proselytes</color> and <color=#1D7CF2FF>Neophytes</color>.";
            IntroText = "Cause chaos and kill your opposition";
            RoleDescription = "You are a Gorgon! Use your gaze of stone to freeze players in place and await their deaths!";
            AlignmentDescription = "You are a Syndicate (Killing) role! It's your job to ensure that the crew dies while you achieve your ulterior motives.";
            AddToRoleHistory(RoleType);
        }
        
        public KillButton GazeButton
        {
            get => _gazeButton;
            set
            {
                _gazeButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        
        public float FreezeTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastGazed;
            var num = CustomGameOptions.PoisonCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
        
        public void Freeze()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance)
                TimeRemaining = 0;
                
            if (TimeRemaining <= 0)
                FreezeKill();
        }

        public void FreezeKill()
        {
            if (!StonedPlayer.Is(RoleEnum.Pestilence))
            {
                Utils.RpcMurderPlayer(Player, StonedPlayer);

                if (!StonedPlayer.Data.IsDead)
                {
                    try
                    {
                        SoundManager.Instance.PlaySound(TownOfUsReworked.KillSFX, false, 1f);
                    } catch {}
                }
            }

            StonedPlayer = null;
            Enabled = false;
            LastGazed = DateTime.UtcNow;
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