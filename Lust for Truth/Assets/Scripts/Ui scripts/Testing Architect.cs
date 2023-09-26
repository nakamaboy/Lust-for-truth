using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DIALOGUE
{
    public class TestingArchitect : MonoBehaviour
    {
        DialogSystem ds;
        TextArchitect architect;

        string[] lines = new string[8]
        {
            "YUGIOOOOH IS THE GREATEST CARD GAME EVEAAAAAAR AHHHHHHHHHHHHHHHHHHHHHHHHHH",
            "DANGIT GRANDPA, WHY YOU NOT TELL THEM MAKOTO!!!!",
            "HOLY MOLY, I'M WITH MY BOWLY",
            "JABA JUMBA. JIBIBIBI HALI JAM KARLA, ALLAH AKBAR",
            "SMEXY TIME WITH ME BABY. ONLY 999$ AN HOUR. NO, IT'S NOT EXPENSIVE, I DON'T THINK SO",
            "JOJOJOOJOJOJOJO",
            "Sayonara suckers!!!$@@$",
            "DIZNEY SUCK MY BALLLLLZZZZZZZZ..."
        };



        // Start is called before the first frame update
        void Start()
        {
            ds = DialogSystem.instance;
            architect = new TextArchitect(ds.dialogueContainer.dialogueText);
            architect.buildMethod = TextArchitect.BuildMethod.typewriter;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (architect.isBuilding)
                {
                    architect.ForceComplete();
                    //if (!architect.hurryUP)
                    //    architect.hurryUP = true;
                    //else architect.ForceComplete();
                }
                else
                    architect.Build(lines[Random.Range(0, lines.Length)]);

            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                architect.Append(lines[Random.Range(0, lines.Length)]);
            }
        }
    }

}
    

   

