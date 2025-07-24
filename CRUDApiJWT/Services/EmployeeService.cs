using CRUDApiJWT.Data;
using CRUDApiJWT.Models;
using Npgsql;

namespace CRUDApiJWT.Services
{
    public class EmployeeService
    {
        private readonly DbHelper _db;
        public EmployeeService(DbHelper db)
        {
            _db = db;
        }

        public async Task<List<Employee>> GetAll()
        {
            return await _db.ExecuteFunctionAsync("sp_GetAllEmployees",   r => new Employee
            {
                EmpId = (int)r["empid"],
                Name = (string)r["name"],
                JoinDate = (DateTime)r["joindate"],
                DOB = (DateTime)r["dob"],
                Gender = (string)r["gender"],
                Department = (string)r["department"],
                Designation = (string)r["designation"]
            });
        }

        public async Task<Employee?> GetById(int empId)
        {
            var param = new[] 
            { 
                new NpgsqlParameter("empid", empId) 
            };

            var sql = "SELECT empid, name, joindate, dob, gender, department, designation FROM employees WHERE empid = @empid";

            return await _db.ExecuteRawItemAsync(sql, r => new Employee
            {
                EmpId = (int)r["empid"],
                Name = (string)r["name"],
                JoinDate = (DateTime)r["joindate"],
                DOB = (DateTime)r["dob"],
                Gender = (string)r["gender"],
                Department = (string)r["department"],
                Designation = (string)r["designation"]
            }, param);
        }

        //public async Task Create(Employee e)
        //{
        //    var pars = new[] {
        //        new NpgsqlParameter("p_empid", e.EmpId),
        //        new NpgsqlParameter("p_name", e.Name),
        //        new NpgsqlParameter("p_joindate", e.JoinDate),
        //        new NpgsqlParameter("p_dob", e.DOB),
        //        new NpgsqlParameter("p_gender", e.Gender),
        //        new NpgsqlParameter("p_dept", e.Department),
        //        new NpgsqlParameter("p_desig", e.Designation)
        //    };
        //    await _db.ExecuteNonQueryAsync("sp_CreateEmployee", pars);
        //}

        public async Task Update(int empId, Employee e)
        {
            var pars = new[] {
                new NpgsqlParameter("p_empid", empId),
                new NpgsqlParameter("p_name", e.Name),
                new NpgsqlParameter("p_joindate", e.JoinDate),
                new NpgsqlParameter("p_dob", e.DOB),
                new NpgsqlParameter("p_gender", e.Gender),
                new NpgsqlParameter("p_dept", e.Department),
                new NpgsqlParameter("p_desig", e.Designation)
            };
            await _db.ExecuteNonQueryAsync("sp_UpdateEmployee", pars);
        }

        public async Task Delete(int empId)
        {
            var pars = new[] { new NpgsqlParameter("p_empid", empId) };
            await _db.ExecuteNonQueryAsync("sp_DeleteEmployee", pars);
        }
    }
}