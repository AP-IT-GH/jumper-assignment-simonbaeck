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
    private GameObject _obstacleTriggerObject;
    private GameObject _collectableObject;
    private Rigidbody _collectableRBody;
    private Vector3 _collectableSpawnpoint;
    private float _collectableSpeed;
    private GameObject _collectableTriggerObject;
    private bool _jumpedOnTime = false;
    private bool _hasJumped = false;
    private int SelectObject;

    private System.Random _random = new System.Random();

    public override void Initialize()
    {
        _agentObject = this.gameObject;
        _agentRBody = this.GetComponent<Rigidbody>();
        _obstacleObject = GameObject.FindGameObjectWithTag("Obstacle");
        _obstacleRBody = GameObject.FindGameObjectWithTag("Obstacle").GetComponent<Rigidbody>();
        _obstacleSpawnpoint = GameObject.FindGameObjectWithTag("Obstacle").transform.localPosition;
        _obstacleTriggerObject = GameObject.FindGameObjectWithTag("ObstacleTrigger"); 
        _obstacleObject = GameObject.FindGameObjectWithTag("Obstacle");
        _collectableRBody = GameObject.FindGameObjectWithTag("Collectable").GetComponent<Rigidbody>();
        _collectableSpawnpoint = GameObject.FindGameObjectWithTag("Collectable").transform.localPosition;
        _collectableTriggerObject = GameObject.FindGameObjectWithTag("CollectableTrigger");
        _collectableTriggerObject = GameObject.FindGameObjectWithTag("CollectableTrigger");
        _collectableObject = GameObject.FindGameObjectWithTag("Collectable");
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
        _collectableObject.transform.localPosition = _collectableSpawnpoint;
        _collectableSpeed = _random.Next(4, 8);
        SelectObject = _random.Next(1, 3);
        _obstacleObject.SetActive(false);
        _collectableObject.SetActive(false);
    }

    private void Update()
    {
        if (SelectObject == 1)
        {
            _obstacleObject.SetActive(true);
            _obstacleRBody.velocity = new Vector3((float)_obstacleSpeed, 0.0f, 0.0f);
            _collectableRBody.velocity = Vector3.zero;
        }   
        else if (SelectObject == 2)
        {
            _collectableObject.SetActive(true);
            _collectableRBody.velocity = new Vector3((float)_obstacleSpeed, 0.0f, 0.0f);
            _obstacleRBody.velocity = Vector3.zero;
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        if (isGrounded && actionBuffers.DiscreteActions[0] == 1)
        {
            float distanceToObstacle = Vector3.Distance(this.transform.localPosition, _obstacleObject.transform.localPosition);
            float distanceToCollectable = Vector3.Distance(this.transform.localPosition, _collectableObject.transform.localPosition);
            if(SelectObject == 1) 
            {
                if (distanceToObstacle < 3.25f && !_hasJumped)
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
            else if(SelectObject == 2)
            {
                if (distanceToCollectable < 4.25f && !_hasJumped)
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
        }

        if (_obstacleObject.transform.localPosition.x > 13.5 || _collectableObject.transform.localPosition.x > 13.5)
        {
            _collectableObject.transform.localPosition = _collectableSpawnpoint;
            _obstacleObject.transform.localPosition = _obstacleSpawnpoint;
            EndEpisode();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(_agentObject.transform.localPosition);
        sensor.AddObservation(_obstacleObject.transform.localPosition);
        sensor.AddObservation(_obstacleTriggerObject.transform.localPosition);
        sensor.AddObservation(_collectableObject.transform.localPosition);
        sensor.AddObservation(_collectableTriggerObject.transform.localPosition);
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
        if (other.gameObject.CompareTag("Obstacle") || other.gameObject.CompareTag("CollectableTrigger"))
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

        if (!isGrounded && (other.gameObject.CompareTag("ObstacleTrigger")|| other.gameObject.CompareTag("Collectable")))
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
