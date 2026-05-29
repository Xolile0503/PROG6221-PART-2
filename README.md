Cybersecurity Assistant Chatbot

## Overview
A comprehensive cybersecurity chatbot built with WPF (Windows Presentation Foundation) in C#, featuring:
- GUI with ASCII art integration
- MySQL database for task management
- Voice recognition and speech synthesis
- Sentiment detection and empathetic responses
- User memory and personalization
- Cybersecurity keyword recognition

Features Implemented

1. GUI Design 
- Clean dark theme interface
- ASCII art banner
- Color-coded messages (user/bot/system/error)
- Responsive layout with proper spacing

 2. Keyword Recognition 
- Cybersecurity topics: passwords, 2FA, privacy, phishing, malware, WiFi, VPN
- Intelligent keyword matching
- Random response variation

3. Database Integration 
- MySQL database for persistent storage
- Add tasks with title, description, and optional reminders
- View, complete, and delete tasks
- User memory storage (name, interests)

4. Voice Implementation 
- Speech recognition for voice input
- Text-to-speech for bot responses
- Microphone support

5. Sentiment Detection 
- Detects worried, frustrated, curious sentiments
- Provides encouraging responses
- Adjusts tone based on emotional state

6. Memory and Recall 
- Remembers user name
- Stores cybersecurity interests
- Personalizes responses and tips

System Requirements

Software Requirements
- Windows 10/11
- Visual Studio 2022
- MySQL Server 8.0+
- .NET Framework 4.7.2 or .NET 6.0+

Required NuGet Packages
```xml
<PackageReference Include="MySql.Data" Version="8.1.0" />
<PackageReference Include="System.Speech" Version="7.0.0" />
