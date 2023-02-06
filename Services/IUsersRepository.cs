using test_crud.Entities;

namespace test_crud.Services
{
    public interface IUsersRepository: IRepositoryBase<Users>
    {
        string GetAuthenticatedUserId();

        string GenerateJwtToken(Guid Id, bool isRefreshToken);
    }
}
