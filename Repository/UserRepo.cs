using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using SignWithMe.Interfaces;
using SignWithMe.Models;

namespace SignWithMe.Repository
{
    public class UserRepo : IUser
    {
        public UserRepo(string pConString)
        {
            Connection = new NpgsqlConnection(pConString);
            Connection.Open();
            string CREATE_USER_TABLE = @"create table if not exists players(
	            Id SERIAL PRIMARY KEY,
	            Username VARCHAR(50) UNIQUE NOT NULL,
	            Password VARCHAR(50) NOT NULL,
	            Level VARCHAR(50) NOT NULL,
                Score VARCHAR(50) NOT NULL,
                Result VARCHAR(50) NOT NULL
            );";
            Connection.Execute(CREATE_USER_TABLE);
        }
        public void AddUser(User user)
        {
            string addQsql = @"insert into players(Username,Password,Level,Score,Result)
                            values(@Username,@Password,@Level,@Score,@Result)";

            Connection.Execute(addQsql, 
            new User()
            {
                 Username = user.Username,
                 Password = user.Password,
                 Level = user.Level,
                 Score = user.Score,
                 Result = user.Result
            });
        }

        public bool AuthUser(User pUser)
        {
            var user = UserbyName(pUser.Username);
            if(user.Password == pUser.Password)
            {
                return true;
            }
            return false;
        }

        public IEnumerable<User> GetAllUsers()
        {
             var users = Connection.Query<User>(@"select * from players");
             return users;
        }

        public User UserbyName(string pUsername)
        {
            var template = new User{ Username =  pUsername };
            var parameters = new DynamicParameters(template);
            string sql = @"select * from players where Username = @Username";
            var user = Connection.QueryFirstOrDefault<User>(sql, parameters);
            return user ;
        }
        private NpgsqlConnection Connection {get; set;}
    }
}