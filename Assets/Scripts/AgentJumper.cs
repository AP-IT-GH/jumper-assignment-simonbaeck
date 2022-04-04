using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AgentJumper : Agent
{
    Rigidbody rBody;
    public bool isGrounded = true;
    public float jumpHeigth = 10f;

    public override void Initialize()
    {
        rBody = GetComponent<Rigidbody>();
    }
    public override void OnEpisodeBegin()
    {
        isGrounded = true;
        transform.localPosition = new Vector3(7.5f, 0.5f, 0f);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        if (isGrounded && actionBuffers.DiscreteActions[0] == 1)
        {
            isGrounded = false;
            rBody.velocity = new Vector3(0, jumpHeigth, 0);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsout = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.Space) == true)
            discreteActionsout[0] = 1;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Hit Obstacle");
            Destroy(other.gameObject);
            EndEpisode();
        }
    }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("AgentZone"))
        {
            Debug.Log("On ground");
            isGrounded = true;
        }
    }
}
