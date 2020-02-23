using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandCentral : MonoBehaviour {

    string splitCommands;

    //Parse commands can be 

    public string[] HandleIncomingCommands(string command) {
        if(!command.Contains("\n")) {
            splitCommands = command;
            return new string[0];
        }

        bool keepLastCommand = false;
        keepLastCommand = command[command.Length - 1] != '\n' ? true : false;

        string[] commands = command.Split('\n');
        commands[0] = splitCommands + commands[0];
        splitCommands = keepLastCommand ? commands[commands.Length - 1] : "";
        return commands;
    }

    public void ParseCommands(string command) {

        string[] commands = HandleIncomingCommands(command);

        for(int i = 0; i < commands.Length; i++) {

            if(commands[i] != "")
                switch(commands[i][0]) {

                    case '0':

                        break;

                    case '1':
                        break;

                    case '2':

                        switch(commands[i][1]) {
                            case '0':
                                Debug.Log("Command recieved");
                                AbilitiesManager.aData[int.Parse(commands[i][2].ToString())].CreateAbility(null);
                                break;
                        }
                        break;
                }
        }

    }
}
