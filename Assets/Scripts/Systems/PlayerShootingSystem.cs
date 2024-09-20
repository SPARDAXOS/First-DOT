using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;


[BurstCompile]
public partial class PlayerShootingSystem : SystemBase {

    private NativeArray<Entity> entities;
    private PlayerShootingConfig targetPlayerShootingConfig;

    private float currentTimer;



    [BurstCompile]
    protected override void OnCreate() {
        RequireForUpdate<PlayerTag>();
        RequireForUpdate<PlayerShootingConfig>();


        currentTimer = 0.0f;
        Debug.Log("On Create! - Player Shooting");
    }

    [BurstCompile]
    protected override void OnDestroy() {

        if (entities.IsCreated)
            entities.Dispose();

        Debug.Log("On Destroy! -  Player Shooting");
    }

    [BurstCompile]
    protected override void OnUpdate() {

        targetPlayerShootingConfig = SystemAPI.GetSingleton<PlayerShootingConfig>();

        if (!entities.IsCreated)
            SetupPool();
        else {
            UpdateShootingTimer();

            EntityCommandBuffer buffer = new EntityCommandBuffer(WorldUpdateAllocator);
            foreach (PlayerDataAspect aspect in SystemAPI.Query<PlayerDataAspect>().WithAll<PlayerTag>()) {
                if (ValidateAction(aspect)) {
                    Entity targetProjectile = SpawnBullet(aspect);
                    if (EntityManager.Exists(targetProjectile))
                        buffer.SetEnabled(targetProjectile, true);

                }
            }

            buffer.Playback(EntityManager);
            buffer.Dispose();
        }
    }


    [BurstCompile]
    private void SetupPool() {
        entities = new NativeArray<Entity>(targetPlayerShootingConfig.poolSize, Allocator.Persistent);
        EntityManager.Instantiate(targetPlayerShootingConfig.projectileEntity, entities);

        foreach (var entity in entities) {
            SystemAPI.SetComponent<LocalTransform>(entity, new LocalTransform {
                Position = new Vector3(0, 0, 0),
                Rotation = quaternion.identity,
                Scale = 1

            });

            EntityManager.SetEnabled(entity, false);
        }
    }

    [BurstCompile]
    private Entity SpawnBullet(PlayerDataAspect aspect) {
        if (currentTimer > 0.0f)
            return Entity.Null;

        foreach (var entity in entities) {
            if (!EntityManager.IsEnabled(entity)) {
                RefRW<LocalTransform> transform = SystemAPI.GetComponentRW<LocalTransform>(entity);
                transform.ValueRW.Position = aspect.transform.ValueRO.Position + (aspect.transform.ValueRO.Right() * targetPlayerShootingConfig.spawnPositionOffset);

                RefRW<PlayerProjectileData> data = SystemAPI.GetComponentRW<PlayerProjectileData>(entity);
                if (data.IsValid)
                    data.ValueRW.movementDirection = aspect.transform.ValueRO.Right();

                currentTimer = targetPlayerShootingConfig.fireDelay;
                return entity;
            }
        }

        return Entity.Null;
    }

    [BurstCompile]
    private void UpdateShootingTimer() {
        if (currentTimer > 0.0f) {
            currentTimer -= SystemAPI.Time.DeltaTime;
            if (currentTimer < 0.0f)
                currentTimer = 0.0f;
            return;
        }
    }

    [BurstCompile]
    private bool ValidateAction(PlayerDataAspect aspect) {
        return aspect.input.ValueRO.isShooting;
    }
}