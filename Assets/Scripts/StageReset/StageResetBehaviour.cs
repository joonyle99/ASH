using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(StageResetLight), typeof(CutscenePlayer))]
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
    }

    private void Update()
    {

    }

    protected override void OnObjectInteractionEnter()
    {
        DialogueController.Instance.StartDialogue(_dialogueData, false);

        List<ResponseContainer> responseFunctions = new List<ResponseContainer>();
        responseFunctions.Add(new ResponseContainer(ResponseButtonType.Accept, AcceptStageReset));
        responseFunctions.Add(new ResponseContainer(ResponseButtonType.Reject, RejectStageReset));
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
        // 최신 데이터 (PersistentData) 가 아닌 씬의 원본 데이터를 가져오기 위해 스테이지 리셋 대상 데이터를 삭제
        PersistentDataManager.Instance.PersistentData.DataGroups[_currentSceneBasicGroupName].Clear();
        // 씬 전환 타입 변경
        SceneChangeManager.Instance.SceneChangeType = SceneChangeType.StageReset;
        // 몬스터가 리스폰 대기하는 코루틴을 정지한다 (씬 전환 코루틴은 유지되기 때문)
        MonsterRespawnManager.Instance.StopRespawnCoroutine();
        // ...
        BossDungeonManager.Instance.MakeDataGroup();
        // 
        SceneChangeManager.Instance.ChangeToPlayableScene(_sceneName, _entrancePassage);
    }
}
