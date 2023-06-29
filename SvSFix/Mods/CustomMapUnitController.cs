using UnityEngine;

namespace SvSFix.Mods;

public class CustomMapUnitController : MapUnitCollisionCharacterControllerComponent
{
    private void Update()
    {
        if (collision_ == null || character_controller_ == null || collision_.IsCollisionInvalid()) return;
        if (!collision_.IsStandGroundHeight()) collision_.ExtrusionAdd();
        var origin_position = Vector3.zero;
        var now_position = Vector3.zero;
        var ground_height = 0f;
        if (collision_.UpdatePrevious(ref origin_position, ref now_position, out ground_height,
                GameTime.ScaledDeltaTime))
        {
            collision_.GravityGroundCapsule(ref now_position, ref ground_height, GameTime.ScaledDeltaTime);
            if ((collision_.bit_mode_ & MapUnitCollision.BitMode.COLLISION_EXTRUSION) != 0)
                character_controller_.Move(now_position - origin_position);
            else
                collision_.unit_base_.transform.position = now_position;
            collision_.UpdateAfter();
            collision_.collider_list_collision_stay_.Clear();
        }
    }

    private void FixedUpdate()
    {
    }
}

public class CustomRigidBodyController : MapUnitCollisionRigidbodyComponent
{
    private void Update()
    {
        if (collision_ == null || rigidbody_component_ == null || collision_.IsCollisionInvalid()) return;
        collision_.ExtrusionAdd();
        var zero = Vector3.zero;
        var zero2 = Vector3.zero;
        var num = 0f;
        if (!collision_.UpdatePrevious(ref zero, ref zero2, out num, GameTime.ScaledDeltaTime)) return;
        collision_.GravityGroundRigidBody(ref zero2, ref num, GameTime.ScaledDeltaTime);
        if ((collision_.bit_mode_ & MapUnitCollision.BitMode.COLLISION_EXTRUSION) !=
            MapUnitCollision.BitMode.SET_COLLISION_NONE)
        {
            rigidbody_component_.velocity = Vector3.zero;
            rigidbody_component_.angularVelocity = Vector3.zero;
            rigidbody_component_.MovePosition(zero2);
        }
        else
        {
            collision_.unit_base_.transform.position = zero2;
        }

        collision_.UpdateAfter();
    }

    private void FixedUpdate()
    {
    }
}