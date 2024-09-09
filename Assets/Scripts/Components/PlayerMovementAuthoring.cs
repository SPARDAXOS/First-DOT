using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEngine;



public struct PlayerMovementData : IComponentData {
    public float speed;
    public Vector2 currentInput;
    public Quaternion targetRotation;
    public Vector3 mousePositionTempDeleteMe;
}


public class PlayerMovementAuthoring : MonoBehaviour {

    public float speed = 1.0f;
    Vector2 currentInput = Vector2.zero;
    Quaternion targetRotation;

    public void UpdateMovement(Vector2 input) {
        currentInput = input;
    }





    private class Baker : Baker<PlayerMovementAuthoring> {
        public override void Bake(PlayerMovementAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<PlayerMovementData>(entity, new PlayerMovementData {
                speed = authoring.speed,
                currentInput = authoring.currentInput,
                targetRotation = authoring.targetRotation
            });


        }
    }
}
