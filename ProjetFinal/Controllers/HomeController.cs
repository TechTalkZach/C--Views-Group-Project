using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjetFinal.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetFinal.Controllers
{
    public class HomeController : Controller
    {
        private readonly QuizExamenContext context;

        public HomeController(QuizExamenContext _context)
        {
            context = _context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult NewQuizz()
        {
            return View();
        }
        public IActionResult SubmitNewQuizz(string UserName, string Email, int easyQuestions, int mediumQuestions, int hardQuestions)
        {
            if(UserName != null && Email != null)   //si le UserName et Email ne sont pas null
            {
                
                if(easyQuestions + mediumQuestions + hardQuestions > 0) //si le quizz contiendra plus de 0 
                {
                    ViewBag.questionsInsuffissantes = "";
                    try
                    {
                        //creation d'une nouvelle entrée dans la table Quizz
                        context.Quiz.Add(new Quiz() { UserName = UserName, Email = Email });
                        context.SaveChanges();
                        /////***************************questions faciles************************//////
                        //indice des questions faciles
                        List<int> indiceQuestionsFaciles = new List<int>();

                        //liste des questions faciles 
                        List<Question> qListEasy = context.Question.Where(question => question.CategoryId == 1).ToList();
                        if(easyQuestions > qListEasy.Count()) //si l'utilisateur a demandé plus de questions faciles qu'il y en a
                        {
                            easyQuestions = qListEasy.Count();
                            Debug.WriteLine(easyQuestions);
                        }
                        Random rnd = new Random();
                        while (indiceQuestionsFaciles.Count()<easyQuestions) //pendant que la liste des indices n'est pas totalement remplie
                        {
                            int randomInt = rnd.Next(0, qListEasy.Count());
                            if (!indiceQuestionsFaciles.Contains(randomInt))
                            {
                                indiceQuestionsFaciles.Add(randomInt);
                            }
                        }
                        //cherchons le dernier Id du quiz qu'on vient d'insérer
                        int lastQuizId = context.Quiz.ToList().Last().QuizId;

                        indiceQuestionsFaciles.ForEach(element =>
                        {
                            context.QuestionQuiz.Add(new QuestionQuiz() { QuestionId = qListEasy[element].QuestionId, QuizId = lastQuizId });
                        });

                        /////***************************questions medium************************//////
                        //indice des questions medium
                        List<int> indiceQuestionsMedium = new List<int>();

                        //liste des questions faciles 
                        List<Question> qListMedium = context.Question.Where(question => question.CategoryId == 2).ToList();
                        if (mediumQuestions > qListMedium.Count()) //si l'utilisateur a demandé plus de questions faciles qu'il y en a
                        {
                            mediumQuestions = qListMedium.Count();
                        }
                        
                        while (indiceQuestionsMedium.Count() < mediumQuestions) //pendant que la liste des indices n'est pas totalement remplie
                        {
                            int randomInt = rnd.Next(0, qListMedium.Count());
                            if (!indiceQuestionsMedium.Contains(randomInt))
                            {
                                indiceQuestionsMedium.Add(randomInt);
                            }
                        }
                        
                        indiceQuestionsMedium.ForEach(element =>
                        {
                            context.QuestionQuiz.Add(new QuestionQuiz() { QuestionId = qListMedium[element].QuestionId, QuizId = lastQuizId });
                        });

                        /////***************************questions hard************************//////
                        //indice des questions hard
                        List<int> indiceQuestionsHard = new List<int>();

                        //liste des questions faciles 
                        List<Question> qListHard = context.Question.Where(question => question.CategoryId == 3).ToList();
                        if (hardQuestions > qListHard.Count()) //si l'utilisateur a demandé plus de questions faciles qu'il y en a
                        {
                            hardQuestions = qListHard.Count();
                        }

                        while (indiceQuestionsHard.Count() < hardQuestions) //pendant que la liste des indices n'est pas totalement remplie
                        {
                            int randomInt = rnd.Next(0, qListHard.Count());
                            if (!indiceQuestionsHard.Contains(randomInt))
                            {
                                indiceQuestionsHard.Add(randomInt);
                            }
                        }

                        indiceQuestionsHard.ForEach(element =>
                        {
                            context.QuestionQuiz.Add(new QuestionQuiz() { QuestionId = qListHard[element].QuestionId, QuizId = lastQuizId });
                        });
                        ViewBag.quizzSaved = "Quizz généré!!";
                        context.SaveChanges();
                    }
                    catch (DataException)
                    {
                        ModelState.AddModelError("", "Error, try again");
                    }
                }
                else
                {
                    ViewBag.questionsInsuffissantes = "Veuillez entrer une quantité de questions pour le quizz!!!";

                }


            }
        
            return View("NewQuizz");
        }

        public IActionResult passQuiz(int QuizId, string UserName, string Email)
        {
            ViewBag.quizzezFound = false;
            if (UserName!=null && Email != null)
            {
                var listQuizUser = context.Quiz.Where(quiz => quiz.UserName == UserName && quiz.Email == Email);
                if (listQuizUser.Count() > 0)
                {
                    ViewBag.quizzezFound = true;
                    ViewBag.noUserFound = "";
                    ViewBag.listeQuizes = listQuizUser;
                    //var user1 = new Quiz() { QuizId = listQuizUser.First().QuizId, UserName = listQuizUser.First().UserName, Email = listQuizUser.First().Email };
                    
                }
                else
                {
                    ViewBag.noUserFound = "Aucun quiz a été trouvé pour cet utilisateur";
                    return View();
                }
            }
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
