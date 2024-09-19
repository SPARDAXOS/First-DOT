using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


[BurstCompile]
public partial class PlayerShootingSystem : SystemBase {

    private NativeArray<Entity> entities;
    private PlayerShootingConfig targetPlayerShootingConfig;

    private List<PlayerDataAspect> shootingCommands;
    private float currentTimer;


    protected override void OnCreate() {
        RequireForUpdate<PlayerTag>();
        RequireForUpdate<PlayerShootingConfig>();

        currentTimer = 0.0f;
        shootingCommands = new List<PlayerDataAspect>();
        Debug.Log("On Create! - Player Shooting");
    }
    protected override void OnDestroy() {

        if (entities.IsCreated)
            entities.Dispose();

        Debug.Log("On Destroy! -  Player Shooting");
    }
    protected override void OnUpdate() {

        targetPlayerShootingConfig = SystemAPI.GetSingleton<PlayerShootingConfig>();

        if (!entities.IsCreated)
            SetupPool();
        else {
            UpdateShootingTimer();

            //TODO: to func
            foreach (PlayerDataAspect aspect in SystemAPI.Query<PlayerDataAspect>().WithAll<PlayerTag>()) {
                if (ValidateAction(aspect))
                    shootingCommands.Add(aspect);
            }

            PoolShootingCommands();
        }
    }


    [BurstCompile]
    private void PoolShootingCommands() {
        if (shootingCommands.Count == 0)
            return;

        foreach (var command in shootingCommands)
            SpawnBullet(command);

        shootingCommands.Clear();
    }


    [BurstCompile]
    private void SetupPool() {
        entities = new NativeArray<Entity>(targetPlayerShootingConfig.poolSize, Allocator.Persistent);
        EntityManager.Instantiate(targetPlayerShootingConfig.bulletEntity, entities);

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
    private void SpawnBullet(PlayerDataAspect aspect) {

        if (currentTimer > 0.0f)
            return;

        bool validProjectileFound = false;
        Entity targetEntity = new Entity();
        
        foreach (var entity in entities) {
            if (!EntityManager.IsEnabled(entity)) {
                RefRW<LocalTransform> transform = SystemAPI.GetComponentRW<LocalTransform>(entity);
                transform.ValueRW.Position = aspect.transform.ValueRO.Position; //then offset it by player right * offset+
                targetEntity = entity;
                validProjectileFound = true;

                currentTimer = targetPlayerShootingConfig.fireDelay;
                Debug.Log("Player Bullet Spawned!");
                break;
            }
        }
        
        if (validProjectileFound)
            EntityManager.SetEnabled(targetEntity, true);
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
