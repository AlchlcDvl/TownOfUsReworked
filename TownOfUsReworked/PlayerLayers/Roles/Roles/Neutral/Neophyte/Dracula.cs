using System;
using System.Collections.Generic;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Dracula : Role
    {
        public DateTime LastBitten { get; set; }
        public bool HasMaj;
        private KillButton _biteButton;
        public PlayerControl ClosestPlayer;
        public List<byte> Converted;

        public Dracula(PlayerControl player) : base(player)
        {
            Name = "Dracula";
            Faction = Faction.Neutral;
            RoleType = RoleEnum.Dracula;
            StartText = "Lead The <color=#7B8968FF>Undead</color> To Victory";
            AbilitiesText = "- You can convert the <color=#8BFDFDFF>Crew</color> into your own sub faction.\n- If the target is a killing role, they are converted to " +
                "<color=#DF7AE8FF>Dampyr</color> otherwise they convert into a <color=#2BD29CFF>Vampire</color>.\n- If the target cannot be converted, they will be attacked instead." +
                "\n- There is a chance that there is a <color=#C0C0C0FF>Vampire Hunter</color>\non the loose. Attempting to convert them will make them kill you.";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Dracula : Colors.Neutral;
            SubFaction = SubFaction.Undead;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralNeo;
            AlignmentName = "Neutral (Neophyte)";
            Converted = new List<byte>();
            Converted.Add(Player.PlayerId);
            RoleDescription = "You are a Dracula! You are the leader of the Undead who drain blood from their enemies. Convert people to your side and " +
                "gain a quick majority.";
            SubFactionColor = Colors.Undead;
            SubFactionName = "Undead";
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        public float ConvertTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastBitten;
            var num = CustomGameOptions.BiteCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public override void Wins()
        {
            UndeadWin = true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            if (Player != PlayerControl.LocalPlayer)
                return;
                
            var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (Utils.UndeadWin())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                writer.Write((byte)WinLoseRPC.UndeadWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            
            return false;
        }

        public KillButton BiteButton
        {
            get => _biteButton;
            set
            {
                _biteButton = value;
                AddToAbilityButtons(value, this);
            }
        }
    }
}