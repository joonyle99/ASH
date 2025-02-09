using UnityEngine;

public class ShiningRock : InteractableObject, IAttackListener
{
    [SerializeField] DialogueData _dialogueOnFirstInteract;
    [SerializeField] DialogueData _dialogueOnFirstAttack;
    [SerializeField] DialogueData _dialogueAfterSecondAttack;
    [SerializeField] DialogueData _dialogueOnConsecutiveInteract;

    [SerializeField] CutscenePlayer _cutscenePlayer;
    [SerializeField] int _hp = 3;
    [SerializeField] GameObject _lightNPC;
    [SerializeField] SoundList _soundList;

    bool _hasPlayedDialogue = false;
    bool _isHittable = false;

    void Awake()
    {
        _lightNPC.SetActive(false);
    }

    protected override void OnObjectInteractionEnter()
    {
        if (!_hasPlayedDialogue)
        {
            DialogueController.Instance.StartDialogue(_dialogueOnFirstInteract, false);                // 거기 누구 있어?
            _hasPlayedDialogue = true;
            _isHittable = true;
        }
        else
        {
            if (_hp == 5)
                DialogueController.Instance.StartDialogue(_dialogueAfterSecondAttack, false);          // 물리적인 공격으로 부서질 거 같아
            else if (_hp == 3)
                DialogueController.Instance.StartDialogue(_dialogueOnConsecutiveInteract, false);      // 곧 부서질 거 같아
            //else if (_hp == 1)
            //    DialogueController.Instance.StartDialogue(_dialogueOnConsecutiveInteract, false);      // 곧 부서질 거 같아
        }
    }

    public override void UpdateInteracting()
    {
        if (!DialogueController.Instance.IsDialogueActive)
        {
            ExitInteraction();
            _hasPlayedDialogue = true;
        }
    }

    public IAttackListener.AttackResult OnHit(AttackInfo attackInfo)
    {
        if (!_isHittable) return IAttackListener.AttackResult.Fail;

        _hp--;
        _soundList.PlaySFX("Hit");

        if (_hp == 4)
        {
            DialogueController.Instance.StartDialogue(_dialogueOnFirstAttack, false);                  // 어 부서지는 소리가 들려
        }
        else if (_hp == 0)
        {
            _lightNPC.transform.parent = transform.parent;
            _lightNPC.SetActive(true);
            _cutscenePlayer.Play();
            _soundList.PlaySFX("Die");
            Destruction.Destruct(gameObject);
        }
        return IAttackListener.AttackResult.Success;
    }
}
