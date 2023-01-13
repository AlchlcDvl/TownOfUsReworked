using System;
using Hazel;
using System.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using System.Linq;
using Object = UnityEngine.Object;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Cannibal : Role
    {
        private KillButton _eatButton;
        public DeadBody CurrentTarget { get; set; }
        public int EatNeed;
        public string body;
        public bool CannibalWin;
        public DateTime LastEaten { get; set; }
        public Dictionary<byte, ArrowBehaviour> BodyArrows = new Dictionary<byte, ArrowBehaviour>();
        
        public Cannibal(PlayerControl player) : base(player)
        {
            Name = "Cannibal";
            StartText = "Eat The Bodies Of The Dead";
            RoleType = RoleEnum.Cannibal;
            Faction = Faction.Neutral;
            LastEaten = DateTime.UtcNow;
            EatNeed = CustomGameOptions.CannibalBodyCount >= PlayerControl.AllPlayerControls._size / 2 ?
                PlayerControl.AllPlayerControls._size / 2 : CustomGameOptions.CannibalBodyCount; //Limits the max required bodies to 1/2 of lobby's size
            body = EatNeed == 1 ? "body" : "bodies";
            AbilitiesText = "- You can consume a body, making it disappear from the game.";
            AttributesText = "- You know when someone dies, so you can find their body.";
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralEvil;
            AlignmentName = "Neutral (Evil)";
            Results = InspResults.EngiAmneThiefCann;
            Color = IsRecruit ? Colors.Cabal : (CustomGameOptions.CustomNeutColors ? Colors.Cannibal : Colors.Neutral);
            RoleDescription = $"You are a Cannibal! You have an everlasting hunger for dead bodies. You need to eat {EatNeed} {body} to win!";
            AlignmentDescription = NEDescription;
            FactionDescription = NeutralFactionDescription;
            Objectives = $"- Eat {EatNeed} {body}.";
        }
        
        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (IsRecruit)
            {
                if (Utils.CabalWin())
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CabalWin,
                        SendOption.Reliable, -1);
                    writer.Write(Player.PlayerId);
                    Wins();
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (EatNeed == 0)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.CannibalWin,
                    SendOption.Reliable, -1);
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return true;
        }

        public override void Wins()
        {
            if (IsRecruit)
                CabalWin = true;
            else
                CannibalWin = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();

            team.Add(PlayerControl.LocalPlayer);

            if (IsRecruit)
            {
                var jackal = Player.GetJackal();

                team.Add(jackal.Player);
                team.Add(jackal.EvilRecruit);
            }

            __instance.teamToShow = team;
        }

        public float EatTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastEaten;
            var num = CustomGameOptions.CannibalCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public KillButton EatButton
        {
            get => _eatButton;
            set
            {
                _eatButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId);

            if (arrow.Value != null)
                Object.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);
                
            BodyArrows.Remove(arrow.Key);
        }
    }
}