using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Jump : Agent
{
    [SerializeField]
    float _jumpSpeed = 8.0f;

    private GameObject _agent;
    private Rigidbody _agentRigidbody;
    private bool _canJump = true;
    private Vector3 _startingPoint;

    public override void Initialize()
    {
        _agent = this.gameObject;
        _agentRigidbody = this.gameObject.GetComponent<Rigidbody>();
        _startingPoint = this.gameObject.transform.position;
    }

    public override void OnEpisodeBegin()
    {
        ResetPlayer();
        // EnemyMove.resetPosition();
        Debug.Log("Start");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(_agent.transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (actions.DiscreteActions[0] == 1)
            agentJump();

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var cActions = actionsOut.ContinuousActions; 
        cActions[0] = Input.GetAxis("Jump");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "obstacle")
        {
            SetReward(-1.0f);
            EndEpisode();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "rewardzone")
        {
            SetReward(1.0f);
            _canJump = true;
        }      
    }

    private void agentJump()
    {
        if (_canJump)
        {
            _canJump = false;
            _agentRigidbody.AddForce(Vector2.up * _jumpSpeed, ForceMode.Impulse);
        }
    }

    private void ResetPlayer()
    {
        _agent.transform.position = _startingPoint;
        _canJump = true;
    }
}
