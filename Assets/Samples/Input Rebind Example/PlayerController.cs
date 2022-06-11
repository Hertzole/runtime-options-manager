using UnityEngine;
using UnityEngine.InputSystem;

namespace Hertzole.SettingsManager.Samples.InputRebinding
{
	public class PlayerController : MonoBehaviour
	{
		[SerializeField]
		private InputActionReference moveInput = default;
		[SerializeField]
		private float moveSpeed = 8;

		private PlayerInput input;

		private Vector2 move;

		private void Awake()
		{
			input = GetComponent<PlayerInput>();
		}

		private void Update()
		{
			transform.Translate(new Vector3(move.x, move.y, 0) * moveSpeed * Time.deltaTime);
		}

		private void OnEnable()
		{
			input.actions.FindAction(moveInput.action.id).performed += OnMovePerformed;
			input.actions.FindAction(moveInput.action.id).canceled += OnMovePerformed;
		}

		private void OnDisable()
		{
			input.actions.FindAction(moveInput.action.id).performed -= OnMovePerformed;
			input.actions.FindAction(moveInput.action.id).canceled += OnMovePerformed;
		}

		private void OnMovePerformed(InputAction.CallbackContext obj)
		{
			move = obj.ReadValue<Vector2>();
		}
	}
}