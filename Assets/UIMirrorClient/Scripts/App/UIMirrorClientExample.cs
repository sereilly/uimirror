using UnityEngine;

public class UIMirrorClientExample : MonoBehaviour
{
    [SerializeField]
    protected NetworkManager networkManager;

    protected void Start()
    {
        networkManager.ServerConnected += Instance_ServerConnected;
        networkManager.ServerDisconnected += Instance_ServerDisconnected;
    }

    private void Instance_ServerDisconnected()
    {
        PageManager.Instance.CurrentPage = PageManager.Page.Connect;
    }

    private void Instance_ServerConnected()
    {
        PageManager.Instance.CurrentPage = PageManager.Page.Canvas;
    }
}
