using System.Collections.Concurrent;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ServerList : MonoBehaviour
{
    [SerializeField]
    protected MachineReceiver machineReceiver;
    [SerializeField]
    protected NetworkManager networkManager;

    public static ServerList Instance { get; private set; }
    ConcurrentDictionary<Machine, float> machines = new ConcurrentDictionary<Machine, float>();
    private const float addressTimeout = 4.0f;
    List<Button> buttons = new List<Button>();
    ConcurrentQueue<Machine> newConnections = new ConcurrentQueue<Machine>();

    [SerializeField]
    protected Button buttonPrefab;
    [SerializeField]
    protected InputField ipInputField;

    private struct Machine
    {
        public string Name;
        public string Address;
    }

    protected void Awake()
    {
        Instance = this;
        machineReceiver.MachineFound += MachineReceiver_MachineFound;
    }

    private void MachineReceiver_MachineFound(string machineName, string address)
    {
        Add(machineName, address);
    }

    public void Add(string name, string address)
    {
        Machine machine = new Machine
        {
            Name = name,
            Address = address
        };

        newConnections.Enqueue(machine);
    }

    protected void Update()
    {
        while (newConnections.TryDequeue(out Machine machine))
        {
            if (!machines.ContainsKey(machine))
            {
                AddButton(machine);
            }
            machines[machine] = Time.time;
        }

        foreach (var kvp in machines)
        {
            if (Time.time - kvp.Value > addressTimeout)
            {
                machines.TryRemove(kvp.Key, out _);
                RemoveButton(kvp.Key.Address);
                break;
            }
        }
    }

    private void AddButton(Machine machine)
    {
        Button button = Instantiate(buttonPrefab);

        button.onClick.AddListener(() =>
        {
            networkManager.ConnectToServer(machine.Address);
        });

        string label = string.Format("{0} ({1})", machine.Name, machine.Address);
        button.GetComponentInChildren<TextMeshProUGUI>().text = label;
        button.name = machine.Address;
        button.transform.SetParent(transform, false);
        buttons.Add(button);
    }

    private void RemoveButton(string ip)
    {
        Button button = buttons.Find(b => b.name == ip);
        if (button)
        {
            buttons.Remove(button);
            Destroy(button.gameObject);
        }
    }

    public void ManualConnect()
    {
        networkManager.ConnectToServer(ipInputField.text);
    }
}
