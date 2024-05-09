# FlightFinder
Flight Finder is a Software Engineering Task
The Criteria Of this task was:
1. Create a simple application named “Flight Finder” with the goal of determining how many
instances of the word “flight” are possible in a supplied string.
2. The application should be written in C# and utilize the ASP .NET framework.
3. A frontend interface should accept a string and return the number of instances of the word
"flight" that can be formed.
4. The frontend should communicate with a backend REST API.
5. Each character may only be used once in composing the word “flight”.
6. Only lower-case characters will be given as an input.
7. No more than 100 characters will be given as an input.

# To Run This
- Open In VS Community, Ensure "Multiple Startup Projects" is enabled and the API and the Web is set to "Start"


# Notes 
In this project, I wanted my ability to utilize Api, MVC, Nunit testing technology. 

I considered that business could be handling larger data than 100 characters, So I wanted to demonstrate my knowledge of optimization by both Sterilizing, and Manipulating the data then exercising Batching.

I also considered appropriate data types, using ImmutableArray was the best for a constant as it is easily scalable. If I was to Scale Up I would have used HashSets rather than Tuples, but as this is not the scope of the project and I like using Tuples, I chose Tuples. 
