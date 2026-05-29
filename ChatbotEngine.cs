using CybersecurityChatbot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CybersecurityChatbotWPF
{
    public enum Sentiment
    {
        Neutral,
        Worried,
        Frustrated,
        Curious
    }

    public class ChatbotEngine
    {
        private DatabaseManager dbManager;
        private Dictionary<string, List<string>> keywordResponses;
        private List<string> defaultResponses;
        private Dictionary<Sentiment, List<string>> encouragementResponses;
        private Random random;

        // Constructor (with DB)
        public ChatbotEngine(DatabaseManager dbManager)
        {
            this.dbManager = dbManager ?? new DatabaseManager();
            this.random = new Random();
            InitializeResponses();
        }

        // Constructor (no DB passed)
        public ChatbotEngine()
        {
            this.dbManager = new DatabaseManager();
            this.random = new Random();
            InitializeResponses();
        }

        private void InitializeResponses()
        {
            keywordResponses = new Dictionary<string, List<string>>
            {
                ["hello"] = new List<string>
                {
                    "Hello! I'm your Cybersecurity Assistant. How can I help you today?",
                    "Hi there! Ready to stay safe online?",
                    "Greetings! I'm here to help with cybersecurity advice."
                },

                ["password"] = new List<string>
                {
                    "🔐 Use strong passwords with letters, numbers, and symbols.",
                    "🔐 Never reuse passwords across accounts.",
                    "🔐 Use a password manager for safety."
                },

                ["2fa"] = new List<string>
                {
                    "🛡️ Two-factor authentication protects your accounts.",
                    "🛡️ Always enable 2FA when possible."
                },

                ["two factor"] = new List<string>
                {
                    "🛡️ Two-factor authentication adds extra security.",
                    "🛡️ 2FA helps prevent unauthorized access."
                },

                ["privacy"] = new List<string>
                {
                    "👁️ Check your privacy settings regularly.",
                    "👁️ Be careful what you share online."
                },

                ["phishing"] = new List<string>
                {
                    "🎣 Always check email links before clicking.",
                    "🎣 Be aware of fake messages asking for passwords."
                },

                ["malware"] = new List<string>
                {
                    "🦠 Keep antivirus software updated.",
                    "🦠 Avoid downloading unknown files."
                },

                ["virus"] = new List<string>
                {
                    "🦠 Run regular antivirus scans.",
                    "🦠 Don't open suspicious attachments."
                },

                ["wifi"] = new List<string>
                {
                    "📡 Avoid public Wi-Fi for sensitive actions.",
                    "📡 Secure your home router with a strong password."
                },

                ["vpn"] = new List<string>
                {
                    "🔒 VPNs protect your internet traffic from hackers."
                },

                ["help"] = new List<string>
                {
                    "I can help with: passwords, phishing, malware, privacy, Wi-Fi security, and tasks."
                },

                ["thanks"] = new List<string>
                {
                    "You're welcome!",
                    "Happy to help!"
                },

                ["thank you"] = new List<string>
                {
                    "You're welcome!",
                    "Glad I could help!"
                },

                ["bye"] = new List<string>
                {
                    "Goodbye! Stay safe online!",
                    "Take care!"
                },

                ["goodbye"] = new List<string>
                {
                    "Goodbye! Stay secure!",
                    "See you next time!"
                }
            };

            defaultResponses = new List<string>
            {
                "I didn't understand that. Try asking about passwords, phishing, malware, or privacy.",
                "Could you rephrase that? I can help with cybersecurity topics.",
                "I'm still learning. Try asking about online safety."
            };

            encouragementResponses = new Dictionary<Sentiment, List<string>>
            {
                [Sentiment.Worried] = new List<string>
                {
                    "It's okay to feel worried. Let's improve your security step by step.",
                    "You're not alone—I'll help you stay safe."
                },

                [Sentiment.Frustrated] = new List<string>
                {
                    "I understand it's frustrating. Let's simplify it.",
                    "Don't worry, we'll fix this together."
                },

                [Sentiment.Curious] = new List<string>
                {
                    "Great curiosity! Learning cybersecurity is powerful.",
                    "Awesome question—knowledge keeps you safe!"
                }
            };
        }

        // SENTIMENT DETECTION
        public Sentiment DetectSentiment(string text)
        {
            string lower = text.ToLower();

            if (lower.Contains("worried") || lower.Contains("scared") || lower.Contains("afraid"))
                return Sentiment.Worried;

            if (lower.Contains("frustrated") || lower.Contains("confused") || lower.Contains("stuck"))
                return Sentiment.Frustrated;

            if (lower.Contains("curious") || lower.Contains("learn") || lower.Contains("tell me"))
                return Sentiment.Curious;

            return Sentiment.Neutral;
        }

        // MAIN RESPONSE ENGINE
        public string GenerateResponse(string userInput, Sentiment sentiment)
        {
            string lower = userInput.ToLower();

            string taskResponse = HandleTaskCommands(userInput);
            if (!string.IsNullOrEmpty(taskResponse))
                return taskResponse;

            string memoryResponse = HandleMemoryCommands(userInput);
            if (!string.IsNullOrEmpty(memoryResponse))
                return memoryResponse;

            string response = null;

            foreach (var entry in keywordResponses)
            {
                if (lower.Contains(entry.Key))
                {
                    response = entry.Value[random.Next(entry.Value.Count)];
                    break;
                }
            }

            if (response == null)
                response = defaultResponses[random.Next(defaultResponses.Count)];

            if (sentiment != Sentiment.Neutral && encouragementResponses.ContainsKey(sentiment))
            {
                string extra = encouragementResponses[sentiment][random.Next(encouragementResponses[sentiment].Count)];
                response = extra + "\n\n" + response;
            }

            if (dbManager != null)
            {
                string name = dbManager.GetMemory("user_name");

                if (!string.IsNullOrEmpty(name))
                {
                    string[] prefixes =
                    {
                        $"Thanks for sharing, {name}. ",
                        $"Good question, {name}! "
                    };

                    response = prefixes[random.Next(prefixes.Length)] + response;
                }
            }

            return response;
        }

        // TASK HANDLING
        private string HandleTaskCommands(string input)
        {
            string lower = input.ToLower();

            Match addMatch = Regex.Match(lower, @"add task\s*[-:]\s*(.+)");
            if (addMatch.Success)
            {
                string title = addMatch.Groups[1].Value.Trim();
                return $"Task '{title}' noted. Please add a description.";
            }

            if (lower.Contains("show task") || lower.Contains("view task"))
            {
                var tasks = dbManager.GetTasks("pending");

                if (tasks.Rows.Count == 0)
                    return "No pending tasks.";

                string result = "📋 Tasks:\n";

                for (int i = 0; i < tasks.Rows.Count; i++)
                {
                    result += $"{i + 1}. {tasks.Rows[i]["title"]}\n";
                }

                return result;
            }

            return null;
        }

        // MEMORY HANDLING
        private string HandleMemoryCommands(string input)
        {
            string lower = input.ToLower();

            Match nameMatch = Regex.Match(lower, @"my name is (\w+)|i'm (\w+)|call me (\w+)");

            if (nameMatch.Success)
            {
                string name = "";

                if (nameMatch.Groups[1].Success)
                    name = nameMatch.Groups[1].Value;
                else if (nameMatch.Groups[2].Success)
                    name = nameMatch.Groups[2].Value;
                else if (nameMatch.Groups[3].Success)
                    name = nameMatch.Groups[3].Value;

                if (!string.IsNullOrEmpty(name))
                {
                    name = char.ToUpper(name[0]) + name.Substring(1);
                    dbManager.SaveMemory("user_name", name);

                    return $"Nice to meet you, {name}! I'll remember your name.";
                }
            }

            Match interestMatch = Regex.Match(lower, @"interested in (\w+)|i like (\w+)");

            if (interestMatch.Success)
            {
                string interest = interestMatch.Groups[1].Success
                    ? interestMatch.Groups[1].Value
                    : interestMatch.Groups[2].Value;

                dbManager.SaveMemory("user_interest", interest);

                return $"Got it! I'll remember you're interested in {interest}.";
            }

            return null;
        }

        // ADD TASK (DB)
        public string AddTaskWithDetails(string title, string description, int? reminderDays = null)
        {
            DateTime? reminder = reminderDays.HasValue
                ? DateTime.Now.AddDays(reminderDays.Value)
                : (DateTime?)null;

            int id = dbManager.AddTask(title, description, reminder);

            if (id > 0)
                return $"Task '{title}' added successfully.";

            return "Failed to add task.";
        }

        // PERSONAL TIP
        public string GetPersonalizedTip()
        {
            string[] tips =
            {
                "Use strong unique passwords.",
                "Enable two-factor authentication.",
                "Avoid clicking unknown links.",
                "Keep software updated.",
                "Use secure Wi-Fi connections."
            };

            return "💡 Tip: " + tips[random.Next(tips.Length)];
        }
    }
}