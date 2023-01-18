using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.InspectorMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class Update
    {
        private static void UpdateMeeting(MeetingHud __instance, Inspector inspector)
        {
            foreach (var player in inspector.Examined)
            {
                foreach (var state in __instance.playerStates)
                {
                    if (player.PlayerId != state.TargetPlayerId) 
                        continue;

                    var playerName = state.NameText.text;
                    player.nameText().color = new Color32(255, 255, 255, 255);

                    player.nameText().text = playerName;
                }
            }
        }

        [HarmonyPriority(Priority.Last)]
        private static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Inspector))
                return;

            var inspector = Role.GetRole<Inspector>(PlayerControl.LocalPlayer);

            foreach (var player in inspector.Examined)
            {
                player.nameText().transform.localPosition = new Vector3(0f, player.Data.DefaultOutfit.HatId == "hat_NoHat" ? 1.5f : 2.0f, -0.5f);

                var playerName = player.nameText().text;
                player.nameText().color = new Color32(255, 255, 255, 255);
                    
                /*if (player.Is(InspResults.SherConsigInspBm))
                    playerName += $"\n<color=#FFCC80FF>Sher</color> <color=#FFFF99FF>Consig</color> <color=#7E3C64FF>Insp</color> <color=#02A752FF>BM</color>";
                else if (player.Is(InspResults.CrewImpAnMurd))
                    playerName += $"\n<color=#8BFDFDFF>Crew</color> <color=#FF0000FF>Imp</color> <color=#008000FF>Anarchist</color> <color=#6F7BEAFF>Murd</color>";
                else if (player.Is(InspResults.GAExeMedicPup))
                    playerName += $"\n<color=#FFFFFFFF>GA</color> <color=#CCCCCCFF>Exe</color> <color=#A680FFFF>Med</color> <color=#00FFFFFF>Pup</color>";
                else if (player.Is(InspResults.DisgMorphCamoAgent))
                    playerName += $"\n<color=#40B4FFFF>Disg</color> <color=#BB45B0FF>Morph</color> <color=#378AC0FF>Camo</color> <color=#CCA3CCFF>Agent</color>";
                else if (player.Is(InspResults.JestJuggWWInv))
                    playerName += $"\n<color=#F7B3DAFF>Jest</color> <color=#A12B56FF>Jugg</color> <color=#9F703AFF>WW</color> <color=#00B3B3FF>Inv</color>";
                else if (player.Is(InspResults.SurvVHVampVig))
                    playerName += $"\n<color=#DDDD00FF>Surv</color> <color=#C0C0C0FF>VH</color> <color=#7B8968FF>Vamp</color> <color=#FFFF00FF>Vig</color>";
                else if (player.Is(InspResults.GFMayorRebelPest))
                    playerName += $"\n<color=#404C08FF>GF</color> <color=#704FA8FF>Mayo</color> <color=#FFFCCEFF>Rebel</color> <color=#424242FF>Pest</color>";
                else if (player.Is(InspResults.EscConsGliPois))
                    playerName += $"\n<color=#803333FF>Esc</color> <color=#801780FF>Cons</color> <color=#00FF00FF>Gli</color> <color=#B5004CFF>Pois</color>";
                else if (player.Is(InspResults.MineMafiSideDamp))
                    playerName += $"\n<color=#AA7632FF>Mine</color> <color=#6400FFFF>Mafi</color> <color=#979C9FFF>Side</color> <color=#DF7AE8FF>Damp</color>";
                else if (player.Is(InspResults.TransWarpTeleTask))
                    playerName += $"\n<color=#00EEFFFF>Trans</color> <color=#8C7140FF>Warp</color> <color=#6AA84FFF>Tele</color> <color=#ABABFFFF>Task</color>";
                else if (player.Is(InspResults.EngiAmneThiefCann))
                    playerName += $"\n<color=#FFA60AFF>Engi</color> <color=#22FFFFFF>Amne</color> <color=#80FF00FF>Thief</color> <color=#8C4005FF>Can</color>";
                else if (player.Is(InspResults.WraithDetGrenVet))
                    playerName += $"\n<color=#FFB875FF>Wraith</color> <color=#4D4DFFFF>Det</color> <color=#85AA5BFF>Gren</color> <color=#998040FF>Vet</color>";
                else if (player.Is(InspResults.ShiftSwapSKDrac))
                    playerName += $"\n<color=#DF851FFF>Shift</color> <color=#66E666FF>Swap</color> <color=#336EFFFF>SK</color> <color=#AC8A00FF>Drac</color>";
                else if (player.Is(InspResults.CoroJaniUTMed))
                    playerName += $"\n<color=#4D99E6FF>Coro</color> <color=#2647A2FF>Jani</color> <color=#005643FF>UT</color> <color=#A680FFFF>Med</color>";
                else if (player.Is(InspResults.ArsoCryoPBOpTroll))
                    playerName += $"\n<color=#EE7600FF>Arso</color> <color=#642DEAFF>Cryo</color> <color=#CFFE61FF>PB</color> <color=#A7D1B3FF>Op</color>";
                else if (player.Is(InspResults.TrackAltTLTM))
                    playerName += $"\n<color=#009900FF>Track</color> <color=#660000FF>Alt</color> <color=#0000FFFF>TL</color> <color=#0000A7FF>TM</color>";*/

                player.nameText().text = playerName;
            }
        }
    }
}