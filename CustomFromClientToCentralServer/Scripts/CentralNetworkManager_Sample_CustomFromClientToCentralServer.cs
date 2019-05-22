using LiteNetLibManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG.MMO
{
    public partial class CentralNetworkManager
    {
        [DevExtMethods("RegisterServerMessages")]
        private void CustomFromClientToCentralServer_RegisterServerMessages()
        {
            // Server will receive this message from map-server → `CentralAppServerRegister`, then send something back to client
            RegisterServerMessage(10001, SampleHandleMsgFromMapServer);
        }

        // Map-Server message handlers
        private void SampleHandleMsgFromMapServer(LiteNetLibMessageHandler messageHandler)
        {
            string text = messageHandler.reader.GetString();
            float clientTime = messageHandler.reader.GetFloat();
            long connectionId = messageHandler.reader.GetLong();
            Debug.Log("[CustomFromClientToCentralServer] central-server receive: " + text + " " + clientTime + " then send lower-case text back to map-server");
            ServerSendPacket(messageHandler.connectionId, LiteNetLib.DeliveryMethod.ReliableOrdered, 10001, (writer) =>
            {
                writer.Put(text.ToLower());
                writer.Put(clientTime);
                writer.Put(connectionId);
            });
        }
    }
}
