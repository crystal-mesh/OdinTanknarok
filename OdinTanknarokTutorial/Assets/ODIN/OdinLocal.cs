using System.Text;
using Fusion;
using OdinNative.Odin;
using UnityEngine;

public class OdinLocal : MonoBehaviour
{
    [SerializeField] private string roomName = "Proximity";
    private bool _isLocal;
    private void Start()
    {
        NetworkObject networkObject = GetComponent<NetworkObject>();
        _isLocal = networkObject.HasStateAuthority;
        if (_isLocal)
        {
            CustomUserData roomData = new CustomUserData
            {
                NetworkId = networkObject.Id.Raw
            };

            // use this, if you'd like differentiate between photon fusion rooms when joining an odin room.
            // string combinedName = networkObject.Runner.SessionInfo.Name + "_" + roomName;
            OdinHandler.Instance.JoinRoom(roomName, roomData);
        }
    }
    private void OnDestroy()
    {
        if (_isLocal)
            OdinHandler.Instance.LeaveRoom(roomName);
    }
}

public class CustomUserData : IUserData
{
    public uint NetworkId;
    public override string ToString()
    {
        return JsonUtility.ToJson(this);
    }
    public bool IsEmpty()
    {
        return string.IsNullOrEmpty(this.ToString());
    }
    public byte[] ToBytes()
    {
        return Encoding.UTF8.GetBytes(ToString());
    }
}