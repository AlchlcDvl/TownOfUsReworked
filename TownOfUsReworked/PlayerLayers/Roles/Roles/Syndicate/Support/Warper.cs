using Hazel;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using Object = UnityEngine.Object;
using Reactor.Networking.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using System;
using Random = UnityEngine.Random;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Warper : Role
    {
        private KillButton _warpButton;
        public DateTime LastWarped { get; set; }
        public DateTime LastKilled { get; set; }
        private KillButton _killButton;
        public PlayerControl ClosestPlayer = null;

        public Warper(PlayerControl player) : base(player)
        {
            Name = "Warper";
            StartText = "Warp The Crew Away From Each Other";
            AbilitiesText = "Separate the Crew";
            Color = CustomGameOptions.CustomSynColors ? Colors.Warper : Colors.Syndicate;
            RoleType = RoleEnum.Warper;
            Faction = Faction.Syndicate;
            FactionColor = Colors.Syndicate;
            FactionName = "Syndicate";
            AlignmentName = SSu;
            //IntroSound = TownOfUsReworked.WarperIntro;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.ChaosDriveKillCooldown, Utils.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public KillButton KillButton
        {
            get => _killButton;
            set
            {
                _killButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public void Warp()
        {
            Dictionary<byte, Vector2> coordinates = GenerateWarpCoordinates();

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.Warp);
            writer.Write((byte)coordinates.Count);

            foreach ((byte key, Vector2 value) in coordinates)
            {
                writer.Write(key);
                writer.Write(value);
            }

            AmongUsClient.Instance.FinishRpcImmediately(writer);
            WarpPlayersToCoordinates(coordinates);
        }

        public static void WarpPlayersToCoordinates(Dictionary<byte, Vector2> coordinates)
        {
            if (coordinates.ContainsKey(PlayerControl.LocalPlayer.PlayerId))
            {
                Coroutines.Start(Utils.FlashCoroutine(Colors.Warper));

                if (Minigame.Instance)
                    Minigame.Instance.Close();

                if (PlayerControl.LocalPlayer.inVent)
                {
                    PlayerControl.LocalPlayer.MyPhysics.RpcExitVent(Vent.currentVent.Id);
                    PlayerControl.LocalPlayer.MyPhysics.ExitAllVents();
                }
            }

            foreach ((byte key, Vector2 value) in coordinates)
            {
                PlayerControl player = Utils.PlayerById(key);
                player.transform.position = value;
            }
        }

        private Dictionary<byte, Vector2> GenerateWarpCoordinates()
        {
            List<PlayerControl> targets = PlayerControl.AllPlayerControls.ToArray().Where(player => !player.Data.IsDead && !player.Data.Disconnected).ToList();
            HashSet<Vent> vents = Object.FindObjectsOfType<Vent>().ToHashSet();
            Dictionary<byte, Vector2> coordinates = new Dictionary<byte, Vector2>(targets.Count);
            var rnd = new System.Random((int)DateTime.Now.Ticks);

            List<Vector3> SkeldPositions = new List<Vector3>()
            {
                new Vector3(-2.2f, 2.2f, 0f), //cafeteria. botton. top left.
                new Vector3(0.7f, 2.2f, 0f), //caffeteria. button. top right.
                new Vector3(-2.2f, -0.2f, 0f), //caffeteria. button. bottom left.
                new Vector3(0.7f, -0.2f, 0f), //caffeteria. button. bottom right.
                new Vector3(10.0f, 3.0f, 0f), //weapons top
                new Vector3(9.0f, 1.0f, 0f), //weapons bottom
                new Vector3(6.5f, -3.5f, 0f), //O2
                new Vector3(11.5f, -3.5f, 0f), //O2-nav hall
                new Vector3(17.0f, -3.5f, 0f), //navigation top
                new Vector3(18.2f, -5.7f, 0f), //navigation bottom
                new Vector3(11.5f, -6.5f, 0f), //nav-shields top
                new Vector3(9.5f, -8.5f, 0f), //nav-shields bottom
                new Vector3(9.2f, -12.2f, 0f), //shields top
                new Vector3(8.0f, -14.3f, 0f), //shields bottom
                new Vector3(2.5f, -16f, 0f), //coms left
                new Vector3(4.2f, -16.4f, 0f), //coms middle
                new Vector3(5.5f, -16f, 0f), //coms right
                new Vector3(-1.5f, -10.0f, 0f), //storage top
                new Vector3(-1.5f, -15.5f, 0f), //storage bottom
                new Vector3(-4.5f, -12.5f, 0f), //storrage left
                new Vector3(0.3f, -12.5f, 0f), //storrage right
                new Vector3(4.5f, -7.5f, 0f), //admin top
                new Vector3(4.5f, -9.5f, 0f), //admin bottom
                new Vector3(-9.0f, -8.0f, 0f), //elec top left
                new Vector3(-6.0f, -8.0f, 0f), //elec top right
                new Vector3(-8.0f, -11.0f, 0f), //elec bottom
                new Vector3(-12.0f, -13.0f, 0f), //elec-lower hall
                new Vector3(-17f, -10f, 0f), //lower engine top
                new Vector3(-17.0f, -13.0f, 0f), //lower engine bottom
                new Vector3(-21.5f, -3.0f, 0f), //reactor top
                new Vector3(-21.5f, -8.0f, 0f), //reactor bottom
                new Vector3(-13.0f, -3.0f, 0f), //security top
                new Vector3(-12.6f, -5.6f, 0f), // security bottom
                new Vector3(-17.0f, 2.5f, 0f), //upper engibe top
                new Vector3(-17.0f, -1.0f, 0f), //upper engine bottom
                new Vector3(-10.5f, 1.0f, 0f), //upper-mad hall
                new Vector3(-10.5f, -2.0f, 0f), //medbay top
                new Vector3(-6.5f, -4.5f, 0f) //medbay bottom
            };

            List<Vector3> MiraPositions = new List<Vector3>()
            {
                new Vector3(-4.5f, 3.5f, 0f), //launchpad top
                new Vector3(-4.5f, -1.4f, 0f), //launchpad bottom
                new Vector3(8.5f, -1f, 0f), //launchpad- med hall
                new Vector3(14f, -1.5f, 0f), //medbay
                new Vector3(16.5f, 3f, 0f), // comms
                new Vector3(10f, 5f, 0f), //lockers
                new Vector3(6f, 1.5f, 0f), //locker room
                new Vector3(2.5f, 13.6f, 0f), //reactor
                new Vector3(6f, 12f, 0f), //reactor middle
                new Vector3(9.5f, 13f, 0f), //lab
                new Vector3(15f, 9f, 0f), //bottom left cross
                new Vector3(17.9f, 11.5f, 0f), //middle cross
                new Vector3(14f, 17.3f, 0f), //office
                new Vector3(19.5f, 21f, 0f), //admin
                new Vector3(14f, 24f, 0f), //greenhouse left
                new Vector3(22f, 24f, 0f), //greenhouse right
                new Vector3(21f, 8.5f, 0f), //bottom right cross
                new Vector3(28f, 3f, 0f), //caf right
                new Vector3(22f, 3f, 0f), //caf left
                new Vector3(19f, 4f, 0f), //storage
                new Vector3(22f, -2f, 0f), //balcony
            };

            List<Vector3> PolusPositions = new List<Vector3>()
            {
                new Vector3(16.6f, -1f, 0f), //dropship top
                new Vector3(16.6f, -5f, 0f), //dropship bottom
                new Vector3(20f, -9f, 0f), //above storrage
                new Vector3(22f, -7f, 0f), //right fuel
                new Vector3(25.5f, -6.9f, 0f), //drill
                new Vector3(29f, -9.5f, 0f), //lab lockers
                new Vector3(29.5f, -8f, 0f), //lab weather notes
                new Vector3(35f, -7.6f, 0f), //lab table
                new Vector3(40.4f, -8f, 0f), //lab scan
                new Vector3(33f, -10f, 0f), //lab toilet
                new Vector3(39f, -15f, 0f), //specimen hall top
                new Vector3(36.5f, -19.5f, 0f), //specimen top
                new Vector3(36.5f, -21f, 0f), //specimen bottom
                new Vector3(28f, -21f, 0f), //specimen hall bottom
                new Vector3(24f, -20.5f, 0f), //admin tv
                new Vector3(22f, -25f, 0f), //admin books
                new Vector3(16.6f, -17.5f, 0f), //office coffe
                new Vector3(22.5f, -16.5f, 0f), //office projector
                new Vector3(24f, -17f, 0f), //office figure
                new Vector3(27f, -16.5f, 0f), //office lifelines
                new Vector3(32.7f, -15.7f, 0f), //lavapool
                new Vector3(31.5f, -12f, 0f), //snowmad below lab
                new Vector3(10f, -14f, 0f), //below storrage
                new Vector3(21.5f, -12.5f, 0f), //storrage vent
                new Vector3(19f, -11f, 0f), //storrage toolrack
                new Vector3(12f, -7f, 0f), //left fuel
                new Vector3(5f, -7.5f, 0f), //above elec
                new Vector3(10f, -12f, 0f), //elec fence
                new Vector3(9f, -9f, 0f), //elec lockers
                new Vector3(5f, -9f, 0f), //elec window
                new Vector3(4f, -11.2f, 0f), //elec tapes
                new Vector3(5.5f, -16f, 0f), //elec-O2 hall
                new Vector3(1f, -17.5f, 0f), //O2 tree hayball
                new Vector3(3f, -21f, 0f), //O2 middle
                new Vector3(2f, -19f, 0f), //O2 gas
                new Vector3(1f, -24f, 0f), //O2 water
                new Vector3(7f, -24f, 0f), //under O2
                new Vector3(9f, -20f, 0f), //right outside of O2
                new Vector3(7f, -15.8f, 0f), //snowman under elec
                new Vector3(11f, -17f, 0f), //comms table
                new Vector3(12.7f, -15.5f, 0f), //coms antenna pult
                new Vector3(13f, -24.5f, 0f), //weapons window
                new Vector3(15f, -17f, 0f), //between coms-office
                new Vector3(17.5f, -25.7f, 0f), //snowman under office
            };

            List<Vector3> dlekSPositions = new List<Vector3>()
            {
                new Vector3(2.2f, 2.2f, 0f), //cafeteria. botton. top left.
                new Vector3(-0.7f, 2.2f, 0f), //caffeteria. button. top right.
                new Vector3(2.2f, -0.2f, 0f), //caffeteria. button. bottom left.
                new Vector3(-0.7f, -0.2f, 0f), //caffeteria. button. bottom right.
                new Vector3(-10.0f, 3.0f, 0f), //weapons top
                new Vector3(-9.0f, 1.0f, 0f), //weapons bottom
                new Vector3(-6.5f, -3.5f, 0f), //O2
                new Vector3(-11.5f, -3.5f, 0f), //O2-nav hall
                new Vector3(-17.0f, -3.5f, 0f), //navigation top
                new Vector3(-18.2f, -5.7f, 0f), //navigation bottom
                new Vector3(-11.5f, -6.5f, 0f), //nav-shields top
                new Vector3(-9.5f, -8.5f, 0f), //nav-shields bottom
                new Vector3(-9.2f, -12.2f, 0f), //shields top
                new Vector3(-8.0f, -14.3f, 0f), //shields bottom
                new Vector3(-2.5f, -16f, 0f), //coms left
                new Vector3(-4.2f, -16.4f, 0f), //coms middle
                new Vector3(-5.5f, -16f, 0f), //coms right
                new Vector3(1.5f, -10.0f, 0f), //storage top
                new Vector3(1.5f, -15.5f, 0f), //storage bottom
                new Vector3(4.5f, -12.5f, 0f), //storrage left
                new Vector3(-0.3f, -12.5f, 0f), //storrage right
                new Vector3(-4.5f, -7.5f, 0f), //admin top
                new Vector3(-4.5f, -9.5f, 0f), //admin bottom
                new Vector3(9.0f, -8.0f, 0f), //elec top left
                new Vector3(6.0f, -8.0f, 0f), //elec top right
                new Vector3(8.0f, -11.0f, 0f), //elec bottom
                new Vector3(12.0f, -13.0f, 0f), //elec-lower hall
                new Vector3(17f, -10f, 0f), //lower engine top
                new Vector3(17.0f, -13.0f, 0f), //lower engine bottom
                new Vector3(21.5f, -3.0f, 0f), //reactor top
                new Vector3(21.5f, -8.0f, 0f), //reactor bottom
                new Vector3(13.0f, -3.0f, 0f), //security top
                new Vector3(12.6f, -5.6f, 0f), // security bottom
                new Vector3(17.0f, 2.5f, 0f), //upper engibe top
                new Vector3(17.0f, -1.0f, 0f), //upper engine bottom
                new Vector3(10.5f, 1.0f, 0f), //upper-mad hall
                new Vector3(10.5f, -2.0f, 0f), //medbay top
                new Vector3(6.5f, -4.5f, 0f) //medbay bottom
            };
            
            var map = GameOptionsManager.Instance.currentNormalGameOptions.MapId;

            foreach (PlayerControl target in targets)
            {
                var coin = Random.RandomRangeInt(0, 2);
                Vector3 destination = new Vector3();

                if (coin == 0 || !SyndicateHasChaosDrive)
                {
                    Vent vent = vents.Random();
                    destination = SendPlayerToVent(vent);
                }
                else if (coin == 1)
                {
                    switch (map)
                    {
                        case 0:
                            destination = SkeldPositions[rnd.Next(SkeldPositions.Count)];
                            break;

                        case 1:
                            destination = MiraPositions[rnd.Next(MiraPositions.Count)];
                            break;

                        case 2:
                            destination = PolusPositions[rnd.Next(PolusPositions.Count)];
                            break;

                        case 3:
                            destination = dlekSPositions[rnd.Next(dlekSPositions.Count)];
                            break;
                        
                        default:
                            break;
                    }
                }

                coordinates.Add(target.PlayerId, destination);
            }

            return coordinates;
        }

        public static Vector3 SendPlayerToVent(Vent vent)
        {
            Vector2 size = vent.GetComponent<BoxCollider2D>().size;
            Vector3 destination = vent.transform.position;
            destination.y += 0.3636f;
            return destination;
        }

        public override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            if (Player != PlayerControl.LocalPlayer)
                return;
                
            var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(Faction) && player != PlayerControl.LocalPlayer)
                    team.Add(player);
            }

            if (IsRecruit)
            {
                var jackal = Player.GetJackal();
                team.Add(jackal.Player);
                team.Add(jackal.GoodRecruit);
            }

            __instance.teamToShow = team;
        }

        public float WarpTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastWarped;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.WarpCooldown, Utils.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
        
        public KillButton WarpButton
        {
            get => _warpButton;
            set
            {
                _warpButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public override void Wins()
        {
            if (IsRecruit)
                CabalWin = true;
            else if (IsPersuaded)
                SectWin = true;
            else if (IsBitten)
                UndeadWin = true;
            else if (IsResurrected)
                ReanimatedWin = true;
            else
                SyndicateWin = true;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (IsRecruit)
            {
                if (Utils.CabalWin())
                {
                    Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.CabalWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (IsPersuaded)
            {
                if (Utils.SectWin())
                {
                    Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.SectWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (IsBitten)
            {
                if (Utils.UndeadWin())
                {
                    Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.UndeadWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (IsResurrected)
            {
                if (Utils.ReanimatedWin())
                {
                    Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.ReanimatedWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (Utils.SyndicateWins())
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.SyndicateWin);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }
    }
}