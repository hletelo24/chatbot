

using Microsoft.Data.SqlClient;
using System.Speech.Synthesis;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using static Microsoft.Data.SqlClient.Internal.SqlClientEventSource;
using static System.Net.Mime.MediaTypeNames;

namespace chatbot
{
    
    public partial class MainWindow : Window
    {
        
        private string userName = "User";
        private string currentTopic = "";
        private Dictionary<string, string> userMemory = new Dictionary<string, string>();
        private List<ActivityLogEntry> activityLog = new List<ActivityLogEntry>();
        private Random random = new Random();

        //new variables
        string connectionString = "";
        private int currentQuestion = 0;
        private int score = 0;
        private bool quizMode = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PlayVoiceGreeting();
            DisplayAsciiArt();
            AddToChat("CyberGuard", "Hello! Welcome to the Cybersecurity Awareness ChatBot.\nI'm here to help you stay safe online.", Colors.Cyan);
            AddToChat("CyberGuard", "What is your name?", Colors.Cyan);
        }

        private void PlayVoiceGreeting()
        {
            try
            {
                SpeechSynthesizer synth = new SpeechSynthesizer();
                synth.SpeakAsync("Hello! Welcome to the Cybersecurity Awareness Bot. I'm here to help you stay safe online.");
            }
            catch { /* Ignore if no speech engine */ }
        }

        private void DisplayAsciiArt()
        {
            string art = @"
   _____ _               _____                 _ 
  / ____| |             / ____|               | |
 | |    | |__   ___ _ _| |  __ _   _  ___ _ __| |_
 | |    | '_ \ / _ \ '__| | |_ | | | |/ _ \ '__| __|
 | |____| | | |  __/ |  | |__| | |_| |  __/ |  | |_
  \_____|_| |_|\___|_|   \_____|\__,_|\___|_|   \__|
            Cybersecurity Awareness Assistant
";
            txtAscii.Text = art;
        }

