using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct PlayerMovementSystem : ISystem {

    public BoundsData targetBoundsData;
    private float3 lastRotationTarget;

    void OnCreate(ref SystemState state) {
        state.RequireForUpdate<PlayerInputData>();
        state.RequireForUpdate<BoundsData>();

        Debug.Log("On Create! - Movement");
    }
    void OnDestroy(ref SystemState state) {
        Debug.Log("On Destroy! - Movement");
    }
    void OnUpdate(ref SystemState state) {

        targetBoundsData = SystemAPI.GetSingleton<BoundsData>();

        foreach (PlayerDataAspect aspect in SystemAPI.Query<PlayerDataAspect>().WithAll<PlayerTag>()) {
            if (ValidateMovementInput(ref state, aspect))
                UpdatePosition(ref state, aspect);

            if (ValidateRotationInput(ref state, aspect))
                UpdateRotation(ref state, aspect);
        }
    }

    private bool ValidateMovementInput(ref SystemState state, PlayerDataAspect aspect) {
        if (aspect.input.ValueRO.currentInput.x == 0 && aspect.input.ValueRO.currentInput.y == 0)
            return false;

        return true;
    }
    private bool ValidateRotationInput(ref SystemState state, PlayerDataAspect aspect) {
        if (aspect.input.ValueRO.rotationTarget.x == lastRotationTarget.x &&
            aspect.input.ValueRO.rotationTarget.y == lastRotationTarget.y &&
            aspect.input.ValueRO.rotationTarget.z == lastRotationTarget.z) 
        {
            return false;
        }

        return true;
    }

    private void UpdatePosition(ref SystemState state, PlayerDataAspect aspect) {
        float3 currentPosition = aspect.transform.ValueRW.Position;
        float3 movement = new float3(aspect.input.ValueRO.currentInput.x, aspect.input.ValueRO.currentInput.y, 0.0f);
        movement *= aspect.data.ValueRO.speed * SystemAPI.Time.DeltaTime;

        float3 result = currentPosition + movement;
        if (result.x > targetBoundsData.rightCameraBounds)
            result.x = targetBoundsData.rightCameraBounds;
        else if (result.x < targetBoundsData.leftCameraBounds)
            result.x = targetBoundsData.leftCameraBounds;

        if (result.y > targetBoundsData.topCameraBounds)
            result.y = targetBoundsData.topCameraBounds;
        else if (result.y < targetBoundsData.bottomCameraBounds)
            result.y = targetBoundsData.bottomCameraBounds;

        aspect.transform.ValueRW.Position = result;
    }
    private void UpdateRotation(ref SystemState state, PlayerDataAspect aspect) {
        float3 distance = (aspect.input.ValueRO.rotationTarget - aspect.transform.ValueRO.Position);
        Vector2 direction = new Vector2(distance.x, distance.y);
        direction.Normalize();

        aspect.transform.ValueRW.Rotation = Quaternion.FromToRotation(Vector3.right, direction);
        lastRotationTarget = aspect.input.ValueRO.rotationTarget;
    }
}
