using UnityEngine;

public class OdinConnectionTest : MonoBehaviour
{
    [SerializeField] private string roomName;
    void Start()
    {
        OdinHandler.Instance.JoinRoom(roomName);    
    }
}