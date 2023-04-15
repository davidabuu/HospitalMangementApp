-- Admin SQL SCRIP
CREATE DATABASE VotingDatabase
USE VotingDatabase
GO
-- CREATE SCHEMA VotingAppSchema    
-- GO
-- -- Registration Table For Admin
-- CREATE TABLE VotingAppSchema.AdminRegistration(
--     AdminId INT IDENTITY(1,1),     
--     FirstName VARCHAR(20),
--     LastName VARCHAR(20),
--     PasswordSalt VARBINARY(MAX),
--     PasswordHash VARBINARY(MAX),
--     EmailAddress NVARCHAR(20),
--     AdminRole VARCHAR(10) DEFAULT 'Admin'

-- )
GO
-- Procedure for Admin Registration and Update
CREATE OR ALTER PROCEDURE spAdminRegistrationAndUpdate
@FirstName VARCHAR(20),
@LastName VARCHAR(20),
@PasswordSalt VARBINARY(MAX),
@PasswordHash VARBINARY(MAX),
@EmailAddress NVARCHAR(20),
@AdminId INT = NULL
AS
BEGIN
IF NOT EXISTS(SELECT * FROM VotingAppSchema.AdminRegistration WHERE EmailAddress = @EmailAddress)
BEGIN
INSERT INTO VotingAppSchema.AdminRegistration(
[FirstName],
[LastName],
[EmailAddress] 
)VALUES(@FirstName, @LastName, @EmailAddress)
END
ELSE
BEGIN
UPDATE VotingAppSchema.AdminRegistration
SET FirstName = @FirstName,
    LastName = @LastName,
    EmailAddress = @EmailAddress,
    PasswordHash = @PasswordHash,
    PasswordSalt = @PasswordSalt
    WHERE AdminId = ISNULL(@AdminId, AdminId)
END
END
GO
-- Procedure for Login Confirmation for Admin
CREATE OR ALTER PROCEDURE spLoginAdminConfirmation
@EmailAddress NVARCHAR(20)
AS
BEGIN
SELECT * FROM VotingAppSchema.AdminRegistration WHERE EmailAddress = @EmailAddress
END

GO
-- Verify User
CREATE OR ALTER PROCEDURE VerifyUser
@EmailAddress NVARCHAR(20),
@UserId INT 
AS
BEGIN
IF EXISTS(SELECT * FROM VotingAppSchema.AdminRegistration WHERE EmailAddress = @EmailAddress)
BEGIN
UPDATE VotingAppSchema.UserRegistration 
SET UserVerification = 1
WHERE UserId = @UserId
END
ELSE
BEGIN
SELECT 'Only Admin can verify users'
END
END

-- User SQL SCRIPT

-- Registration Table For User
CREATE TABLE VotingAppSchema.UserRegistration(
    UserId INT IDENTITY(1,1),     
    FirstName VARCHAR(20),
    LastName VARCHAR(20),
    PasswordSalt VARBINARY(MAX),
    PasswordHash VARBINARY(MAX),
    EmailAddress NVARCHAR(20),
    UserRole VARCHAR(10) DEFAULT 'User',
    VoteForPresident BIT DEFAULT 0,
    VoteForVisePresident BIT DEFAULT 0,
    VoteForPRO BIT DEFAULT 0,
    UserVerification BIT DEFAULT 0

)
GO
-- Procedure for User Registration and Update
CREATE OR ALTER PROCEDURE spUserRegistrationAndUpdate
@FirstName VARCHAR(20),
@LastName VARCHAR(20),
@PasswordSalt VARBINARY(MAX),
@PasswordHash VARBINARY(MAX),
@EmailAddress NVARCHAR(20),
@UserId INT = NULL
AS
BEGIN
IF NOT EXISTS(SELECT * FROM VotingAppSchema.UserRegistration WHERE EmailAddress = @EmailAddress)
BEGIN
INSERT INTO VotingAppSchema.UserRegistration(
[FirstName],
[LastName],
[EmailAddress] 
)VALUES(@FirstName, @LastName, @EmailAddress)
END
ELSE
BEGIN
UPDATE VotingAppSchema.UserRegistration
SET FirstName = @FirstName,
    LastName = @LastName,
    EmailAddress = @EmailAddress,
    PasswordHash = @PasswordHash,
    PasswordSalt = @PasswordSalt
    WHERE UserId = ISNULL(@UserId, UserId)
