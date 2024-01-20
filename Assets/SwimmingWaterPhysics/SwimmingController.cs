using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SwimmingController : MonoBehaviour
{
    [SerializeField] private float swimmingForce;
    [SerializeField] private float resistanceForce;
    [SerializeField] private float deadZone;
    [SerializeField] private Transform trackingSpace;

    private new Rigidbody rigidbody;
    private Vector3 currentDirection;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        // Check if either the left or right Pico controller's trigger button is pressed
        bool rightButtonPressed = GvrControllerInput.GetDevice(GvrControllerHand.Right).GetButton(GvrControllerButton.Trigger);
        bool leftButtonPressed = GvrControllerInput.GetDevice(GvrControllerHand.Left).GetButton(GvrControllerButton.Trigger);

        if (rightButtonPressed && leftButtonPressed)
        {
            // Get local velocities of left and right Pico controllers
            Vector3 leftHandDirection = GvrControllerInput.GetDevice(GvrControllerHand.Left).GetAngularVelocity();
            Vector3 rightHandDirection = GvrControllerInput.GetDevice(GvrControllerHand.Right).GetAngularVelocity();
            Vector3 localVelocity = leftHandDirection + rightHandDirection;

            // Invert the localVelocity and check if it's beyond the deadZone
            localVelocity *= -1f;
            if (localVelocity.sqrMagnitude > deadZone * deadZone)
            {
                AddSwimmingForce(localVelocity);
            }
        }
        ApplyResistanceForce();
    }

    private void ApplyResistanceForce()
    {
        if (rigidbody.velocity.sqrMagnitude > 0.01f && currentDirection != Vector3.zero)
        {
            rigidbody.AddForce(-rigidbody.velocity * resistanceForce, ForceMode.Acceleration);
        }
        else
        {
            currentDirection = Vector3.zero;
        }
    }

    private void AddSwimmingForce(Vector3 localVelocity)
    {
        // Transform local velocity to world space
        Vector3 worldSpaceVelocity = trackingSpace.TransformDirection(localVelocity);
        rigidbody.AddForce(worldSpaceVelocity * swimmingForce, ForceMode.Acceleration);
        currentDirection = worldSpaceVelocity.normalized;
    }
}
