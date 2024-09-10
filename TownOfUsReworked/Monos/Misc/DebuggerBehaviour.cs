using Reactor.Utilities.ImGui;

namespace TownOfUsReworked.Monos;

// Based off of Reactor.Debugger but merged with MCI and added some functions and changes of my own for testing
public class DebuggerBehaviour : MonoBehaviour
{
    [HideFromIl2Cpp]
    public DragWindow TestWindow { get; }

    [HideFromIl2Cpp]
    public DragWindow CooldownsWindow { get; }

    public byte ControllingFigure;

    public static DebuggerBehaviour Instance { get; private set; }

    public DebuggerBehaviour(IntPtr ptr) : base(ptr)
    {
        Instance = this;

        TestWindow = new(new(20, 20, 0, 0), "Reworked Debugger", () =>
        {
            GUILayout.Label("Name: " + DataManager.Player.Customization.Name);

            if (GUILayout.Button("Close Testing Menu"))
                TestWindow.Enabled = false;

            if (CustomPlayer.Local && !NoLobby() && !CustomPlayer.LocalCustom.Dead && !IsEnded() && WinState == WinLose.None)
                CustomPlayer.Local.Collider.enabled = GUILayout.Toggle(CustomPlayer.Local.Collider.enabled, "Enable Player Collider");

            if (IsLobby())
            {
                TownOfUsReworked.IsTest = GUILayout.Toggle(TownOfUsReworked.IsTest, "Toggle Test Mode");
                TownOfUsReworked.LobbyCapped = GUILayout.Toggle(TownOfUsReworked.LobbyCapped, "Toggle Lobby() Cap");
                TownOfUsReworked.Persistence = GUILayout.Toggle(TownOfUsReworked.Persistence, "Toggle Bot Persistence");

                if (GUILayout.Button("Spawn Bot"))
                {
                    if ((AllPlayers().Count < GameSettings.LobbySize && TownOfUsReworked.LobbyCapped) || (!TownOfUsReworked.LobbyCapped && AllPlayers().Count < 128))
                    {
                        MCIUtils.CleanUpLoad();
                        MCIUtils.CreatePlayerInstance();
                        TownOfUsReworked.MCIActive = true;
                    }
                }

                if (GUILayout.Button("Remove Last Bot"))
                {
                    MCIUtils.RemovePlayer((byte)MCIUtils.Clients.Count);
                    ControllingFigure = 0;

                    if (MCIUtils.Clients.Count == 0)
                        TownOfUsReworked.MCIActive = false;
                }

                if (GUILayout.Button("Remove All Bots"))
                {
                    MCIUtils.RemoveAllPlayers();
                    ControllingFigure = 0;
                    TownOfUsReworked.MCIActive = false;
                }

                // if (GUILayout.Button("Load Last Settings"))
                //     SettingsPatches.PresetButton.LoadPreset("Last Used");
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
                    ControllingFigure = CycleByte(AllPlayers().Count - 1, 0, ControllingFigure, true);
                    MCIUtils.SwitchTo(ControllingFigure);
                }
                else if (GUILayout.Button("Previous Player"))
                {
                    ControllingFigure = CycleByte(AllPlayers().Count - 1, 0, ControllingFigure, false);
                    MCIUtils.SwitchTo(ControllingFigure);
                }

                if (GUILayout.Button("End Game"))
                    CheckEndGame.PerformStalemate();

                if (GUILayout.Button("Fix All Sabotages"))
                {
                    FixExtentions.Fix();
                    DefaultOutfitAll();
                }

                if (GUILayout.Button("Complete Tasks"))
                    CustomPlayer.Local.myTasks.ForEach(x => CustomPlayer.Local.RpcCompleteTask(x.Id));

                if (GUILayout.Button("Complete Everyone's Tasks"))
                    AllPlayers().ForEach(x => x.myTasks.ForEach(y => x.RpcCompleteTask(y.Id)));

                if (GUILayout.Button("Redo Intro Sequence"))
                {
                    HUD().StartCoroutine(HUD().CoFadeFullScreen(UColor.clear, UColor.black));
                    HUD().StartCoroutine(HUD().CoShowIntro());
                }

                if (Meeting())
                {
                    if (GUILayout.Button("End Meeting()"))
                        Meeting().RpcClose();
                }
                else
                {
                    if (GUILayout.Button("Start Meeting()"))
                        CallMeeting(CustomPlayer.Local);
                }

                if (GUILayout.Button("Kill Self"))
                    RpcMurderPlayer(CustomPlayer.Local);

                if (GUILayout.Button("Kill All"))
                    AllPlayers().ForEach(x => RpcMurderPlayer(x));

                if (GUILayout.Button("Revive Self"))
                    CustomPlayer.Local.Revive();

                if (GUILayout.Button("Revive All"))
                    AllPlayers().ForEach(x => x.Revive());

                if (GUILayout.Button("Log Dump"))
                {
                    LogMessage(CustomPlayer.Local.Data.PlayerName);
                    PlayerLayer.LocalLayers.ForEach(LogMessage);
                    LogMessage("Is Dead - " + CustomPlayer.Local.HasDied());
                    LogMessage("Location - " + CustomPlayer.LocalCustom.Position);
                }

                if (GUILayout.Button("Flash"))
                {
                    var r = (byte)URandom.RandomRangeInt(0, 256);
                    var g = (byte)URandom.RandomRangeInt(0, 256);
                    var b = (byte)URandom.RandomRangeInt(0, 256);
                    Flash(new Color32(r, g, b, 255));
                }

                if (GUILayout.Button("Open Cooldowns Menu"))
                    CooldownsWindow.Enabled = true;
            }

            if (CustomPlayer.Local)
            {
                var position = CustomPlayer.LocalCustom.Position;
                GUILayout.Label($"Player Position\nx: {position.x:00.00} y: {position.y:00.00} z: {position.z:00.00}");
            }

            var mouse = Input.mousePosition;
            GUILayout.Label($"Mouse Position\nx: {mouse.x:00.00} y: {mouse.y:00.00} z: {mouse.z:00.00}");
        })
        {
            Enabled = false
        };

