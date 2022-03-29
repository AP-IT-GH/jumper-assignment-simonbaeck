using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public float distanceToGround = 0.55f;
    public float jumpSpeed = 6.0f;
    private Rigidbody _rBody;

    // Start is called before the first frame update
    void Start()
    {
        this._rBody = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Space) && isGrounded())
        {
            Vector3 jumpVelocity = new Vector3(0, jumpSpeed, 0);
            _rBody.velocity = _rBody.velocity + jumpVelocity;
        }
    }

    bool isGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, distanceToGround);
    }
}
