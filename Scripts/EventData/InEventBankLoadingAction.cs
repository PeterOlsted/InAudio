using InAudioSystem;
using UnityEngine;

namespace InAudioSystem
{
    public enum BankHookActionType
    {
        Load = 0,
        Unload = 1,
    }


    public class InEventBankLoadingAction : AudioEventAction
    {
        public InAudioBankLink BankLink;
        public BankHookActionType LoadingAction = BankHookActionType.Load;


        public override Object Target
        {
            get { return BankLink; }
            set
            {
                if (value is InAudioBankLink)
                    BankLink = value as InAudioBankLink;
            }
        }

        public override string ObjectName
        {
            get
            {
                if (BankLink != null)
                    return BankLink.GetName;
                else
                {
                    return "Missing Bank";
                }
            }
        }
    }

}