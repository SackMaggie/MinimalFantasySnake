using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace Snake.Movement
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

        public Func<bool> CheckCanMoveFunc;
        public Func<MovementContext, bool> RequestMovementFunc;
        public Action<bool> RequestCycleUnitAction;

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
        /// Hook from PlayerInput component -> <see cref="PlayerInput.actionEvents"/>
        /// </summary>
        /// <param name="callbackContext"></param>
        public void Move(CallbackContext callbackContext)
        {
            //Debug.Log($"{callbackContext.ReadValue<Vector2>()} {callbackContext.phase}");
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
                try
                {
                    if (!CheckCanMove())
                    {
                        Debug.Log("Movement is not allowed");
                        return;
                    }
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

                    if (RequestMovement(movementContext))
                        this.currentDirection = direction;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        private static Direction ConvertToDirection(Vector2 vector2)
        {
            // there's a problem when two keys is pressed at the same time such as W,D key we will get both x and y as 0.71
            // which mean it will go up and right ?
            // as mentioned above two key is pressed, let's just not move?

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

        private bool CheckCanMove()
        {
            try
            {
                return CheckCanMoveFunc.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
                return false;
            }
        }

        private bool RequestMovement(MovementContext movementContext)
        {
            try
            {
                return RequestMovementFunc.Invoke(movementContext);
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
                return false;
            }
        }

        /// <summary>
        /// Hook from PlayerInput component -> <see cref="PlayerInput.actionEvents"/>
        /// </summary>
        /// <param name="callbackContext"></param>
        public void HeroRotateForward(CallbackContext callbackContext)
        {
            if (callbackContext.phase == InputActionPhase.Performed)
                RequestCycleUnitAction.Invoke(true);
        }

        /// <summary>
        /// Hook from PlayerInput component -> <see cref="PlayerInput.actionEvents"/>
        /// </summary>
        /// <param name="callbackContext"></param>
        public void HeroRotateBackward(CallbackContext callbackContext)
        {
            if (callbackContext.phase == InputActionPhase.Performed)
                RequestCycleUnitAction.Invoke(false);
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

        public static bool GetOposite(this Direction oldDirection, Direction newDirection)
        {
            /// This only work in the scenario where we design each <see cref="Direction"/> pair with oposite value
            /// Could avoid casting if use other type
            return ((sbyte)oldDirection + (sbyte)newDirection) == 0;
        }

        public static Vector2Int GetRelativePosition(this Direction direction, Vector2Int currentPosition, short distance = 1)
        {
            switch (direction)
            {
                case Direction.UP:
                    currentPosition.y += distance;
                    break;
                case Direction.DOWN:
                    currentPosition.y -= distance;
                    break;
                case Direction.LEFT:
                    currentPosition.x -= distance;
                    break;
                case Direction.RIGHT:
                    currentPosition.x += distance;
                    break;
                default:
                    throw new NotImplementedException(direction.ToString());
            }
            return currentPosition;
        }

        public static float GetRotationAngle(this Direction direction) => direction switch
        {
            Direction.UP => 0,
            Direction.DOWN => 180,
            Direction.LEFT => 270,
            Direction.RIGHT => (float)90,
            _ => throw new NotImplementedException(direction.ToString()),
        };
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