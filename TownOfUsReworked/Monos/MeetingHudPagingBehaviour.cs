using System;
using System.Collections.Generic;
using System.Linq;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;
using HarmonyLib;
using TownOfUsReworked.Data;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Roles;

namespace TownOfUsReworked.Monos
{
    [HarmonyPatch]
    public class MeetingHudPagingBehaviour : AbstractPagingBehaviour
    {
        public MeetingHudPagingBehaviour(IntPtr ptr) : base(ptr) {}

        public MeetingHud meetingHud = null!;

        [HideFromIl2Cpp]
        public IEnumerable<PlayerVoteArea> Targets => meetingHud.playerStates.OrderBy(p => p.AmDead);
        public override int MaxPageIndex => (Targets.Count() - 1) / MaxPerPage;

        public override void Update()
        {
            base.Update();

            if (meetingHud.state is MeetingHud.VoteStates.Animating or MeetingHud.VoteStates.Proceeding)
                return; //TimerText does not update there

            meetingHud.TimerText.text += $" ({PageIndex + 1}/{MaxPageIndex + 1})";
        }

        public override void OnPageChanged()
        {
            var i = 0;

            foreach (var button in Targets)
            {
                if (i >= PageIndex * MaxPerPage && i < (PageIndex + 1) * MaxPerPage && !Ability.GetAbilities<Assassin>(AbilityEnum.Assassin).Any(x => x.Phone != null) &&
                    !Role.GetRoles<Guesser>(RoleEnum.Guesser).Any(x => x.Phone != null))
                {
                    button.gameObject.SetActive(true);
                    var relativeIndex = i % MaxPerPage;
                    var row = relativeIndex / 3;
                    var col = relativeIndex % 3;
                    var buttonTransform = button.transform;
                    buttonTransform.localPosition = meetingHud.VoteOrigin + new Vector3(meetingHud.VoteButtonOffsets.x * col, meetingHud.VoteButtonOffsets.y * row,
                        buttonTransform.localPosition.z);
                }
                else
                    button.gameObject.SetActive(false);

                i++;
            }
        }
    }
}