using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemP : MonoBehaviour
{
    public string _id;
    public string _name;
    public int _count;
    public bool _unnatural;
    public bool _locked;

    AllFather _allFather;
    string _sceneName;
    public GameObject _obj;

    void Start()
    {
        //Если не поставить айди здесь то где тогда????
        //Хотя признаю, айди внутри условий можно задать

        _obj = gameObject;

        _allFather = GameObject.Find("AllFather").GetComponent<AllFather>();
        _sceneName = gameObject.scene.name;
        Save s = _allFather.Load(_id);

        bool hasNoId = (s._position == Vector3.zero); //ПОЧЕМУ?
        //Так ну по идее позиция 0 означает что такой записи нет
        //Тоесть объект не был сохранён, и ничего более это не значит
        //Объекты новые, если не были сохранены
        //Объект имеет айди, если был загружен через лоадер
        //Если объект был брошен то ещё не успел сохраниться в момент броска?
        //Короче если айди не был задан - значит его нужно задать?
        //А потом уже чекнуть наличие записи
        //В ином случае айди уже задан

        if (hasNoId)
        {
            //Если айди не был задан создателем то логично задать его здесь
            _id = "" + transform.position.x + transform.position.y + transform.position.z;
            //Раз айди задали здесь, то самое время перезагрузить сохранение с заданным айди
            s = _allFather.Load(_id);
            //Отсутсвие ЭТОЙ строки было багом, ведь да?
            //По каким то причинам эта строка сдивнула все предметы...
            //КАК ПОНЯТЬ???????????????????????
            //Ну вот кто мог сдвинуть предметы?
        }

        //_unnatural задаётся при броске либо в лоадере,
        //тоесть оно задано всегда когда нужно

        if (_unnatural)
        {
            //Ну допустим, тут айди было присвоено и сохранено ранее
            //А загружено оно было именно моим лоадером
            //Который и задал айди, значит тут всё норм должно быть
            //А вот при броске ну будет записи и позицию загружать нет смысла
            transform.position = s._position;
            transform.rotation = s._rotation;
            _count = s._count;
            _name = s._name;
        }
        else
        {
            //Если айди не было то ону же задасться в первом пункте
            //Если айди был то тогда вообще пофиг           
            //А вот уже после задания айди необходимо перезагружать сохранение

            //Давай рассуждать логически
            //Записи в сохранении есть но там нет одного параметра
            //Там нет unnatural. Так его и не должно быть, проблемы не вижу.
            //Ааааа там нет параметра _destroyed А ОН ДОЛЖЕН БЫТЬ
            if (s._destroyed)
                Destroy();
            else if(!string.IsNullOrEmpty(s._name))
            {
                //По идее позицию ставим только если была такая запись
                transform.position = s._position;
                transform.rotation = s._rotation;
            }
            //Туплю, объекты двигаются, независимо от наличия или отсутствия записи,
            //Вот он баг, пофикшено.
        }
        StartCoroutine(Saver(UnityEngine.Random.Range(9f, 11f)));
    }

    IEnumerator Saver(float t)
    {
        while (true)
        {
            yield return new WaitForSeconds(t);
            if (transform.position.y < -700)
                Destroy();
            Save();
        }
    }

    public void ToggleLock(bool locked)
    {
        _locked = locked;
    }

    public void Save()
    {
        Save s = new Save();
        s._name = _name;
        s._count = _count;
        s._position = transform.position;
        s._rotation = transform.rotation;
        s._scene = _sceneName;
        s._unnatural = _unnatural;
        s._locked = _locked;
        _allFather.Save(_id, s);

        Save s2 = _allFather.Load(_sceneName);
        if (s2._strings == null)
            s2._strings = new List<string>();
        if (!s2._strings.Contains(_id))
            s2._strings.Add(_id);
        _allFather.Save(_sceneName, s2);
    }

    public void Destroy()
    {
        Debug.Log($"Destroyed {_name}");
        if (_unnatural)
            _allFather.Destroy(_id); //КАК ПОНЯТЬ?
        else
        {
            //ТАК НУ ОБЪЕКТЫ же натуральные, значит ЗНАЧИТ должно бить сюда
            Save s = new Save();
            s._destroyed = true; //НУ ВОТ СКАЗАНО ЖЕ записать что удалено
            _allFather.Save(_id, s);
            Debug.Log($"Была сделана записть что объект по айди {_id} был удалён.");
        }
        Destroy(gameObject);
    }
}