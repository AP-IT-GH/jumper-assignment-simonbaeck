using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float moveSpeed = 3.5f;
    public float endPoint = 9.5f;

    private void FixedUpdate()
    {
        this.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);

        if (this.transform.position.x > endPoint)
        {
            Destroy(this.gameObject);
        }
    }


}
