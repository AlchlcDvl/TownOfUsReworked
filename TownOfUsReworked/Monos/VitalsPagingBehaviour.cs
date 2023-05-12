namespace TownOfUsReworked.Monos
{
    [HarmonyPatch]
    public class VitalsPagingBehaviour : AbstractPagingBehaviour
    {
        public VitalsPagingBehaviour(IntPtr ptr) : base(ptr) {}

        public VitalsMinigame vitalsMinigame = null!;

        [HideFromIl2Cpp]
        public IEnumerable<VitalsPanel> Targets => vitalsMinigame.vitals.ToArray();
        public override int MaxPageIndex => (Targets.Count() - 1) / MaxPerPage;

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
                    var col = relativeIndex % 3;
                    var buttonTransform = panel.transform;
                    buttonTransform.localPosition = new(vitalsMinigame.XStart + (vitalsMinigame.XOffset * col), vitalsMinigame.YStart + (vitalsMinigame.YOffset * row),
                        buttonTransform.position.z);
                }
                else
                    panel.gameObject.SetActive(false);

                i++;
            }
        }
    }
}