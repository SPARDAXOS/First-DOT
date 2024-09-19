using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;





public partial struct PlayerInputSystem : ISystem {


    public struct InputData {
        public float currentHorizontalInput;
        public float currentVerticalInput;
        public float lastHorizontalInput;
        public float lastVerticalInput;

        public Vector3 currentMousePosition;
        public Vector3 lastMousePosition;

        public bool currentLeftMouseClicked;
        public bool lastLeftMouseClicked;
    }

    public InputData currentData;


    void OnCreate(ref SystemState state) {
        state.RequireForUpdate<PlayerInputData>();
    }
    void OnDestroy(ref SystemState state) {

    }
    void OnUpdate(ref SystemState state) {
        UpdateMovementInput(ref state);
        UpdateRotationInput(ref state);
        UpdateActionInput(ref state);
        RunUpdateDataJob(ref state);
    }


    [BurstCompile]
    private void UpdateRotationInput(ref SystemState state) {
        Vector3 position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10.0f));
        currentData.currentMousePosition = position;
    }

    [BurstCompile]
    private void UpdateMovementInput(ref SystemState state) {
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

        currentData.currentHorizontalInput = horizontalInput;
        currentData.currentVerticalInput = verticalInput;
    }

    [BurstCompile]
    private void UpdateActionInput(ref SystemState state) {
        currentData.currentLeftMouseClicked = Input.GetKey(KeyCode.Mouse0);
    }

    [BurstCompile]
    private void RunUpdateDataJob(ref SystemState state) {
        bool condition1 = (currentData.currentHorizontalInput != currentData.lastHorizontalInput || currentData.currentVerticalInput != currentData.lastVerticalInput);
        bool condition2 = (currentData.currentMousePosition != currentData.lastMousePosition);
        bool condition3 = (currentData.currentLeftMouseClicked != currentData.lastLeftMouseClicked);

        if (!condition1 && !condition2 && !condition3)
            return;

        if (condition1) {
            currentData.lastHorizontalInput = currentData.currentHorizontalInput;
            currentData.lastVerticalInput = currentData.currentVerticalInput;
        }
        if (condition2) {
            currentData.lastMousePosition = currentData.currentMousePosition;
        }
        if (condition3) {
            currentData.lastLeftMouseClicked = currentData.currentLeftMouseClicked;
        }

        UpdatePlayerDataJob Update = new UpdatePlayerDataJob { targetDataRef = currentData };
        Update.Schedule();
    }


    [BurstCompile]
    [WithAll(typeof(PlayerTag))]
    public partial struct UpdatePlayerDataJob : IJobEntity {

        public InputData targetDataRef;

        [BurstCompile]
        private void UpdatePlayerData(ref PlayerInputData data) {
            data.currentInput = new Vector2(targetDataRef.currentHorizontalInput, targetDataRef.currentVerticalInput);
            data.rotationTarget = targetDataRef.currentMousePosition;
            data.isShooting = targetDataRef.currentLeftMouseClicked;
            Debug.Log("Input system updated!");
        }

        public void Execute(ref PlayerInputData data) {
            UpdatePlayerData(ref data);
        }
    }
}
