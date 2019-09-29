using DotNetCore.Objects;
using DotNetCoreArchitecture.CrossCutting.Resources;
using DotNetCoreArchitecture.Database;
using DotNetCoreArchitecture.Domain;
using DotNetCoreArchitecture.Infra;
using DotNetCoreArchitecture.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetCoreArchitecture.Application
{
    public sealed class UserApplicationService : IUserApplicationService
    {
        public UserApplicationService
        (
            ISignInService signInService,
            IUnitOfWork unitOfWork,
            IUserLogApplicationService userLogApplicationService,
            IUserRepository userRepository
        )
        {
            SignInService = signInService;
            UnitOfWork = unitOfWork;
            UserLogApplicationService = userLogApplicationService;
            UserRepository = userRepository;
        }

        private ISignInService SignInService { get; }

        private IUnitOfWork UnitOfWork { get; }

        private IUserLogApplicationService UserLogApplicationService { get; }

        private IUserRepository UserRepository { get; }

        public async Task<IDataResult<long>> AddAsync(AddUserModel addUserModel)
        {
            var validation = new AddUserModelValidator().Valid(addUserModel);

            if (validation.IsError)
            {
                return DataResult<long>.Error(validation.Message);
            }

            addUserModel.SignIn = SignInService.CreateSignIn(addUserModel.SignIn);

            var userEntity = UserEntityFactory.Create(addUserModel);

            userEntity.Add();

            await UserRepository.AddAsync(userEntity);

            await UnitOfWork.SaveChangesAsync();

            return DataResult<long>.Success(userEntity.UserId);
        }

        public async Task<IResult> DeleteAsync(long userId)
        {
            await UserRepository.DeleteAsync(userId);

            await UnitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task InactivateAsync(long userId)
        {
            var userEntity = UserEntityFactory.Create(userId);

            userEntity.Inactivate();

            await UserRepository.UpdateStatusAsync(userEntity);

            await UnitOfWork.SaveChangesAsync();
        }

        public async Task<PagedList<UserModel>> ListAsync(PagedListParameters parameters)
        {
            return await UserRepository.ListAsync<UserModel>(parameters);
        }

        public async Task<IEnumerable<UserModel>> ListAsync()
        {
            return await UserRepository.ListAsync<UserModel>();
        }

        public async Task<UserModel> SelectAsync(long userId)
        {
            return await UserRepository.SelectAsync<UserModel>(userId);
        }

        public async Task<IDataResult<TokenModel>> SignInAsync(SignInModel signInModel)
        {
            var validation = new SignInModelValidator().Valid(signInModel);

            if (validation.IsError)
            {
                return DataResult<TokenModel>.Error(validation.Message);
            }

            var signedInModel = await UserRepository.SignInAsync(signInModel);

            if (!SignInService.Validate(signedInModel, signInModel))
            {
                return DataResult<TokenModel>.Error(Texts.LoginPasswordInvalid);
            }

            var userLogModel = new UserLogModel(signedInModel.UserId, LogType.SignIn);

            await UserLogApplicationService.AddAsync(userLogModel);

            await UnitOfWork.SaveChangesAsync();

            var tokenModel = SignInService.CreateToken(signedInModel);

            return DataResult<TokenModel>.Success(tokenModel);
        }

        public async Task SignOutAsync(SignOutModel signOutModel)
        {
            var userLogModel = new UserLogModel(signOutModel.UserId, LogType.SignOut);

            await UserLogApplicationService.AddAsync(userLogModel);

            await UnitOfWork.SaveChangesAsync();
        }

        public async Task<IResult> UpdateAsync(UpdateUserModel updateUserModel)
        {
            var validation = new UpdateUserModelValidator().Valid(updateUserModel);

            if (validation.IsError)
            {
                return Result.Error(validation.Message);
            }

            var userEntity = await UserRepository.SelectAsync(updateUserModel.UserId);

            if (userEntity == default)
            {
                return Result.Success();
            }

            userEntity.ChangeEmail(updateUserModel.Email);

            userEntity.ChangeFullName(updateUserModel.FullName.Name, updateUserModel.FullName.Surname);

            await UserRepository.UpdateAsync(userEntity.UserId, userEntity);

            await UnitOfWork.SaveChangesAsync();

            return Result.Success();
        }
    }
}
