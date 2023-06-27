CREATE DATABASE HospitalDatabase
USE HospitalDatabase
GO
CREATE SCHEMA HospitalSchema
GO

--Admin Login
CREATE TABLE HospitalSchema.AdminLogin(
    AdminID INT IDENTITY(1,1),
    UserName VARCHAR(15),
    PasswordSalt VARBINARY(MAX),
    PasswordHash VARBINARY(MAX),
    AdminRole VARCHAR(15)
)

--Registration For Doctors
CREATE TABLE HospitalSchema.DoctorsRegistration(
    DoctorsId INT IDENTITY(1,1) PRIMARY KEY,     
    FirstName VARCHAR(20),
    LastName VARCHAR(20),
    Gender VARCHAR(10),
    Qualification VARCHAR(20),
    SpecialistIn VARCHAR(20),
    EmailAddress NVARCHAR(20),
    PasswordHash VARBINARY(MAX),
    PasswordSalt VARBINARY(MAX),
    PhoneNumber VARCHAR(20),
    Verified BIT DEFAULT 0
)


--Registration For Patients
CREATE TABLE HospitalSchema.PatientsRegistration(
    PatientsId INT IDENTITY(1,1) PRIMARY KEY,     
    FirstName VARCHAR(20),
    LastName VARCHAR(20),
    Gender VARCHAR(10),
    Age INT,
    EmailAddress NVARCHAR(20),
    PasswordHash VARBINARY(MAX),
    PasswordSalt VARBINARY(MAX),
    PhoneNumber VARCHAR(20),
    PatientsAddress NVARCHAR(MAX),
    PatientsState VARCHAR(15)
)

-- Patients Complaint Table
CREATE TABLE HospitalSchema.PatientsComplaint(
    ID INT IDENTITY(1,1) PRIMARY KEY,
    TypeOfSickeness NVARCHAR(20),
    Problem NVARCHAR
)

--Laboratory Test Table
CREATE TABLE HospitalSchema.LabTestTable(
    LabID INT IDENTITY(1,1) PRIMARY KEY,
    NameOfTest NVARCHAR,
    NameOfLab NVARCHAR,
    Results BIT,
    TestDate DATE,
    TestTime TIME
)

--Appointement Table
CREATE TABLE HospitalSchema.AppointementTable(
    ID INT IDENTITY(1,1) PRIMARY KEY,
    DoctorsID INT,
    PatientsID INT,
    AppointmentDate DATE,
    AppointmentTime TIME
)

--Prescirption Table
CREATE TABLE HospitalSchema.PrescriptionTable(
    ID INT IDENTITY(1,1) PRIMARY KEY,
    DoctorsID INT,
    PatientsID INT,
    Quantity INT,
    Medication NVARCHAR,
    Diagnosis NVARCHAR
)
GO

--Procedure to Add Admin 
CREATE OR ALTER PROCEDURE spAdminLogin
@UserName VARCHAR(15),
@PasswordSalt VARBINARY(MAX),
@PasswordHash VARBINARY(MAX),
@AdminRole VARCHAR(15)
AS
BEGIN
INSERT INTO  HospitalSchema.AdminLogin(
    [UserName],
    [PasswordSalt],
    [PasswordHash],
    [AdminRole]
)VALUES(@UserName, @PasswordSalt, @PasswordHash, @AdminRole)
END
GO

--Procedure to Add Doctors
CREATE OR ALTER PROCEDURE spAddDoctors
@FirstName VARCHAR(20),
@LastName VARCHAR(20),
@Gender VARCHAR(10),
@Qualification VARCHAR(20),
@SpecialistIn VARCHAR(20),
@EmailAddress NVARCHAR(20),
@PasswordHash VARBINARY(MAX),
@PasswordSalt VARBINARY(MAX),
@PhoneNumber VARCHAR(20),
@Verified BIT
AS
BEGIN
INSERT INTO  HospitalSchema.DoctorsRegistration(
[FirstName],
[LastName],
[Gender],
[Qualification],
[SpecialistIn],
[EmailAddress],
[PasswordHash],
[PasswordSalt],
[PhoneNumber],
[Verified]
)VALUES(@FirstName, @LastName, @Gender, @Qualification, @SpecialistIn, @EmailAddress, @PasswordHash, @PasswordSalt, @PhoneNumber, @Verified)
END

