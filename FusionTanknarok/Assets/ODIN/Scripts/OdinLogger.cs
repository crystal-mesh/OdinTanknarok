using System;
using Fusion;
using OdinNative.Odin.Room;
using UnityEngine;

namespace ODIN.Scripts
{
    public class OdinLogger : MonoBehaviour
    {
        private void Start()
        {
            OdinHandler.Instance.OnMediaAdded.AddListener(OnMediaAdded);
            OdinHandler.Instance.OnMediaRemoved.AddListener((room, args) => OdinLog("OnMediaRemoved", room, args.Peer.Id, args.MediaStreamId));
            OdinHandler.Instance.OnPeerJoined.AddListener((arg0, args) => OdinLog("OnPeerJoined", arg0, args.PeerId));
            OdinHandler.Instance.OnPeerLeft.AddListener((arg0, args) => OdinLog("OnPeerLeft", arg0, args.PeerId));
            OdinHandler.Instance.OnRoomJoined.AddListener(arg0 => OdinLog("OnRoomJoined", arg0.Room, 0));
            OdinHandler.Instance.OnRoomLeft.AddListener(arg0 => OdinLog("OnRoomLeft", arg0.RoomName, 0));
        }

        private void OnMediaAdded(object room, MediaAddedEventArgs arg1)
        {
            OdinLog("OnMediaAdded", room, arg1.PeerId, arg1.Media.Id);
        }


        private void OdinLog(string Prefix, object room, ulong peerId, long mediaId = -1)
        {
            if(room is Room asRoom)
                Debug.Log($"ODIN: {Prefix}: media: {mediaId}, peer: {peerId}, room: {asRoom.Config.Name}");
            if (room is string roomName)
            {
                Debug.Log($"ODIN: {Prefix}: media: {mediaId}, peer: {peerId}, room: {roomName}");
            }
        }
    }
}