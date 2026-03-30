using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Drone : MonoBehaviour
{
    //That drone is for snakes.
    public string _type;
    public string _type2;
    public List<GameObject> _lasers;
    public List<GameObject> _points;
    public GameObject _sparklePrefab;
    public float _rotationSpeed;
    public float _yRot;
    public float _zRot;
    public float _fireDelay;
    public int _fireCount;
    public float _currentFireDelay;
    public SnakeHead _head;
    private string _color;
    private int _layerMaskForLasers;
    private Optimiser _opti;
    private float _damageMultiplier;
    
    public void Start()
    {
        _opti = new Optimiser(gameObject.scene.name);
        
        _layerMaskForLasers = 1 << LayerMask.NameToLayer("Player") |
                         1 << LayerMask.NameToLayer("Static") |
                         1 << LayerMask.NameToLayer("Enemies") |
                         1 << LayerMask.NameToLayer("Items") |
                         1 << LayerMask.NameToLayer("Default");
    }

    public void Init(string type, string type2, string color, float _damageMultiplier)
    {
        _type = type;
        _type2 = type2;
        _color = color;

        if (color == "red")
            _sparklePrefab = S.RedSparkle;
        else if (color == "green")
            _sparklePrefab = S.GreenSparkle;
        else if (color == "blue")
            _sparklePrefab = S.BlueSparkle;
        else if (color == "purple")
            _sparklePrefab = S.PurpleSparkle;

        if (type == "6lasers")
        {
            _lasers = new List<GameObject>(6);

            for (int i = 0; i < 6; i++)
            {
                Add();
            }
            _rotationSpeed = 2f + UnityEngine.Random.Range(0, 4f);
        }
        else if (type == "4lasers")
        {
            _lasers = new List<GameObject>(4);

            for (int i = 0; i < 4; i++)
                Add();

            _rotationSpeed = 2f + UnityEngine.Random.Range(0, 4f);
        }
        else if (type == "3lasers")
        {
            _lasers = new List<GameObject>(3);

            for (int i = 0; i < 3; i++)
                Add();

            _rotationSpeed = 2f + UnityEngine.Random.Range(0, 4f);
        }
        else if (type == "2lasers")
        {
            _lasers = new List<GameObject>(2);

            for (int i = 0; i < 2; i++)
                Add();

            _rotationSpeed = 2f + UnityEngine.Random.Range(0, 4f);
        }
        else if (type == "flat2lasers")
        {
            _lasers = new List<GameObject>(2);

            for (int i = 0; i < 2; i++)
                Add();
        }
        
        void Add()
        {
            if (_color == "red")
            {
                _lasers.Add(Instantiate(S.RedLaser, transform.position, transform.rotation, transform));
                _points.Add(Instantiate(S.RedPoint, transform.position, transform.rotation, transform));
            }
            else if (_color == "blue")
            {
                _lasers.Add(Instantiate(S.BlueLaser, transform.position, transform.rotation, transform));
                _points.Add(Instantiate(S.BluePoint, transform.position, transform.rotation, transform));
            }
            else if (_color == "green")
            {
                _lasers.Add(Instantiate(S.GreenLaser, transform.position, transform.rotation, transform));
                _points.Add(Instantiate(S.GreenPoint, transform.position, transform.rotation, transform));
            }
            else if (_color == "purple")
            {
                _lasers.Add(Instantiate(S.PurpleLaser, transform.position, transform.rotation, transform));
                _points.Add(Instantiate(S.PurplePoint, transform.position, transform.rotation, transform));
            }
        }

        if (type2 == "1")
        {
            _fireCount = 1;
            _fireDelay = UnityEngine.Random.Range(0.6f, 5f);
        }
        else if (type2 == "2")
        {
            _fireCount = 2;
            _fireDelay = UnityEngine.Random.Range(0.7f, 6f);
        }
        else if (type2 == "3")
        {
            _fireCount = 3;
            _fireDelay = UnityEngine.Random.Range(0.8f, 7f);
        }
        else if (type2 == "sniper")
        {
            _fireCount = 1;
            _fireDelay = UnityEngine.Random.Range(1f, 3.5f);
        }

        if (UnityEngine.Random.value > 0.5f)
            _rotationSpeed = -_rotationSpeed;
    }

    public void Work(float walk, float deltaWalk)
	{
		if (_type == "6lasers")
		{           
            for (int i = 0; i < 6; i++)
            {
                Vector3 originalVector = transform.right;
                var rotated = RotateAroundLocalAxis(originalVector, 60 * i + _rotationSpeed * walk, "z");
                Laser(rotated, i, deltaWalk);
            }
        }
        else if (_type == "4lasers")
        {
            for (int i = 0; i < 4; i++)
            {
                Vector3 originalVector = transform.right;
                var rotated = RotateAroundLocalAxis(originalVector, 90 * i + _rotationSpeed * walk, "z");
                Laser(rotated, i, deltaWalk);
            }
        }
        else if (_type == "3lasers")
        {
            for (int i = 0; i < 3; i++)
            {
                Vector3 originalVector = transform.right;
                var rotated = RotateAroundLocalAxis(originalVector, 120 * i + _rotationSpeed * walk, "z");
                Laser(rotated, i, deltaWalk);
            }
        }
        else if (_type == "2lasers")
        {
            for (int i = 0; i < 2; i++)
            {
                Vector3 originalVector = transform.right;
                var rotated = RotateAroundLocalAxis(originalVector, 180 * i + _rotationSpeed * walk, "z");
                Laser(rotated, i, deltaWalk);
            }
        }
        else if (_type == "flat2lasers")
        {
            for (int i = 0; i < 2; i++)
            {
                Vector3 originalVector = transform.right;
                var rotated = RotateAroundLocalAxis(originalVector, 180 * i, "z");
                Laser(rotated, i, deltaWalk);
            }
        }
    }

    public void Update()
    {
        if(_opti.Optimise(transform.position))
        {
            Do();
            _opti.Reset();
        }
        
        void Do()
        {
            _currentFireDelay -= _opti.DeltaTime;
            
            if (_currentFireDelay < 0)
            {
                if (SeePlayer())
                {
                    _currentFireDelay = _fireDelay;
    
                    if (_type2 == "1")
                        Shoot2(1);
                    else if (_type2 == "2")
                        Shoot2(2);
                    else if (_type2 == "3")
                        Shoot2(3);
                    else if (_type2 == "sniper")
                        Shoot1(S.PlayerTarget(_head._sceneName) - transform.position);
                }
            }
        }

        void Shoot2(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 originalVector = transform.right;
                var rotated = RotateAroundLocalAxis(originalVector, UnityEngine.Random.Range(-60, 60), "y");
                rotated = RotateAroundLocalAxis(rotated, UnityEngine.Random.Range(-180, 180), "z");

                Shoot1(rotated);
            }
        }

        void Shoot1(UnityEngine.Vector3 dir)
        {
            Quaternion rotation = Quaternion.LookRotation(dir);
            GameObject fireball = null;

            if (_color == "red")
                fireball = Instantiate(S.FireballRed, transform.position + dir.normalized, rotation, S.Loader.Roots[_head._sceneName]);
            else if (_color == "blue")
                fireball = Instantiate(S.FireballBlue, transform.position + dir.normalized, rotation, S.Loader.Roots[_head._sceneName]);
            else if (_color == "green")
                fireball = Instantiate(S.FireballGreen, transform.position + dir.normalized, rotation, S.Loader.Roots[_head._sceneName]);
            else if (_color == "purple")
                fireball = Instantiate(S.FireballPurple, transform.position + dir.normalized, rotation, S.Loader.Roots[_head._sceneName]);

            Fireball eb = fireball.GetComponent<Fireball>();
            eb._active = true;
            eb._speed = 30;
            eb._damage = 5 * _damageMultiplier;
            Destroy(fireball, 15);

            AudioSource shot = Instantiate(S.Shot);
            shot.transform.position = transform.position;
            shot.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            float distance = (transform.position - S.Camera.transform.position).magnitude;
            shot.volume = MathF.Min(0.5f, 60 / (distance * distance));
            shot.Play();
            Destroy(shot, 5);
        }
    }

    Vector3 RotateAroundLocalAxis(Vector3 vector, float angle, string axis)
    {
        Vector3 localAxis;
        switch (axis.ToLower())
        {
            case "x": localAxis = transform.right; break;
            case "y": localAxis = transform.up; break;
            case "z": localAxis = transform.forward; break;
            default:
                Debug.LogWarning($"Unknown axis: {axis}. Using forward (Z) axis.");
                localAxis = transform.forward;
                break;
        }

        Quaternion rotation = Quaternion.AngleAxis(angle, localAxis);
        return rotation * vector;
    }

    void Laser(Vector3 direction, int i, float deltaWalk)
    {
        Vector3 from = transform.position;

        Ray ray = new Ray(from, direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _layerMaskForLasers))
        {
            _lasers[i].transform.rotation = Quaternion.LookRotation(hit.point - from);

            float len = (hit.point - from).magnitude;
            var scale = _lasers[i].transform.localScale;
            scale.z = len;
            _lasers[i].transform.localScale = scale;
            _points[i].transform.position = hit.point;
            _points[i].transform.rotation = S.RandRot.Get();

            GameObject go = hit.collider.gameObject;
            if (go.CompareTag("Player"))
            {
                S.PS.Damage(0.7f * _damageMultiplier);
            }

            if (S.Ps._currentSceneName == _head._sceneName || S.FakePlayerScene == _head._sceneName)
            {
                int period = (int)(60f * 1f / deltaWalk);
                if (S.RND.Next(0, period) == 0)
                {
                    GameObject sparkle = Instantiate(_sparklePrefab);
                    sparkle.transform.position = hit.point;
                    sparkle.transform.rotation = Quaternion.LookRotation(hit.normal);
                } /////////////
            }
        }
    }

    bool SeePlayer()
    {
        return _head._seePlayer && (_head._sceneName == S.Ps._currentSceneName || _head._sceneName == S.FakePlayerScene);
    }
}