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
            OdinHandler.Instance.OnMediaAdded.AddListener(OnMediaAdded);
            OdinHandler.Instance.OnPeerJoined.AddListener(OnPeerJoined);
            OdinHandler.Instance.OnRoomJoined.AddListener(OnRoomJoined);



            if (networkObject.HasInputAuthority)
            {
                // TODO: Get Room name from fusion
                _isLocalPlayer = true;
                MyUserData roomData = new MyUserData();
                roomData.fusionId = networkObject.Id.Raw;
                OdinHandler.Instance.JoinRoom(roomName, roomData);
            }
        }

        private void OnRoomJoined(RoomJoinedEventArgs arg0)
        {
            if(!networkObject.HasInputAuthority)
                Debug.Log($"---------------- ODIN: OnRoomJoined was called, room {arg0.Room.Config.Name}---------------- ");
        }

        private void OnPeerJoined(object arg0, PeerJoinedEventArgs arg1)
        {
            if(!networkObject.HasInputAuthority  && arg0 is Room room)
                Debug.Log($"----------------  ODIN: OnPeerJoined was called, room {room.Config.Name}, peer {arg1.PeerId} ---------------- ");

        }

        private void OnMediaAdded(object arg0, MediaAddedEventArgs arg1)
        {
            ulong peerId = arg1.PeerId;
            long mediaId = arg1.Media.Id;
            Room room = OdinHandler.Instance.Rooms[roomName];
            Peer peer = room?.RemotePeers[peerId];
            if (null != room && null != peer)
            {
                MyUserData peerUserData = JsonUtility.FromJson<MyUserData>(peer.UserData.ToString());
                if (null != peerUserData && peerUserData.fusionId == networkObject.Id.Raw)
                {
                    Debug.Log($" ---------------- ODIN: OnMediaAdded, found 3d playback candidate, room {room.Config.Name}, peer {arg1.PeerId}, media {arg1.Media.Id}---------------- ");
                    
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

        private void OnCreatedMediaObject(string inRoomName, ulong peerId, long mediaId)
        {
            
            Room room = OdinHandler.Instance.Rooms[inRoomName];
            Peer peer = room?.RemotePeers[peerId];
            if (null != room && null != peer)
            {
                MyUserData peerUserData = JsonUtility.FromJson<MyUserData>(peer.UserData.ToString());
                if (null != peerUserData && peerUserData.fusionId == networkObject.Id.Raw)
                {
                    Debug.Log($" ---------------- ODIN: OnCreatedMediaObject, found 3d playback candidate, room {inRoomName}, peer {peerId}, media {mediaId} ---------------- ");

                    // if(_spawnedPlayback)
                    //     Destroy(_spawnedPlayback.gameObject);
                    //
                    // _spawnedPlayback = Instantiate(playbackPrefab, transform);
                    // _spawnedPlayback.transform.localPosition = Vector3.zero;
                    // _spawnedPlayback.RoomName = room.Config.Name;
                    // _spawnedPlayback.PeerId = peer.Id;
                    // _spawnedPlayback.MediaStreamId = mediaId;
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
