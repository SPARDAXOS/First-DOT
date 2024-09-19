using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


public struct BoundsData : IComponentData {
    public float horizontalCameraExtent;
    public float verticalCameraExtent;

    public float leftCameraBounds;
    public float rightCameraBounds;
    public float topCameraBounds;
    public float bottomCameraBounds;
}

public class BoundsDataAuthoring : MonoBehaviour {




    private class Baker : Baker<BoundsDataAuthoring> {
        public override void Bake(BoundsDataAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<BoundsData>(entity, new BoundsData {
                horizontalCameraExtent = 0.0f,
                verticalCameraExtent = 0.0f,
                leftCameraBounds = 0.0f,
                rightCameraBounds = 0.0f,
                topCameraBounds = 0.0f,
                bottomCameraBounds = 0.0f,

            });

        }
    }
}
