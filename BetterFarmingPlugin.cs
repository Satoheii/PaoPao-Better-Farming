using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace Satohei.PaoPao.BetterFarming;

[BepInPlugin("com.satohei.paopao.betterfarming", "Better Farming", "1.0.0")]
public sealed class BetterFarmingPlugin : BasePlugin
{
    private Harmony _harmony = null!;

    public override void Load()
    {
        FarmingCore.Initialize(Config);
        _harmony = new Harmony("com.satohei.paopao.betterfarming");
        _harmony.PatchAll(typeof(BetterFarmingPlugin).Assembly);
        Log.LogInfo("Better Farming 1.0.0 loaded.");
    }

    public override bool Unload()
    {
        _harmony?.UnpatchSelf();
        return true;
    }
}
