using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using SvSFix.Mods;

namespace SvSFix;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("neptunia-sisters-vs-sisters.exe")]
public class Plugin : BasePlugin
{
    public static Plugin Instance { get; private set; }

    public override void Load()
    {
        Instance = this;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        Log.LogInfo($"Inyectando Clases...");
        ClassInjector.RegisterTypeInIl2Cpp<CustomMapUnitController>();
        ClassInjector.RegisterTypeInIl2Cpp<CustomRigidBodyController>();
        Log.LogInfo($"Done!");

        Harmony.CreateAndPatchAll(typeof(FrameratePatches));
    }

    public override bool Unload()
    {
        return base.Unload();
    }
}