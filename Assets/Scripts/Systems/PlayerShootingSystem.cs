using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public partial struct PlayerShootingSystem : ISystem {



    void OnCreate(ref SystemState state) {
        state.RequireForUpdate<PlayerTag>();

        Debug.Log("On Create! - Player Shooting");
    }
    void OnDestroy(ref SystemState state) {
        Debug.Log("On Destroy! -  Player Shooting");
    }
    void OnUpdate(ref SystemState state) {
        Debug.Log("On Update! -  Player Shooting");
    }



}
