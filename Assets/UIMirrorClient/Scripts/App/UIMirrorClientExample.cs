using UnityEngine;

public class UIMirrorClientExample : MonoBehaviour
{
    protected void Start()
    {
        NetworkManager.Instance.ServerConnected += Instance_ServerConnected;
        NetworkManager.Instance.ServerDisconnected += Instance_ServerDisconnected;
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
