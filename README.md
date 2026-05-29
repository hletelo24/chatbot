CyberGuard Bot - Cybersecurity Awareness Chatbot

Overview

CyberGuard Bot is a desktop chatbot application developed using C# and WPF (Windows Presentation Foundation).
The application is designed to educate users about cybersecurity awareness through an interactive chat interface.

The chatbot provides cybersecurity tips, responds to user input, and maintains an activity log while offering a modern dark-themed user interface.

---

Features

- Interactive chatbot interface
- Modern dark UI design
- ASCII art welcome display
- Chat history display
- User input handling
- Activity logging system
- Clear chat functionality
- Keyboard Enter key support
- Cybersecurity awareness responses

---

Technologies Used

- C#
- WPF (Windows Presentation Foundation)
- XAML
- .NET Framework / .NET

---

Project Structure

chatbot/
│
├── MainWindow.xaml
├── MainWindow.xaml.cs
├── App.xaml
├── App.xaml.cs
└── README.md

---

User Interface Components

Chat Area

The main section where users interact with the CyberGuard Bot.

Includes:

- ASCII Art Display
- Chat History
- Input TextBox
- Send Button

---

Sidebar

Contains utility buttons for managing the application.

Buttons:

- Show Activity Log
- Clear Chat

---

Main Functionalities

Send Messages

Users can type messages into the input box and click the Send button or press Enter.

private void btnSend_Click(object sender, RoutedEventArgs e)
{
    // Send message logic
}

---

Keyboard Support

private void txtInput_KeyDown(object sender, KeyEventArgs e)
{
    // Enter key handling
}

---

Activity Log

Displays recorded chatbot activities and interactions.

private void btnLog_Click(object sender, RoutedEventArgs e)
{
    // Show activity log
}

---

Clear Chat

Clears the current conversation history.

private void btnClear_Click(object sender, RoutedEventArgs e)
{
    // Clear chat logic
}

---

UI Design

Theme Colors

Element| Color
Background| #1E1E2F
Text| #E0E0E0
Accent Blue| #00AAFF
Accent Green| #00FFAA
Accent Purple| #AA66FF

---

How to Run the Project

Requirements

- Visual Studio 2022 or later
- .NET Framework / .NET SDK
- Windows OS

---

Steps

1. Open the solution in Visual Studio.
2. Build the project.
3. Run the application using:

F5

or click:

Start Debugging

---

Future Improvements

- AI-powered chatbot integration
- Voice recognition
- Database storage for logs
- Real-time cybersecurity news
- User authentication system
- Animated chatbot avatar

---

Educational Purpose

This project was created for educational purposes to demonstrate:

- WPF UI development
- Event-driven programming
- Chatbot design concepts
- Cybersecurity awareness systems

---

Author

Developed by:
Hletelo Mathevula

---

License

This project is for educational and academic use.