        private void btnSend_Click(object sender, RoutedEventArgs e) => ProcessUserInput();
        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) ProcessUserInput();
        }

        private void ProcessUserInput()
        {
            string input = txtInput.Text.Trim();
            if (string.IsNullOrEmpty(input)) return;

            AddToChat(userName, input, Colors.White);
            txtInput.Clear();

            if (quizMode)
            {
                CheckAnswer(input);
            }
            else
            {
                string response = GetBotResponse(input.ToLower());

                if (!string.IsNullOrEmpty(response))
                {
                    AddToChat("CyberGuard", response, Colors.Cyan);
                }
            }

            LogActivity("User Input", input);
        }

        private string GetBotResponse(string input)
        {
            // Name capture
            if (userName == "User" && (input.Contains("name") || input.Length < 20))
            {
                userName = input.Length > 2 ? input : "Friend";
                return $"Nice to meet you, {userName}! How can I help you with cybersecurity today?";
            }
            if (input.Contains("quiz"))
            {
                StartQuiz();
                return "";
            }
            if (input.Contains("update my password"))
            {
                AddTask(
                   "Update Password",
                   "Change account password",
                   DateTime.Now.AddDays(1));

                AddToChat("CyberGaurd","Reminder set to update your password tomorrow.",Colors.Violet);

                AddToActivityLog(
                   "Password reminder created");
            }
            if (input.Contains("update my profile"))
            {
                AddTask(
                   "Update Profile",
                   "Update Profile",
                   DateTime.Now.AddDays(1));

                AddToChat("CyberGaurd", "Reminder set to update your profile tomorrow.", Colors.Violet);

                AddToActivityLog(
                   "Profile reminder created");
            }
            if (input.Contains("learn about cybersecurity"))
            {
                AddTask(
                   "Learn Cybersecurity",
                   "Learn about Cybersecurity",
                   DateTime.Now.AddDays(1));

                AddToChat("CyberGaurd", "Reminder set to Learn about cybersecurity.", Colors.Violet);

                AddToActivityLog(
                   "Learning reminder created");
            }
            // Sentiment Detection
            if (input.Contains("worried") || input.Contains("scared") || input.Contains("afraid"))
                return "It's completely normal to feel worried. " + GetRandomTip();

            if (input.Contains("frustrated") || input.Contains("angry"))
                return "I understand this can be frustrating. Let's take it step by step. " + GetRandomTip();

            // Memory
            if (input.Contains("interested in") || input.Contains("my favourite"))
            {
                string topic = input.Contains("privacy") ? "privacy" : input.Contains("password") ? "password" : "general";
                userMemory["favouriteTopic"] = topic;
                return $"Great! I'll remember you're interested in {topic}. I'll tailor my advice accordingly.";
            }
            if (input.Contains("activity log") || input.Contains("what have you done for me"))
            {
                ShowActivityLog();
                return "";
            }
            // Keyword Recognition + Random Responses
            if (input.Contains("password"))
                return GetRandomResponse("password");

            if (input.Contains("phishing") || input.Contains("scam"))
                return GetRandomResponse("phishing");

            if (input.Contains("privacy"))
                return GetRandomResponse("privacy");

            if (input.Contains("tip") || input.Contains("another"))
                return GetRandomTip();

            if (input.Contains("how are you"))
                return "I'm doing great, thank you! Always ready to help you stay secure.";

            if (input.Contains("purpose") || input.Contains("who are you"))
                return "I am CyberGuard, your personal Cybersecurity Awareness Assistant.";

            // Default
            return "I didn’t quite understand that. Could you rephrase? Try asking about passwords, phishing, or privacy.";
        }

        private string GetRandomResponse(string topic)
        {
            var responses = topic switch
            {
                "password" => new List<string>
                {
                    "Use strong, unique passwords for every account. Consider a password manager.",
                    "Enable two-factor authentication (2FA) wherever possible.",
                    "Never share your password via email or chat."
                },
                "phishing" => new List<string>
                {
                    "Always check the sender's email address carefully. Hover over links before clicking.",
                    "Scammers often create urgency. Don't panic and click.",
                    "Report suspicious emails to your organisation or use reportphishing@apwg.org"
                },
                "privacy" => new List<string>
                {
                    "Review app permissions regularly and limit what you share.",
                    "Use a VPN on public Wi-Fi networks.",
                    "Enable privacy settings on all your social media accounts."
                },
                _ => new List<string> { "Stay safe online!" }
            };

            return responses[random.Next(responses.Count)];
        }

        private string GetRandomTip() => GetRandomResponse("password"); // reuse or expand

        private void AddToChat(string sender, string message, Color color)
        {
            chatHistory.Inlines.Add(new System.Windows.Documents.Run($"{sender}: ") 
                { Foreground = new SolidColorBrush(Colors.Yellow) });
            chatHistory.Inlines.Add(new System.Windows.Documents.Run(message + "\n\n") 
                { Foreground = new SolidColorBrush(color) });

             // Requires ScrollViewer with ScrollToEnd extension or use rich text
        }

        private void LogActivity(string action, string details)
        {
            activityLog.Add(new ActivityLogEntry
            {
                Timestamp = DateTime.Now,
                Action = action,
                Details = details
            });
        }

        // Button Handlers for Part 3
        

        private void btnLog_Click(object sender, RoutedEventArgs e)
        {
            string logText = "Recent Activity:\n" + 
                string.Join("\n", activityLog.TakeLast(5).Select(l => 
                    $"[{l.Timestamp:HH:mm}] {l.Action}: {l.Details}"));
            AddToChat("CyberGuard", logText, Colors.Violet);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            chatHistory.Text = "";
        }

        //NEW METHODS
        private void AddTask(string title, string description, DateTime? reminder)
        {
            using (SqlConnection conn =
                   new SqlConnection(connectionString))
            {
                conn.Open();

                string query =
                @"INSERT INTO Tasks
                (Title, Description, ReminderDate)
                VALUES
                (@title,@desc,@reminder)";

                SqlCommand cmd =
                    new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@desc", description);
                cmd.Parameters.AddWithValue("@reminder", reminder);

                cmd.ExecuteNonQuery();
            }

            AddToActivityLog($"Task Added: {title}");
        }

        private string GetTasks()
        {
            StringBuilder sb = new StringBuilder();
                
            using (SqlConnection conn =
                   new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT * FROM Tasks";

                SqlCommand cmd =
                    new SqlCommand(query, conn);

                SqlDataReader reader =
                    cmd.ExecuteReader();

                while (reader.Read())
                {
                    sb.AppendLine(
                    $"{reader["Id"]}. " +
                    $"{reader["Title"]} | " +
                    $"{reader["Description"]}");
                }
            }

            return sb.ToString();
        }

        private void CompleteTask(int id)
        {
            using (SqlConnection conn =
                   new SqlConnection(connectionString))
            {
                conn.Open();

                string query =
                "UPDATE Tasks SET Completed=1 WHERE Id=@id";

                SqlCommand cmd =
                    new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id", id);

                cmd.ExecuteNonQuery();
            }

            AddToActivityLog($"Task {id} completed");
        }

        private void DeleteTask(int id)
        {
            using (SqlConnection conn =
                   new SqlConnection(connectionString))
            {
                conn.Open();

                string query =
                "DELETE FROM Tasks WHERE Id=@id";

                SqlCommand cmd =
                    new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id", id);

                cmd.ExecuteNonQuery();
            }

            AddToActivityLog($"Task {id} deleted");
        }
        //END OF TASK METHODS

        // START OF GAME METHODS

        private List<QuizQuestion> quizQuestions =
        new List<QuizQuestion>()
        {
            new QuizQuestion
            {
                Question="What is phishing?",
                Options=new[]
                {
                    "A scam email",
                    "A firewall",
                    "An antivirus",
                    "Encryption"
                },
                CorrectAnswer="A scam email",
                Explanation="Phishing tricks users into revealing information."
            },

            new QuizQuestion
            {
                Question="Strong passwords should contain?",
                Options=new[]
                {
                    "Only letters",
                    "Letters and numbers",
                    "Mixed characters",
                    "Your birthday"
                },
                CorrectAnswer="Mixed characters",
                Explanation="Strong passwords use symbols, numbers and letters."
            }

    // Add 10+ total questions
};


        private void StartQuiz()
        {
            currentQuestion = 0;
            score = 0;
            quizMode = true;

            AddToChat("CyberGaurd","Quiz Started!",Colors.Violet);

            AskQuestion();

            AddToActivityLog("Quiz Started");
        }
        private void AskQuestion()
        {
            if (currentQuestion >= quizQuestions.Count)
            {
                EndQuiz();
                return;
            }

            QuizQuestion q =
                quizQuestions[currentQuestion];

            AddToChat("CyberGaurd",
                $"{q.Question}\n" +
                $"A) {q.Options[0]}\n" +
                $"B) {q.Options[1]}\n" +
                $"C) {q.Options[2]}\n" +
                $"D) {q.Options[3]}",Colors.Violet
                );
        }
        private void CheckAnswer(string answer)
        {
            if (string.IsNullOrWhiteSpace(answer))
            {
                AddToChat("CyberGuard",
                    "Please enter A, B, C or D.",
                    Colors.Red);
                return;
            }

            QuizQuestion q = quizQuestions[currentQuestion];

            int index = char.ToUpper(answer[0]) - 'A';

            if (index >= 0 &&
                index < q.Options.Length &&
                q.Options[index] == q.CorrectAnswer)
            {
                score++;

                AddToChat("CyberGuard",
                    "✅ Correct!\n" + q.Explanation,
                    Colors.Green);
            }
            else
            {
                AddToChat("CyberGuard",
                    $"❌ Incorrect.\nCorrect Answer: {q.CorrectAnswer}\n{q.Explanation}",
                    Colors.Red);
            }

            currentQuestion++;

            AskQuestion();
        }

        private void EndQuiz()
        {
            quizMode = false;

            AddToChat("CyberGaurd",
            $"Quiz Complete!\n" +
            $"Score: {score}/{quizQuestions.Count}",Colors.Violet);

            if (score >= 8)
                AddToChat("CyberGaurd","Excellent! Cybersecurity Pro!",Colors.Violet);
            else
                AddToChat("CyberGaurd", "Keep learning to stay safe online!", Colors.Violet);

            AddToActivityLog(
            $"Quiz Completed: {score}/{quizQuestions.Count}");
        }
        private void AddToActivityLog(string action)
        {
            activityLog.Add(new ActivityLogEntry
            {
                Timestamp = DateTime.Now,
                Action = action,
                Details = ""
            });
        }

        private void ShowActivityLog()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Recent Activity:");

            foreach(ActivityLogEntry item in activityLog.TakeLast(10))
            {
                sb.AppendLine($"{item.Action} - {item.Timestamp}");
            }

            AddToChat("CyberGaurd", sb.ToString(), Colors.Violet);
        }

        private void btnQuiz_Click(object sender, RoutedEventArgs e)
        {
            StartQuiz();
        }

        private void btnTasks_Click(object sender, RoutedEventArgs e)
        {
            string tasks = GetTasks();
            AddToChat("CyberGuard",
            string.IsNullOrEmpty(tasks)
                ? "No tasks found."
                : tasks,
            Colors.Violet);
        }

        
    }

    public class ActivityLogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Action { get; set; }
        public string Details { get; set; }
    }
    
}