using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyScript : MonoBehaviour {

    SpawnerOutput lobbyText;
    SpawnerOutput startGame;

    // Use this for initialization
    void Start() {

        lobbyText = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(TextWrapper));
        lobbyText.script.transform.position = UIDrawer.UINormalisedPosition(new Vector3(0.5f, 0.9f));

        UIDrawer.GetTypeInElement<Text>(lobbyText).text = "Lobby";

        startGame = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(ButtonWrapper));
        startGame.script.transform.position = UIDrawer.UINormalisedPosition(new Vector3(0.5f, 0.1f));

        UIDrawer.GetTypeInElement<Text>(startGame).text = "Start Game";
        UIDrawer.GetTypeInElement<Button>(startGame).onClick.AddListener(() => {
            ServerChannel sC = NetworkMessageEncoder.encoders[(int)NetworkEncoderTypes.SERVER_CHANNEL] as ServerChannel;
            sC.CommunicateServerSide(ServerSideMethods.START_GAME);
        });
    }

    // Update is called once per frame
    void Update() {

    }
}
