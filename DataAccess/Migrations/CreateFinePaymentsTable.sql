CREATE TABLE Fine_Payments(
    PaymentID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    Amount DECIMAL(10, 2) NOT NULL,
    PaymentDate DATE DEFAULT GETDATE(),
    PaymentMethod VARCHAR(50),
    TransactionID VARCHAR(100),
    FOREIGN KEY(UserID) REFERENCES Users(UserID)
);
