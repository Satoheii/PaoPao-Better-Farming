using System;
using HarmonyLib;
using PawPaw.Game.Systems.Farm.Garden.GardenBedRelated;
using PawPaw.Game.Systems.Farm.Garden.GardenBedRelated.Components;
using PawPaw.Game.Units.Common;

namespace Satohei.PaoPao.BetterFarming;

[HarmonyPatch]
internal static class FarmingPatches
{
    [HarmonyPatch(typeof(GardenBed), nameof(GardenBed.ToggleInteractionPrompt))]
    [HarmonyPrefix]
    private static void ApplyInteractionTiming(GardenBed __instance, Character character, out FarmingCore.InteractionTimingState __state)
    {
        __state = FarmingCore.ApplyInteractionTiming(__instance, character);
    }

    [HarmonyPatch(typeof(GardenBed), nameof(GardenBed.ToggleInteractionPrompt))]
    [HarmonyFinalizer]
    private static Exception RestoreInteractionTiming(GardenBed __instance, FarmingCore.InteractionTimingState __state, Exception __exception)
    {
        FarmingCore.RestoreInteractionTiming(__instance, __state);
        return __exception;
    }

    [HarmonyPatch(typeof(GardenBedWateringComponent), nameof(GardenBedWateringComponent.Water))]
    [HarmonyPrefix]
    private static void ScaleWaterAmount(ref float waterAmount)
    {
        waterAmount = FarmingCore.ScaleWaterAmount(waterAmount);
    }
}
