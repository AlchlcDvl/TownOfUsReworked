using HarmonyLib;
using TownOfUsReworked.Classes;
using UnityEngine;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Roles.AllRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class GlobalHUD
    {
        private static Sprite Lock = TownOfUsReworked.Lock;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || GameStates.IsLobby)
                return;

            __instance.KillButton.gameObject.SetActive(false);
            __instance.ReportButton.gameObject.SetActive(GameStates.IsRoaming);
            var role = Role.GetRole(PlayerControl.LocalPlayer);

            if (Utils.CanVent(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.Data) && GameStates.IsInGame)
            {
                Sprite Vent;

                if (PlayerControl.LocalPlayer.Is(Faction.Intruder))
                    Vent = TownOfUsReworked.IntruderVent;
                else if (PlayerControl.LocalPlayer.Is(Faction.Syndicate))
                    Vent = TownOfUsReworked.SyndicateVent;
                else if (PlayerControl.LocalPlayer.Is(Faction.Crew))
                    Vent = TownOfUsReworked.CrewVent;
                else if (PlayerControl.LocalPlayer.Is(Faction.Neutral))
                    Vent = TownOfUsReworked.NeutralVent;
                else
                    Vent = __instance.ImpostorVentButton.graphic.sprite;

                __instance.ImpostorVentButton.graphic.sprite = Vent;
            }

            if (MapBehaviour.Instance)
            {
                __instance.ImpostorVentButton.gameObject.SetActive(false);
                __instance.ReportButton.gameObject.SetActive(false);
            }
            else
            {
                __instance.ImpostorVentButton.gameObject.SetActive(GameStates.IsRoaming);
                __instance.ReportButton.gameObject.SetActive(GameStates.IsRoaming);
            }

            if (role != null)
            {
                GameObject[] lockImg = { null, null, null, null, null, null, null, null, null, null, null };

                if (role.IsBlocked)
                {
                    if (__instance.UseButton != null || __instance.PetButton != null)
                    {
                        if (lockImg[0])
                        {
                            lockImg[0] = new GameObject();
                            var lockImgR = lockImg[0].AddComponent<SpriteRenderer>();
                            lockImgR.sprite = Lock;
                        }

                        if (__instance.UseButton != null)
                        {
                            lockImg[0].transform.position = new Vector3(__instance.UseButton.transform.position.x, __instance.UseButton.transform.position.y, -50f);
                            lockImg[0].layer = 5;
                            __instance.UseButton.enabled = false;
                            __instance.UseButton.graphic.color = Palette.DisabledClear;
                            __instance.UseButton.graphic.material.SetFloat("_Desat", 1f);
                        }
                        else
                        {
                            lockImg[0].transform.position =  new Vector3(__instance.PetButton.transform.position.x, __instance.PetButton.transform.position.y, -50f);
                            lockImg[0].layer = 5;
                            __instance.PetButton.enabled = false;
                            __instance.PetButton.graphic.color = Palette.DisabledClear;
                            __instance.PetButton.graphic.material.SetFloat("_Desat", 1f);
                        }
                    }

                    if (__instance.SabotageButton != null)
                    {
                        if (lockImg[1] == null)
                        {
                            lockImg[1] = new GameObject();
                            var lockImgR = lockImg[1].AddComponent<SpriteRenderer>();
                            lockImgR.sprite = Lock;
                        }

                        lockImg[1].layer = 5;
                        lockImg[1].transform.position = new Vector3(__instance.SabotageButton.transform.position.x, __instance.SabotageButton.transform.position.y, -50f);
                        __instance.SabotageButton.enabled = false;
                        __instance.SabotageButton.graphic.color = Palette.DisabledClear;
                        __instance.SabotageButton.graphic.material.SetFloat("_Desat", 1f);
                    }

                    if (__instance.ReportButton != null)
                    {
                        if (lockImg[2] == null)
                        {
                            lockImg[2] = new GameObject();
                            var lockImgR = lockImg[2].AddComponent<SpriteRenderer>();
                            lockImgR.sprite = Lock;
                        }

                        lockImg[2].transform.position = new Vector3(__instance.ReportButton.transform.position.x, __instance.ReportButton.transform.position.y, -50f);
                        lockImg[2].layer = 5;
                        __instance.ReportButton.enabled = false;
                        __instance.ReportButton.graphic.color = Palette.DisabledClear;
                        __instance.ReportButton.graphic.material.SetFloat("_Desat", 1f);
                        __instance.ReportButton.SetActive(false);
                    }

                    int i = 3;

                    if (role.AbilityButtons.Count > 0)
                    {
                        foreach (var button in role.AbilityButtons)
                        {
                            if (lockImg[i] == null)
                            {
                                lockImg[i] = new GameObject();
                                var lockImgR = lockImg[i].AddComponent<SpriteRenderer>();
                                lockImgR.sprite = Lock;
                            }

                            lockImg[i].transform.position = new Vector3(button.transform.position.x, button.transform.position.y, -50f);
                            lockImg[i].layer = 5;
                            button.enabled = false;
                            button.graphic.color = Palette.DisabledClear;
                            button.graphic.material.SetFloat("_Desat", 1f);

                            i++;
                        }
                    }

                    if (MapBehaviour.Instance)
                        MapBehaviour.Instance.Close();

                    if (Minigame.Instance)
                        Minigame.Instance.Close();
                }
                else
                {
                    foreach (var lockThing in lockImg)
                    {
                        if (lockThing != null)
                            lockThing.SetActive(false);
                    }

                    if (__instance.ReportButton != null)
                    {
                        __instance.ReportButton.gameObject.SetActive(GameStates.IsRoaming);
                        __instance.ReportButton.enabled = true;
                    }

                    if (__instance.SabotageButton != null)
                        __instance.SabotageButton.enabled = true;

                    if (__instance.UseButton != null)
                    {
                        __instance.UseButton.enabled = true;
                        __instance.UseButton.graphic.color = Palette.EnabledColor;
                        __instance.UseButton.graphic.material.SetFloat("_Desat", 0f);
                    }
                    else
                    {
                        __instance.PetButton.enabled = true;
                        __instance.PetButton.graphic.color = Palette.EnabledColor;
                        __instance.PetButton.graphic.material.SetFloat("_Desat", 0f);
                    }

                    if (role.AbilityButtons.Count > 0)
                    {
                        foreach (var button in role.AbilityButtons)
                        {
                            button.enabled = true;
                            button.graphic.color = Palette.EnabledColor;
                            button.graphic.material.SetFloat("_Desat", 0f);
                        }
                    }
                }
            }
        }
    }
}