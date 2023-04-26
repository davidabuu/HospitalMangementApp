CREATE DATABASE SolarDatabase
USE SolarDatabase
GO
-- CREATE SCHEMA SolarAppSchema    
GO
-- Registration Table For Users
-- CREATE TABLE SolarAppSchema.UserRegistration(
--     UserId INT IDENTITY(1,1),     
--     FirstName VARCHAR(20),
--     LastName VARCHAR(20),
--     EmailAddress NVARCHAR(20),
--     PasswordHash VARBINARY(MAX),
--     PasswordSalt VARBINARY(MAX),
    -- PhoneNumber VARCHAR(20),
    -- Latitude DECIMAL (9,6),
    -- Longitude DECIMAL (9,6),
--     IsVerified BIT DEFAULT 0

-- )
-- Registration Table For Admin
-- CREATE TABLE SolarAppSchema.AdminRegistration(
--     AdminId INT IDENTITY(1,1),     
--     EmailAddress NVARCHAR(20),
--     AdminRole VARCHAR(10) DEFAULT 'Admin',
--     AdminCode VARCHAR(10) DEFAULT 'code',
--     UserSolarId INT
-- )

-- Table For Solar Panel Details 
-- CREATE TABLE SolarAppSchema.SolarDetails(
--     SolarId INT IDENTITY(1,1),     
--     GetDate DATE,
--     GetCurrent DECIMAL (5,2),
--     Voltage DECIMAL (5,2),
--     Radiance DECIMAL (5,2),
--     GetStatus BIT DEFAULT 0,
--     UserId INT
-- )
--  Create A Table for Users Solar Power Plant
CREATE TABLE SolarAppSchema.UserSolarPowerPlant(
    UserId INT,
    Capacity DECIMAL(5,2),
    ShortCircuitVoltage DECIMAL(5,2),
    InverterCapactity DECIMAL(5,2)
)

CREATE TABLE SolarAppSchema.UserMonitoringDevice(
    UserId INT,
    MacAddress VARCHAR(20),
    IpAddress VARCHAR(20),
    Port SMALLINT
)

GO
-- Sql Script to Verify a User
CREATE OR ALTER PROCEDURE spUserVerification
@UserId INT,
@EmailAddress NVARCHAR(20)
AS
BEGIN
IF EXISTS(SELECT * FROM SolarAppSchema.AdminRegistration WHERE EmailAddress = @EmailAddress)
BEGIN
UPDATE SolarAppSchema.UserRegistration
SET IsVerified = 1
WHERE UserId = @UserId
END
END
GO
CREATE OR ALTER PROCEDURE spSolarDetailsPerUser
    @UserId INT,
    @GetDate DATE,
    @GetCurrent DECIMAL (5,2),
    @Voltage DECIMAL (5,2),
    @Radiance DECIMAL (5,2),
    @GetStatus BIT
AS
BEGIN
    DECLARE @is_Verified BIT
    SELECT @is_Verified = IsVerified FROM SolarAppSchema.UserRegistration WHERE UserId = @UserId

    IF EXISTS(SELECT * FROM SolarAppSchema.UserRegistration WHERE UserId = @UserId AND IsVerified = 1 )
    BEGIN
        INSERT INTO SolarAppSchema.SolarDetails(
            [UserId],
            [GetDate],
            [GetCurrent],
            [Voltage],
            [Radiance],
            [GetStatus]
        )VALUES(@UserId, @GetDate, @GetCurrent, @Voltage, @Radiance, @GetStatus) 

        SELECT 'Data successfully inserted' AS [Message]
    END
    ELSE
    BEGIN
        IF EXISTS (SELECT * FROM SolarAppSchema.UserRegistration WHERE UserId = @UserId AND IsVerified = 0)
        BEGIN
            SELECT 'User is not verified yet' AS [Message]
        END
        ELSE 
        BEGIN
            SELECT 'User is not found' AS [Message]
        END
    END
END


