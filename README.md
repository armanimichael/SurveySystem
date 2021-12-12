# Survey System API

The REST Api for a Survey system.

User are allowed to create, see and compile surveys composed of multiple questions. 

## Configuration

### Database

`DB:SqlServerConnectionString`: DB connection string

### SMTP Server / Emails

`Smtp:User`: SMTP Username

`Smtp:Password`: SMTP Password

`Smtp:Host`: SMTP Host

`Smtp:UseSsl`: Enable SMTP SSL

`Smtp:Port`: SMTP PORT

`Email:From`: SMTP Sender email

`Email:FromName`: SMTP Sender name

### JWT

`JWT:ValidIssuer`: JWT Issuer

`JWT:ValidAudience`: JWT Audience 

`JWT:SecretKey`: JWT Private Key