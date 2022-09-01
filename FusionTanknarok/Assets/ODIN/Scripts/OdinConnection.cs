using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Fusion;
using OdinNative.Odin;
using OdinNative.Odin.Peer;
using OdinNative.Odin.Room;
using OdinNative.Unity.Audio;
using UnityEngine;

namespace ODIN.Scripts
{
    public class OdinConnection : MonoBehaviour
    {
        [SerializeField] private string roomName = "Voice";
        [SerializeField] private NetworkObject networkObject;
        [SerializeField] private PlaybackComponent playbackPrefab;

        private bool _isLocalPlayer = false;
        private PlaybackComponent _spawnedPlayback;

        private void Start()
        {
            OdinHandler.Instance.OnCreatedMediaObject.AddListener(OnCreatedMediaObject);

            if (networkObject.HasInputAuthority)
            {
                // TODO: Get Room name from fusion
                _isLocalPlayer = true;
                MyUserData roomData = new MyUserData();
                roomData.fusionId = networkObject.Id.Raw;
                OdinHandler.Instance.JoinRoom(roomName, roomData);
            }
        }

        private void OnCreatedMediaObject(string roomName, ulong peerId, long mediaId)
        {
            Room room = OdinHandler.Instance.Rooms[roomName];
            Peer peer = room?.RemotePeers[peerId];
            if (null != room && null != peer)
            {
                MyUserData peerUserData = JsonUtility.FromJson<MyUserData>(peer.UserData.ToString());
                if (null != peerUserData && peerUserData.fusionId == networkObject.Id.Raw)
                {
                    if(_spawnedPlayback)
                        Destroy(_spawnedPlayback.gameObject);
                    
                    _spawnedPlayback = Instantiate(playbackPrefab, transform);
                    _spawnedPlayback.transform.localPosition = Vector3.zero;
                    _spawnedPlayback.RoomName = room.Config.Name;
                    _spawnedPlayback.PeerId = peer.Id;
                    _spawnedPlayback.MediaStreamId = mediaId;
                }
            }
        }
        
        private void OnDisable()
        {
            if(_spawnedPlayback)
                Destroy(_spawnedPlayback.gameObject);
            if (_isLocalPlayer )
            {
                OdinHandler.Instance.LeaveRoom(roomName);
            }
        }
    }

    public class MyUserData : IUserData
    {
        public uint fusionId;
        
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
}
