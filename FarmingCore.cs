using System;
using BepInEx.Configuration;
using PawPaw.Game.Systems.Farm.Garden.GardenBedRelated;
using PawPaw.Game.Systems.Farm.Garden.GardenBedRelated.SaveData;
using PawPaw.Game.Units.Common;
using UnityEngine;

namespace Satohei.PaoPao.BetterFarming;

internal static class FarmingCore
{
    internal const bool DefaultEnabled = true;
    internal const int DefaultTillingAnimationCycles = 2;
    internal const int DefaultHarvestAnimationCycles = 2;
    internal const int DefaultClearingAnimationCycles = 2;
    internal const int DefaultCropClearingAnimationCycles = 3;
    internal const float DefaultWaterSpeedMultiplier = 5f;
    internal const int MinimumAnimationCycles = 1;
    internal const int MaximumAnimationCycles = 20;
    internal const float MinimumSpeedMultiplier = 0.1f;
    internal const float MaximumSpeedMultiplier = 20f;
    internal const float AnimationExitBlendNormalizedDuration = 0.125f;

    private static ConfigEntry<bool> Enabled { get; set; } = null!;
    private static ConfigEntry<int> TillingAnimationCycles { get; set; } = null!;
    private static ConfigEntry<int> HarvestAnimationCycles { get; set; } = null!;
    private static ConfigEntry<int> ClearingAnimationCycles { get; set; } = null!;
    private static ConfigEntry<int> CropClearingAnimationCycles { get; set; } = null!;
    private static ConfigEntry<float> WaterSpeedMultiplier { get; set; } = null!;

    internal readonly struct InteractionTimingState
    {
        internal GrowState State { get; }
        internal float OriginalDuration { get; }
        internal bool Applied { get; }

        internal InteractionTimingState(GrowState state, float originalDuration)
        {
            State = state;
            OriginalDuration = originalDuration;
            Applied = true;
        }
    }

    internal static void Initialize(ConfigFile config)
    {
        Enabled = config.Bind("General", "Enabled", DefaultEnabled, "Enable faster farming actions.");
        TillingAnimationCycles = config.Bind("Animation Cycles", "TillingAnimationCycles", DefaultTillingAnimationCycles, "Complete tilling after this many full animation cycles.");
        HarvestAnimationCycles = config.Bind("Animation Cycles", "HarvestAnimationCycles", DefaultHarvestAnimationCycles, "Complete harvesting after this many full animation cycles.");
        ClearingAnimationCycles = config.Bind("Animation Cycles", "ClearingAnimationCycles", DefaultClearingAnimationCycles, "Complete clearing a dead plant after this many full animation cycles.");
        CropClearingAnimationCycles = config.Bind("Animation Cycles", "CropClearingAnimationCycles", DefaultCropClearingAnimationCycles, "Complete clearing a planted crop after this many full digging animation cycles.");
        WaterSpeedMultiplier = config.Bind("Action Speeds", "WaterSpeedMultiplier", DefaultWaterSpeedMultiplier, "Watering speed relative to the original game.");
    }

    internal static InteractionTimingState ApplyInteractionTiming(GardenBed gardenBed, Character character)
    {
        if (!Enabled.Value)
        {
            return default;
        }

        var state = gardenBed.GrowthComponent.CurrentGrowStatus.State;
        if (state == GrowState.NotProcessed && gardenBed._timeToProcessGardenBed > 0f)
        {
            var originalDuration = gardenBed._timeToProcessGardenBed;
            gardenBed._timeToProcessGardenBed = GetAnimationDuration(character, "Dig", TillingAnimationCycles.Value, originalDuration);
            return new InteractionTimingState(state, originalDuration);
        }

        if ((state == GrowState.Seeded || state == GrowState.Growing) && gardenBed._timeToProcessGardenBed > 0f)
        {
            var originalDuration = gardenBed._timeToProcessGardenBed;
            gardenBed._timeToProcessGardenBed = GetAnimationDuration(character, "Dig", CropClearingAnimationCycles.Value, originalDuration);
            return new InteractionTimingState(state, originalDuration);
        }

        if (state == GrowState.Harvestable && gardenBed._timeToCollectHarvest > 0f)
        {
            var originalDuration = gardenBed._timeToCollectHarvest;
            gardenBed._timeToCollectHarvest = GetAnimationDuration(character, "Weeds_clean", HarvestAnimationCycles.Value, originalDuration);
            return new InteractionTimingState(state, originalDuration);
        }

        if (state == GrowState.Dead && gardenBed._timeToClearGardenBed > 0f)
        {
            var originalDuration = gardenBed._timeToClearGardenBed;
            gardenBed._timeToClearGardenBed = GetAnimationDuration(character, "Weeds_clean", ClearingAnimationCycles.Value, originalDuration);
            return new InteractionTimingState(state, originalDuration);
        }

        return default;
    }

    internal static void RestoreInteractionTiming(GardenBed gardenBed, InteractionTimingState timingState)
    {
        if (!timingState.Applied)
        {
            return;
        }

        if (timingState.State == GrowState.NotProcessed || timingState.State == GrowState.Seeded || timingState.State == GrowState.Growing)
        {
            gardenBed._timeToProcessGardenBed = timingState.OriginalDuration;
        }
        else if (timingState.State == GrowState.Harvestable)
        {
            gardenBed._timeToCollectHarvest = timingState.OriginalDuration;
        }
        else if (timingState.State == GrowState.Dead)
        {
            gardenBed._timeToClearGardenBed = timingState.OriginalDuration;
        }
    }

    internal static float ScaleWaterAmount(float waterAmount)
    {
        if (!Enabled.Value || waterAmount <= 0f)
        {
            return waterAmount;
        }

        var multiplier = WaterSpeedMultiplier.Value;
        if (!float.IsFinite(multiplier))
        {
            multiplier = DefaultWaterSpeedMultiplier;
        }

        return waterAmount * ClampSpeed(multiplier);
    }

    private static float GetAnimationDuration(Character character, string clipNamePart, int cycles, float fallbackDuration)
    {
        var controller = character?.Avatar?.Animator?.runtimeAnimatorController;
        if (controller == null)
        {
            return fallbackDuration;
        }

        var clips = controller.animationClips;
        for (var index = 0; index < clips.Length; index++)
        {
            var clip = clips[index];
            if (clip != null && clip.name.IndexOf(clipNamePart, StringComparison.OrdinalIgnoreCase) >= 0 && clip.length > 0f)
            {
                var duration = clip.length * Math.Clamp(cycles, MinimumAnimationCycles, MaximumAnimationCycles);
                var completionLeadTime = (clip.length * AnimationExitBlendNormalizedDuration) + Time.smoothDeltaTime;
                return float.IsFinite(completionLeadTime) && completionLeadTime > 0f && completionLeadTime < duration
                    ? duration - completionLeadTime
                    : duration;
            }
        }

        return fallbackDuration;
    }

    private static float ClampSpeed(float multiplier)
    {
        return Math.Clamp(multiplier, MinimumSpeedMultiplier, MaximumSpeedMultiplier);
    }
}
