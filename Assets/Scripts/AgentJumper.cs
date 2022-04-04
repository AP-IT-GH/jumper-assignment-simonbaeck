using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AgentJumper : Agent
{
    Rigidbody _rBody;
    public bool isGrounded = true;
    public float jumpHeigth = 10f;

    public override void Initialize()
    {
        _rBody = GetComponent<Rigidbody>();
    }
    public override void OnEpisodeBegin()
    {
        SetReward(1.0f);
        isGrounded = true;
        transform.localPosition = new Vector3(7.5f, 0.5f, 0f);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        if (isGrounded && actionBuffers.DiscreteActions[0] == 1)
        {
            isGrounded = false;
            _rBody.velocity = new Vector3(0, jumpHeigth, 0);
            AddReward(-0.001f);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsout = actionsOut.DiscreteActions;
        if(Input.GetAxis("Jump") == 1)
        {
            discreteActionsout[0] = 1;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Hit Obstacle");
            Destroy(other.gameObject);
            SetReward(-1.0f);
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
