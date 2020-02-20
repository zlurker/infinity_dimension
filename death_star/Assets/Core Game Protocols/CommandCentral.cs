using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandCentral : MonoBehaviour {

    string splitCommands;

    //Parse commands can be 
    public void ParseCommands(string command) {

        bool keepLastCommand = false;
        keepLastCommand = command[command.Length - 1] != '\n' ? true : false;

        string[] commands = command.Split('\n');
        commands[0] = splitCommands + commands[0];
        splitCommands = keepLastCommand ? commands[commands.Length - 1] : "";

        int commandLen = keepLastCommand ? commands.Length - 1 : commands.Length;

        for(int i = 0; i < commands.Length; i++)
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
