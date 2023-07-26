using Reactor.Utilities.ImGui;

namespace TownOfUsReworked.Monos
{
    //Based off of Reactor.Debugger but merged with MCI and added some functions of my own for testing
    public class DebuggerBehaviour : MonoBehaviour
    {
        [HideFromIl2Cpp]
        public DragWindow TestWindow { get; }
        private static byte ControllingFigure;

        public DebuggerBehaviour(IntPtr ptr) : base(ptr)
        {
            TestWindow = new(new(20, 20, 0, 0), "Reworked Debugger", () =>
            {
                GUILayout.Label("Name: " + DataManager.Player.Customization.Name);

                if (CustomPlayer.Local && !NoLobby && !CustomPlayer.LocalCustom.IsDead && !IsEnded && !GameHasEnded)
                    CustomPlayer.Local.Collider.enabled = GUILayout.Toggle(CustomPlayer.Local.Collider.enabled, "Enable Player Collider");

                if (IsLobby)
                {
                    TownOfUsReworked.IsTest = GUILayout.Toggle(TownOfUsReworked.IsTest, "Toggle Test Mode");
                    TownOfUsReworked.LobbyCapped = GUILayout.Toggle(TownOfUsReworked.LobbyCapped, "Toggle Lobby Cap");
                    TownOfUsReworked.Persistence = GUILayout.Toggle(TownOfUsReworked.Persistence, "Toggle Bot Persistence");

                    if (GUILayout.Button("Spawn Bot"))
                    {
                        if ((CustomPlayer.AllPlayers.Count < CustomGameOptions.LobbySize && TownOfUsReworked.LobbyCapped) || !TownOfUsReworked.LobbyCapped)
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
                    TownOfUsReworked.SameVote = GUILayout.Toggle(TownOfUsReworked.SameVote, "Toggle All Bots Vote");
                    Role.SyndicateHasChaosDrive = GUILayout.Toggle(Role.SyndicateHasChaosDrive, "Toggle Chaos Drive");

                    if (Role.SyndicateHasChaosDrive)
                        RoleGen.AssignChaosDrive();
                    else
                        Role.DriveHolder = null;

                    if (GUILayout.Button("Next Player"))
                    {
                        ControllingFigure = (byte)CycleFloat(CustomPlayer.AllPlayers.Count - 1, 0, ControllingFigure, true);
                        MCIUtils.SwitchTo(ControllingFigure);
                    }
                    else if (GUILayout.Button("Previous Player"))
                    {
                        ControllingFigure = (byte)CycleFloat(CustomPlayer.AllPlayers.Count - 1, 0, ControllingFigure, false);
                        MCIUtils.SwitchTo(ControllingFigure);
                    }

                    if (GUILayout.Button("End Game"))
                    {
                        PlayerLayer.NobodyWins = true;
                        EndGame();
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
                        DefaultOutfitAll();
                    }

                    if (GUILayout.Button("Complete Tasks"))
                        CustomPlayer.Local.myTasks.ForEach(x => CustomPlayer.Local.RpcCompleteTask(x.Id));

                    if (GUILayout.Button("Complete Everyone's Tasks"))
                        CustomPlayer.AllPlayers.ForEach(x => x.myTasks.ForEach(y => x.RpcCompleteTask(y.Id)));

                    if (GUILayout.Button("Redo Intro Sequence"))
                    {
                        HUD.StartCoroutine(HUD.CoFadeFullScreen(Color.clear, Color.black));
                        HUD.StartCoroutine(HUD.CoShowIntro());
                    }

                    if (GUILayout.Button("Start Meeting") && !Meeting)
                    {
                        CustomPlayer.Local.RemainingEmergencies++;
                        CustomPlayer.Local.CmdReportDeadBody(null);
                    }

                    if (GUILayout.Button("End Meeting") && Meeting)
                        Meeting.RpcClose();

                    if (GUILayout.Button("Kill Self"))
                        RpcMurderPlayer(CustomPlayer.Local, CustomPlayer.Local);

                    if (GUILayout.Button("Kill All"))
                        CustomPlayer.AllPlayers.ForEach(x => RpcMurderPlayer(x, x));

                    if (GUILayout.Button("Revive Self"))
                        CustomPlayer.Local.Revive();

                    if (GUILayout.Button("Revive All"))
                        CustomPlayer.AllPlayers.ForEach(x => x.Revive());

                    if (GUILayout.Button("Log Dump"))
                    {
                        PlayerLayer.LocalLayers.ForEach(x => LogSomething(x.Name));
                        LogSomething("Is Dead - " + CustomPlayer.LocalCustom.IsDead);
                        LogSomething("Location - " + CustomPlayer.LocalCustom.Position);
                    }

                    if (GUILayout.Button("Flash"))
                    {
                        var r = (byte)URandom.RandomRangeInt(0, 256);
                        var g = (byte)URandom.RandomRangeInt(0, 256);
                        var b = (byte)URandom.RandomRangeInt(0, 256);
                        var flashColor = new Color32(r, g, b, 255);
                        Flash(flashColor, "Flash!");
                    }
                }

                if (CustomPlayer.Local)
                {
                    var position = CustomPlayer.LocalCustom.Position;
                    GUILayout.Label($"Player Position\nx: {position.x:00.00} y: {position.y:00.00} z: {position.z:00.00}");

                    var mouse = Input.mousePosition;
                    GUILayout.Label($"Mouse Position\nx: {mouse.x:00.00} y: {mouse.y:00.00} z: {mouse.z:00.00}");
                }
            })
            {
                Enabled = false,
            };
        }

        public void Update()
        {
            if (NoPlayers || !IsLocalGame)
            {
                if (TestWindow.Enabled)
                    TestWindow.Enabled = false;

                return; //You must ensure you are only playing on local
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                TestWindow.Enabled = !TestWindow.Enabled;
                SettingsPatches.PresetButton.LoadPreset("Last Used", true);

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