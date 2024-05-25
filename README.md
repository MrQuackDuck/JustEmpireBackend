<h1><img src="https://github.com/MrQuackDuck/JustEmpireBackend/assets/61251075/e263f541-9e62-4852-9cd4-bb990b09e9bf" height=80 /><div>JustEmpire (BackEnd)</div></h1>
<p>
  <a href="https://dotnet.microsoft.com/en-us/apps/aspnet"><img src="https://img.shields.io/badge/ASP.NET-gray?color=2A4FBD&logo=dotnet" /></a>
  <a href="https://www.sqlite.org/"><img src="https://img.shields.io/badge/SQLite-gray?color=488BB5&logo=sqlite" /></a>
  <a href="https://docs.docker.com/get-docker/"><img src="https://img.shields.io/badge/Docker-gray?color=1C90ED&logo=docker&logoColor=FFFFFF" /></a>
  <a href="https://learn.microsoft.com/en-us/ef/core/"><img src="https://img.shields.io/badge/EF_Core-gray?color=F07427&logo=dotnet" /></a>
  <a href="https://serilog.net/"><img src="https://img.shields.io/badge/Serilog-gray?color=f5230c" /></a>
</p>

<b>BackEnd</b> for <b>JustEmpire</b> - a website where you can find interesting news and projects. <br>
🌌 It serves as a <b>creative hub</b> for community members, so they can share their ideas and projects.

<b>FrontEnd</b> can be found <a href="https://github.com/MrQuackDuck/JustEmpireFrontend/">here</a>.

## 🌠 Key features

\- Autorization with <b>JWT</b> tokens. <br>
\- <b>Full permission support</b> (customizable in <a href="https://github.com/MrQuackDuck/JustEmpireBackend/blob/master/JustEmpire/Ranks.json">JSON file</a>). <br>
\- Approvement system (e.g: <i>if the user doesn't have enough permissions to do something, his <b>action needs to be confirmed</b></i>)<br>
\- File-based logging with <a href="https://serilog.net/">Serilog</a>. <br>

## 🐳 Deploy (in a container)
1. Install <a href="https://docs.docker.com/get-docker/">Docker</a>
2. Clone this repo <br>
   **>** `git clone https://github.com/MrQuackDuck/JustEmpireBackend.git`
3. Jump into the folder <br>
   **>** `cd .\JustEmpireBackend\`
4. Run the container <br>
   **>** `docker compose up`

## 🔐 Admin panel credentials
<b>Login</b>: admin<br>
<b>Password</b>: admin

## 📃 About community

Briefly, <b>JustEmpire</b> is a <b>closed community</b> I am part of. 
One day I decided to make a website so other members would have the ability to post news and projects they're creating. 
Another reason for making this website was to practice my skills in creating web applications with <b>Angular</b> & <b>ASP.NET</b>.

## 🖥 Development server

1. Install <a href="https://dotnet.microsoft.com/en-us/download/dotnet/7.0">.NET 7</a>
2. Clone this repo
3. Run `dotnet run` command
