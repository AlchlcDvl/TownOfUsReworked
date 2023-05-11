using HarmonyLib;
using UnityEngine;
using System.Linq;
using System.Collections;
using Reactor.Utilities;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;
using TownOfUsReworked.PlayerLayers.Roles;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch]
    public static class BlackmailMeetingUpdate
    {
        private static bool ShookAlready;

        #pragma warning disable
        public static Sprite PrevXMark;
        public static Sprite PrevOverlay;
        public const float LetterXOffset = 0.22f;
        public const float LetterYOffset = -0.32f;
        #pragma warning restore

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public static class MeetingHudStart
        {
            public static void Postfix(MeetingHud __instance)
            {
                ShookAlready = false;

                foreach (var role in Role.GetRoles<Blackmailer>(RoleEnum.Blackmailer))
                {
                    if (role.BlackmailedPlayer == null)
                        continue;

                    if (role.BlackmailedPlayer == PlayerControl.LocalPlayer && !role.BlackmailedPlayer.Data.IsDead)
                        Coroutines.Start(BlackmailShhh());

                    if (!role.BlackmailedPlayer.Data.IsDead)
                    {
                        var playerState = __instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == role.BlackmailedPlayer.PlayerId);
                        playerState.XMark.gameObject.SetActive(true);

                        if (PrevXMark == null)
                            PrevXMark = playerState.XMark.sprite;

                        playerState.XMark.sprite = AssetManager.GetSprite("BlackmailLetter");
                        playerState.XMark.transform.localScale *= 0.75f;
                        playerState.XMark.transform.localPosition = new(playerState.XMark.transform.localPosition.x + LetterXOffset, playerState.XMark.transform.localPosition.y +
                            LetterYOffset, playerState.XMark.transform.localPosition.z);
                    }
                }

                foreach (var role in Role.GetRoles<PromotedGodfather>(RoleEnum.PromotedGodfather))
                {
                    if (role.BlackmailedPlayer == null)
                        continue;

                    if (role.BlackmailedPlayer == PlayerControl.LocalPlayer && !role.BlackmailedPlayer.Data.IsDead)
                        Coroutines.Start(BlackmailShhh());

                    if (!role.BlackmailedPlayer.Data.IsDead)
                    {
                        var playerState = __instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == role.BlackmailedPlayer.PlayerId);
                        playerState.XMark.gameObject.SetActive(true);

                        if (PrevXMark == null)
                            PrevXMark = playerState.XMark.sprite;

                        playerState.XMark.sprite = AssetManager.GetSprite("BlackmailLetter");
                        playerState.XMark.transform.localScale *= 0.75f;
                        playerState.XMark.transform.localPosition = new(playerState.XMark.transform.localPosition.x + LetterXOffset, playerState.XMark.transform.localPosition.y +
                            LetterYOffset, playerState.XMark.transform.localPosition.z);
                    }
                }
            }

            public static IEnumerator BlackmailShhh()
            {
                yield return HudManager.Instance.CoFadeFullScreen(Color.clear, new Color(0f, 0f, 0f, 0.98f));
                var TempPosition = HudManager.Instance.shhhEmblem.transform.localPosition;
                var TempDuration = HudManager.Instance.shhhEmblem.HoldDuration;
                HudManager.Instance.shhhEmblem.transform.localPosition = new(HudManager.Instance.shhhEmblem.transform.localPosition.x,
                    HudManager.Instance.shhhEmblem.transform.localPosition.y, HudManager.Instance.FullScreen.transform.position.z + 1f);
                HudManager.Instance.shhhEmblem.TextImage.text = "YOU ARE BLACKMAILED";
                HudManager.Instance.shhhEmblem.HoldDuration = 2.5f;
                yield return HudManager.Instance.ShowEmblem(true);
                HudManager.Instance.shhhEmblem.transform.localPosition = TempPosition;
                HudManager.Instance.shhhEmblem.HoldDuration = TempDuration;
                yield return HudManager.Instance.CoFadeFullScreen(new Color(0f, 0f, 0f, 0.98f), Color.clear);
                yield return null;
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        public static class MeetingHud_Update
        {
            public static void Postfix(MeetingHud __instance)
            {
                foreach (var role in Role.GetRoles<Blackmailer>(RoleEnum.Blackmailer))
                {
                    if (role.BlackmailedPlayer == null)
                        continue;

                    if (!role.BlackmailedPlayer.Data.IsDead)
                    {
                        var playerState = __instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == role.BlackmailedPlayer.PlayerId);
                        playerState.Overlay.gameObject.SetActive(true);

                        if (PrevOverlay == null)
                            PrevOverlay = playerState.Overlay.sprite;

                        playerState.Overlay.sprite = AssetManager.GetSprite("BlackmailOverlay");

                        if (__instance.state != MeetingHud.VoteStates.Animating && !ShookAlready)
                        {
                            ShookAlready = true;
                            __instance.StartCoroutine(Effects.SwayX(playerState.transform));
                        }
                    }
                }

                foreach (var role in Role.GetRoles<PromotedGodfather>(RoleEnum.PromotedGodfather))
                {
                    if (role.BlackmailedPlayer == null)
                        continue;

                    if (!role.BlackmailedPlayer.Data.IsDead)
                    {
                        var playerState = __instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == role.BlackmailedPlayer.PlayerId);
                        playerState.Overlay.gameObject.SetActive(true);

                        if (PrevOverlay == null)
                            PrevOverlay = playerState.Overlay.sprite;

                        playerState.Overlay.sprite = AssetManager.GetSprite("BlackmailOverlay");

                        if (__instance.state != MeetingHud.VoteStates.Animating && !ShookAlready)
                        {
                            ShookAlready = true;
                            __instance.StartCoroutine(Effects.SwayX(playerState.transform));
                        }
                    }
                }
            }
        }
    }
}