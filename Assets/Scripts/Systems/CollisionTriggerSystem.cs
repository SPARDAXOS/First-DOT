using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Burst;
using System;
using Unity.Collections;


[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
[BurstCompile]
public partial class CollisionTriggerSystem : SystemBase {

    [BurstCompile]
    protected override void OnCreate() {

    }

    [BurstCompile]
    protected override void OnDestroy() {

    }

    [BurstCompile]
    protected override void OnUpdate() {

        var endSimulationECBSystem = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var bulletEnemyJob = new BulletEnemyCollisionJob {
            allPlayerProjectiles = GetComponentLookup<PlayerProjectileTag>(false),
            allEnemies = GetComponentLookup<EnemyTag>(false),
            ECB = endSimulationECBSystem.CreateCommandBuffer(World.Unmanaged),
        };
        
        Dependency = bulletEnemyJob.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), Dependency);
    }
}


[BurstCompile]
public partial struct BulletEnemyCollisionJob : ITriggerEventsJob {

    public ComponentLookup<PlayerProjectileTag> allPlayerProjectiles;
    public ComponentLookup<EnemyTag> allEnemies;
    public EntityCommandBuffer ECB;

    [BurstCompile]
    public void Execute(TriggerEvent triggerEvent) {

        Entity entityA = triggerEvent.EntityA;
        Entity entityB = triggerEvent.EntityB;

        if (allPlayerProjectiles.HasComponent(entityA) && allPlayerProjectiles.HasComponent(entityB)) // Projectile/Projectile
            return;

        if (allPlayerProjectiles.HasComponent(entityA) && allEnemies.HasComponent(entityB)) { // Projectile/Enemy
            ECB.SetEnabled(entityA, false);
            ECB.SetEnabled(entityB, false);
        }
        else if (allEnemies.HasComponent(entityA) && allPlayerProjectiles.HasComponent(entityB)) { // Enemy/Projectile
            ECB.SetEnabled(entityA, false);
            ECB.SetEnabled(entityB, false);
        }
    }
}

