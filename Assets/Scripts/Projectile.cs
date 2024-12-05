using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    private Vector3 direction;

    public Ragdoll ragdoll;
    public Rigidbody playerBody;

    private AudioSource audioSource;
    public AudioClip clip;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (clip != null) { 
            audioSource.clip = clip;
        }
    }

    public void Initialize(Rigidbody playerRB, float speed)
    {
        this.speed = speed;
        direction = (playerRB.position - transform.position).normalized;
        ragdoll = GameObject.FindWithTag("Player").GetComponent<Ragdoll>();
        playerBody = playerRB;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.CompareTag("Player")) {
            Debug.Log("Apple collided with Player");
            audioSource.Play();
            Vector3 hitDirection = (collision.gameObject.transform.position - gameObject.transform.position).normalized;
            ragdoll.RagDollModeOn();
            playerBody.AddForce(hitDirection, ForceMode.Impulse);
            Debug.Log("Apple hit player!");
        }

        if (collision.gameObject.CompareTag("Enemy")) {
            return;
        }

        Destroy(gameObject);
    }
}