GO
-- Get User Details Complete
CREATE OR ALTER PROCEDURE spGetUserDetails
@UserId INT
AS
BEGIN
IF EXISTS(SELECT * FROM SolarAppSchema.UserRegistration WHERE UserId = @UserId AND IsVerified = 1)
BEGIN
SELECT * FROM SolarAppSchema.UserRegistration AS UserInfo
LEFT JOIN SolarAppSchema.SolarDetails AS SolarDetails
ON UserInfo.UserId = SolarDetails.UserId
END
END
GO

EXEC spGetUserDetails
@UserId  = 1
EXEC spGetUserDetails
          @UserId = 1
GO
-- Procedure for User Registration
CREATE OR ALTER PROCEDURE spUserRegistration
@FirstName VARCHAR(20),
@LastName VARCHAR(20),
@PasswordSalt VARBINARY(MAX),
@PasswordHash VARBINARY(MAX),
@EmailAddress NVARCHAR(20),
@PhoneNumber VARCHAR(20),
@Latitude DECIMAL (9,6),
@Longitude DECIMAL (9,6)
AS
BEGIN
IF NOT EXISTS(SELECT * FROM SolarAppSchema.UserRegistration WHERE EmailAddress = @EmailAddress)
BEGIN
INSERT INTO SolarAppSchema.UserRegistration(
[FirstName],
[LastName],
[PasswordHash],
[PasswordSalt],
[EmailAddress],
[Latitude],
[Longitude],
[PhoneNumber]
)VALUES(@FirstName, @LastName,@PasswordHash,@PasswordSalt, @EmailAddress, @Latitude, @Longitude, @PhoneNumber)
END
END


-- Admin Verify Users
GO
CREATE OR ALTER PROCEDURE spAdminVerifyUser
@EmailAddress NVARCHAR(20),
@UserId INT 
AS
BEGIN
IF EXISTS(SELECT * FROM SolarAppSchema.AdminRegistration WHERE EmailAddress = @EmailAddress)
BEGIN
UPDATE SolarAppSchema.UserRegistration 
SET IsVerified = 1
WHERE UserId = @UserId
END
END



USE SolarDatabase


GO
-- Create Procedure to add A User Solar Power Plant Data
CREATE OR ALTER PROCEDURE spUserPowerPlantData
@UserId INT,
@EmailAddress NVARCHAR(20),
@Capacity DECIMAL(5,2),
@ShortCircuitVoltage DECIMAL(5,2),
@InverterCapactity DECIMAL(5,2)
AS
BEGIN
IF EXISTS(SELECT * FROM SolarAppSchema.AdminRegistration WHERE EmailAddress = @EmailAddress)
BEGIN
IF EXISTS(SELECT * FROM SolarAppSchema.UserRegistration WHERE UserId = @UserId AND IsVerified = 1)
BEGIN
INSERT INTO SolarAppSchema.UserSolarPowerPlant(
    [UserId],
    [Capacity],
    [ShortCircuitVoltage],
    [InverterCapactity]
)VALUES(@UserId, @Capacity, @ShortCircuitVoltage, @InverterCapactity)
END
END
ELSE 
SELECT 'You are not allowed to call this function'
END
GO
--Create Procdure for User Monitoring Device
CREATE OR ALTER PROCEDURE spUserMonitoringDevice
@UserId INT,
@EmailAddress NVARCHAR(20),
@MacAddress VARCHAR(20),
@IpAddress VARCHAR(20),
@Port SMALLINT
AS
BEGIN
IF EXISTS(SELECT * FROM SolarAppSchema.AdminRegistration WHERE EmailAddress = @EmailAddress)
BEGIN
IF EXISTS(SELECT * FROM SolarAppSchema.UserRegistration WHERE UserId = @UserId AND IsVerified = 1)
BEGIN
INSERT INTO SolarAppSchema.UserMonitoringDevice(
    [UserId],
    [MacAddress],
    [IpAddress],
    [Port]
)VALUES(@UserId, @MacAddress, @IpAddress, @Port)
END
END
ELSE
SELECT 'Not Allowed'
END