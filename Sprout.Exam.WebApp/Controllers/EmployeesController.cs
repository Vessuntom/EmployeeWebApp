﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Sprout.Exam.Business.DataTransferObjects;
using Sprout.Exam.Common.Enums;
using Sprout.Exam.WebApp.Services;

namespace Sprout.Exam.WebApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        /// <summary>
        /// Refactor this method to go through proper layers and fetch from the DB.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            var result = _employeeService.GetEmployees().Select(e => e.ToDto()).ToList();
            return Ok(result);
        }

        /// <summary>
        /// Refactor this method to go through proper layers and fetch from the DB.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var result = _employeeService.GetById(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Refactor this method to go through proper layers and update changes to the DB.
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody]EditEmployeeDto input)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                var item = await _employeeService.Update(input);
                return Ok(item);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Refactor this method to go through proper layers and insert employees to the DB.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]CreateEmployeeDto input)
        {

            //var id = await Task.FromResult(StaticEmployees.ResultList.Max(m => m.Id) + 1);

            //StaticEmployees.ResultList.Add(new EmployeeDto
            //{
            //    Birthdate = input.Birthdate.ToString("yyyy-MM-dd"),
            //    FullName = input.FullName,
            //    Id = id,
            //    Tin = input.Tin,
            //    TypeId = input.TypeId
            //});


            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var id = await _employeeService.AddAsync(input);


            return Created($"/api/employees/{id}", id);
        }


        /// <summary>
        /// Refactor this method to go through proper layers and perform soft deletion of an employee to the DB.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _employeeService.DeleteEmployee(id);
                return Ok(id);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }



        /// <summary>
        /// Refactor this method to go through proper layers and use Factory pattern
        /// </summary>
        /// <param name="id"></param>
        /// <param name="absentDays"></param>
        /// <param name="workedDays"></param>
        /// <returns></returns>
        [HttpPost("calculate")]
        public async Task<IActionResult> Calculate([FromBody] CalculateDto inputDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                decimal amount = _employeeService.Calculate(inputDto.Id, inputDto.AbsentDays, inputDto.WorkedDays);
                return Ok(String.Format("{0:0.00}", Math.Round(amount, 2)));
            }
            catch(NotImplementedException e)
            {
                return NotFound("Employee Type not found");
            }
        }

    }
}
