using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidBodyController : MonoBehaviour
{
    private Vector3 PlayerMovementInput;
    private Rigidbody Playerbody;

    public float Speed;
    public float Jumpforce;

    // Start is called before the first frame update
    void Start()
    {
        Playerbody = GetComponent<Rigidbody>();
        GetComponent<Rigidbody>().isKinematic = false;
    }

    private void MovePlayer() {
        Vector3 MoveVector = transform.TransformDirection(PlayerMovementInput) * Speed;
        Playerbody.velocity = new Vector3(MoveVector.x, Playerbody.velocity.y, MoveVector.z);

        if (Input.GetKeyDown(KeyCode.Space)) {
            Playerbody.AddForce(Vector3.up * Jumpforce, ForceMode.Impulse);
        }
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
        Debug.Log(PlayerMovementInput);
    }

    void FixedUpdate()
    {
        MovePlayer();
    }
}
