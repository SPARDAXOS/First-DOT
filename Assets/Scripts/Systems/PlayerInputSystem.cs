using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public partial struct PlayerInputSystem : ISystem {

    float lastHorizontalInput;
    float lastVerticalInput;


    void OnCreate(ref SystemState state) {
        state.RequireForUpdate<PlayerMovementData>();
        Debug.Log("On Create!");
    }
    void OnDestroy(ref SystemState state) {
        Debug.Log("On Destroy!");
    }
    void OnUpdate(ref SystemState state) {

        CheckMovementInput(ref state);
        CheckRotationInput(ref state);
    }


    private void CheckRotationInput(ref SystemState state) {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10.0f));
        foreach (RefRW<PlayerMovementData> data in SystemAPI.Query<RefRW<PlayerMovementData>>()) {
            data.ValueRW.mousePositionTempDeleteMe = mousePosition;
        }
    }

    private void CheckMovementInput(ref SystemState state) {
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

        if (lastHorizontalInput == horizontalInput && lastVerticalInput == verticalInput)
            return;

        lastHorizontalInput = horizontalInput;
        lastVerticalInput = verticalInput;

        foreach (RefRW<PlayerMovementData> data in SystemAPI.Query<RefRW<PlayerMovementData>>()) {
            data.ValueRW.currentInput = new Vector2(horizontalInput, verticalInput);
        }
    }
}
