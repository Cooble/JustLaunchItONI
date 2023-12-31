using HarmonyLib;

namespace ONI_IgnoreAutoLaunchWarning
{
    public class IgnoreAutoLaunchWarningPatch
    {
        /*[HarmonyPatch(typeof(Db))]
        [HarmonyPatch("Initialize")]
        public class Db_Initialize_Patch
        {
            public static void Prefix()
            {
                //Debug.Log("I execute before Db.Initialize!");
            }

            public static void Postfix()
            {
               //Debug.Log("I execute after Db.Initialize!");
            }
        }*/

        [HarmonyPatch(typeof(CraftModuleInterface))]
        [HarmonyPatch("CheckReadyForAutomatedLaunchCommand")]
        public class CraftModuleInterface_CheckReadyForAutomatedLaunchCommand_Patch
        {
            static bool Prefix(ref bool __result, CraftModuleInterface __instance)
            {
                // Original
                // __result = EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketPrep, __instance) == ProcessCondition.Status.Ready
                //         && EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketStorage, __instance) == ProcessCondition.Status.Ready;

                // Patched
                __result = EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketPrep,__instance) != ProcessCondition.Status.Failure 
                           && EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketStorage,__instance) != ProcessCondition.Status.Failure;

                // Return false to skip the original method execution
                return false;
            }
            // Copied from src since this method is private in the original src
            private static ProcessCondition.Status EvaluateConditionSet(
                ProcessCondition.ProcessConditionType conditionType, CraftModuleInterface __instance)
            {
                ProcessCondition.Status conditionSet = ProcessCondition.Status.Ready;
                foreach (ProcessCondition condition1 in __instance.GetConditionSet(conditionType))
                {
                    ProcessCondition.Status condition2 = condition1.EvaluateCondition();
                    if (condition2 < conditionSet)
                        conditionSet = condition2;
                    if (conditionSet == ProcessCondition.Status.Failure)
                        break;
                }
                return conditionSet;
            }
        }

    }
}
