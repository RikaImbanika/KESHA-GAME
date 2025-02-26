using System.Collections;
using System.Collections.Generic;
using UnityEditor.TextCore.Text;
using UnityEngine;

public class FirstZombie2 : MonoBehaviour
{
    public Zombie _zombie;
    bool _summoned;
    bool _inBattle;

    void Start()
    {
        StartCoroutine(DelayCoroutine());

        IEnumerator DelayCoroutine()
        {
            yield return new WaitForSeconds(0.5f);

            if (!(S.SM.LoadBool("gunWasBuyed") ?? false) || !(S.SM.LoadBool("ammoWasBuyed") ?? false))
            {
                _zombie.gameObject.SetActive(false);
                _zombie._active = false;
            }
        }
    }

    void Update()
    {
        if ((S.SM.LoadBool("gunWasBuyed") ?? false) && (S.SM.LoadBool("ammoWasBuyed") ?? false))
        {
            if (S.PS._currentSceneName == "Hall" && !_zombie._dead)
                StartBattle();
            else
                StopBattle();

            void StartBattle()
            {
                if (!_inBattle)
                {
                    _inBattle = true;

                    if (!(S.SM.LoadBool("firstZombieInHall") ?? false))
                        StartCoroutine(DelayCoroutine());
                    else
                    {
                        S.AudioManager.PlayFirstZombieTheme();
                        Spawn();
                    }

                    IEnumerator DelayCoroutine()
                    {
                        S.SM.Save("firstZombieInHall", true);

                        yield return new WaitForSeconds(2f);

                        S.AudioManager.Play("Door");
                        if (!_zombie.gameObject.activeInHierarchy)
                            _zombie.gameObject.SetActive(true);

                        yield return new WaitForSeconds(0.3f);

                        S.AudioManager.PlayFirstZombieTheme();
                        Spawn();
                    }

                    void Spawn()
                    {
                        _zombie.gameObject.SetActive(true);
                        _zombie._active = true;
                        _zombie._followPlayer = true;
                        _zombie._realSpeed = 100;
                        _zombie._stopSpeed = 0.06f;
                        _zombie._ani.Rebind();

                        GameObject newZombie = Instantiate(_zombie.gameObject, _zombie.transform.position, _zombie.transform.rotation);
                        
                        _zombie.transform.position = _zombie.transform.position + new Vector3(-1000, -1000, -1000);

                        DelayCoroutine2();

                        IEnumerator DelayCoroutine2()
                        {
                            var gg = _zombie;
                            _zombie = newZombie.GetComponent<Zombie>();
                            yield return new WaitForSeconds(5f);
                            Destroy(gg.gameObject);                           
                        }
                    }
                }
            }

            void StopBattle()
            {
                if (_inBattle)
                {
                    _inBattle = false;
                    S.AudioManager.StopFirstZombieTheme();
                }
            }
        }
    }
}
