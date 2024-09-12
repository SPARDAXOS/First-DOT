using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct EnemyControllerSystem : ISystem {

    private Entity playerEntity;
    private RefRO<LocalTransform> playerTransform;
    private bool playerEntityQueried;
    private bool valid;

    void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EnemyData>();

        Debug.Log("On Create! - EnemyController");
    }
    void OnDestroy(ref SystemState state) {
        Debug.Log("On Destroy! - EnemyController");
    }
    void OnUpdate(ref SystemState state) {

        if (!playerEntityQueried)
            QueryPlayerEntity(ref state);

        if (!valid) {
            Debug.LogWarning("EnemyControlleSystem is invalid!");
            return;
        }

        playerTransform = SystemAPI.GetComponentRO<LocalTransform>(playerEntity);

        foreach (EnemyDataAspect enemy in SystemAPI.Query<EnemyDataAspect>()) {
            float3 playerDirection = playerTransform.ValueRO.Position - enemy.transform.ValueRO.Position;
            math.normalize(playerDirection);

            float3 currentPosition = enemy.transform.ValueRW.Position;
            enemy.transform.ValueRW.Position = currentPosition + playerDirection * enemy.data.ValueRO.speed * SystemAPI.Time.DeltaTime;
        }

        Debug.Log("On Update! - EnemyController");
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
