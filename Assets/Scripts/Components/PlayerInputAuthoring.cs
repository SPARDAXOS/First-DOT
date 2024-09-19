using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;



public struct PlayerInputData : IComponentData {
    public Vector2 currentInput;
    public float3 rotationTarget;
    public bool isShooting;
}


public class PlayerInputAuthoring : MonoBehaviour {





    private class Baker : Baker<PlayerInputAuthoring> {
        public override void Bake(PlayerInputAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<PlayerInputData>(entity, new PlayerInputData {});


        }
    }
}
