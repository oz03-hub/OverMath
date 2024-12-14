using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float DestroyTime = 2.0f;
    private Vector3 offset = new Vector3(0, 1, 0);
    void Start()
    {
        Destroy(gameObject, DestroyTime);
        transform.localPosition += offset;
    }
}
