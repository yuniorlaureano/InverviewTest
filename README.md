# InterviewTest Project
## Set up instructions:

### Project set up

> The project needs .NET CORE 6 and SQL server to run. **InterviewTest.Api** is the project api that have to be set as the start up project.

### Database set up
> The project: **InterviewTest.Migrations**, contains the migrations to create the tables in the database. If the script takes long to run
it might be due to permissions issues, **in this case just create the data base manually then run the migrations.** The current databse name is: 
**InterviewTest**, it can be changed inside the configuration file. This migration project is 
a console application which can be run independently.

### Seeding databse
> After creating the databse and configure the project, the tests in **InterviewTest.Test** cam be run, and the databse will be seeded with 
test data. 

## Project Organization
- InterviewTest.Api
> Entry point application
- InterviewTest.Common
> Holds the dtos and commong classes for the other layers
- InterviewTest.Data
> Serve as the data layer. It contains an implementation of ADO.NET library, and the repositories to 
comunicate with the database.
- InterviewTest.Entity
> Contains the classes that represent the tables in the databse
- InterviewTest.Migrations
> Contains the scripts to set up the tables for the database. It is a console project, can be run 
manually
- InterviewTest.Service
> It is a bridge between **InterviewTest.Data** and **InterviewTest.Api**. It runs logic and interact
with the data in high level details
- InterviewTest.Test
>It contains the tests for the whole application.

## Main endpoints
- POST: /Users. It is a free endpoint to create users. Whe the tests are runs, a user is created like this:
**email:** "admin@gmail.com", and **password**: "admin". other user can be created. 

- POST: /Security/login. Allow the authentication of the user. It issues a JWT token, which can be provided into 
 swagger and grant access to the other endpoints. 