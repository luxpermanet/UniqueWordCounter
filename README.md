# Unique Word Counter
This project is aimed to count unique word occurrences in a file  
Type: .NET Core Console App  
Development Environment: Visual Studio 2017 RC  
Usage: UniqueWordCounter.exe /filePath:"FullyQualifiedFilePath" /encoding:utf-8 /fileReadParallelism:10 /lineProcessParallelism:100  

Parameters' meaning:  
filePath: This is your input file  
encoding: Input file's encoding  
fileReadParallelism: Determines how many threads can read from your file at a time  
lineProcessParallelism: Determines how many threads can process the lines from the file at a time
