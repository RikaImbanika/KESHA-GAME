using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public GameObject playerObject;

	[Header("Movement")]
	private float moveSpeed;
	public float walkSpeed;
	public float sprintSpeed;
	public float crouchSpeed;

	public float gravity;

	public float groundDrag;
	public float airDrag;

	public float jumpingSpeedBoost;

	[Header("Jumping")]
	public float jumpForce;
	public float jumpCooldown;
	public float airMultiplier;
	public float airWallFriction;
	public float airWallFrictionRb;
	public float beforeJumpSpeedSave;
	bool readyToJump;

	[Header("Crouching")]
	public float crouchScale;
	public bool isCrouching;
	public float crouchDeltaDown;
	public float crouchDeltaUp;

	[Header("Keybinds")]
	public KeyCode jumpKey = KeyCode.Space;
	public KeyCode sprintKey = KeyCode.LeftShift;
	public KeyCode crouchKey = KeyCode.LeftControl;

	[Header("Ground Check")]
	public float playerHeigh;
	public LayerMask whatIsGround;
	public bool grounded;

	public Transform orientation;

	public float horizontalInput;
	public float verticalInput;
	public bool h;
	public bool v;
	public bool hv;

	Vector3 moveDirection;

	Rigidbody rb;

	public PhysicMaterial pm;

	public Inventory inventory;

	public Camera camera;

	public LayerMask pickable;

	public Camera showingCamera;

	private void OnCollisionStay(Collision other)
	{
		if (!grounded)
			rb.velocity = new Vector3(rb.velocity.x * airWallFriction, rb.velocity.y, rb.velocity.z * airWallFriction);
	}

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.freezeRotation = true;
		readyToJump = true;
	}

	private void Update()
	{
		MyInput();
		GetSpeed();
		CutSpeed();

		if (grounded && readyToJump)
		{
			pm.staticFriction = 10;
			pm.dynamicFriction = 10;
			rb.velocity = new Vector3(rb.velocity.x * groundDrag, rb.velocity.y * groundDrag, rb.velocity.z * groundDrag);
		}
		else
		{
			pm.staticFriction = airWallFrictionRb;
			pm.dynamicFriction = airWallFrictionRb;
			rb.velocity = new Vector3(rb.velocity.x * airDrag, rb.velocity.y, rb.velocity.z * airDrag);
		}

		if (transform.position.y < -500)
			transform.position = new Vector3(transform.position.x, 500, transform.position.z);
	}

	private void FixedUpdate()
	{
		MovePlayer();
		Crouch();
		Gravity();
	}

	private void Gravity()
	{
		rb.AddForce(Vector3.down * gravity);
	}

	private void GetSpeed()
	{
		if (Input.GetKey(sprintKey))
			moveSpeed = sprintSpeed;
		else if (grounded)
			moveSpeed = walkSpeed;

		if (isCrouching && grounded)
			moveSpeed = crouchSpeed;
	}

	private void MyInput()
	{
		if (!showingCamera.enabled)
			if (!inventory.opened)
				if (!inventory._marketOpened)
				{
					horizontalInput = Input.GetAxisRaw("Horizontal");
					verticalInput = Input.GetAxisRaw("Vertical");

					if (Input.GetKey(jumpKey) && readyToJump && grounded)
					{
						Jump();

						Invoke(nameof(ResetJump), jumpCooldown);
					}

					if (Input.GetKeyDown(crouchKey))
						isCrouching = true;

					if (Input.GetKeyUp(crouchKey))
						isCrouching = false;
				}
	}

	private void Crouch()
	{
		if (isCrouching)
		{
			if (playerObject.transform.localScale.y > crouchScale)
			{
				playerObject.transform.localScale = new Vector3(playerObject.transform.localScale.x, playerObject.transform.localScale.y - crouchDeltaDown, playerObject.transform.localScale.z);
				playerObject.transform.localPosition = new Vector3(playerObject.transform.localPosition.x, playerObject.transform.localPosition.y + crouchDeltaDown, playerObject.transform.localPosition.z);
				transform.position = new Vector3(transform.position.x, transform.position.y - crouchDeltaDown, transform.position.z);
			}
		}

		if (!isCrouching)
		{
			if (playerObject.transform.localScale.y < 1)
			{
				playerObject.transform.localScale = new Vector3(playerObject.transform.localScale.x, playerObject.transform.localScale.y + crouchDeltaUp, playerObject.transform.localScale.z);
				playerObject.transform.localPosition = new Vector3(playerObject.transform.localPosition.x, playerObject.transform.localPosition.y - crouchDeltaUp, playerObject.transform.localPosition.z);
				transform.position = new Vector3(transform.position.x, transform.position.y + crouchDeltaUp, transform.position.z);
			}
		}
	}

	private void MovePlayer()
	{
		if (!showingCamera.enabled)
			if (!inventory.opened)
			{
				float multiplier = 1;
				if (verticalInput != 0 && horizontalInput != 0)
					multiplier = 0.7071f;

				moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
				moveDirection = new Vector3(moveDirection.x, 0f, moveDirection.z);

				if (grounded)
					rb.AddForce(moveDirection.normalized * moveSpeed * 60f * multiplier, ForceMode.Force);
				else
					rb.AddForce(moveDirection.normalized * walkSpeed * 60f * airMultiplier * multiplier, ForceMode.Force);

				if (isCrouching && !grounded)
					rb.velocity = new Vector3(rb.velocity.x * 0.9f, rb.velocity.y, rb.velocity.z * 0.9f);
			} 
	}

	private void CutSpeed()
	{
		Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

		float limit = moveSpeed;
		if (!grounded)
			limit *= jumpingSpeedBoost;

		if (flatVelocity.magnitude > limit)
		{
			Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
			rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
		}
	}

	private void Jump()
	{
		readyToJump = false;

		if (horizontalInput == 0 && verticalInput == 0)
			rb.velocity = new Vector3(rb.velocity.x * beforeJumpSpeedSave, jumpForce, rb.velocity.z * beforeJumpSpeedSave);
		else
			rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
	}

	private void ResetJump()
	{
		readyToJump = true;
	}
}
