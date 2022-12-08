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
            AbilitiesText = "- You can consume a body, making it disappear from the game.\n- You know when someone dies, so you can find their body.";
            AttributesText = "- None.";
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralEvil;
            AlignmentName = "Neutral (Evil)";
            IntroText = "Eat the bodies of the dead";
            CoronerDeadReport = "The sharp canines and traces of blood in the mouth indicate that this body is a Cannibal!";
            CoronerKillerReport = "";
            Results = InspResults.EngiAmneThiefCann;
            Color = CustomGameOptions.CustomNeutColors ? Colors.Cannibal : Colors.Neutral;
            SubFaction = SubFaction.None;
            RoleDescription = $"You are a Cannibal! You have an everlasting hunger for dead bodies. You need to eat {EatNeed} {body} to win!";
            AlignmentDescription = "You are a Neutral (Evil) role! You have a confliction win condition over others and upon achieving it will end the game. " +
                "Finish your objective before they finish you!";
            FactionDescription = "Your faction is Neutral! You do not have any team mates and can only by yourself or by other players after finishing" +
                " a certain objective.";
            Objectives = $"- Eat {EatNeed} {body}.";
            IntroSound = null;
            Attack = AttackEnum.None;
            Defense = DefenseEnum.None;
            AttackString = "None";
            DefenseString = "None";
            AddToRoleHistory(RoleType);
        }
        
        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (EatNeed == 0)
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
            CannibalWin = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var cannibalTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            cannibalTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = cannibalTeam;
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