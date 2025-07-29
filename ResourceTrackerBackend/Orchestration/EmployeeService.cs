using ResourceTrackerBackend.Models;
using ResourceTrackerBackend.DAO;
using System.Collections.Generic;

namespace ResourceTrackerBackend.Orchestration
{
    public class EmployeeService
    {
        private readonly IEmployeeOrch _repository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(IEmployeeOrch repository, ILogger<EmployeeService> logger)
        {
            _repository = repository;
            _logger = logger;
        }


        public Details AddEmployee(Details emp)
        {
            try
            {
                return _repository.Add(emp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding employee.");
                return null;
            }
        }
        public List<Details> GetAllEmployees()
        {
            try
            {
                return _repository.GetAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all employees.");
                return new List<Details>();
            }
        }


        public Details? GetEmployee(int id)
        {
            try
            {
                return _repository.Get(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving employee with ID {id}.");
                return null;
            }
        }

        public bool UpdateEmployee(int id, Details updated)
        {
            try
            {
                return _repository.Update(id, updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating employee with ID {id}.");
                return false;
            }
        }
        public Details? DeleteEmployee(int id)
        {
            try
            {
                return _repository.Delete(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting employee with ID {id}.");
                return null;
            }
        }

        public List<string> GetDesignations() => _repository.GetDesignations();
        public List<string> GetLocations() => _repository.GetLocations();
        public List<string> GetBillableStatuses() => _repository.GetBillableStatuses();
        public List<string> GetSkills() => _repository.GetSkills();

        public void AddSkillIfNotExists(string skill)
        {
            try
            {
                _repository.AddSkillIfNotExists(skill);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding skill: {skill}");
            }
        }

        public List<(int EmpId, bool Success)> BulkUpdateEmployees(List<Details> updatedEmployees)
        {
            var result = new List<(int EmpId, bool Success)>();

            foreach (var emp in updatedEmployees)
            {
                if (!emp.EmpId.HasValue)
                {
                    _logger.LogWarning("Skipping employee update due to missing EmpId.");
                    result.Add((-1, false));
                    continue;
                }

                try
                {
                    var success = _repository.Update(emp.EmpId.Value, emp);
                    result.Add((emp.EmpId.Value, success));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating employee with ID {emp.EmpId}.");
                    result.Add((emp.EmpId.Value, false));
                }
            }

            return result;
        }

        public List<Details> GetEmployeesByIds(List<int> ids)
        {
            return _repository.GetByIds(ids);
        }

        public List<string> GetProjects() => _repository.GetProjects();


    }
}
