using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DIALOGUE;

namespace TESTING
{
    public class TestParsing : MonoBehaviour
    {
 
        
        void Start()
        {
            SendFileToParse();

            //string line = "Speaker \"Dialogue goes in here!\" Command(arguments here)";
            //DialogueParser.Parse(line);

        }
        void SendFileToParse()
        {
            List<string> lines = FileManager.ReadTextAsset("Vol 2" , false);
            foreach (string line in lines)
            {
                DIALOGUE_LINE dl = DialogueParser.Parse(line); 
            }

        }

    }
}
