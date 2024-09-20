using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct EnemyControllerSystem : ISystem {

    private Entity playerEntity;
    private RefRO<LocalTransform> playerTransform;
    private bool playerEntityQueried;
    private bool valid;


    [BurstCompile]
    void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EnemyData>();
        state.RequireForUpdate<EnemyTag>();
        state.RequireForUpdate<PlayerTag>();

        Debug.Log("On Create! - EnemyController");
    }

    [BurstCompile]
    void OnDestroy(ref SystemState state) {
        Debug.Log("On Destroy! - EnemyController");
    }

    [BurstCompile]
    void OnUpdate(ref SystemState state) {
        if (!playerEntityQueried)
            QueryPlayerEntity(ref state);

        if (!valid) {
            Debug.LogWarning("EnemyControlleSystem is invalid!");
            return;
        }

        playerTransform = SystemAPI.GetComponentRO<LocalTransform>(playerEntity);

        var updateEnemyMovementJob = new UpdateEnemyMovementJob {
            deltaTime = SystemAPI.Time.DeltaTime,
            targetPlayerTransform = playerTransform.ValueRO
        };

        updateEnemyMovementJob.Schedule();
        Debug.Log("On Update! - EnemyController");
    }


    [BurstCompile]
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



[BurstCompile]
[WithAll(typeof(EnemyTag))]
public partial struct UpdateEnemyMovementJob : IJobEntity {

    public LocalTransform targetPlayerTransform;
    public float deltaTime;


    [BurstCompile]
    private void UpdateEnemyData(ref EnemyDataAspect data) {

        float3 playerDirection = targetPlayerTransform.Position - data.transform.ValueRO.Position;
        math.normalize(playerDirection);

        float3 currentPosition = data.transform.ValueRW.Position;
        data.transform.ValueRW.Position = currentPosition + (playerDirection * data.data.ValueRO.speed * deltaTime);
    }

    [BurstCompile]
    public void Execute(EnemyDataAspect data) {
        UpdateEnemyData(ref data);
    }
}
