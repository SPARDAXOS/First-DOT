using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Burst;


[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial class CollisionTriggerSystem : SystemBase {


    private EndSimulationEntityCommandBufferSystem.Singleton endSimulationECBSystem;

    protected override void OnCreate() {


        Debug.Log("On Create! - CollisionTriggerSystem");
    }
    protected override void OnDestroy() {

        Debug.Log("On Destroy! - CollisionTriggerSystem");
    }
    protected override void OnUpdate() {

        endSimulationECBSystem = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        //Simulation simulation = SystemAPI.GetSingleton<SimulationSingleton>().AsSimulation();

        var bulletEnemyJob = new BulletEnemyCollisionJob {
            allPlayerProjectiles = GetComponentLookup<PlayerProjectileTag>(false),
            allEnemies = GetComponentLookup<EnemyTag>(false),
            ECB = endSimulationECBSystem.CreateCommandBuffer(World.Unmanaged),
            world = World
        };

        Dependency = bulletEnemyJob.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), Dependency);

        Debug.Log("On Update! - CollisionTriggerSystem");
    }

}


[BurstCompile]
public partial struct BulletEnemyCollisionJob : ITriggerEventsJob {

    public ComponentLookup<PlayerProjectileTag> allPlayerProjectiles;
    public ComponentLookup<EnemyTag> allEnemies;
    public EntityCommandBuffer ECB;

    public World world; //This is a problem. either i fix it or i attempt to change how the spawner works.

    [BurstCompile]
    public void Execute(TriggerEvent triggerEvent) {

        Entity entityA = triggerEvent.EntityA;
        Entity entityB = triggerEvent.EntityB;

        if (allPlayerProjectiles.HasComponent(entityA) && allPlayerProjectiles.HasComponent(entityB)) // Projectile/Projectile
            return;

        if (allPlayerProjectiles.HasComponent(entityA) && allEnemies.HasComponent(entityB)) { // Projectile/Enemy
            ECB.SetEnabled(entityA, false);
            SpawnerSystem system = world.GetExistingSystemManaged<SpawnerSystem>();
            if (system != null) {
                system.NotifyDispawn();
            }
            ECB.SetEnabled(entityB, false);
        }
        else if (allEnemies.HasComponent(entityA) && allPlayerProjectiles.HasComponent(entityB)) { // Enemy/Projectile
            ECB.SetEnabled(entityA, false);
            SpawnerSystem system = world.GetExistingSystemManaged<SpawnerSystem>();
            if (system != null) {
                system.NotifyDispawn();
            }

            ECB.SetEnabled(entityB, false);
        }
    }
}
