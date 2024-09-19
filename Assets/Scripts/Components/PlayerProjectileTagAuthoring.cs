using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerProjectileTagAuthoring : MonoBehaviour {
    public class Baker : Baker<PlayerProjectileTagAuthoring> {
        public override void Bake(PlayerProjectileTagAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlayerProjectileTag());
        }
    }
}

public struct PlayerProjectileTag : IComponentData {

}
