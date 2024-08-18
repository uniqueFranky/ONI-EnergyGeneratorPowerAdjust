using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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

        private static void addSlideBar(GameObject go)
        {
            Adjustor ad = go.AddComponent<Adjustor>();
            EnergyGenerator gen = go.GetComponent<EnergyGenerator>();
            ad.originalFormula = new EnergyGenerator.Formula()
            {
                meterTag = gen.formula.meterTag
            };
            if (gen.formula.inputs != null)
            {
                ad.originalFormula.inputs = (EnergyGenerator.InputItem[]) gen.formula.inputs.Clone();
            }

            if (gen.formula.outputs != null)
            {
                ad.originalFormula.outputs = (EnergyGenerator.OutputItem[]) gen.formula.outputs.Clone();
            }
        }        

        [HarmonyPatch]
        public class GeneratorPatch
        {
            static IEnumerable<MethodBase> TargetMethods()
            {
                List<MethodBase> list = new List<MethodBase>();
                // Hydrogen
                list.Add(AccessTools.Method(typeof(HydrogenGeneratorConfig), nameof(HydrogenGeneratorConfig.DoPostConfigureComplete)));
                
                // Methane
                list.Add(AccessTools.Method(typeof(MethaneGeneratorConfig), nameof(MethaneGeneratorConfig.DoPostConfigureComplete)));
                
                // Petroleum
                list.Add(AccessTools.Method(typeof(PetroleumGeneratorConfig), nameof(PetroleumGeneratorConfig.DoPostConfigureComplete)));
                
                // Wood Gas
                list.Add(AccessTools.Method(typeof(WoodGasGeneratorConfig), nameof(WoodGasGeneratorConfig.DoPostConfigureComplete)));
                
                // Coal
                list.Add(AccessTools.Method(typeof(GeneratorConfig), nameof(GeneratorConfig.DoPostConfigureComplete)));
                return list;
            }

            static void Postfix(GameObject go)
            {
                addSlideBar(go);
            }
        }
    }
}