        CooldownsWindow = new(new(20, 20, 0, 0), "Cooldown Debugger", () =>
        {
            if (GUILayout.Button("Cancel Cooldowns"))
                CustomPlayer.Local.GetButtons().ForEach(x => x.CooldownTime = 0f);

            if (GUILayout.Button("Click Buttons"))
                CustomPlayer.Local.GetButtons().ForEach(x => x.Clicked());

            if (GUILayout.Button("Cancel Effects"))
                CustomPlayer.Local.GetButtons().ForEach(x => x.ClickedAgain = true);

            if (GUILayout.Button("Reset Full Cooldown"))
                CustomPlayer.Local.GetButtons().ForEach(x => x.StartCooldown());

            if (GUILayout.Button("Reset Fail Cooldown"))
                CustomPlayer.Local.GetButtons().ForEach(x => x.StartCooldown(CooldownType.Fail));

            if (GUILayout.Button("Reset Initial Cooldown"))
                CustomPlayer.Local.GetButtons().ForEach(x => x.StartCooldown(CooldownType.Start));

            if (GUILayout.Button("Reset Meeting() Cooldown"))
                CustomPlayer.Local.GetButtons().ForEach(x => x.StartCooldown(CooldownType.Meeting));

            if (GUILayout.Button("Close Cooldowns Menu"))
                CooldownsWindow.Enabled = false;
        })
        {
            Enabled = false
        };
    }

    public void Update()
    {
        if (NoPlayers() || !IsLocalGame())
        {
            if (TestWindow.Enabled)
                TestWindow.Enabled = false;

            if (CooldownsWindow.Enabled)
                CooldownsWindow.Enabled = false;

            return; // You must ensure you are only playing on local
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            TestWindow.Enabled = !TestWindow.Enabled;

            if (CooldownsWindow.Enabled && !TestWindow.Enabled)
                CooldownsWindow.Enabled = false;

            if (!TestWindow.Enabled && IsLobby())
            {
                MCIUtils.RemoveAllPlayers();
                TownOfUsReworked.MCIActive = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.F2) && IsInGame())
            CooldownsWindow.Enabled = !CooldownsWindow.Enabled;
    }

    public void OnGUI()
    {
        TestWindow.OnGUI();
        CooldownsWindow.OnGUI();
    }

    public void OnDestroy()
    {
        TestWindow.Enabled = false;
        CooldownsWindow.Enabled = false;
    }
}