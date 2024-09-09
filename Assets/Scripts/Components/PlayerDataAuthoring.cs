using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


public struct PlayerData : IComponentData {

    public float maximumHealth;
    public float currentHealth;
}


public class PlayerDataAuthoring : MonoBehaviour {

    public float maximumHealth;
    public float currentHealth;


    private class Baker : Baker<PlayerDataAuthoring> {

        public override void Bake(PlayerDataAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new PlayerData { currentHealth = authoring.currentHealth, maximumHealth = authoring.maximumHealth });
        }
    }

}
