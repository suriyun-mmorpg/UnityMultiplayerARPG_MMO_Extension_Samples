# UnityMultiplayerARPG_MMO_Extension_Samples

This is collection of dev extension samples for https://github.com/insthync/UnityMultiplayerARPG_MMO

## CustomFromClientToCentralServer

This extension will send text from client to central-server, then when central-server receives text it will send lower-case text back

### Workflows
* **Client** send message to **Map-Server** 
* Then **Map-Server** send message to **Central-Server**
* After that **Central-Server** send lower-case text back to **Map-Server**
* Finally **Map-Server** send text which receives from **Central-Server** to **Client**
