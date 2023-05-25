using Reactor.Utilities.ImGui;

namespace TownOfUsReworked.Monos
{
    //Based off of Reactor.Debugger but merged with MCI and added some functions of my own for testing
    public class Debugger : MonoBehaviour
    {
        [HideFromIl2Cpp]
        public DragWindow TestWindow { get; }
        private static int ControllingFigure;

        public Debugger(IntPtr ptr) : base(ptr)
        {
            TestWindow = new(new(20, 20, 0, 0), "MCI Debugger", () =>
            {
                GUILayout.Label("Name: " + DataManager.Player.Customization.Name);

                if (PlayerControl.LocalPlayer != null && !ConstantVariables.NoLobby && !PlayerControl.LocalPlayer.Data.IsDead && !ConstantVariables.IsEnded &&
                    !ConstantVariables.GameHasEnded)
                {
                    PlayerControl.LocalPlayer.Collider.enabled = GUILayout.Toggle(PlayerControl.LocalPlayer.Collider.enabled, "Enable Player Collider");
                }

                if (ConstantVariables.IsLobby)
                {
                    TownOfUsReworked.LobbyCapped = GUILayout.Toggle(TownOfUsReworked.LobbyCapped, "Toggle Lobby Cap");
                    TownOfUsReworked.Persistence = GUILayout.Toggle(TownOfUsReworked.Persistence, "Toggle Bot Persistence");

                    if (GUILayout.Button("Spawn Bot"))
                    {
                        if ((PlayerControl.AllPlayerControls.Count < CustomGameOptions.LobbySize && TownOfUsReworked.LobbyCapped) || !TownOfUsReworked.LobbyCapped)
                        {
                            MCIUtils.CleanUpLoad();
                            MCIUtils.CreatePlayerInstance();
                            TownOfUsReworked.MCIActive = true;
                        }
                    }

                    if (GUILayout.Button("Remove Last Bot"))
                    {
                        MCIUtils.RemovePlayer((byte)MCIUtils.Clients.Count);

                        if (MCIUtils.Clients.Count == 0)
                            TownOfUsReworked.MCIActive = false;
                    }

                    if (GUILayout.Button("Remove All Bots"))
                    {
                        MCIUtils.RemoveAllPlayers();
                        TownOfUsReworked.MCIActive = false;
                    }
                }
                else if (TownOfUsReworked.MCIActive)
                {
                    Role.SyndicateHasChaosDrive = GUILayout.Toggle(Role.SyndicateHasChaosDrive, "Enable Chaos Drive");

                    if (Role.SyndicateHasChaosDrive)
                        RoleGen.AssignChaosDrive();
                    else
                        Role.DriveHolder = null;

                    if (GUILayout.Button("Next Player"))
                    {
                        ControllingFigure++;

                        if (ControllingFigure == PlayerControl.AllPlayerControls.Count)
                            ControllingFigure = 0;

                        MCIUtils.SwitchTo((byte)ControllingFigure);
                    }
                    else if (GUILayout.Button("Previous Player"))
                    {
                        ControllingFigure--;

                        if (ControllingFigure < 0)
                            ControllingFigure = PlayerControl.AllPlayerControls.Count - 1;

                        MCIUtils.SwitchTo((byte)ControllingFigure);
                    }

                    if (GUILayout.Button("End Game"))
                    {
                        PlayerLayer.NobodyWins = true;
                        Utils.EndGame();
                    }

                    if (GUILayout.Button("Fix All Sabotages"))
                    {
                        ShipStatus.Instance.RpcRepairSystem(SystemTypes.Doors, 79);
                        ShipStatus.Instance.RpcRepairSystem(SystemTypes.Doors, 80);
                        ShipStatus.Instance.RpcRepairSystem(SystemTypes.Doors, 81);
                        ShipStatus.Instance.RpcRepairSystem(SystemTypes.Doors, 82);
                        ShipStatus.Instance.RpcRepairSystem(SystemTypes.LifeSupp, 16);
                        ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 16);
                        ShipStatus.Instance.RpcRepairSystem(SystemTypes.Laboratory, 16);
                        ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 16 | 0);
                        ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 16 | 1);
                        ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 16 | 0);
                        ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 16 | 1);
                        ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 0);
                        Utils.DefaultOutfitAll();
                    }

                    if (GUILayout.Button("Complete Tasks"))
                        PlayerControl.LocalPlayer.myTasks.ForEach(x => PlayerControl.LocalPlayer.RpcCompleteTask(x.Id));

                    if (GUILayout.Button("Redo Intro Sequence"))
                    {
                        HudManager.Instance.StartCoroutine(HudManager.Instance.CoFadeFullScreen(Color.clear, Color.black));
                        HudManager.Instance.StartCoroutine(HudManager.Instance.CoShowIntro());
                    }

                    if (GUILayout.Button("Start Meeting") && !MeetingHud.Instance)
                        PlayerControl.LocalPlayer.CmdReportDeadBody(null);

                    if (GUILayout.Button("End Meeting") && MeetingHud.Instance)
                        PlayerControl.AllPlayerControls.ForEach(x => MeetingHud.Instance.CmdCastVote(x.PlayerId, x.PlayerId));

                    if (GUILayout.Button("Kill Self"))
                        Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);

                    if (GUILayout.Button("Kill All"))
                        PlayerControl.AllPlayerControls.ForEach(x => Utils.RpcMurderPlayer(x, x));

                    if (GUILayout.Button("Revive Self"))
                        Utils.Revive(PlayerControl.LocalPlayer);

                    if (GUILayout.Button("Revive All"))
                        PlayerControl.AllPlayerControls.ForEach(x => Utils.Revive(x));

                    if (GUILayout.Button("Log Dump"))
                    {
                        PlayerLayer.LocalLayers.ForEach(x => Utils.LogSomething(x.Name));
                        Utils.LogSomething("Is Dead - " + PlayerControl.LocalPlayer.Data.IsDead);
                    }
                }

                if (GUILayout.Button("Flash"))
                {
                    var r = (byte)URandom.RandomRangeInt(0, 256);
                    var g = (byte)URandom.RandomRangeInt(0, 256);
                    var b = (byte)URandom.RandomRangeInt(0, 256);
                    var flashColor = new Color32(r, g, b, 255);
                    Utils.Flash(flashColor);
                }

                if (PlayerControl.LocalPlayer)
                {
                    var position = PlayerControl.LocalPlayer.transform.position;
                    GUILayout.Label($"x: {position.x:00.00}");
                    GUILayout.Label($"y: {position.y:00.00}");
                    GUILayout.Label($"z: {position.z:00.00}");
                }
            })
            {
                Enabled = false,
            };
        }

        public void Update()
        {
            if (ConstantVariables.NoPlayers || !ConstantVariables.IsLocalGame)
            {
                TestWindow.Enabled = false;
                return; //You must ensure you are only playing on local
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                TestWindow.Enabled = !TestWindow.Enabled;
                SettingsPatches.PresetButton.LoadPreset("LastUsed", true);

                if (!TestWindow.Enabled)
                {
                    MCIUtils.RemoveAllPlayers();
                    TownOfUsReworked.MCIActive = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.F2))
                TestWindow.Enabled = !TestWindow.Enabled;
        }

        public void OnGUI() => TestWindow.OnGUI();
    }
}