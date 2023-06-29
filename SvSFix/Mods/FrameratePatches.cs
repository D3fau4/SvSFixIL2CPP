using HarmonyLib;
using UnityEngine;

namespace SvSFix.Mods;

[HarmonyPatch]
public class FrameratePatches
{
    [HarmonyPatch(typeof(GameFrame), nameof(GameFrame.SetGameSceneFrameRateTarget), typeof(GameScene))]
    [HarmonyPrefix]
    public static bool ModifyFramerateTarget()
    {
        Application.targetFrameRate =
            0; // Disables the 60FPS limiter that takes place when VSync is disabled. We will be using our own framerate limiting logic anyways.
        QualitySettings.vSyncCount = 1;
        GameFrame.now_target_frame_ = 0;
        GameTime.TargetFrameRate = 0;
        return false;
    }

    [HarmonyPatch(typeof(MapUnitCollisionCharacterControllerComponent), "FixedUpdate")]
    [HarmonyPatch(typeof(MapUnitCollisionRigidbodyComponent), "FixedUpdate")]
    [HarmonyPrefix]
    public static bool NullifyFixedUpdate()
    {
        return
            false; // We are simply going to tell FixedUpdate to fuck off, and then reimplement everything in an Update method.
    }

    [HarmonyPatch(typeof(MapUnitCollisionCharacterControllerComponent),
        nameof(MapUnitCollisionCharacterControllerComponent.Setup), typeof(GameObject), typeof(float), typeof(float),
        typeof(MapUnitBaseComponent))]
    [HarmonyPostfix]
    public static void ReplaceWithCustomCharacterControllerComponent()
    {
        Plugin.Instance.Log.LogInfo("Hooked!");
        // This may or may not work properly. Normally I'd get the instance per HarmonyX's documentation, but that doesn't work here for some arbitrary reason.
        var c = Object.FindObjectsOfType<MapUnitCollisionCharacterControllerComponent>();
        Plugin.Instance.Log.LogInfo("Found " + c[0].name + " possessing a CharacterController component.");
        var newMuc = c[0].transform.gameObject.AddComponent<CustomMapUnitController>();
        var ogMuc =
            c[0].gameObject.GetComponent<MapUnitCollisionCharacterControllerComponent>();
        if (ogMuc != null)
        {
            if (newMuc != null)
            {
                // Copies the properties of the original component before we opt out of using it, and use our own.
                newMuc.character_controller_ = ogMuc.character_controller_;
                newMuc.collision_ = ogMuc.collision_;
                newMuc.rigid_body_ = ogMuc.rigid_body_;
                newMuc.character_controller_unit_radius_scale_ = ogMuc.character_controller_unit_radius_scale_;
                ogMuc.enabled = false; // Would probably be better if we just disabled the original component.
            }
            else
            {
                Plugin.Instance.Log.LogError("New Character Controller Component returned null.");
            }
        }
        else
        {
            Plugin.Instance.Log.LogError("Original Character Controller Component returned null.");
        }
    }

    [HarmonyPatch(typeof(MapUnitCollisionRigidbodyComponent), nameof(MapUnitCollisionRigidbodyComponent.Setup),
        typeof(GameObject), typeof(float), typeof(float), typeof(MapUnitBaseComponent))]
    [HarmonyPostfix]
    public static void ReplaceWithCustomRigidBodyComponent()
    {
        Plugin.Instance.Log.LogInfo("Hooked!");
        // This may or may not work properly. Normally I'd get the instance per HarmonyX's documentation, but that doesn't work here for some arbitrary reason.
        var c = Object.FindObjectsOfType<MapUnitCollisionRigidbodyComponent>();
        Plugin.Instance.Log.LogInfo("Found " + c[0].name + " possessing a RigidBodyController component.");
        var newRbc = c[0].transform.gameObject.AddComponent<CustomRigidBodyController>();
        var ogRbc =
            c[0].transform.gameObject.GetComponent<MapUnitCollisionRigidbodyComponent>();
        if (ogRbc != null)
        {
            if (newRbc != null)
            {
                // Copies the properties of the original component before we opt out of using it, and use our own.
                newRbc.collision_ = ogRbc.collision_;
                newRbc.character_controller_unit_radius_scale_ = ogRbc.character_controller_unit_radius_scale_;
                newRbc.extrusion_speed_ = ogRbc.extrusion_speed_;
                newRbc.hit_extrusion_count_ = ogRbc.hit_extrusion_count_;
                newRbc.hit_extrusion_move_vector_power_ = ogRbc.hit_extrusion_move_vector_power_;
                newRbc.hit_extrusion_vector_ = ogRbc.hit_extrusion_vector_;
                newRbc.rigidbody_component_ = ogRbc.rigidbody_component_;
                ogRbc.enabled = false; // Would probably be better if we just disabled the original component.
            }
            else
            {
                Plugin.Instance.Log.LogError("New Rigid Body Component returned null.");
            }
        }
        else
        {
            Plugin.Instance.Log.LogError("Original Rigid Body Component returned null.");
        }
    }
}