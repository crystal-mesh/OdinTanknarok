using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Fusion;
using OdinNative.Odin;
using OdinNative.Odin.Room;
using OdinNative.Unity.Audio;
using UnityEngine;

namespace ODIN.Scripts
{
    public class OdinConnection : MonoBehaviour
    {
        [SerializeField] private string room = "Voice";
        [SerializeField] private NetworkObject networkObject;
        [SerializeField] private PlaybackComponent playbackPrefab;


        private PlaybackComponent _spawnedPlayback;
        private void Start()
        {
            OdinHandler.Instance.OnMediaAdded.AddListener(OnMediaAdded);
            OdinHandler.Instance.OnPeerJoined.AddListener(OnPeerJoined);
            OdinHandler.Instance.OnRoomJoined.AddListener(OnRoomJoined);
            if (networkObject.HasInputAuthority)
            {
                MyUserData roomData = new MyUserData();
                roomData.fusionId = networkObject.Id.Raw;
                OdinHandler.Instance.JoinRoom(room, roomData);
            }
        }

        private void OnRoomJoined(RoomJoinedEventArgs arg0)
        {
            Debug.Log("ODIN: On Room Joined!");
        }

        private void OnPeerJoined(object arg0, PeerJoinedEventArgs arg1)
        {
            Debug.Log("ODIN: On Peer Joined!");
        }

        private void OnMediaAdded(object roomObject, MediaAddedEventArgs eventArgs)
        {
            if (roomObject is Room room)
            {
                MyUserData peerUserData = JsonUtility.FromJson<MyUserData>(eventArgs.Peer.UserData.ToString());
                if (peerUserData.fusionId == networkObject.Id.Raw)
                {
                    _spawnedPlayback = Instantiate(playbackPrefab, transform);
                    _spawnedPlayback.transform.localPosition = Vector3.zero;
                    _spawnedPlayback.RoomName = room.Config.Name;
                    _spawnedPlayback.PeerId = eventArgs.PeerId;
                    _spawnedPlayback.MediaStreamId = eventArgs.Media.Id;
                }
            }
        }

        private void OnDisable()
        {
            if (_spawnedPlayback )
            {
                Destroy(_spawnedPlayback);
                OdinHandler.Instance.LeaveRoom(room);
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
