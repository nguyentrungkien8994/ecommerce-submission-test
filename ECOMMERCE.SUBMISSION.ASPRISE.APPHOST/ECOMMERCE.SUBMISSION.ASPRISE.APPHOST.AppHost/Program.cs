var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.ECOMMERCE_SUBMISSION_ASPRISE_APPHOST_ApiService>("apiservice");
var apiAccount = builder.AddProject<Projects.ECOMMERCE_SUBMISSION_API_ACCOUNT>("ecommerce-submission-api-account");

builder.AddProject<Projects.ECOMMERCE_SUBMISSION_WEB_ADMIN>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithReference(apiAccount)
    .WaitFor(apiAccount);

builder.AddProject<Projects.ECOMMERCE_SUBMISSION_API_ORDER>("ecommerce-submission-api-order");
builder.AddProject<Projects.ECOMMERCE_SUBMISSION_API_PAYMENT>("ecommerce-submission-api-payment");
builder.AddProject<Projects.ECOMMERCE_SUBMISSION_API_EMAIL>("ecommerce-submission-api-email");

builder.Build().Run();
