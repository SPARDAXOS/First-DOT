using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public partial class SpawnerSystem : SystemBase {

   public NativeArray<Entity> entities;
   public SpawnerSystemConfig targetConfig;

    public int currentlyActiveEnemies = 0;
    public float currentTimer = 0.0f;

    protected override void OnCreate() {
        RequireForUpdate<SpawnerSystemConfig>();

        Debug.Log("On Create! - Spawner");
    }
    protected override void OnDestroy() {
        Debug.Log("On Destroy! - Spawner");

        if (entities.IsCreated)
            entities.Dispose();
    }
    protected override void OnUpdate() {
        Debug.Log("On Update! - Spawner");

        if (!entities.IsCreated)
            SetupPool();
        else
            UpdateSpawns();
    }


    [BurstCompile]
    private void SetupPool() {
        targetConfig = SystemAPI.GetSingleton<SpawnerSystemConfig>();

        entities = new NativeArray<Entity>(targetConfig.poolSize, Allocator.Persistent);
        EntityManager.Instantiate(targetConfig.enemyEntity, entities);

        foreach(var entity in entities) {
            SystemAPI.SetComponent<LocalTransform>(entity, new LocalTransform {
                Position = new Vector3(10, 0, 0),
                Rotation = quaternion.identity,
                Scale = 1.0f

            });
        }
    }

    [BurstCompile]
    private void UpdateSpawns() {
        if (currentlyActiveEnemies == targetConfig.maximumActiveEnemies || currentlyActiveEnemies == entities.Length)
            return;

        if (currentTimer > 0.0f) {
            currentTimer -= SystemAPI.Time.DeltaTime;
            return;
        }





        //On Success.
        currentTimer = targetConfig.spawnDelay;
    }



}
