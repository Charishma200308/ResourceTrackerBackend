using ResourceTrackerBackend.Models;
using System.Collections.Generic;

namespace ResourceTrackerBackend.DAO
{
    public interface IEmployeeOrch 
    {
        Details Add(Details emp);
        List<Details> GetAll();
        Details? Get(int id);
        bool Update(int id, Details updated);
        Details? Delete(int id);
        List<string> GetDesignations();
        List<string> GetLocations();
        List<string> GetBillableStatuses();
        List<string> GetSkills();
        void AddSkillIfNotExists(string skill);
        List<Details> GetByIds(List<int> ids);

        List<string> GetProjects();
        InviteDeatils InviteUser(int empId);

        Task BulkUpdateEmployeesAsync(BulkUpdateRequest request);

        PagedEmployeeResult GetEmployeesPaged(PagedEmployeeRequest request);

        Task AddEmployeesBulkToDbAsync(List<Details> employees);
    }


}
