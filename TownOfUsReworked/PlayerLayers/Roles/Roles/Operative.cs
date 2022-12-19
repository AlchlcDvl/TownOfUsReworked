using Reactor.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.OperativeMod;
using TownOfUsReworked.Lobby.CustomOption;
using UnityEngine;
using TownOfUsReworked.Extensions;
using Hazel;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Operative : Role
    {
        public static AssetBundle bundle = loadBundle();
        public static Material bugMaterial = bundle.LoadAsset<Material>("trap").DontUnload();
        public List<Bug> bugs = new List<Bug>();
        public DateTime lastBugged { get; set; }
        public int UsesLeft;
        public TextMeshPro UsesText;
        public List<RoleEnum> buggedPlayers;
        public bool ButtonUsable => UsesLeft != 0;

        public Operative(PlayerControl player) : base(player)
        {
            Name = "Operative";
            StartText = "Detect Which Roles Are Here";
            AbilitiesText = "Place bugs around the map to discover the <color=#FF0000FF>Intruders</color>";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Operative : Colors.Crew;
            SubFaction = SubFaction.None;
            RoleType = RoleEnum.Operative;
            lastBugged = DateTime.UtcNow;
            buggedPlayers = new List<RoleEnum>();
            Faction = Faction.Crew;
            FactionName = "Crew";
            UsesLeft = CustomGameOptions.MaxBugs;
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewInvest;
            AlignmentName = "Crew (Investigative)";
            IntroText = "Eject all <color=#FF0000FF>evildoers</color>";
            Results = InspResults.ArsoCryoPBOpTroll;
            Attack = AttackEnum.None;
            Defense = DefenseEnum.None;
            AttackString = "None";
            DefenseString = "None";
            IntroSound = null;
            Base = false;
            IsRecruit = false;
            AddToRoleHistory(RoleType);
        }

        public float BugTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - lastBugged;
            var num = CustomGameOptions.BugCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public static AssetBundle loadBundle()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream("TownOfUsReworked.Resources.Sounds.operativeshader");
            var assets = stream.ReadFully();
            return AssetBundle.LoadFromMemory(assets);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }

        public override void Wins()
        {
            CrewWin = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead | Player.Data.Disconnected)
                return true;

            if (Utils.CrewWins())
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CrewWin,
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
