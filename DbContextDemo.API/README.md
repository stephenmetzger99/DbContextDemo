This is a demo of dbcontext injection in ASP.NET Core Web API.

This is mot meant to be a shining architectural example, but rather a simple demo to illustrate how EF Core dbcontext injection works.

The solution contains 2 projects
1) a Web API project with logging endabled to show the context injection in action.


The layers are as follows:
- API (using minimal apis)
- Application (contains services)
- Persistance (contains dbcontext, models, repositories, and some seed data)


2) a simple client front end that consumes the API in order to demonstrate that the API works 
