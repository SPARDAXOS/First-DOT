using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EnemyDataAuthoring : MonoBehaviour {

    public float startingHealth = 1.0f;
    public float speed = 10.0f;

    public class Baker : Baker<EnemyDataAuthoring> {
        public override void Bake(EnemyDataAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new EnemyData {
                currentHealth = authoring.startingHealth,
                speed = authoring.speed
            });
        }
    }


}

public struct EnemyData : IComponentData {
    public float currentHealth;
    public float speed;
}