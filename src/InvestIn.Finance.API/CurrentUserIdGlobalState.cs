using HotChocolate;

namespace InvestIn.Finance.API
{
    public class CurrentUserIdGlobalState : GlobalStateAttribute
    {
        public CurrentUserIdGlobalState() : base("currentUserId")
        {
        }
    }
}