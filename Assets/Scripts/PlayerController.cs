using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Animator animation_controller;
    private CharacterController character_controller;
    public Vector3 movement_direction;
    public float walking_velocity;
    public Text text;
    public float speed;
    private MovementState current_state;

    private enum MovementState {
        Walking,
        Idle,
        Jumping,
    };

    // Start is called before the first frame update
    void Start()
    {
        animation_controller = GetComponent<Animator>();
        character_controller = GetComponent<CharacterController>();
        movement_direction = new Vector3(0.0f, 0.0f, 0.0f);
        walking_velocity = 2.5f;
        speed = 0.0f;
    }

    void UpdateVelocity() {
        switch (this.current_state) {
            case MovementState.Walking:
                this.speed = walking_velocity; break;
            case MovementState.Idle:
                this.speed = 0.0f; break;
        }
    }

    void SetState(MovementState state) {
        this.current_state = state;
        this.animation_controller.SetBool("Walking", state==MovementState.Walking);
        if (state == MovementState.Jumping) {
            this.animation_controller.SetTrigger("Jumping");
        }
    }

    void CheckState() {
        if (Input.GetKey(KeyCode.W))
        {
            if (character_controller.isGrounded && Input.GetKey(KeyCode.Space))
            {
                SetState(MovementState.Jumping);
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
        UpdateVelocity();

        if (Input.GetKey(KeyCode.A)) {
            transform.Rotate(0.0f, -0.7f, 0.0f);
        }

        if (Input.GetKey(KeyCode.D)) {
            transform.Rotate(0.0f, 0.7f, 0.0f);
        }

        float xdirection = Mathf.Sin(Mathf.Deg2Rad * transform.rotation.eulerAngles.y);
        float ydirection = 0.0f; // Handle jump
        float zdirection = Mathf.Cos(Mathf.Deg2Rad * transform.rotation.eulerAngles.y);
        movement_direction = new Vector3(xdirection, ydirection, zdirection);
        character_controller.Move(movement_direction * speed * Time.deltaTime);
    }
}
