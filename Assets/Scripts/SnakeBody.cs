using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SnakeBody : MonoBehaviour
{
    public GameObject _ball; // For full system following path
    public GameObject _ballInBall; // Rotating around Right axis while moving
    public GameObject _ballInBallInBall; // For initial random rotation
    public GameObject _ballInBallInBallInBall; // For easy parenting
    public GameObject _droneObj;
    public Drone _drone;
    public NavMeshObstacle _obstacle;
    public Renderer _renderer;
    private int _number;
    private float _R;
    private float _startRotation;
    public int _ballIndex;

    public Renderer Renderer
    {
        get { return _renderer; }
        set { _renderer = value; }
    }

    public NavMeshObstacle Obstacle
    {
        get { return _obstacle; }
        set { _obstacle = value; }
    }
    public int Number
    {
        get { return _number; }
        set { _number = value; }
    }

    public float R
    {
        get { return _R; }
        set { _R = value; }
    }

    public GameObject Ball
    {
        get { return _ball; }
        set { _ball = value; }
    }

    public GameObject BallInBall
    {
        get { return _ballInBall; }
        set { _ballInBall = value; }
    }

    public GameObject BallInBallInBall
    {
        get { return _ballInBallInBall; }
        set { _ballInBallInBall = value; }
    }

    public GameObject BallInBallInBallInBall
    {
        get { return _ballInBallInBallInBall; }
        set { _ballInBallInBallInBall = value; }
    }

    public GameObject DroneObj
    {
        get { return _droneObj; }
        set { _droneObj = value; }
    }

    public Drone Drone
    {
        get { return _drone; }
        set { _drone = value; }
    }

    public float StartRotation
    {
        get { return _startRotation; }
        set { _startRotation = value; }
    }

    public void Die(GameObject prefab, Transform root)
    {
        Transform oldTransform = _ballInBallInBallInBall.transform;
        Vector3 worldPos = oldTransform.position;
        Quaternion worldRot = oldTransform.rotation;
        Vector3 worldScale = oldTransform.lossyScale;
        GameObject newInstance = Instantiate(prefab, worldPos, worldRot);
        newInstance.transform.localScale = worldScale;
        newInstance.transform.SetParent(root, true);

        ItemP itemP = newInstance.GetComponent<ItemP>();
        itemP._forLoader = true;

        Destroy(_ballInBallInBallInBall);

        Destroy(gameObject);
    }
}