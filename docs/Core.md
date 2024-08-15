# Core
## Integration

### Data Sources
For Scan purposes:
Probably, custom implementation for each type of database is needed.

For CRUD operations:
Multiple database integrations could be simplified by using existing ORM library.

#### Postgres

#### Mysql

#### ...

### Authentication
Our implementation of authentication is possible to integrate with chosen data source so our users could easly skip this part while they are implementing other more important functions

### Authorization
Our implementation of authorization is possible to integrate with chosen data source so our users could easly skip this part while they are implementing other more important functions

## Authentication
Login
Register
Forgot password
Email verification
User identity stored in database

### Types

#### JWT Bearer

#### SSO

##### Github

##### Google

##### ...

## Authorization

### Types

#### Roles

#### Permissions

## Project
Entity stored in database

To this entity could be wired other microservices which our cloud we be providing

### Data Sources
Add/Remove data source connections to project

### Authentication
On specific data source chosen inside project
Support tables will be created

Configure which types of authentication you want to have enabled/disabled

### Authorization
On specific data source chosen inside project
Support tables will be created

Create roles/permissions
Assign permissions to roles
Assing roles/permissions to user

### Microservices
Enable/Disable and use other microservices on cloud platform inside specific project

### Functions

#### Languages

##### Javascript

##### Python

##### C#

##### Java

##### Go

##### ...

#### Boilerplates
##### Components

###### Hook

  - Validate
  - Input
  - OnCreated
  - OnUpdated
  - OnDeleted

#### Library

#### Docker

