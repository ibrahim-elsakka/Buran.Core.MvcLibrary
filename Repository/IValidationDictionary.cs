namespace Buran.Core.MvcLibrary.Repository
{
    public interface IValidationDictionary
    {
        bool IsValid { get; }
        void AddError(string key, string errorMessage);
    }
}