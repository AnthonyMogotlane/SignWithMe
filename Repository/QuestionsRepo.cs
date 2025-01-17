using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Dapper;
using SignWithMe.Interfaces;
using SignWithMe.Models;

namespace SignWithMe.Repository
{
    public class QuestionsRepo : IQuestions
    {
        public QuestionsRepo(string pConString)
        {
            Connection = new NpgsqlConnection(pConString);
            Connection.Open();
            string CREATE_QUES_TABLE = @"create table if not exists questions(
	            Id SERIAL PRIMARY KEY,
	            QuestionName VARCHAR(150) NOT NULL,
	            Answer VARCHAR(150) NOT NULL
            );";
            Connection.Execute(CREATE_QUES_TABLE);
        }
        public void AddQuestion(Question pQuestion)
        {
            string addQsql = @"insert into questions(QuestionName, Answer)
                            values(@QuestionName, @Answer)";
            Connection.Execute(addQsql, new Question(){ QuestionName= pQuestion.QuestionName, Answer = pQuestion.Answer});
        }
        public IEnumerable<Question> GetAllQuestions()
        {
             var Questions = Connection.Query<Question>(@"select * from questions");
             return Questions;
        }
        public Question GetQuestionById(int id)
        {
             var template = new Question { Id = id };
            var parameters = new DynamicParameters(template);
            string sql = @"select * from  questions where id = @id";
            var question = Connection.QueryFirstOrDefault<Question>(sql, parameters);
            return question;
        }

        private NpgsqlConnection Connection {get; set;}

    }
}