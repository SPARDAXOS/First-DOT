using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public partial struct PlayerInputSystem : ISystem {





    void OnCreate(ref SystemState state) {
        Debug.Log("On Create!");
    }
    void OnDestroy(ref SystemState state) {
        Debug.Log("On Destroy!");
    }
    void OnUpdate(ref SystemState state) {

        float horizontalInput = 0.0f;
        if (Input.GetKey(KeyCode.A))
            horizontalInput -= 1.0f;
        if (Input.GetKey(KeyCode.D))
            horizontalInput += 1.0f;

        float verticalInput = 0.0f;
        if (Input.GetKey(KeyCode.S)) 
            verticalInput -= 1.0f;
        if (Input.GetKey(KeyCode.W)) 
            verticalInput += 1.0f;


        //foreach (RefRW<LocalTransform> local in SystemAPI.Query<RefRW<LocalTransform>>()) {
        //    float3 position = new float3(horizontalInput, 0.0f, 0.0f);
        //    local.ValueRW = local.ValueRO.Translate(position);
        //}



        foreach (RefRW<PlayerMovementData> data in SystemAPI.Query<RefRW<PlayerMovementData>>()) {
            data.ValueRW.currentInput = new Vector2(horizontalInput, verticalInput);
        }

    }
}
