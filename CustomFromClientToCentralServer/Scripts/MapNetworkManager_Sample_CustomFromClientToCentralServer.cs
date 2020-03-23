using LiteNetLibManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG.MMO
{
    public partial class MapNetworkManager
    {
        [Header("Sample - CustomFromClientToCentralServer")]
        public string sampleMessageToCentralServer = "QWERTY";
        private Coroutine sampleMessageToCentralCoroutine;

        [DevExtMethods("Init")]
        private void CustomFromClientToCentralServer_Init()
        {
            // Start coroutine to send message every seconds
            if (sampleMessageToCentralCoroutine != null)
                StopCoroutine(sampleMessageToCentralCoroutine);
            sampleMessageToCentralCoroutine = StartCoroutine(SampleMessageToCentralRoutine());
        }

        IEnumerator SampleMessageToCentralRoutine()
        {
            while (true)
            {
                if (IsClientConnected)
                {
                    // Send message from client to map-server
                    Debug.Log("[CustomFromClientToCentralServer] client send: " + sampleMessageToCentralServer + " " + ServerUnixTime);
                    ClientSendPacket(LiteNetLib.DeliveryMethod.ReliableOrdered, 10001, (writer) =>
                    {
                        writer.Put(sampleMessageToCentralServer);
                        writer.Put(ServerUnixTime);
                    });
                }
                yield return new WaitForSeconds(1);
            }
        }

        [DevExtMethods("RegisterClientMessages")]
        private void CustomFromClientToCentralServer_RegisterClientMessages()
        {
            // Clients will receive this message from server
            RegisterClientMessage(10001, SampleHandleMsgFromMapServer);
        }

        [DevExtMethods("RegisterServerMessages")]
        private void CustomFromClientToCentralServer_RegisterServerMessages()
        {
            // In map-server there is client which connect to central-server, the field is `CentralAppServerRegister`
            // So developer may register `CentralAppServerRegister` messages here
            CentralAppServerRegister.RegisterMessage(10001, SampleHandleMsgFromCentralServer);

            // Server will receive this message from clients, then send it to central server by `CentralAppServerRegister`
            RegisterServerMessage(10001, SampleHandleMsgFromClient);
        }

        // Client message handlers
        private void SampleHandleMsgFromMapServer(LiteNetLibMessageHandler messageHandler)
        {
            Debug.Log("[CustomFromClientToCentralServer] client receive: " + messageHandler.reader.GetString() + " " + messageHandler.reader.GetInt());
        }

        // CentralAppServerRegister message handlers
        private void SampleHandleMsgFromCentralServer(LiteNetLibMessageHandler messageHandler)
        {
            string text = messageHandler.reader.GetString();
            long clientTime = messageHandler.reader.GetLong();
            long connectionId = messageHandler.reader.GetLong();
            Debug.Log("[CustomFromClientToCentralServer] map-server receive from central-server: " + text + " " + clientTime + " then send this to client");
            ServerSendPacket(connectionId, LiteNetLib.DeliveryMethod.ReliableOrdered, 10001, (writer) =>
            {
                writer.Put(text);
                writer.Put(ServerUnixTime);
            });
        }

        // Server message handlers
        private void SampleHandleMsgFromClient(LiteNetLibMessageHandler messageHandler)
        {
            // Receive message from client at map-server
            // Then keep transport handler at server to use later to send message to client
            string text = messageHandler.reader.GetString();
            long clientTime = messageHandler.reader.GetLong();
            Debug.Log("[CustomFromClientToCentralServer] map-server receive from client: " + text + " " + clientTime + " then send this to central-server");
            CentralAppServerRegister.SendPacket(LiteNetLib.DeliveryMethod.ReliableOrdered, 10001, (writer) =>
            {
                writer.Put(sampleMessageToCentralServer);
                writer.Put(ServerUnixTime);
                writer.Put(messageHandler.connectionId);
            });
        }
    }
}
