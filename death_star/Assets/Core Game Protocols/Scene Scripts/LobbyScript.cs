using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyScript : MonoBehaviour {

    public static LobbyScript lobbyInst;

    PlayerCustomDataTrasmitter cDT;
    ImageDependenciesTransfer iDT;
    ManifestEncoder mE;
    bool startInitiated;

    SpawnerOutput lobbyText;
    SpawnerOutput startGame;
    SpawnerOutput progressOfFiles;

    // Use this for initialization
    void Start() {
        lobbyInst = this;
        cDT = NetworkMessageEncoder.encoders[(int)NetworkEncoderTypes.CUSTOM_DATA_TRASMIT] as PlayerCustomDataTrasmitter;
        iDT = NetworkMessageEncoder.encoders[(int)NetworkEncoderTypes.IMAGE_DATA_TRANSMIT] as ImageDependenciesTransfer;
        mE = NetworkMessageEncoder.encoders[(int)NetworkEncoderTypes.MANIFEST] as ManifestEncoder;

        ResetGameplayNetworkHelpers();

        lobbyText = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(TextWrapper));
        lobbyText.script.transform.position = UIDrawer.UINormalisedPosition(new Vector3(0.5f, 0.9f));

        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Text>(lobbyText).text = "Lobby";

        progressOfFiles = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(TextWrapper));
        progressOfFiles.script.transform.position = UIDrawer.UINormalisedPosition(new Vector3(0.5f, 0.5f));
        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Text>(progressOfFiles).verticalOverflow = VerticalWrapMode.Overflow;

        startGame = LoadedData.GetSingleton<UIDrawer>().CreateScriptedObject(typeof(ButtonWrapper));
        startGame.script.transform.position = UIDrawer.UINormalisedPosition(new Vector3(0.5f, 0.1f));

        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Text>(startGame,"Text").text = "Start Game";

        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Button>(startGame).onClick.AddListener(() => {
            ServerChannel sC = NetworkMessageEncoder.encoders[(int)NetworkEncoderTypes.SERVER_CHANNEL] as ServerChannel;
            sC.CommunicateServerSide(ServerSideMethods.START_GAME);
        });
    }

    public void OnStartSignal() {
        cDT.SendFiles();
        iDT.SendArtAssets();
        startInitiated = true;
    }

    public void ResetGameplayNetworkHelpers() {
        AbilitiesManager.aData = new Dictionary<int, AbilitiesManager.PlayerAssetData>();
        cDT.ResetTransmitter();
        iDT.ResetTransfer();
        mE.ResetManifestEncoder();
    }

    // Update is called once per frame
    void Update() {
        string text = "Datafiles: " + cDT.sentFiles.ToString() + "/" + cDT.expectedFiles.ToString() + "\n";
        text += "Art Assets: " + iDT.sentFiles.ToString() + "/" + iDT.expectedFiles.ToString() + "\n";
        text += "Manifest: " + mE.sentData.ToString() + "/" + mE.expectedDataCount.ToString() + "\n";

        text += LoadedData.GetCurrentTimestamp();
        LoadedData.GetSingleton<UIDrawer>().GetTypeInElement<Text>(progressOfFiles).text = text;

        if(cDT.sentFiles == cDT.expectedFiles && iDT.sentFiles == iDT.expectedFiles && startInitiated && mE.expectedDataCount == mE.sentData)
            SceneTransitionData.LoadScene("Gameplay");
    }
}
