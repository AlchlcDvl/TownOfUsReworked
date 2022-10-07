using System;
using Hazel;

namespace TownOfUs.Roles
{
    public class Cannibal : Role
    {
        public KillButton _eatButton;
        public DeadBody CurrentTarget { get; set; }
        public DateTime LE { get; set; }
        public int EatNeed;
        public bool CannibalWin;
        public DateTime LastEaten;
        
        public Cannibal(PlayerControl player) : base(player)
        {
            Name = "Cannibal";
            ImpostorText = () => "Eat Bodies";
            if (CustomGameOptions.CustomNeutColors) Color = Patches.Colors.Cannibal;
            else Color = Patches.Colors.Neutral;
            RoleType = RoleEnum.Cannibal;
            Faction = Faction.Neutral;
            LastEaten = DateTime.UtcNow;
            EatNeed = CustomGameOptions.CannibalBodyCount >= PlayerControl.AllPlayerControls._size / 2 ? PlayerControl.AllPlayerControls._size / 2 : CustomGameOptions.CannibalBodyCount; // Limit max bodies to 1/2 of lobby
            var body = EatNeed == 1 ? "Body" : "Bodies";
            TaskText = () => $"Eat {EatNeed} {body} to Win\nFake Tasks:";
            FactionName = "Neutral";
            FactionColor = Patches.Colors.Neutral;
            Alignment = Alignment.NeutralEvil;
            AlignmentName = "Neutral (Evil)";
            AddToRoleHistory(RoleType);
        }
        
        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (EatNeed == 0)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.CannibalWin, SendOption.Reliable, -1);
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            return true;
        }

        public void Wins()
        {
            CannibalWin = true;
        }

        public void Loses()
        {
            LostByRPC = true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var cannibalTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            cannibalTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = cannibalTeam;
        }

        public float CannibalTimer()
        {
            var t = DateTime.UtcNow - LE;
            var i = CustomGameOptions.CannibalCd * 1000;
            if (i - (float) t.TotalMilliseconds < 0) return 0;
            return (i - (float) t.TotalMilliseconds) / 1000;
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
    }
}