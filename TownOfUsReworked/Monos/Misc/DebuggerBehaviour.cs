using Reactor.Utilities.ImGui;

namespace TownOfUsReworked.Monos;

// Based off of Reactor.Debugger but merged with MCI and added some functions and changes of my own for testing
public class DebuggerBehaviour : MonoBehaviour
{
    [HideFromIl2Cpp]
    public DragWindow TestWindow { get; }

    [HideFromIl2Cpp]
    public BaseTab[] Tabs { get; } =
    {
        new TestingTab(),
        new GameTab(),
        new CooldownsTab(),
    };

    [HideFromIl2Cpp]
    public BaseTab SelectedTab { get; private set; }

    public byte ControllingFigure { get; set; }

    public static DebuggerBehaviour Instance { get; private set; }

    public DebuggerBehaviour(IntPtr ptr) : base(ptr)
    {
        SelectedTab = Tabs[0];

        Instance = this;

        TestWindow = new(new(20, 20, 0, 0), "Reworked Debugger", () =>
        {
            GUILayout.Label("Name: " + DataManager.Player.Customization.Name);

            if (CustomPlayer.Local)
            {
                var position = CustomPlayer.LocalCustom.Position;
                GUILayout.Label($"Player Position\nx: {position.x:00.00} y: {position.y:00.00} z: {position.z:00.00}");
            }

            var mouse = Input.mousePosition;
            GUILayout.Label($"Mouse Position\nx: {mouse.x:00.00} y: {mouse.y:00.00} z: {mouse.z:00.00}");

            if (!IsLocalGame())
                return;

            GUILayout.BeginHorizontal();

            foreach (var tab in Tabs)
            {
                if (GUILayout.Toggle(tab == SelectedTab, tab.Name, new GUIStyle(GUI.skin.button)))
                    SelectedTab = tab;
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(5f);

            SelectedTab.OnGUI();
        })
        {
            Enabled = false
        };
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            TestWindow.Enabled = !TestWindow.Enabled;

            if (!TestWindow.Enabled && IsLobby() && IsLocalGame())
                MCIUtils.RemoveAllPlayers();
        }
    }

    public void OnGUI() => TestWindow.OnGUI();

    public void OnDestroy() => TestWindow.Enabled = false;
}