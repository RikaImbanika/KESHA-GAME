using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PortalsBase : MonoBehaviour
{
    //Room name, than portal ID, than you will get your portal)
    private Dictionary<string, Dictionary<string, Portal>> _portals;
    //This is when you first need to get portal id, but know room
    private Dictionary<string, List<string>> _freePortals;
    //This is if other portal takes that portal first
    private Dictionary<string, Dictionary<string, string>> _takenFreePortals;
    //This is exactly which scenes should be connected
    private Dictionary<string, List<string>> _connections;

    //First we fill connections
    //Than portalPlacers take connections
    //Than they open freePorts
    //Than they found each other freePorts
    //Than they get ids from freeports and can work

    public void AddPortal(string sceneName, string portId, Portal portal)
    {
        if (!Portals.ContainsKey(sceneName))
            Portals.Add(sceneName, new Dictionary<string, Portal>());

        Portals[sceneName][portId] = portal;
    }

    public void AddFreePort(string sceneName, string portId)
    {
        if (!FreePorts.ContainsKey(sceneName))
        {
            FreePorts.Add(sceneName, new List<string>());
            TakenFreePorts.Add(sceneName, new Dictionary<string, string>());
        }

        if (!FreePorts[sceneName].Contains(portId))
        {
            if (!TakenFreePorts[sceneName].ContainsKey(portId))
            {
                FreePorts[sceneName].Add(portId);
                Debug.Log($"I placed freeport {sceneName}, {portId}");
            }
            else
                Debug.Log($"My freeport {sceneName}, {portId} already placed and already taken.");
        }
        else
            Debug.Log($"My freeport {sceneName}, {portId} already placed.");
    }

    public IEnumerator TakeFreePort(string sceneName, string thatPortId, string secSceneName)
    {
        string secondPortId = thatPortId;

        while (!FreePorts.ContainsKey(secSceneName))
            yield return new WaitForSeconds(0.5f);

        yield return new WaitForSeconds(S.RND.Next(500) * 0.01f);

        while (true)
        {
            while (FreePorts[secSceneName].Count <= 0)
            {
                if (S.PortalsBase.TakenFreePorts[sceneName].ContainsKey(thatPortId))
                {
                    secondPortId = S.PortalsBase.TakenFreePorts[sceneName][thatPortId];
                    S.PortalsBase.TakenFreePorts[sceneName].Remove(thatPortId);

                    Debug.Log($"My freeport {sceneName}, {thatPortId} taken by {secondPortId}!");
                    yield return secondPortId;
                    yield break;
                }

                Debug.Log($"My freeport {sceneName}, {thatPortId} not taken yet.");

                yield return new WaitForSeconds(0.5f);
            }

            int index = S.RND.Next(FreePorts[secSceneName].Count);

            if (index > -1)
            {
                Debug.Log($"Freeport Index == {index}.");

                secondPortId = FreePorts[secSceneName][index];
            }
            if (secondPortId != thatPortId)
                break;

            yield return new WaitForSeconds(0.05f);
        }

        Debug.Log($"I take freeport {secSceneName}, {secondPortId}.");

        S.PortalsBase.TakenFreePorts[secSceneName].Add(secondPortId, thatPortId);
        S.PortalsBase.FreePorts[secSceneName].Remove(secondPortId);
        S.PortalsBase.FreePorts[sceneName].Remove(thatPortId);

        yield return secondPortId;
    }

    public void AddConnection(string sceneName1, string sceneName2)
    {
        if (!Connections.ContainsKey(sceneName1))
            Connections.Add(sceneName1, new List<string>());

        Connections[sceneName1].Add(sceneName2);
    }

    public string TakeConnection(string sceneName)
    {
        string secondSceneName = "";
        lock (Connections)
        {
            if (Connections.ContainsKey(sceneName))
            {
                int count = Connections[sceneName].Count;
                int index = S.RND.Next(count);
                secondSceneName = Connections[sceneName].ElementAt(index);
                Connections[sceneName].RemoveAt(index);

                if (count == 1)
                    Connections.Remove(sceneName);
            }
        }
        return secondSceneName;
    }

    public Dictionary<string, Dictionary<string, Portal>> Portals
    {
        get
        {
            return _portals;
        }
        set
        {
            _portals = value;
        }
    }

    public Dictionary<string, List<string>> FreePortals
    {
        get
        {
            return _freePortals;
        }
        set
        {
            _freePortals = value;
        }
    }

    public Dictionary<string, List<string>> FreePorts
    {
        get
        {
            return _freePortals;
        }
        set
        {
            _freePortals = value;
        }
    }

        public Dictionary<string, List<string>> Connections
    {
        get
        {
            return _connections;
        }
        set
        {
            _connections = value;
        }
    }

    public Dictionary<string, Dictionary<string, string>> TakenFreePorts
    {
        get
        {
            return _takenFreePortals;
        }
        set
        {
            _takenFreePortals = value;
        }
    }

    void Start()
    {
        _portals = new Dictionary<string, Dictionary<string, Portal>>();
        _freePortals = new Dictionary<string, List<string>>();
        _takenFreePortals = new Dictionary<string, Dictionary<string, string>>();
        _connections = new Dictionary<string, List<string>>();
        AddConnection("Income", "Corridor");
        AddConnection("Corridor", "Income");
        S.PortalsBase = this;
    }
}