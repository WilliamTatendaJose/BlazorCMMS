-- Fix NULL Values in Existing WorkOrder Records
-- This script sets empty strings for NULL values to match the C# model defaults

UPDATE WorkOrders SET RequestedBy = '' WHERE RequestedBy IS NULL;
UPDATE WorkOrders SET RequestReason = '' WHERE RequestReason IS NULL;
UPDATE WorkOrders SET ApprovedBy = '' WHERE ApprovedBy IS NULL;
UPDATE WorkOrders SET ApprovalNotes = '' WHERE ApprovalNotes IS NULL;
UPDATE WorkOrders SET RejectedBy = '' WHERE RejectedBy IS NULL;
UPDATE WorkOrders SET RejectionReason = '' WHERE RejectionReason IS NULL;
UPDATE WorkOrders SET Category = '' WHERE Category IS NULL;
UPDATE WorkOrders SET SubCategory = '' WHERE SubCategory IS NULL;
UPDATE WorkOrders SET Location = '' WHERE Location IS NULL;
UPDATE WorkOrders SET Building = '' WHERE Building IS NULL;
UPDATE WorkOrders SET Floor = '' WHERE Floor IS NULL;
UPDATE WorkOrders SET ContactPerson = '' WHERE ContactPerson IS NULL;
UPDATE WorkOrders SET ContactPhone = '' WHERE ContactPhone IS NULL;
UPDATE WorkOrders SET ContactEmail = '' WHERE ContactEmail IS NULL;
UPDATE WorkOrders SET SafetyPermitNumber = '' WHERE SafetyPermitNumber IS NULL;
UPDATE WorkOrders SET RecurrencePattern = '' WHERE RecurrencePattern IS NULL;
UPDATE WorkOrders SET LastModifiedBy = '' WHERE LastModifiedBy IS NULL;
UPDATE WorkOrders SET Department = '' WHERE Department IS NULL;
UPDATE WorkOrders SET Originator = '' WHERE Originator IS NULL;
UPDATE WorkOrders SET JobNumber = '' WHERE JobNumber IS NULL;
UPDATE WorkOrders SET AcknowledgedBy = '' WHERE AcknowledgedBy IS NULL;
UPDATE WorkOrders SET JobType = '' WHERE JobType IS NULL;
UPDATE WorkOrders SET ArtisanName = '' WHERE ArtisanName IS NULL;
UPDATE WorkOrders SET ArtisanSignature = '' WHERE ArtisanSignature IS NULL;
UPDATE WorkOrders SET HousekeepingNotes = '' WHERE HousekeepingNotes IS NULL;
UPDATE WorkOrders SET DetailsOfRequest = '' WHERE DetailsOfRequest IS NULL;
UPDATE WorkOrders SET DetailsOfWorkCarriedOut = '' WHERE DetailsOfWorkCarriedOut IS NULL;
UPDATE WorkOrders SET CorrectiveAction = '' WHERE CorrectiveAction IS NULL;
UPDATE WorkOrders SET OriginatorVerification = '' WHERE OriginatorVerification IS NULL;
UPDATE WorkOrders SET EngineeringForemanVerification = '' WHERE EngineeringForemanVerification IS NULL;
UPDATE WorkOrders SET HODVerification = '' WHERE HODVerification IS NULL;
UPDATE WorkOrders SET BreakdownType = '' WHERE BreakdownType IS NULL;
UPDATE WorkOrders SET AnyOtherDetails = '' WHERE AnyOtherDetails IS NULL;

PRINT 'NULL values updated to empty strings successfully!';

-- Verify the update
SELECT COUNT(*) AS TotalWorkOrders FROM WorkOrders;
PRINT 'Total work orders updated.';
