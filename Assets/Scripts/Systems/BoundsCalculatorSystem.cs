using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public partial struct BoundsCalculatorSystem : ISystem {

    void OnCreate(ref SystemState state) {
        state.RequireForUpdate<BoundsData>();

    }
    void OnDestroy(ref SystemState state) {

    }
    void OnUpdate(ref SystemState state) {
        var handle = World.DefaultGameObjectInjectionWorld.GetExistingSystem<BoundsCalculatorSystem>();
        World.DefaultGameObjectInjectionWorld.Unmanaged.ResolveSystemStateRef(handle).Enabled = false;

        CalculateCameraBounds(ref state);

    }


    [BurstCompile]
    private void CalculateCameraBounds(ref SystemState state) {

        Camera cameraComp = Camera.main;

        float horizontalCameraExtent = 2.0f * cameraComp.orthographicSize * cameraComp.aspect;
        float verticalCameraExtent = 2.0f * cameraComp.orthographicSize;

        float leftCameraBounds = cameraComp.transform.position.x - horizontalCameraExtent / 2.0f;
        float rightCameraBounds = cameraComp.transform.position.x + horizontalCameraExtent / 2.0f;
        float topCameraBounds = cameraComp.transform.position.y + verticalCameraExtent / 2.0f;
        float bottomCameraBounds = cameraComp.transform.position.y - verticalCameraExtent / 2.0f;

        foreach (RefRW<BoundsData> data in SystemAPI.Query<RefRW<BoundsData>>()) {
            data.ValueRW.horizontalCameraExtent = horizontalCameraExtent;
            data.ValueRW.verticalCameraExtent = verticalCameraExtent;
            
            data.ValueRW.topCameraBounds = topCameraBounds;
            data.ValueRW.bottomCameraBounds = bottomCameraBounds;
            data.ValueRW.rightCameraBounds = rightCameraBounds;
            data.ValueRW.leftCameraBounds = leftCameraBounds;
        }
    }
}
