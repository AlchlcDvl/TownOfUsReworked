using System;
using AmongUs.Data;
using Reactor.Utilities.ImGui;
using UnityEngine;
using TownOfUsReworked.Data;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.Classes;
using Il2CppInterop.Runtime.Attributes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.MultiClientInstancing
{
    public class Debugger : MonoBehaviour
    {
        [HideFromIl2Cpp]
        public DragWindow TestWindow { get; }
        private static int ControllingFigure;

        public Debugger(IntPtr ptr) : base(ptr)
        {
            TestWindow = new DragWindow(new Rect(20, 20, 0, 0), "MCI Debugger", () =>
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
                        MCIUtils.RemovePlayer((byte)InstanceControl.Clients.Count);

                        if (InstanceControl.Clients.Count == 0)
                            TownOfUsReworked.MCIActive = false;
                    }

                    if (GUILayout.Button("Remove All Bots"))
                    {
                        MCIUtils.RemoveAllPlayers();
                        TownOfUsReworked.MCIActive = false;
                    }
                }
                else
                {
                    Role.SyndicateHasChaosDrive = GUILayout.Toggle(Role.SyndicateHasChaosDrive, "Enable Chaos Drive");

                    if (Role.SyndicateHasChaosDrive)
                        RoleGen.AssignChaosDrive();
                    else
                        Role.DriveHolder = null;

                    if (TownOfUsReworked.MCIActive)
                    {
                        if (GUILayout.Button("Next Player"))
                        {
                            ControllingFigure++;

                            if (ControllingFigure == PlayerControl.AllPlayerControls.Count)
                                ControllingFigure = 0;

                            InstanceControl.SwitchTo((byte)ControllingFigure);
                        }
                        else if (GUILayout.Button("Previous Player"))
                        {
                            ControllingFigure--;

                            if (ControllingFigure < 0)
                                ControllingFigure = PlayerControl.AllPlayerControls.Count - 1;

                            InstanceControl.SwitchTo((byte)ControllingFigure);
                        }
                    }

                    if (GUILayout.Button("End Game"))
                    {
                        Role.NobodyWins = true;
                        Objectifier.NobodyWins = true;
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
                    {
                        foreach (var task in PlayerControl.LocalPlayer.myTasks)
                            PlayerControl.LocalPlayer.RpcCompleteTask(task.Id);
                    }

                    if (GUILayout.Button("Redo Intro Sequence"))
                    {
                        HudManager.Instance.StartCoroutine(HudManager.Instance.CoFadeFullScreen(Color.clear, Color.black));
                        HudManager.Instance.StartCoroutine(HudManager.Instance.CoShowIntro());
                    }

                    if (GUILayout.Button("Start Meeting") && !MeetingHud.Instance)
                        PlayerControl.LocalPlayer.CmdReportDeadBody(null);

                    if (GUILayout.Button("End Meeting") && MeetingHud.Instance)
                    {
                        foreach (var player in PlayerControl.AllPlayerControls)
                            MeetingHud.Instance.CmdCastVote(player.PlayerId, player.PlayerId);
                    }

                    if (GUILayout.Button("Kill Self"))
                        Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);

                    if (GUILayout.Button("Kill All"))
                    {
                        foreach (var player in PlayerControl.AllPlayerControls)
                            Utils.RpcMurderPlayer(player, player);
                    }

                    if (GUILayout.Button("Revive Self"))
                        Utils.Revive(PlayerControl.LocalPlayer);

                    if (GUILayout.Button("Revive All"))
                    {
                        foreach (var player in PlayerControl.AllPlayerControls)
                        {
                            if (player.Data.IsDead)
                                Utils.Revive(player);
                        }
                    }

                    if (PlayerControl.LocalPlayer)
                    {
                        var position = PlayerControl.LocalPlayer.transform.position;
                        GUILayout.Label($"x: {position.x}");
                        GUILayout.Label($"y: {position.y}");
                        GUILayout.Label($"z: {position.z}");
                    }
                }
            })
            {
                Enabled = false,
            };
        }

        #pragma warning disable
        private void Update() {}

        private void OnGUI() => TestWindow.OnGUI();
        #pragma warning restore
    }
}