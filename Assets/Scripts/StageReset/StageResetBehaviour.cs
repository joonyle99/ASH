using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(LightNPC), typeof(CutscenePlayer))]
public class StageResetBehaviour : InteractableObject
{
    [Header("Dialogue")]
    [SerializeField] private DialogueData _dialogueData;

    [Header("CutScene")]
    [SerializeField] private CutscenePlayer _cutscenePlayer;

    private LightNPC _lightNPC;

    private string _sceneName = "";
    private string _entrancePassage = "";

    private void Awake()
    {
        _lightNPC = GetComponent<LightNPC>();
    }
    private void Start()
    {
        _sceneName = SceneManager.GetActiveScene().name;
        _entrancePassage = "Enter " + _sceneName;
    }

    private void Update()
    {

    }

    protected override void OnObjectInteractionEnter()
    {
        DialogueController.Instance.StartDialogue(_dialogueData);

        List<ResponseFunctionContainer> responseFunctions = new List<ResponseFunctionContainer>();
        responseFunctions.Add(new ResponseFunctionContainer(ResponseButtonType.Reject, RejectStageReset));
        responseFunctions.Add(new ResponseFunctionContainer(ResponseButtonType.Accept, AcceptStageReset));
        DialogueController.Instance.View.OpenResponsePanel02(responseFunctions);
    }

    public override void UpdateInteracting()
    {
        if (!DialogueController.Instance.IsDialogueActive)
        {
            ExitInteraction();
        }
    }

    private void AcceptStageReset()
    {
        DialogueController.Instance.ShutdownDialogue();
        _cutscenePlayer.Play();
    }

    private void RejectStageReset()
    {
        DialogueController.Instance.ShutdownDialogue();
    }
    public void ResetStage()
    {
        PersistentDataManager.Instance.PersistentData._dataGroups.Clear();
        SceneChangeManager.Instance.ChangeToPlayableScene(_sceneName, _entrancePassage);
    }
}
