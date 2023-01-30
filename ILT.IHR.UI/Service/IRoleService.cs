using ILT.IHR.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ILT.IHR.UI.Service
{
    public interface IRoleService
    {
        Task<Response<List<Role>>> GetRoles();
        Task<IEnumerable<Module>> GetModules();
        Task<Response<Role>> GetRoleByIdAsync(int Id);
        Task<Response<RolePermission>> GetRolePermissionByIdAsync(int Id);
        Task<Response<RolePermission>> UpdateRolePermission(int Id, RolePermission updateObject);
        Task<Response<RolePermission>> SaveRolePermission(RolePermission obj);
        Task DeleteRolePermission(int id);
    }
}