GO

--Procedure to Add Patients
CREATE OR ALTER PROCEDURE spAddPatients
@FirstName VARCHAR(20),
@LastName VARCHAR(20),
@Gender VARCHAR(10),
@Age INT,
@EmailAddress NVARCHAR(20),
@PasswordHash VARBINARY(MAX),
@PasswordSalt VARBINARY(MAX),
@PhoneNumber VARCHAR(20),
@PatientsAddress NVARCHAR(MAX),
@PatientsState VARCHAR(15)
AS
BEGIN
INSERT INTO HospitalSchema.PatientsRegistration(
[FirstName],
[LastName],
[Gender],
[Age],
[EmailAddress],
[PasswordHash],
[PasswordSalt],
[PhoneNumber],
[PatientsAddress],
[PatientsState]
)VALUES(@FirstName, @LastName, @Gender, @Age, @EmailAddress, @PasswordHash, @PasswordSalt, @PhoneNumber, @PatientsAddress, @PatientsState)
END
GO



--Procedure to Add Patients Complain
CREATE OR ALTER PROCEDURE spPatientsComplain
@TypeOfSickeness NVARCHAR(20),
@Problem NVARCHAR
AS
BEGIN
INSERT INTO HospitalSchema.PatientsComplaint(
[TypeOfSickeness],
[Problem]
)VALUES(@TypeOfSickeness, @Problem)
END



GO
--Procedure to Add Lab Tests
CREATE OR ALTER PROCEDURE spLabTests
@NameOfTest NVARCHAR,
@NameOfLab NVARCHAR,
@Results BIT,
@TestDate DATE,
@TestTime TIME
AS
BEGIN
INSERT INTO HospitalSchema.LabTestTable(
[NameOfTest],
[NameOfLab],
[Results],
[TestDate],
[TestTime]
)VALUES(@NameOfTest, @NameOfLab, @Results, @TestDate, @TestTime)
END

GO

--Procedure to Add Appointment Table
CREATE OR ALTER PROCEDURE spAppiontmentTable
@DoctorsID INT,
@PatientsID INT,
@AppointmentDate DATE,
@AppointmentTime TIME
AS
BEGIN
INSERT INTO HospitalSchema.AppointementTable(
[DoctorsID],
[PatientsID],
[AppointmentDate],
[AppointmentTime]
)VALUES(@DoctorsID, @PatientsID, @AppointmentDate, @AppointmentTime)
END

GO

CREATE OR ALTER PROCEDURE spPresriptionTable
@DoctorsID INT,
@PatientsID INT,
@Quantity INT,
@Medication NVARCHAR,
@Diagnosis NVARCHAR
AS
BEGIN
INSERT INTO HospitalSchema.PrescriptionTable(
[DoctorsID],
[PatientsID],
[Quantity],
[Medication],
[Diagnosis]
)VALUES(@DoctorsID, @PatientsID, @Quantity, @Medication, @Diagnosis)
END

GO
--Procedure for Login Confirmation
CREATE OR ALTER PROCEDURE spLoginConfirmation_GetForAdmins
    @UserName NVARCHAR(30)
AS
BEGIN
    SELECT [Auth].[PasswordHash],
        [Auth].[PasswordSalt] 
    FROM HospitalSchema.DoctorsRegistration AS Auth 
        WHERE Auth.EmailAddress = @UserName
END;
GO
--Procedure for Login Confirmation
CREATE OR ALTER PROCEDURE spLoginConfirmation_GetForDoctors
    @EmailAddress NVARCHAR(30)
AS
BEGIN
    SELECT [Auth].[PasswordHash],
        [Auth].[PasswordSalt] 
    FROM HospitalSchema.DoctorsRegistration AS Auth 
        WHERE Auth.EmailAddress = @EmailAddress
END;
GO

GO
--Procedure for Login Confirmation
CREATE OR ALTER PROCEDURE spLoginConfirmation_GetForPatients
    @EmailAddesss NVARCHAR(30)
AS
BEGIN
    SELECT [Auth].[PasswordHash],
        [Auth].[PasswordSalt] 
    FROM HospitalSchema.PatientsRegistration AS Auth 
        WHERE Auth.EmailAddress = @EmailAddesss
END;
GO


