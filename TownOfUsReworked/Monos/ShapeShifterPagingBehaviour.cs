namespace TownOfUsReworked.Monos
{
    public class ShapeShifterPagingBehaviour : AbstractPagingBehaviour
    {
        public ShapeShifterPagingBehaviour(IntPtr ptr) : base(ptr) {}

        public ShapeshifterMinigame shapeshifterMinigame = null!;

        [HideFromIl2Cpp]
        public IEnumerable<ShapeshifterPanel> Targets => shapeshifterMinigame.potentialVictims.ToArray();
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
                    buttonTransform.localPosition = new(shapeshifterMinigame.XStart + (shapeshifterMinigame.XOffset * col), shapeshifterMinigame.YStart + (row *
                        shapeshifterMinigame.YOffset), buttonTransform.localPosition.z);
                }
                else
                    panel.gameObject.SetActive(false);

                i++;
            }
        }
    }
}