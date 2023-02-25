using System.Collections.Generic;
using Bulwark.Auth.Repositories.Exception;
using Bulwark.Auth.Repositories.Model;
using Bulwark.Auth.Repositories.Tests.Models;
using Bulwark.Auth.TestFixture;


namespace Bulwark.Auth.Repositories.Tests
{
    public class MongoDbAuthorizationTests : IClassFixture<MongoDbRandomFixture>
    {
        private readonly IAuthorizationRepository _authorizationRepository;
        private readonly IMongoCollection<RoleModel> _mongoRoleCollection;
        private readonly IMongoCollection<Account> _mongoAccountCollection;
        private readonly MongoDbRandomFixture _dbFixture;

        public MongoDbAuthorizationTests(MongoDbRandomFixture dbFixture)
        {
            _dbFixture = dbFixture;
            _authorizationRepository = new MongoDbAuthorization(dbFixture.Db);
            _mongoRoleCollection = dbFixture.Db.GetCollection<RoleModel>("role");
            _mongoAccountCollection = dbFixture.Db.GetCollection<Account>("account");
        }

        [Fact]
        public async void ReadAccountRoles()
        {
            await CreateRole("role1");
            await CreateRole("role2");
            var roles = await ReadRole("role1");
            var roles2 = await ReadRole("role2");
            var roleIds = new List<string>();
            roleIds.Add(roles.Id);
            roleIds.Add(roles2.Id);
            var userId = await CreateAccount(roleIds);
            var userRoles =
                await _authorizationRepository.ReadAccountRoles(userId.ToString());
            Assert.Equal(2, userRoles.Count);
        }
        
        [Fact]
        public async void ReadAccountPermissions()
        {
            await CreateRole("role1");
            await CreateRole("role2");
            var roles = await ReadRole("role1");
            var roles2 = await ReadRole("role2");
            var roleIds = new List<string>();
            roleIds.Add(roles.Id);
            roleIds.Add(roles2.Id);
            var userId = await CreateAccount(roleIds);
            var userRoles =
                await _authorizationRepository.ReadAccountPermissions(userId.ToString());
            Assert.Equal(2, userRoles.Count);
        }

        private async Task<string> CreateAccount(List<string> roles)
        {
            var account = new Account
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Email = $"test-{Guid.NewGuid().ToString()}@lateflip.io",
                Roles = roles,
                IsVerified = true,
                IsEnabled = true,
                IsDeleted = false,
                Created = DateTime.Now,
                Modified = DateTime.Now
            };

            await _mongoAccountCollection.InsertOneAsync(account);
            return account.Id;

        }
        private async Task CreateRole(string role)
        {
            try
            {
                var roleModel = new RoleModel()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Name = role,
                    Permissions = new List<string>(),
                    Created = DateTime.Now,
                    Modified = DateTime.Now
                };

                roleModel.Permissions.Add("test:write");
                roleModel.Permissions.Add("test:read");

                await _mongoRoleCollection.InsertOneAsync(roleModel);
            }
            catch (MongoWriteException e)
            {
                if (e.WriteError.Category == ServerErrorCategory.DuplicateKey)
                {
                    throw new BulwarkDbDuplicateException($"Duplicate key: {role}", e);
                }

                throw new BulwarkDbException($"Error creating role: {role}", e);
            }
        }

        private async Task<RoleModel> ReadRole(string name)
        {
            try
            {
                var role = await _mongoRoleCollection
                    .Find(x => x.Name == name)
                    .FirstOrDefaultAsync();
                if (role == null)
                {
                    throw new BulwarkDbException($"Role: {name} not found");
                }

                return role;
            }
            catch (MongoException e)
            {
                throw new BulwarkDbException($"Error reading role: {name}", e);
            }
        }
    }
}
