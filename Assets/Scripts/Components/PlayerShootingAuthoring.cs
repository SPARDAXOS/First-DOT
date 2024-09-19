using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerShootingAuthoring : MonoBehaviour {

    public GameObject bulletPrefab;
    public int poolSize;
    public float fireDelay;
    public float spawnPositionOffset;

    public class Baker : Baker<PlayerShootingAuthoring> {
        public override void Bake(PlayerShootingAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new PlayerShootingConfig {
                bulletEntity = GetEntity(authoring.bulletPrefab, TransformUsageFlags.Dynamic),
                poolSize = authoring.poolSize,
                fireDelay = authoring.fireDelay,
                spawnPositionOffset = authoring.spawnPositionOffset
            });
        }
    }


}

public struct PlayerShootingConfig : IComponentData {
    public Entity bulletEntity;
    public int poolSize;
    public float fireDelay;
    public float spawnPositionOffset;
}