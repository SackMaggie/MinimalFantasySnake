using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace Snake
{
    /// <summary>
    /// Based on GDD
    /// Press WASD on the keyboard to move in the up, left, right, and down directions.
    /// The player cannot move in the opposite direction. If the player character is facing up, they can go left, up, or right, but not down.
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class SnakeMovement : CustomMonoBehaviour
    {
        public PlayerInput playerInput;
        public Direction currentDirection;

        public Func<MovementContext, bool> onMove;

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            if (playerInput == null)
                playerInput = GetComponent<PlayerInput>();
        }

        /// <summary>
        /// Hook from PlayerInput component
        /// </summary>
        /// <param name="callbackContext"></param>
        public void Move(CallbackContext callbackContext)
        {
            Debug.Log($"{callbackContext.ReadValue<Vector2>()} {callbackContext.phase}");
            switch (callbackContext.phase)
            {
                case InputActionPhase.Performed:
                    Vector2 vector2 = callbackContext.ReadValue<Vector2>();
                    if (CheckInputValid(vector2))
                        ProcessInput(vector2);
                    else
                        Debug.Log("Performed Input with zero value ??");
                    break;
            }

            static bool CheckInputValid(Vector2 vector2) => !(Mathf.Approximately(vector2.x, 0) && Mathf.Approximately(vector2.y, 0));

            void ProcessInput(Vector2 input)
            {
                Direction direction = ConvertToDirection(input);
                Direction currentDirection = this.currentDirection;
                if (currentDirection.IsOposite(direction))
                    throw new InvalidOperationException($"Can't go backward, from {currentDirection} to {direction}");

                MovementContext movementContext = new MovementContext()
                {
                    newDirection = direction,
                    oldDirection = currentDirection,
                    rawInput = input,
                };

                if (onMove.Invoke(movementContext))
                    this.currentDirection = direction;
            }
        }

        private static Direction ConvertToDirection(Vector2 vector2)
        {
            switch (vector2.y)
            {
                case > 0:
                    return Direction.UP;
                case < 0:
                    return Direction.DOWN;
            }

            switch (vector2.x)
            {
                case > 0:
                    return Direction.RIGHT;
                case < 0:
                    return Direction.LEFT;
            }

            throw new InvalidOperationException($"Unable to define direction from {vector2}");
        }
    }

    public static class SnakeMovementExtension
    {
        public static bool IsOposite(this Direction oldDirection, Direction newDirection)
        {
            /// This only work in the scenario where we design each <see cref="Direction"/> pair with oposite value
            /// Could avoid casting if use other type
            return ((sbyte)oldDirection + (sbyte)newDirection) == 0;
        }
    }

    public record MovementContext
    {
        public Direction newDirection;
        public Direction oldDirection;
        public Vector2 rawInput;
    }

    /// <summary>
    /// Intentionaly define oposite direction as a negative and positive pair of the same value
    /// So when checking for oposite value, we can just add value of two direction and check if it zero
    /// </summary>
    public enum Direction : sbyte
    {
        UP = 1,
        DOWN = -1,
        LEFT = -2,
        RIGHT = 2,
    }
}