using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public enum Level
{
    One, Two 
}

public class CrossRoadJumper : Agent
{
    public float jumpHeight = 6.5f;
    public TextMeshProUGUI score;
    
    #region GameObjects
    private GameObject _horizontalObject;
    private GameObject _horizontalEnd;
    private GameObject _verticalObject;
    private GameObject _verticalEnd;
    private GameObject _agentObject;
    #endregion

    private Level level;

    private Vector3 _hTargetSpawn;
    private Vector3 _vTargetSpawn;

    private bool _isGrounded = true;
    private bool _hasJumped = false;
    private bool _jumpedInTime;

    private float _obstacleSpeed;

    private System.Random _random = new System.Random();

    public override void Initialize()
    {
        _horizontalObject = GameObject.FindGameObjectWithTag("HorizontalTarget");
        _horizontalEnd = GameObject.FindGameObjectWithTag("HorizontalEnd");
        _verticalObject = GameObject.FindGameObjectWithTag("VerticalTarget");
        _verticalEnd = GameObject.FindGameObjectWithTag("VerticalEnd");
        _hTargetSpawn = _horizontalObject.transform.localPosition;
        _vTargetSpawn = _verticalObject.transform.localPosition;
        _agentObject = GameObject.FindGameObjectWithTag("Agent");
        level = Level.One;
    }

    public override void OnEpisodeBegin()
    {
        level = Level.One;
        _isGrounded = true;
        _hasJumped = false;
        _horizontalObject.transform.localPosition = _hTargetSpawn;
        _verticalObject.transform.localPosition= _vTargetSpawn;
        _obstacleSpeed = _random.Next(8, 12);

        _horizontalObject.GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, (float)_obstacleSpeed);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(_agentObject.transform.localPosition);
        sensor.AddObservation(_horizontalObject.transform.localPosition);
        sensor.AddObservation(_verticalObject.transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        float distanceToTarget;
        if (level == Level.One)
            distanceToTarget = Vector3.Distance(this.transform.localPosition, _horizontalObject.transform.localPosition);
        else
            distanceToTarget = Vector3.Distance(this.transform.localPosition, _verticalObject.transform.localPosition);

        if (_isGrounded && actionBuffers.DiscreteActions[0] == 1)
        {
            if (distanceToTarget < 3.25f && !_hasJumped)
            {
                SetReward(0.25f);
                score.text = GetCumulativeReward().ToString();
                score.color = Color.green;
                _jumpedInTime = true;
                _hasJumped = true;
                _isGrounded = false;
                _agentObject.GetComponent<Rigidbody>().velocity = new Vector3(0, jumpHeight, 0);
            }
            else if (!_hasJumped)
            {
                SetReward(-0.25f);
                score.text = GetCumulativeReward().ToString();
                score.color = Color.red;
                _jumpedInTime = false;
                _hasJumped = true;
                _isGrounded = false;
                _agentObject.GetComponent<Rigidbody>().velocity = new Vector3(0, jumpHeight, 0);
            }
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;
        if (Input.GetAxis("Jump") == 1)
        {
            discreteActions[0] = 1;
        }
    }

    void Update()
    {     
        if (level == Level.One)
        {
            if (_horizontalObject.transform.localPosition.z > _horizontalEnd.transform.localPosition.z)
            {
                _horizontalObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                _horizontalObject.transform.localPosition = _hTargetSpawn;
                _hasJumped = false;
                level = Level.Two;
            } 
            else
            {
                _horizontalObject.GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, (float)_obstacleSpeed);
            }
        }
        else if (level == Level.Two)
        {
            if (_verticalObject.transform.localPosition.x > _verticalEnd.transform.localPosition.x)
            {
                _verticalObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                EndEpisode();
            } 
            else
            {
                _verticalObject.GetComponent<Rigidbody>().velocity = new Vector3((float)_obstacleSpeed, 0.0f, 0.0f);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Trigger") && !_isGrounded)
        {
            if (_jumpedInTime)
            {
                SetReward(0.25f);
                score.text = GetCumulativeReward().ToString();
                score.color = Color.green;
            }
            else
            {
                SetReward(-0.25f);
                score.text = GetCumulativeReward().ToString();
                score.color = Color.red;
            }

        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("TrainingGround"))
        {
            _isGrounded = true;
        }

        if (other.gameObject == _horizontalObject || other.gameObject == _verticalObject)
        {
            SetReward(-1.0f);
            _horizontalObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            _verticalObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            score.text = GetCumulativeReward().ToString();
            score.color = Color.red;
            level = Level.One;
            EndEpisode();
        }
    }
}
