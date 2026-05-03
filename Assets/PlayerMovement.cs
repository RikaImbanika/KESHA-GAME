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
	public float pushingDrag;

	public float jumpingSpeedBoost;
	public float pushingSpeedBoost;

	[Header("Jumping")]
	public float jumpForce;
	public float jumpCooldown;
	public float airMultiplier;
	public float airWallFriction;
	public float airWallFrictionRb;
	public float beforeJumpSpeedSave;
	public bool readyToJump;

	[Header("Pushing")]
	public float pushingCooldown;
	public float pushingMultiplier;
	public bool readyToPush;
    public bool _pushed;

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

	[Header("Sounds")]
	private string _stepType;
	private float _distBeforeStep;
	private float _timeBeforeStep;
	private int _lastStepSoundId;
	int _dirtStepsSoundsCount;
	int _plankStepsSoundsCount;
	int _tilesStepsSoundsCount;
	string[] _dirtStepSoundsNames;
	string[] _plankStepSoundsNames;
	string[] _tilesStepSoundsNames;
	bool _stopped;

	[Header("Other")]
	public Transform orientation;

	public float horizontalInput;
	public float verticalInput;
	public bool h;
	public bool v;
	public bool hv;

	Vector3 moveDirection;

	public Rigidbody rb;

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
		_dirtStepsSoundsCount = 3;
		_dirtStepSoundsNames = new string[_dirtStepsSoundsCount];
		for (int i = 0; i < _dirtStepsSoundsCount; i++)
			_dirtStepSoundsNames[i] = $"dirtStep{i + 1}";

		_plankStepsSoundsCount = 5;
		_plankStepSoundsNames = new string[_plankStepsSoundsCount];
		for (int i = 0; i < _plankStepsSoundsCount; i++)
			_plankStepSoundsNames[i] = $"plankStep{i + 1}";

		_tilesStepsSoundsCount = 5;
		_tilesStepSoundsNames = new string[_tilesStepsSoundsCount];
		for (int i = 0; i < _tilesStepsSoundsCount; i++)
			_tilesStepSoundsNames[i] = $"tilesStep{i + 1}";

		StartCoroutine(LateStart());

		IEnumerator LateStart()
		{
			while (S.PS == null)
				yield return new WaitForSeconds(0.1f);

			rb = GetComponent<Rigidbody>();
			rb.freezeRotation = true;
			readyToJump = true;
			readyToPush = true;
		}
	}

	private void Update()
	{
		if (S.PS == null)
			return;

		MyInput();
		GetSpeed();
		CutSpeed();

		if (_pushed)
		{
			pm.staticFriction = 0;
			pm.dynamicFriction = 0;
			rb.velocity = new Vector3(rb.velocity.x * pushingDrag, rb.velocity.y * pushingDrag, rb.velocity.z * pushingDrag);
		}
		else if (grounded && readyToJump)
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
			S.GaymeBroker.OhNoTeso();

		if (grounded && readyToPush)
			_pushed = false;

		if (!S.PS._isTeleporting)
		{
			S.PS._prevCamPos = S.PS._camPos;
			S.PS._camPos = S.Camera.transform.position;
		}

		//

		StepsSounds();

		//

		float k = Time.deltaTime * 60f;

		MovePlayer(k);
		Crouch(k);
		Gravity(k);
	}

	private void StepsSounds()
	{
		_stepType = "planks";

		string sn = S.PS._currentSceneName;

		if (sn.Contains("MR"))
		{
			if (sn != "MR 1" && sn != "MR 3")
				_stepType = "dirt";
		}
		else if (sn == "Income")
			_stepType = "dirt";
		else if (sn.Contains("TL") || sn == "Start")
		{
			if (sn != "TL 0")
				_stepType = "tiles";
		}

		Vector2 p1 = new Vector2(S.PS._prevCamPos.x, S.PS._prevCamPos.z);
		Vector2 p2 = new Vector2(S.PS._camPos.x, S.PS._camPos.z);

		float moved = (p1 - p2).magnitude;

		_distBeforeStep -= moved / 60f * 2.5f;
		_timeBeforeStep -= Time.deltaTime;

		if (grounded && ((_distBeforeStep <= 0 && _timeBeforeStep <= 0) || (_stopped && moved > 0.01f)) && ((horizontalInput != 0 || verticalInput != 0) || _distBeforeStep <= 0))
		{
			DoStep();
		}
		
		_stopped = moved < 0.01f;
	}

	private void DoStep()
	{
		_distBeforeStep = 0.2f;

		int count = _dirtStepsSoundsCount;
		if (_stepType == "planks")
			count = _plankStepsSoundsCount;
		else if (_stepType == "tiles")
			count = _tilesStepsSoundsCount;

		int id = S.RND.Next(count);

		while (id == _lastStepSoundId)
			id = S.RND.Next(count);

		_lastStepSoundId = id;

		float pitch = 1f + (float)S.RND.NextDouble() * 0.25f;

		if (_stepType == "dirt")
			S.AudioManager.Play(_dirtStepSoundsNames[id], pitch);
		else if (_stepType == "planks")
			S.AudioManager.Play(_plankStepSoundsNames[id], pitch);
		else if (_stepType == "tiles")
			S.AudioManager.Play(_tilesStepSoundsNames[id], pitch);
	}

	private void Gravity(float k)
	{
		rb.AddForce(Vector3.down * gravity * k);
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
					}

					if (Input.GetKeyDown(crouchKey))
						isCrouching = true;

					if (Input.GetKeyUp(crouchKey))
						isCrouching = false;
				}
	}

	private void Crouch(float k)
	{
		if (isCrouching)
		{
			if (playerObject.transform.localScale.y > crouchScale)
			{
				float buf = crouchDeltaDown * k;
				playerObject.transform.localScale = new Vector3(playerObject.transform.localScale.x, playerObject.transform.localScale.y - buf, playerObject.transform.localScale.z);
				playerObject.transform.localPosition = new Vector3(playerObject.transform.localPosition.x, playerObject.transform.localPosition.y + buf, playerObject.transform.localPosition.z);
				transform.position = new Vector3(transform.position.x, transform.position.y - buf, transform.position.z);
			}
		}

		if (!isCrouching)
		{
			if (playerObject.transform.localScale.y < 1)
			{
				float buf = crouchDeltaUp * k;
				playerObject.transform.localScale = new Vector3(playerObject.transform.localScale.x, playerObject.transform.localScale.y + buf, playerObject.transform.localScale.z);
				playerObject.transform.localPosition = new Vector3(playerObject.transform.localPosition.x, playerObject.transform.localPosition.y - buf, playerObject.transform.localPosition.z);
				transform.position = new Vector3(transform.position.x, transform.position.y + buf, transform.position.z);
			}
		}
	}

	private void MovePlayer(float k)
	{
		if (!showingCamera.enabled)
			if (!inventory.opened)
			{
				float multiplier = k;
				if (verticalInput != 0 && horizontalInput != 0)
					multiplier = k * 0.7071f;

				Vector3 fwd = orientation.forward;

				if (Mathf.Abs(orientation.forward.y) > 0.999f)
					fwd = Quaternion.Euler(0, orientation.eulerAngles.y, 0) * Vector3.forward;

				moveDirection = fwd * verticalInput + orientation.right * horizontalInput;
				moveDirection = new Vector3(moveDirection.x, 0f, moveDirection.z).normalized;

				if (_pushed)
					rb.AddForce(moveDirection * walkSpeed * 60f * pushingMultiplier * multiplier, ForceMode.Force);
				else if (grounded)
					rb.AddForce(moveDirection * moveSpeed * 60f * multiplier, ForceMode.Force);
				else
					rb.AddForce(moveDirection * walkSpeed * 60f * airMultiplier * multiplier, ForceMode.Force);

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
		if (_pushed)
			limit *= pushingSpeedBoost;

		if (flatVelocity.magnitude > limit)
		{
			Vector3 limitedVelocity = flatVelocity.normalized * limit;
			rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
		}
	}

	public void Push(Vector3 force, bool forced = false)
	{
		if (readyToPush || forced)
		{
			readyToPush = false;
			rb.velocity += force;
			_pushed = true;
			Invoke(nameof(ResetPush), pushingCooldown);
		}
	}

	private void Jump()
	{
		readyToJump = false;

		if (horizontalInput == 0 && verticalInput == 0)
			rb.velocity = new Vector3(rb.velocity.x * beforeJumpSpeedSave, jumpForce, rb.velocity.z * beforeJumpSpeedSave);
		else
			rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);

		DoStep();
		_distBeforeStep = 0f;
		_timeBeforeStep = 0.3f;

		Invoke(nameof(ResetJump), jumpCooldown);
	}

	private void ResetJump()
	{
		readyToJump = true;
	}

	private void ResetPush()
	{
		readyToPush = true;
	}
}
