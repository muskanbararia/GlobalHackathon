DROP TABLE [Transaction]
GO
DROP TABLE [AcceptedRequest]
GO
DROP TABLE MatchedRequest
GO
DROP TABLE [LaundryRequest]
GO

CREATE TABLE [dbo].[LaundryRequest] (
    [RequestID]     INT  PRIMARY KEY IDENTITY,
	[UserId]       VARCHAR (20) FOREIGN KEY REFERENCES USERS(UserId),
    [WashingTime]    DATETIME NOT NULL,
    [WhitesOnly]   BIT NOT NULL CHECK([WhitesOnly] IN (0,1)),
    [DenimsOrTrousersOnly]     BIT  NOT NULL CHECK([DenimsOrTrousersOnly] IN (0,1)),
	[GarmentsOnly]    BIT  NOT NULL CHECK([GarmentsOnly] IN (0,1)),
    [UnderGarmentsOnly]    BIT  NOT NULL CHECK([UnderGarmentsOnly] IN (0,1)),
    [Weight] INT  NOT NULL CHECK([Weight] BETWEEN 1 AND 16),
    [WashingMachine]  BIT NOT NULL CHECK([WashingMachine] IN (0,1)),
	[Status] VARCHAR(50) CONSTRAINT chk_LaundryRequest_Status
		CHECK([Status] IN ('Active','Inactive')) 
);
GO


CREATE TABLE MatchedRequest
(
	MatchedRequestId INT PRIMARY KEY IDENTITY,
	OwnerId VARCHAR(20) FOREIGN KEY REFERENCES USERS(UserId),
	WasherId VARCHAR(20) FOREIGN KEY REFERENCES USERS(UserId),
	OwnerRequestId INT FOREIGN KEY REFERENCES LaundryRequest(RequestID),
	WasherRequestId INT FOREIGN KEY REFERENCES LaundryRequest(RequestID),
	[Status] VARCHAR(20) NOT NULL CONSTRAINT chk_MatchedRequest_Status
				CHECK ([Status] IN ('Inactive','Pending','Accepted','Rejected')),
	Distance NUMERIC(5,2) NOT NULL,
	CONSTRAINT uq_RequestsMatched UNIQUE(OwnerId,WasherId,OwnerRequestId,WasherRequestId)
)
GO

CREATE TABLE [dbo].[AcceptedRequest] (
    [AcceptedRequestID]     INT  PRIMARY KEY IDENTITY,
    [OwnerId] VARCHAR(20),
	[WasherId] VARCHAR(20) ,
    [OwnerRequestId] INT ,
	[WasherRequestId] INT ,
	[TimeStamp] DATETIME NOT NULL,
	[Status] VARCHAR(20) NOT NULL CHECK([Status] IN ('Complete','Incomplete','Active')),
	CONSTRAINT fk_RequestsAccepted 
		FOREIGN KEY (OwnerId,WasherId,OwnerRequestId,WasherRequestId) 
		REFERENCES MatchedRequest(OwnerId,WasherId,OwnerRequestId,WasherRequestId)
);

CREATE TABLE [Transaction]
(
	TransactionId INT PRIMARY KEY IDENTITY,
	UserId VARCHAR(20) FOREIGN KEY REFERENCES USERS(UserId),
	Laundrocash INT CONSTRAINT chk_Transaction_Laundrocash CHECK(Laundrocash>0) NOT NULL,
	TransactionType CHAR CONSTRAINT chk_TransactionType 
					CHECK (TransactionType IN ('D','C')) NOT NULL,
	[Message] VARCHAR(50),
	TransactionDateTime DATETIME NOT NULL DEFAULT GETDATE(),
	RequestId INT FOREIGN KEY REFERENCES LaundryRequest(RequestID)
)
GO

CREATE FUNCTION ufn_ViewMatchedRequests(@UserId VARCHAR(20))
RETURNS TABLE
AS
RETURN
(SELECT * FROM [MatchedRequest]  where [OwnerId]=@UserId OR [WasherId]=@UserId)