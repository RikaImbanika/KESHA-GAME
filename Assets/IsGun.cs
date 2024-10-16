using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsGun : MonoBehaviour
{
    public AllFather _allFather;
    private GameObject _cameraHolder;
    private GameObject _mainCamera;
    public GameObject _ray;
    private GameObject _spot;
    private GameObject _sparkle;
    public int _sparklesCount;
    private Transform _root;
    public float _rayRight;
    public float _rayDown;
    private AudioManager _audioManager;
    private Canvas _canvas;
    private Inventory _inventory;

    void Start()
    {
        _allFather = GameObject.Find("AllFather").GetComponent<AllFather>();

        _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");       

        _root = _allFather.transform;
        _sparkle = _allFather._sparkle;
        _spot = _allFather._spot;
        _ray.transform.SetParent(_root);

        _canvas = GameObject.FindObjectOfType<Canvas>();
        _inventory = _canvas.GetComponent<Inventory>();
    }

    public void Fire()
    {
        if (_audioManager == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag("AudioManager");
            _audioManager = go.GetComponent<AudioManager>();
        }

        if (_inventory.CountOfItem("Ammo") > 0)
        {
            _inventory.Remove("Ammo", 1);

            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            Ray ray = Camera.main.ScreenPointToRay(screenCenter);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                _audioManager.Play("plasma", 1);

                NoSpots noSpots = hit.collider.gameObject.GetComponent<NoSpots>();
                if (noSpots == null)
                {
                    GameObject spot = Instantiate(_spot);
                    spot.transform.position = hit.point;
                    spot.transform.rotation = Quaternion.LookRotation(hit.normal);
                    spot.transform.Rotate(0f, 0f, UnityEngine.Random.Range(-180, 180));

                    float x = spot.transform.localScale.x;// / hit.collider.gameObject.transform.localScale.x;
                    float y = spot.transform.localScale.y;// / hit.collider.gameObject.transform.localScale.y;
                    float z = spot.transform.localScale.z;// / hit.collider.gameObject.transform.localScale.z;
                    float f = 2.5f;
                    spot.transform.localScale = new Vector3(x * f, y * f, z * f);
                    spot.transform.SetParent(hit.collider.gameObject.transform);

                    _allFather._spots.Add(spot);
                    if (_allFather._spots.Count > 300)
                    {
                        Destroy(_allFather._spots[0]);
                        _allFather._spots.RemoveAt(0);
                    }
                }

                Zombie zombie = hit.collider.gameObject.GetComponent<Zombie>();
                if (zombie != null)
                {
                    if (zombie._health <= 15)
                        TakeRedCrystal();
                    zombie.Damage(15);
                }
                else
                {
                    Spider spider = hit.collider.gameObject.GetComponent<Spider>();
                    if (spider != null)
                    {
                        if (spider._health <= 15)
                            TakeRedCrystal();
                        spider.Damage(15);
                    }
                    else
                    {
                        Door door = hit.collider.gameObject.GetComponent<Door>();
                        if (door != null)
                        {
                            if (door._locked)
                            {
                                door.Unlock();
                                _audioManager.Play("arfa", 1);
                            }
                        }
                        else
                        {
                            Stamp stamp = hit.collider.gameObject.GetComponent<Stamp>();
                            if (stamp != null)
                            {
                                stamp.Unlock();
                                _audioManager.Play("arfa", 1);
                            }
                        }
                    }
                }

                Vector3 from = _mainCamera.transform.position + _mainCamera.transform.right * _rayRight - _mainCamera.transform.up * _rayDown;

                GameObject ray2 = Instantiate(_ray);
                ray2.GetComponent<IsRay>()._active = true;
                float rx = (hit.point.x + from.x) / 2;
                float ry = (hit.point.y + from.y) / 2;
                float rz = (hit.point.z + from.z) / 2;
                ray2.transform.position = new Vector3(rx, ry, rz);
                ray2.transform.rotation = Quaternion.LookRotation(hit.point - from);

                float desiredLength = (hit.point - from).magnitude;
                ray2.transform.SetParent(null, true);

                var scale = ray2.transform.localScale;
                float factor = 100;
                scale.z = desiredLength * factor;
                ray2.transform.localScale = scale;

                for (int i = 0; i < _sparklesCount; i++)
                {
                    GameObject sparkle = Instantiate(_sparkle);
                    sparkle.transform.position = hit.point;
                    sparkle.transform.rotation = Quaternion.LookRotation(hit.normal);
                    sparkle.GetComponent<IsSparkle>()._active = true;
                }
            }
        }
        else
            _audioManager.Play("noAmmo", 1);
    }

    public void TakeRedCrystal()
    {
        Item _item = new Item();
        _item._name = "redCrystal"; //////////////////////////
        _item._count = 1;
        _allFather._inventory.Take(_item);
    }

    public float Length(Vector3 v)
    {
        return MathF.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z); //
    }
}
