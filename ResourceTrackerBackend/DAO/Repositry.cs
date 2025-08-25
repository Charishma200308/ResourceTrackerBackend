using Microsoft.Data.SqlClient;
using ResourceTrackerBackend.Models;
using System.Data;

namespace ResourceTrackerBackend.DAO
{
    public class Repositry : IEmployeeOrch
    {
        private readonly string _connectionString;
        private readonly ILogger<Repositry> _logger;

        public Repositry(IConfiguration configuration, ILogger<Repositry> logger)
        {
            _connectionString = configuration.GetConnectionString("EmployeeDb");
            _logger = logger;
        }

        public Details Add(Details emp)
        {
            try
            {
                using SqlConnection conn = new(_connectionString);
                using SqlCommand cmd = new("AddResource", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@name", emp.name);
                cmd.Parameters.AddWithValue("@dsgntion", emp.dsgntion);
                cmd.Parameters.AddWithValue("@reporting", emp.reporting);
                cmd.Parameters.AddWithValue("@billable", emp.billable);
                cmd.Parameters.AddWithValue("@skills", emp.skills); 
                cmd.Parameters.AddWithValue("@projalloc", emp.projalloc); 
                cmd.Parameters.AddWithValue("@location", emp.location);
                cmd.Parameters.AddWithValue("@mail", emp.mail);
                cmd.Parameters.AddWithValue("@doj", emp.doj);
                cmd.Parameters.AddWithValue("@remarks", emp.remarks);

                SqlParameter outputIdParam = new("@NewEmpId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputIdParam);

                conn.Open();
                cmd.ExecuteNonQuery();

                int result = (int)outputIdParam.Value;

                if (result == -1)
                {
                    return null;
                }

                emp.EmpId = result;
                return emp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding new employee.");
                throw;
            }
        }

        public List<Details> GetAll()
        {
            var list = new List<Details>();

            try
            {
                using SqlConnection conn = new(_connectionString);
                using SqlCommand cmd = new("GetAllResources" , conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new Details
                    {
                        EmpId = Convert.ToInt32(reader["EmpId"]),
                        name = reader["name"].ToString(),
                        dsgntion = reader["dsgntion"].ToString(),
                        reporting = reader["reporting"].ToString(),
                        billable = reader["billable"].ToString(),
                        skills = reader["skills"].ToString(),          
                        projalloc = reader["projalloc"].ToString(),    
                        location = reader["location"].ToString(),
                        mail = reader["mail"].ToString(),
                        doj = Convert.ToDateTime(reader["doj"]).ToString("yyyy-MM-dd"),
                        remarks = reader["remarks"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving all employees.");
            }

            return list;
        }

        public Details? Get(int id)
        {
            try
            {
                using SqlConnection conn = new(_connectionString);
                using SqlCommand cmd = new("GetResourceById", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@EmpId", id);

                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new Details
                    {
                        EmpId = Convert.ToInt32(reader["EmpId"]),
                        name = reader["name"].ToString(),
                        dsgntion = reader["dsgntion"].ToString(),
                        reporting = reader["reporting"].ToString(),
                        billable = reader["billable"].ToString(),
                        skills = reader["skills"].ToString(),
                        projalloc = reader["projalloc"].ToString(),
                        location = reader["location"].ToString(),
                        mail = reader["mail"].ToString(),
                        doj = Convert.ToDateTime(reader["doj"]).ToString("yyyy-MM-dd"),
                        remarks = reader["remarks"].ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving employee with ID {id}.");
            }

            return null;
        }

        public bool Update(int id, Details emp)
        {
            try
            {
                using SqlConnection conn = new(_connectionString);
                using SqlCommand cmd = new("UpdateResource", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@EmpId", id);
                cmd.Parameters.AddWithValue("@name", emp.name);
                cmd.Parameters.AddWithValue("@dsgntion", emp.dsgntion);
                cmd.Parameters.AddWithValue("@reporting", emp.reporting);
                cmd.Parameters.AddWithValue("@billable", emp.billable);
                cmd.Parameters.AddWithValue("@skills", emp.skills);
                cmd.Parameters.AddWithValue("@projalloc", emp.projalloc);
                cmd.Parameters.AddWithValue("@location", emp.location);
                cmd.Parameters.AddWithValue("@mail", emp.mail);
                cmd.Parameters.AddWithValue("@doj", emp.doj);
                cmd.Parameters.AddWithValue("@remarks", emp.remarks);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating employee with ID {id}.");
                return false;
            }
        }

        public Details? Delete(int id)
        {
            try
            {
                var emp = Get(id);
                if (emp == null) return null;

                using SqlConnection conn = new(_connectionString);
                using SqlCommand cmd = new("DeleteResource", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@EmpId", id);

                conn.Open();
                cmd.ExecuteNonQuery();
                return emp;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting employee with ID {id}.");
                return null;
            }
        }

        public List<string> GetDesignations()
        {
            var list = new List<string>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT dsgntion FROM dbo.Designation ORDER BY dsgntion", conn);
            conn.Open();
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read()) list.Add(rdr.GetString(0));
            return list;
        }

        public List<string> GetLocations()
        {
            var list = new List<string>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT location FROM dbo.Location ORDER BY location", conn);
            conn.Open();
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read()) list.Add(rdr.GetString(0));
            return list;
        }

        public List<string> GetBillableStatuses()
        {
            var list = new List<string>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT billable FROM dbo.BillableStatus ORDER BY billable", conn);
            conn.Open();
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read()) list.Add(rdr.GetString(0));
            return list;
        }

        public List<string> GetSkills()
        {
            var list = new List<string>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT skill FROM dbo.Skill ORDER BY skill", conn);
            conn.Open();
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(rdr.GetString(0));
            }
            return list;
        }

        public void AddSkillIfNotExists(string skillName)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(@"
            IF NOT EXISTS (SELECT 1 FROM dbo.Skill WHERE skill = @skill)
            BEGIN
                INSERT INTO dbo.Skill (skill) VALUES (@skill)
            END", conn);

            cmd.Parameters.AddWithValue("@skill", skillName.Trim());

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public List<Details> GetByIds(List<int> ids)
        {
            var result = new List<Details>();

            if (ids == null || ids.Count == 0)
                return result;

            try
            {
                using SqlConnection conn = new(_connectionString);
                using SqlCommand cmd = conn.CreateCommand();

                var parameters = string.Join(",", ids.Select((id, index) => $"@id{index}"));
                cmd.CommandText = $"SELECT * FROM EmployeeDetails WHERE EmpId IN ({parameters})";

                for (int i = 0; i < ids.Count; i++)
                {
                    cmd.Parameters.AddWithValue($"@id{i}", ids[i]);
                }

                conn.Open();
                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    result.Add(new Details
                    {
                        EmpId = Convert.ToInt32(reader["EmpId"]),
                        name = reader["name"].ToString(),
                        dsgntion = reader["dsgntion"].ToString(),
                        reporting = reader["reporting"].ToString(),
                        billable = reader["billable"].ToString(),
                        skills = reader["skills"].ToString(),
                        projalloc = reader["projalloc"].ToString(),
                        location = reader["location"].ToString(),
                        mail = reader["mail"].ToString(),
                        doj = Convert.ToDateTime(reader["doj"]).ToString("yyyy-MM-dd"),
                        remarks = reader["remarks"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching employees by ID list.");
            }

            return result;
        }

        public List<string> GetProjects()
        {
            var list = new List<string>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT projalloc FROM dbo.Project ORDER BY projalloc", conn);
            conn.Open();
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                list.Add(rdr.GetString(0));
            }
            return list;
        }

        public InviteDeatils InviteUser(int empId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("dbo.InviteUser", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@EmpId", empId);

            var usernameParam = new SqlParameter("@Username", SqlDbType.NVarChar, 50) { Direction = ParameterDirection.Output };
            var passwordParam = new SqlParameter("@Password", SqlDbType.NVarChar, 100) { Direction = ParameterDirection.Output };
            var resultParam = new SqlParameter("@Result", SqlDbType.Int) { Direction = ParameterDirection.Output };

            cmd.Parameters.Add(usernameParam);
            cmd.Parameters.Add(passwordParam);
            cmd.Parameters.Add(resultParam);

            conn.Open();
            cmd.ExecuteNonQuery();

            int result = (int)resultParam.Value;

            if (result <= 0)
            {
                throw new InvalidOperationException("InviteUser failed with code: " + result);
            }

            return new InviteDeatils
            {
                Username = usernameParam.Value.ToString(),
                Password = passwordParam.Value.ToString(),
                UserId = result
            };
        }

        public async Task BulkUpdateEmployeesAsync(BulkUpdateRequest request)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var employeeIdsTable = new DataTable();
            employeeIdsTable.Columns.Add("EmpId", typeof(int));
            foreach (var id in request.EmployeeIds)
            {
                employeeIdsTable.Rows.Add(id);
            }

            using var command = new SqlCommand("dbo.BulkUpdateEmployeesByIds", connection);
            command.CommandType = CommandType.StoredProcedure;

            var tvpParam = command.Parameters.AddWithValue("@EmpIds", employeeIdsTable);
            tvpParam.SqlDbType = SqlDbType.Structured;
            tvpParam.TypeName = "dbo.BulkEmployeeUpdateType";  

            command.Parameters.AddWithValue("@dsgntion", (object?)request.dsgntion ?? DBNull.Value);
            command.Parameters.AddWithValue("@reporting", (object?)request.reporting ?? DBNull.Value);
            command.Parameters.AddWithValue("@billable", (object?)request.billable?? DBNull.Value); 
            command.Parameters.AddWithValue("@skills", (object?)request.skills ?? DBNull.Value);
            command.Parameters.AddWithValue("@projalloc", (object?)request.projalloc ?? DBNull.Value);
            command.Parameters.AddWithValue("@location", (object?)request.location ?? DBNull.Value);
            command.Parameters.AddWithValue("@doj", (object?)request.doj ?? DBNull.Value);

            await command.ExecuteNonQueryAsync();
        }

        public PagedEmployeeResult GetEmployeesPaged(PagedEmployeeRequest request)
        {
            var result = new PagedEmployeeResult
            {
                Employees = new List<Details>()
            };

            try
            {
                // Step 1: Get all employees using your existing SP
                List<Details> allEmployees = GetAll();

                // Step 2: Apply filtering
                if (request.Filters != null && request.Filters.Any())
                {
                    foreach (var filter in request.Filters)
                    {
                        var field = filter.Field?.ToLower();
                        var value = filter.Value?.ToLower();

                        if (string.IsNullOrEmpty(field) || string.IsNullOrEmpty(value))
                            continue;

                        allEmployees = field switch
                        {
                            "name" => allEmployees.Where(e => e.name?.ToLower().Contains(value) ?? false).ToList(),
                            "dsgntion" => allEmployees.Where(e => e.dsgntion?.ToLower().Contains(value) ?? false).ToList(),
                            "projalloc" => allEmployees.Where(e => e.projalloc?.ToLower().Contains(value) ?? false).ToList(),
                            "location" => allEmployees.Where(e => e.location?.ToLower().Contains(value) ?? false).ToList(),
                            "skills" => allEmployees.Where(e => e.skills?.ToLower().Contains(value) ?? false).ToList(),
                            _ => allEmployees
                        };
                    }
                }


                // Step 3: Apply sorting
                switch (request.SortColumn?.ToLower())
                {
                    case "name":
                        allEmployees = request.SortDir == "desc"
                            ? allEmployees.OrderByDescending(e => e.name).ToList()
                            : allEmployees.OrderBy(e => e.name).ToList();
                        break;

                    case "dsgntion":
                        allEmployees = request.SortDir == "desc"
                            ? allEmployees.OrderByDescending(e => e.dsgntion).ToList()
                            : allEmployees.OrderBy(e => e.dsgntion).ToList();
                        break;

                    case "projalloc":
                        allEmployees = request.SortDir == "desc"
                            ? allEmployees.OrderByDescending(e => e.projalloc).ToList()
                            : allEmployees.OrderBy(e => e.projalloc).ToList();
                        break;

                    default:
                        allEmployees = allEmployees.OrderBy(e => e.EmpId).ToList();
                        break;
                }

                // Step 4: Set TotalCount before paging
                result.TotalCount = allEmployees.Count;

                // Step 5: Apply paging
                result.Employees = allEmployees
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LINQ-based GetEmployeesPaged.");
            }

            return result;
        }

        public async Task AddEmployeesBulkToDbAsync(List<Details> employees)
        {
            var table = ConvertToDataTable(employees);

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand("dbo.AddResourcesBulk", connection);
            command.CommandType = CommandType.StoredProcedure;

            var param = command.Parameters.AddWithValue("@Employees", table);
            param.SqlDbType = SqlDbType.Structured;
            param.TypeName = "dbo.EmployeeType";

            await command.ExecuteNonQueryAsync();
        }

        private DataTable ConvertToDataTable(List<Details> employees)
        {
            var dt = new DataTable();

            dt.Columns.Add("name", typeof(string));
            dt.Columns.Add("dsgntion", typeof(string));
            dt.Columns.Add("reporting", typeof(string));
            dt.Columns.Add("billable", typeof(string));
            dt.Columns.Add("skills", typeof(string));
            dt.Columns.Add("projalloc", typeof(string));
            dt.Columns.Add("location", typeof(string));
            dt.Columns.Add("mail", typeof(string));
            dt.Columns.Add("doj", typeof(string));
            dt.Columns.Add("remarks", typeof(string));

            foreach (var emp in employees)
            {
                dt.Rows.Add(emp.name, emp.dsgntion, emp.reporting, emp.billable, emp.skills, emp.projalloc, emp.location, emp.mail, emp.doj, emp.remarks);
            }

            return dt;
        }
    }

}


//using Microsoft.Data.SqlClient;
//using ResourceTrackerBackend.Models;
//using System.Collections.Concurrent;
//using System.Data;

//namespace ResourceTrackerBackend.DAO
//{
//    public class Repositry : IEmployeeOrch
//    {
//        private readonly string _connectionString;
//        private readonly ILogger<Repositry> _logger;

//        public Repositry(IConfiguration configuration, ILogger<Repositry> logger)
//        {
//            _connectionString = configuration.GetConnectionString("EmployeeDb");
//            _logger = logger;
//        }
//        public Details Add(Details emp)
//        {
//            try
//            {
//                using SqlConnection conn = new(_connectionString);
//                using SqlCommand cmd = new("AddResource", conn)
//                {
//                    CommandType = CommandType.StoredProcedure
//                };

//                //cmd.Parameters.AddWithValue("@EmpId", emp.EmpId);
//                cmd.Parameters.AddWithValue("@name", emp.name);
//                cmd.Parameters.AddWithValue("@dsgntion", emp.dsgntion);
//                cmd.Parameters.AddWithValue("@reporting", emp.reporting);
//                cmd.Parameters.AddWithValue("@billable", emp.billable);
//                cmd.Parameters.AddWithValue("@skills", emp.skills);
//                cmd.Parameters.AddWithValue("@projalloc", emp.projalloc);
//                cmd.Parameters.AddWithValue("@location", emp.location);
//                cmd.Parameters.AddWithValue("@mail", emp.mail);
//                cmd.Parameters.AddWithValue("@doj", emp.doj);
//                cmd.Parameters.AddWithValue("@remarks", emp.remarks);

//                SqlParameter outputIdParam = new("@NewEmpId", SqlDbType.Int)
//                {
//                    Direction = ParameterDirection.Output
//                };
//                cmd.Parameters.Add(outputIdParam);

//                conn.Open();
//                cmd.ExecuteNonQuery();

//                int result = (int)outputIdParam.Value;

//                if (result == -1)
//                {
//                    return null;
//                }

//                emp.EmpId = result;
//                return emp;
//            }

//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error while adding new employee.");
//                return null;
//            }
//        }

//        public List<Details> GetAll()
//        {
//            var list = new List<Details>();

//            try
//            {
//                using SqlConnection conn = new(_connectionString);
//                using SqlCommand cmd = new("GetAllResources", conn)
//                {
//                    CommandType = CommandType.StoredProcedure
//                };

//                conn.Open();
//                using SqlDataReader reader = cmd.ExecuteReader();

//                while (reader.Read())
//                {
//                    list.Add(new Details
//                    {
//                        EmpId = Convert.ToInt32(reader["EmpId"]),
//                        name = reader["name"].ToString(),
//                        dsgntion = reader["dsgntion"].ToString(),
//                        reporting = reader["reporting"].ToString(),
//                        billable = reader["billable"].ToString(),
//                        skills = reader["skills"].ToString(),
//                        projalloc = reader["projalloc"].ToString(),
//                        location = reader["location"].ToString(),
//                        mail = reader["mail"].ToString(),
//                        doj = Convert.ToDateTime(reader["doj"]).ToString("yyyy-MM-dd"),
//                        remarks = reader["remarks"].ToString()
//                    });
//                }
//            }

//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error while retrieving all employees.");
//            }

//            return list;

//        }

//        public Details? Get(int id)
//        {
//            try
//            {
//                using SqlConnection conn = new(_connectionString);
//                using SqlCommand cmd = new("GetResourceById", conn)
//                {
//                    CommandType = CommandType.StoredProcedure
//                };

//                cmd.Parameters.AddWithValue("@EmpId", id);

//                conn.Open();
//                using SqlDataReader reader = cmd.ExecuteReader();

//                if (reader.Read())
//                {
//                    return new Details
//                    {
//                        EmpId = Convert.ToInt32(reader["EmpId"]),
//                        name = reader["name"].ToString(),
//                        dsgntion = reader["dsgntion"].ToString(),
//                        reporting = reader["reporting"].ToString(),
//                        billable = reader["billable"].ToString(),
//                        skills = reader["skills"].ToString(),
//                        projalloc = reader["projalloc"].ToString(),
//                        location = reader["location"].ToString(),
//                        mail = reader["mail"].ToString(),
//                        doj = Convert.ToDateTime(reader["doj"]).ToString("yyyy-MM-dd"),
//                        remarks = reader["remarks"].ToString()
//                    };
//                }
//            }

//            catch (Exception ex)
//            {
//                _logger.LogError(ex, $"Error retrieving employee with ID {id}.");
//            }

//            return null;
//        }

//        public bool Update(int id, Details emp)
//        {
//            try
//            {
//                using SqlConnection conn = new(_connectionString);
//                using SqlCommand cmd = new("UpdateResource", conn)
//                {
//                    CommandType = CommandType.StoredProcedure
//                };

//                cmd.Parameters.AddWithValue("@EmpId", id);
//                cmd.Parameters.AddWithValue("@name", emp.name);
//                cmd.Parameters.AddWithValue("@dsgntion", emp.dsgntion);
//                cmd.Parameters.AddWithValue("@reporting", emp.reporting);
//                cmd.Parameters.AddWithValue("@billable", emp.billable);
//                cmd.Parameters.AddWithValue("@skills", emp.skills);
//                cmd.Parameters.AddWithValue("@projalloc", emp.projalloc);
//                cmd.Parameters.AddWithValue("@location", emp.location);
//                cmd.Parameters.AddWithValue("@mail", emp.mail);
//                cmd.Parameters.AddWithValue("@doj", emp.doj);
//                cmd.Parameters.AddWithValue("@remarks", emp.remarks);

//                conn.Open();
//                int rowsAffected = cmd.ExecuteNonQuery();
//                return rowsAffected > 0;
//            }

//            catch (Exception ex)
//            {
//                _logger.LogError(ex, $"Error updating employee with ID {id}.");
//                return false;
//            }
//        }

//        public Details? Delete(int id)
//        {
//            try
//            {
//                var emp = Get(id);
//                if (emp == null) return null;

//                using SqlConnection conn = new(_connectionString);
//                using SqlCommand cmd = new("DeleteResource", conn)
//                {
//                    CommandType = CommandType.StoredProcedure
//                };

//                cmd.Parameters.AddWithValue("@EmpId", id);

//                conn.Open();
//                cmd.ExecuteNonQuery();
//                return emp;
//            }

//            catch (Exception ex)
//            {
//                _logger.LogError(ex, $"Error deleting employee with ID {id}.");
//                return null;
//            }
//        }

//    }
//}
