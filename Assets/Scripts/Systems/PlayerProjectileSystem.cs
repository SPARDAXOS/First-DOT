using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using System;


[BurstCompile]
public partial struct PlayerProjectileSystem : ISystem {

    private PlayerShootingConfig targetPlayerShootingConfig;
    private BoundsData targetBoundsData;


    [BurstCompile]
    void OnCreate(ref SystemState state) {
        state.RequireForUpdate<PlayerProjectileData>();
        state.RequireForUpdate<BoundsData>();

    }

    [BurstCompile]
    void OnDestroy(ref SystemState state) {

    }

    [BurstCompile]
    void OnUpdate(ref SystemState state) {

        targetPlayerShootingConfig = SystemAPI.GetSingleton<PlayerShootingConfig>();
        targetBoundsData = SystemAPI.GetSingleton<BoundsData>();

        EntityCommandBuffer buffer = new EntityCommandBuffer(state.WorldUpdateAllocator);
        var projectileMovementJob = new UpdateProjectileMovementJob {
            boundsData = targetBoundsData,
            deltaTime = SystemAPI.Time.DeltaTime,
            ECB = buffer
        };
        projectileMovementJob.Schedule();

        state.Dependency.Complete();
        buffer.Playback(state.EntityManager);
        buffer.Dispose();

        Debug.Log("PlayerProjectileSystem Updated!");
    }
}


[BurstCompile]
[WithAll(typeof(PlayerProjectileTag))]
public partial struct UpdateProjectileMovementJob : IJobEntity {

    public BoundsData boundsData;
    public float deltaTime;
    public EntityCommandBuffer ECB;

    [BurstCompile]
    private void UpdateProjectileMovement(ref PlayerProjectileDataAspect aspect) {

        float3 currentPosition = aspect.transform.ValueRW.Position;
        float3 movement = aspect.data.ValueRO.movementDirection * aspect.data.ValueRO.projectileSpeed * deltaTime;
        float3 result = currentPosition + movement;

        if (IsOutOfBounds(ref result))
            ECB.SetEnabled(aspect.entity, false);
        else
            aspect.transform.ValueRW.Position = result;
    }

    [BurstCompile]
    private bool IsOutOfBounds(ref float3 position) {

        if (position.x > boundsData.rightCameraBounds)
            return true;
        else if (position.x < boundsData.leftCameraBounds)
            return true;

        if (position.y > boundsData.topCameraBounds)
            return true;
        else if (position.y < boundsData.bottomCameraBounds)
            return true;

        return false;
    }


    [BurstCompile]
    public void Execute(PlayerProjectileDataAspect data) {
        UpdateProjectileMovement(ref data);
    }
}
