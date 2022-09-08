using Fusion;
using OdinNative.Odin.Peer;
using OdinNative.Odin.Room;
using OdinNative.Unity.Audio;
using UnityEngine;

public class OdinRemote : MonoBehaviour
{
    [SerializeField] private PlaybackComponent playbackPrefab;
    private PlaybackComponent _spawnedPlayback;

    private void Start()
    {
        OdinHandler.Instance.OnMediaAdded.AddListener(MediaAdded);
        OdinHandler.Instance.CreatePlayback = false;
    }
    private void MediaAdded(object roomObject, MediaAddedEventArgs eventArgs)
    {
        ulong peerId = eventArgs.PeerId;
        long mediaId = eventArgs.Media.Id;
        if (roomObject is Room room)
        {
            Peer peer = room.RemotePeers[peerId];
            CustomUserData userData = JsonUtility.FromJson<CustomUserData>(peer.UserData.ToString());
            NetworkObject networkObject = GetComponent<NetworkObject>();
            if (userData.NetworkId == networkObject.Id.Raw)
            {
                _spawnedPlayback = Instantiate(playbackPrefab, transform);
                _spawnedPlayback.transform.localPosition = Vector3.zero;
                _spawnedPlayback.RoomName = room.Config.Name;
                _spawnedPlayback.PeerId = peerId;
                _spawnedPlayback.MediaStreamId = mediaId;
            }
        }
    }
    
    private void OnDisable()
    {
        if (null != _spawnedPlayback)
            Destroy(_spawnedPlayback.gameObject);
    }
}