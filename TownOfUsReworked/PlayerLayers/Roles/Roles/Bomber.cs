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

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Bomber : Role
    {
        public static AssetBundle bundle = loadBundle();
        public static Material bombMaterial = bundle.LoadAsset<Material>("trap").DontUnload();
        public DateTime LastPlaced { get; set; }
        public DateTime LastDetonated { get; set; }

        public Bomber(PlayerControl player) : base(player)
        {
            Name = "Bomber";
            StartText = "Make People Go Boom";
            AbilitiesText = "- You can place bombs which you can detonate at any time to kill anyone within a certain radius.";
            AttributesText = "- Your bombs can even kill you and your fellow Syndicate so be careful when making people explode.";
            Color = IsRecruit ? Colors.Cabal : (CustomGameOptions.CustomSynColors ? Colors.Bomber : Colors.Syndicate);
            RoleType = RoleEnum.Bomber;
            Faction = Faction.Syndicate;
            FactionName = "Syndicate";
            FactionColor = Colors.Syndicate;
            RoleAlignment = RoleAlignment.SyndicateKill;
            AlignmentName = "Syndicate (Killing)";
            IntroText = SyndicateIntro;
            Results = InspResults.ArsoCryoPBOpTroll;
            Objectives = IsRecruit ? JackalWinCon : SyndicateWinCon;
            Attack = AttackEnum.Basic;
            AttackString = "Basic";
            RoleDescription = "You are a Bomber! You are a powerful demolitionist who can get a large number of body counts by detonating bombs placed at key points on the map. Be careful" + 
                " though, as any unofortunate Syndicate in the bomb's radius will also die. Perfectly timed detonations might be the key to victory.";
            FactionDescription = SyndicateFactionDescription;
            AlignmentDescription = SyKDescription;
            AddToRoleHistory(RoleType);
        }

        public float BombTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastPlaced;
            var num = CustomGameOptions.BombCooldown * 1000f;
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

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();

            team.Add(PlayerControl.LocalPlayer);

            if (!IsRecruit)
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Is(Faction) && player != PlayerControl.LocalPlayer)
                        team.Add(player);
                }
            }
            else
            {
                var jackal = Player.GetJackal();

                team.Add(jackal.Player);
                team.Add(jackal.GoodRecruit);
            }

            __instance.teamToShow = team;
        }

        public override void Wins()
        {
            if (IsRecruit)
                CabalWin = true;
            else
                SyndicateWin = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
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
            else if (Utils.SyndicateWins())
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
