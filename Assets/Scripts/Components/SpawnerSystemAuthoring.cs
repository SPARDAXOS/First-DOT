using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SpawnerSystemAuthoring : MonoBehaviour {

    public GameObject enemyPrefab;
    public float scaleMax;
    public float scaleMin;
    public int poolSize;
    public float spawnDelay;
    public float spawnPositionOffset;

    public class Baker : Baker<SpawnerSystemAuthoring> {
        public override void Bake(SpawnerSystemAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new SpawnerSystemConfig {
                enemyEntity = GetEntity(authoring.enemyPrefab, TransformUsageFlags.Dynamic),
                scaleMax = authoring.scaleMax,
                scaleMin = authoring.scaleMin,
                poolSize = authoring.poolSize,
                spawnDelay = authoring.spawnDelay,
                spawnPositionOffset = authoring.spawnPositionOffset
            });
        }
    }


}

public struct SpawnerSystemConfig : IComponentData {
    public Entity enemyEntity;
    public float scaleMax;
    public float scaleMin;
    public int poolSize;

    public float spawnDelay;
    public float spawnPositionOffset;
}
