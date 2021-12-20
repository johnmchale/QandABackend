using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QandA.Data;
using QandA.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QandA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IDataRepository _dataRepository;

        public QuestionsController(IDataRepository dataRepository)
        {
            _dataRepository = dataRepository;
        }

        [HttpGet]
        public IEnumerable<QuestionGetManyResponse> GetQuestions(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return _dataRepository.GetQuestions();
            }
            else
            {
                return _dataRepository.GetQuestionsBySearch(search);
            }
        }

        [HttpGet("unanswered")]
        public IEnumerable<QuestionGetManyResponse> GetUnansweredQuestions()
        {
            return _dataRepository.GetUnansweredQuestions();
        }

        [HttpGet("{questionId}")]
        public ActionResult<QuestionGetSingleResponse> GetQuestion(int questionId)
        {
            // call the data repository to get the question
            var question = _dataRepository.GetQuestion(questionId);

            // return HTTP status code 404 if the question isn't found
            if (question == null)
            {
                return NotFound();
            }

            // return question in response with status code 200
            return question;
        }

        [HttpPost]
        public ActionResult<QuestionGetSingleResponse> PostQuestion(QuestionPostRequest questionPostRequest)
        {
            // call the data repository to save the question
            var savedQuestion = _dataRepository.PostQuestion(questionPostRequest);

            // return HTTP status code 201
            return CreatedAtAction(nameof(GetQuestion),
                new { questionId = savedQuestion.QuestionId },
                savedQuestion);
        }

        [HttpPut("{questionId}")]
        public ActionResult<QuestionGetSingleResponse> PutQuestion(int questionId, QuestionPutRequest questionPutRequest)
        {
            // get the question from the data repository
            // return HTTP status code 404 if the question isn't found
            var question = _dataRepository.GetQuestion(questionId);
            if (question == null)
            {
                return NotFound();
            }

            // update the question model
            questionPutRequest.Title = string.IsNullOrEmpty(questionPutRequest.Title) ?
                    question.Title :
                    questionPutRequest.Title;
            questionPutRequest.Content = string.IsNullOrEmpty(questionPutRequest.Content) ?
                    question.Content :
                    questionPutRequest.Content;

            // call the data repository with the updated question model to update the question in the database
            // return the saved question
            var savedQuestion = _dataRepository.PutQuestion(questionId, questionPutRequest);
            return savedQuestion;
        }

        [HttpDelete("{questionId}")]
        public ActionResult DeleteQuestion(int questionId)
        {
            var question = _dataRepository.GetQuestion(questionId);
            if (question == null)
            {
                return NotFound();
            }
            _dataRepository.DeleteQuestion(questionId);
            return NoContent();
        }

        [HttpPost("answer")]
        public ActionResult<AnswerGetResponse> PostAnswer(AnswerPostRequest answerPostRequest)
        {
            var questionExists = _dataRepository.QuestionExists(answerPostRequest.QuestionId.Value);
            if (!questionExists)
            {
                return NotFound();
            }
            
            var savedAnswer = _dataRepository.PostAnswer(answerPostRequest);
            return savedAnswer;
        }

    }
}
