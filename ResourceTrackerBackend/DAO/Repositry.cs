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

                // Create parameterized IN clause
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
