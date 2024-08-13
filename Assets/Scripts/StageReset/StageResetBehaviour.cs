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

    [Header("PreserveState")]
    [SerializeField, Tooltip("현재 씬의 일반적인 게임오브젝트의 PreserveState GroupName")]
    private string _currentSceneBasicGroupName;

    private void Awake()
    {
        _lightNPC = GetComponent<LightNPC>();
    }
    private void Start()
    {
        _sceneName = SceneManager.GetActiveScene().name;
        _entrancePassage = "Enter " + _sceneName;
        Debug.Log(_entrancePassage);
    }

    private void Update()
    {

    }

    protected override void OnObjectInteractionEnter()
    {
        DialogueController.Instance.StartDialogue(_dialogueData);

        List<ResponseContainer> responseFunctions = new List<ResponseContainer>();
        responseFunctions.Add(new ResponseContainer(ResponseButtonType.Reject, RejectStageReset));
        responseFunctions.Add(new ResponseContainer(ResponseButtonType.Accept, AcceptStageReset));
        DialogueController.Instance.View.OpenResponsePanel(responseFunctions);
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
        PersistentDataManager.Instance.PersistentData.DataGroups[_currentSceneBasicGroupName].Clear();
        SceneChangeManager.Instance.SceneChangeType = SceneChangeType.StageReset;
        SceneChangeManager.Instance.ChangeToPlayableScene(_sceneName, _entrancePassage);
        BossDungeonManager.Instance.MakeDataGroup();
    }
}
