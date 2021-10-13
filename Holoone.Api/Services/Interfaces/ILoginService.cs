using Holoone.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Holoone.Api.Services.Interfaces
{
    public interface ILoginService
    {
        Task<IEnumerable<UserPermissions>> GetLoginAsync();
        Task<UserPermissions> GetLoginAsync(string id);
        Task AddLoginAsync(UserPermissions userPermission);
        Task UpdateLoginAsync(UserPermissions userPermission);
        Task DeleteLoginAsync(UserPermissions userPermission);
    }
}
