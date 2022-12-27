using HarmonyLib;
using InnerNet;
using UnityEngine;

namespace TownOfUsReworked.Patches
{
    //Thanks to Town Of Host for this code
    [HarmonyPatch(typeof(ControllerManager), nameof(ControllerManager.Update))]
    class ControllerManagerUpdatePatch
    {
        static PlayerControl bot = null;
        static readonly (int, int)[] resolutions = { (480, 270), (640, 360), (800, 450), (1280, 720), (1600, 900) };
        static int resolutionIndex = 0;

        public static void Postfix(ControllerManager __instance)
        {
            if (Input.GetKeyDown(KeyCode.F11))
            {
                resolutionIndex++;

                if (resolutionIndex >= resolutions.Length)
                    resolutionIndex = 0;

                ResolutionManager.SetResolution(resolutions[resolutionIndex].Item1, resolutions[resolutionIndex].Item2, false);
            }

            if (AmongUsClient.Instance.AmHost)
            {
                if (Input.GetKeyDown(KeyCode.LeftShift) && GameStates.IsCountDown)
                    GameStartManager.Instance.countDownTimer = 0;

                if (Input.GetKeyDown(KeyCode.C) && GameStates.IsCountDown)
                    GameStartManager.Instance.ResetStartState();
            }

            if (!TownOfUsReworked.isTest)
                return;
            
            if (Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(KeyCode.N))
            {
                if (bot == null)
                {
                    bot = UnityEngine.Object.Instantiate(AmongUsClient.Instance.PlayerPrefab);
                    bot.PlayerId = 15;
                    GameData.Instance.AddPlayer(bot);
                    AmongUsClient.Instance.Spawn(bot, -2, SpawnFlags.None);
                    bot.transform.position = PlayerControl.LocalPlayer.transform.position;
                    bot.NetTransform.enabled = true;
                    GameData.Instance.RpcSetTasks(bot.PlayerId, new byte[0]);
                }

                bot.RpcSetColor((byte)PlayerControl.LocalPlayer.CurrentOutfit.ColorId);
                bot.RpcSetName(PlayerControl.LocalPlayer.name);
                bot.RpcSetPet(PlayerControl.LocalPlayer.CurrentOutfit.PetId);
                bot.RpcSetSkin(PlayerControl.LocalPlayer.CurrentOutfit.SkinId);
                bot.RpcSetNamePlate(PlayerControl.LocalPlayer.CurrentOutfit.NamePlateId);

                // new LateTask(() => bot.NetTransform.RpcSnapTo(new Vector2(0, 15)), 0.2f, "Bot TP Task");
                // new LateTask(() => { foreach (var pc in PlayerControl.AllPlayerControls) pc.RpcMurderPlayer(bot); }, 0.4f, "Bot Kill Task");
                // new LateTask(() => bot.Despawn(), 0.6f, "Bot Despawn Task");
            }
                
            if (Input.GetKeyDown(KeyCode.X))
                PlayerControl.LocalPlayer.Data.Object.SetKillTimer(0f);
            
            if (Input.GetKeyDown(KeyCode.O))
            {
                foreach (var task in PlayerControl.LocalPlayer.myTasks)
                    PlayerControl.LocalPlayer.RpcCompleteTask(task.Id);
            }
            
            if (Input.GetKeyDown(KeyCode.G))
            {
                HudManager.Instance.StartCoroutine(HudManager.Instance.CoFadeFullScreen(Color.clear, Color.black));
                HudManager.Instance.StartCoroutine(DestroyableSingleton<HudManager>.Instance.CoShowIntro());
            }
            
            if (Input.GetKeyDown(KeyCode.P))
            {
                ShipStatus.Instance.RpcRepairSystem(SystemTypes.Doors, 79);
                ShipStatus.Instance.RpcRepairSystem(SystemTypes.Doors, 80);
                ShipStatus.Instance.RpcRepairSystem(SystemTypes.Doors, 81);
                ShipStatus.Instance.RpcRepairSystem(SystemTypes.Doors, 82);
            }
        }
    }
}
