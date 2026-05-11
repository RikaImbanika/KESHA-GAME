using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class Console : MonoBehaviour
{
    public TextMeshPro _tmp;
    public bool _opened;
    public string _input;
    private string[] _words;
    private string[] _wordsCaseSensetive;

    private float _backspaceDelay = 0.3f;    // Delay before next backspace
    private float _backspaceRepeatRate = 0.05f; // Interval before backspaces
    private float _nextBackspaceActionTime;

    void Start()
    {
        S.Console = this;

        StartCoroutine(FIXER());

        IEnumerator FIXER()
        {
            while (_tmp.gameObject.activeInHierarchy)
            {
                _tmp.gameObject.SetActive(false);
                yield return new WaitForSeconds(0.1f);
            }

            while (_tmp.text.Length > 1)
            {
                _tmp.text = "/";
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Slash))
        {
            ToggleConsole("Open");
        }

        if (_opened)
        {
            string inputThisFrame = UnityEngine.Input.inputString;
            inputThisFrame = Regex.Replace(inputThisFrame, @"[^a-zA-Z0-9 \-]", "");

            if (!string.IsNullOrEmpty(inputThisFrame))
            {
                _input += inputThisFrame;
                _input = Regex.Replace(_input, @" +", " ").TrimStart();
                UpdateDisplay();
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
    }

    public void ToggleConsole(string reason)
    {
        _opened = !_opened;
        ToggleConsole(reason, _opened);
    }

    public void ToggleConsole(string reason, bool value)
    {
        _opened = value;

        _tmp.gameObject.SetActive(_opened);
        Cursor.visible = _opened;
        _input = "/";
        UpdateDisplay();

        if (reason == "Open" || reason == "Close" && _opened)
            S.AM.Play("Kill", 1f);
        else if (reason == "Open" || reason == "Close" && !_opened)
            S.AM.Play("Inventory", 1f);
        else if (reason == "Success")
            S.AM.Play("Kill", 1.1f);
        else if (reason == "Error")
        {
            S.AM.Play("Not Enough Cash", 1f);
            _opened = !_opened;
            _tmp.gameObject.SetActive(_opened);
            Cursor.visible = _opened;
        }
        else
            S.AM.Play("Wrong", 1f);
    }

    void Backspace()
    {
        if (_input.Length > 1)
        {
            _input = _input.Remove(_input.Length - 1);
            UpdateDisplay();
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
        else if (_words[0] == "scene" ||
            _words[0] == "gotoscene" ||
            _words[0] == "room" ||
            _words[0] == "gotoroom" ||
            _words[0] == "goscene" ||
            _words[0] == "goroom")
            GoToScene();
        else
            ToggleConsole("Error");
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
        if (_words.Length <= 1)
        {
            ToggleConsole("Error");
            return;
        }

        float amount = 10f;

        bool numberNext = float.TryParse(_words[1], out amount);

        if (numberNext)
        {
            ToggleConsole("Success", false);
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
            ToggleConsole("Error");
            return;
        }

        float amount = 10f;

        bool numberNext = float.TryParse(_words[1], out amount);

        if (numberNext)
        {
            S.PS.Heal(amount);
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

    void UpdateDisplay()
    {
        //Debug.Log(_input);
        _tmp.text = _input;
    }
}