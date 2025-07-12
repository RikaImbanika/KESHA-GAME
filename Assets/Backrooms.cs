using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Backrooms : MonoBehaviour
{
    System.Random _rnd;

    List<string> _allRoomsList;

    //Room BR 5 should be at least 3 rooms from start
    //Room BR 8 should be maximally far
    //Rooms not lead to themselves +
    //No too small circles +
    //Everything connected by one net +

    void Start()
    {
        StartCoroutine(Yep());

        IEnumerator Yep()
        {
            _rnd = new System.Random();

            while (true)
            {
                yield return new WaitForSeconds(0.5f);

                try
                {
                    int delme = _rnd.Next(S.Loader._rooms.Count);
                    break;
                }
                catch (Exception ex)
                {
                    Debug.Log($"Backrooms are waiting for _rooms... {ex.Message}");
                    continue;
                }
            }

            yield return new WaitForSeconds(1);

            FillAllRoomsList();

            yield return new WaitForSeconds(3);

            for (int i = 0; i < 100; i++)
                yield return SwitchOneDoor(i);
        }
    }

    IEnumerator SwitchOneDoor(int n)
    {
        Debug.Log($"n {n}");

        int i = 0;

        for (; i < 100; i++)
        {
            int id1 = -1;
            int id2 = -1;

            while (true)
            {
                id1 = _rnd.Next(S.Loader._rooms.Count);
                if (S.Loader._rooms.ElementAt(id1).Key.Contains("BR") || S.Loader._rooms.ElementAt(id1).Key.Contains("Hall"))
                    break;
            }

            while (true)
            {
                id2 = _rnd.Next(S.Loader._rooms.Count);
                if (id2 != id1) //Should this stroke be here?
                    if (S.Loader._rooms.ElementAt(id2).Key.Contains("BR") || S.Loader._rooms.ElementAt(id2).Key.Contains("Hall"))
                        break;
            }

            KeyValuePair<string, RoomModel> a = S.Loader._rooms.ElementAt(id1);
            KeyValuePair<string, RoomModel> b = S.Loader._rooms.ElementAt(id2);

            int currentDoorNumber1 = 1 + _rnd.Next(a.Value._doors.Count);
            int currentDoorNumber2 = 1 + _rnd.Next(b.Value._doors.Count);

            if (S.Loader._rooms.ElementAt(id1).Key.Contains("Hall"))
                currentDoorNumber1 = 2;
            else if (S.Loader._rooms.ElementAt(id2).Key.Contains("Hall"))
                currentDoorNumber2 = 2;

            if (a.Value._doors.Count <= 0)
                Debug.Log($"a is empty! key = {a.Key}, value = {a.Value}");

            string currentRoom1name = (string)a.Key.Clone();
            string currentRoom2name = (string)b.Key.Clone();

            string nextRoom1name = (string)a.Value._doors[currentDoorNumber1]._nextSceneName.Clone();
            string nextRoom2name = (string)b.Value._doors[currentDoorNumber2]._nextSceneName.Clone();

            int nextDoorNumber1 = a.Value._doors[currentDoorNumber1]._nextDoorNumber;
            int nextDoorNumber2 = b.Value._doors[currentDoorNumber2]._nextDoorNumber;

            if (currentRoom1name.Equals(nextRoom1name) || currentRoom2name.Equals(nextRoom2name) || currentRoom1name.Equals(nextRoom2name) || currentRoom2name.Equals(nextRoom1name) || nextRoom1name.Equals(nextRoom2name) && nextDoorNumber1.Equals(nextDoorNumber2))
            {
                Debug.Log($"Nonsense.");
                continue;
            }

            S.Loader._rooms[currentRoom1name]._doors[currentDoorNumber1]._nextSceneName = (string)nextRoom2name.Clone();
            S.Loader._rooms[currentRoom2name]._doors[currentDoorNumber2]._nextSceneName = (string)nextRoom1name.Clone();

            S.Loader._rooms[currentRoom1name]._doors[currentDoorNumber1]._nextDoorNumber = nextDoorNumber2;
            S.Loader._rooms[currentRoom2name]._doors[currentDoorNumber2]._nextDoorNumber = nextDoorNumber1;

            //

            S.Loader._rooms[nextRoom1name]._doors[nextDoorNumber1]._nextSceneName = (string)currentRoom2name.Clone();
            S.Loader._rooms[nextRoom2name]._doors[nextDoorNumber2]._nextSceneName = (string)currentRoom1name.Clone();

            S.Loader._rooms[nextRoom1name]._doors[nextDoorNumber1]._nextDoorNumber = currentDoorNumber2;
            S.Loader._rooms[nextRoom2name]._doors[nextDoorNumber2]._nextDoorNumber = currentDoorNumber1;

            //

            S.Loader._map[currentRoom1name].Remove(nextRoom1name);
            S.Loader._map[nextRoom2name].Remove(currentRoom2name);

            S.Loader._map[currentRoom2name].Remove(nextRoom2name);
            S.Loader._map[nextRoom1name].Remove(currentRoom1name);

            //

            S.Loader._map[currentRoom1name].Add(nextRoom2name);
            S.Loader._map[nextRoom1name].Add(currentRoom2name);

            S.Loader._map[currentRoom2name].Add(nextRoom1name);
            S.Loader._map[nextRoom2name].Add(currentRoom1name);

            if (!CheckNet())
            {
                Undo("Wrong net.");
                continue;
            }

            if (!CheckNotSmallCircles())
            {
                Undo("Small circles.");
                continue;
            }


            if (!Check5and8())
            {
                Undo("Wrong 5 and 8.");
                continue;
            }

            Debug.Log($"GOOD! Switched backrooms #{n}. (Try #{i})");

            break;

            void Undo(string sms)
            {
                S.Loader._rooms[currentRoom1name]._doors[currentDoorNumber1]._nextSceneName = (string)nextRoom1name.Clone();
                S.Loader._rooms[currentRoom2name]._doors[currentDoorNumber2]._nextSceneName = (string)nextRoom2name.Clone();

                S.Loader._rooms[currentRoom1name]._doors[currentDoorNumber1]._nextDoorNumber = nextDoorNumber1;
                S.Loader._rooms[currentRoom2name]._doors[currentDoorNumber2]._nextDoorNumber = nextDoorNumber2;

                //

                S.Loader._rooms[nextRoom1name]._doors[nextDoorNumber1]._nextSceneName = (string)currentRoom1name.Clone();
                S.Loader._rooms[nextRoom2name]._doors[nextDoorNumber2]._nextSceneName = (string)currentRoom2name.Clone();

                S.Loader._rooms[nextRoom1name]._doors[nextDoorNumber1]._nextDoorNumber = currentDoorNumber1;
                S.Loader._rooms[nextRoom2name]._doors[nextDoorNumber2]._nextDoorNumber = currentDoorNumber2;

                //

                S.Loader._map[currentRoom1name].Remove(nextRoom2name);
                S.Loader._map[nextRoom2name].Remove(currentRoom1name);

                S.Loader._map[currentRoom2name].Remove(nextRoom1name);
                S.Loader._map[nextRoom1name].Remove(currentRoom2name);

                //

                S.Loader._map[currentRoom1name].Add(nextRoom1name);
                S.Loader._map[nextRoom1name].Add(currentRoom1name);

                S.Loader._map[currentRoom2name].Add(nextRoom2name);
                S.Loader._map[nextRoom2name].Add(currentRoom2name);

                Debug.Log($"No no no. ({sms})");
            }
        }

        yield return null;
    }

    void FillAllRoomsList()
    {
        _allRoomsList = new List<string>();

        _allRoomsList.Add("BR 1");
        _allRoomsList.Add("BR 2");
        _allRoomsList.Add("BR 3");
        _allRoomsList.Add("BR 4");
        _allRoomsList.Add("BR 5");
        _allRoomsList.Add("BR 6");
        _allRoomsList.Add("BR 7");
        _allRoomsList.Add("BR 8");
        _allRoomsList.Add("BR 1R");
        _allRoomsList.Add("BR 2R");
        _allRoomsList.Add("BR 3R");
        _allRoomsList.Add("BR 4R");
        _allRoomsList.Add("BR 6R");
        _allRoomsList.Add("BR 7R");
        _allRoomsList.Add("Hall");
    }

    bool Check5and8()
    {
        FillAllRoomsList();

        string firstOne = "nope";

        foreach (string room in _allRoomsList)
        {
            List<string> neighbours = S.Loader._map[room];
            foreach (string neighbour in neighbours)
                if (neighbour.Equals("Hall"))
                {
                    firstOne = (string)room.Clone();
                    break;
                }
        }

        if (firstOne.Equals("BR 5") || firstOne.Equals("BR 8") || firstOne.Equals("nope"))
        {
            Debug.Log("Incorrect.");
            return false;
        }

        List<string> currentLevel = new List<string>();
        currentLevel.Add(firstOne);
        _allRoomsList.Remove(firstOne);

        int br5dist = -1;
        int br8dist = -1;

        for (int level = 0; level < 30; level++)
        {
            int count = currentLevel.Count;
            for (int j = 0; j < count; j++)
            {
                string room = currentLevel[0];

                List<string> neighbours = S.Loader._map[room];

                for (int k = 0; k < neighbours.Count; k++)
                {
                    string neighbour = neighbours[k];

                    if (_allRoomsList.Contains(neighbour))
                    {
                        currentLevel.Add(neighbour);
                        _allRoomsList.Remove(neighbour);
                    }
                }

                currentLevel.RemoveAt(0);
            }

            if (currentLevel.Contains("BR 5") && br5dist == -1)
                br5dist = level + 1;
            if (currentLevel.Contains("BR 8") && br8dist == -1)
                br8dist = level + 1;
            if (br5dist != -1 && br8dist != -1)
                break;
        }

        if (br5dist > 1 && br8dist > 2)
        {
            Debug.Log($"Correct!");
            return true;
        }
        else
        {
            Debug.Log($"Wrong. ({br5dist} and {br8dist})");
            return false;
        }
    }

    bool CheckNet()
    {
        FillAllRoomsList();

        List<string> weAreHere = new List<string>();

        weAreHere.Add("BR 1");

        while (weAreHere.Count > 0)
        {
            List<string> gg = S.Loader._map[weAreHere[0]];

            foreach (string room in gg)
            {
                if (_allRoomsList.Contains(room))
                {
                    _allRoomsList.Remove(room);
                    weAreHere.Add(room);
                }
            }

            weAreHere.RemoveAt(0);
        }

        return _allRoomsList.Count == 0;
    }

    bool CheckNotSmallCircles()
    {
        FillAllRoomsList();

        string ourRoom = _allRoomsList.ElementAt(0);

        List<string> neighbours = S.Loader._map[ourRoom];
        foreach (string neighbour in neighbours)
        {
            int i = 0;

            foreach (string nb2 in neighbours)
                if (string.Equals(neighbour, nb2))
                    i++;

            if (i != 1)
                return false;
        }

        return true;
    }
}
