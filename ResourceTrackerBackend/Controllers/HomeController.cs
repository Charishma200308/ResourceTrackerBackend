using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ResourceTrackerBackend.Models;
using ResourceTrackerBackend.Orchestration;

namespace ResourceTrackerBackend.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class EmployeeDetailsController : ControllerBase
    {
        private readonly EmployeeService _employeeService;
        private readonly ILogger<EmployeeDetailsController> _logger;

        public EmployeeDetailsController(EmployeeService employeeService, ILogger<EmployeeDetailsController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        [HttpPost("AddEmployee")]
        public IActionResult AddEmployee([FromBody] Details empData)
        {
            try
            {
                var addedEmployee = _employeeService.AddEmployee(empData);

                if (addedEmployee == null)
                {
                    return BadRequest(new { message = "Email already exists." });
                }

                _logger.LogInformation("Employee added successfully with ID: {EmpId}", addedEmployee.EmpId);
                return Ok(new { message = "Employee Added", addedEmployee.EmpId });
            }

            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Error occured while adding Employee");
                return StatusCode(500, new { message = "An error occurred while adding employee.", error = ex.Message });
            }
        }

        [HttpGet("GetAll")]
        public IActionResult GetAllEmployees()
        {
            Task.Delay(1000);
            try
            {
                var employees = _employeeService.GetAllEmployees();

                _logger.LogInformation("Retrieved {Count} employees.", employees.Count);
                return Ok(new { message = "Employee Added", employees });
            }

            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Error occured while fetching Employees data");
                return StatusCode(500, new { message = "An error occurred while fetching employees.", error = ex.Message });
            }
        }

        [HttpGet("{empId}")]
        public IActionResult GetEmployee(int empId)
        {
            try
            {
                var employee = _employeeService.GetEmployee(empId);
                if (employee == null)
                    return NotFound(new { message = "Employee not Found" });

                _logger.LogInformation("Employee with ID {EmpId} retrived.", empId);

                return Ok(employee);
            }

            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Error occured while retriving Employee data with Id {Empid}", empId);

                return StatusCode(500, new { message = "An error occurred while fetching employee.", error = ex.Message });
            }
        }

        [HttpPut("{empId}")]
        public IActionResult UpdateEmployee(int empId, [FromBody] Details updatedDetails)
        {
            try
            {
                var success = _employeeService.UpdateEmployee(empId, updatedDetails);
                if (!success)
                    return NotFound(new { message = "Employee not Found" });

                _logger.LogInformation("Employee with ID {EmpId} updated.", empId);

                return Ok(updatedDetails);
            }

            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Error occured while updating Employee data with Id {Empid}", empId);

                return StatusCode(500, new { message = "An error occurred while updating employee.", error = ex.Message });
            }
        }

        [HttpDelete("{empId}")]
        public IActionResult DeleteEmployee(int empId)
        {
            Task.Delay(1000);
            try
            {
                var removedEmployee = _employeeService.DeleteEmployee(empId);
                if (removedEmployee == null)
                    return NotFound(new { message = "Employee not Found" });

                _logger.LogInformation("Employee with ID {EmpId} deleted successfully.", empId);
                return Ok(removedEmployee);
            }

            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Error occured while deleting Employee Id with {Empid}", empId);
                return StatusCode(500, new { message = "An error occurred while deleting employee.", error = ex.Message });
            }
        }

        [HttpGet("designations")]
        public ActionResult<IEnumerable<string>> GetDesignations() =>
            Ok(_employeeService.GetDesignations());

        [HttpGet("locations")]
        public ActionResult<IEnumerable<string>> GetLocations() =>
            Ok(_employeeService.GetLocations());

        [HttpGet("billableStatuses")]
        public ActionResult<IEnumerable<string>> GetBillableStatuses() =>
            Ok(_employeeService.GetBillableStatuses());

        [HttpGet("skills")]
        public ActionResult<IEnumerable<string>> GetSkills() =>
            Ok(_employeeService.GetSkills());

        [HttpPost("skills/add")]
        public IActionResult AddSkill([FromBody] string skill)
        {
            if (string.IsNullOrWhiteSpace(skill))
                return BadRequest(new { message = "Skill cannot be empty" });

            _employeeService.AddSkillIfNotExists(skill);
            return Ok(new { message = "Skill added (if not already present)" });
        }

        [HttpPut("bulk-update")]
        public IActionResult BulkUpdateEmployees([FromBody] List<Details> employees)
        {
            if (employees == null || employees.Count == 0)
                return BadRequest(new { message = "No employee data provided." });

            var updateResults = _employeeService.BulkUpdateEmployees(employees);

            var summary = new
            {
                Total = updateResults.Count,
                SuccessCount = updateResults.Count(x => x.Success),
                Failed = updateResults.Where(x => !x.Success).Select(x => x.EmpId).ToList()
            };

            return Ok(new { message = "Bulk update completed.", summary });
        }

        [HttpPost("get-by-ids")]
        public IActionResult GetEmployeesByIds([FromBody] List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return BadRequest(new { message = "No IDs provided." });

            var employees = _employeeService.GetEmployeesByIds(ids);

            return Ok(employees);
        }

        [HttpGet("projects")]
        public ActionResult<IEnumerable<string>> GetProjects()
        {
            try
            {
                var projects = _employeeService.GetProjects();
                return Ok(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving project list.");
                return StatusCode(500, new { message = "Failed to retrieve projects.", error = ex.Message });
            }
        }

    }
}
