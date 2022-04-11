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

    private System.Random _random = new System.Random();

    public override void Initialize()
    {
        _agentObject = this.gameObject;
        _agentRBody = this.GetComponent<Rigidbody>();
        _obstacleObject = GameObject.FindGameObjectWithTag("Obstacle");
        _obstacleRBody = GameObject.FindGameObjectWithTag("Obstacle").GetComponent<Rigidbody>();
        _obstacleSpawnpoint = GameObject.FindGameObjectWithTag("Obstacle").transform.localPosition;
    }
    public override void OnEpisodeBegin()
    {
        SetReward(0.0f);
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
        if (isGrounded && actionBuffers.ContinuousActions[0] == 1)
        {
            isGrounded = false;
            _agentRBody.velocity = new Vector3(0, jumpHeight, 0);
            scoreText.text = GetCumulativeReward().ToString();
            scoreText.color = Color.red;
            AddReward(0.5f);
        }

        if (_obstacleObject.transform.localPosition.x > 13)
        {
            _obstacleObject.transform.localPosition = _obstacleSpawnpoint;
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continousActionsout = actionsOut.ContinuousActions;
        if(Input.GetAxis("Jump") == 1)
        {
            continousActionsout[0] = 1;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Hit Obstacle");
            scoreText.text = GetCumulativeReward().ToString();
            scoreText.color = Color.red;
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
