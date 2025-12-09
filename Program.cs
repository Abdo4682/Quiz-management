using System;
using System.Linq;
using System.Collections.Frozen;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace project
{
    class Program
    {
        public static string checkForNull(string input)
        {
            while (string.IsNullOrWhiteSpace(input) || input == "Not provided")
            {
                Console.Write("You must enter a value try again: ");
                input = Console.ReadLine() ?? "Not provided";
            }
            return input;
        }
        class Question
        {
            public string Text;
            public string [] Choices;
            public string CorrectChoice;
            public string [] Keywords;
            public string Type;
            public Question(string _Text, string[] _Choices, string _CorrectChoice, string [] _Keywords, string _Type)
            {
                Text = _Text;
                Choices = _Choices;
                CorrectChoice = _CorrectChoice;
                Keywords = _Keywords;
                Type = _Type;
            }
        }
        class Quiz
        {
            public List<Question> Questions = new List<Question>();
            protected string id;
            protected string topic;
            protected string difficultylevel = "Not determined yet";
            
            public Quiz(string _id, string _topic)
            {
                id = _id;
                topic = _topic;
            }

            public string getQuizID()
            {
                return id;
            }
            public string getTopic()
            {
                return topic;
            }
            public virtual string getDifficultyLevel(int avg)
            {
                return difficultylevel;
            }

            public string info()
            {
                return $"Quiz id: {id}, topic: {topic}";
            }
        }   

        class MultipleChoiceQuiz : Quiz
        {
            public MultipleChoiceQuiz(string _id, string _topic) : base(_id, _topic) {}
            public string [] choices_arr = Array.Empty<string>();
            public void addQuestion()
            {
                Console.Write("Enter question text: ");
                string questionText = Console.ReadLine() ?? "Not provided";
                questionText = Program.checkForNull(questionText);

                int choices_num;
                Console.Write("Enter number of choices: ");
                int.TryParse(Console.ReadLine(), out choices_num);

                Console.Write("Enter answer choices (seperated by commas): ");
                string choices_str = Console.ReadLine() ?? "Not provided"; 
                choices_arr = choices_str.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim()).ToArray();

                while (choices_arr.Length != choices_num)
                {
                    Console.Write($"number of choices != {choices_num} \nEnter choices again (seperated by commas):");
                    choices_str = Console.ReadLine() ?? "Not provided";
                    choices_arr = choices_str.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim()).ToArray();
                }

                Console.Write("Enter the correct choice: ");
                string correct = Console.ReadLine() ?? "Not provided";
                correct = Program.checkForNull(correct);

                bool exists = false;
                while (!exists)
                {
                    foreach(string choice in choices_arr)
                    {
                        if (correct.ToLower() == choice.ToLower())
                        {
                            exists = true;
                        }
                    }
                    if (!exists)
                    {
                        Console.Write("Correct answer doesn't exist in the choices try again: ");
                        correct = Console.ReadLine() ?? "Not provided";
                        correct = Program.checkForNull(correct);
                    }
                }

                Questions.Add(new Question(questionText, choices_arr, correct, Array.Empty<string>(), "mcq"));  
            }
            public override string getDifficultyLevel(int avg_choices_num)
            {
                if (avg_choices_num > 4)
                {
                    return "Hard";
                }
                else if(avg_choices_num == 4)
                {
                    return "Medium";
                }
                else
                {
                    return "Easy";
                }
            }
        }

        class EssayQuiz : Quiz
        {
            public EssayQuiz(string _id, string _topic) : base(_id, _topic) {}
            public string [] keywords_arr = Array.Empty<string>();
            public void addQuestion()
            {
                Console.Write("Enter question text: ");
                string questionText = Console.ReadLine() ?? "Not provided";
                questionText = Program.checkForNull(questionText);

                Console.Write("Enter keywords for correct answer (seperated by commas): ");
                string keywords_str = Console.ReadLine() ?? "Not provided";
                keywords_str = Program.checkForNull(keywords_str);

                keywords_arr = keywords_str.Split(",").Select(c => c.Trim()).ToArray();  
                Questions.Add(new Question(questionText, Array.Empty<string>(), string.Empty, keywords_arr, "essay"));            
            }
            public override string getDifficultyLevel(int avg_keywords_num)
            {
                if (avg_keywords_num > 5)
                {
                    return "Hard";
                }
                else if (avg_keywords_num == 5)
                {
                    return "Medium";
                }
                else
                {
                    return "Easy";
                }
            }
        }

        class Student
        {
            public string id;
            public string name;
            public int grade;
            public string getStudentID()
            {
                return id;
            }
            public string getName()
            {
                return name;
            }
            public int getGrade()
            {
                return grade;
            }
            public Student(string _id, string _name, int _grade)
            {
                id = _id;
                name = _name;
                grade = _grade;
            }
        }

        class Grade
        {
            public string studentName;
            public string studentID;
            public string quizTopic;
            public int mark;
            public int fullMark;
            public string getStudentName()
            {
                return studentName;
            }
            public string getStudentID()
            {
                return studentID;
            }
            public string getQuizTopic()
            {
                return quizTopic;
            }
            public int getMark()
            {
                return mark;
            }
            public int getFullMark()
            {
                return fullMark;
            }
            public Grade(string _studentID, string _quizTopic, int _mark, int _fullMark, string _studentName)
            {
                studentID = _studentID;
                quizTopic = _quizTopic;
                mark = _mark;
                fullMark = _fullMark;
                studentName = _studentName;
            }
        }

        class QuizManager
        {
            private List<MultipleChoiceQuiz> McqQuizzes = new List<MultipleChoiceQuiz>();
            private List<EssayQuiz> EssayQuizzes = new List<EssayQuiz>();
            private List<Student> Students = new List<Student>();
            private List<Grade> Grades = new List<Grade>();
            public void createQuiz()
            {
                Console.Write("Enter quiz type (Mcq or Essay): ");
                string type = Console.ReadLine() ?? "Not provided";
                while (type.ToLower() != "mcq" && type.ToLower() != "essay")
                {
                    Console.Write("Wrong type try again (Mcq or Essay): ");
                    type = Console.ReadLine() ?? "Not provided";
                }

                Console.Write("Enter quiz id: ");
                string ID = Console.ReadLine() ?? "Not provided";
                ID = Program.checkForNull(ID);

                Console.Write("Enter quiz topic: ");
                string topic = Console.ReadLine() ?? "Not provided";
                topic = Program.checkForNull(topic);

                if (type.ToLower() == "mcq")
                {
                    McqQuizzes.Add(new MultipleChoiceQuiz(ID, topic));
                    Console.WriteLine("Quiz was created successfully.");
                }
                else if (type.ToLower() == "essay")
                {
                    EssayQuizzes.Add(new EssayQuiz(ID, topic));
                    Console.WriteLine("Quiz was created successfully.");
                }
            }

            public void addQuestionToQuiz()
            {
                Console.Write("Enter quiz type (Mcq or Essay): ");
                string type = Console.ReadLine() ?? "Not provided";
                while (type.ToLower() != "mcq" && type.ToLower() != "essay")
                {
                    Console.Write("Wrong type try again (Mcq or Essay): ");
                    type = Console.ReadLine() ?? "Not provided";
                }

                Console.Write("Enter quiz id: ");
                string ID = Console.ReadLine() ?? "Not provided";
                ID = Program.checkForNull(ID);     

                if (type.ToLower() == "mcq")
                {
                    foreach(MultipleChoiceQuiz quiz in McqQuizzes)
                    {
                        if (quiz.getQuizID() == ID)
                        {
                            quiz.addQuestion();
                            Console.WriteLine("Question was added successfully.");
                            return;
                        }
                    }
                    Console.WriteLine("Quiz id was not found ");     
                }      
                else if (type.ToLower() == "essay")
                {
                    foreach(EssayQuiz quiz in EssayQuizzes)
                    {
                        if (quiz.getQuizID() == ID)
                        {
                            quiz.addQuestion();
                            Console.WriteLine("Question was added successfully.");
                            return;
                        }
                    }
                    Console.WriteLine("Quiz id was not found ");
                }
            }

            public void gradeQuiz()
            {
                Console.Write("Enter quiz type (Mcq or Essay): ");
                string type = Console.ReadLine() ?? "Not provided";
                while (type.ToLower() != "mcq" && type.ToLower() != "essay")
                {
                    Console.Write("Wrong type try again (Mcq or Essay): ");
                    type = Console.ReadLine() ?? "Not provided";
                }

                Console.Write("Enter quiz ID: ");
                string quizID = Console.ReadLine() ?? "Not provided";
                quizID = Program.checkForNull(quizID);    

                Console.Write("Enter student ID: ");
                string studentID = Console.ReadLine() ?? "Not provided";
                studentID = Program.checkForNull(studentID);

                Console.Write("Enter student name: ");
                string studentName = Console.ReadLine() ?? "Not provided";
                studentName = Program.checkForNull(studentName);                

                int i = 1;
                int mark = 0;
                int fullmark = 0;
                if (type.ToLower() == "mcq")
                {
                    foreach (MultipleChoiceQuiz quiz in McqQuizzes)
                    {
                        if (quiz.getQuizID() == quizID)       
                        {    
                            foreach (Question q in quiz.Questions)
                            {                                  
                                Console.Write($"Enter student's answer for Q{i}: ");
                                i++;
                                string answer = Console.ReadLine() ?? "Not provided";
                                answer = Program.checkForNull(answer);
                                if (answer == q.CorrectChoice)
                                {
                                    mark++;
                                }
                                fullmark++;
                            }
                            Console.WriteLine($"{studentName} scored {mark} out of {fullmark} in {quiz.getTopic()}.");                           
                            Students.Add(new Student(studentID, studentName, mark));
                            Grades.Add(new Grade(studentID, quiz.getTopic(), mark, fullmark, studentName));
                        }
                    }
                }
                else if (type.ToLower() == "essay")
                {
                    foreach (EssayQuiz quiz in EssayQuizzes)
                    {
                        if (quiz.getQuizID() == quizID)
                        {
                            foreach (Question q in quiz.Questions)
                            {
                                Console.Write($"Enter student's answer for Q{i}: ");
                                i++;
                                string answer = Console.ReadLine() ?? "Not provided";
                                answer = checkForNull(answer);
                                foreach (string keyword in q.Keywords)
                                {
                                    string pattern = keyword;
                                    int count = Regex.Matches(answer, pattern, RegexOptions.IgnoreCase).Count;
                                    if (count > 0)
                                    {
                                        mark++;
                                    }
                                    fullmark++;
                                }
                            }
                            Console.WriteLine($"{studentName} scored {mark} out of {fullmark} in {quiz.getTopic()}.");
                            Students.Add(new Student(studentID, studentName, mark));
                            Grades.Add(new Grade(studentID, quiz.getTopic(), mark, fullmark, studentName));
                        }
                    }
                }
                if (fullmark == 0)
                {
                    Console.WriteLine("Quiz doesn't have any questions or doesn't exist.");
                }
            }

            public void viewQuizzes()
            {
                foreach(MultipleChoiceQuiz quiz in McqQuizzes)
                {
                    int sum = 0;
                    int count = 0;
                    foreach(Question q in quiz.Questions)
                    {
                        sum += q.Choices.Length;
                        count ++;
                    }
                    if (count == 0)
                    {
                        count = 1;
                    }
                    Console.WriteLine(quiz.info() + ", difficulty level: " + quiz.getDifficultyLevel(sum / count));
                } 
                foreach(EssayQuiz quiz in EssayQuizzes)
                {
                    int sum = 0;
                    int count = 0;
                    foreach(Question q in quiz.Questions)
                    {
                        sum += q.Keywords.Length;
                        count ++;
                    }
                    if (count == 0)
                    {
                        count = 1;
                    }
                    Console.WriteLine(quiz.info() + ", difficulty level: " + quiz.getDifficultyLevel(sum / count));
                }
            }

            public void viewStudentGrades()
            {
                foreach (Grade g in Grades)
                {
                    Console.WriteLine($"Student ID: {g.getStudentID()}, {g.getStudentName()} scored {g.getMark()} out of {g.getFullMark()} in {g.getQuizTopic()}.");
                }
            }
        }
        static void Main(string[] args)
        {
            QuizManager manager = new QuizManager();
            while (true)
            {
                Console.WriteLine("---Quiz Management System---");
                Console.WriteLine("1. Create Quiz");
                Console.WriteLine("2. Add Question to Quiz");
                Console.WriteLine("3. Grade Quiz");
                Console.WriteLine("4. View Quizzes");
                Console.WriteLine("5. View Student Grades");
                Console.WriteLine("6. Exit");
                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine() ?? string.Empty;

                switch (choice)
                {
                    case "1": manager.createQuiz(); break;
                    case "2": manager.addQuestionToQuiz(); break;
                    case "3": manager.gradeQuiz(); break;
                    case "4": manager.viewQuizzes(); break;
                    case "5": manager.viewStudentGrades(); break;
                    case "6": return;
                    default: Console.WriteLine("Invalid choice. Please try again."); break;
                }
                Console.WriteLine();
            }
        }
    }
}
