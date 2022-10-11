CREATE LOGIN antonio   
    WITH PASSWORD = 'antonio';  
GO  

-- Creates a database user for the login created above.  
CREATE USER antonio FOR LOGIN antonio;  
GO  

