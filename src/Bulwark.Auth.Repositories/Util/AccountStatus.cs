using Bulwark.Auth.Repositories.Model;

namespace Bulwark.Auth.Repositories.Util;

public static class AccountStatus
{
    public static bool Healthy(AccountModel account)
    {
        if (account.IsDeleted)
        {
            throw new System.Exception("Account has been deleted");
        }

        if (!account.IsEnabled)
        {
            throw new System.Exception("Account has been disabled");
        }

        if (!account.IsVerified)
        {
            throw new System.Exception("Account has not been verified");
        }

        return true;
    }
}


