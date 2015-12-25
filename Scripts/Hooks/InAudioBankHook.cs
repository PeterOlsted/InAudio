using System.Collections.Generic;
using InAudioSystem;
using UnityEngine;

[AddComponentMenu("InAudio/Event Hook/Audio Bank Hook")]
public class InAudioBankHook : MonoBehaviour
{
    public BankHookActions EnableActions = new BankHookActions("On Enable");

    public BankHookActions StartActions = new BankHookActions("On Start");

    public BankHookActions DisableActions = new BankHookActions("On Disable");

    public BankHookActions DestroyActions = new BankHookActions("On Destroy");

    void OnEnable()
    {
        for (int i = 0; i < EnableActions.Actions.Count; ++i)
        {
            var action = EnableActions.Actions[i];
            LoadOrUnload(action);
        }
    }

    void Start()
    {
        for (int i = 0; i < StartActions.Actions.Count; ++i)
        {
            var action = StartActions.Actions[i];
            LoadOrUnload(action);
        }
    }

    void OnDisable()
    {
        for (int i = 0; i < DisableActions.Actions.Count; ++i)
        {
            var action = DisableActions.Actions[i];
            LoadOrUnload(action);

        }
    }

    void OnDestroy()
    {
        for (int i = 0; i < DestroyActions.Actions.Count; ++i)
        {
            var action = DestroyActions.Actions[i];
            LoadOrUnload(action);

        }
    }

    private static void LoadOrUnload(BankHookAction action)
    {
        if (action.BankAction == BankHookActionType.Load)
            InAudio.LoadBank(action.Bank);
        else if (action.BankAction == BankHookActionType.Unload)
            InAudio.UnloadBank(action.Bank);
    }
}

namespace InAudioSystem
{
    [System.Serializable]
    public class BankHookActions
    {
        public BankHookActions(string title)
        {
            Title = title;
        }

        [SerializeField] 
        public string Title;

        [SerializeField] 
        public List<BankHookAction> Actions = new List<BankHookAction>();
    }

    [System.Serializable]
    public class BankHookAction
    {
        [SerializeField]
        public InAudioBankLink Bank;

        [SerializeField]
        public BankHookActionType BankAction = BankHookActionType.Load;
    }

    

}
