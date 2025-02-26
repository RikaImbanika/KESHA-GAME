using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsGun : MonoBehaviour
{
    private GameObject _cameraHolder;
    public GameObject _ray;
    public int _sparklesCount;
    private Transform _root;
    public float _rayRight;
    public float _rayDown;

    void Start()
    {
        StartCoroutine(Start0());

        IEnumerator Start0()
        {
            S.IsGun = this;

            while (S.AllFather == null)
            {
                yield return new WaitForSeconds(0.1f);
                Debug.Log("IsGun waiting for S.AllFater");
            }

            _root = S.AllFather.transform;
            _ray.transform.SetParent(_root);
        }
    }

    public void Fire()
    {
        if (S.Inventory.CountOfItem("Ammo") > 0)
        {
            S.Inventory.Remove("Ammo", 1);

            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            Ray ray = Camera.main.ScreenPointToRay(screenCenter);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                S.AudioManager.Play("plasma", 1);

                NoSpots noSpots = hit.collider.gameObject.GetComponent<NoSpots>();
                if (noSpots == null)
                {
                    GameObject spot = Instantiate(S.Spot);
                    spot.transform.position = hit.point;
                    spot.transform.rotation = Quaternion.LookRotation(hit.normal);
                    spot.transform.Rotate(0f, 0f, UnityEngine.Random.Range(-180, 180));

                    float x = spot.transform.localScale.x;// / hit.collider.gameObject.transform.localScale.x;
                    float y = spot.transform.localScale.y;// / hit.collider.gameObject.transform.localScale.y;
                    float z = spot.transform.localScale.z;// / hit.collider.gameObject.transform.localScale.z;
                    float f = 2.5f;
                    spot.transform.localScale = new Vector3(x * f, y * f, z * f);
                    spot.transform.SetParent(hit.collider.gameObject.transform);

                    S.AllFather._spots.Add(spot);
                    if (S.AllFather._spots.Count > 300)
                    {
                        Destroy(S.AllFather._spots[0]);
                        S.AllFather._spots.RemoveAt(0);
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
                                S.AudioManager.Play("arfa", 1);
                            }
                        }
                        else
                        {
                            Stamp stamp = hit.collider.gameObject.GetComponent<Stamp>();
                            if (stamp != null)
                            {
                                stamp.Unlock();
                                S.AudioManager.Play("arfa", 1);
                            }
                        }
                    }
                }

                Vector3 from = S.Camera.transform.position + S.Camera.transform.right * _rayRight - S.Camera.transform.up * _rayDown;

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
                    GameObject sparkle = Instantiate(S.Sparkle);
                    sparkle.transform.position = hit.point;
                    sparkle.transform.rotation = Quaternion.LookRotation(hit.normal);
                    sparkle.GetComponent<IsSparkle>()._active = true;
                }
            }
        }
        else
            S.AudioManager.Play("noAmmo", 1);
    }

    public void TakeRedCrystal()
    {
        S.Inventory.Take("RedCrystal", 1);
    }

    public float Length(Vector3 v)
    {
        return MathF.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z); //
    }
}
