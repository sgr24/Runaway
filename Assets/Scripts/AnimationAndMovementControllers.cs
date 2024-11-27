using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAndMovementControllers : MonoBehaviour
{
    PlayerInput playerInput;

    Vector2 currentMovementInput;
    Vector3 currentMovement;
    bool isMovementPressed;

    void Awake()
    {
        playerInput= new PlayerInput();

        playerInput.CharacterControls.Move.started += context => {
            currentMovementInput = context.ReadValue<Vector2>();
            currentMovement.x = currentMovementInput.x;
            currentMovement.z = currentMovementInput.y;
            isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
        };
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

    void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }
}
