using HarmonyLib;
using KSerialization;
using UnityEngine;

namespace EnergyGeneratorPowerAdjust
{
    public class Patch
    {
        // Strings Needed By SlideBar
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public class StringPatch
        {
            public static void Prefix()
            {
                Strings.Add(Adjustor.TooltipKey, Adjustor.Tooltip);
                Strings.Add(Adjustor.TitleKey, Adjustor.Title);
            }
        }

        // Change the actual emitting power of the energy generator
        [HarmonyPatch(typeof(Generator))]
        [HarmonyPatch(nameof(Generator.WattageRating), MethodType.Getter)]
        public class GeneratorWattageRatingPatch
        {
            public static void Postfix(Generator __instance, ref float __result)
            {
                Adjustor ad = __instance.GetComponent<Adjustor>();
                if (ad != null) // `ad != null` means the generator is equipped with an adjustor
                // Manual Generators can not be adjusted
                {
                    __result = ad.Power;
                    UnityEngine.Debug.Log("set power to " + __result);
                }
            }
        }
        
        // Hydrogen
        [HarmonyPatch(typeof(HydrogenGeneratorConfig))]
        [HarmonyPatch(nameof(HydrogenGeneratorConfig.DoPostConfigureComplete))]
        public class HydrogenPatch
        {
            public static void Postfix(HydrogenGeneratorConfig __instance, GameObject go)
            {
                go.AddComponent<Adjustor>();
            }
        }
    }
}