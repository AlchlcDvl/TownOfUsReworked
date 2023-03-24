using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.MCI
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public sealed class MCIControls
    {
        private static int ControllingFigure;

        public static void Postfix()
        {
            if (!GameStates.IsLocalGame)
                return; //You must ensure you are only playing on local

            if (Input.GetKeyDown(KeyCode.F1))
            {
                if (!GameStates.IsLobby)
                    return; //Don't try to add bots when you are playtesting

                ControllingFigure = PlayerControl.LocalPlayer.PlayerId;

                if (PlayerControl.AllPlayerControls.Count >= 15 && TownOfUsReworked.LobbyCapped)
                    return; //Toggle lobby limit

                MCIUtils.CleanUpLoad();
                MCIUtils.CreatePlayerInstance();

                if (!TownOfUsReworked.MCIActive)
                    TownOfUsReworked.MCIActive = true;
            }

            if (!TownOfUsReworked.MCIActive)
                return;

            if (Input.GetKeyDown(KeyCode.F2))
            {
                ControllingFigure++;

                if (ControllingFigure >= PlayerControl.AllPlayerControls.Count)
                    ControllingFigure = 0;

                InstanceControl.SwitchTo((byte)ControllingFigure);
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                ControllingFigure--;

                if (ControllingFigure < 0)
                    ControllingFigure = PlayerControl.AllPlayerControls.Count - 1;

                InstanceControl.SwitchTo((byte)ControllingFigure);
            }

            if (Input.GetKeyDown(KeyCode.F4))
            {
                if (GameStates.IsLobby)
                    return;

                Role.NobodyWins = true;
                Objectifier.NobodyWins = true;
                Utils.EndGame();
            }

            if (Input.GetKeyDown(KeyCode.F5))
            {
                if (GameStates.IsLobby)
                    return;

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

            if (Input.GetKeyDown(KeyCode.F6))
            {
                if (GameStates.IsLobby)
                    return;

                foreach (var task in PlayerControl.LocalPlayer.myTasks)
                    PlayerControl.LocalPlayer.RpcCompleteTask(task.Id);
            }

            if (Input.GetKeyDown(KeyCode.F7))
            {
                if (GameStates.IsLobby)
                    return;

                HudManager.Instance.StartCoroutine(HudManager.Instance.CoFadeFullScreen(Color.clear, Color.black));
                HudManager.Instance.StartCoroutine(HudManager.Instance.CoShowIntro());
            }

            if (Input.GetKeyDown(KeyCode.F8))
            {
                if (!GameStates.IsLobby)
                    return;

                MCIUtils.RemoveAllPlayers();
            }

            if (Input.GetKeyDown(KeyCode.F9))
            {
                if (!GameStates.IsLobby)
                    return;

                TownOfUsReworked.LobbyCapped = !TownOfUsReworked.LobbyCapped;
                HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"The lobby limit is now {(TownOfUsReworked.LobbyCapped ? "" : "un")}capped.");
            }

            if (Input.GetKeyDown(KeyCode.F10))
            {
                if (!GameStates.IsLobby)
                    return;

                MCIUtils.RemovePlayer((byte)InstanceControl.Clients.Count);
            }

            if (Input.GetKeyDown(KeyCode.F11))
            {
                if (!GameStates.IsLobby)
                    return;

                TownOfUsReworked.Persistence = !TownOfUsReworked.Persistence;
                HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"The robots will now{(TownOfUsReworked.Persistence ? "" : " no longer")} persist.");
            }
        }
    }
}