END
END
GO
-- Procedure for Login Confirmation for User
CREATE OR ALTER PROCEDURE spLoginUserConfirmation
@EmailAddress NVARCHAR(20)
AS
BEGIN
SELECT * FROM VotingAppSchema.UserRegistration WHERE EmailAddress = @EmailAddress
END
-- Candidate Script
USE VotingDatabase
-- Registration Table For Candidate
CREATE TABLE VotingAppSchema.CandidateRegistration(
    CandidateId INT IDENTITY(1,1),     
    FirstName VARCHAR(20),
    LastName VARCHAR(20),
    ImageData VARBINARY(MAX) DEFAULT 0x00,
    CandidateRole VARCHAR(20),
    VoteCount INT DEFAULT 0
)
GO
-- Procedure for Candidate Registration and Update
CREATE OR ALTER PROCEDURE spCandidateRegistrationAndUpdate
@FirstName VARCHAR(20),
@LastName VARCHAR(20),
@EmailAddress NVARCHAR(20),
@CandidateRole VARCHAR(20) 
AS
BEGIN
IF EXISTS(SELECT * FROM VotingAppSchema.AdminRegistration WHERE EmailAddress = @EmailAddress)
BEGIN
INSERT INTO VotingAppSchema.CandidateRegistration(
[FirstName],
[LastName],
[CandidateRole] 
)VALUES(@FirstName, @LastName, @CandidateRole)
END
END

GO

-- Vote For Candidate and Check if User Is Verified
-- CREATE OR ALTER PROCEDURE spVoteCandidate
-- @EmailAddress NVARCHAR(20),
-- @CandidateId INT
-- AS
-- BEGIN
-- IF EXISTS(SELECT UserVerification FROM VotingAppSchema.UserRegistration WHERE EmailAddress = @EmailAddress)
-- BEGIN
-- UPDATE VotingAppSchema.CandidateRegistration
-- SET VoteCount = VoteCount + 1
-- WHERE CandidateId = @CandidateId
-- DECLARE @CandidateRole VARCHAR
-- SELECT @CandidateRole  = CandidateRole FROM VotingAppSchema.CandidateRegistration WHERE CandidateId = @CandidateId
-- IF NOT EXISTS( SELECT @EmailAddress FROM VotingAppSchema.CandidateRegistration WHERE @CandidateRole = 1)
-- BEGIN
-- UPDATE VotingAppSchema.UserRegistration
-- SET @CandidateRole  = 1
-- WHERE EmailAddress = @EmailAddress
-- END
-- END
-- ELSE
-- SELECT 'Sorry you can''t vote because you are not yet verified'
-- END

CREATE OR ALTER PROCEDURE spVoteCandidate
@EmailAddress NVARCHAR(20),
@CandidateId INT
AS
BEGIN
DECLARE @is_Verfied BIT
SELECT @is_Verfied  = UserVerification FROM VotingAppSchema.UserRegistration WHERE EmailAddress = @EmailAddress
IF @is_Verfied = 1
BEGIN
DECLARE @CandidateRole VARCHAR
SELECT @CandidateRole  = CandidateRole FROM VotingAppSchema.CandidateRegistration WHERE CandidateId = @CandidateId
DECLARE @CheckRoleStatus BIT
SELECT @CheckRoleStatus = @CandidateRole FROM VotingAppSchema.UserRegistration WHERE EmailAddress = @EmailAddress
IF @CheckRoleStatus = 1
BEGIN
SELECT 'You Have Already Voted for this Candidate Category'
END
ELSE
BEGIN
UPDATE VotingAppSchema.UserRegistration
SET @CandidateRole  = 1
WHERE EmailAddress = @EmailAddress
END 
END
ELSE
BEGIN
PRINT 'You Are Not Verified To Vote Yet'
END
END

GO


CREATE OR ALTER PROCEDURE sppp
@int INT
AS
BEGIN
IF(@int = 2)
BEGIN
SELECT 'true'
END
ELSE 
BEGIN
SELECT 'False'
END
END

EXEC sppp
@int = 1