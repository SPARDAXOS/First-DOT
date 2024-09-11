using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SpawnerSystemAuthoring : MonoBehaviour {

    public GameObject enemyPrefab;
    public int poolSize;
    public int maximumActiveEnemies;
    public float spawnDelay;

    public class Baker : Baker<SpawnerSystemAuthoring> {
        public override void Bake(SpawnerSystemAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new SpawnerSystemConfig {
                enemyEntity = GetEntity(authoring.enemyPrefab, TransformUsageFlags.Dynamic),
                poolSize = authoring.poolSize,
                maximumActiveEnemies = authoring.maximumActiveEnemies,
                spawnDelay = authoring.spawnDelay
            });
        }
    }


}

public struct SpawnerSystemConfig : IComponentData {
    public Entity enemyEntity;
    public int poolSize;
    public int maximumActiveEnemies;
    public float spawnDelay;
}
