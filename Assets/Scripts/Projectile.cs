using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    private Vector3 direction;

    public Ragdoll ragdoll;
    public Rigidbody playerBody;

    public AudioSource audioSource;
    public AudioClip clip;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Initialize(Rigidbody playerRB, float speed, AudioClip hitClip)
    {
        audioSource = GetComponent<AudioSource>();
        clip = hitClip;
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
        Debug.Log($"[APPLE] Collided with: {collision.gameObject.name}");
        if (collision.gameObject.CompareTag("Player")) {
            Debug.Log("Apple collided with Player");
            audioSource.PlayOneShot(clip);
            Vector3 hitDirection = (collision.gameObject.transform.position - gameObject.transform.position).normalized;
            ragdoll.RagDollModeOn();
            playerBody.AddForce(hitDirection, ForceMode.Impulse);
            Debug.Log("Apple hit player!");
            
            GetComponent<SphereCollider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;

            Destroy(gameObject, clip.length);
            return;
        }

        if (collision.gameObject.CompareTag("Enemy")) {
            return;
        }

        Destroy(gameObject);
    }
}
