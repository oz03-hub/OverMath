using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    public GameObject rig;
    public Animator animator;
    public AudioClip[] recoverClips;
    public AudioSource audioSource;

    public void RagDollModeOn() {
        Debug.Log("RagDoll ON");
        GetComponent<CharacterController>().enabled = false;
        GetComponent<PlayerController>().enabled = false;
        GetComponent<TrailRenderer>().enabled = false;

        animator.enabled = false;
        foreach (Collider collider in ragdollbits)
        {
            collider.enabled = true;
        }

        foreach (Rigidbody rigidb in rigidbodies)
        {
            rigidb.isKinematic = false;
        }

        //GetComponent<Rigidbody>().isKinematic = false;
        Invoke(nameof(RagDollModeOff), 2f);
    }

    public void RagDollModeOff() {
        Debug.Log("RagDoll OFF");
        if (Random.Range(0, 10) > 3)
        {
            Debug.Log("Playing Audio For recovery");
            int randIndx = Random.Range(0, recoverClips.Length);
            AudioClip clip = recoverClips[randIndx];
            audioSource.PlayOneShot(clip);
        }
        transform.position = rigidbodies[0].position;
        GetComponent<CharacterController>().enabled = true;
        GetComponent<PlayerController>().enabled = true;
        animator.enabled = true;
        foreach (Collider collider in ragdollbits) {
            collider.enabled = false;
        }

        foreach (Rigidbody rigidb in rigidbodies) {
            rigidb.isKinematic = true;
        }

        //GetComponent<Rigidbody>().isKinematic = true;
    }

    Collider[] ragdollbits;
    Rigidbody[] rigidbodies;
    void GetRagDollBits() {
        ragdollbits = rig.GetComponentsInChildren<Collider>();
        rigidbodies= rig.GetComponentsInChildren<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        GetRagDollBits();
        RagDollModeOff();
    }
    void Update()
    {
        
    }
}
