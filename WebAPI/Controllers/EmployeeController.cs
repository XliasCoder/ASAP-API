using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using WebAPI.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public EmployeeController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        [HttpGet]
        public JsonResult Get()
        {
            string query = @"
                    select EmployeeId, EmployeeName, Department,
                    convert(varchar(10),DateOfJoining,120) as DateOfJoining
                    ,PhotoFileName
                    from dbo.Employee
                    ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader); ;

                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }


        [HttpPost]
        public JsonResult Post(Employee emp)
        {
            try
            {
                if (String.IsNullOrEmpty(emp.EmployeeName) || String.IsNullOrEmpty(emp.Department) || emp==null)
                {
                    return new JsonResult("All fields are required");
                }
                else
                {
                    string query = @"
                    insert into dbo.Employee 
                    (EmployeeName,Department,DateOfJoining,PhotoFileName)Values (@EmployeeName, @Department, @DateOfJoining, @PhotoFileName)";
                    
                    DataTable table = new DataTable();
                    string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
                    SqlDataReader myReader;
                    using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                    {
                        myCon.Open();
                        using (SqlCommand myCommand = new SqlCommand(query, myCon))
                        {
                            myCommand.Parameters.AddWithValue("@EmployeeName", emp.EmployeeName);
                            myCommand.Parameters.AddWithValue("@Department", emp.Department);
                            myCommand.Parameters.AddWithValue("@DateOfJoining", emp.DateOfJoining);
                            myCommand.Parameters.AddWithValue("@PhotoFileName", emp.PhotoFileName);
                            myReader = myCommand.ExecuteReader();
                            table.Load(myReader); ;

                            myReader.Close();
                            myCon.Close();
                        }
                    }
                }
                return new JsonResult("Added Successfully");
            }
            catch(Exception e) 
            {
                return new JsonResult("ERROR: "+ e.Message);
            }

            
        }


        [HttpPut]
        public JsonResult Put(Employee emp)
        {
            try
            {
                string query = @"
                    update dbo.Employee set 
                    EmployeeName = '" + emp.EmployeeName + @"'
                    ,Department = '" + emp.Department + @"'
                    ,DateOfJoining = '" + emp.DateOfJoining + @"'
                    where EmployeeId = " + emp.EmployeeId + @" 
                    ";
                DataTable table = new DataTable();
                string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
                SqlDataReader myReader;
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader); ;

                        myReader.Close();
                        myCon.Close();
                    }
                }
                return new JsonResult("Updated Successfully");
            }
            catch (Exception e) 
            {
                return new JsonResult("Updated Failed: ERROR: "+ e.Message);
            }

        }


        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            try
            {
                string query = @"
                    delete from dbo.Employee
                    where EmployeeId = " + id + @" 
                    ";
                DataTable table = new DataTable();
                string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
                SqlDataReader myReader;
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader); ;

                        myReader.Close();
                        myCon.Close();
                    }
                }

                return new JsonResult("Deleted Successfully");
            }catch(Exception e) 
            {
                return new JsonResult("Deleted Failed: "+ e.Message);
            }
        }


        [Route("SaveFile")]
        [HttpPost]
        public JsonResult SaveFile()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string filename = postedFile.FileName;
                var physicalPath = _env.ContentRootPath + "/Photos/" + filename;

                using(var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                return new JsonResult(filename);
            }
            catch (Exception)
            {

                return new JsonResult("ERROR_anonymous.png");
            }
        }


        [Route("GetAllDepartmentNames")]
        public JsonResult GetAllDepartmentNames()
        {
            try
            {
                string query = @"
                    select DepartmentName from dbo.Department
                    ";
                DataTable table = new DataTable();
                string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
                SqlDataReader myReader;
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader); ;

                        myReader.Close();
                        myCon.Close();
                    }
                }

                return new JsonResult(table);
            }catch(Exception e) 
            {
                return new JsonResult(e.Message);
            }
        }


    }
}
