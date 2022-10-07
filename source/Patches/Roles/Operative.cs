using Reactor.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using TownOfUs.CrewmateRoles.OperativeMod;
using UnityEngine;

namespace TownOfUs.Roles
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
            ImpostorText = () => "Detect Which Roles Are Here";
            TaskText = () => "Place bugs around the map to discover the <color=#FF0000FF>Intruders</color>";
            if (CustomGameOptions.CustomCrewColors) Color = Patches.Colors.Operative;
            else Color = Patches.Colors.Crew;
            RoleType = RoleEnum.Operative;
            lastBugged = DateTime.UtcNow;
            buggedPlayers = new List<RoleEnum>();
            Faction = Faction.Crewmates;
            FactionName = "Crew";
            UsesLeft = CustomGameOptions.MaxBugs;
            FactionColor = Patches.Colors.Crew;
            Alignment = Alignment.CrewInvest;
            AlignmentName = "Crew (Investigative)";
            AddToRoleHistory(RoleType);
        }

        public float BugTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - lastBugged;
            var num = CustomGameOptions.BugCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }


        public static AssetBundle loadBundle()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream("TownOfUs.Resources.operativeshader");
            var assets = stream.ReadFully();
            return AssetBundle.LoadFromMemory(assets);
        }
    }
}
