namespace InAudioSystem
{



    public class InFolderData : InAudioNodeBaseData, IBankUsage
    {

        public bool OverrideParentBank;
        public InAudioBankLink BankLink;

        public bool ParentBankOverride
        {
            get { return OverrideParentBank; }
            set { OverrideParentBank = value; }
        }

        public InAudioBankLink BankConnection
        {
            get { return BankLink; }
            set { BankLink = value; }
        }
    }

}