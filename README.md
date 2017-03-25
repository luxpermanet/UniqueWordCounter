# Unique Word Counter
This project is aimed to count unique word occurrences in a file  
Type: .NET Core Console App  
Target Framework: .NETCoreApp 1.0
Development Environment: Visual Studio 2017 RC  
Usage: UniqueWordCounter.exe /filePath:"FullyQualifiedFilePath" /encoding:utf-8 /fileReadParallelism:10 /lineProcessParallelism:100  

Parameters' meaning:  
filePath: This is your input file  
encoding: Input file's encoding  
fileReadParallelism: Determines how many threads can read from your file at a time  
lineProcessParallelism: Determines how many threads can process the lines from the file at a time

Generating EXE files:   
1) Replace runtime identifier in the .csproj file. I have added win81-x64 as my computer was win 8.1 - 64 bit  
2) Open Developer Command Prompt for VS 2017 RC  
3) Run command "dotnet build -r win81-x64 <.csprojFilePath>" by changing win81-x64 with the one you provided in .csproj file and also by specifying the .csproj full file path between quotation marks
