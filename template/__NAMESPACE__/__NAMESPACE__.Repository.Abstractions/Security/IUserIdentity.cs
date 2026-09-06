namespace __NAMESPACE__.Repository.Abstractions.Security
{
    public interface IUserIdentity
    {
        Guid GetSubject();
        string GetUserName();
    }
}
