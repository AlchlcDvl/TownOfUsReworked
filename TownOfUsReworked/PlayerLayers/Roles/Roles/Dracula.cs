using System;
using System.Collections.Generic;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Dracula : Role
    {
        public DateTime LastBitten { get; set; }
        public bool HasMaj;
        private KillButton _biteButton;
        public PlayerControl ClosestPlayer;
        public List<PlayerControl> Converted = new List<PlayerControl>();

        public Dracula(PlayerControl player) : base(player)
        {
            Name = "Dracula";
            Faction = Faction.Neutral;
            RoleType = RoleEnum.Dracula;
            StartText = "Lead The <color=#7B8968FF>Undead</color> To Victory";
            AbilitiesText = "- You can convert the <color=#8BFDFDFF>Crew</color> into your own sub faction.";
            AttributesText = "- If the target is a killing role, they are converted to <color=#DF7AE8FF>Dampyr</color> otherwise they convert into a " +
                "<color=#2BD29CFF>Vampire</color>.\n- If the target cannot be converted, they will be attacked instead.\n- There is a chance that there" +
                " is a <color=#C0C0C0FF>Vampire Hunter</color>\non the loose. Attempting to convert them will make them kill you.";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Dracula : Colors.Neutral;
            SubFaction = SubFaction.Undead;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralNeo;
            AlignmentName = "Neutral (Neophyte)";
            Attack = AttackEnum.Basic;
            AttackString = "Basic";
            FactionDescription = NeutralFactionDescription;
            AlignmentDescription = NNDescription;
            RoleDescription = "You are a Dracula! You are the leader of the Undead who drain blood from their enemies. Convert people to your side and " +
                "gain a quick majority.";
            Results = InspResults.ShiftSwapSKDrac;
            Objectives = UndeadWinCon;
            SubFactionColor = Colors.Undead;
            AddToRoleHistory(RoleType);
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

        public override void Loses()
        {
            LostByRPC = true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();

            team.Add(PlayerControl.LocalPlayer);

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(SubFaction) && player != PlayerControl.LocalPlayer)
                    team.Add(player);
            }

            __instance.teamToShow = team;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (Utils.UndeadWin())
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,(byte)CustomRPC.UndeadWin,
                    SendOption.Reliable,-1);
                writer.Write(Player.PlayerId);
                Wins();
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
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public List<PlayerControl> Vamps()
        {
            var AllVamps = new List<PlayerControl>();

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(SubFaction.Undead))
                    AllVamps.Add(player);
            }

            return AllVamps;
        }

        public List<PlayerControl> AliveVamps()
        {
            var vamp = Vamps();

            foreach (var player in vamp)
            {
                if (player.Data.IsDead || player.Data.Disconnected)
                    vamp.Remove(player);
            }

            return vamp;
        }
    }
}