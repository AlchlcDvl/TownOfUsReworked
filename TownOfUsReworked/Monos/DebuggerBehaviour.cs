using Reactor.Utilities.ImGui;

namespace TownOfUsReworked.Monos
{
    //Based off of Reactor.Debugger but merged with MCI and added some functions of my own for testing
    public class DebuggerBehaviour : MonoBehaviour
    {
        [HideFromIl2Cpp]
        public DragWindow TestWindow { get; }
        private static int ControllingFigure;

        public DebuggerBehaviour(IntPtr ptr) : base(ptr)
        {
            TestWindow = new(new(20, 20, 0, 0), "Reworked Debugger", () =>
            {
                GUILayout.Label("Name: " + DataManager.Player.Customization.Name);

                if (CustomPlayer.Local != null && !ConstantVariables.NoLobby && !CustomPlayer.LocalCustom.IsDead && !ConstantVariables.IsEnded &&
                    !ConstantVariables.GameHasEnded)
                {
                    CustomPlayer.Local.Collider.enabled = GUILayout.Toggle(CustomPlayer.Local.Collider.enabled, "Enable Player Collider");
                }

                if (ConstantVariables.IsLobby)
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
                        ControllingFigure++;

                        if (ControllingFigure == CustomPlayer.AllPlayers.Count)
                            ControllingFigure = 0;

                        MCIUtils.SwitchTo((byte)ControllingFigure);
                    }
                    else if (GUILayout.Button("Previous Player"))
                    {
                        ControllingFigure--;

                        if (ControllingFigure < 0)
                            ControllingFigure = CustomPlayer.AllPlayers.Count - 1;

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
                        CustomPlayer.Local.myTasks.ForEach(x => CustomPlayer.Local.RpcCompleteTask(x.Id));

                    if (GUILayout.Button("Redo Intro Sequence"))
                    {
                        Utils.HUD.StartCoroutine(Utils.HUD.CoFadeFullScreen(Color.clear, Color.black));
                        Utils.HUD.StartCoroutine(Utils.HUD.CoShowIntro());
                    }

                    if (GUILayout.Button("Start Meeting") && !Utils.Meeting)
                    {
                        CustomPlayer.Local.RemainingEmergencies++;
                        CustomPlayer.Local.CmdReportDeadBody(null);
                    }

                    if (GUILayout.Button("End Meeting") && Utils.Meeting)
                        Utils.Meeting.RpcClose();

                    if (GUILayout.Button("Kill Self"))
                        Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, CustomPlayer.Local);

                    if (GUILayout.Button("Kill All"))
                        CustomPlayer.AllPlayers.ForEach(x => Utils.RpcMurderPlayer(x, x));

                    if (GUILayout.Button("Revive Self"))
                        Utils.Revive(CustomPlayer.Local);

                    if (GUILayout.Button("Revive All"))
                        CustomPlayer.AllPlayers.ForEach(x => Utils.Revive(x));

                    if (GUILayout.Button("Log Dump"))
                    {
                        PlayerLayer.LocalLayers.ForEach(x => Utils.LogSomething(x.Name));
                        Utils.LogSomething("Is Dead - " + CustomPlayer.LocalCustom.IsDead);
                        Utils.LogSomething("Location - " + CustomPlayer.Local.transform.position);
                    }

                    if (GUILayout.Button("Flash"))
                    {
                        var r = (byte)URandom.RandomRangeInt(0, 256);
                        var g = (byte)URandom.RandomRangeInt(0, 256);
                        var b = (byte)URandom.RandomRangeInt(0, 256);
                        var flashColor = new Color32(r, g, b, 255);
                        Utils.Flash(flashColor, "Flash!");
                    }
                }

                if (CustomPlayer.Local)
                {
                    var position = CustomPlayer.Local.transform.position;
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