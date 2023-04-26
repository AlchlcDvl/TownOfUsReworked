using System.Collections;
using System.Linq;
using HarmonyLib;
using Reactor.Utilities;
using UnityEngine;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.SwapperMod
{
    [HarmonyPatch]
    public static class SwapVotes
    {
        private static IEnumerator Slide2D(Transform target, Vector2 source, Vector2 dest, float duration)
        {
            var temp = default(Vector3);
            temp.z = target.position.z;

            for (var time = 0f; time < duration; time += Time.deltaTime)
            {
                var t = time / duration;
                temp.x = Mathf.SmoothStep(source.x, dest.x, t);
                temp.y = Mathf.SmoothStep(source.y, dest.y, t);
                target.position = temp;
                yield return null;
            }

            temp.x = dest.x;
            temp.y = dest.y;
            target.position = temp;
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
        public static class VotingComplete
        {
            public static void Postfix()
            {
                foreach (var role in Role.GetRoles<Swapper>(RoleEnum.Swapper))
                {
                    foreach (var button in role.MoarButtons.Where(button => button != null))
                        button.SetActive(false);

                    if (role.IsDead || role.Disconnected || role.Swap1 == null || role.Swap2 == null)
                        continue;

                    PlayerControl swapPlayer1 = null;
                    PlayerControl swapPlayer2 = null;

                    foreach (var player in PlayerControl.AllPlayerControls)
                    {
                        if (player.PlayerId == role.Swap1.TargetPlayerId)
                            swapPlayer1 = player;

                        if (player.PlayerId == role.Swap2.TargetPlayerId)
                            swapPlayer2 = player;
                    }

                    if (swapPlayer1 == null || swapPlayer2 == null || swapPlayer1.Data.IsDead || swapPlayer1.Data.Disconnected || swapPlayer2.Data.IsDead || swapPlayer2.Data.Disconnected)
                        continue;

                    var pool1 = role.Swap1.PlayerIcon.transform;
                    var name1 = role.Swap1.NameText.transform;
                    var background1 = role.Swap1.Background.transform;
                    var mask1 = role.Swap1.MaskArea.transform;
                    var whiteBackground1 = role.Swap1.PlayerButton.transform;
                    var pooldest1 = (Vector2)pool1.position;
                    var namedest1 = (Vector2)name1.position;
                    var backgroundDest1 = (Vector2)background1.position;
                    var whiteBackgroundDest1 = (Vector2)whiteBackground1.position;
                    var maskdest1 = (Vector2)mask1.position;

                    var pool2 = role.Swap2.PlayerIcon.transform;
                    var name2 = role.Swap2.NameText.transform;
                    var background2 = role.Swap2.Background.transform;
                    var mask2 = role.Swap2.MaskArea.transform;
                    var whiteBackground2 = role.Swap2.PlayerButton.transform;

                    var pooldest2 = (Vector2)pool2.position;
                    var namedest2 = (Vector2)name2.position;
                    var backgrounddest2 = (Vector2)background2.position;
                    var maskdest2 = (Vector2)mask2.position;

                    var whiteBackgroundDest2 = (Vector2)whiteBackground2.position;

                    var duration = Mathf.Clamp(2f / (Role.GetRoles(RoleEnum.Swapper).Count == 0 ? 1 : Role.GetRoles(RoleEnum.Swapper).Count), 0.25f, 2f);

                    Coroutines.Start(Slide2D(pool1, pooldest1, pooldest2, duration));
                    Coroutines.Start(Slide2D(pool2, pooldest2, pooldest1, duration));
                    Coroutines.Start(Slide2D(name1, namedest1, namedest2, duration));
                    Coroutines.Start(Slide2D(name2, namedest2, namedest1, duration));
                    Coroutines.Start(Slide2D(mask1, maskdest1, maskdest2, duration));
                    Coroutines.Start(Slide2D(mask2, maskdest2, maskdest1, duration));
                    Coroutines.Start(Slide2D(whiteBackground1, whiteBackgroundDest1, whiteBackgroundDest2, duration));
                    Coroutines.Start(Slide2D(whiteBackground2, whiteBackgroundDest2, whiteBackgroundDest1, duration));
                }
            }
        }
    }
}