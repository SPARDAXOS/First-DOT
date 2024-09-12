using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EnemyTagAuthoring : MonoBehaviour {
    public class Baker : Baker<EnemyTagAuthoring> {
        public override void Bake(EnemyTagAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EnemyTag());
        }
    }
}

public struct EnemyTag : IComponentData {

}
