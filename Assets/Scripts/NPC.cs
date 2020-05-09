
using UnityEngine;
using Yarn.Unity;

public class NPC : MonoBehaviour
{
    public string characterName = "";

    public string talkToNode = "";

    public bool Move = false;
    public bool Inspect = false;
    public bool Interact = false;

    [Header("Optional")]
    public YarnProgram scriptToLoad;

    void Start()
    {
        if (scriptToLoad != null)
        {
            DialogueRunner dialogueRunner = FindObjectOfType<DialogueRunner>();
            dialogueRunner.Add(scriptToLoad);
        }
    }
}
