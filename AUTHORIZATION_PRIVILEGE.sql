-- 1. Create the Admin Role if it doesn't exist
IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Admin')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'Admin', 'ADMIN', NEWID());
END

-- 2. Variables for IDs
DECLARE @AdminRoleId NVARCHAR(128);
DECLARE @UserId NVARCHAR(128);

-- 3. Get the IDs from the tables
SELECT @AdminRoleId = Id FROM AspNetRoles WHERE Name = 'Admin';
SELECT @UserId = Id FROM AspNetUsers WHERE Email = 'inzamamkhan71@gmail.com';

-- 4. Assign the role to the user
IF (@UserId IS NOT NULL AND @AdminRoleId IS NOT NULL)
BEGIN
    -- Only insert if the link doesn't already exist
    IF NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @AdminRoleId)
    BEGIN
        INSERT INTO AspNetUserRoles (UserId, RoleId) 
        VALUES (@UserId, @AdminRoleId);
        PRINT 'Success! User is now an Admin.';
    END
    ELSE
    BEGIN
        PRINT 'User is already an Admin.';
    END
END
ELSE
BEGIN
    PRINT 'Error: Could not find User or Role. Check if the email is correct.';
END

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'User')
BEGIN
    INSERT INTO AspNetRoles (Id, Name)
    VALUES (CAST(NEWID() AS NVARCHAR(128)), 'User');
    PRINT 'Success: "User" role created.';
END
ELSE
BEGIN
    PRINT 'Notice: "User" role already exists.';
END