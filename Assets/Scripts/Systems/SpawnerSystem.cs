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

    private NativeArray<Entity> entities;
    private SpawnerSystemConfig targetConfig;
    private BoundsData targetBoundsData;

    private int currentlyActiveEnemies = 0;
    private float currentTimer = 0.0f;


    protected override void OnCreate() {
        RequireForUpdate<SpawnerSystemConfig>();
        RequireForUpdate<BoundsData>();
 
        Debug.Log("On Create! - Spawner");
    }
    protected override void OnDestroy() {
        Debug.Log("On Destroy! - Spawner");

        if (entities.IsCreated)
            entities.Dispose();
    }
    protected override void OnUpdate() {
        Debug.Log("On Update! - Spawner");

        targetConfig = SystemAPI.GetSingleton<SpawnerSystemConfig>();
        targetBoundsData = SystemAPI.GetSingleton<BoundsData>();

        if (!entities.IsCreated)
            SetupPool();
        else
            UpdateSpawns();
    }


    public void NotifyDispawn() { 
        
    }

    [BurstCompile]
    private void SetupPool() {
        entities = new NativeArray<Entity>(targetConfig.poolSize, Allocator.Persistent);
        EntityManager.Instantiate(targetConfig.enemyEntity, entities);

        foreach(var entity in entities) {
            SystemAPI.SetComponent<LocalTransform>(entity, new LocalTransform {
                Position = new Vector3(0, 0, 0),
                Rotation = quaternion.identity,
                Scale = UnityEngine.Random.Range(targetConfig.scaleMin, targetConfig.scaleMax)

            });

            EntityManager.SetEnabled(entity, false);
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

        foreach (var entity in entities) {
            if (!EntityManager.IsEnabled(entity)) {
                RefRW<LocalTransform> transform = SystemAPI.GetComponentRW<LocalTransform>(entity);
                transform.ValueRW.Position = CalculateSpawnPosition();
                EntityManager.SetEnabled(entity, true);
                currentlyActiveEnemies++;
                break;
            }
        }

        currentTimer = targetConfig.spawnDelay;
    }

    [BurstCompile]
    private float3 CalculateSpawnPosition() {

        Camera mainCamera = Camera.main;
        float horizontalPoint;
        float verticalPoint;

        float horizontalOrVertical = UnityEngine.Random.Range(0, 2);
        if (horizontalOrVertical == 0) { //Horizontal Randomization
            horizontalPoint = UnityEngine.Random.Range(targetBoundsData.leftCameraBounds, targetBoundsData.rightCameraBounds);
            int upOrDown = UnityEngine.Random.Range(0, 2);
            if (upOrDown == 0) { //Up
                verticalPoint = mainCamera.transform.position.x + targetBoundsData.verticalCameraExtent / 2;
                verticalPoint += targetConfig.spawnPositionOffset;
            }
            else { //Down
                verticalPoint = mainCamera.transform.position.x - targetBoundsData.verticalCameraExtent / 2;
                verticalPoint -= targetConfig.spawnPositionOffset;
            }
        }
        else { //Horizontal Randomization
            int leftOrRight = UnityEngine.Random.Range(0, 2);
            if (leftOrRight == 0) { //Right
                horizontalPoint = mainCamera.transform.position.x + targetBoundsData.horizontalCameraExtent / 2;
                horizontalPoint += targetConfig.spawnPositionOffset;
            }
            else { //Left
                horizontalPoint = mainCamera.transform.position.x - targetBoundsData.horizontalCameraExtent / 2;
                horizontalPoint -= targetConfig.spawnPositionOffset;
            }

            verticalPoint = UnityEngine.Random.Range(targetBoundsData.bottomCameraBounds, targetBoundsData.topCameraBounds);
        }

        return new float3(horizontalPoint, verticalPoint, 0.0f);
    }

}
