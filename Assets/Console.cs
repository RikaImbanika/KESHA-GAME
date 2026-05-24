using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Console : MonoBehaviour
{
    public TextMeshPro _consoleTmp;
    public TextMeshPro _historyTmp;
    private TextMeshPro[] _history;
    private float[] _historyTimesLeft;
    private Vector3 _basePos;
    private float _bottomOffset;
    private float _step;
    private int _headIndex;
    private int _count;
    
    public bool _opened;
    public string _input;
    private string[] _words;
    private string[] _wordsCaseSensetive;

    private float _backspaceDelay = 0.3f; // Delay before next backspace
    private float _backspaceRepeatRate = 0.05f; // Interval before backspaces
    private float _nextBackspaceActionTime;
    private bool _openedOnce;

    void Start()
    {
        StartCoroutine(LateStart());

        IEnumerator LateStart()
        {
            _count = 28;

            _history = new TextMeshPro[_count];
            _historyTimesLeft = new float[_count];

            _basePos = _consoleTmp.transform.position;

            _step = 0.5f;
            _bottomOffset = 1.25f;

            for (int i = 0; i < _count; i++)
            {
                _history[i] = Instantiate(_historyTmp);
                _history[i].transform.position = _basePos + Vector3.up * (_bottomOffset + i * _step);
                _history[i].text = $"";
                _history[i].enableWordWrapping = false;
                _history[i].gameObject.SetActive(false);
            }

            Destroy(_historyTmp);

            while (_consoleTmp.gameObject.activeInHierarchy)
            {
                _consoleTmp.gameObject.SetActive(false);
                yield return new WaitForSeconds(0.1f);
            }

            while (_consoleTmp.text.Length > 1)
            {
                _consoleTmp.text = "/";
                yield return new WaitForSeconds(0.1f);
            }

            S.Console = this;
        }
    }

    void Update()
    {
        bool skipOne;

        if (Input.GetKeyDown(KeyCode.Slash) && !Input.GetKey(KeyCode.LeftShift))
        {
            ToggleConsole("Open");
        }
        else if (_opened)
        {
            string inputThisFrame = UnityEngine.Input.inputString;

            inputThisFrame = S.TextProcessor.ConvertCyrillicToLatinLayout(inputThisFrame);

            inputThisFrame = Regex.Replace(inputThisFrame, @"[^a-zA-Z0-9 .,?;""'\-]", "");
            if (!string.IsNullOrEmpty(inputThisFrame))
            {
                _input += inputThisFrame;
                _input = Regex.Replace(_input, @" +", " ").TrimStart();
                UpdateInput();
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                Execute(_input);
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                Backspace();
                _nextBackspaceActionTime = Time.time + _backspaceDelay;
            }

            if (Input.GetKey(KeyCode.Backspace) && Time.time >= _nextBackspaceActionTime)
            {
                Backspace();
                _nextBackspaceActionTime = Time.time + _backspaceRepeatRate;
            }
        }
        else if (!_opened)
        {
            for (int i = 0; i < _count; i++)
            {
                if (_historyTimesLeft[i] > 0)
                {
                    _historyTimesLeft[i] -= Time.deltaTime;
                    if (_historyTimesLeft[i] < 0)
                    {
                        _historyTimesLeft[i] = 0;
                        _history[i].gameObject.SetActive(false);
                    }
                    else if (_historyTimesLeft[i] < 3)
                    {
                        float t = _historyTimesLeft[i] / 3;
                        Color clr = _history[i].color;
                        clr.a = Mathf.SmoothStep(0, 1, t);
                        _history[i].color = clr;
                    }
                }
            }
        }
    }

    public void ToggleConsole(string reason)
    {
        _opened = !_opened;
        ToggleConsole(reason, _opened);
    }

    public void ToggleConsole(string reason, bool value)
    {
        _opened = value;

        if (!_openedOnce && _opened)
        {
            AddMessage("Hello!", Color.yellow);
            AddMessage("To close console: /", Color.yellow);
            _openedOnce = true;
        }

        _consoleTmp.gameObject.SetActive(_opened);
        Cursor.visible = _opened;

        if (reason == "Open" || reason == "Close" && _opened)
            S.AM.Play("Kill", 1f);
        else if (reason == "Open" || reason == "Close" && !_opened)
            S.AM.Play("Inventory", 1f);
        else if (reason == "Success")
        {
            S.AM.Play("Kill", 1.1f);
            AddMessage(_input, Color.green);
        }
        else if (reason == "Error")
        {
            S.AM.Play("Not Enough Cash", 1f);
            _opened = true;
            _consoleTmp.gameObject.SetActive(_opened);

            Cursor.visible = _opened;

            AddMessage($"Error executing: \"{_input}\"", Color.red);
        }
        else if (reason == "Message")
        {
            S.AM.Play("Kill", 1f);
            _opened = true;
            _consoleTmp.gameObject.SetActive(_opened);

            Cursor.visible = _opened;

            AddMessage($"GG: {_input.Substring(1)}", Color.magenta);
        }
        else
            S.AM.Play("Wrong", 1f);

        _input = "/";
        UpdateInput();

        for (int i = 0; i < _count; i++)
        {
            if (_opened)
            {
                _history[i].gameObject.SetActive(true);
                Color clr = _history[i].color;
                clr.a = 1f;
                _history[i].color = clr;
            }
            else
                if (_historyTimesLeft[i] <= 0)
                    _history[i].gameObject.SetActive(false);
        }
    }

    void Backspace()
    {
        if (_input.Length > 1)
        {
            _input = _input.Remove(_input.Length - 1);
            UpdateInput();
        }
    }

    void Execute(string command)
    {
        command = command.Substring(1);
        command = command.Trim();
        string commandCaseSensetive = command;
        command = command.ToLower();
        Debug.Log($"EXECUTING: {command}");
        _words = command.Split(' ');
        _wordsCaseSensetive = commandCaseSensetive.Split(' ');

        if (_words[0] == "get" ||
            _words[0] == "take" ||
            _words[0] == "pick" ||
            _words[0] == "grab" ||
            _words[0] == "give" ||
            _words[0] == "getitem" ||
            _words[0] == "getitems" ||
            _words[0] == "takeitem" ||
            _words[0] == "takeitems" ||
            _words[0] == "pickitem" ||
            _words[0] == "pickitems" ||
            _words[0] == "grabitem" ||
            _words[0] == "grabitems" ||
            _words[0] == "giveitem" ||
            _words[0] == "giveme" ||
            _words[0] == "gimme" ||
            _words[0] == "givme" ||
            _words[0] == "giv" ||
            _words[0] == "giveitems")
            Get();
        else if (_words[0] == "dam" ||
            _words[0] == "damage" ||
            _words[0] == "hurt" ||
            _words[0] == "hit" ||
            _words[0] == "attack" ||
            _words[0] == "attackplayer" ||
            _words[0] == "damme" ||
            _words[0] == "damageme" ||
            _words[0] == "hurtme" ||
            _words[0] == "hitme" ||
            _words[0] == "attackme" ||
            _words[0] == "damself" ||
            _words[0] == "damageself" ||
            _words[0] == "hurtself" ||
            _words[0] == "hitself" ||
            _words[0] == "attackself" ||
            _words[0] == "damplayer" ||
            _words[0] == "damageplayer" ||
            _words[0] == "hurtplayer" ||
            _words[0] == "hitplayer" ||
            _words[0] == "attackplayer")
            Damage();
        else if (_words[0] == "heal" ||
            _words[0] == "healself" ||
            _words[0] == "healplayer" ||
            _words[0] == "healme")
            Heal();
        else if (_words[0] == "kill" ||
            _words[0] == "die" ||
            _words[0] == "killem" ||
            _words[0] == "killplayer" ||
            _words[0] == "killself" ||
            _words[0] == "killme" ||
            _words[0] == "suicide")
            Kill();
        else if (_words[0] == "save" ||
            _words[0] == "sv" ||
            _words[0] == "checkpoint" ||
            _words[0] == "cp")
            Save();
        else if (_words[0] == "push" ||
            _words[0] == "psh" ||
            _words[0] == "p" ||
            _words[0] == "pushme" ||
            _words[0] == "pshme" ||
            _words[0] == "kickme")
            Push();
        else if (_words[0] == "jump" ||
            _words[0] == "jmp" ||
            _words[0] == "pushup" ||
            _words[0] == "kickup" ||
            _words[0] == "kickmeup" ||
            _words[0] == "pushmeup" ||
            _words[0] == "jumpup" ||
            _words[0] == "fly")
            Jump();
        else if (_words[0] == "speed" ||
            _words[0] == "setspeed" ||
            _words[0] == "speedset" ||
            _words[0] == "spd" ||
            _words[0] == "s" ||
            _words[0] == "sp" ||
            _words[0] == "fast" ||
            _words[0] == "velocity")
            Speed();
        else if (_words[0] == "tp" ||
            _words[0] == "teleport" ||
            _words[0] == "goto" ||
            _words[0] == "go")
            Teleport();
        else if ((_words[0] == "scene" && _words.Length >= 2) ||
            _words[0] == "gotoscene" ||
            (_words[0] == "room" && _words.Length >= 2) ||
            _words[0] == "gotoroom" ||
            _words[0] == "goscene" ||
            _words[0] == "goroom")
            GoToScene();
        else if (_words[0] == "summon" ||
            _words[0] == "smmn" ||
            _words[0] == "sumon" ||
            _words[0] == "summn" ||
            _words[0] == "smmon" ||
            _words[0] == "smn")
            Summon();
        else if (_words[0] == "help" ||
            _words[0] == "hlp" ||
            _words[0] == "commands" ||
            _words[0] == "helpme" ||
            _words[0] == "info" ||
            _words[0] == "commandlist")
            Help();
        else if (_words[0] == "help" ||
            _words[0] == "hlp" ||
            _words[0] == "commands" ||
            _words[0] == "helpme" ||
            _words[0] == "info" ||
            _words[0] == "commandlist")
            Help();
        else if (_words[0] == "combinations" ||
            _words[0] == "combi" ||
            _words[0] == "hotkeys" ||
            _words[0] == "keys" ||
            _words[0] == "combis" ||
            _words[0] == "combies")
            Combinations();
        else if (_words[0] == "coordinates" ||
            _words[0] == "coords" ||
            _words[0] == "position" ||
            _words[0] == "mypos" ||
            _words[0] == "pos" ||
            _words[0] == "location" ||
            _words[0] == "mylocation" ||
            _words[0] == "playerlocation" ||
            _words[0] == "where" ||
            _words[0] == "whereami" ||
            _words[0] == "myposition")
            Coordinates();
        else if (_words[0] == "scene" ||
            _words[0] == "scenename" ||
            _words[0] == "room" ||
            _words[0] == "roomname" ||
            _words[0] == "currentscene" ||
            _words[0] == "currentroom" ||
            _words[0] == "currentscenename" ||
            _words[0] == "currentroomname" ||
            _words[0] == "whatscene" ||
            _words[0] == "whatroom" ||
            _words[0] == "playerscene" ||
            _words[0] == "playerroom" ||
            _words[0] == "myscene" ||
            _words[0] == "myroom")
            SceneName();
        else if (_words[0] == "snakes" ||
            _words[0] == "wherearesnakes" ||
            _words[0] == "wheresnakes" ||
            _words[0] == "snakeswhere")
            Snakes();
        else if (_words[0] == "scenes" ||
            _words[0] == "rooms" ||
            _words[0] == "allscenes" ||
            _words[0] == "allrooms" ||
            _words[0] == "sceneslist" ||
            _words[0] == "roomslist" ||
            _words[0] == "allsceneslist" ||
            _words[0] == "allroomslist" ||
            _words[0] == "scenesnames" ||
            _words[0] == "roomsnames" ||
            _words[0] == "allscenesnames" ||
            _words[0] == "allroomsnames")
            Scenes();
        else if (_words[0] == "items" ||
            _words[0] == "things" ||
            _words[0] == "allitems" ||
            _words[0] == "allthings" ||
            _words[0] == "itemslist" ||
            _words[0] == "thingslist" ||
            _words[0] == "allitemslist" ||
            _words[0] == "allthingslist" ||
            _words[0] == "itemsnames" ||
            _words[0] == "thingsnames" ||
            _words[0] == "allitemsnames" ||
            _words[0] == "allthingsnames")
            Items();
        else if (!string.IsNullOrWhiteSpace(command))
            ToggleConsole("Message");
        else
            ToggleConsole("Close");
    }

    void Items()
    {
        ToggleConsole("Success", true);

        string[] items = S.II.Names;
        for (int i = 0; i < items.Length; i++)
            items[i] = $"\"{items[i]}\"";

        string full = string.Join(", ", items);
        int maxLen = 90;
        int start = 0;

        while (start < full.Length)
        {
            if (full.Length - start <= maxLen)
            {
                AddMessage(full.Substring(start) + ".", Color.yellow);
                break;
            }

            int end = start + maxLen;
            int splitPos = full.LastIndexOf(", ", end, end - start);
            if (splitPos == -1 || splitPos <= start)
                splitPos = end;

            AddMessage(full.Substring(start, splitPos - start), Color.yellow);
            start = splitPos + 2;
        }
    }

    void Scenes()
    {
        ToggleConsole("Success", true);

        int count = SceneManager.sceneCountInBuildSettings;
        string[] names = new string[count];
        for (int i = 0; i < count; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            names[i] = $"\"{Path.GetFileNameWithoutExtension(path)}\"";
        }
        AddMessage(string.Join(", ", names) + ".", Color.yellow);
    }

    void SceneName()
    {
        ToggleConsole("Success", true);
        string sceneNameFromLoader = S.SM.LoadString("sceneName") ?? "no";
        AddMessage($"{S.Ps._currentSceneName} (from loader: {sceneNameFromLoader})", Color.yellow);
    }

    void Coordinates()
    {
        ToggleConsole("Success", true);
                
        Vector3 pos = S.Ph.transform.position;
        AddMessage($"{pos.x} {pos.y} {pos.z}", Color.yellow);
    }

    void Combinations()
    {
        Info("Combinations");
    }

    void Help()
    {
        Info("Help");
    }

    void Info(string path)
    {
        ToggleConsole("Success", true);

        TextAsset help = Resources.Load<TextAsset>($"Texts/{path}");
        string[] lines = help.text.Split('\n');
        foreach (string line in lines)
            AddMessage(line.Trim('\r'), Color.yellow);
    }

    void Snakes()
    {
        ToggleConsole("Success", true);

        string str = "";

        if (S.Backrooms == null || S.Backrooms._snakes == null)
        {
            AddMessage("Not initialised yet.", Color.yellow);
        }

        foreach (KeyValuePair<string, byte> kvp in S.Backrooms._snakes)
        {
            if (kvp.Value > 0)
            {
                string type = "Unknown";
                if (kvp.Value == 1)
                        type = "Sun";
                else if (kvp.Value == 2)
                    type = "Ice";
                else if (kvp.Value == 3)
                    type = "Nature";
                else if (kvp.Value == 4)
                    type = "Silent";
                str += $"{type} - \"{kvp.Key}\", ";
            }
        }

        str = $"{str.Remove(str.Length - 2)}.";

        AddMessage(str, Color.yellow);
    }

    void Summon()
    {
        string who;

        if (_words.Length > 1)
        {
            who = _words[1];
            int count = 1;

            if (_words.Length > 2)
            {
                int number = 1;
                bool numberNext = int.TryParse(_words[2], out number);

                if (numberNext && number > 1)
                {
                    count = number;
                    who = _words[1];
                }
                else
                {
                    int.TryParse(_words[1], out number);
                    count = number;
                    who = _words[2];
                }
            }

            who = who.ToLower();
            Transform root = S.Loader.Roots[S.Ps._currentSceneName];

            for (int i = 0; i < count; i++)
            {
                float dx = Random.Range(-0.2f, 0.2f);
                float dy = Random.Range(-0.2f, 0.2f);
                float dz = Random.Range(-0.2f, 0.2f);
                Vector3 position = S.Camera.transform.position + new Vector3(dx, dy, dz);

                if (who == "zombella")
                    Instantiate(S.Zombella, position, Quaternion.identity, root);
                else if (who == "bakalavrus" ||
                    who == "bakalavr" ||
                    who == "bakalavrian" ||
                    who == "bakalavrium" ||
                    who == "baka")
                    Instantiate(S.Bakalavrus, position, Quaternion.identity, root);
                else if (who == "ghost" ||
                    who == "ghast" ||
                    who == "gost" ||
                    who == "gast")
                    Instantiate(S.Ghost, position, Quaternion.identity, root);
                else if (who == "musculus" ||
                    who == "musculum" ||
                    who == "muskulus" ||
                    who == "muskulum")
                    Instantiate(S.Musculus, position, Quaternion.identity, root);
                else if (who == "spider" ||
                    who == "laserspider" ||
                    who == "laser_spider" ||
                    who == "spiderus")
                    Instantiate(S.Spider, position, Quaternion.identity, root);
                else if (who == "firefly" ||
                    who == "fly" ||
                    who == "gloomworm" ||
                    who == "lighter" ||
                    who == "gloomfly" ||
                    who == "lumie" ||
                    who == "glowie" ||
                    who == "glowbug" ||
                    who == "firebug" ||
                    who == "flybug" ||
                    who == "sparkfly" ||
                    who == "twinklebug" ||
                    who == "sparkfly" ||
                    who == "sparklefly" ||
                    who == "sparkle")
                    SummonFirefly(position, root);
                else
                {
                    Instantiate(S.Zombella, position, Quaternion.identity, root);
                    ToggleConsole("Error");
                    return;
                }
            }
            ToggleConsole("Success");
        }
        else
            ToggleConsole("Error");
    }

    void SummonFirefly(Vector3 pos, Transform root)
    {
        GameObject fireflySpawnerObj = Instantiate(S.FireflySpawner, pos, Quaternion.identity, root);
        FireflySpawner fireflySpawner = fireflySpawnerObj.GetComponent<FireflySpawner>();
        fireflySpawner._instant = true;
    }

    void Teleport()
    {
        float number1;
        float number2;
        float number3;

        if (_words.Length > 1)
        {
            bool numberNext = float.TryParse(_words[1], out number1);

            if (numberNext)
            {
                if (_words.Length > 2)
                {
                    numberNext = float.TryParse(_words[2], out number2);

                    if (numberNext)
                    {
                        if (_words.Length > 3)
                        {
                            numberNext = float.TryParse(_words[3], out number3);

                            if (numberNext)
                                ThreeNumbers();
                            else
                                TwoNumbers();
                        }
                        else
                            TwoNumbers();
                    }
                    else
                        OnlyOneNumber();
                }
                else
                    OnlyOneNumber();
            }
            else
                GoToScene();
        }
        else
        {
            ToggleConsole("Error");
            return;
        }

        void ThreeNumbers()
        {
            Teleport3D();
        }

        void TwoNumbers()
        {
            TeleportHorizontally();
        }

        void OnlyOneNumber()
        {
            //TO DO: Something here.
            ToggleConsole("Error");
            return;
        }

        void TeleportHorizontally()
        {
            S.Ph.transform.position = new Vector3(number1, S.Ph.transform.position.y, number2);
            ToggleConsole("Success");
        }

        void Teleport3D()
        {
            S.Ph.transform.position = new Vector3(number1, number2, number3);
            ToggleConsole("Success");
        }
    }

    void GoToScene()
    {
        string sceneName = _input.Substring(_input.IndexOf(" ") + 1);

        if (S.Loader._aliases.ContainsKey(sceneName))
            sceneName = S.Loader._aliases[sceneName];

        if (!S.Loader._map.ContainsKey(sceneName))
        {
            ToggleConsole("Error");
            return;
        }

        Vector3 forward = -S.PM.orientation.forward;
        S.Loader.GoTo(sceneName, 1, forward);
        ToggleConsole("Success");
    }

    void Get()
    {
        if (_words.Length > 1)
        {
            int count = 1;
            string itemName = "";
            bool numberNext = int.TryParse(_words[1], out count);

            if (numberNext)
            {
                itemName = _wordsCaseSensetive[2];
                S.Inventory.Take(itemName, count);
                ToggleConsole("Success");
                return;
            }
            else
            {
                itemName = _wordsCaseSensetive[1];
                if (_words.Length > 2)
                {
                    bool correct = int.TryParse(_words[2], out count);
                    if (correct)
                    {
                        S.Inventory.Take(itemName, count);
                        ToggleConsole("Success");
                        return;
                    }
                    else
                    {
                        ToggleConsole("Error");
                        return;
                    }
                }
                else
                {
                    ToggleConsole("Success");
                    S.Inventory.Take(itemName, 1);
                }
            }
        }
        else
        {
            ToggleConsole("Error");
            return;
        }
    }

    void Push()
    {
        float number1 = 100f;
        float number2 = 0f;
        float number3 = 0f;

        Vector3 forward = S.PM.orientation.forward;

        if (_words.Length > 1)
        {
            bool numberNext = float.TryParse(_words[1], out number1);

            if (numberNext)
            {
                if (_words.Length == 2)
                {
                    S.PM.Push((forward + Vector3.up) * number1 * 100f, true);
                    ToggleConsole("Success");
                    return;
                }
                else
                {
                    numberNext = float.TryParse(_words[2], out number2);

                    if (numberNext)
                    {
                        Vector3 originalVector = new Vector3(number2, 0, number1);
                        float cameraYRotation = Camera.main.transform.eulerAngles.y;
                        Quaternion horizontalRotation = Quaternion.Euler(0, cameraYRotation, 0);
                        Vector3 rotatedVector = horizontalRotation * originalVector;

                        if (_words.Length == 3)
                        {
                            TwoNumbers(rotatedVector);
                        }
                        else
                        {
                            numberNext = float.TryParse(_words[3], out number3);

                            if (numberNext)
                                ThreeNumbers(horizontalRotation);
                            else
                                TwoNumbers(rotatedVector);
                        }
                    }
                    else
                        OneNumber();
                }
            }
            else
                NoNumber();
        }
        else
            NoNumber();


        void NoNumber()
        {
            S.PM.Push((forward * 3f + Vector3.up) * 100f, true);
            ToggleConsole("Success");
            return;
        }

        void OneNumber()
        {
            S.PM.Push((forward * 3f + Vector3.up) * number1 * 100f, true);
            ToggleConsole("Success");
            return;
        }

        void TwoNumbers(Vector3 rotatedVector)
        {
            S.PM.Push(rotatedVector * 100f);
            ToggleConsole("Success");
            return;
        }

        void ThreeNumbers(Quaternion horizontalRotation)
        {
            Vector3 originalVector3 = new Vector3(number3, number2, number1);
            Vector3 rotatedVector3 = horizontalRotation * originalVector3 * 100f;

            S.PM.Push(rotatedVector3);
            ToggleConsole("Success");
            return;
        }
    }

    void Damage()
    {
        if (_words.Length < 1)
        {
            ToggleConsole("Error");
            return;
        }
        else if (_words.Length == 1)
        {
            ToggleConsole("Success");
            S.PS.Damage(10);
            return;
        }

        float amount;

        bool numberNext = float.TryParse(_words[1], out amount);

        if (numberNext && amount > 0)
        {
            ToggleConsole("Success");
            S.PS.Damage(amount);
            return;
        }
        else
        {
            ToggleConsole("Error");
            return;
        }
    }

    void Jump()
    {
        float amount = 100f;

        if (_words.Length > 1)
        {

            bool numberNext = float.TryParse(_words[1], out amount);

            if (numberNext)
            {
                S.PM.Push(new Vector3(0, amount * 100f, 0));
                ToggleConsole("Success");
                return;
            }
            else
            {
                S.PM.Push(new Vector3(0, amount, 0));
                ToggleConsole("Success");
                return;
            }
        }
        else
        {
            S.PM.Push(new Vector3(0, amount, 0));
            ToggleConsole("Success");
            return;
        }
    }

    void Save()
    {
        S.SM.SaveCurrentToLast();
        Transform goTransform = S.Canvas.transform.Find("Game saved");
        GameObject go = goTransform.gameObject;
        go.SetActive(true);

        StartCoroutine(HideAfterDelay(go, 1f));

        IEnumerator HideAfterDelay(GameObject go2, float delay)
        {
            yield return new WaitForSeconds(delay);
            go2.SetActive(false);
        }

        ToggleConsole("Success");
    }

    void Kill()
    {
        ToggleConsole("Success", false);
        S.PS.Damage(100f);
    }

    void Heal()
    {
        if (_words.Length <= 1)
        {
            S.PS.Heal(100f);
            ToggleConsole("Success");
        }

        float amount = 10f;

        bool numberNext = float.TryParse(_words[1], out amount);

        if (numberNext && amount > 0)
        {
            S.PS.Heal(amount);
            ToggleConsole("Success");
        }
        else if (numberNext && amount < 0)
        {
            S.Ps.Damage(-amount);
            ToggleConsole("Success");
        }
        else
        {
            ToggleConsole("Error");
            return;
        }
    }

    void Speed()
    {
        ToggleConsole("Success");

        // if (_words.Length > 1)
        // {
        //     float n1 = 1;
        //     bool numberNext = float.TryParse(_words[1], out n1);

        //     if (numberNext)
        //     {
        //         S.PM._cheatSpeed = n1;
        //         ToggleConsole("Success");
        //         return;
        //     }
        //     else
        //     {
        //         S.PM._cheatSpeed = 1f;
        //         ToggleConsole("Error");
        //         return;
        //     }
        // }
        // else
        // {
        //     S.PM._cheatSpeed = 1f;
        //     ToggleConsole("Error");
        //     return;
        // }
    }

    void UpdateInput()
    {
        _consoleTmp.text = _input;
    }

    public void AddMessage(string message, Color clr)
    {
        const int limit = 90;
        const int countLimit = 5;
        const int minLineLength = limit - 15;

        string remaining = message;

        for (int i = 0; i < countLimit; i++)
        {
            if (remaining.Length <= limit)
            {
                AddMessage0(remaining, clr);
                break;
            }

            if (i == countLimit - 1)
            {
                string truncated = remaining.Substring(0, limit - 3);
                AddMessage0(truncated + "...", clr);
                break;
            }

            string line = remaining.Substring(0, limit);
            int lastSpace = line.LastIndexOf(' ');

            if (lastSpace >= 0 && lastSpace >= minLineLength)
            {
                string part = remaining.Substring(0, lastSpace);
                AddMessage0(part, clr);

                remaining = remaining.Substring(lastSpace + 1).TrimStart();
            }
            else
            {
                AddMessage0(line, clr);
                remaining = remaining.Substring(limit).TrimStart();
            }
        }
    }

    void AddMessage0(string message, Color clr)
    {
        _history[_headIndex].text = message;
        _history[_headIndex].color = clr;

        _historyTimesLeft[_headIndex] = 6f;
        _history[_headIndex].gameObject.SetActive(true);

        _history[_headIndex].transform.position = _basePos + Vector3.up * _bottomOffset;

        int idx = (_headIndex + 1) % _count;
        for (int i = 1; i < _count; i++)
        {
            _history[idx].transform.position = _basePos + Vector3.up * (_bottomOffset + i * _step);
            idx = (idx + 1) % _count;
        }

        _headIndex--;

        if (_headIndex < 0)
            _headIndex = _count - 1;
    }
}