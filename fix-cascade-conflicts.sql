-- Fix cascade conflicts in Feedbacks table
-- Step 1: Drop the problematic CASCADE constraint that was created
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Feedbacks_Customers_CustomerId')
    ALTER TABLE [Feedbacks] DROP CONSTRAINT [FK_Feedbacks_Customers_CustomerId];

-- Step 2: Recreate with RESTRICT to avoid cascade conflicts
ALTER TABLE [Feedbacks] ADD CONSTRAINT [FK_Feedbacks_Customers_CustomerId] 
    FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([Id]) ON DELETE NO ACTION;

-- Step 3: Create the missing Doctor foreign key with RESTRICT
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Feedbacks_Doctors_DoctorId')
    ALTER TABLE [Feedbacks] ADD CONSTRAINT [FK_Feedbacks_Doctors_DoctorId] 
        FOREIGN KEY ([DoctorId]) REFERENCES [Doctors] ([Id]) ON DELETE NO ACTION;

PRINT 'Cascade conflicts fixed successfully!'; 