using System.Linq;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using System;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Jackal : Role
    {
        public PlayerControl EvilRecruit = null;
        public PlayerControl GoodRecruit = null;
        public PlayerControl BackupRecruit = null;
        public KillButton _recruitButton;
        public DateTime LastRecruited { get; set; }
        public bool Recruited => BackupRecruit != null;

        public Jackal(PlayerControl player) : base(player)
        {
            Name = "Jackal";
            Faction = Faction.Neutral;
            RoleType = RoleEnum.Jackal;
            StartText = " ";
            AbilitiesText = " ";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Jackal : Colors.Neutral;
            SubFaction = SubFaction.Cabal;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralNeo;
            AlignmentName = "Neutral (Neophyte)";
            IntroText = "Gain a majority";
            Results = InspResults.SurvVHVampVig;
            IntroSound = null;
            Attack = AttackEnum.None;
            Defense = DefenseEnum.None;
            AttackString = "None";
            Base = false;
            IsRecruit = false;
            DefenseString = "None";
            AddToRoleHistory(RoleType);
        }

        public override void Wins()
        {
            CabalWin = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead | Player.Data.Disconnected)
                return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Data.IsImpostor() |
                x.Is(RoleAlignment.NeutralKill) | (x.Is(RoleAlignment.NeutralNeo) && !x.Is(RoleEnum.Jackal)) | x.Is(Faction.Syndicate) |
                x.Is(RoleAlignment.NeutralPros)) && !x.IsRecruit()) == 0)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.UndeadWin,
                    SendOption.Reliable, -1);
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            
            return false;
        }

        public KillButton RecruitButton
        {
            get => _recruitButton;
            set
            {
                _recruitButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
    }
}