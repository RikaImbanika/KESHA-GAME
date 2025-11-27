using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Drone : MonoBehaviour
{
	public string _type;
    public string _type2;
    public List<GameObject> _lasers;
    public float _rotationSpeed;
    public float _yRot;
    public float _zRot;
    public float _fireDelay;
    public int _fireCount;
    public float _currentFireDelay;
    public SnakeHead _head;
    
    private int _layerMask;
    private Optimiser _opti;
    
    public void Start()
    {
        _opti = new Optimiser(gameObject.scene.name);
        
        _layerMask = 1 << LayerMask.NameToLayer("Player") |
                         1 << LayerMask.NameToLayer("Static") |
                         1 << LayerMask.NameToLayer("Enemies") |
                         1 << LayerMask.NameToLayer("Items") |
                         1 << LayerMask.NameToLayer("Default");
    }

    public void Init(string type, string type2)
    {
        _type = type;
        _type2 = type2;
        
        if (type == "6lasers")
        {
            _lasers = new List<GameObject>(6);

            for (int i = 0; i < 6; i++)
                _lasers.Add(Instantiate(S.RedLaser));
            _rotationSpeed = 2f + UnityEngine.Random.Range(0, 4f);
        }
        else if (type == "4lasers")
        {
            _lasers = new List<GameObject>(4);

            for (int i = 0; i < 4; i++)
                _lasers.Add(Instantiate(S.RedLaser));
            _rotationSpeed = 2f + UnityEngine.Random.Range(0, 4f);
        }
        else if (type == "3lasers")
        {
            _lasers = new List<GameObject>(3);

            for (int i = 0; i < 3; i++)
                _lasers.Add(Instantiate(S.RedLaser));

            _rotationSpeed = 2f + UnityEngine.Random.Range(0, 4f);
        }
        else if (type == "2lasers")
        {
            _lasers = new List<GameObject>(2);

            for (int i = 0; i < 2; i++)
                _lasers.Add(Instantiate(S.RedLaser));
            _rotationSpeed = 2f + UnityEngine.Random.Range(0, 4f);
        }
        else if (type == "rotated2lasers")
        {
            _lasers = new List<GameObject>(2);

            for (int i = 0; i < 2; i++)
                _lasers.Add(Instantiate(S.RedLaser));
            _yRot = UnityEngine.Random.Range(-55f, 55f);
            _zRot = UnityEngine.Random.Range(-90, 90f);
        }
        else if (type == "flat2lasers")
        {
            _lasers = new List<GameObject>(2);

            for (int i = 0; i < 2; i++)
                _lasers.Add(Instantiate(S.RedLaser));
        }

        if (type2 == "1")
        {
            _fireCount = 1;
            _fireDelay = UnityEngine.Random.Range(0.6f, 5f);
        }
        else if (type2 == "2")
        {
            _fireCount = 2;
            _fireDelay = UnityEngine.Random.Range(0.6f, 5f);
        }
        else if (type2 == "3")
        {
            _fireCount = 2;
            _fireDelay = UnityEngine.Random.Range(0.6f, 5f);
        }
        else if (type2 == "sniper")
        {
            _fireCount = 1;
            _fireDelay = UnityEngine.Random.Range(1f, 3.5f);
        }

        if (UnityEngine.Random.value > 0.5f)
            _rotationSpeed = -_rotationSpeed;
    }

    public void Work(float walk)
	{
		if (_type == "6lasers")
		{           
            for (int i = 0; i < 6; i++)
            {
                Vector3 originalVector = transform.right;
                var rotated = RotateAroundLocalAxis(originalVector, 60 * i + _rotationSpeed * walk, "z");
                Laser(rotated, i);
            }
        }
        else if (_type == "4lasers")
        {
            for (int i = 0; i < 4; i++)
            {
                Vector3 originalVector = transform.right;
                var rotated = RotateAroundLocalAxis(originalVector, 90 * i + _rotationSpeed * walk, "z");
                Laser(rotated, i);
            }
        }
        else if (_type == "3lasers")
        {
            for (int i = 0; i < 3; i++)
            {
                Vector3 originalVector = transform.right;
                var rotated = RotateAroundLocalAxis(originalVector, 120 * i + _rotationSpeed * walk, "z");
                Laser(rotated, i);
            }
        }
        else if (_type == "2lasers")
        {
            for (int i = 0; i < 2; i++)
            {
                Vector3 originalVector = transform.right;
                var rotated = RotateAroundLocalAxis(originalVector, 180 * i + _rotationSpeed * walk, "z");
                Laser(rotated, i);
            }
        }
        else if (_type == "flat2lasers")
        {
            for (int i = 0; i < 2; i++)
            {
                Vector3 originalVector = transform.right;
                var rotated = RotateAroundLocalAxis(originalVector, 180 * i, "z");
                Laser(rotated, i);
            }
        }
        else if (_type == "rotated2lasers")
        {
            for (int i = 0; i < 2; i++)
            {
                Vector3 originalVector = transform.right;
                var rotated = RotateAroundLocalAxis(originalVector, 180 * i * _yRot, "y");
                rotated = RotateAroundLocalAxis(rotated, 180 * i * _zRot, "z");
                Laser(rotated, i);
            }
        }
    }

    public void Update()
    {
        if(_opti.Optimise(transform._pos))
        {
            Do();
            _opti.Reset();
        }
        
        void Do()
        {
            _currentFireDelay -= _opti.DeltaTime;
            if (_head._seePlayer)
            {
                if (_currentFireDelay < 0)
                {
                    _currentFireDelay = _fireDelay;
    
                    if (_type2 == "1")
                        Shoot2(1);
                    else if (_type2 == "2")
                        Shoot2(2);
                    else if (_type2 == "3")
                        Shoot2(3);
                    else if (_type2 == "sniper")
                    Shoot1(S.Ph.transform.position - transform.position);
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
            Quaternion rq = Quaternion.LookRotation(dir);

            GameObject bullet = Instantiate(_theBullet);
            bullet.transform.position = transform.position + dir;
            bullet.transform.rotation = rq;
            EnemyBullet eb = bullet.GetComponent<EnemyBullet>();
            eb._active = true;
            eb._speed = 30;
            Destroy(bullet, 15);

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
        Vector3 localVector = transform.InverseTransformDirection(vector);
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        if (axis == "y")
            rotation = Quaternion.Euler(0, angle, 0);
        else if (axis == "x")
            rotation = Quaternion.Euler(angle, 0, 0);
        Vector3 rotated = rotation * localVector;
        return transform.TransformDirection(rotated);
    }

    void Laser(Vector3 direction, int i)
    {
        Vector3 from = transform.position;

        Ray ray = new Ray(from, direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _layerMask))
        {
            _lasers[i].transform.rotation = Quaternion.LookRotation(hit.point - from);

            float desiredLength = (hit.point - from).magnitude;

            var scale = _lasers[i].transform.localScale;
            float factor = 0.65ff; //
            scale.z = desiredLength * factor;
            _lasers[i].transform.localScale = scale;

            GameObject go = hit.collider.gameObject;
            if (go.CompareTag("Player"))
            {
                S.PS.Damage(0.7f);
            }
        }
    }
}
