using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;

public class DropScript : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    public GameObject icicle;
    public bool dropped;
    private Vector3 startPosition;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = icicle.transform.position;
    }
    void Awake()
    {
        _rigidbody = icicle.GetComponent<Rigidbody2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !dropped)
        {//When an icicle is triggered to drop it will reset after a short time
            _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
            Invoke("resetIcicle", 0.9f);
            dropped = true;
        }
    }
    void resetIcicle()
    {
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        icicle.transform.position = startPosition;
        dropped = false;
    }
}
