using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Lobby.CustomOption;
using AmongUs.GameOptions;
using System.Linq;
using TownOfUsReworked.Patches;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDEverything
    {
        public static Sprite Revive => TownOfUsReworked.ReviveSprite;
        public static Sprite Bug => TownOfUsReworked.BugSprite;
        public static Sprite Placeholder => TownOfUsReworked.Placeholder;
        public static Sprite Fix => TownOfUsReworked.EngineerFix;
        public static Sprite Arrow => TownOfUsReworked.Arrow;
        public static Sprite Medic => TownOfUsReworked.MedicSprite;
        public static Sprite Examine => TownOfUsReworked.ExamineSprite;
        public static Sprite Interrogate => TownOfUsReworked.SeerSprite;
        public static Sprite Rewind => TownOfUsReworked.Rewind;
        public static Sprite Track => TownOfUsReworked.TrackSprite;
        public static Sprite Alert => TownOfUsReworked.AlertSprite;
        public static Sprite Shoot => TownOfUsReworked.ShootSprite;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Retributionist))
                return;

            var role = Role.GetRole<Retributionist>(PlayerControl.LocalPlayer);

            if (role.RevivedRole == null)
                return;
            
            var copyRole = role.RevivedRole.RoleType;

            if (copyRole == RoleEnum.Altruist)
            {
                var data = PlayerControl.LocalPlayer.Data;
                var isDead = data.IsDead;
                var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
                var maxDistance = GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];
                var flag = (CustomGameOptions.GhostTasksCountToWin || !data.IsDead) && (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) && PlayerControl.LocalPlayer.CanMove;
                var allocs = Physics2D.OverlapCircleAll(truePosition, maxDistance, LayerMask.GetMask(new[] { "Players", "Ghost" }));
                DeadBody closestBody = null;
                var closestDistance = float.MaxValue;
                var allBodies = Object.FindObjectsOfType<DeadBody>();

                if (role.ReviveButton == null)
                {
                    role.ReviveButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.ReviveButton.graphic.enabled = true;
                    role.ReviveButton.graphic.sprite = Revive;
                    role.ReviveButton.gameObject.SetActive(false);
                }

                foreach (var body in allBodies.Where(x => Vector2.Distance(x.TruePosition, truePosition) <= maxDistance))
                {
                    var distance = Vector2.Distance(truePosition, body.TruePosition);

                    if (!(distance < closestDistance))
                        continue;

                    closestBody = body;
                    closestDistance = distance;
                }
                
                role.ReviveButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && !role.ReviveUsed);
                KillButtonTarget.SetTarget(role.ReviveButton, closestBody, role);
                role.ReviveButton.SetCoolDown(0f, 1f);

                var renderer = role.ReviveButton.graphic;

                if (role.CurrentTarget != null)
                {
                    renderer.color = Palette.EnabledColor;
                    renderer.material.SetFloat("_Desat", 0f);
                }
                else
                {
                    renderer.color = Palette.DisabledClear;
                    renderer.material.SetFloat("_Desat", 1f);
                }
            }
            else if (copyRole == RoleEnum.Operative)
            {
                if (role.BugButton == null)
                {
                    role.BugButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.BugButton.graphic.enabled = true;
                    role.BugButton.graphic.sprite = Bug;
                    role.BugButton.gameObject.SetActive(false);
                }

                if (role.BugUsesText == null && role.BugUsesLeft > 0)
                {
                    role.BugUsesText = Object.Instantiate(role.BugButton.cooldownTimerText, role.BugButton.transform);
                    role.BugUsesText.transform.localPosition = new Vector3(role.BugUsesText.transform.localPosition.x + 0.26f, role.BugUsesText.transform.localPosition.y + 0.29f,
                        role.BugUsesText.transform.localPosition.z);
                    role.BugUsesText.transform.localScale = role.BugUsesText.transform.localScale * 0.65f;
                    role.BugUsesText.alignment = TMPro.TextAlignmentOptions.Right;
                    role.BugUsesText.fontStyle = TMPro.FontStyles.Bold;
                    role.BugUsesText.gameObject.SetActive(false);
                }

                if (role.BugUsesText != null)
                    role.BugUsesText.text = $"{role.BugUsesLeft}";

                role.BugButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && role.BugButtonUsable);
                role.BugUsesText.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && role.BugButtonUsable);

                if (role.BugButtonUsable)
                    role.BugButton.SetCoolDown(role.BugTimer(), CustomGameOptions.BugCooldown);

                var renderer = role.BugButton.graphic;
                
                if (!role.BugButton.isCoolingDown && role.BugButtonUsable)
                {
                    renderer.color = Palette.EnabledColor;
                    renderer.material.SetFloat("_Desat", 0f);
                }
                else
                {
                    renderer.color = Palette.DisabledClear;
                    renderer.material.SetFloat("_Desat", 1f);
                }
            }
            else if (copyRole == RoleEnum.Chameleon)
            {
                if (role.SwoopButton == null)
                {
                    role.SwoopButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.SwoopButton.graphic.enabled = true;
                    role.SwoopButton.graphic.sprite = Placeholder;
                    role.SwoopButton.gameObject.SetActive(false);
                }

                role.SwoopButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));

                if (role.IsSwooped)
                    role.SwoopButton.SetCoolDown(role.SwoopTimeRemaining, CustomGameOptions.SwoopDuration);
                else
                    role.SwoopButton.SetCoolDown(role.SwoopTimer(), CustomGameOptions.SwoopCooldown);

                var renderer = role.SwoopButton.graphic;

                if (role.IsSwooped || !role.SwoopButton.isCoolingDown)
                {
                    renderer.color = Palette.EnabledColor;
                    renderer.material.SetFloat("_Desat", 0f);
                }
                else
                {
                    renderer.color = Palette.DisabledClear;
                    renderer.material.SetFloat("_Desat", 1f);
                }
            }
            else if (copyRole == RoleEnum.Engineer)
            {
                if (role.FixButton == null)
                {
                    role.FixButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.FixButton.graphic.enabled = true;
                    role.FixButton.graphic.sprite = Fix;
                    role.FixButton.gameObject.SetActive(false);
                }

                if (role.FixUsesText == null && role.FixUsesLeft > 0)
                {
                    role.FixUsesText = Object.Instantiate(__instance.KillButton.cooldownTimerText, __instance.KillButton.transform);
                    role.FixUsesText.gameObject.SetActive(false);
                    role.FixUsesText.transform.localPosition = new Vector3(role.FixUsesText.transform.localPosition.x + 0.26f, role.FixUsesText.transform.localPosition.y + 0.29f,
                        role.FixUsesText.transform.localPosition.z);
                    role.FixUsesText.transform.localScale = role.FixUsesText.transform.localScale * 0.65f;
                    role.FixUsesText.alignment = TMPro.TextAlignmentOptions.Right;
                    role.FixUsesText.fontStyle = TMPro.FontStyles.Bold;
                }

                if (role.FixUsesText != null)
                    role.FixUsesText.text = $"{role.FixUsesLeft}";
                
                role.FixButton.SetCoolDown(0f, 10f);
                role.FixButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && role.FixButtonUsable);

                if (!ShipStatus.Instance)
                    return;

                var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();

                if (system == null)
                    return;

                var camouflager = Role.GetRoleValue<Camouflager>(RoleEnum.Camouflager);
                var concealer = Role.GetRoleValue<Concealer>(RoleEnum.Concealer);
                var shapeshifter = Role.GetRoleValue<Shapeshifter>(RoleEnum.Shapeshifter);

                var specials = system.specials.ToArray();
                var dummyActive = system.dummy.IsActive;
                var active = specials.Any(s => s.IsActive) || camouflager.Camouflaged || concealer.Concealed || shapeshifter.Shapeshifted;
                var renderer = role.FixButton.graphic;
                
                if (active && !dummyActive && role.FixButtonUsable)
                {
                    renderer.color = Palette.EnabledColor;
                    renderer.material.SetFloat("_Desat", 0f);
                }
                else
                {
                    renderer.color = Palette.DisabledClear;
                    renderer.material.SetFloat("_Desat", 1f);
                }
            }
            else if (copyRole == RoleEnum.Coroner)
            {
                var truePosition = PlayerControl.LocalPlayer.GetTruePosition();

                if (!PlayerControl.LocalPlayer.Data.IsDead)
                {
                    var validBodies = Object.FindObjectsOfType<DeadBody>().Where(x => Murder.KilledPlayers.Any(y => y.PlayerId == x.ParentId &&
                        y.KillTime.AddSeconds(CustomGameOptions.CoronerArrowDuration) > System.DateTime.UtcNow));

                    foreach (var bodyArrow in role.BodyArrows.Keys)
                    {
                        if (!validBodies.Any(x => x.ParentId == bodyArrow))
                            role.DestroyCoronerArrow(bodyArrow);
                    }

                    foreach (var body in validBodies)
                    {
                        if (!role.BodyArrows.ContainsKey(body.ParentId))
                        {
                            var gameObj = new GameObject();
                            var arrow = gameObj.AddComponent<ArrowBehaviour>();
                            gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                            var renderer = gameObj.AddComponent<SpriteRenderer>();
                            renderer.sprite = Arrow;
                            arrow.image = renderer;
                            gameObj.layer = 5;
                            role.BodyArrows.Add(body.ParentId, arrow);
                        }
                        
                        role.BodyArrows.GetValueSafe(body.ParentId).target = body.TruePosition;
                    }
                }
                else
                {
                    if (role.BodyArrows.Count != 0)
                    {
                        role.BodyArrows.Values.DestroyAll();
                        role.BodyArrows.Clear();
                    }
                }
            }
            else if (copyRole == RoleEnum.Medic)
            {
                if (role.ShieldButton == null)
                {
                    role.ShieldButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.ShieldButton.graphic.enabled = true;
                    role.ShieldButton.graphic.sprite = Medic;
                    role.ShieldButton.gameObject.SetActive(false);
                }

                role.ShieldButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && !role.UsedAbility);
                role.ShieldButton.SetCoolDown(0f, 1f);
                Utils.SetTarget(ref role.ClosestPlayer, role.ShieldButton);
                var renderer = role.ShieldButton.graphic;
                
                if (role.ClosestPlayer != null)
                {
                    renderer.color = Palette.EnabledColor;
                    renderer.material.SetFloat("_Desat", 0f);
                }
                else
                {
                    renderer.color = Palette.DisabledClear;
                    renderer.material.SetFloat("_Desat", 1f);
                }
            }
            else if (copyRole == RoleEnum.Detective)
            {
                if (role.ExamineButton == null)
                {
                    role.ExamineButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.ExamineButton.graphic.enabled = true;
                    role.ExamineButton.graphic.sprite = Examine;
                    role.ExamineButton.gameObject.SetActive(false);
                }

                role.ExamineButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
                role.ExamineButton.SetCoolDown(role.ExamineTimer(), CustomGameOptions.ExamineCd);
                Utils.SetTarget(ref role.ClosestPlayer, role.ExamineButton);
                var renderer = role.ExamineButton.graphic;
                
                if (role.ClosestPlayer != null && !role.ExamineButton.isCoolingDown)
                {
                    renderer.color = Palette.EnabledColor;
                    renderer.material.SetFloat("_Desat", 0f);
                }
                else
                {
                    renderer.color = Palette.DisabledClear;
                    renderer.material.SetFloat("_Desat", 1f);
                }
            }
            else if (copyRole == RoleEnum.Inspector)
            {
                if (role.InspectButton == null)
                {
                    role.InspectButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.InspectButton.graphic.enabled = true;
                    role.InspectButton.graphic.sprite = Placeholder;
                    role.InspectButton.gameObject.SetActive(false);
                }

                role.InspectButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
                role.InspectButton.SetCoolDown(role.InspectTimer(), CustomGameOptions.InspectCooldown);
                var notinspected = PlayerControl.AllPlayerControls.ToArray().Where(x => !role.InspectedPlayers.Contains(x.PlayerId)).ToList();
                Utils.SetTarget(ref role.ClosestPlayer, role.InspectButton, notinspected);
                var renderer = role.InspectButton.graphic;
                
                if (role.ClosestPlayer != null && !role.InspectButton.isCoolingDown)
                {
                    renderer.color = Palette.EnabledColor;
                    renderer.material.SetFloat("_Desat", 0f);
                }
                else
                {
                    renderer.color = Palette.DisabledClear;
                    renderer.material.SetFloat("_Desat", 1f);
                }
            }
            else if (copyRole == RoleEnum.Sheriff)
            {
                if (role.InterrogateButton == null)
                {
                    role.InterrogateButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.InterrogateButton.graphic.enabled = true;
                    role.InterrogateButton.graphic.sprite = Interrogate;
                    role.InterrogateButton.gameObject.SetActive(false);
                }

                role.InterrogateButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
                role.InterrogateButton.SetCoolDown(role.InterrogateTimer(), CustomGameOptions.InterrogateCd);
                var notInvestigated = PlayerControl.AllPlayerControls.ToArray().Where(x => !role.Interrogated.Contains(x.PlayerId)).ToList();
                Utils.SetTarget(ref role.ClosestPlayer, role.InterrogateButton, notInvestigated);
                var renderer = role.InterrogateButton.graphic;

                if (role.ClosestPlayer != null && !role.InterrogateButton.isCoolingDown)
                {
                    renderer.color = Palette.EnabledColor;
                    renderer.material.SetFloat("_Desat", 0f);
                }
                else
                {
                    renderer.color = Palette.DisabledClear;
                    renderer.material.SetFloat("_Desat", 1f);
                }
            }
            else if (copyRole == RoleEnum.TimeLord)
            {
                if (role.RewindButton == null)
                {
                    role.RewindButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.RewindButton.graphic.enabled = true;
                    role.RewindButton.graphic.sprite = Rewind;
                    role.RewindButton.gameObject.SetActive(false);
                }

                if (role.RewindUsesText == null && role.RewindUsesLeft > 0)
                {
                    role.RewindUsesText = Object.Instantiate(role.RewindButton.cooldownTimerText, role.RewindButton.transform);
                    role.RewindUsesText.transform.localPosition = new Vector3(role.RewindUsesText.transform.localPosition.x + 0.26f, role.RewindUsesText.transform.localPosition.y + 0.29f,
                        role.RewindUsesText.transform.localPosition.z);
                    role.RewindUsesText.transform.localScale = role.RewindUsesText.transform.localScale * 0.65f;
                    role.RewindUsesText.alignment = TMPro.TextAlignmentOptions.Right;
                    role.RewindUsesText.fontStyle = TMPro.FontStyles.Bold;
                    role.RewindUsesText.gameObject.SetActive(false);
                }

                if (role.RewindUsesText != null)
                    role.RewindUsesText.text = $"{role.RewindUsesLeft}";

                role.RewindUsesText.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && role.RewindButtonUsable);
                role.RewindButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && role.RewindButtonUsable);

                if (role.RewindButtonUsable)
                    role.RewindButton.SetCoolDown(role.TimeLordRewindTimer(), role.GetCooldown());

                var renderer = role.RewindButton.graphic;
                
                if (!role.RewindButton.isCoolingDown && !RecordRewind.rewinding && role.RewindButtonUsable)
                {
                    renderer.color = Palette.EnabledColor;
                    renderer.material.SetFloat("_Desat", 0f);
                    role.RewindUsesText.color = Palette.EnabledColor;
                    role.RewindUsesText.material.SetFloat("_Desat", 0f);
                }
                else
                {
                    renderer.color = Palette.DisabledClear;
                    renderer.material.SetFloat("_Desat", 1f);
                    role.RewindUsesText.color = Palette.DisabledClear;
                    role.RewindUsesText.material.SetFloat("_Desat", 0f);
                }
            }
            else if (copyRole == RoleEnum.Tracker)
            {
                if (role.TrackButton == null)
                {
                    role.TrackButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.TrackButton.graphic.enabled = true;
                    role.TrackButton.graphic.sprite = Track;
                    role.TrackButton.gameObject.SetActive(false);
                }

                if (role.TrackUsesText == null && role.TrackUsesLeft > 0)
                {
                    role.TrackUsesText = Object.Instantiate(role.TrackButton.cooldownTimerText, role.TrackButton.transform);
                    role.TrackUsesText.transform.localPosition = new Vector3(role.TrackUsesText.transform.localPosition.x + 0.26f, role.TrackUsesText.transform.localPosition.y + 0.29f,
                        role.TrackUsesText.transform.localPosition.z);
                    role.TrackUsesText.transform.localScale = role.TrackUsesText.transform.localScale * 0.65f;
                    role.TrackUsesText.alignment = TMPro.TextAlignmentOptions.Right;
                    role.TrackUsesText.fontStyle = TMPro.FontStyles.Bold;
                    role.TrackUsesText.gameObject.SetActive(false);
                }

                if (role.TrackUsesText != null)
                    role.TrackUsesText.text = $"{role.TrackUsesLeft}";

                role.TrackButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && role.TrackButtonUsable);
                role.TrackUsesText.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && role.TrackButtonUsable);
                role.TrackButton.SetCoolDown(role.TrackerTimer(), CustomGameOptions.TrackCd);
                var notTracked = PlayerControl.AllPlayerControls.ToArray().Where(x => !role.IsTracking(x)).ToList();
                Utils.SetTarget(ref role.ClosestPlayer, role.TrackButton, notTracked);
                var renderer = role.TrackButton.graphic;
                
                if (role.ClosestPlayer != null && role.TrackButtonUsable && !role.TrackButton.isCoolingDown)
                {
                    renderer.color = Palette.EnabledColor;
                    renderer.material.SetFloat("_Desat", 0f);
                    role.TrackUsesText.color = Palette.EnabledColor;
                    role.TrackUsesText.material.SetFloat("_Desat", 0f);
                }
                else
                {
                    renderer.color = Palette.DisabledClear;
                    renderer.material.SetFloat("_Desat", 1f);
                    role.TrackUsesText.color = Palette.DisabledClear;
                    role.TrackUsesText.material.SetFloat("_Desat", 1f);
                }
            }
            else if (copyRole == RoleEnum.VampireHunter)
            {
                if (role.StakeButton == null)
                {
                    role.StakeButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.StakeButton.graphic.enabled = true;
                    role.StakeButton.graphic.sprite = Placeholder;
                    role.StakeButton.gameObject.SetActive(false);
                }

                role.StakeButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && !role.VampsDead);
                role.StakeButton.SetCoolDown(role.StakeTimer(), CustomGameOptions.StakeCooldown);
                Utils.SetTarget(ref role.ClosestPlayer, role.StakeButton);
                var renderer = role.StakeButton.graphic;
                
                if (role.ClosestPlayer != null && !role.StakeButton.isCoolingDown)
                {
                    renderer.color = Palette.EnabledColor;
                    renderer.material.SetFloat("_Desat", 0f);
                }
                else
                {
                    renderer.color = Palette.DisabledClear;
                    renderer.material.SetFloat("_Desat", 1f);
                }
            }
            else if (copyRole == RoleEnum.Veteran)
            {
                if (role.AlertButton == null)
                {
                    role.AlertButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.AlertButton.graphic.enabled = true;
                    role.AlertButton.graphic.sprite = Alert;
                    role.AlertButton.gameObject.SetActive(false);
                }

                if (role.AlertUsesText == null && role.AlertUsesLeft > 0)
                {
                    role.AlertUsesText = Object.Instantiate(role.AlertButton.cooldownTimerText, role.AlertButton.transform);
                    role.AlertUsesText.transform.localPosition = new Vector3(role.AlertUsesText.transform.localPosition.x + 0.26f, role.AlertUsesText.transform.localPosition.y + 0.29f,
                        role.AlertUsesText.transform.localPosition.z);
                    role.AlertUsesText.transform.localScale = role.AlertUsesText.transform.localScale * 0.65f;
                    role.AlertUsesText.alignment = TMPro.TextAlignmentOptions.Right;
                    role.AlertUsesText.fontStyle = TMPro.FontStyles.Bold;
                    role.AlertUsesText.gameObject.SetActive(false);
                }

                if (role.AlertUsesText != null)
                    role.AlertUsesText.text = $"{role.AlertUsesLeft}";

                role.AlertButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && role.AlertButtonUsable);
                role.AlertUsesText.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && role.AlertButtonUsable);

                if (role.AlertButtonUsable)
                {
                    if (role.AlertEnabled)
                        role.AlertButton.SetCoolDown(role.AlertTimeRemaining, CustomGameOptions.AlertDuration);
                    else
                        role.AlertButton.SetCoolDown(role.AlertTimer(), CustomGameOptions.AlertCd);
                }

                var renderer = role.AlertButton.graphic;
                
                if (!role.AlertButton.isCoolingDown && role.AlertButtonUsable && !role.OnAlert)
                {
                    renderer.color = Palette.EnabledColor;
                    renderer.material.SetFloat("_Desat", 0f);
                    role.AlertUsesText.color = Palette.EnabledColor;
                    role.AlertUsesText.material.SetFloat("_Desat", 0f);
                }
                else
                {
                    renderer.color = Palette.DisabledClear;
                    renderer.material.SetFloat("_Desat", 1f);
                    role.AlertUsesText.color = Palette.DisabledClear;
                    role.AlertUsesText.material.SetFloat("_Desat", 1f);
                }
            }
            else if (copyRole == RoleEnum.Vigilante)
            {
                if (role.ShootButton == null)
                {
                    role.ShootButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.ShootButton.graphic.enabled = true;
                    role.ShootButton.graphic.sprite = Shoot;
                    role.ShootButton.gameObject.SetActive(false);
                }

                role.ShootButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
                role.ShootButton.SetCoolDown(role.KillTimer(), CustomGameOptions.VigiKillCd);
                Utils.SetTarget(ref role.ClosestPlayer, role.ShootButton);
                var renderer = role.ShootButton.graphic;
                
                if (role.ClosestPlayer != null && !role.ShootButton.isCoolingDown)
                {
                    renderer.color = Palette.EnabledColor;
                    renderer.material.SetFloat("_Desat", 0f);
                }
                else
                {
                    renderer.color = Palette.DisabledClear;
                    renderer.material.SetFloat("_Desat", 1f);
                }
            }
        }
    }
}