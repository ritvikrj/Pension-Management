using JsonWebToken.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JsonWebToken
{
    public class AppDBContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
            LoadUsers();
        }

        public void LoadUsers()
        {
            
            using (var reader = new StreamReader(@"data.csv"))
            {
                List<string> listA = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');
                    listA.Add(values[0]);
                }
                for (int i = 0; i <20; i++)
                {
                    var eachline = listA[i].Split(',');
                    User user =  new User() { Aadhar= eachline[5], Name = eachline[0] };
                    Users.Add(user);   
                }
            }
            
            //user = new User() { Aadhar = 102, Name = "Admin" };
            // Users.Add(user);
        }

        public List<User> GetUsers()
        {
            return Users.Local.ToList<User>();
        }
    }
}

