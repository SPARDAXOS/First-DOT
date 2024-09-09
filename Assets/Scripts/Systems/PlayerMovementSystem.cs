using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct PlayerMovementSystem : ISystem {



    void OnCreate(ref SystemState state) {
        state.RequireForUpdate<PlayerMovementData>();

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



        foreach ((RefRO<PlayerMovementData> data, RefRW<LocalTransform> transform) in SystemAPI.Query<RefRO<PlayerMovementData>, RefRW<LocalTransform>>()) {



            transform.ValueRW = transform.ValueRW.Translate(new float3(data.ValueRO.currentInput.x, data.ValueRO.currentInput.y, 0.0f) * data.ValueRO.speed * SystemAPI.Time.DeltaTime);
            Debug.Log("MovementUpdated!");
        }

    }

}
