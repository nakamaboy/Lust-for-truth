using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace DIALOGUE
{
    public class DialogSystem : MonoBehaviour
    {
        public DialogueContainer dialogueContainer = new DialogueContainer();
        public static DialogSystem instance;
        // Start is called before the first frame update
        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                DestroyImmediate(gameObject);

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}

