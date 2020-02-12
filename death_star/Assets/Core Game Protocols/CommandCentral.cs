using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandCentral {

    public static void ParseCommands(string command) {

        string[] commands = command.Split('\n');

        for(int i = 0; i < commands.Length; i++) {
            switch(commands[i][0]) {

                case '0':

                    break;
            }
        }
    }
}
