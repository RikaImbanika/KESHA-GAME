using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public int _sparklesCount;
    private Transform _root;
    public float _rayRight = 2f;
    public float _rayDown = 0.5f;
    public float _rayWidth = 6f;
    private int _layerMask;

    void Start()
    {
        StartCoroutine(Start0());

        IEnumerator Start0()
        {
            S.Gun = this;

            _layerMask = ~(1 << LayerMask.NameToLayer("Player") |
                1 << LayerMask.NameToLayer("Particles") |
                1 << LayerMask.NameToLayer("Inv Walls, Triggers") |
                1 << LayerMask.NameToLayer("Transparent Items"));

            while (S.AllFather == null)
            {
                yield return new WaitForSeconds(0.1f);
                Debug.Log("IsGun waiting for S.AllFater");
            }

            _root = S.AllFather.transform;
        }
    }

    public void Fire()
    {
        if (S.Inventory.CountOfItem("Ammo") > 0)
        {
            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            Ray ray = Camera.main.ScreenPointToRay(screenCenter);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1001f, _layerMask))
            {
                S.Inventory.Remove("Ammo", 1);

                S.AudioManager.Play("plasma", 1);

                NoSpots noSpots = hit.collider.gameObject.GetComponent<NoSpots>();
                if (noSpots == null)
                {
                    GameObject spot = Instantiate(S.Spot);
                    spot.transform.position = hit.point;
                    spot.transform.rotation = Quaternion.LookRotation(hit.normal);
                    spot.transform.Rotate(0f, 0f, UnityEngine.Random.Range(-180, 180));

                    float x = spot.transform.localScale.x;
                    float y = spot.transform.localScale.y;
                    float z = spot.transform.localScale.z;
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
                    zombie.Damage(15);
                    float healthPercent = MathF.Max(zombie._health / zombie._maxHealth, 0);
                    S.LifeBars.Show(zombie._visibleName, healthPercent);
                }
                else
                {
                    Spider spider = hit.collider.gameObject.GetComponent<Spider>();
                    if (spider != null)
                    {
                        spider.Damage(15);
                        float healthPercent = MathF.Max(spider._health / spider._maxHealth, 0);
                        S.LifeBars.Show("The Spider", healthPercent);
                    }
                    else
                    {
                        Door door = hit.collider.gameObject.GetComponent<Door>();
                        if (door != null)
                        {
                            if (door._locked)
                                door._stamp.Unlock();
                        }
                        else
                        {
                            Stamp stamp = hit.collider.gameObject.GetComponent<Stamp>();
                            if (stamp != null)
                                stamp.Unlock();
                        }
                    }
                }

                Vector3 from = S.Camera.transform.position + S.Camera.transform.right * _rayRight - S.Camera.transform.up * _rayDown;

                float desiredLength = (hit.point - from).magnitude;

                GameObject ray2 = Instantiate(S.BlueRay);

                ray2.transform.SetParent(null, true);

                var scale = ray2.transform.localScale;
                float factor = 1; //
                scale.z = desiredLength * factor;
                scale.y *= _rayWidth;
                scale.x *= _rayWidth;
                ray2.transform.localScale = scale;

                float rx = from.x;
                float ry = from.y;
                float rz = from.z;
                ray2.transform.position = new Vector3(rx, ry, rz);
                ray2.transform.rotation = Quaternion.LookRotation(hit.point - from);

                for (int i = 0; i < _sparklesCount; i++)
                {
                    GameObject sparkle = Instantiate(S.PlayerSparkle);
                    sparkle.transform.position = hit.point;
                    sparkle.transform.rotation = Quaternion.LookRotation(hit.normal);

                    int gg2 = S.RND.Next(25);

                    float decay = 1f;

                    if (gg2 == 0)
                        decay = 0.5f;
                    else if (gg2 == 1)
                        decay = 2f;
                    else if (gg2 == 2)
                        decay = 0.25f;
                    else if (gg2 == 3)
                        decay = 4f;

                    sparkle.GetComponent<Sparkle3>()._minimisingSpeedCoef = decay;
                }

                Instantiate(S.BlueHitPoint, hit.point, Quaternion.identity);

                int gg = S.RND.Next(10);

                if (gg == 0)
                {
                    GameObject sparkle = Instantiate(S.BlueHeavySparkle);
                    sparkle.transform.position = hit.point;
                    sparkle.transform.rotation = Quaternion.LookRotation(hit.normal);
                }
            }
            else
                S.AudioManager.Play("noAmmo", 2);
        }
        else
            S.AudioManager.Play("noAmmo", 1);
    }

    public float Length(Vector3 v)
    {
        return MathF.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z); //
    }
}
