CREATE TABLE Customers (
  CustomerID INT PRIMARY KEY,
  Name NVARCHAR(50) NOT NULL,
  Address NVARCHAR(100) NOT NULL,
  Phone VARCHAR(20) NOT NULL
);

CREATE TABLE Orders (
  OrderID INT PRIMARY KEY,
  CustomerID INT FOREIGN KEY REFERENCES Customers(CustomerID),
  OrderDate DATE NOT NULL,
  TotalAmount INT NOT NULL
);