using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerProjectileDataAuthoring : MonoBehaviour {

    public float projectileSpeed = 2.0f;
    public float projectileDamage = 5.0f;

    public class Baker : Baker<PlayerProjectileDataAuthoring> {
        public override void Bake(PlayerProjectileDataAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new PlayerProjectileData {
                projectileSpeed = authoring.projectileSpeed,
                projectileDamage = authoring.projectileDamage,
            });
        }
    }
}

public struct PlayerProjectileData : IComponentData {
    public float projectileSpeed;
    public float projectileDamage;
    public float3 movementDirection;
}