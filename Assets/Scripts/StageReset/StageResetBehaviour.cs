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
    [SerializeField, Tooltip("���� ���� �Ϲ����� ���ӿ�����Ʈ�� PreserveState GroupName")]
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
        // �ֽ� ������ (PersistentData) �� �ƴ� ���� ���� �����͸� �������� ���� �������� ���� ��� �����͸� ����
        PersistentDataManager.Instance.PersistentData.DataGroups[_currentSceneBasicGroupName].Clear();
        // �� ��ȯ Ÿ�� ����
        SceneChangeManager.Instance.SceneChangeType = SceneChangeType.StageReset;
        // ���Ͱ� ������ ����ϴ� �ڷ�ƾ�� �����Ѵ� (�� ��ȯ �ڷ�ƾ�� �����Ǳ� ����)
        MonsterRespawnManager.Instance.StopRespawnCoroutine();
        // ...
        BossDungeonManager.Instance.MakeDataGroup();
        // 
        SceneChangeManager.Instance.ChangeToPlayableScene(_sceneName, _entrancePassage);
    }
}
