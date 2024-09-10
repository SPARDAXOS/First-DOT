using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct PlayerMovementSystem : ISystem {



    void OnCreate(ref SystemState state) {
        state.RequireForUpdate<PlayerInputData>();

        Debug.Log("On Create! - Movement");
    }
    void OnDestroy(ref SystemState state) {
        Debug.Log("On Destroy! - Movement");
    }
    void OnUpdate(ref SystemState state) {



        //foreach (RefRW<LocalTransform> local in SystemAPI.Query<RefRW<LocalTransform>>()) {
        //    float3 position = new float3(horizontalInput, 0.0f, 0.0f);
        //    local.ValueRW = local.ValueRO.Translate(position);
        //}



        foreach (PlayerMovementAspect aspect in SystemAPI.Query<PlayerMovementAspect>().WithAll<PlayerTag>()) {

            aspect.transform.ValueRW = aspect.transform.ValueRW.Translate(new float3(aspect.input.ValueRO.currentInput.x, aspect.input.ValueRO.currentInput.y, 0.0f) * aspect.data.ValueRO.speed * SystemAPI.Time.DeltaTime);

            float3 distance = (aspect.input.ValueRO.rotationTarget - aspect.transform.ValueRO.Position);
            Vector2 direction = new Vector2(distance.x, distance.y);
            direction.Normalize();

            Vector2 forwardVector = new Vector2(aspect.transform.ValueRO.Right().x, aspect.transform.ValueRO.Right().y);
            //Quaternion result = Quaternion.LookRotation(direction, new Vector3(0.0f, 0.0f, 1));
            //result *= Quaternion.Euler(-90.0f, 0.0f, 0.0f);
            //Quaternion result = Quaternion.Euler(0.0f, 0.0f, Mathf.Cos(direction.x) / Mathf.Sin(direction.y));

            aspect.transform.ValueRW.Rotation = Quaternion.FromToRotation(Vector3.right, direction);
            //transform.ValueRW = transform.ValueRW.RotateZ(Vector2.Angle(direction, forwardVector));

            Debug.Log("MovementUpdated!");
        }

    }

}
