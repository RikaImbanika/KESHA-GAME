using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstZombie2 : MonoBehaviour
{
    AllFather _allFather;
    public Zombie _zombie;
    bool _summoned;
    AudioManager _audioManager;
    PlayerStorage _playerStorage;
    bool _inBattle;

    void Start()
    {
        _playerStorage = GameObject.Find("Player").GetComponent<PlayerStorage>();
        _allFather = GameObject.Find("AllFather").GetComponent<AllFather>();
        GameObject go = GameObject.FindGameObjectWithTag("AudioManager");
        _audioManager = go.GetComponent<AudioManager>();

        StartCoroutine(DelayCoroutine());

        IEnumerator DelayCoroutine()
        {
            yield return new WaitForSeconds(0.5f);

            if (!_allFather.Load("gunWasBuyed")._bool || !_allFather.Load("ammoWasBuyed")._bool)
            {
                _zombie.gameObject.SetActive(false);
                _zombie._active = false;
            }
        }
    }

    void Update()
    {
        if (_allFather.Load("gunWasBuyed")._bool && _allFather.Load("ammoWasBuyed")._bool)
        {
            if (_playerStorage._currentSceneName == "Hall" && !_zombie._dead)
                StartBattle();
            else
                StopBattle();

            void StartBattle()
            {
                if (!_inBattle)
                {
                    _inBattle = true;

                    if (!_allFather.Load("firstZombieInHall")._bool)
                        StartCoroutine(DelayCoroutine());
                    else
                    {
                        _audioManager.PlayFirstZombieTheme();
                        Spawn();
                    }

                    IEnumerator DelayCoroutine()
                    {
                        _allFather.Save("firstZombieInHall", new Save(true));

                        yield return new WaitForSeconds(2f);

                        _audioManager.Play("Door");
                        if (!_zombie.gameObject.activeInHierarchy)
                            _zombie.gameObject.SetActive(true);

                        yield return new WaitForSeconds(0.3f);

                        _audioManager.PlayFirstZombieTheme();
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
                    _audioManager.StopFirstZombieTheme();
                }
            }
        }
    }
}
