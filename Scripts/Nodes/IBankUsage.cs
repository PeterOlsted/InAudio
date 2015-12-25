namespace InAudioSystem
{
    public interface IBankUsage 
    {
        bool ParentBankOverride { get; set; }
        InAudioBankLink BankConnection { get; set; }
    }
}