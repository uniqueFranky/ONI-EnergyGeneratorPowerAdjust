using KSerialization;

namespace EnergyGeneratorPowerAdjust
{
    // Power Adjust SlideBar
    public class Adjustor: KMonoBehaviour, IIntSliderControl
    {
        public static string TooltipKey = "STRINGS.UI.UISIDESCREENS.GENERATOR_ADJUSTOR.TOOLTIP";
        public static string Tooltip = "Adjust power of the generator";
        public static string TitleKey = "STRINGS.UI.UISIDESCREENS.GENERATOR_ADJUSTOR.TITLE";
        public static string Title = "Power Adjust";

        [Serialize]
        public float Power { get; set; }

        public EnergyGenerator.Formula originalFormula;
            
        [MyCmpReq]
        private EnergyGenerator energyGenerator;

        float ISliderControl.GetSliderValue(int index) => Power > 0 ? Power : energyGenerator.BaseWattageRating;
        float ISliderControl.GetSliderMin(int index) => energyGenerator.BaseWattageRating / 4;
        float ISliderControl.GetSliderMax(int index) => energyGenerator.BaseWattageRating * 4;
        string ISliderControl.GetSliderTooltip(int index) => $"The generator will generate {STRINGS.UI.PRE_KEYWORD}{Power} Watt{STRINGS.UI.PST_KEYWORD}";
        string ISliderControl.GetSliderTooltipKey(int index) => TooltipKey;

        void ISliderControl.SetSliderValue(float value, int index)
        {
            // set power
            Power = value;
            
            // set input amount
            if (energyGenerator.formula.inputs != null)
            {
                for (int i = 0; i < energyGenerator.formula.inputs.Length; i++)
                {
                    energyGenerator.formula.inputs[i].consumptionRate = originalFormula.inputs[i].consumptionRate * Power /
                                                                        energyGenerator.BaseWattageRating;
                    // FIXME: Here maxStoredMass doesn't work!
                    energyGenerator.formula.inputs[i].maxStoredMass = originalFormula.inputs[i].maxStoredMass * Power /
                                                                      energyGenerator.BaseWattageRating;
                }
            }
            
            // set output amount
            if (energyGenerator.formula.outputs != null)
            {
                for (int i = 0; i < energyGenerator.formula.outputs.Length; i++)
                {
                    energyGenerator.formula.outputs[i].creationRate = originalFormula.outputs[i].creationRate * Power /
                                                                      energyGenerator.BaseWattageRating;
                }
            }
            
            
        }
        int ISliderControl.SliderDecimalPlaces(int index) => 0;
        string ISliderControl.SliderTitleKey => TitleKey;
        string ISliderControl.SliderUnits => "Watt";
    }
}