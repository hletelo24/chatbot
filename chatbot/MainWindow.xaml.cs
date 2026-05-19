using System.Speech.Synthesis;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace chatbot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        private string userName = "User";
        private string currentTopic = "";
        private Dictionary<string, string> userMemory = new Dictionary<string, string>();
        private List<ActivityLogEntry> activityLog = new List<ActivityLogEntry>();
        private Random random = new Random();

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

            string response = GetBotResponse(input.ToLower());
            AddToChat("CyberGuard", response, Colors.Cyan);

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
    }

    public class ActivityLogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Action { get; set; }
        public string Details { get; set; }
    }
    
}