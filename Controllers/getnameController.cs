using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using finalproject.Data;
using finalproject.Models;
using System.Data.SqlClient;
using finalproject;

    namespace finalproject.Controllers
    {
        [Route("[controller]")]
        [ApiController]
        public class getnameController : ControllerBase
        {
            // getting all book search catagory
            [HttpGet("{cat}")]
            public IEnumerable<useralls2> Get(string cat)
            {
                List<useralls2> li = new List<useralls2>();
                SqlConnection conn1 = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=project;Integrated Security=True;Pooling=False");

                string sql;
                sql = "SELECT * FROM userall where role ='" + cat + "' ";
                SqlCommand comm = new SqlCommand(sql, conn1);
                conn1.Open();
                SqlDataReader reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    li.Add(new useralls2
                    {
                        name = (string)reader["name"],
                    });

                }

                reader.Close();
                conn1.Close();
                return li;
            }

        }
    }
