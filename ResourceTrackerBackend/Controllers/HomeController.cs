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
        public async Task<IActionResult> BulkUpdate([FromBody] BulkUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _employeeService.BulkUpdateEmployeesAsync(request);
                return Ok(new { Message = "Bulk update successful." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Bulk update failed.", Details = ex.Message });
            }
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

        [HttpPost("invite/{empId}")]
        public IActionResult Invite(int empId)
        {
            var result = _employeeService.InviteEmployee(empId);
            if (result == null)
                return BadRequest("Failed to invite employee.");

            return Ok(result); // returns username and password
        }

        [HttpPost("paged")]
        public IActionResult GetEmployeesPaged([FromBody] PagedEmployeeRequest request)
        {
            try
            {
                var result = _employeeService.GetEmployeesPaged(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving paged employee list.");
                return StatusCode(500, new { message = "Error retrieving employee list.", error = ex.Message });
            }
        }

        [HttpPost("bulk-add")]
        public async Task<IActionResult> AddEmployeesBulk([FromBody] List<Details> employees)
        {
            if (employees == null || !employees.Any())
                return BadRequest("Employee list is empty");

            await _employeeService.AddEmployeesBulkAsync(employees);

            return Ok(new { message = "Employees added successfully." });
        }
    }
}
