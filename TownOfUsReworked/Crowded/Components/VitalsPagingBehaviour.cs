using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Il2CppInterop.Runtime.Attributes;
using HarmonyLib;
//using TMPro;

namespace TownOfUsReworked.Crowded.Components
{
    [HarmonyPatch]
    public class VitalsPagingBehaviour : AbstractPagingBehaviour
    {
        public VitalsPagingBehaviour(IntPtr ptr) : base(ptr) {}

        public VitalsMinigame vitalsMinigame = null!;

        [HideFromIl2Cpp]
        public IEnumerable<VitalsPanel> Targets => vitalsMinigame.vitals.ToArray();
        public override int MaxPageIndex => (Targets.Count() - 1) / MaxPerPage;
        /*public TextMeshPro Text;

        public override void Update()
        {
            base.Update();

            if (Text == null)
                Text = Instantiate(MeetingHud.Instance.TimerText, MeetingHud.Instance.TimerText.transform);

            Text.text = $"Page ({PageIndex + 1}/{MaxPageIndex + 1})";
            Text.gameObject.SetActive(true);
        }*/

        public override void OnPageChanged()
        {
            var i = 0;

            foreach (var panel in Targets)
            {
                if (i >= PageIndex * MaxPerPage && i < (PageIndex + 1) * MaxPerPage)
                {
                    panel.gameObject.SetActive(true);
                    var relativeIndex = i % MaxPerPage;
                    var row = relativeIndex / 3;
                    var buttonTransform = panel.transform;
                    buttonTransform.localPosition = new Vector3(vitalsMinigame.XStart + (vitalsMinigame.XOffset * (relativeIndex % 3)), vitalsMinigame.YStart + (vitalsMinigame.YOffset *
                        row), buttonTransform.position.z);
                }
                else
                    panel.gameObject.SetActive(false);

                i++;
            }
        }

        //public void Close() => Text.gameObject.SetActive(false);
    }
}