namespace TestAssignment.Services.Interfaces
{
    internal interface IUserDialog
    {
        void ShowInformation(string Message, string Caption);
        void ShowWarning(string Message, string Caption);
        void ShowError(string Message, string Caption);
        bool Confirm(string Message, string Caption, bool Exclamation = false);
    }
}
