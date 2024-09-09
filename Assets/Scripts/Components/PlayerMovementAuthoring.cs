using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;



public struct PlayerMovementData : IComponentData {
    public float speed;
    public Vector2 currentInput;
    public float3 rotationTarget;
}


public class PlayerMovementAuthoring : MonoBehaviour {

    public float speed = 1.0f;
    Vector2 currentInput = Vector2.zero;


    //Not called
    public void UpdateMovement(Vector2 input) {
        currentInput = input;
    }





    private class Baker : Baker<PlayerMovementAuthoring> {
        public override void Bake(PlayerMovementAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<PlayerMovementData>(entity, new PlayerMovementData {
                speed = authoring.speed,
                currentInput = authoring.currentInput
            });


        }
    }
}
