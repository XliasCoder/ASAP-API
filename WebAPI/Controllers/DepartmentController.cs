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

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public DepartmentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public JsonResult Get()
        {
            try
            {
                string query = @"
                    select DepartmentId, DepartmentName from dbo.Department";
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
            catch (Exception e)
            {
                return new JsonResult("ERROR: " + e.Message);
            }
        }


        [HttpPost]
        public JsonResult Post(Department dep)
        {
            try
            {
                if (String.IsNullOrEmpty(dep.DepartmentName)) 
                {
                    return new JsonResult("Required Field");

                }

                string query = "INSERT INTO dbo.Department VALUES(@dept)";
                DataTable table = new DataTable();
                string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
                SqlDataReader myReader;
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@dept", dep.DepartmentName);
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader); ;

                        myReader.Close();
                        myCon.Close();
                    }
                }

                return new JsonResult("Added Successfully");
            }catch(Exception e) 
            {
                return new JsonResult("ERROR: "+ e.Message);

            }
        }


        [HttpPut]
        public JsonResult Put(Department dep)
        {
            try
            {
                string query = @"
                    update dbo.Department set 
                    DepartmentName = '" + dep.DepartmentName + @"'
                    where DepartmentId = " + dep.DepartmentId + @" 
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
            }catch(Exception e) 
            {
                return new JsonResult("Updated Failed: "+ e.Message);

            }
        }


        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            try
            {
                string query = @"
                    delete from dbo.Department
                    where DepartmentId = " + id + @" 
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
                return new JsonResult("Deletion Failed: " + e.Message);
            }
        }
    }
}
