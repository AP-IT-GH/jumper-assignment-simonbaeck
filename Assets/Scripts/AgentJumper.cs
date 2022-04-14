using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using TMPro;

public class AgentJumper : Agent
{
    public bool isGrounded = true;
    public float jumpHeight = 6.5f;
    public TextMeshProUGUI scoreText;

    private GameObject _agentObject;
    private Rigidbody _agentRBody;
    private GameObject _obstacleObject;
    private Rigidbody _obstacleRBody;
    private Vector3 _obstacleSpawnpoint;
    private float _obstacleSpeed;
    private GameObject _triggerObject;
    private bool _jumpedOnTime = false;
    private bool _hasJumped = false;

    private System.Random _random = new System.Random();

    public override void Initialize()
    {
        _agentObject = this.gameObject;
        _agentRBody = this.GetComponent<Rigidbody>();
        _obstacleObject = GameObject.FindGameObjectWithTag("Obstacle");
        _obstacleRBody = GameObject.FindGameObjectWithTag("Obstacle").GetComponent<Rigidbody>();
        _obstacleSpawnpoint = GameObject.FindGameObjectWithTag("Obstacle").transform.localPosition;
        _triggerObject = GameObject.FindGameObjectWithTag("Trigger");
    }
    public override void OnEpisodeBegin()
    {
        SetReward(0.0f);
        _jumpedOnTime = false;
        _hasJumped = false;
        isGrounded = true;
        _agentObject.transform.localPosition = new Vector3(7.5f, 0.5f, 0f);
        _obstacleObject.transform.localPosition = _obstacleSpawnpoint;
        _obstacleSpeed = _random.Next(4, 8);
    }

    private void Update()
    {
        _obstacleRBody.velocity = new Vector3((float)_obstacleSpeed, 0.0f, 0.0f);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        print(actionBuffers.DiscreteActions[0]);
        if (isGrounded && actionBuffers.DiscreteActions[0] == 1)
        {
            float distanceToTarget = Vector3.Distance(this.transform.localPosition, _obstacleObject.transform.localPosition);
            if (distanceToTarget < 3.25f && !_hasJumped)
            {
                SetReward(0.5f);
                scoreText.text = GetCumulativeReward().ToString();
                scoreText.color = Color.green;
                _jumpedOnTime = true;
                _hasJumped = true;
                isGrounded = false;
                _agentRBody.velocity = new Vector3(0, jumpHeight, 0);
            }
            else if (!_hasJumped)
            {
                SetReward(-0.5f);
                scoreText.text = GetCumulativeReward().ToString();
                scoreText.color = Color.red;
                isGrounded = false;
                _jumpedOnTime = false;
                _hasJumped = true;
                _agentRBody.velocity = new Vector3(0, jumpHeight, 0);
            }
        }

        if (_obstacleObject.transform.localPosition.x > 13.5)
        {
            _obstacleObject.transform.localPosition = _obstacleSpawnpoint;
            EndEpisode();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(_agentObject.transform.localPosition);
        sensor.AddObservation(_obstacleObject.transform.localPosition);
        sensor.AddObservation(_triggerObject.transform.localPosition);
        // sensor.AddObservation(_hasJumped);
        // sensor.AddObservation(_jumpedOnTime);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;
        if(Input.GetAxis("Jump") == 1)
        {
            discreteActions[0] = 1;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            if (!_jumpedOnTime && _hasJumped)
            {
                SetReward(-0.5f);
                scoreText.text = GetCumulativeReward().ToString();
                scoreText.color = Color.red;
                EndEpisode();
            }
            else if (!_jumpedOnTime && !_hasJumped)
            {
                SetReward(-1.0f);
                scoreText.text = GetCumulativeReward().ToString();
                scoreText.color = Color.red;
                EndEpisode();
            }
        }

        if (other.gameObject.CompareTag("Trigger") && !isGrounded)
        {
            if (_jumpedOnTime)
            {
                SetReward(0.5f);
                scoreText.text = GetCumulativeReward().ToString();
                scoreText.color = Color.green;
            } 
            else 
            {
                SetReward(-0.5f);
                scoreText.text = GetCumulativeReward().ToString();
                scoreText.color = Color.red;
            }
            
        }
    }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("AgentZone"))
        {
            isGrounded = true;
        }
    }
}
