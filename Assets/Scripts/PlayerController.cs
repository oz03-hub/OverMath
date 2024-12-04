using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Animator animation_controller;
    private CharacterController character_controller;
    public Vector3 movement_direction;
    public Vector3 last_movement_direction;
    public float walking_velocity;
    public Text text;
    public float speed;
    public float smooth_time = 0.1f;
    public float rotation_speed = 10.0f;
    private MovementState current_state;
    private float dash_timer;
    private Vector3 dash_direction;
    private bool dashing;
    public float interactionRange = 1.95f;
    public LayerMask interactableLayer;
    private Interactable currentInteractable;
    private int? heldNumber = null;
    public GameLevelManager gameLevelManager;

    private TrailRenderer trail_renderer;

    private AudioSource audio_source;
    public AudioClip stepClip;
    public AudioClip thudClip;

    private enum MovementState {
        Walking,
        Idle,
        Jumping,
        Sprinting,
        Dashing,
    };

    // Start is called before the first frame update
    void Start()
    {
        if (currentInteractable == null) {
            Debug.Log("No interactable found");
        } else {
            Debug.Log("Interactable found");
        }
        animation_controller = GetComponent<Animator>();
        character_controller = GetComponent<CharacterController>();
        movement_direction = Vector3.forward;
        last_movement_direction = Vector3.forward;
        walking_velocity = 7.0f;
        speed = 0.0f;

        trail_renderer = GetComponent<TrailRenderer>();

        audio_source = GetComponent<AudioSource>();
        dashing = false;
        dash_timer = Time.time;
    }

    private void Step() {
        audio_source.PlayOneShot(stepClip, 0.1f);
    }

    private void Thud() {
        audio_source.PlayOneShot(thudClip);
    }

    void UpdateSpeed() {
        switch (this.current_state) {
            case MovementState.Walking:
                this.trail_renderer.enabled = false;
                this.speed = walking_velocity; break;
            case MovementState.Sprinting:
                this.trail_renderer.enabled = true;
                this.speed = walking_velocity * 1.5f; break;
            case MovementState.Idle:
                this.trail_renderer.enabled = false;
                this.speed = 0.0f; break;
            case MovementState.Dashing:
                this.trail_renderer.enabled = true;
                this.speed = walking_velocity * 4; break;
        }
    }

    void SetState(MovementState state) {
        this.current_state = state;
        this.animation_controller.SetBool("Walking", state==MovementState.Walking);
        if (state == MovementState.Jumping) {
            this.animation_controller.SetTrigger("Jumping");
        }
        this.animation_controller.SetBool("Sprinting", state==MovementState.Sprinting);
        if (state == MovementState.Dashing)
        {
            dash_direction = movement_direction;
        }
    }

    void CheckState() {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            if (character_controller.isGrounded && Input.GetKey(KeyCode.Space))
            {
                SetState(MovementState.Jumping);
            }
            //else if (Input.GetKey(KeyCode.Q))
            //{
            //        if (Time.time - dash_timer > 5.0f) {
            //            dashing = true;
            //            dash_timer = Time.time;
            //            SetState(MovementState.Dashing);
            //        } else
            //        {
            //            SetState(MovementState.Walking);
            //        }
            //    }
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                SetState(MovementState.Sprinting);
            }
            else
            {
                SetState(MovementState.Walking);
            }
        } else if (character_controller.isGrounded && Input.GetKey(KeyCode.Space)) {
            SetState(MovementState.Jumping);
        } else {
            SetState(MovementState.Idle);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckState();
        UpdateSpeed();

        //if (dash_timer > 0)
        //{
        //    dash_timer -= Time.deltaTime;
        //    movement_direction = dash_direction;
        //}

        //if (dash_timer - Time.time < 1.0f)
        //{
        //    movement_direction = dash_direction;
        //    character_controller.Move(movement_direction * speed * Time.deltaTime);
        //    return;
        //}
        //else {
        //    dashing = false;
        //}

        float xdirection = Input.GetAxisRaw("Horizontal");
        float zdirection = Input.GetAxisRaw("Vertical");
        Vector3 input_direction = new Vector3(xdirection, 0.0f, zdirection).normalized;

        if (input_direction.magnitude > 0)
        {
            movement_direction = Vector3.Lerp(movement_direction, input_direction, smooth_time);
            last_movement_direction = movement_direction;

            Quaternion target_rotation = Quaternion.LookRotation(movement_direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, target_rotation, Time.deltaTime * rotation_speed);
        }
        else
        {
            movement_direction = Vector3.zero;

            Quaternion target_rotation = Quaternion.LookRotation(last_movement_direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, target_rotation, Time.deltaTime * rotation_speed);
        }


        character_controller.Move(movement_direction * speed * Time.deltaTime);

        DetectInteractable();

        // Handle dropping the number (if holding any)
        if (heldNumber != null)
        {
            if (Input.GetKeyDown(KeyCode.G)) // Example key for dropping
            {
                heldNumber = null; // No longer holding anything
                gameLevelManager.UpdateIngredientText(""); // Clear the UI text
                Debug.Log("Dropped the number.");
            }

            // Prevent picking up another number while holding one
            return; // Skip the logic below, but allow dropping and blinking
        }

        if (currentInteractable != null && Input.GetKeyDown(KeyCode.F))
        {
            currentInteractable.Interact();

            NumberComponent numberComponent = currentInteractable.GetComponent<NumberComponent>();
            if (numberComponent != null)
            {
                heldNumber  = numberComponent.numberValue;
                gameLevelManager.UpdateIngredientText(heldNumber.ToString());
            }
            ClearCurrentHighlight();
        }
    }
    void DetectInteractable()
    {
        // Find all colliders within the interaction range
        // If there are any objects within this range, they are stored in hitColliders as an array of colliders
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange, interactableLayer);
        // If there are one or more object in this range
        if (hitColliders.Length > 0)
        {
            Interactable nearestInteractable = hitColliders[0].GetComponent<Interactable>();
            if (nearestInteractable != currentInteractable)
            {
                ClearCurrentHighlight();
                currentInteractable = nearestInteractable;
                currentInteractable?.Highlight(true);
            }
        }
        else
        {
            ClearCurrentHighlight();
        }
    }
    void ClearCurrentHighlight()
    {
        if (currentInteractable != null)
        {
            currentInteractable?.Highlight(false);
            currentInteractable = null;
        }
    }

    // void OnDrawGizmosSelected()
    // {
    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawWireSphere(transform.position, interactionRange);
    // }
}
