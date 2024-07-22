var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.CyberNadzor>("cybernadzor");

builder.Build().Run();
