using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerShootingAuthoring : MonoBehaviour {

    public GameObject projectilePrefab;
    public int poolSize;
    public float fireDelay;
    public float spawnPositionOffset;


    public class Baker : Baker<PlayerShootingAuthoring> {
        public override void Bake(PlayerShootingAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new PlayerShootingConfig {
                projectileEntity = GetEntity(authoring.projectilePrefab, TransformUsageFlags.Dynamic),
                poolSize = authoring.poolSize,
                fireDelay = authoring.fireDelay,
                spawnPositionOffset = authoring.spawnPositionOffset,
            });
        }
    }


}

public struct PlayerShootingConfig : IComponentData {
    public Entity projectileEntity;
    public int poolSize;
    public float fireDelay;
    public float spawnPositionOffset;
}