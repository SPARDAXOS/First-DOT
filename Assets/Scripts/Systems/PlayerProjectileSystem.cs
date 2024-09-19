using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct PlayerProjectileSystem : ISystem {

    private PlayerShootingConfig targetPlayerShootingConfig;
    private BoundsData targetBoundsData;

    private Entity playerEntity;
    private RefRO<LocalTransform> playerTransform;
    private bool playerEntityQueried;
    private bool valid;


    void OnCreate(ref SystemState state) {
        state.RequireForUpdate<PlayerProjectileData>();
        state.RequireForUpdate<BoundsData>();

    }
    void OnDestroy(ref SystemState state) {

    }
    void OnUpdate(ref SystemState state) {

        if (!playerEntityQueried)
            QueryPlayerEntity(ref state);

        if (!valid) {
            Debug.LogWarning("PlayerProjectileSystem is invalid!");
            return;
        }
        
        //Make sure this only updates if there are any active projectiles!

        targetPlayerShootingConfig = SystemAPI.GetSingleton<PlayerShootingConfig>();
        playerTransform = SystemAPI.GetComponentRO<LocalTransform>(playerEntity);
        targetBoundsData = SystemAPI.GetSingleton<BoundsData>();

        //Use the buffer method in the other thing too! the shooting i think it was.
        EntityCommandBuffer buffer = new EntityCommandBuffer(state.WorldUpdateAllocator);

        foreach (PlayerProjectileDataAspect aspect in SystemAPI.Query<PlayerProjectileDataAspect>().WithAll<PlayerProjectileTag>()) {
            if (!state.EntityManager.IsEnabled(aspect.entity))
                continue;

            float3 currentPosition = aspect.transform.ValueRW.Position;
            float3 movement = aspect.data.ValueRO.movementDirection * aspect.data.ValueRO.projectileSpeed * SystemAPI.Time.DeltaTime;
            float3 result = currentPosition + movement;

            if (IsOutOfBounds(ref state, ref result))
                buffer.SetEnabled(aspect.entity, false);
            else
                aspect.transform.ValueRW.Position = result;
        }

        buffer.Playback(state.EntityManager);
        Debug.Log("PlayerProjectileSystem Updated!");
    }


    private bool IsOutOfBounds(ref SystemState state, ref float3 position) {

        if (position.x > targetBoundsData.rightCameraBounds)
            return true;
        else if (position.x < targetBoundsData.leftCameraBounds)
            return true;

        if (position.y > targetBoundsData.topCameraBounds)
            return true;
        else if (position.y < targetBoundsData.bottomCameraBounds)
            return true;

        return false;
    }
    private void QueryPlayerEntity(ref SystemState state) {
        if (playerEntityQueried)
            return;

        playerEntity = SystemAPI.GetSingletonEntity<PlayerTag>();
        if (!state.EntityManager.Exists(playerEntity))
            valid = false;
        else
            valid = true;

        playerEntityQueried = true;
    }
}
