using CRUDApiJWT.Models;
using CRUDApiJWT.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CRUDApiJWT.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/employees")]
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeService _svc;
        public EmployeesController(EmployeeService svc) => _svc = svc;

        [HttpGet]
        [Authorize(Roles = "hr,admin")]
        public async Task<IActionResult> GetAll() =>
            Ok(await _svc.GetAll());

        [HttpGet("{empId}")]
        public async Task<IActionResult> GetById(int empId)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var userEmpId = int.Parse(User.FindFirst("userId")!.Value);

            if (role == "employee" && empId != userEmpId)
                return Forbid();

            var emp = await _svc.GetById(empId);
            return emp == null ? NotFound() : Ok(emp);
        }

        //[HttpPost]
        ////[Authorize(Roles = "hr,admin")]
        //public async Task<IActionResult> Create(Employee e)
        //{
        //    await _svc.Create(e);
        //    return CreatedAtAction(nameof(GetById), new { empId = e.EmpId }, e);
        //}

        [HttpPut("{empId}")]
        //[Authorize(Roles = "hr,admin")]
        public async Task<IActionResult> Update(int empId, Employee e)
        {
            await _svc.Update(empId, e);
            return NoContent();
        }

        [HttpDelete("{empId}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int empId)
        {
            await _svc.Delete(empId);
            return NoContent();
        }
    }
}
